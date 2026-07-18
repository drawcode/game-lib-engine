using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UIElements;

using Engine.Animation;
using Engine.UI.Bitty;
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

            // The VALUE bridge (3A). Same idea as the click bridge one line up: one bubbling
            // callback per root, and the element's own name is the broadcast key. A migrated
            // slider/toggle/textfield reaches the existing name-keyed OnSliderChange /
            // OnCheckboxChange / OnProfileInputChanged handlers with no per-widget MonoBehaviour
            // (NGUI needed a SliderEvents/CheckboxEvents component on each; UI Toolkit does not).
            root.RegisterCallback<ChangeEvent<float>>(OnSliderChanged);
            root.RegisterCallback<ChangeEvent<bool>>(OnToggleChanged);
            root.RegisterCallback<ChangeEvent<string>>(OnInputChanged);
        }

        public void UnregisterRoot(VisualElement root) {

            if (root == null || !roots.Contains(root)) {
                return;
            }

            root.UnregisterCallback<ClickEvent>(OnClick);
            root.UnregisterCallback<PointerDownEvent>(OnPointerDown);

            root.UnregisterCallback<ChangeEvent<float>>(OnSliderChanged);
            root.UnregisterCallback<ChangeEvent<bool>>(OnToggleChanged);
            root.UnregisterCallback<ChangeEvent<string>>(OnInputChanged);

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

        // VALUE bridge handlers. Guard on the element name: a ScrollView's internal scroller is a
        // Slider too and fires ChangeEvent<float>, but its name is "unity-..."; real widgets carry
        // the prefab GameObject name (e.g. "SliderAudioMusicVolume"). Skipping the "unity-" prefix
        // keeps scroll gestures off the value bus.

        private static bool IsBridgedControl(VisualElement el) {

            return el != null
                && !string.IsNullOrEmpty(el.name)
                && !el.name.StartsWith("unity-");
        }

        private void OnSliderChanged(ChangeEvent<float> evt) {

            VisualElement el = evt.target as VisualElement;

            if (!IsBridgedControl(el)) {
                return;
            }

            UIEvents.BroadcastSliderChange(el.name, evt.newValue);
        }

        private void OnToggleChanged(ChangeEvent<bool> evt) {

            VisualElement el = evt.target as VisualElement;

            if (!IsBridgedControl(el)) {
                return;
            }

            UIEvents.BroadcastCheckboxChange(el.name, evt.newValue);
        }

        private void OnInputChanged(ChangeEvent<string> evt) {

            VisualElement el = evt.target as VisualElement;

            if (!IsBridgedControl(el)) {
                return;
            }

            UIEvents.BroadcastInputChange(el.name, evt.newValue);
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

        public void SetImageTexture(UIRef r, Texture texture) {

            VisualElement el = El(r);

            if (el == null) {
                return;
            }

            if (texture is RenderTexture rt) {
                el.style.backgroundImage = Background.FromRenderTexture(rt);
            }
            else if (texture is Texture2D t2) {
                el.style.backgroundImage = Background.FromTexture2D(t2);
            }
            else {
                return;
            }

            // "Display exactly this texture": clear any styled tint (a themed gold tint would
            // recolor an RT's live pixels) and stretch it over the whole element — overriding any
            // placeholder background-size the class carried (e.g. the coin's shrunken gold circle).
            el.style.unityBackgroundImageTintColor = Color.white;
            el.style.backgroundSize =
                new BackgroundSize(Length.Percent(100), Length.Percent(100));
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
            LoadView(viewKey, UILayers.auto, onReady);
        }

        public void LoadView(string viewKey, int sortingOrder, Action<UIRef> onReady) {

            if (onReady == null) {
                return;
            }

            if (string.IsNullOrEmpty(viewKey)) {
                onReady(UIRef.none);
                return;
            }

            // A view resolves one of two ways under the same key:
            //   * a UXML VisualTreeAsset  -> app-IP screens authored native (credits, menus)
            //   * a bitty JSON TextAsset  -> data-driven screens built at runtime (settings rows)
            // UXML wins if both exist. Resources.Load is type-disambiguated, so a .uxml and a .json
            // at the same path never collide. This is the D6 template/bitty split at the load seam.
            VisualTreeAsset asset = Resources.Load<VisualTreeAsset>(viewPath + viewKey);
            BittyNode bitty = null;

            if (asset == null) {

                TextAsset json = Resources.Load<TextAsset>(viewPath + viewKey);

                if (json != null) {

                    bitty = BittyParser.Parse(json.text);

                    if (bitty != null) {
                        // Bitty builds its own VisualElements; PanelRenderer still needs SOME
                        // VisualTreeAsset to spin up a panel and fire the reload callback, so a
                        // shared empty host stub stands in and we graft the built tree under it.
                        asset = BittyHostAsset();
                    }
                }
            }

            if (asset == null) {
                LogUtil.LogWarning("UIToolkitBackend.LoadView: no UXML or bitty view at "
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
            // Explicit band (UILayers.chrome/overlay) when the panel declares one; otherwise the
            // original auto-assign, which is load order within the panel band.
            renderer.sortingOrder =
                sortingOrder == UILayers.auto ? _nextSortingOrder++ : sortingOrder;

            bool[] delivered = { false };
            BittyNode bittyToBuild = bitty;

            PanelRenderer.UIReloadCallback cb = null;
            cb = (r, root) => {

                if (delivered[0] || root == null) {
                    return;
                }

                delivered[0] = true;

                VisualElement viewRoot;

                if (bittyToBuild != null) {

                    // Build the data-driven tree and attach it straight to `root`. Its root node is
                    // named viewKey (unique), so this stays isolated even when several bitty views
                    // share one PanelSettings and `root` is the shared panel root — a generic
                    // host-element lookup would ambiguously match another open view's stub. The
                    // stub asset exists only to spin the PanelRenderer up; nothing is read from it.
                    VisualElement built = BittyToolkitBuilder.Build(bittyToBuild);

                    if (built != null) {
                        root.Add(built);
                    }

                    viewRoot = built ?? root;
                }
                else {
                    // The callback's `root` may be the shared panel root when views share a
                    // PanelSettings. Bind and show/hide against THIS view's own subtree, found by
                    // the UXML root's name, so one panel's ops never touch another's. Fall back to
                    // root only if the named element isn't found (shouldn't happen).
                    viewRoot = root.Q(viewKey) ?? root;
                }

                RegisterRoot(viewRoot);
                viewHosts[viewRoot] = go;

                ConfigureScrollViews(viewRoot);
                ConfigurePicking(viewRoot);

                onReady(UIRef.Of(viewRoot, viewKey));
            };

            renderer.RegisterUIReloadCallback(cb);

            // Assign last: this is what marks the renderer dirty and schedules the deferred load.
            renderer.visualTreeAsset = asset;
        }

        // The shared empty stub every bitty view is hosted in (see LoadView). One asset, loaded
        // once: it produces a single #bitty-host element the built tree is grafted under. Missing
        // stub is a content bug — bitty views cannot render without it — so it warns loudly.
        private static VisualTreeAsset _bittyHost;

        private static VisualTreeAsset BittyHostAsset() {

            if (_bittyHost == null) {
                _bittyHost = Resources.Load<VisualTreeAsset>("ui/bitty-host");

                if (_bittyHost == null) {
                    LogUtil.LogWarning("UIToolkitBackend: no Resources/ui/bitty-host.uxml — "
                        + "bitty views cannot be hosted");
                }
            }

            return _bittyHost;
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

        // SCROLLERS
        //
        // The reusable scroll pattern (credits, worlds, levels, statistics, achievements...).
        // Two things NGUI's UIDraggablePanel gave for free that UI Toolkit's ScrollView does not:
        //  1. drag the CONTENT AREA to scroll, with a MOUSE as well as touch — UI Toolkit only
        //     drag-scrolls on touch, and on desktop expects mouse wheel / scrollbar drag;
        //  2. a single, thin scrollbar (the thin styling is in common.uss; visibility is set here).
        // Both are wired for every .ngui-scrollview when a view loads, so no panel repeats it.

        private static void ConfigureScrollViews(VisualElement root) {

            if (root == null) {
                return;
            }

            root.Query<ScrollView>(className: "ngui-scrollview").ForEach(WireScrollView);
        }

        // Layout wrappers must not block pointer input. The converter emits full-bleed
        // (inset:0) container elements; if they stayed pickable, panel.Pick would return one of
        // them everywhere the panel's root spans — including the transparent margin over the
        // always-on NGUI header — so the click-through guard would block the NGUI back button.
        // Making containers ignore-picking means only real widgets (backer, labels, buttons)
        // register as "over UI", and taps on the panel's transparent areas fall through to NGUI.
        private static void ConfigurePicking(VisualElement root) {

            if (root == null) {
                return;
            }

            if (root.ClassListContains("ngui-container") || root.ClassListContains("ngui-root")) {
                root.pickingMode = PickingMode.Ignore;
            }

            root.Query<VisualElement>(className: "ngui-container").ForEach(
                e => e.pickingMode = PickingMode.Ignore);
        }

        private static void WireScrollView(ScrollView sv) {

            sv.mode = ScrollViewMode.Vertical;
            sv.touchScrollBehavior = ScrollView.TouchScrollBehavior.Elastic;
            // Auto shows the bar only when content overflows; ScrollDrag then fades it in on
            // scroll and out when idle (like a mobile scroller).
            sv.verticalScrollerVisibility = ScrollerVisibility.Auto;
            sv.horizontalScrollerVisibility = ScrollerVisibility.Hidden;

            new ScrollDrag(sv);
        }

        // Pointer-drag-to-scroll for both mouse and touch, with momentum and an auto-hide bar.
        // NGUI's UIDraggablePanel had all three; UI Toolkit's ScrollView has none for mouse.
        private class ScrollDrag {

            private const float threshold = 6f;       // px of drag before we take over (clicks survive)
            private const float tickDt = 0.016f;      // scheduler tick ~60Hz
            private const float decayPerTick = 0.93f; // momentum falloff per tick (~0.1s half-life)
            private const float minVelocity = 30f;    // px/s: below this the flick stops
            private const float idleHideSeconds = 0.8f;

            private readonly ScrollView sv;
            private readonly Scroller scroller;

            private bool active;
            private bool capturing;
            private float startY;
            private float startOffsetY;
            private int pointerId;

            private float velocity;       // px/s of scrollOffset (the flick)
            private float lastPointerY;
            private float lastMoveTime;
            private bool barShown;
            private float lastActivity;   // Time.unscaledTime of last scroll movement

            public ScrollDrag(ScrollView sv) {

                this.sv = sv;
                this.scroller = sv.verticalScroller;

                VisualElement viewport = sv.contentViewport;
                viewport.RegisterCallback<PointerDownEvent>(OnDown);
                viewport.RegisterCallback<PointerMoveEvent>(OnMove);
                viewport.RegisterCallback<PointerUpEvent>(OnUp);
                viewport.RegisterCallback<PointerCaptureOutEvent>(OnCaptureOut);

                // One ticker drives both momentum decay and the idle-hide fade.
                sv.schedule.Execute(Tick).Every(16);

                SetBar(false);
                lastActivity = -999f;
            }

            private float MaxOffset() {
                float range = sv.contentContainer.resolvedStyle.height
                            - sv.contentViewport.resolvedStyle.height;
                return range > 0f ? range : 0f;
            }

            private void SetBar(bool show) {

                if (barShown == show || scroller == null) {
                    return;
                }

                barShown = show;
                // Opacity fade (common.uss puts a transition on the scroller). Auto still manages
                // display based on overflow; we only animate opacity within that.
                scroller.style.opacity = show ? 1f : 0f;
            }

            private void OnDown(PointerDownEvent e) {
                active = true;
                capturing = false;
                velocity = 0f;
                startY = e.position.y;
                lastPointerY = e.position.y;
                lastMoveTime = Time.unscaledTime;
                startOffsetY = sv.scrollOffset.y;
                pointerId = e.pointerId;
            }

            private void OnMove(PointerMoveEvent e) {

                if (!active) {
                    return;
                }

                float dy = e.position.y - startY;

                if (!capturing) {

                    if (Mathf.Abs(dy) < threshold) {
                        return;
                    }

                    capturing = true;
                    sv.contentViewport.CapturePointer(pointerId);
                }

                float newOffset = Mathf.Clamp(startOffsetY - dy, 0f, MaxOffset());
                sv.scrollOffset = new Vector2(sv.scrollOffset.x, newOffset);

                // Scroll velocity from this move's pointer motion over the REAL elapsed time
                // (PointerMove fires at variable intervals, so a fixed dt would distort the flick).
                // Drag content up => scroll down, hence the negation. Smoothed so one jittery frame
                // doesn't dominate the release velocity.
                float now = Time.unscaledTime;
                float dt = now - lastMoveTime;
                if (dt > 0.0001f) {
                    float instant = -(e.position.y - lastPointerY) / dt;
                    velocity = Mathf.Lerp(velocity, instant, 0.6f);
                }
                lastPointerY = e.position.y;
                lastMoveTime = now;

                lastActivity = now;
                SetBar(true);
                e.StopPropagation();
            }

            private void OnUp(PointerUpEvent e) {

                active = false;

                if (capturing && sv.contentViewport.HasPointerCapture(pointerId)) {
                    sv.contentViewport.ReleasePointer(pointerId);
                }

                capturing = false;
                // velocity carries into Tick -> the flick/swoosh.
            }

            private void OnCaptureOut(PointerCaptureOutEvent e) {
                active = false;
                capturing = false;
            }

            private void Tick() {

                // Momentum: keep scrolling after release, decaying to a stop.
                if (!active && Mathf.Abs(velocity) > minVelocity) {

                    float offset = sv.scrollOffset.y + velocity * tickDt;
                    float max = MaxOffset();

                    if (offset <= 0f || offset >= max) {
                        velocity = 0f;   // hit an edge, stop
                    }

                    sv.scrollOffset = new Vector2(sv.scrollOffset.x, Mathf.Clamp(offset, 0f, max));
                    velocity *= decayPerTick;
                    lastActivity = Time.unscaledTime;
                    SetBar(true);
                }
                else if (Mathf.Abs(velocity) <= minVelocity) {
                    velocity = 0f;
                }

                // Auto-hide: fade the bar out once nothing has scrolled for a moment.
                if (!active && velocity == 0f
                        && Time.unscaledTime - lastActivity > idleHideSeconds) {
                    SetBar(false);
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
