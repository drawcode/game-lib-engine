using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UIElements;

using Engine.Animation;
using Engine.Utility;

namespace Engine.UI {

    // The UI Toolkit backend. Together with VisualElementTweenTarget, this is one of only two
    // files in the engine allowed to reference UnityEngine.UIElements. The Phase 2 gate greps
    // for leaks outside these two — if a UIElements type ever appears above the provider layer,
    // the seam has failed and the platform is no longer swappable.
    public class UIToolkitBackend : IUIBackend {

        private static UIToolkitBackend _instance = null;

        public static UIToolkitBackend Instance {
            get {

                if (_instance == null) {
                    _instance = new UIToolkitBackend();
                }

                return _instance;
            }
        }

        // Resources path prefix for LoadView. Set by the app if its UXML lives elsewhere.
        public static string viewPath = "ui/views/";

        private readonly List<VisualElement> roots = new List<VisualElement>();
        private int _currentPointerId = -1;

        public bool Handles(object native) {
            return native is VisualElement;
        }

        private static VisualElement El(UIRef r) {

            if (r == null || !r.alive) {
                return null;
            }

            return r.native as VisualElement;
        }

        // ROOTS / BRIDGE
        // Minimal click + pointer wiring so the backend has a real pointer source. Chunk 2.8
        // builds the full UIToolkitClickBridge on top of this (button audio, data payloads,
        // the UICamera.Raycast coexistence guard).

        public void RegisterRoot(VisualElement root) {

            if (root == null || roots.Contains(root)) {
                return;
            }

            roots.Add(root);

            // Bubbling, not trickle-down: the click must reach the element that was actually
            // hit, so its name is what gets broadcast.
            root.RegisterCallback<ClickEvent>(OnClick);
            root.RegisterCallback<PointerDownEvent>(OnPointerDown);
        }

        public void UnregisterRoot(VisualElement root) {

            if (root == null || !roots.Contains(root)) {
                return;
            }

            root.UnregisterCallback<ClickEvent>(OnClick);
            root.UnregisterCallback<PointerDownEvent>(OnPointerDown);

            roots.Remove(root);
        }

        private void OnClick(ClickEvent evt) {

            VisualElement el = evt.target as VisualElement;

            if (el == null || string.IsNullOrEmpty(el.name)) {
                return;
            }

            // The whole reason the pilot is cheap: the click path is already name-keyed, so
            // broadcasting the element name reaches every existing OnButtonClickEventHandler
            // (string) unmodified. No new event model, no per-panel rewiring.
            UIEvents.BroadcastClick(el.name);
        }

        private void OnPointerDown(PointerDownEvent evt) {
            _currentPointerId = evt.pointerId;
        }

        // RESOLUTION

        // Q() is already a descendant query, which is the UI Toolkit idiom — there is no
        // meaningful direct-child-only lookup the way Transform.Find is. Resolve and
        // ResolveDeep therefore coincide here; they differ only on the GameObject backend.
        public UIRef Resolve(UIRef root, string name) {

            VisualElement el = El(root);

            if (el == null || string.IsNullOrEmpty(name)) {
                return UIRef.none;
            }

            VisualElement found = el.Q(name);

            if (found == null) {
                return UIRef.none;
            }

            return UIRef.Of(found, found.name);
        }

        public UIRef ResolveDeep(UIRef root, string name) {
            return Resolve(root, name);
        }

        public List<UIRef> ResolveLike(UIRef root, string code) {

            List<UIRef> results = new List<UIRef>();

            VisualElement el = El(root);

            if (el == null || string.IsNullOrEmpty(code)) {
                return results;
            }

            el.Query<VisualElement>().ForEach(child => {

                if (!string.IsNullOrEmpty(child.name) && child.name.Contains(code)) {
                    results.Add(UIRef.Of(child, child.name));
                }
            });

            return results;
        }

        // LABELS

        public void SetLabelValue(UIRef r, string val) {

            TextElement text = El(r) as TextElement;

            if (text == null || text.text == val) {
                return;
            }

            text.text = val;
        }

        public string GetLabelValue(UIRef r) {

            TextElement text = El(r) as TextElement;

            if (text == null) {
                // null, not "" — matches the GameObject backend, whose callers distinguish them.
                return null;
            }

            return text.text;
        }

        public void SetLabelColor(UIRef r, Color c) {
            ColorTween(El(r), c);
        }

        // INPUTS

        public void SetInputValue(UIRef r, string val) {

            TextField field = El(r) as TextField;

            if (field == null) {
                return;
            }

            field.value = val;
        }

        public string GetInputValue(UIRef r) {

            TextField field = El(r) as TextField;

            if (field == null) {
                return null;
            }

            return field.value;
        }

