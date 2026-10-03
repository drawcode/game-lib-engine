using System;

using UnityEngine.UIElements;

namespace Engine.UI {

    // OPTIONAL backend capability: the change half of a text input. Kept out of IUIBackend so
    // adding it breaks no existing implementer (core libs are shared, additive-only), the same
    // shape as IUIFloatingStickBackend.
    //
    // The legacy path never needed it: InputEvents is a MonoBehaviour on the NGUI UIInput's own
    // GameObject and rebroadcasts its OnSubmit onto the Messenger bus (EVENT_ITEM_CHANGE). A
    // toolkit TextField is a VisualElement -- no GameObject, so no component, so nothing
    // broadcasts and a migrated input is inert however correctly it is bound. Panels register
    // through UIInputChange.SetInputHandlerChange instead.
    public interface IUIInputChangeBackend {

        // onChange gets the committed text. A delayed field (UXML is-delayed="true") commits on
        // Enter or focus loss, which is what NGUI's OnSubmit did; an undelayed one fires per key.
        void SetInputHandlerChange(UIRef r, Action<string> onChange);
    }

    // Provider-layer entry point for the input change handler, alongside UIUtil's
    // SetToggleHandlerChange / SetSliderHandlerChange.
    //
    // Lives in its own file, not in UIUtil/UIToolkitBackend, because both were under a
    // concurrent edit when this landed (B0a of plan-toolkit-paths-for-all-ngui-ugui). A backend
    // that implements IUIInputChangeBackend wins; until UIToolkitBackend does, a TextField is
    // registered here directly. TODO: fold into UIUtil.SetInputHandlerChange and move the
    // TextField branch into UIToolkitBackend as IUIInputChangeBackend -- callers of this method
    // keep working either way.
    public static class UIInputChange {

        // NGUIBackend has no change half on purpose: its widgets already broadcast through
        // InputEvents, so registering here too would fire the handler twice. A null or dead ref
        // no-ops.
        public static void SetInputHandlerChange(UIRef r, Action<string> onChange) {

            if (r == null || !r.alive || onChange == null) {
                return;
            }

            IUIInputChangeBackend capable = UIPlatform.For(r) as IUIInputChangeBackend;

            if (capable != null) {
                capable.SetInputHandlerChange(r, onChange);
                return;
            }

            TextField field = r.native as TextField;

            // panel == null: a torn-down view's element still reports alive (UIRef.alive cannot
            // see a destroyed view host), and registering on it would leak the closure.
            if (field == null || field.panel == null) {
                return;
            }

            field.RegisterValueChangedCallback(evt => onChange(evt.newValue));
        }
    }
}
