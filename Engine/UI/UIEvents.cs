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

        // VALUE bus (Phase 3 / 3A). The button bus above broadcasts a name; a value control also
        // broadcasts its new value. These names MUST stay byte-identical to game-lib-games'
        // SliderEvents/CheckboxEvents/InputEvents.EVENT_ITEM_CHANGE — those are the wire format the
        // panels' Messenger<string,T> listeners already subscribe to. Engine cannot depend on
        // game-lib-games, so (exactly as with the button constants) the strings are duplicated here
        // with the same values, not code-referenced. A migrated toolkit slider/toggle/input then
        // reaches the SAME OnSliderChange/OnCheckboxChange/OnProfileInputChanged handlers untouched,
        // with no NGUI SliderEvents/CheckboxEvents MonoBehaviour on the widget.
        public const string EVENT_SLIDER_CHANGE = "event-slider-item-change";
        public const string EVENT_CHECKBOX_CHANGE = "event-checkbox-item-change";
        public const string EVENT_INPUT_CHANGE = "event-input-item-change";

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

        // The value bridge's broadcasters — the toolkit backend calls these from ChangeEvent
        // callbacks. Same shape as SliderEvents.OnSliderChange etc., so the receiving handler
        // cannot tell an NGUI change from a toolkit change.

        public static void BroadcastSliderChange(string elementName, float value) {

            if (string.IsNullOrEmpty(elementName)) {
                return;
            }

            Messenger<string, float>.Broadcast(EVENT_SLIDER_CHANGE, elementName, value);
        }

        public static void BroadcastCheckboxChange(string elementName, bool value) {

            if (string.IsNullOrEmpty(elementName)) {
                return;
            }

            Messenger<string, bool>.Broadcast(EVENT_CHECKBOX_CHANGE, elementName, value);
        }

        public static void BroadcastInputChange(string elementName, string value) {

            if (string.IsNullOrEmpty(elementName)) {
                return;
            }

            Messenger<string, string>.Broadcast(EVENT_INPUT_CHANGE, elementName, value);
        }
    }
}
