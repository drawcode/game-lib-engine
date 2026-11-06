using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Engine.Game.Data;

namespace Engine.Game.App.BaseApp {

    // GAMEPLAY TRACKING: the player's small first-time and running markers -- a tutorial played, a
    // hint seen, an intro shown, a counter -- in their OWN profile blob ("tracking", next to mode,
    // rpg, ...), so the core profile and the mode blob don't grow with every one-off flag.
    //
    // Flat on purpose: one "<group>:<code>" key per marker, each value a bool, number or string. A
    // JSON load gives primitives back (bool, long or double, string), so every reader converts
    // instead of casting, and the file stays a plain, portable, hand-readable JSON object:
    //   { "tutorial:tutorial-attack": true, "hint:joystick": 3 }
    // Saved and loaded with the other blobs by BaseGameState (per profile slot); writers change it
    // in memory and the caller's usual GameState.SaveProfile writes it.
    public class GameProfileTracking : DataObject {

        // the blob type BaseGameState.getProfileTrackingKey passes to getKey
        public static string blobType = "tracking";

        private static volatile GameProfileTracking current;
        private static object syncRoot = new Object();

        public static GameProfileTracking Current {
            get {
                if (current == null) {
                    lock (syncRoot) {
                        if (current == null) {
                            current = new GameProfileTracking();
                        }
                    }
                }
                return current;
            }
            set {
                current = value;
            }
        }

        public static string Key(string group, string code) {
            return (group ?? "") + ":" + (code ?? "");
        }

        object Raw(string group, string code) {
            object raw;
            return TryGetValue(Key(group, code), out raw) ? raw : null;
        }

        // FLAGS: set means true, "true", or a non-zero number

        public bool IsSet(string group, string code) {

            object raw = Raw(group, code);

            if (raw == null) {
                return false;
            }

            if (raw is bool) {
                return (bool)raw;
            }

            string text = Convert.ToString(raw, CultureInfo.InvariantCulture).Trim().ToLowerInvariant();

            if (text == "true") {
                return true;
            }

            double number;
            return double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out number) && number != 0;
        }

        public void Mark(string group, string code) {
            SetFlag(group, code, true);
        }

        // an explicit false is kept (it overrides an older store a caller falls back to)
        public void SetFlag(string group, string code, bool value) {
            Set<object>(Key(group, code), value);
        }

        // whether anything is stored under the key: a marker, a number, a string or an item
        public bool Has(string group, string code) {
            return ContainsKey(Key(group, code));
        }

        // removes a marker or an item (an item's sidecar file goes at the next save)
        public void Clear(string group, string code) {
            if (IsSidecar(group, code)) {
                DropSidecar(SidecarName(group, code));
            }
            Remove(Key(group, code));
        }

        // NUMBERS: counters, stages, timestamps (unix seconds)

        public double GetNumber(string group, string code, double defaultValue = 0) {

            object raw = Raw(group, code);

            if (raw == null || raw is bool) {
                return defaultValue;
            }

            double number;
            return double.TryParse(Convert.ToString(raw, CultureInfo.InvariantCulture), NumberStyles.Float,
                CultureInfo.InvariantCulture, out number) ? number : defaultValue;
        }

        public void SetNumber(string group, string code, double value) {
            Set<object>(Key(group, code), value);
        }

        // adds to the stored number (0 when unset) and returns the new value
        public double Add(string group, string code, double by = 1) {
            double value = GetNumber(group, code) + by;
            SetNumber(group, code, value);
            return value;
        }

        // STRINGS

        public string GetString(string group, string code, string defaultValue = null) {
            object raw = Raw(group, code);
            return raw == null ? defaultValue : Convert.ToString(raw, CultureInfo.InvariantCulture);
        }

        public void SetString(string group, string code, string value) {
            Set<object>(Key(group, code), value);
        }

        // the codes stored under a group, in no particular order
        public List<string> Codes(string group) {

            List<string> codes = new List<string>();
            string prefix = Key(group, "");

            foreach (string key in Keys) {
                if (key.StartsWith(prefix, StringComparison.Ordinal)) {
                    codes.Add(key.Substring(prefix.Length));
                }
            }

            return codes;
        }

        // ITEMS: any object (a Dictionary, a List, a DataObject, a primitive) under a key, with a
        // TYPE TAG so a reader knows what it holds without guessing from the key.
        //   small, inline:  "arcade:stages": { "$type": "arcade-stages", "$value": { ... } }
        //   big, sidecar:   "arcade:stages": { "$type": "arcade-stages", "$sidecar": "arcade--stages" }
        //   the sidecar file "<tracking blob key>--arcade--stages" (.json, or .agsv in envelope mode):
        //                   { "$type": "arcade-stages", "$key": "arcade:stages", "$value": { ... } }
        // An item whose JSON is longer than sidecarThreshold characters goes to its sidecar, and it
        // comes back inline (the file deleted) when it shrinks. Readers take either form; a typed read
        // of another type gets nothing. Sidecars go through the same profile store as the blobs
        // (BaseGameState: same folder, envelope and flags, unchanged writes skipped), so they are as
        // portable as the profile. A value is copied in on SetItem: change it, then SetItem again.

        public const string typeField = "$type";
        public const string valueField = "$value";
        public const string sidecarField = "$sidecar";
        public const string keyField = "$key";

        public static int sidecarThreshold = 2048;

