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
        //
        // `alive` alone cannot keep that promise for UI Toolkit: a VisualElement is not a
        // UnityEngine.Object, so there is no destroyed-but-not-null overload to read and a ref
        // into a torn-down view still reports alive (writing a style on one throws from inside
        // UIElements). The toolkit backend closes that gap for every op in UIToolkitBackend.El /
        // HostAlive, using the liveness of the GameObject that hosts the view — the half only the
        // backend can know. Keep any new backend op resolving through El, or it opts out of this.
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

    // A named label inside a view, resolved ONCE per view instead of on every write.
    //
    // UIUtil.UpdateLabelObject(view, name, text) resolves by name on every call: on UI Toolkit
    // that is a Q() walk plus a new UIRef, ~30 B a call — fine for a one-off write, but a HUD
    // writing five labels every frame paid it five times a frame (measured). This holds the
    // resolved ref and writes through the same backend ops, so the text that lands is identical.
    //
    // Staleness is the whole point of the design, because a toolkit view's elements do not stay
    // valid: a reload builds a NEW view UIRef (and a free sets UIRef.none), and one frame after a
    // free Unity RECYCLES the old elements (blank, parentless, panel == null). So it rebinds when
    // the view ref it was resolved under is no longer the one passed in, and when the cached
    // element stops answering — UIUtil.GetLabelValue returns null for exactly the refs
    // UIToolkitBackend.El rejects (host destroyed, freed-view marker, panel == null), so a
    // recycled element can never be written to.
    //
    // A view with no element of that name resolves to UIRef.none once and stays that way until
    // the view ref changes (UXML views are static; that is what saves the per-frame Q()). Every
    // op on UIRef.none is a no-op, as with UpdateLabelObject.
    public sealed class UIViewLabel {

        private readonly string _name;
        private UIRef _view;
        private UIRef _label = UIRef.none;

        public UIViewLabel(string name) {
            _name = name == null ? "" : name;
        }

        public string name {
            get {
                return _name;
            }
        }

        // The element named `name` under `view`, or UIRef.none.
        public UIRef Resolve(UIRef view) {

            bool rebind = !ReferenceEquals(view, _view);

            // Same view, but the element we hold went dead under it (recycled, host destroyed).
            if (!rebind && _label != UIRef.none && UIUtil.GetLabelValue(_label) == null) {
                rebind = true;
            }

            if (rebind) {
                _view = view;
                _label = view == null ? UIRef.none : UIUtil.ResolveDeep(view, _name);
            }

            return _label;
        }

        // True when the label already shows `text`, or there is no live label to show anything
        // (then a write would no-op anyway). Callers use it as the "skip the format" guard: it
        // reads the element's CURRENT text, not a remembered one, so a recycled or rebuilt element
        // — or another writer — makes it false and the caller refills.
        public bool Shows(UIRef view, string text) {

            UIRef r = Resolve(view);

            if (r == UIRef.none) {
                return true;
            }

            string current = UIUtil.GetLabelValue(r);

            return current == null || string.Equals(current, text);
        }

        public void Set(UIRef view, string text) {
            UIUtil.SetLabelValue(Resolve(view), text);
        }

        // Drop the cached element (e.g. from FreeToolkitView); the next call resolves again.
        public void Clear() {
            _view = null;
            _label = UIRef.none;
        }
    }
}
