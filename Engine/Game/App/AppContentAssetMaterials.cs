using System;
using System.Collections.Generic;
using System.IO;
using Engine.Game.App.BaseApp;
using Engine.Utility;

namespace Engine.Game.App
{
    public class AppContentAssetMaterials : BaseAppContentAssetMaterials<AppContentAssetMaterial>
    {
        private static volatile AppContentAssetMaterial current;
        private static volatile AppContentAssetMaterials instance;
        private static System.Object syncRoot = new System.Object();
        private string DATA_KEY = "app-content-asset-material-data";

        public static AppContentAssetMaterial Current
        {
            get
            {
                if (current == null)
                {
                    lock (syncRoot)
                    {
                        if (current == null)
                            current = new AppContentAssetMaterial();
                    }
                }

                return current;
            }
            set
            {
                current = value;
            }
        }

        public static AppContentAssetMaterials Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new AppContentAssetMaterials(true);
                    }
                }

                return instance;
            }
        }

        public AppContentAssetMaterials()
        {
            Reset();
        }

        public AppContentAssetMaterials(bool loadData)
        {
            Reset();
            path = "data/" + DATA_KEY + ".json";
            pathKey = DATA_KEY;
            LoadData();
        }
    }

    public partial class AppContentAssetMaterial : BaseAppContentAssetMaterial
    {

        public override void Reset()
        {
            base.Reset();
        }

    }
}