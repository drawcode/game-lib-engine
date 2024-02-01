using System;
using System.Collections.Generic;
using System.IO;
using Engine.Game.App.BaseApp;

namespace Engine.Game.App
{
    public class AppContentChoices : BaseAppContentChoices<AppContentChoice>
    {
        private static volatile AppContentChoice current;
        private static volatile AppContentChoices instance;
        private static object syncRoot = new System.Object();
        /*
        public static string APP_STATE_BOOKS = "app-state-books";
        public static string APP_STATE_CARDS = "app-state-cards";
        public static string APP_STATE_GAMES = "app-state-games";
        public static string APP_STATE_SETTINGS = "app-state-settings";
        public static string APP_STATE_TROPHIES = "app-state-trophies";
        */

        public static string DATA_KEY = "app-content-choice-data";

        public static AppContentChoice Current
        {
            get
            {
                if (current == null)
                {
                    lock (syncRoot)
                    {
                        if (current == null)
                            current = new AppContentChoice();
                    }
                }

                return current;
            }
            set
            {
                current = value;
            }
        }

        public static AppContentChoices Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new AppContentChoices(true);
                    }
                }

                return instance;
            }
            set
            {
                instance = value;
            }
        }

        public AppContentChoices()
        {
            Reset();
            //ChangeState(APP_STATE_BOOKS);
        }

        public AppContentChoices(bool loadData)
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

    public class AppContentChoice : BaseAppContentChoice
    {
        // Attributes that are added or changed after launch should be like this to prevent
        // profile conversions.

        public AppContentChoice()
        {
            Reset();
        }

        public override void Reset()
        {
            base.Reset();
        }
    }
}