using UnityEngine;

namespace Engine.UI {

    // Opaque handle to a backend element: a GameObject under NGUI/uGUI, a VisualElement
    // under UI Toolkit. Call sites and panel code hold UIRefs and never see the backend
    // type — the same trick as ITweenTarget.native, and the reason TweenUtil.ResolveTarget
    // (object) can take a UIRef's native straight through with no tween-side changes.
    //
    // Deliberately NOT [Serializable] and NOT a UnityEngine.Object: when a panel's
    // #else-branch field flips to UIRef, Unity simply ignores the field, so nothing is
    // carried over from the prefab and the value MUST come from BindElements at runtime.
    // That is the point — it makes name-binding the only path and makes a half-inspector /
    // half-bound hybrid impossible to write by accident.
    public class UIRef {

        // Never null — mirrors TweenPresets.Get, which also never returns null. Every
        // backend op no-ops on a ref that is not alive, so a missed bind degrades to
        // "nothing happens", never to a NullReferenceException.
        public static readonly UIRef none = new UIRef(null, "");

        private readonly object _native;
        private readonly string _name;

        public UIRef(object native, string name) {
            _native = native;
            _name = name == null ? "" : name;
        }

        public object native {
            get {
                return _native;
            }
        }

        public string name {
            get {
                return _name;
            }
        }

        // Unity's Object overloads ==, so a destroyed GameObject is "null" without the C#
        // reference being null. Non-Unity natives (VisualElement) take the plain check.
        public bool alive {
            get {

                if (_native == null) {
                    return false;
                }

                if (_native is Object) {
                    return (Object)_native;
                }

                return true;
            }
        }

        // Convenience for the GameObject-based backends; returns null for a VisualElement
        // native, which is exactly what those backends want (they will not claim it).
        public GameObject gameObject {
            get {
                return _native as GameObject;
            }
        }

        public static UIRef Of(GameObject go) {

            if (go == null) {
                return none;
            }

            return new UIRef(go, go.name);
        }

        public static UIRef Of(object native, string name) {

            if (native == null) {
                return none;
            }

            return new UIRef(native, name);
        }
    }
}
