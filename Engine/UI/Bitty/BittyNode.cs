using System.Collections.Generic;

namespace Engine.UI.Bitty {

    // The parsed, backend-AGNOSTIC representation of one bitty element.
    //
    // This is the tree that BittyParser produces from schema-v1 JSON and that a backend's
    // element factory (for UI Toolkit: BittyToolkitBuilder) turns into native VisualElements.
    // It deliberately names no UI framework type — a BittyNode tree is portable, and the same
    // tree could be built by a DOM backend or any other. That is the whole reason the parse and
    // the element factory are split across the provider boundary (design doc D6): parsing is
    // agnostic, building is not.
    //
    // Field set mirrors BittySchema.Keys 1:1 (the vocabulary v1 pinned). Nothing here is
    // speculative — every field is consumed by the option-row pattern (3A) or the credits/list
    // work, which are the schema's first real consumers.
    public class BittyNode {

        // type/name — the invariant. `name` does three jobs (element name, bind key, broadcast
        // string). See BittySchema's header comment; do not let these drift apart.
        public string type = BittySchema.Types.container;
        public string name;

        // class — one or many USS classes. JSON allows string OR string[]; the parser normalizes
        // both to this list, so the builder never branches on shape.
        public List<string> classes = new List<string>();

        // text — literal, or "@loc:key". Localization is resolved at BUILD time (BittyToolkitBuilder
        // routes @loc: through Locos), not here, so the node stays a pure data record.
        public string text;

        // value — bool | string | float, per type (toggle=bool, textfield=string, slider=float).
        // Kept as object because the schema is polymorphic here; the builder coerces per element.
        public object value;

        // label — a field label (option rows, inputs). Literal or "@loc:key".
        public string label;

        // choices — dropdown options.
        public List<string> choices;

        // style — inline escape hatch. NOT applied at runtime in v1 (styling is class + tokens,
        // per BittySchema's styling rule); retained so a round-trip through the node preserves it
        // and a future backend may honor it. Anything shared/themed must be a class, not this.
        public string style;

        // template — native presentation reference (e.g. "panel-settings.uxml"). When set, the
        // backend resolves the node to that native asset instead of building children. This is
        // what keeps app-IP screens pixel-native while their data stays portable.
        public string template;

        // pattern — a catalog entry name (BittySchema.Patterns). Expanded by BittyPatterns BEFORE
        // the builder runs, so the builder only ever sees concrete element trees.
        public string pattern;

        // bind — { property: dataPath }. The panel controller reads these; the platform does not
        // auto-bind data (there is no global data store) — bind is carried for the controller and
        // for pattern expansion (option-row reads bind["value"] as its control's data path).
        public Dictionary<string, string> bind;

        // events — { nativeEvent: elementNameToBroadcast }. Maps a native event to a name on the
        // existing name-keyed bus. "click" is redundant with the bubbling ClickEvent (the element's
        // own name already broadcasts), so events is mainly for value controls whose broadcast name
        // should differ from the element name (rare) — v1 keeps it for completeness.
        public Dictionary<string, string> events;

        public List<BittyNode> children = new List<BittyNode>();

        public bool HasChildren {
            get {
                return children != null && children.Count > 0;
            }
        }
    }
}
