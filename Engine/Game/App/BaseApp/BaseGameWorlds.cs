using System;
using System.Collections.Generic;
using System.IO;
using Engine.Utility;
using Engine.Game.Data;

namespace Engine.Game.App.BaseApp {
    public class BaseGameWorlds<T> : DataObjects<T> where T : DataObject, new() {
        private static T current;
        private static volatile BaseGameWorlds<T> instance;
        private static object syncRoot = new Object();
        public static string BASE_DATA_KEY = "game-world-data";

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

        public static BaseGameWorlds<T> BaseInstance {
            get {
                if (instance == null) {
                    lock (syncRoot) {
                        if (instance == null)
                            instance = new BaseGameWorlds<T>(true);
                    }
                }

                return instance;
            }
        }

        public BaseGameWorlds() {
            Reset();
        }

        public BaseGameWorlds(bool loadData) {
            Reset();
            path = "data/" + BASE_DATA_KEY + ".json";
            pathKey = BASE_DATA_KEY;
            LoadData();
        }

        public override void LoadState() {
            base.LoadState();

            if (!currentStateCode.IsNullOrEmpty()) {
                ChangeCurrent(currentStateCode);
            }
        }

        public void ChangeCurrentAbsolute(string code) {
            //GameWorlds.Current.code = "changeme";
            ChangeCurrent(code);
        }

        /*
        public override void ChangeCurrent(string code) {

            if (GameWorlds.Current.code != code) {
                        UnityEngine.Debug.Log("BaseGameWorlds:ChangeCurrent:" + code);

                GameWorld world = GameWorlds.Instance.GetById(code); 
                //string originalCode = code;
                if (!string.IsNullOrEmpty(world.code)) {
                    GameWorlds.Current = world;
                }

                if(GameWorlds.Current != null) {
                    //GameProfiles.Current.SetCurrentGameWorld(GameWorlds.Current.code);
                    //GameState.SaveProfile();
                    base.ChangeCurrent(code);
                }
            }
        }
        */

#if USE_GAME_LIB_GAMES
        public GameWorld GetByWorldNum(int worldNum) {
            foreach (GameWorld item in GameWorlds.Instance.GetAll()) {
                if (item.data.world_num == worldNum) {
                    return item;
                }
            }
            return null;
        }
#endif
    }

    public class BaseGameWorld : GameDataObject {
        // Attributes that are added or changed after launch should be like this to prevent
        // profile conversions.

        public virtual GameDataObjectItem data {
            get {
                return Get<GameDataObjectItem>(BaseDataObjectKeys.data);
            }

            set {
                Set<GameDataObjectItem>(BaseDataObjectKeys.data, value);
            }
        }

        public BaseGameWorld() {
            Reset();
        }

        public override void Reset() {
            base.Reset();
        }

        public void Clone(BaseGameWorld toCopy) {
            base.Clone(toCopy);
        }

        // Attributes that are added or changed after launch should be like this to prevent
        // profile conversions.

        // ADDITIVE localization route (content, not UI strings). Read by
        // BaseGameUIPanelWorlds.UpdateMetaLabels/BaseGameUIPanelResults.GetWorldCodeDisplay on
        // migrated (toolkitViewKey) screens. Only `description` is keyed -- `display_name` is a
        // proper noun ("Planet Z: Zedlands") and stays English per the localization standard's
        // glossary, so it is deliberately left un-overridden. Key convention:
        // game_world_<code>_desc. TrOrDefault falls back to the raw English value whenever the
        // key is absent, so any other game on this shared lib that ships no such key sees its
        // data unchanged. Key string cached per `code`, not rebuilt per call -- same pattern as
        // BaseGameUIPanelHeader.TitleKey().
        private static readonly Dictionary<string, string> descLocKeys = new Dictionary<string, string>();

        public override string description {
            get {
                string raw = base.description;
                string c = code;

                if (string.IsNullOrEmpty(c)) {
                    return raw;
                }

                string key;

                if (!descLocKeys.TryGetValue(c, out key)) {
                    key = "game_world_" + c.Replace('-', '_') + "_desc";
                    descLocKeys[c] = key;
                }

                return L10n.TrOrDefault(key, raw);
            }
        }
    }
}