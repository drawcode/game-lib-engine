using System;
using System.Collections.Generic;
using System.IO;

using Engine.Game.App.BaseApp;
using Engine.Utility;

using UnityEngine;

namespace Engine.Game.App
{
    public class AppColorPresets : BaseAppColorPresets<AppColorPreset>
    {
        private static volatile AppColorPreset current;
        private static volatile AppColorPresets instance;
        private static System.Object syncRoot = new System.Object();
        private string DATA_KEY = "app-color-preset-data";

        public static AppColorPreset Current
        {
            get
            {
                if (current == null)
                {
                    lock (syncRoot)
                    {
                        if (current == null)
                            current = new AppColorPreset();
                    }
                }

                return current;
            }
            set
            {
                current = value;
            }
        }

        public static AppColorPresets Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new AppColorPresets(true);
                    }
                }

                return instance;
            }
        }

        public AppColorPresets()
        {
            Reset();
        }

        public AppColorPresets(bool loadData)
        {
            Reset();
            path = "data/" + DATA_KEY + ".json";
            pathKey = DATA_KEY;
            LoadData();
        }
    }

    // ----------------------------------------------------------------------------
    // OVERRIDE TO CUSTOMIZE 

    public partial class AppColorPreset : BaseAppColorPreset
    {

    }
}