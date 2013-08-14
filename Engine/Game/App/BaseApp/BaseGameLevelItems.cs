using System;
using System.Collections.Generic;
using System.IO;
using Engine.Data.Json;
using Engine.Utility;

public class BaseGameLevelItems<T> : DataObjects<T> where T : new() {
    private static T current;
    private static volatile BaseGameLevelItems<T> instance;
    private static object syncRoot = new Object();

    public string BASE_DATA_KEY = "game-level-item-data";

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

    public static BaseGameLevelItems<T> BaseInstance {
        get {
            if (instance == null) {
                lock (syncRoot) {
                    if (instance == null)
                        instance = new BaseGameLevelItems<T>(true);
                }
            }

            return instance;
        }
        set {
            instance = value;
        }
    }

    public BaseGameLevelItems() {
        Reset();
    }

    public BaseGameLevelItems(bool loadData) {
        Reset();
        path = "data/" + BASE_DATA_KEY + ".json";
        pathKey = BASE_DATA_KEY;
        LoadData();
    }

    public void LoadDataItem(string code) {
    }

    public void SaveDataItem(string code) {
    }

    public virtual List<T> GetByPackId(string packId) {
        List<T> packLevels = new List<T>();
        foreach (T gameLevel in GetAll()) {
            List<string> packs = (List<string>)GetType().GetProperty("pack").GetValue(gameLevel, null);
            foreach (string pack in packs) {
                if (pack.ToLower() == packId.ToLower()) {
                    packLevels.Add(gameLevel);
                }
            }
        }
        return packLevels;
    }

    public virtual List<T> GetAllUnlocked() {
        List<T> gameLevels = GetAll();
        List<T> gameLevelsFiltered = new List<T>();
        foreach (T gameLevel in gameLevels) {

            //bool hasAccess = GameDatas.Instance.CheckIfHasAccessToLevel(gameLevel.code);
            //if(hasAccess) {
            //	gameLevelsFiltered.Add(gameLevel);
            //}
        }
        return gameLevelsFiltered;
    }

    public virtual T GetDefault() {
        T levelReturn = new T();
        foreach (T level in GetAll()) {
            return level;
        }
        return levelReturn;
    }

    public virtual T GetDefaultByPack(string packId) {
        T levelReturn = new T();
        foreach (T level in GetByPackId(packId)) {
            string code = (string)GetType().GetProperty("code").GetValue(level, null);
            if (code != "all") {
                return level;
            }
        }
        return levelReturn;
    }
	
}

public class BaseGameLevelItem : GameDataObject {

    // Attributes that are added or changed after launch should be like this to prevent
    // profile conversions.

    public BaseGameLevelItem() {
        Reset();
    }

    public override void Reset() {
        base.Reset();
    }

    public void Clone(BaseGameLevelItem toCopy) {
        base.Clone(toCopy);
    }

    // Attributes that are added or changed after launch should be like this to prevent
    // profile conversions.
}