using System;
using System.Collections.Generic;
using System.IO;
using Engine.Game.Data;

namespace Engine.Game.App.BaseApp
{
    public class BaseAppStateMeta
    {
        //public static string appModeTypeGameDefault = "app-mode-game-default";
        public static string appStateGame = "app-state-game";
    }

    public class BaseAppStates<T> : DataObjects<T> where T : DataObject, new()
    {
        private static T current;
        private static volatile BaseAppStates<T> instance;
        private static object syncRoot = new Object();
        private string BASE_DATA_KEY = "app-state-data";

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

        public static BaseAppStates<T> BaseInstance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new BaseAppStates<T>(true);
                    }
                }

                return instance;
            }
            set
            {
                instance = value;
            }
        }

        public BaseAppStates()
        {
            Reset();
        }

        public BaseAppStates(bool loadData)
        {
            Reset();
            path = "data/" + BASE_DATA_KEY + ".json";
            pathKey = BASE_DATA_KEY;
            LoadData();
        }

        public bool isAppStateGame
        {
            get
            {
                return IsAppState(AppStateMeta.appStateGame);
            }
        }

        public bool IsAppState(string code)
        {
            if (AppStates.Current.code == code)
            {
                return true;
            }
            return false;
        }

        public void ChangeState(string code)
        {
            if (AppStates.Current.code != code)
            {
                AppState app_state = AppStates.Instance.GetByCode(code);

                if (app_state != null)
                {
                    AppStates.Current = app_state;

                    GameProfiles.Current.SetCurrentAppState(code);

                    LogUtil.Log("AppStates:ChangeState:code:" + AppStates.Current.code);
                }
            }
        }
    }

    public class BaseAppState : GameDataObject
    {
        // Attributes that are added or changed after launch should be like this to prevent
        // profile conversions.

        public BaseAppState()
        {
            Reset();
        }

        public override void Reset()
        {
            base.Reset();
        }
    }
}