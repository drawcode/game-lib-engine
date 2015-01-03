using System;
using System.Collections.Generic;
using System.IO;

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

public class BaseAppContentStateMeta {
    //public static string appModeTypeGameDefault = "app-mode-game-default";
    public static string appContentStateGameArcade = "app-content-state-game-arcade";
    public static string appContentStateGameChallenge = "app-content-state-game-challenge";
    public static string appContentStateGameMissions = "app-content-state-game-missions";
    public static string appContentStateGameCoop = "app-content-state-game-coop";
    public static string appContentStateGameMultiplayerCoop = "app-content-state-game-mulitiplayer-coop";
    public static string appContentStateGameMultiplayerMatchup = "app-content-state-game-mulitiplayer-matchup";
    public static string appContentStateGameMultiplayer = "app-content-state-game-mulitiplayer";
    public static string appContentStateGameMatchup = "app-content-state-game-matchup";
    public static string appContentStateGameTraining = "app-content-state-game-training";
    public static string appContentStateGameTrainingChoice = "app-content-state-game-training-choice";
    public static string appContentStateGameTrainingCollection = "app-content-state-game-training-collection";
    public static string appContentStateGameContent = "app-content-state-game-content";
    public static string appContentStateGameTips = "app-content-state-game-tips";
}

public class BaseAppContentStates<T> : DataObjects<T> where T : DataObject, new() {
    private static T current;
    private static volatile BaseAppContentStates<T> instance;
    private static object syncRoot = new Object();
    private string BASE_DATA_KEY = "app-content-state-data";

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

    public static BaseAppContentStates<T> BaseInstance {
        get {
            if (instance == null) {
                lock (syncRoot) {
                    if (instance == null)
                        instance = new BaseAppContentStates<T>(true);
                }
            }

            return instance;
        }
        set {
            instance = value;
        }
    }

    public BaseAppContentStates() {
        Reset();
    }

    public BaseAppContentStates(bool loadData) {
        Reset();
        path = "data/" + BASE_DATA_KEY + ".json";
        pathKey = BASE_DATA_KEY;
        LoadData();
    }

    public bool isAppContentStateGameArcade {
        get {
            return IsAppContentState(AppContentStateMeta.appContentStateGameArcade);
        }
    }

    public bool isAppContentStateGameCoop {
        get {
            return IsAppContentState(AppContentStateMeta.appContentStateGameCoop);
        }
    }
        
    public bool isAppContentStateGameMissions {
        get {
            return IsAppContentState(AppContentStateMeta.appContentStateGameMissions);
        }
    }

    public bool isAppContentStateGameChallenge {
        get {
            return IsAppContentState(AppContentStateMeta.appContentStateGameChallenge);
        }
    }

    public bool isAppContentStateGameContent {
        get {
            return IsAppContentState(AppContentStateMeta.appContentStateGameContent);
        }
    }

    public bool isAppContentStateGameTraining {
        get {
            return IsAppContentState(AppContentStateMeta.appContentStateGameTraining);
        }
    }

    public bool isAppContentStateGameTrainingChoice {
        get {
            return IsAppContentState(AppContentStateMeta.appContentStateGameTrainingChoice);
        }
    }

    public bool isAppContentStateGameTrainingCollection {
        get {
            return IsAppContentState(AppContentStateMeta.appContentStateGameTrainingCollection);
        }
    }

    public bool IsAppContentState(string code) {
        if (AppContentStates.Current.code == code) {
            return true;
        }
        return false;
    }

    public void ChangeState(string code) {

        LogUtil.Log("ChangeState:code:" + code);

        if (AppContentStates.Current.code != code) {

            LogUtil.Log("ChangeState:code-CHANGE:" + code);

            AppContentState app_content_state = AppContentStates.Instance.GetByCode(code);

            if (app_content_state != null) {

                AppContentStates.Current = app_content_state;

                GameProfiles.Current.SetCurrentAppContentState(code);

                LogUtil.Log("AppContentStates:code:" + AppContentStates.Current.code);

                string modeType = app_content_state.type;
                string mode = app_content_state.key;
                string state = app_content_state.GetAppStateDefault();

                LogUtil.Log("AppContentStates:modeType:" + modeType);
                LogUtil.Log("AppContentStates:mode:" + mode);
                LogUtil.Log("AppContentStates:state:" + state);

                AppStates.Instance.ChangeState(state);
                AppModes.Instance.ChangeState(mode);
                AppModeTypes.Instance.ChangeState(modeType);
            }
        }
    }

