using System;
using System.Collections.Generic;
using System.IO;
using Engine.Game.App.BaseApp;

namespace Engine.Game.App
{
    public class AppContentAssets : BaseAppContentAssets<AppContentAsset>
    {
        private static volatile AppContentAsset current;
        private static volatile AppContentAssets instance;
        private static object syncRoot = new System.Object();
        /*
        public static string APP_STATE_BOOKS = "app-state-books";
        public static string APP_STATE_CARDS = "app-state-cards";
        public static string APP_STATE_GAMES = "app-state-games";
        public static string APP_STATE_SETTINGS = "app-state-settings";
        public static string APP_STATE_TROPHIES = "app-state-trophies";
        */

        public static string DATA_KEY = "app-content-asset-data";

        public static AppContentAsset Current
        {
            get
            {
                if (current == null)
                {
                    lock (syncRoot)
                    {
                        if (current == null)
                            current = new AppContentAsset();
                    }
                }

                return current;
            }
            set
            {
                current = value;
            }
        }

        public static AppContentAssets Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new AppContentAssets(true);
                    }
                }

                return instance;
            }
            set
            {
                instance = value;
            }
        }

        public AppContentAssets()
        {
            Reset();
            //ChangeState(APP_STATE_BOOKS);
        }

        public AppContentAssets(bool loadData)
        {
            Reset();
            path = "data/" + DATA_KEY + ".json";
            pathKey = DATA_KEY;
            LoadData();
        }

        public void ChangeState(string code)
        {
            if (Current.code != code)
            {
                Current = GetById(code);
            }
        }

        /*
        public List<AppContentAsset> GetListByPack(string packCode) {
            List<AppContentAsset> filteredList = new List<AppContentAsset>();
            foreach(AppContentAsset obj in GetAll()) {
                if(packCode.ToLower() == obj.pack_code.ToLower()) {
                    filteredList.Add(obj);
                }
            }

            return filteredList;
        }
        */

        public List<AppContentAsset> GetListByCodeAndPackCode(string assetCode, string packCode)
        {
            List<AppContentAsset> filteredList = new List<AppContentAsset>();
            foreach (AppContentAsset obj in GetListByPack(packCode))
            {
                if (assetCode.ToLower() == obj.code.ToLower())
                {
                    filteredList.Add(obj);
                }
            }

            return filteredList;
        }

        public string GetAppContentAssetContentPath(string packCode, string asset, bool versioned)
        {
            string packPath = PathUtil.Combine(
                ContentPaths.appCachePathSharedPacks,
                packCode);
            string packPathContent = PathUtil.Combine(
                packPath,
                ContentConfig.currentContentContent);

            string file = "";
            AppContentAsset appContentAsset = AppContentAssets.Instance.GetById(asset);
            if (appContentAsset != null)
            {
                file = appContentAsset.code
                    + "."
                    + appContentAsset.GetVersionFileExt();
            }

            string fullPath = PathUtil.Combine(packPathContent, file);
            if (versioned)
            {
                fullPath = Contents.GetFullPathVersioned(fullPath);
            }
            return fullPath;
        }


        /*
        public void ChangeState(AppState app_stateTo) {
            if(lastAppState != app_stateTo) {
                app_state = app_stateTo;
                HandleStateChange();
            }
        }

        public void HandleStateChange() {
            if(app_state == AppState.StateNotSet) {
                OnAppStateNotStarted();
            }
            else if(app_state == AppState.StateBook) {
                OnAppStateBooks();
            }
            else if(app_state == AppState.StateCards) {
                OnAppStateCards();
            }
            else if(app_state == AppState.StateGames) {
                OnAppStateGames();
            }
        }

        public void OnAppStateNotStarted() {
            app_state = AppState.StateNotSet;
            appSubState = AppSubState.StateSubEnvironment;

            Messenger<AppState, AppSubState>.Broadcast(AppViewerAppControllerMessages.StateNotSet, app_state, appSubState);
        }

        public void OnAppStateBooks() {
            app_state = AppState.StateBook;
            appSubState = AppSubState.StateSubEnvironment;

            currentApp = new App();
            currentApp.appCode = "default";

            Messenger<AppState, AppSubState>.Broadcast(AppViewerAppControllerMessages.StateBooks, app_state, appSubState);
        }

        public void OnAppStateCards() {
            app_state = AppState.StateCards;
            appSubState = AppSubState.StateSubEnvironment;

            currentApp = new App();
            currentApp.appCode = "default";

            Messenger<AppState, AppSubState>.Broadcast(AppViewerAppControllerMessages.StateCards, app_state, appSubState);
        }

        public void OnAppStateGames() {
            app_state = AppState.StateCards;
            appSubState = AppSubState.StateSubEnvironment;

            currentApp = new App();
            currentApp.appCode = "default";

            Messenger<AppState, AppSubState>.Broadcast(AppViewerAppControllerMessages.StateGames, app_state, appSubState);
        }
        */
    }

    public class AppContentAsset : BaseAppContentAsset
    {

        // Attributes that are added or changed after launch should be like this to prevent
        // profile conversions.

        public AppContentAsset()
        {
            Reset();
        }

        public override void Reset()
        {
            base.Reset();
            keys = new List<string>();
            content_attributes = new Dictionary<string, object>();
        }

        public string GetContentString(string key)
        {
            string content = "";
            if (content_attributes.ContainsKey(key))
            {
                content = content_attributes[key].ToString();
            }
            return content;
        }

        public string GetVersion()
        {
            return GetContentString(AppContentAssetAttributes.version);
        }

        public string GetVersionFileIncrement()
        {
            return GetContentString(AppContentAssetAttributes.version_file_increment);
        }

        public string GetVersionRequiredApp()
        {
            return GetContentString(AppContentAssetAttributes.version_required_app);
        }

        public string GetVersionType()
        {
            return GetContentString(AppContentAssetAttributes.version_type);
        }

        public string GetVersionFileType()
        {
            return GetContentString(AppContentAssetAttributes.version_file_type);
        }

        public string GetVersionFileExt()
        {
            return GetContentString(AppContentAssetAttributes.version_file_ext);
        }

    }
}