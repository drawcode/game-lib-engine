using UnityEngine;
using UnityEngine.UI;

using Engine.Utility;

namespace Engine.Animation {

    public class TransformTweenTarget : ITweenTarget {

        private readonly GameObject go;
        private readonly Transform tr;

        public TransformTweenTarget(GameObject go) {
            this.go = go;
            this.tr = go ? go.transform : null;
        }

        public object native {
            get {
                return tr;
            }
        }

        public string targetId {
            get {
                if (!go) {
                    return "0";
                }

                return go.GetEntityId().ToString();
            }
        }

        public bool alive {
            get {
                return go;
            }
        }

        public Vector3 GetPosition(TweenCoord coord) {

            if (!tr) {
                return Vector3.zero;
            }

            if (coord == TweenCoord.local) {
                return tr.localPosition;
            }

            return tr.position;
        }

        public void SetPosition(Vector3 v, TweenCoord coord) {

            if (!tr) {
                return;
            }

            if (coord == TweenCoord.local) {
                tr.localPosition = v;
            }
            else {
                tr.position = v;
            }
        }

        public Vector3 GetScale() {

            if (!tr) {
                return Vector3.one;
            }

            return tr.localScale;
        }

        public void SetScale(Vector3 v) {

            if (!tr) {
                return;
            }

            tr.localScale = v;
        }

        public Vector3 GetRotation(TweenCoord coord) {

            if (!tr) {
                return Vector3.zero;
            }

            if (coord == TweenCoord.local) {
                return tr.localEulerAngles;
            }

            return tr.eulerAngles;
        }

        public void SetRotation(Vector3 euler, TweenCoord coord) {

            if (!tr) {
                return;
            }

            if (coord == TweenCoord.local) {
                tr.localEulerAngles = euler;
            }
            else {
                tr.eulerAngles = euler;
            }
        }

        // ALPHA / COLOUR CARRIER RESOLUTION
        //
        // These lookups used to run on EVERY tween tick. One alpha tween on a target that carries
        // none of them therefore issued SIX failing GetComponent calls per frame -- UISlicedSprite,
        // UISprite, UITiledSprite, CanvasGroup, Graphic, Renderer -- and in the Editor a failing
        // GetComponent builds its own null-error message (the GetComponentNullErrorMessage sample).
        //
        // Measured live 2026-09-09: exactly ONE active alpha tween produced exactly SIX
        // GetComponentNullErrorMessage samples in a frame, every one of them under
        // AnimationEasing.Update(). A round left running long enough to accumulate items reached
        // 413 per frame -- the same class of fault iteration 11 fixed in GamePlayerItem.
        //
        // A TransformTweenTarget is built once per tween (TweenUtil.ResolveTarget news one and the
        // tick closure holds it), so resolving on first use and caching collapses per-tick lookups
        // to one resolution per tween, and drops the per-tick Graphic[] allocation in SetColor.
        //
        // The trade: a component ADDED to the target mid-tween is not picked up, and neither is a
        // Graphic re-parented under a CanvasGroup mid-tween. Nothing here adds a renderer or widget
        // to an object while it is being faded, and the cache lives no longer than the tween.
        // Destroyed components are still handled -- every use below re-tests the cached reference
        // with Unity's null-overloaded truthiness, so a torn-down target simply stops applying.

        private bool alphaResolved = false;
        private CanvasGroup alphaGroup = null;
        private Graphic alphaGraphic = null;
        private Renderer alphaRenderer = null;

#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3
        private UIWidget alphaWidget = null;
#endif

        private bool colorResolved = false;
        private CanvasGroup colorGroup = null;
        private Graphic colorGroupGraphic = null;
        private Graphic[] colorGroupGraphics = null;
        private Graphic colorGraphic = null;
        private Renderer colorRenderer = null;

#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3
        private UIWidget colorWidget = null;
        private Light colorLight = null;
#endif

        // Same priority order the per-tick code used: NGUI sprite-on-self, then CanvasGroup, then
        // Graphic, then Renderer -- first hit wins and the rest are never looked up.
        private void ResolveAlphaCarriers() {

            if (alphaResolved) {
                return;
            }

            alphaResolved = true;

            if (!tr) {
                return;
            }

#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3
            alphaWidget = GetSelfSpriteWidget();

            if (alphaWidget) {
                return;
            }
#endif

            alphaGroup = tr.GetComponent<CanvasGroup>();

            if (alphaGroup) {
                return;
            }

            alphaGraphic = tr.GetComponent<Graphic>();

            if (alphaGraphic) {
                return;
            }

            alphaRenderer = tr.GetComponent<Renderer>();
        }

