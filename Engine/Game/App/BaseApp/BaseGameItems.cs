using System;
using System.Collections.Generic;
using System.IO;
using Engine.Game.Data;

namespace Engine.Game.App.BaseApp {
    public class BaseGameItems<T> : DataObjects<T> where T : DataObject, new() {
        private static T current;
        private static volatile BaseGameItems<T> instance;
        private static object syncRoot = new Object();
        private string BASE_DATA_KEY = "game-item-data";

        public static T BaseCurrent {
            get {
                if (current == null) {
                    lock (syncRoot) {
                        if (current == null)
                            current = new T();
                    }
                }

                return current;
            }
            set {
                current = value;
            }
        }

        public static BaseGameItems<T> BaseInstance {
            get {
                if (instance == null) {
                    lock (syncRoot) {
                        if (instance == null)
                            instance = new BaseGameItems<T>(true);
                    }
                }

                return instance;
            }
            set {
                instance = value;
            }
        }

        public BaseGameItems() {
            Reset();
        }

        public BaseGameItems(bool loadData) {
            Reset();
            path = "data/" + BASE_DATA_KEY + ".json";
            pathKey = BASE_DATA_KEY;
            LoadData();
        }
    }

    public class BaseGameItem : GameDataObjectMeta {
        // Attributes that are added or changed after launch should be like this to prevent
        // profile conversions.

        public virtual string GetModelCode() {
            return data.GetModel(code).code;
        }

        public BaseGameItem() {
            Reset();
        }

        public override void Reset() {
            base.Reset();
        }

        public void Clone(BaseGameItem toCopy) {
            base.Clone(toCopy);
        }

        // ADDITIVE localization route (content, not UI strings). Item names reach migrated
        // (toolkitViewKey) screens through mission action text: AppContentCollectItem
        // .UpdateDisplayValues copies display_name into {{action_display_name}} ("Collect: 5
        // Action Coin"). Key convention: game_item_<code>_name / _desc. TrOrDefault falls back to
        // the raw English value whenever the key is absent, so any other game on this shared lib
        // that ships no such key sees its data unchanged. Key strings cached per `code`.
        private static readonly Dictionary<string, string> nameLocKeys = new Dictionary<string, string>();
        private static readonly Dictionary<string, string> descLocKeys = new Dictionary<string, string>();

        public override string display_name {
            get {
                return LocalizedField(base.display_name, nameLocKeys, "_name");
            }

            set {
                base.display_name = value;
            }
        }

        public override string description {
            get {
                return LocalizedField(base.description, descLocKeys, "_desc");
            }

            set {
                base.description = value;
            }
        }

        private string LocalizedField(string raw, Dictionary<string, string> keys, string suffix) {

            string c = code;

            if (string.IsNullOrEmpty(c) || string.IsNullOrEmpty(raw)) {
                return raw;
            }

            string key;

            if (!keys.TryGetValue(c, out key)) {
                key = "game_item_" + c.Replace('-', '_') + suffix;
                keys[c] = key;
            }

            return L10n.TrOrDefault(key, raw);
        }

        // Attributes that are added or changed after launch should be like this to prevent
        // profile conversions.
    }
}