using System;
using System.Collections.Generic;
using System.IO;
using Engine.Content;
using Engine.Game.Data;
using Engine.Utility;

namespace Engine.Game.App
{
    public class GamePacks : DataObjects<GamePack>
    {
        private static volatile GamePack current;
        private static volatile GamePacks instance;
        private static object syncRoot = new System.Object();
        public static string DATA_KEY = "pack-data";

#if UNITY_IPHONE
        public static string currentPacksPlatform = "ios";
#elif UNITY_ANDROID
    public static string currentPacksPlatform = "android";
#else
    public static string currentPacksPlatform = "web";
#endif

#if UNITY_IPHONE
        public static uint currentPacksIncrement = 3007; // Used for unity cache
#elif UNITY_ANDROID
    public static uint currentPacksIncrement = 1001; // Used for unity cache
#else
    public static uint currentPacksIncrement = 50; // Used for unity cache
#endif

        public static string currentPacksVersion = "1.1"; // used for actual version and on dlc storage
        public static string currentPacksGame = "game-[app]";
        public static string currentGameBundle = "com.[app].[app]";

        public static string currentPackPath = "";
        public static string currentPacksPath = "";
        public static string currentPackBundlePath = "";
        public static string currentPackDataPath = "";
        public static string currentPackScenesPath = "";

        public static GamePack Current
        {
            get
            {
                if (current == null)
                {
                    lock (syncRoot)
                    {
                        if (current == null)
                            current = new GamePack();
                    }
                }

                return current;
            }
            set
            {
                current = value;
            }
        }

        public static GamePacks Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new GamePacks(true);
                    }
                }

                return instance;
            }
        }

        public GamePacks()
        {
            Reset();
        }

        public GamePacks(bool loadData)
        {
            Reset();
            path = "data/" + DATA_KEY + ".json";
            pathKey = DATA_KEY;
            LoadData();
        }

        public void InitPackPaths()
        {

            currentPacksPath = Path.Combine(ContentPaths.appCachePlatformPath, ContentConfig.contentCachePacks);
            currentPackPath = Path.Combine(currentPacksPath, GamePacks.Current.code);
            currentPackBundlePath = Path.Combine(currentPackPath, ContentConfig.contentCacheAssetBundles);//"packs/" + pack + "/" + assetName + ".unity3d");
            currentPackDataPath = Path.Combine(currentPackPath, ContentConfig.contentCacheData);
            currentPackScenesPath = Path.Combine(currentPackPath, ContentConfig.contentCacheScenes);
        }

        public void ChangeCurrentGamePack(string code)
        {
            Current = GetById(code);
            LogUtil.Log("Changing Pack: code:" + code);
            LogUtil.Log("Changing Pack: name:" + Current.name);

            InitPackPaths();
        }

        public bool OwnsAllPacks()
        {
            foreach (GamePack pack in GetAll())
            {
                //if(pack.code != GameProducts.ITEM_PACK_SX_2012_ALL
                //  && pack.code != GamePacks.PACK_DEFAULT) {
                if (!Contents.CheckGlobalContentAccess(pack.code))
                {
                    return false;
                }
                //}
            }
            return true;
        }

        public bool OwnsAtLeastOnePack()
        {
            foreach (GamePack pack in GetAll())
            {
                //if(pack.code != GameProducts.ITEM_PACK_SX_2012_ALL
                //  && pack.code != GamePacks.PACK_DEFAULT) {
                if (Contents.CheckGlobalContentAccess(pack.code))
                {
                    return true;
                }
                //}
            }
            return false;
        }
    }

    public class GamePack : GameDataObject
    {

        // Attributes that are added or changed after launch should be like this to prevent
        // profile conversions.

        public GamePack()
        {
            Reset();
        }

        public override void Reset()
        {
            base.Reset();
        }

        public void Clone(GamePack toCopy)
        {

            base.Clone(toCopy);
        }

        // Attributes that are added or changed after launch should be like this to prevent
        // profile conversions.

        /*
        public double GetInitialDifficulty() {
            return GetAttributeDoubleValue(GameLevelKeys.LEVEL_INITIAL_DIFFICULTY);
        }

        public void SetInitialDifficulty(double val) {
            SetAttributeDoubleValue(GameLevelKeys.LEVEL_INITIAL_DIFFICULTY, val);
        }
        */

    }
}