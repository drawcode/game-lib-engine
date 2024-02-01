using System;
using System.Collections.Generic;
using System.IO;
using Engine.Game.App.BaseApp;

namespace Engine.Game.App
{
    public class AppContentTips : BaseAppContentTips<AppContentTip>
    {
        private static volatile AppContentTip current;
        private static volatile AppContentTips instance;
        private static object syncRoot = new System.Object();
        /*
        public static string APP_STATE_BOOKS = "app-state-books";
        public static string APP_STATE_CARDS = "app-state-cards";
        public static string APP_STATE_GAMES = "app-state-games";
        public static string APP_STATE_SETTINGS = "app-state-settings";
        public static string APP_STATE_TROPHIES = "app-state-trophies";
        */

        public static string DATA_KEY = "app-content-tip-data";

        public static AppContentTip Current
        {
            get
            {
                if (current == null)
                {
                    lock (syncRoot)
                    {
                        if (current == null)
                            current = new AppContentTip();
                    }
                }

                return current;
            }
            set
            {
                current = value;
            }
        }

        public static AppContentTips Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new AppContentTips(true);
                    }
                }

                return instance;
            }
            set
            {
                instance = value;
            }
        }

        public AppContentTips()
        {
            Reset();
            //ChangeState(APP_STATE_BOOKS);
        }

        public AppContentTips(bool loadData)
        {
            Reset();
            path = "data/" + DATA_KEY + ".json";
            pathKey = DATA_KEY;
            LoadData();
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

    public class AppContentTip : BaseAppContentTip
    {
        // Attributes that are added or changed after launch should be like this to prevent
        // profile conversions.

        public AppContentTip()
        {
            Reset();
        }

        public override void Reset()
        {
            base.Reset();
        }
    }
}