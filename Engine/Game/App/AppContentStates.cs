using System;
using System.Collections.Generic;
using System.IO;

using Engine.Events;
using Engine.Data.Json;

/*
AppContentStates
app-content-state-game-arcade
app-content-state-game-challenge
app-content-state-game-training-choice-quiz
app-content-state-game-training-collection-safety
app-content-state-game-training-collection-smarts
app-content-state-game-training-content
app-content-state-game-training-tips

*/

public class AppContentStateMeta : BaseAppContentStateMeta {
    //public static string appModeTypeGameDefault = "app-mode-game-default";
    public static string appContentStateGameTrainingChoiceQuiz = "app-content-state-game-training-choice-quiz";
    public static string appContentStateGameTrainingCollectionSafety = "app-content-state-game-training-collection-safety";
    public static string appContentStateGameTrainingCollectionSmarts = "app-content-state-game-training-collection-smarts";


    public static string appContentStateGameAR = "app-content-state-game-ar";
    public static string appContentStateGameARSettings = "app-content-state-game-ar-settings";
    public static string appContentStateGameVR = "app-content-state-game-vr";
}

public class AppContentStates : BaseAppContentStates<AppContentState> {
    private static volatile AppContentState current;
    private static volatile AppContentStates instance;
    private static object syncRoot = new System.Object();
    /*
    public static string APP_STATE_BOOKS = "app-state-books";
    public static string APP_STATE_CARDS = "app-state-cards";
    public static string APP_STATE_GAMES = "app-state-games";
    public static string APP_STATE_SETTINGS = "app-state-settings";
    public static string APP_STATE_TROPHIES = "app-state-trophies";
    */
    
    public static string DATA_KEY = "app-content-state-data";
        
    public static AppContentState Current {
        get {
            if (current == null) {
                lock (syncRoot) {
                    if (current == null) 
                        current = new AppContentState();
                }
            }
            
            return current;
        }
        set {
            current = value;
        }
    }
        
    public static AppContentStates Instance {
        get {
            if (instance == null) {
                lock (syncRoot) {
                    if (instance == null) 
                        instance = new AppContentStates(true);
                }
            }

            LogUtil.Log("AppContentStates:Instanceget:");
            
            return instance;
        }
        set {
            instance = value;
        }
    }
            
    public AppContentStates() {
        Reset();
        //ChangeState(APP_STATE_BOOKS);
    }
    
    public AppContentStates(bool loadData) {
        Reset();
        path = "data/" + DATA_KEY + ".json";
        pathKey = DATA_KEY;
        LoadData();
    }

    public bool isAppContentStateGameTrainingChoiceQuiz {
        get {
            return IsAppContentState(AppContentStateMeta.appContentStateGameTrainingChoiceQuiz);
        }
    }

    public bool isAppContentStateGameTrainingCollectionSafety {
        get {
            return IsAppContentState(AppContentStateMeta.appContentStateGameTrainingCollectionSafety);
        }
    }

    public bool isAppContentStateGameTrainingCollectionSmarts {
        get {
            return IsAppContentState(AppContentStateMeta.appContentStateGameTrainingCollectionSmarts);
        }
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

public class AppContentState : BaseAppContentState {
        
    // Attributes that are added or changed after launch should be like this to prevent
    // profile conversions.
            
    public AppContentState() {
        Reset();
    }
    
    public override void Reset() {
        base.Reset();
    }

    
}
