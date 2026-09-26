using UnityEngine;

namespace Engine.UI {

    // The GameObject half of UIRef as extension methods (bitty gap-list step 3), so agnostic code
    // can hold a UIRef without naming UIRef.Of/.gameObject. Those originals stay: UIRef.Of is a
    // static call and .gameObject a property, and extension methods can replace neither.
    public static class UIRefUnityExtensions {

        public static UIRef ToUIRef(this GameObject go) {
            return UIRef.Of(go);
        }

        public static GameObject AsGameObject(this UIRef r) {
            return r.gameObject;
        }
    }
}
