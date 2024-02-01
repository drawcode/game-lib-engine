using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Engine.Game.Data;

namespace Engine.Game.App.BaseApp
{
    public class BaseGameConfigs<T> : DataObjects<T> where T : DataObject, new()
    {
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

        public static T BaseCurrent
        {
            get
            {
                if (current == null)
                {
                    lock (syncRoot)
                    {
                        if (current == null)
                            current = new T();
                    }
                }

                return current;
            }
            set
            {
                current = value;
            }
        }

        public static BaseGameConfigs<T> BaseInstance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new BaseGameConfigs<T>(true);
                    }
                }

                return instance;
            }
            set
            {
                instance = value;
            }
        }

        public BaseGameConfigs()
        {
            Reset();
        }

        public BaseGameConfigs(bool loadData)
        {
            Reset();
            path = "data/" + BASE_DATA_KEY + ".json";
            pathKey = BASE_DATA_KEY;
            LoadData();
        }

        public static bool isGameRunning
        {
            get
            {

#if USE_GAME_LIB_GAMES

                if (GameController.IsGameRunning
                    && !isUIRunning)
                {
                    return true;
                }
#endif

                return false;
            }
        }

        public static bool isGamePaused
        {
            get
            {

#if USE_GAME_LIB_GAMES

                if (GameController.IsGamePaused
                    && !isUIRunning)
                {
                    return true;
                }

#endif
                return false;
            }
        }

        public static bool isGameContentDisplay
        {
            get
            {

#if USE_GAME_LIB_GAMES

                if (GameController.IsGameContentDisplay
                    && !isUIRunning)
                {
                    return true;
                }
#endif
                return false;
            }
        }

        public static bool isUIRunning
        {
            get
            {

#if USE_GAME_LIB_GAMES
#if USE_GAME_LIB_GAMES_UI
                if (GameUIController.Instance == null)
                {
                    return false;
                }

                if (GameUIController.Instance.uiVisible)
                {
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

    public class BaseGameConfig : Config
    {
        public virtual GameLeaderboardData data
        {
            get
            {
                return Get<GameLeaderboardData>(BaseDataObjectKeys.data);
            }

            set
            {
                Set(BaseDataObjectKeys.data, value);
            }
        }

        public BaseGameConfig()
        {
            Reset();
        }

        public override void Reset()
        {
            base.Reset();
        }
    }
}