        // SLIDERS

        public void SetSliderValue(UIRef r, float val) {

            VisualElement el = El(r);

            if (el == null) {
                return;
            }

            Slider slider = el as Slider;

            if (slider != null) {
                slider.value = val;
                return;
            }

            Scroller scroller = el as Scroller;

            if (scroller != null) {
                scroller.value = val;
                return;
            }

            // Mirrors the GameObject backend's Slider → Scrollbar → Image-as-fill fallback.
            SetImageFillValue(r, val);
        }

        public float GetSliderValue(UIRef r) {

            VisualElement el = El(r);

            if (el == null) {
                return 0f;
            }

            Slider slider = el as Slider;

            if (slider != null) {
                return slider.value;
            }

            Scroller scroller = el as Scroller;

            if (scroller != null) {
                return scroller.value;
            }

            return GetImageFillValue(r);
        }

        // TOGGLES

        public void SetToggleValue(UIRef r, bool val) {

            Toggle toggle = El(r) as Toggle;

            if (toggle == null) {
                return;
            }

            toggle.value = val;
        }

        public bool GetToggleValue(UIRef r) {

            Toggle toggle = El(r) as Toggle;

            if (toggle == null) {
                return false;
            }

            return toggle.value;
        }

        // IMAGES

        // UI Toolkit has no fillAmount. Horizontal width-percent is the fill model for v1 —
        // it covers progress bars and meters, which is every current consumer (SetImageFillValue
        // has zero call sites today; fills arrive through SetSliderValue's fallback).
        // Radial/vertical gauges land with the HUD in 3H, as a dedicated fill element.
        public void SetImageFillValue(UIRef r, float val) {

            VisualElement el = El(r);

            if (el == null) {
                return;
            }

            el.style.width = Length.Percent(Mathf.Clamp01(val) * 100f);
        }

        public float GetImageFillValue(UIRef r) {

            VisualElement el = El(r);

            if (el == null) {
                return 0f;
            }

            if (el.style.width.keyword != StyleKeyword.Undefined) {
                return 0f;
            }

            Length w = el.style.width.value;

            if (w.unit != LengthUnit.Percent) {
                return 0f;
            }

            return w.value / 100f;
        }

        public void SetSpriteColor(UIRef r, Color c) {
            ColorTween(El(r), c);
        }

        // Colors animate over .5s to match the GameObject backend, whose SetSpriteColor /
        // SetLabelColor go through TweenUtil.ColorToObject(go, c, .5f, 0f). Reuses the Phase 1
        // VisualElementTweenTarget, which already picks style.color for TextElements and
        // unityBackgroundImageTintColor for everything else — so this does not duplicate that
        // decision. Exact easing parity is confirmed at the pilot (2.10).
        private static void ColorTween(VisualElement el, Color c) {

            if (el == null) {
                return;
            }

            ITweenTarget target = TweenUtil.ResolveTarget(el);

            if (target == null) {
                return;
            }

            TweenMeta meta = new TweenMeta();
            meta.time = .5f;
            meta.delay = 0f;
            meta.stopCurrent = true;

            TweenUtil.backend.ColorTo(target, c, meta);
        }

        // BUTTONS

        public bool IsButton(UIRef r) {
            return El(r) is Button;
        }

        public void SetButtonColor(UIRef r, Color c) {
            ColorTween(El(r), c);
        }

        public void SetButtonHandlerClick(UIRef r, Action onClick) {

            Button button = El(r) as Button;

            if (button == null || onClick == null) {
                return;
            }

            button.clicked += onClick;
        }

        // VISIBILITY

        // display, not visibility: display removes the element from layout, which is what
        // SetActive does on the GameObject side. Tweens must never call these (gate learning #1).
        public void Show(UIRef r) {

            VisualElement el = El(r);

            if (el == null) {
                return;
            }

            el.style.display = DisplayStyle.Flex;
        }

        public void Hide(UIRef r) {

            VisualElement el = El(r);

            if (el == null) {
                return;
            }

            el.style.display = DisplayStyle.None;
        }

        public bool IsVisible(UIRef r) {

            VisualElement el = El(r);

            if (el == null) {
                return false;
            }

            if (el.style.display.keyword == StyleKeyword.Undefined) {
                return el.style.display.value != DisplayStyle.None;
            }

            if (el.panel != null) {
                return el.resolvedStyle.display != DisplayStyle.None;
            }

            return true;
        }

        // LAYOUT