    /*
    public List<AppContentState> GetListByPack(string packCode) {
    List<AppContentState> filteredList = new List<AppContentState>();
    foreach(AppContentState obj in GetAll()) {
    if(packCode.ToLower() == obj.pack_code.ToLower()) {
    filteredList.Add(obj);
    }
    }
    
    return filteredList;
    }
    */
    
    public List<AppContentState> GetListByCodeAndPackCode(string stateCode, string packCode) {

        List<AppContentState> filteredList = new List<AppContentState>();

        foreach (AppContentState obj in AppContentStates.Instance.GetListByPack(packCode)) {
            if (stateCode.ToLower() == obj.code.ToLower()) {
                filteredList.Add(obj);
            }
        }
    
        return filteredList;
    }
    
    public List<AppContentState> GetByAppState(string app_state) {

        Dictionary<string, int> packSorts = new Dictionary<string, int>();
        List<AppContentState> app_content_states = new List<AppContentState>();

        foreach (AppContentState state in AppContentStates.Instance.GetAll()) {
            if (state.app_states.Contains(app_state)
                || state.app_states.Contains("*")) {

                int sortOrder = 1;
                if (packSorts.ContainsKey(state.pack_code)) {
                    sortOrder = packSorts[state.pack_code];
                }
                else {
                    GamePack gamePack = GamePacks.Instance.GetById(state.pack_code);
                    if (gamePack != null) {
                        sortOrder = gamePack.sort_order;
                        packSorts.Add(state.pack_code, sortOrder);
                    }
                }
                state.sort_order = state.sort_order * sortOrder;
                app_content_states.Add(state);
            }
        }
        app_content_states = AppContentStates.Instance.SortList(app_content_states);
    
        return app_content_states;
    }
}

public class DataPlatformStoreMeta {
    public string platform = "";
    public string url = "";
    public string storeUrl = "";
    public string appId = "";
    public string locale = "en";
    public string price = "0";
}

public class BaseAppContentStateKeys {
    public static string app_states = "app_states";
    public static string required_packs = "required_packs";
    public static string required_trackers = "required_trackers";
}

public class BaseAppContentState : GameDataObject {

    // type

    // Attributes that are added or changed after launch should be like this to prevent
    // profile conversions.

    public BaseAppContentState() {
        Reset();
    }

    public override void Reset() {
        base.Reset();
        app_states = new List<string>();
        required_packs = new List<string>();
        required_trackers = new List<string>();
    }

    public string GetAppStateDefault() {
        foreach (string app_state in app_states) {
            return app_state;
        }
        return "";
    }

    public DataPlatformStoreMeta GetDataPlatformAttribute(string key) {
        key = GetPlatformAttributeKey(key);
        //UnityEngine.LogUtil.LogWarning("key:" + key);
        string json = GetAttributeStringValue(key);
        //UnityEngine.LogUtil.LogWarning("json:" + json);
        //UnityEngine.LogUtil.LogWarning("json:" + json.Replace("\\\"", "\""));
        if (!string.IsNullOrEmpty(json)) {
            try {
                return JsonMapper.ToObject<DataPlatformStoreMeta>(json);
            }
            catch (Exception e) {
                LogUtil.LogWarning("Error parsing DataPlatformStoreMeta" + e);
            }
        }
        return null;
    }
    
    public DataPlatformStoreMeta GetDataPlatformStoreMeta() {
        string key = "storemeta";
        return GetDataPlatformAttribute(key);
    }
    
    public bool IsExternalContent() {
        return GetDataPlatformStoreMeta() != null;
    }
}