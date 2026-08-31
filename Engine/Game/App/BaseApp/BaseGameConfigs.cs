using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Engine.Game.Data;

namespace Engine.Game.App.BaseApp {

    // OFF-SCREEN EDGE INDICATOR DIALS
    //
    // Deliberately NOT on BaseGameConfigs<T>: that class is generic, so its statics are only
    // reachable through a closed type (the app's `GameConfigs`), which the engine lib cannot
    // name. BaseGameProfile lives here and needs the same numbers for its slider bounds, so
    // they sit in a non-generic class both sides can see.
    //
    // `scale` multiplies the distance-derived size in BaseGamePlayerIndicator.ScaleIndicator.
    // It ships at .9 -- the 10% shrink asked for on device -- and Settings: Controls writes the
    // player's own value over it from ATT_CONTROL_INDICATOR_SCALE.
    //
    // `edgeBorderScale` multiplies the indicator prefab's authored `clampBorderSize` (90 design
    // units) rather than replacing it, so the authored margin stays the one place that number
    // is written down. .5 puts them at 45 units. 90 was worst on the vertical axis: the visible
    // area measures +/-692.5 x +/-320 container units, so it inset the top and bottom dots by
    // 28% of the half-height while the sides sat at 13% -- which is why only SOME of them read
    // as far from the edge.
    public static class GameIndicatorConfigs {
        public static float scale = .9f;
        public static float scaleMin = .5f;
        public static float scaleMax = 1.5f;

        public static float edgeBorderScale = .5f;
    }

    public class BaseGameConfigs<T> : DataObjects<T> where T : DataObject, new() {
        private static T current;
        private static volatile BaseGameConfigs<T> instance;
        private static object syncRoot = new Object();

        public static string BASE_DATA_KEY = "game-config-data";

        public static string MULTIPLAYER_GAME_CODE = "default";
        public static string MULTIPLAYER_GAME_TYPE = "default";

        public static string defaultGameLevelCode = "1-1";
        public static string defaultGameWorldCode = "default";

        public static bool usePooledGamePlayers = true;
        public static bool usePooledIndicators = false;
        public static bool usePooledProjectiles = true;
        public static bool usePooledItems = true;
        public static bool usePooledLevelItems = true;

        public static bool useShadowStatic = true;

        public static bool globalReady = false;

        public static bool useCoinRewardsForAchievements = true;
        public static double coinRewardAchievementPoint = 50;

        public static bool useNetworking = false;

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

        public static BaseGameConfigs<T> BaseInstance {
            get {
                if (instance == null) {
                    lock (syncRoot) {
                        if (instance == null)
                            instance = new BaseGameConfigs<T>(true);
                    }
                }

                return instance;
            }
            set {
                instance = value;
            }
        }

        public BaseGameConfigs() {
            Reset();
        }

        public BaseGameConfigs(bool loadData) {
            Reset();
            path = "data/" + BASE_DATA_KEY + ".json";
            pathKey = BASE_DATA_KEY;
            LoadData();
        }

        public static bool isGameRunning {
            get {

#if USE_GAME_LIB_GAMES

                if (GameController.IsGameRunning
                    && !isUIRunning) {
                    return true;
                }
#endif

                return false;
            }
        }

        public static bool isGamePaused {
            get {

#if USE_GAME_LIB_GAMES

                if (GameController.IsGamePaused
                    && !isUIRunning) {
                    return true;
                }

#endif
                return false;
            }
        }

        public static bool isGameContentDisplay {
            get {

#if USE_GAME_LIB_GAMES

                if (GameController.IsGameContentDisplay
                    && !isUIRunning) {
                    return true;
                }
#endif
                return false;
            }
        }

        public static bool isUIRunning {
            get {

#if USE_GAME_LIB_GAMES
#if USE_GAME_LIB_GAMES_UI
                if (GameUIController.Instance == null) {
                    return false;
                }

                if (GameUIController.Instance.uiVisible) {
                    return true;
                }
#endif
#endif
                return false;
            }
        }
    }


    /*
    public class GameStringsData : GameDataObject {

        public virtual string datatype {
            get {
                return Get<string>(BaseDataObjectKeys.datatype);
            }

            set {
                Set(BaseDataObjectKeys.datatype, value);
            }
        }

        public virtual string direction {
            get {
                return Get<string>(BaseDataObjectKeys.direction);
            }

            set {
                Set(BaseDataObjectKeys.direction, value);
            }
        }

        public virtual List<GameNetworkData> networks {
            get {
                return Get<List<GameNetworkData>>(BaseDataObjectKeys.networks);
            }

            set {
                Set(BaseDataObjectKeys.networks, value);
            }
        }
    }
    */

    public class BaseGameConfig : Config {
        public virtual GameLeaderboardData data {
            get {
                return Get<GameLeaderboardData>(BaseDataObjectKeys.data);
            }

            set {
                Set(BaseDataObjectKeys.data, value);
            }
        }

        public BaseGameConfig() {
            Reset();
        }

        public override void Reset() {
            base.Reset();
        }
    }
}