        // UIGrid.Reposition() exists because NGUI had no layout engine. UI Toolkit does — a
        // grid is flex-wrap in USS and reflows itself. This is intentionally a no-op, not a
        // gap: the 2 GridReposition call sites become free once their panels are UXML.
        public void GridReposition(UIRef r) {
        }

        // VIEW LIFECYCLE
        //
        // Each migrated panel gets its OWN PanelRenderer (Unity 6.5's replacement for the
        // deprecated UIDocument). A view is a GameObject carrying a PanelRenderer with the view's
        // VisualTreeAsset assigned. This is what makes load/unload real: destroying that
        // GameObject drops the UXML — the persistent one-UIDocument-holds-everything model never
        // unloaded anything.
        //
        // PanelSettings is supplied by the scene host (UIToolkitHost) at startup; all views share
        // it (one runtime panel, many renderer subtrees) so we don't pay a render target per view.
        //
        // Loading is DEFERRED: PanelRenderer builds its tree on a later panel update and hands the
        // root to its reload callback. So LoadView takes a continuation and fires it from there.

        public static PanelSettings panelSettings;

        // view-subtree root -> the PanelRenderer GameObject that owns it (for DestroyView).
        private readonly Dictionary<VisualElement, GameObject> viewHosts
            = new Dictionary<VisualElement, GameObject>();

        private int _nextSortingOrder = 100;

        public void LoadView(string viewKey, Action<UIRef> onReady) {

            if (onReady == null) {
                return;
            }

            if (string.IsNullOrEmpty(viewKey)) {
                onReady(UIRef.none);
                return;
            }

            VisualTreeAsset asset = Resources.Load<VisualTreeAsset>(viewPath + viewKey);

            if (asset == null) {
                LogUtil.LogWarning("UIToolkitBackend.LoadView: no VisualTreeAsset at "
                    + viewPath + viewKey);
                onReady(UIRef.none);
                return;
            }

            if (panelSettings == null) {
                LogUtil.LogWarning("UIToolkitBackend.LoadView: no PanelSettings registered "
                    + "(is UIToolkitHost in the scene?); cannot host '" + viewKey + "'");
                onReady(UIRef.none);
                return;
            }

            GameObject go = new GameObject("uitk-" + viewKey);
            PanelRenderer renderer = go.AddComponent<PanelRenderer>();
            renderer.panelSettings = panelSettings;
            renderer.sortingOrder = _nextSortingOrder++;

            bool[] delivered = { false };

            PanelRenderer.UIReloadCallback cb = null;
            cb = (r, root) => {

                if (delivered[0] || root == null) {
                    return;
                }

                delivered[0] = true;

                // The callback's `root` may be the shared panel root when views share a
                // PanelSettings. Bind and show/hide against THIS view's own subtree, found by the
                // UXML root's name, so one panel's ops never touch another's. Fall back to root
                // only if the named element isn't found (shouldn't happen).
                VisualElement viewRoot = root.Q(viewKey) ?? root;

                RegisterRoot(viewRoot);
                viewHosts[viewRoot] = go;

                onReady(UIRef.Of(viewRoot, viewKey));
            };

            renderer.RegisterUIReloadCallback(cb);

            // Assign last: this is what marks the renderer dirty and schedules the deferred load.
            renderer.visualTreeAsset = asset;
        }

        // Frees the UXML: destroying the PanelRenderer GameObject is the whole point of the
        // per-panel model. Unregisters the click bridge for that view's root first.
        public void DestroyView(UIRef view) {

            VisualElement el = El(view);

            if (el == null) {
                return;
            }

            UnregisterRoot(el);

            GameObject go = null;

            if (viewHosts.TryGetValue(el, out go)) {
                viewHosts.Remove(el);

                if (go != null) {
                    UnityEngine.Object.Destroy(go);
                }
            }
        }

        // POINTER / EVENT SOURCE

        public int currentPointerId {
            get {
                return _currentPointerId;
            }
        }

        // The coexistence guard: NGUI's UICamera.Raycast early-outs on this so a tap that lands
        // on a migrated Toolkit panel never also reaches an NGUI collider behind it. Picks that
        // land on a root itself are ignored — the transparent root must let gameplay taps fall
        // through, or the whole screen becomes a click blocker.
        public bool IsPointerOver(Vector2 screenPos) {

            for (int i = 0; i < roots.Count; i++) {

                VisualElement root = roots[i];

                if (root == null || root.panel == null) {
                    continue;
                }

                Vector2 panelPos = RuntimePanelUtils.ScreenToPanel(
                    root.panel, new Vector2(screenPos.x, Screen.height - screenPos.y));

                VisualElement picked = root.panel.Pick(panelPos);

                if (picked != null && picked != root) {
                    return true;
                }
            }

            return false;
        }
    }
}