        // runtime only, never serialized (a Dictionary subclass writes its entries, not its fields)
        IProfileSidecarStore sidecarStore;
        readonly Dictionary<string, Dictionary<string, object>> sidecarDocs =
            new Dictionary<string, Dictionary<string, object>>();
        readonly HashSet<string> sidecarDirty = new HashSet<string>();
        readonly HashSet<string> sidecarDropped = new HashSet<string>();

        // BaseGameState hands its store over after each load; without one every item stays inline
        public void UseSidecarStore(IProfileSidecarStore store) {
            sidecarStore = store;
        }

        // file-safe and readable: "arcade:stages" -> "arcade--stages"
        public static string SidecarName(string group, string code) {
            StringBuilder name = new StringBuilder();
            foreach (char c in Key(group, code).ToLowerInvariant()) {
                if (c == ':') {
                    name.Append("--");
                }
                else if ((c >= 'a' && c <= 'z') || (c >= '0' && c <= '9') || c == '-' || c == '_') {
                    name.Append(c);
                }
                else {
                    name.Append('_');
                }
            }
            return name.ToString();
        }

        static string Str(IDictionary<string, object> dict, string key) {
            object raw;
            return dict != null && dict.TryGetValue(key, out raw) && raw != null
                ? Convert.ToString(raw, CultureInfo.InvariantCulture) : null;
        }

        IDictionary<string, object> ItemHeader(string group, string code) {
            IDictionary<string, object> header = Raw(group, code) as IDictionary<string, object>;
            return header != null && header.ContainsKey(typeField)
                && (header.ContainsKey(valueField) || header.ContainsKey(sidecarField)) ? header : null;
        }

        public bool HasItem(string group, string code) {
            return ItemHeader(group, code) != null;
        }

        public string ItemType(string group, string code) {
            return Str(ItemHeader(group, code), typeField);
        }

        public bool IsSidecar(string group, string code) {
            IDictionary<string, object> header = ItemHeader(group, code);
            return header != null && header.ContainsKey(sidecarField);
        }

        // The item's value: as written before a load; after one a Dictionary<string, object>,
        // List<object> or primitive. Null when absent, of another type (type given), or when its
        // sidecar is missing, unreadable or tagged for another type or key.
        public object GetItem(string group, string code, string type = null) {

            IDictionary<string, object> header = ItemHeader(group, code);

            if (header == null || (type != null && Str(header, typeField) != type)) {
                return null;
            }

            if (!header.ContainsKey(sidecarField)) {
                return header[valueField];
            }

            string name = Str(header, sidecarField);
            Dictionary<string, object> doc;

            if (name == null) {
                return null;
            }

            if (!sidecarDocs.TryGetValue(name, out doc)) {

                string json = sidecarStore != null ? sidecarStore.Load(name) : null;

                if (string.IsNullOrEmpty(json)) {
                    return null;
                }

                try {
                    doc = json.FromJson<Dictionary<string, object>>();
                }
                catch (Exception) {
                    return null;
                }

                if (doc == null || Str(doc, typeField) != Str(header, typeField)
                    || Str(doc, keyField) != Key(group, code)) {
                    return null;
                }

                sidecarDocs[name] = doc;
            }

            object value;
            return doc.TryGetValue(valueField, out value) ? value : null;
        }

        // The item as T: the value itself when it already is one, else a JSON round trip into T.
        public T GetItem<T>(string group, string code, string type) {

            object value = GetItem(group, code, type);

            if (value == null) {
                return default(T);
            }

            if (value is T) {
                return (T)value;
            }

            try {
                return value.ToJson(false).FromJson<T>();
            }
            catch (Exception) {
                return default(T);
            }
        }

        public void SetItem(string group, string code, string type, object value) {

            string name = SidecarName(group, code);
            string json = value == null ? null : value.ToJson(false);
            bool wasSidecar = IsSidecar(group, code);

            Dictionary<string, object> header = new Dictionary<string, object>();
            header[typeField] = type;

            if (sidecarStore != null && json != null && json.Length > sidecarThreshold) {

                Dictionary<string, object> doc = new Dictionary<string, object>();
                doc[typeField] = type;
                doc[keyField] = Key(group, code);
                doc[valueField] = value;

                sidecarDocs[name] = doc;
                sidecarDirty.Add(name);
                sidecarDropped.Remove(name);

                header[sidecarField] = name;
            }
            else {

                if (wasSidecar) {
                    DropSidecar(name);
                }

                header[valueField] = value;
            }

            Set<object>(Key(group, code), header);
        }

        void DropSidecar(string name) {
            sidecarDocs.Remove(name);
            sidecarDirty.Remove(name);
            sidecarDropped.Add(name);
        }

        // Called by BaseGameState right after it saves this blob: writes the sidecars changed since
        // the last save and deletes the dropped ones.
        public void SaveSidecars() {

            if (sidecarStore == null) {
                return;
            }

            foreach (string name in sidecarDropped) {
                sidecarStore.Delete(name);
            }

            sidecarDropped.Clear();

            foreach (string name in sidecarDirty) {
                Dictionary<string, object> doc;
                if (sidecarDocs.TryGetValue(name, out doc)) {
                    sidecarStore.Save(name, doc);
                }
            }

            sidecarDirty.Clear();
        }
    }

    // Where a tracking blob keeps its sidecars. Names are SidecarName()s; the store scopes them to the
    // blob (BaseGameState: "<tracking blob key>--<name>").
    public interface IProfileSidecarStore {
        string Load(string name);
        void Save(string name, object document);
        void Delete(string name);
    }
}
