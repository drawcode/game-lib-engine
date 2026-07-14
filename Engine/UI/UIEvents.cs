using Engine.Events;

namespace Engine.UI {

    // The name-keyed UI event bus.
    //
    // This is NOT part of IUIBackend, and that split is the whole point. ~199 of the ~200
    // IsButtonClicked call sites use the (string, string) overload: a pure comparison of a
    // broadcast button name against a constant. It touches no UI object at all, so putting
    // it on the backend interface would force every backend to implement string equality.
    //
    // The click path was ALREADY backend-agnostic before this migration: ButtonEvents
    // broadcasts gameObject.name, BaseUIController re-broadcasts by name, and panels switch
    // on the string in OnButtonClickEventHandler(string). UI Toolkit therefore needs no new
    // event model — UIToolkitClickBridge just calls BroadcastClick(element.name) and every
    // existing handler keeps working untouched.
    //
    // The event-name values below MUST stay byte-identical to ButtonEvents' (game-lib-games)
    // — they are the wire format between the broadcaster and ~200 live listeners. ButtonEvents
    // aliases these; engine cannot depend on game-lib-games, so the constants live here.
    public class UIEvents {

        public const string EVENT_BUTTON_CLICK = "event-button-click";
        public const string EVENT_BUTTON_CLICK_OBJECT = "event-button-click-object";
        public const string EVENT_BUTTON_CLICK_DATA = "event-button-click-data";

        public static bool IsButtonClicked(string button, string buttonClickedName) {

            if (button == null) {
                return false;
            }

            return buttonClickedName == button;
        }

        public static bool IsButtonClickedLike(string button, string buttonClickedName) {

            if (button == null) {
                return false;
            }

            return buttonClickedName.Contains(button);
        }

        public static void BroadcastClick(string elementName) {

            if (string.IsNullOrEmpty(elementName)) {
                return;
            }

            Messenger<string>.Broadcast(EVENT_BUTTON_CLICK, elementName);
        }
    }
}
