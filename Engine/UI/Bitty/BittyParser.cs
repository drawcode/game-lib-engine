using System;
using System.Collections.Generic;

using MiniJSON;

namespace Engine.UI.Bitty {

    // schema-v1 JSON  ->  BittyNode tree.  AGNOSTIC — no UI framework named here.
    //
    // Built on MiniJSON (Engine/Utility) rather than a typed FromJson<> because the schema is
    // deliberately polymorphic: `class` is string OR string[], `value` is bool|string|float, and
    // `children` recurses. MiniJSON hands back Dictionary<string,object>/List<object>/primitives,
    // which is exactly the shape to normalize into the strongly-shaped BittyNode once, here, so no
    // downstream layer has to branch on JSON shape again.
    //
    // Numbers arrive from MiniJSON as double or long; string/list accessors below coerce so a
    // token authored as 8 vs 8.0 vs "8" all read the same. A malformed file returns null and logs
    // — a bad view must fail to load (panel stays on NGUI), never half-build.
    public static class BittyParser {

        public static BittyNode Parse(string json) {

            if (string.IsNullOrEmpty(json)) {
                return null;
            }

            object root = null;

            try {
                root = Json.Deserialize(json);
            }
            catch (Exception e) {
                LogUtil.LogError("BittyParser: JSON parse failed: " + e.Message);
                return null;
            }

            Dictionary<string, object> dict = root as Dictionary<string, object>;

            if (dict == null) {
                LogUtil.LogError("BittyParser: root is not a JSON object");
                return null;
            }

            BittyNode node = NodeFromDict(dict, true);

            // Patterns are expanded at parse time so the builder only ever sees concrete elements.
            return BittyPatterns.Expand(node);
        }

        private static BittyNode NodeFromDict(Dictionary<string, object> d, bool isRoot) {

            BittyNode n = new BittyNode();

            n.type = Str(d, BittySchema.Keys.type) ?? BittySchema.Types.container;
            n.name = Str(d, BittySchema.Keys.name);

            // The root object names itself with "view" (schema D6), inner nodes with "name". Map
            // it so the invariant (name == element name == bind key) holds at the root too — the
            // panel's toolkitViewKey must resolve to this element.
            if (isRoot && string.IsNullOrEmpty(n.name)) {
                n.name = Str(d, "view");
            }

            AddClasses(n, d);

            n.text = Str(d, BittySchema.Keys.text);
            n.label = Str(d, BittySchema.Keys.label);
            n.style = Str(d, BittySchema.Keys.style);
            n.template = Str(d, BittySchema.Keys.template);
            n.pattern = Str(d, BittySchema.Keys.pattern);

            object rawValue = null;
            if (d.TryGetValue(BittySchema.Keys.value, out rawValue)) {
                n.value = rawValue;
            }

            n.choices = StrList(d, BittySchema.Keys.choices);
            n.bind = StrMap(d, BittySchema.Keys.bind);
            n.events = StrMap(d, BittySchema.Keys.events);

            object rawChildren = null;
            if (d.TryGetValue(BittySchema.Keys.children, out rawChildren)) {

                List<object> list = rawChildren as List<object>;

                if (list != null) {
                    for (int i = 0; i < list.Count; i++) {

                        Dictionary<string, object> child = list[i] as Dictionary<string, object>;

                        if (child != null) {
                            n.children.Add(NodeFromDict(child, false));
                        }
                    }
                }
            }

            return n;
        }

        // `class` accepts a single string or an array; normalize both to the node's class list.
        private static void AddClasses(BittyNode n, Dictionary<string, object> d) {

            object raw = null;

            if (!d.TryGetValue(BittySchema.Keys.cls, out raw) || raw == null) {
                return;
            }

            string single = raw as string;

            if (single != null) {

                if (!string.IsNullOrEmpty(single)) {
                    n.classes.Add(single);
                }

                return;
            }

            List<object> list = raw as List<object>;

            if (list != null) {
                for (int i = 0; i < list.Count; i++) {

                    string c = list[i] as string;

                    if (!string.IsNullOrEmpty(c)) {
                        n.classes.Add(c);
                    }
                }
            }
        }

        private static string Str(Dictionary<string, object> d, string key) {

            object raw = null;

            if (!d.TryGetValue(key, out raw) || raw == null) {
                return null;
            }

            return raw as string ?? raw.ToString();
        }

        private static List<string> StrList(Dictionary<string, object> d, string key) {

            object raw = null;

            if (!d.TryGetValue(key, out raw) || raw == null) {
                return null;
            }

            List<object> list = raw as List<object>;

            if (list == null) {
                return null;
            }

            List<string> result = new List<string>();

            for (int i = 0; i < list.Count; i++) {

                if (list[i] != null) {
                    result.Add(list[i].ToString());
                }
            }

            return result;
        }

        private static Dictionary<string, string> StrMap(Dictionary<string, object> d, string key) {

            object raw = null;

            if (!d.TryGetValue(key, out raw) || raw == null) {
                return null;
            }

            Dictionary<string, object> map = raw as Dictionary<string, object>;

            if (map == null) {
                return null;
            }

            Dictionary<string, string> result = new Dictionary<string, string>();

            foreach (KeyValuePair<string, object> pair in map) {

                if (pair.Value != null) {
                    result[pair.Key] = pair.Value.ToString();
                }
            }

            return result;
        }
    }
}
