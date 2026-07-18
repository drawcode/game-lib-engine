using System.Collections.Generic;

using UnityEngine;

namespace Engine.UI {

    // Backend registration + per-object dispatch.
    //
    // This is the ONE place the UI seam deliberately diverges from the tween precedent.
    // TweenUtil has a single lazy `backend` property plus SetBackend, because exactly one
    // tween backend is ever live. UI needs TWO live at once for all of Phases 2-3 (NGUI
    // screens and UI Toolkit screens coexisting in the same frame), so registration is a
    // list and dispatch is an ordered Handles() probe.
    //
    // Backends are disjoint by native type (GameObject vs VisualElement), so probe order is
    // not load-bearing today — but it is defined (first registered wins) so that it stays
    // predictable if a third backend ever overlaps.
    public class UIPlatform {

        private static List<IUIBackend> _backends = null;
        private static IUIBackend _viewBackend = null;

        // Test seam. With this false and Reset() called, no backend claims anything, For()
        // returns null, and UIUtil's GameObject overloads fall back to their original bodies.
        // That is what lets UIBackendTests run the SAME GameObject down both the legacy path
        // and the backend path and assert they agree — the Phase 2 behavior-preservation gate.
        // Nothing in production touches this.
        public static bool autoRegisterDefaults = true;

        private static List<IUIBackend> backends {
            get {

                if (_backends == null) {

                    _backends = new List<IUIBackend>();

                    if (autoRegisterDefaults) {
                        RegisterDefaults();
                    }
                }

                return _backends;
            }
        }

        // Lazy self-registration, mirroring TweenUtil's lazy EasingTweenBackend default:
        // no bootstrap call site to forget, and no behavior change for a project that
        // never touches UIPlatform directly. Defines gate which backends exist at all.
        private static void RegisterDefaults() {

            // Load the token source the first time the platform spins up: tokens.json seeds
            // TweenPresets (durations/eases), making panel timing data-driven. NOTHING else called
            // Load in production (found 3B — only tests did), which went unnoticed because
            // TweenPresets' built-in table matches the token defaults; the chrome-show/hide
            // variance is the first preset that only exists through the tokens. Touching
            // `current` after Load seeds the built-in defaults even when no tokens.json ships.
            Engine.UI.Bitty.UITokens.Load();
            Engine.UI.Bitty.UITokens tokens = Engine.UI.Bitty.UITokens.current;

            // NGUIBackend is the GameObject backend: it carries BOTH the NGUI and the uGUI
            // probes, because UIUtil's GameObject resolver bodies always probed both in the
            // same method. It is the extraction of those bodies, unchanged.
            Register(NGUIBackend.Instance);

#if USE_UI_TOOLKIT
            Register(UIToolkitBackend.Instance);

            // The Toolkit backend owns view loading whenever it is compiled in.
            //
            // NGUIBackend.LoadView is a deliberate no-op: NGUI screens are Resources prefabs
            // instantiated by the app's content pipeline (AppContentAssets.LoadAssetUI), which
            // lives above the engine and never goes through IUIBackend. So leaving NGUIBackend as
            // the view backend meant LoadView could never return anything — the pilot silently
            // stayed on NGUI with no error, which is exactly the kind of quiet failure this seam
            // exists to prevent.
            //
            // This does NOT force panels onto UI Toolkit. A panel becomes a toolkit panel only if
            // a UXML view actually exists for its key; otherwise LoadView returns none and the
            // panel stays on NGUI. THAT is the per-panel switch — the presence of the view, not a
            // global flag.
            _viewBackend = UIToolkitBackend.Instance;
#endif
        }

        public static void Register(IUIBackend backend) {

            if (backend == null) {
                return;
            }

            if (backends.Contains(backend)) {
                return;
            }

            backends.Add(backend);
        }

        public static IUIBackend For(object native) {

            if (native == null) {
                return null;
            }

            List<IUIBackend> list = backends;

            for (int i = 0; i < list.Count; i++) {
                if (list[i].Handles(native)) {
                    return list[i];
                }
            }

            return null;
        }

        public static IUIBackend For(UIRef r) {

            if (r == null) {
                return null;
            }

            return For(r.native);
        }

        // The coexistence click-through guard's entry point. NGUI's UICamera.Raycast asks this
        // before its own raycast: if a UI Toolkit panel has actual (pickable) content under the
        // pointer, NGUI must not also hit a collider behind it. Backend-agnostic — NGUIBackend
        // always answers false, only the Toolkit backend does a real panel.Pick.
        public static bool IsPointerOverUI(Vector2 screenPos) {

            List<IUIBackend> list = backends;

            for (int i = 0; i < list.Count; i++) {
                if (list[i].IsPointerOver(screenPos)) {
                    return true;
                }
            }

            return false;
        }

        // Which backend builds a NEW screen. Separate from For() because LoadView("panel-x")
        // has no native object to dispatch on yet. This is the switch a per-panel migration
        // actually flips in Phase 3: the panel's view key starts resolving through
        // UIToolkitBackend instead of NGUIBackend, and nothing else about the panel changes.
        public static IUIBackend viewBackend {
            get {

                // Touch `backends` first: RegisterDefaults is what assigns _viewBackend, and if
                // this getter ran before it, _viewBackend would latch onto NGUIBackend — whose
                // LoadView is a no-op — and no panel could ever load a UXML view.
                List<IUIBackend> registered = backends;

                if (_viewBackend == null && registered.Count > 0) {
                    _viewBackend = NGUIBackend.Instance;
                }

                return _viewBackend;
            }
            set {
                _viewBackend = value;
            }
        }

        // LAYERS
        //
        // The ordered containers a view is hosted in. Named here — not on a backend type — so the
        // panel system can ask for a layer without naming UIToolkitHost (or any other backend
        // host). The host registers the resolver; everything above the provider layer sees only
        // strings in and UIRefs out.

        // KILL SWITCH.
        //
        // Set false and every panel falls back to NGUI on its next show — no view is loaded, no
        // UIDocument work happens, nothing above the provider layer changes. NGUI remains the
        // shipping path for the whole of Phase 3 by design; this makes that reversible in one
        // line rather than by reverting per-panel code.
        //
        // Exists because UI Toolkit is currently costing us memory and scene-transition time
        // that NGUI was not (2026-07-14). Until that is understood and fixed, the toolkit path
        // must be trivially switchable off.
        public static bool toolkitViewsEnabled = true;

        // Test seam: drop all registrations so a test can install a fake.
        public static void Reset() {
            _backends = null;
            _viewBackend = null;
        }
    }
}
