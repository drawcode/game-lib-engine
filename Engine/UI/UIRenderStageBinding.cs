using UnityEngine;

namespace Engine.UI {

    // Ties a UIRenderStage (3D subtree -> private camera -> RenderTexture) to ONE element of a UI
    // view for the element's whole life (B9 S1, 2026-10-03). The six hand-wired callers (header
    // coin + large rig, HUD coin, Products, ProductCurrency, notification coin) each re-solve the
    // same three problems by hand — when to render, how big the RT is, and who detaches — and
    // each got at least one wrong at some point. This is the opt-in path that solves them once;
    // the existing callers are deliberately left as they are.
    //
    // UIRef-only, like the rest of the engine above the provider layer: no UnityEngine.UIElements
    // type appears here. The backend answers visibility (IUIBackend.IsVisible) and, when it can,
    // size and liveness (IUIBackendElementGeometry).
    //
    // LIFETIME. Bind() attaches the stage and hangs this component on the stage's own
    // "ui-render-stage-*" GameObject, so Unbind() -> UIRenderStage.Detach() restores the content's
    // layers, releases the RT and removes the binding in one destroy. Nothing else is created.
    //
    // WHEN TO BIND. From BindElements / the LoadToolkitView continuation (i.e. once the view
    // exists), NEVER from OnEnable/OnDisable or any show/hide callback: a binding made on
    // activation is re-made on every pool cycle, and Attach on content that is ALREADY staged
    // records the stage layer as the "original" one, so the later Detach restores the wrong layer
    // and the content vanishes from the legacy cameras for good. Free from FreeToolkitView
    // (UIPanelBase.StageLegacy3D does both halves).
    //
    // VISIBILITY, every LateUpdate, no allocation:
    //   camera.enabled = content.activeInHierarchy
    //                 && backend.IsVisible(visibilityRoot) && backend.IsVisible(element)
    //                 && element alive (geometry backend: attached to a panel, rule 172)
    // Short-circuits in that order, cheapest first. The camera is only WRITTEN on a change. The
    // stage light is never toggled — it is directional and lights the whole layer, and some stages
    // (Products, notification coin) borrow another stage's light with lightIntensity 0.
    //
    // RESOLUTION. RT side = max side of the element's pixel size, rounded UP to a multiple of 64,
    // clamped to [minSize, maxSize] (128..512). No geometry backend, or no layout yet: the
    // caller's options.size. The RT is re-created only when that bucket CHANGES and the new bucket
    // has held for resizeStableFrames visible frames, so a scale-in tween does not churn RTs.
    public class UIRenderStageBinding : MonoBehaviour {

        // Plain options bag; null means all defaults. Defaults match UIRenderStage.Attach's.
        public class Options {

            // < 0: the layer named by defaultLayerName, else fallbackLayerName (see DefaultLayer).
            public int layer = -1;

            // Fallback RT side when the backend cannot measure the element (and the first RT
            // before layout lands).
            public int size = 256;

            // Clamp for the measured side. Measured sides round up to 64.
            public int minSize = 128;
            public int maxSize = 512;

            public float framePadding = 1.15f;
            public bool keepColliderLayers = false;
            public bool followContent = false;

            // <= 0 borrows the layer's existing stage light (see UIRenderStage.Attach).
            public float lightIntensity = 1.1f;
        }

        // The widget layer every current caller uses; UI3D on older project configs.
        public static string defaultLayerName = "UIWidget3D";
        public static string fallbackLayerName = "UI3D";

        // Visible frames a new resolution bucket must hold before the RT is re-created.
        public static int resizeStableFrames = 10;

        public const int sizeStep = 64;

        public static int DefaultLayer() {

            int layer = LayerMask.NameToLayer(defaultLayerName);

            if (layer < 0) {
                layer = LayerMask.NameToLayer(fallbackLayerName);
            }

            return layer;
        }

        // Max side, rounded up to sizeStep, clamped. <= 0 (not laid out) answers `fallback`.
        public static int ResolveSize(Vector2 pixelSize, int fallback, int minSize, int maxSize) {

            float side = Mathf.Max(pixelSize.x, pixelSize.y);

            if (float.IsNaN(side) || side <= 0f) {
                return fallback;
            }

            int rounded = Mathf.CeilToInt(side / sizeStep) * sizeStep;

            return Mathf.Clamp(rounded, minSize, maxSize);
        }

        private UIRenderStage _stage;
        private GameObject _content;
        private UIRef _element = UIRef.none;
        private UIRef _visibilityRoot = UIRef.none;
        private IUIBackend _backend;
        private IUIBackendElementGeometry _geometry;
        private Options _options;

        private bool _visible;
        private int _size;
        private int _pendingSize;
        private int _pendingFrames;
        private bool _unbound;

        public UIRenderStage stage {
            get {
                return _stage;
            }
        }

        public GameObject content {
            get {
                return _content;
            }
        }

        public UIRef element {
            get {
                return _element;
            }
        }

        public bool isVisible {
            get {
                return _visible;
            }
        }

        // Current RT side in pixels.
        public int size {
            get {
                return _size;
            }
        }

        public bool isBound {
            get {
                return !_unbound && _stage != null;
            }
        }

