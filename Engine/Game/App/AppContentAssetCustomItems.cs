using System;
using System.Collections.Generic;
using System.IO;
using Engine.Game.App.BaseApp;
using Engine.Utility;

namespace Engine.Game.App
{
    public class AppContentAssetCustomItems : BaseAppContentAssetCustomItems<AppContentAssetCustomItem>
    {
        private static volatile AppContentAssetCustomItem current;
        private static volatile AppContentAssetCustomItems instance;
        private static System.Object syncRoot = new System.Object();
        private string DATA_KEY = "app-content-asset-custom-item-data";

        public static AppContentAssetCustomItem Current
        {
            get
            {
                if (current == null)
                {
                    lock (syncRoot)
                    {
                        if (current == null)
                            current = new AppContentAssetCustomItem();
                    }
                }

                return current;
            }
            set
            {
                current = value;
            }
        }

        public static AppContentAssetCustomItems Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new AppContentAssetCustomItems(true);
                    }
                }

                return instance;
            }
        }

        public AppContentAssetCustomItems()
        {
            Reset();
        }

        public AppContentAssetCustomItems(bool loadData)
        {
            Reset();
            path = "data/" + DATA_KEY + ".json";
            pathKey = DATA_KEY;
            LoadData();
        }
    }

    public partial class AppContentAssetCustomItem : BaseAppContentAssetCustomItem
    {

    }
}