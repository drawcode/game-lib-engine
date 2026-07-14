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

        public float GetAlpha() {

            if (!tr) {
                return 1f;
            }

#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3
            float nguiAlpha;

            if (TryGetNguiAlpha(out nguiAlpha)) {
                return nguiAlpha;
            }
#endif

            CanvasGroup group = tr.GetComponent<CanvasGroup>();

            if (group) {
                return group.alpha;
            }

            Graphic graphic = tr.GetComponent<Graphic>();

            if (graphic) {
                return graphic.color.a;
            }

            Renderer renderer = tr.GetComponent<Renderer>();

            if (renderer) {
                return renderer.material.color.a;
            }

            return 1f;
        }

        public void SetAlpha(float a) {

            if (!tr) {
                return;
            }

#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3
            if (TrySetNguiAlpha(a)) {
                return;
            }
#endif

            CanvasGroup group = tr.GetComponent<CanvasGroup>();

            if (group) {
                group.alpha = a;
                return;
            }

            Graphic graphic = tr.GetComponent<Graphic>();

            if (graphic) {
                Color c = graphic.color;
                c.a = a;
                graphic.color = c;
                return;
            }

            Renderer renderer = tr.GetComponent<Renderer>();

            if (renderer) {
                Color c = renderer.material.color;
                c.a = a;
                renderer.material.color = c;
            }
        }

        public Color GetColor() {

            if (!tr) {
                return Color.white;
            }

#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3
            Color nguiColor;

            if (TryGetNguiColor(out nguiColor)) {
                return nguiColor;
            }
#endif

            CanvasGroup group = tr.GetComponent<CanvasGroup>();

            if (group) {
                Graphic childGraphic = tr.GetComponentInChildren<Graphic>(true);

                if (childGraphic) {
                    return childGraphic.color;
                }
            }

            Graphic selfGraphic = tr.GetComponent<Graphic>();

            if (selfGraphic) {
                return selfGraphic.color;
            }

            Renderer renderer = tr.GetComponent<Renderer>();

            if (renderer) {
                return renderer.material.color;
            }

            return Color.white;
        }

        public void SetColor(Color c) {

            if (!tr) {
                return;
            }

#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3
            if (TrySetNguiColor(c)) {
                return;
            }
#endif

            CanvasGroup group = tr.GetComponent<CanvasGroup>();

            if (group) {

                Graphic[] graphics = tr.GetComponentsInChildren<Graphic>(true);

                for (int i = 0; i < graphics.Length; i++) {
                    graphics[i].color = c;
                }

                return;
            }

            Graphic selfGraphic = tr.GetComponent<Graphic>();

            if (selfGraphic) {
                selfGraphic.color = c;
                return;
            }

            Renderer renderer = tr.GetComponent<Renderer>();

            if (renderer) {
                renderer.material.color = c;
            }
        }

#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3

        // SPRITE-ON-SELF ONLY (Phase 1 gate finding, trace-verified): pre-flip,
        // facade fades were real ONLY for GOs carrying an NGUI sprite themselves
        // (the old FadeToObject sprite-routing to UITweenerUtil); every other
        // NGUI fade — containers, UIPanel hosts, labels — was a LeanTween no-op
        // that a decade of scenes and choreography (BaseGameUIPanelBackgrounds
        // Show/Hide* calls, the PanelBackgroundUI backer) silently depends on.
        // Driving panel or generic-widget alpha here dims/blanks whole screens.
        private bool TryGetNguiAlpha(out float alpha) {

            UIWidget widget = GetSelfSpriteWidget();

            if (widget) {
                alpha = widget.alpha;
                return true;
            }

            alpha = 1f;
            return false;
        }

        private bool TrySetNguiAlpha(float value) {

            UIWidget widget = GetSelfSpriteWidget();

            if (widget) {
                widget.alpha = value;
                return true;
            }

            return false;
        }

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

        // Mirrors NGUI 2.7 TweenColor.Awake()/color setter: UIWidget (self or
        // children), Renderer.material (self) and Light (self) are all touched
        // together, not mutually exclusive.
        private bool TryGetNguiColor(out Color color) {

            UIWidget widget = tr.GetComponentInChildren<UIWidget>();

            if (widget) {
                color = widget.color;
                return true;
            }

            Renderer renderer = tr.GetComponent<Renderer>();

            if (renderer) {
                color = renderer.material.color;
                return true;
            }

            Light light = tr.GetComponent<Light>();

            if (light) {
                color = light.color;
                return true;
            }

            color = Color.white;
            return false;
        }

        private bool TrySetNguiColor(Color value) {

            bool found = false;

            UIWidget widget = tr.GetComponentInChildren<UIWidget>();

            if (widget) {
                widget.color = value;
                found = true;
            }

            Renderer renderer = tr.GetComponent<Renderer>();

            if (renderer) {
                renderer.material.color = value;
                found = true;
            }

            Light light = tr.GetComponent<Light>();

            if (light) {
                light.color = value;
                light.enabled = (value.r + value.g + value.b) > 0.01f;
                found = true;
            }

            return found;
        }
#endif
    }
}