        // Colour resolves ALL carriers, no early out: TrySetNguiColor drove widget, renderer and
        // light together ("not mutually exclusive" -- NGUI 2.7 TweenColor.Awake), and SetColor's
        // CanvasGroup branch needs the whole child Graphic set.
        private void ResolveColorCarriers() {

            if (colorResolved) {
                return;
            }

            colorResolved = true;

            if (!tr) {
                return;
            }

#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3
            colorWidget = tr.GetComponentInChildren<UIWidget>();
            colorLight = tr.GetComponent<Light>();
#endif

            colorGroup = tr.GetComponent<CanvasGroup>();

            if (colorGroup) {

                colorGroupGraphics = tr.GetComponentsInChildren<Graphic>(true);

                // GetComponentsInChildren returns self first, so [0] is what the old
                // GetComponentInChildren<Graphic>(true) call resolved to.
                if (colorGroupGraphics != null && colorGroupGraphics.Length > 0) {
                    colorGroupGraphic = colorGroupGraphics[0];
                }
            }

            colorGraphic = tr.GetComponent<Graphic>();
            colorRenderer = tr.GetComponent<Renderer>();
        }

        public float GetAlpha() {

            if (!tr) {
                return 1f;
            }

            ResolveAlphaCarriers();

#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3
            if (alphaWidget) {
                return alphaWidget.alpha;
            }
#endif

            if (alphaGroup) {
                return alphaGroup.alpha;
            }

            if (alphaGraphic) {
                return alphaGraphic.color.a;
            }

            if (alphaRenderer) {
                return alphaRenderer.material.color.a;
            }

            return 1f;
        }

        public void SetAlpha(float a) {

            if (!tr) {
                return;
            }

            ResolveAlphaCarriers();

#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3

            // SPRITE-ON-SELF ONLY (Phase 1 gate finding, trace-verified): pre-flip,
            // facade fades were real ONLY for GOs carrying an NGUI sprite themselves
            // (the old FadeToObject sprite-routing to UITweenerUtil); every other
            // NGUI fade -- containers, UIPanel hosts, labels -- was a LeanTween no-op
            // that a decade of scenes and choreography (BaseGameUIPanelBackgrounds
            // Show/Hide* calls, the PanelBackgroundUI backer) silently depends on.
            // Driving panel or generic-widget alpha here dims/blanks whole screens.
            if (alphaWidget) {
                alphaWidget.alpha = a;
                return;
            }
#endif

            if (alphaGroup) {
                alphaGroup.alpha = a;
                return;
            }

            if (alphaGraphic) {
                Color cg = alphaGraphic.color;
                cg.a = a;
                alphaGraphic.color = cg;
                return;
            }

            if (alphaRenderer) {
                Color cr = alphaRenderer.material.color;
                cr.a = a;
                alphaRenderer.material.color = cr;
            }
        }

        public Color GetColor() {

            if (!tr) {
                return Color.white;
            }

            ResolveColorCarriers();

#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3
            if (colorWidget) {
                return colorWidget.color;
            }

            if (colorRenderer) {
                return colorRenderer.material.color;
            }

            if (colorLight) {
                return colorLight.color;
            }
#endif

            if (colorGroup && colorGroupGraphic) {
                return colorGroupGraphic.color;
            }

            if (colorGraphic) {
                return colorGraphic.color;
            }

            if (colorRenderer) {
                return colorRenderer.material.color;
            }

            return Color.white;
        }

        public void SetColor(Color c) {

            if (!tr) {
                return;
            }

            ResolveColorCarriers();

#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3

            // Mirrors NGUI 2.7 TweenColor.Awake()/color setter: UIWidget (self or
            // children), Renderer.material (self) and Light (self) are all touched
            // together, not mutually exclusive.
            bool found = false;

            if (colorWidget) {
                colorWidget.color = c;
                found = true;
            }

            if (colorRenderer) {
                colorRenderer.material.color = c;
                found = true;
            }

            if (colorLight) {
                colorLight.color = c;
                colorLight.enabled = (c.r + c.g + c.b) > 0.01f;
                found = true;
            }

            if (found) {
                return;
            }
#endif

            if (colorGroup) {

                if (colorGroupGraphics != null) {

                    for (int i = 0; i < colorGroupGraphics.Length; i++) {

                        if (colorGroupGraphics[i]) {
                            colorGroupGraphics[i].color = c;
                        }
                    }
                }

                return;
            }

            if (colorGraphic) {
                colorGraphic.color = c;
                return;
            }

            if (colorRenderer) {
                colorRenderer.material.color = c;
            }
        }

#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3

        private UIWidget GetSelfSpriteWidget() {

            UIWidget widget = tr.GetComponent<UISlicedSprite>();

            if (!widget) {
                widget = tr.GetComponent<UISprite>();
            }

            if (!widget) {
                widget = tr.GetComponent<UITiledSprite>();
            }

            return widget;
        }
#endif
    }
}
