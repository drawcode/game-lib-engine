using System;
using System.Collections.Generic;
using System.IO;

/*
AppModes
app-mode-game-default
app-mode-game-arcade
app-mode-game-challenge
app-mode-game-training
app-mode-game-matchup
app-mode-game-coop

*/

public class BaseAppModeMeta {
    public static string appModeGameDefault = "app-mode-game-default";
    public static string appModeGameArcade = "app-mode-game-arcade";
    public static string appModeGameChallenge = "app-mode-game-challenge";
    public static string appModeGameTraining = "app-mode-game-training";
    public static string appModeGameMatchup = "app-mode-game-matchup";
    public static string appModeGameCoop = "app-mode-game-coop";
}

public class BaseAppModes<T> : DataObjects<T> where T : new() {
    private static T current;
    private static volatile BaseAppModes<T> instance;
    private static object syncRoot = new Object();

    private string BASE_DATA_KEY = "app-mode-data";

    public static T BaseCurrent {
        get {
            if (current == null) {
                lock (syncRoot) {
                    if (current == null)
                        current = new T();
                }
            }

            return current;
        }
        set {
            current = value;
        }
    }

    public static BaseAppModes<T> BaseInstance {
        get {
            if (instance == null) {
                lock (syncRoot) {
                    if (instance == null)
                        instance = new BaseAppModes<T>(true);
                }
            }

            return instance;
        }
        set {
            instance = value;
        }
    }

    public BaseAppModes() {
        Reset();
    }

    public BaseAppModes(bool loadData) {
        Reset();
        path = "data/" + BASE_DATA_KEY + ".json";
        pathKey = BASE_DATA_KEY;
        LoadData();
    }

    public bool isAppModeGameDefault {
        get {
            return IsAppMode(AppModeMeta.appModeGameDefault);
        }
    }

    public bool isAppModeGameArcade {
        get {
            return IsAppMode(AppModeMeta.appModeGameArcade);
        }
    }

    public bool isAppModeGameChallenge {
        get {
            return IsAppMode(AppModeMeta.appModeGameChallenge);
        }
    }

    public bool isAppModeGameTraining {
        get {
            return IsAppMode(AppModeMeta.appModeGameTraining);
        }
    }

    public bool isAppModeGameMatchup {
        get {
            return IsAppMode(AppModeMeta.appModeGameMatchup);
        }
    }

    public bool isAppModeGameCoop {
        get {
            return IsAppMode(AppModeMeta.appModeGameCoop);
        }
    }

    public bool IsAppMode(string code) {
        if(AppModes.Current.code == code) {
            return true;
        }
        return false;
    }

    public void ChangeState(string code) {

        if(AppModes.Current.code != code) {

            AppMode appMode = AppModes.Instance.GetByCode(code);

            if(appMode != null) {
                AppModes.Current = appMode;

                GameProfiles.Current.SetCurrentAppMode(code);

                LogUtil.Log("AppModes:ChangeState:code:" + AppModes.Current.code);
            }
        }
    }
}

public class BaseAppMode : GameDataObject {

    // Attributes that are added or changed after launch should be like this to prevent
    // profile conversions.

    public BaseAppMode() {
        Reset();
    }

    public override void Reset() {
        base.Reset();
    }
}