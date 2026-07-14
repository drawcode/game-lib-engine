namespace Engine.UI.Bitty {

    // bitty schema v1 — the VOCABULARY only.
    //
    // Deliberately no parser and no pattern implementations here. User decision (2026-07-13):
    // Phase 2 ships the schema + token source; the bitty runtime lands in Phase 3, where waves
    // 3A (option-row) and 3D (list) are its first real consumers. Designing a pattern catalog
    // before a single real panel has exercised it is how you get a catalog that fits nothing.
    //
    // What v1 must pin down NOW is the shared vocabulary, because two things are already being
    // built against it: the prefab→UXML converter (2.4/2.5) emits these names, and the Phase 3
    // runtime will read them. If they disagree, everything the converter produces is garbage.
    //
    // The prototype in game-drawlabs-assets-common-1 is NOT the ancestor of this. It had no
    // class/USS support, no events, no theming, no patterns, no template reference, and no
    // consumers. What survives from it is the key vocabulary (type/name/text/value/label/
    // choices/style/children) and the recursion shape. Everything below that it lacked —
    // `class`, `template`, `pattern`, `bind`, `events` — is what makes the schema usable.
    //
    // ---------------------------------------------------------------------------------------
    // THE ONE INVARIANT: `name` does three jobs at once.
    //
    //   1. the element's name in the backend (VisualElement.name / GameObject.name)
    //   2. the BindElements key that binds it to a panel's UIRef field
    //   3. the string the click bridge broadcasts on the name-keyed event bus
    //
    // One identifier, three jobs. That is the whole reason the platform holds together, and
    // why the pilot is cheap: existing OnButtonClickEventHandler(string) handlers keep working
    // untouched. Break this and every layer needs its own lookup table.
    // ---------------------------------------------------------------------------------------
    public class BittySchema {

        public const int version = 1;

        // KEYS — the recognized element keys.
        public class Keys {

            public const string type = "type";          // required; see Types
            public const string name = "name";          // the invariant above
            public const string cls = "class";          // string | string[] — USS classes
            public const string text = "text";          // literal, or "@loc:key" to localize
            public const string value = "value";        // bool | string | float, per type
            public const string label = "label";        // field label (option rows, inputs)
            public const string choices = "choices";    // string[] — dropdowns
            public const string style = "style";        // inline escape hatch; see the rule below
            public const string children = "children";  // nested element list
            public const string template = "template";  // native presentation ref, e.g. "panel-settings.uxml"
            public const string pattern = "pattern";    // catalog entry; see Patterns
            public const string bind = "bind";          // { property: dataPath }
            public const string events = "events";      // { nativeEvent: elementNameToBroadcast }
        }

        // TYPES — the element types v1 understands.
        public class Types {

            public const string container = "container";
            public const string label = "label";
            public const string button = "button";
            public const string image = "image";
            public const string textfield = "textfield";
            public const string toggle = "toggle";
            public const string slider = "slider";
            public const string dropdown = "dropdown";
            public const string scrollview = "scrollview";
            public const string row = "row";
        }

        // PATTERNS — the catalog. v1 fixes the NAMES and their required keys; Phase 3 implements
        // the renderers. These six are the patterns this game actually instantiates (settings
        // rows, list screens, dialogs, header/footer chrome, HUD widgets, the load overlay) —
        // nothing speculative, per the plan's abstraction-overreach guard.
        public class Patterns {

            public const string dialog = "dialog";           // requires: name, children (+ optional title)
            public const string list = "list";               // requires: name, item template + data path
            public const string optionRow = "option-row";    // requires: name, label, a value-bearing child
            public const string chrome = "chrome";           // header/footer bars
            public const string hudWidget = "hud-widget";    // requires: name, bind
            public const string overlay = "overlay";         // loading/transition overlay
        }

        // Localization prefix. "@loc:settings.title" routes through Locos.GetString, which is
        // how UILocalizedLabel survives the migration without a per-label component.
        public const string locPrefix = "@loc:";

        public static bool IsLocalized(string text) {

            if (string.IsNullOrEmpty(text)) {
                return false;
            }

            return text.StartsWith(locPrefix);
        }

        public static string LocKey(string text) {

            if (!IsLocalized(text)) {
                return text;
            }

            return text.Substring(locPrefix.Length);
        }

        // STYLING RULE (enforced by review, not by code — stated here so there is one place to
        // point at): styling is `class` + tokens. Inline `style` is an escape hatch for
        // one-off geometry only. Anything reused twice, or anything themed (color, font, size,
        // duration), MUST be a token or a class. The prototype got this exactly backwards —
        // it had no class support at all and inlined every color as a hex literal, which is
        // precisely why it could never be themed.
    }
}
