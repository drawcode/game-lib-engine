using System;
using System.Collections.Generic;
using System.IO;
using Engine.Game.App.BaseApp;

namespace Engine.Game.App
{
    public class AppModeMeta : BaseAppModeMeta
    {
        //public static string appModeTypeGameDefault = "app-mode-game-default";
    }

    public class AppModes : BaseAppModes<AppMode>
    {

        private static volatile AppMode current;
        private static volatile AppModes instance;
        private static object syncRoot = new System.Object();
        public static string DATA_KEY = "app-mode-data";

        public static AppMode Current
        {

            get
            {
                if (current == null)
                {
                    lock (syncRoot)
                    {
                        if (current == null)
                            current = new AppMode();
                    }
                }

                return current;
            }

            set
            {
                current = value;
            }
        }

        public static AppModes Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new AppModes(true);
                    }
                }

                return instance;
            }
            set
            {
                instance = value;
            }
        }

        public AppModes()
        {
            Reset();
        }

        public AppModes(bool loadData)
        {
            Reset();
            path = "data/" + DATA_KEY + ".json";
            pathKey = DATA_KEY;
            LoadData();
        }
    }

    public class AppMode : BaseAppMode
    {

        // Attributes that are added or changed after launch should be like this to prevent
        // profile conversions.

        public AppMode()
        {
            Reset();
        }

        public override void Reset()
        {
            base.Reset();
        }
    }
}