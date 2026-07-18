using UnityEngine;
using UnityEngine.UIElements;

using Engine.Utility;

namespace Engine.Animation {

    // The only Tween/ file allowed to reference UnityEngine.UIElements.
    public class VisualElementTweenTarget : ITweenTarget {

        private readonly VisualElement element;

        public VisualElementTweenTarget(VisualElement element) {
            this.element = element;
        }

        // Lets TweenUtil.ResolveTarget(object) build a VisualElementTweenTarget
        // without itself naming UnityEngine.UIElements types.
        public static ITweenTarget TryCreate(object native) {

            VisualElement element = native as VisualElement;

            if (element == null) {
                return null;
            }

            return new VisualElementTweenTarget(element);
        }

        public object native {
            get {
                return element;
            }
        }

        public string targetId {
            get {
                if (element == null) {
                    return "0";
                }

                return element.GetHashCode().ToString();
            }
        }

        public bool alive {
            get {
                return element != null;
            }
        }

        // TweenCoord is ignored: style.translate/rotate/scale are always self-relative.

        // Getters are keyword-aware: element.style only reflects inline values, so an
        // unset property (StyleKeyword.Null) must fall back to resolvedStyle when the
        // element is attached to a panel — style.opacity.value would read 0 otherwise.

        public Vector3 GetPosition(TweenCoord coord) {

            if (element == null) {
                return Vector3.zero;
            }

            if (element.style.translate.keyword == StyleKeyword.Undefined) {
                Translate t = element.style.translate.value;
                return new Vector3(t.x.value, t.y.value, t.z);
            }

            if (element.panel != null) {
                return element.resolvedStyle.translate;
            }

            return Vector3.zero;
        }

        // Writing a transform inline style (translate/scale/rotate) on an element that is no longer
        // attached to a panel throws inside Unity's InlineStyleAccess (ApplyStyleTranslate NRE).
        // That happens when a panel is pooled away mid-tween: the view is destroyed but the tween
        // is still ticking. Skip the write when detached — the element is going away anyway, and
        // TweenUtil.Cancel on FreeToolkitView stops the tween shortly after.
        private bool detached {
            get {
                return element == null || element.panel == null;
            }
        }

        public void SetPosition(Vector3 v, TweenCoord coord) {

            if (detached) {
                return;
            }

            element.style.translate = new Translate(v.x, v.y, v.z);
        }

        public Vector3 GetScale() {

            if (element == null) {
                return Vector3.one;
            }

            if (element.style.scale.keyword == StyleKeyword.Undefined) {
                return element.style.scale.value.value;
            }

            if (element.panel != null) {
                return element.resolvedStyle.scale.value;
            }

            return Vector3.one;
        }

        public void SetScale(Vector3 v) {

            if (detached) {
                return;
            }

            element.style.scale = new Scale(v);
        }

        public Vector3 GetRotation(TweenCoord coord) {

            if (element == null) {
                return Vector3.zero;
            }

            if (element.style.rotate.keyword == StyleKeyword.Undefined) {
                Rotate r = element.style.rotate.value;
                return new Vector3(0f, 0f, (float)r.angle.ToDegrees());
            }

            if (element.panel != null) {
                return new Vector3(0f, 0f, (float)element.resolvedStyle.rotate.angle.ToDegrees());
            }

            return Vector3.zero;
        }

        public void SetRotation(Vector3 euler, TweenCoord coord) {

            if (detached) {
                return;
            }

            element.style.rotate = new Rotate(new Angle(euler.z, AngleUnit.Degree));
        }

        public float GetAlpha() {

            if (element == null) {
                return 1f;
            }

            if (element.style.opacity.keyword == StyleKeyword.Undefined) {
                return element.style.opacity.value;
            }

            if (element.panel != null) {
                return element.resolvedStyle.opacity;
            }

            return 1f;
        }

        public void SetAlpha(float a) {

            if (element == null) {
                return;
            }

            element.style.opacity = a;
        }

        // Background tint for regular elements, style.color for text elements.
        public Color GetColor() {

            if (element == null) {
                return Color.white;
            }

            if (element is TextElement) {

                if (element.style.color.keyword == StyleKeyword.Undefined) {
                    return element.style.color.value;
                }

                if (element.panel != null) {
                    return element.resolvedStyle.color;
                }

                return Color.white;
            }

            if (element.style.unityBackgroundImageTintColor.keyword == StyleKeyword.Undefined) {
                return element.style.unityBackgroundImageTintColor.value;
            }

            if (element.panel != null) {
                return element.resolvedStyle.unityBackgroundImageTintColor;
            }

            return Color.white;
        }

        public void SetColor(Color c) {

            if (element == null) {
                return;
            }

            if (element is TextElement) {
                element.style.color = c;
            }
            else {
                element.style.unityBackgroundImageTintColor = c;
            }
        }
    }
}
