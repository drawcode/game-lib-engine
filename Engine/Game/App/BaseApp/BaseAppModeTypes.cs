using System;
using System.Collections.Generic;
using System.IO;
using Engine.Game.Data;

namespace Engine.Game.App.BaseApp
{
    /*
    AppModesTypes
    app-mode-type-game-default
    app-mode-type-game-choice
    app-mode-type-game-collection
    app-mode-type-game-content
    app-mode-type-game-tips
    */

    public class BaseAppModeTypeMeta
    {
        public static string appModeTypeGameDefault = "app-mode-type-game-default";
        public static string appModeTypeGameChoice = "app-mode-type-game-choice";
        public static string appModeTypeGameCollection = "app-mode-type-game-collection";
        public static string appModeTypeGameContent = "app-mode-type-game-content";
        public static string appModeTypeGameTips = "app-mode-type-game-tips";
    }

    public class BaseAppModeTypes<T> : DataObjects<T> where T : DataObject, new()
    {
        private static T current;
        private static volatile BaseAppModeTypes<T> instance;
        private static object syncRoot = new Object();

        private string BASE_DATA_KEY = "app-mode-type-data";

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

        public static BaseAppModeTypes<T> BaseInstance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new BaseAppModeTypes<T>(true);
                    }
                }

                return instance;
            }
            set
            {
                instance = value;
            }
        }

        public BaseAppModeTypes()
        {
            Reset();
        }

        public BaseAppModeTypes(bool loadData)
        {
            Reset();
            path = "data/" + BASE_DATA_KEY + ".json";
            pathKey = BASE_DATA_KEY;
            LoadData();
        }

        public bool isAppModeTypeGameDefault
        {
            get
            {
                return IsAppModeType(AppModeTypeMeta.appModeTypeGameDefault);
            }
        }

        public bool isAppModeTypeGameChoice
        {
            get
            {
                return IsAppModeType(AppModeTypeMeta.appModeTypeGameChoice);
            }
        }

        public bool isAppModeTypeGameCollection
        {
            get
            {
                return IsAppModeType(AppModeTypeMeta.appModeTypeGameCollection);
            }
        }

        public bool isAppModeTypeGameContent
        {
            get
            {
                return IsAppModeType(AppModeTypeMeta.appModeTypeGameContent);
            }
        }

        public bool isAppModeTypeGameTips
        {
            get
            {
                return IsAppModeType(AppModeTypeMeta.appModeTypeGameTips);
            }
        }

        public bool IsAppModeType(string code)
        {
            if (AppModeTypes.Current.code == code)
            {
                return true;
            }
            return false;
        }

        public void ChangeState(string code)
        {
            if (AppModeTypes.Current.code != code)
            {
                AppModeType appModeType = AppModeTypes.Instance.GetByCode(code);

                if (appModeType != null)
                {
                    AppModeTypes.Current = appModeType;

                    GameProfiles.Current.SetCurrentAppModeType(code);

                    LogUtil.Log("AppModeTypes:ChangeState:code:" + AppModeTypes.Current.code);
                }
            }
        }
    }

    public class BaseAppModeType : GameDataObject
    {
        // Attributes that are added or changed after launch should be like this to prevent
        // profile conversions.

        public BaseAppModeType()
        {
            Reset();
        }

        public override void Reset()
        {
            base.Reset();
        }
    }
}