        // Stage `content` into the element `elementName` (deep lookup) of `view`. The view root is
        // also the visibility root: a panel hidden by display on its view root stops rendering.
        // Returns null (nothing attached, no layer touched) when the view or element is not alive,
        // the content is null, or no stage layer exists.
        public static UIRenderStageBinding Bind(
            GameObject content, UIRef view, string elementName, Options options = null) {

            if (view == null || !view.alive || string.IsNullOrEmpty(elementName)) {
                return null;
            }

            return Bind(content, UIUtil.ResolveDeep(view, elementName), view, options);
        }

        // Element already resolved. visibilityRoot may be UIRef.none (element visibility only).
        public static UIRenderStageBinding Bind(
            GameObject content, UIRef element, UIRef visibilityRoot, Options options) {

            if (content == null || element == null || !element.alive) {
                return null;
            }

            IUIBackend backend = UIPlatform.For(element);

            if (backend == null) {
                return null;
            }

            Options o = options != null ? options : new Options();

            int layer = o.layer >= 0 ? o.layer : DefaultLayer();

            if (layer < 0) {
                return null;
            }

            IUIBackendElementGeometry geometry = backend as IUIBackendElementGeometry;

            int size = o.size;
            Vector2 px;

            if (geometry != null && geometry.TryGetElementPixelSize(element, out px)) {
                size = ResolveSize(px, o.size, o.minSize, o.maxSize);
            }

            UIRenderStage stage = UIRenderStage.Attach(
                content, layer, size, o.framePadding, o.keepColliderLayers, o.followContent,
                o.lightIntensity);

            if (stage == null) {
                return null;
            }

            UIRenderStageBinding binding = stage.gameObject.AddComponent<UIRenderStageBinding>();
            binding._stage = stage;
            binding._content = content;
            binding._element = element;
            binding._visibilityRoot = visibilityRoot != null ? visibilityRoot : UIRef.none;
            binding._backend = backend;
            binding._geometry = geometry;
            binding._options = o;
            binding._size = size;
            binding._pendingSize = size;

            backend.SetImageTexture(element, stage.texture);

            // Start from the real state, not "camera on": Attach leaves the camera enabled, and a
            // panel binding while hidden must not render a frame.
            binding._visible = true;
            binding.Refresh();

            return binding;
        }

        void LateUpdate() {
            Refresh();
        }

        // One visibility + resolution pass. LateUpdate calls it; public so EditMode tests (no
        // player loop) and callers that just changed state can apply it immediately.
        public void Refresh() {

            if (_unbound || _stage == null) {
                return;
            }

            bool visible = ComputeVisible();

            if (visible != _visible) {
                _visible = visible;
                _stage.SetVisible(visible);
                _pendingFrames = 0;
            }
        }

        private bool ComputeVisible() {

            if (_content == null || !_content.activeInHierarchy) {
                return false;
            }

            if (_visibilityRoot.alive && !_backend.IsVisible(_visibilityRoot)) {
                return false;
            }

            if (!_backend.IsVisible(_element)) {
                return false;
            }

            if (_geometry == null) {
                return true;
            }

            Vector2 px;

            // false = recycled / detached element (rule 172): never render into, never style it.
            if (!_geometry.TryGetElementPixelSize(_element, out px)) {
                return false;
            }

            TrackSize(px);

            return true;
        }

        private void TrackSize(Vector2 px) {

            int bucket = ResolveSize(px, _size, _options.minSize, _options.maxSize);

            if (bucket == _size) {
                _pendingFrames = 0;
                return;
            }

            if (bucket != _pendingSize) {
                _pendingSize = bucket;
                _pendingFrames = 0;
            }

            _pendingFrames++;

            if (_pendingFrames < resizeStableFrames) {
                return;
            }

            _pendingFrames = 0;
            Resize(bucket);
        }

        // Swap the stage's RT for one of `side` pixels and re-point camera + element at it. Only
        // runs on a bucket change, never per frame. The element was just proven alive by the
        // caller (TryGetElementPixelSize), and SetImageTexture resolves through the backend's own
        // liveness guard regardless.
        private void Resize(int side) {

            Camera cam = _stage.stageCamera;
            RenderTexture old = _stage.texture;

            if (cam == null || side <= 0) {
                return;
            }

            RenderTexture rt = new RenderTexture(side, side, 16, RenderTextureFormat.ARGB32);
            rt.name = _stage.gameObject.name;

            cam.targetTexture = rt;
            _stage.texture = rt;
            _size = side;

            _backend.SetImageTexture(_element, rt);

            if (old != null) {

                old.Release();

                if (Application.isPlaying) {
                    Destroy(old);
                }
                else {
                    DestroyImmediate(old);
                }
            }
        }

        // Detach the stage: content layers restored, RT released, this component destroyed with
        // the stage object. Idempotent and safe during scene teardown (stage already destroyed).
        // Does NOT write the element — its view is usually being freed in the same call.
        public void Unbind() {

            if (_unbound) {
                return;
            }

            _unbound = true;

            UIRenderStage s = _stage;
            _stage = null;
            _element = UIRef.none;
            _visibilityRoot = UIRef.none;

            if (s != null) {
                s.Detach();
            }
        }
    }
}
