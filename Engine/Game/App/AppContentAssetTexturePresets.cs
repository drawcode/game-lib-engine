using System;
using System.Collections.Generic;
using System.IO;
using Engine.Game.App.BaseApp;
using Engine.Utility;

namespace Engine.Game.App
{
    public class AppContentAssetTexturePresets : BaseAppContentAssetTexturePresets<AppContentAssetTexturePreset>
    {

        private static volatile AppContentAssetTexturePreset current;
        private static volatile AppContentAssetTexturePresets instance;
        private static System.Object syncRoot = new System.Object();
        private string DATA_KEY = "app-content-asset-texture-preset-data";

        public static AppContentAssetTexturePreset Current
        {
            get
            {
                if (current == null)
                {
                    lock (syncRoot)
                    {
                        if (current == null)
                            current = new AppContentAssetTexturePreset();
                    }
                }

                return current;
            }
            set
            {
                current = value;
            }
        }

        public static AppContentAssetTexturePresets Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new AppContentAssetTexturePresets(true);
                    }
                }

                return instance;
            }
        }

        public AppContentAssetTexturePresets()
        {
            Reset();
        }

        public AppContentAssetTexturePresets(bool loadData)
        {
            Reset();
            path = "data/" + DATA_KEY + ".json";
            pathKey = DATA_KEY;
            LoadData();
        }

        public static List<AppContentAssetTexturePreset> All
        {
            get
            {
                return GetAllItems();
            }
        }

        public static List<AppContentAssetTexturePreset> GetAllItems()
        {
            return Instance.GetAll();
        }
    }

    public class AppContentAssetTexturePreset : BaseAppContentAssetTexturePreset
    {

        public AppContentAssetTexturePreset()
        {
            Reset();
        }

        public override void Reset()
        {

        }

    }
}