using System;
using System.Collections.Generic;
using System.IO;

using Engine.Data.Json;
using Engine.Utility;

using UnityEngine;

public class BaseGameLevelLayouts<T> : DataObjects<T> where T : DataObject, new() {
    private static T current;
    private static volatile BaseGameLevelLayouts<T> instance;
    private static System.Object syncRoot = new System.Object();
    public static string BASE_DATA_KEY = "game-level-layout-data";

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

    public static BaseGameLevelLayouts<T> BaseInstance {
        get {
            if (instance == null) {
                lock (syncRoot) {
                    if (instance == null)
                        instance = new BaseGameLevelLayouts<T>(true);
                }
            }

            return instance;
        }
        set {
            instance = value;
        }
    }

    public BaseGameLevelLayouts() {
        Reset();
    }

    public BaseGameLevelLayouts(bool loadData) {
        Reset();
        path = "data/" + BASE_DATA_KEY + ".json";
        pathKey = BASE_DATA_KEY;
        LoadData();
    }

    public virtual T GetDefaultLevelLayout() {
        T levelReturn = new T();
        foreach (T level in GetAll()) {
            return level;
        }
        return levelReturn;
    }

    public void SetGameLevelLayout(GameLevelLayout gameLevelLayout) {
        bool found = false;
        
        for (int i = 0; i < items.Count; i++) {
            if (GameLevelLayouts.Instance.items[i].code.ToLower() == gameLevelLayout.code.ToLower()) {
                GameLevelLayouts.Instance.items[i] = gameLevelLayout;
                found = true;
                break;
            }
        }
        
        if (!found) {
            GameLevelLayouts.Instance.items.Add(gameLevelLayout);
        }
    }

    public List<GameLevelLayout> GetByWorldId(string worldCode) {
        List<GameLevelLayout> filteredLevelLayouts = new List<GameLevelLayout>();
        foreach (GameLevelLayout levelLayout in GameLevelLayouts.Instance.GetAll()) {
            if (levelLayout.world_code == worldCode) {
                filteredLevelLayouts.Add(levelLayout);
            }
        }
        return filteredLevelLayouts;
    }

    public void ChangeCurrentAbsolute(string code) {
        GameLevelLayouts.Current.code = "changeme";
        ChangeCurrent(code);
    }
    
    public void ChangeCurrent(string code) {

        if (GameLevelLayouts.Current.code != code) {

            GameLevelLayouts.Current = GameLevelLayouts.Instance.GetById(code);

            string originalCode = code;
            if (string.IsNullOrEmpty(GameLevelLayouts.Current.code)) {
                //code = "level-" + code;
                GameLevelLayouts.Current = GameLevelLayouts.Instance.GetById(code);
            }
            
            if (string.IsNullOrEmpty(GameLevelLayouts.Current.code)) {
                // TODO not found add?
                GameLevelLayout obj = new GameLevelLayout();
                obj.code = code;
                obj.date_created = DateTime.Now;
                obj.date_modified = DateTime.Now;
                obj.description = originalCode;
                obj.display_name = code;
                obj.name = originalCode;
                obj.game_id = GameversesConfig.apiGameId;
                obj.key = originalCode;
                GameLevelLayouts.Instance.items.Add(obj);
            }
            
            if (string.IsNullOrEmpty(GameLevelLayouts.Current.code)) {
                GameLevelLayouts.Current = GameLevelLayouts.Instance.GetById(code);
            }

            LogUtil.Log("Changing LevelLayout: code:" + code);    
        }
    }

}

public class BaseGameLevelLayout : GameDataObject {

    public virtual GameDataObjectItem data {
        get {
            return Get<GameDataObjectItem>(BaseDataObjectKeys.data);
        }
        
        set {
            Set<GameDataObjectItem>(BaseDataObjectKeys.data, value);
        }
    } 

    public BaseGameLevelLayout() {
        Reset();
    }

    public override void Reset() {
        base.Reset();
    }

    // Attributes that are added or changed after launch should be like this to prevent
    // profile conversions.
}