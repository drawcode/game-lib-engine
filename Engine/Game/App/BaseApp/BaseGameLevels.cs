using System;
using System.Collections.Generic;
using System.IO;
using Engine.Data.Json;
using Engine.Utility;

public class BaseGameLevels<T> : DataObjects<T> where T : DataObject, new() {
    private static T current;
    private static volatile BaseGameLevels<T> instance;
    private static object syncRoot = new Object();

    public static string BASE_DATA_KEY = "game-level-data";    
    
    public static float gridHeight = 1f;
    public static float gridWidth = 90f;
    public static float gridDepth = 90f;
    public static float gridBoxSize = 4f;
    
    public static bool centeredX = true;
    public static bool centeredY = false;
    public static bool centeredZ = true;

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

    public static BaseGameLevels<T> BaseInstance {
        get {
            if (instance == null) {
                lock (syncRoot) {
                    if (instance == null)
                        instance = new BaseGameLevels<T>(true);
                }
            }

            return instance;
        }
        set {
            instance = value;
        }
    }

    public BaseGameLevels() {
        Reset();
    }

    public BaseGameLevels(bool loadData) {
        Reset();
        path = "data/" + BASE_DATA_KEY + ".json";
        pathKey = BASE_DATA_KEY;
        LoadData();
    }

    public virtual List<T> GetLevelsByPackId(string packId) {
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
        //List<T> gameLevels = GetAll();
        List<T> gameLevelsFiltered = new List<T>();
        //foreach (T gameLevel in gameLevels) {

            //bool hasAccess = GameDatas.Instance.CheckIfHasAccessToLevel(gameLevel.code);
            //if(hasAccess) {
            //	gameLevelsFiltered.Add(gameLevel);
            //}
        //}
        return gameLevelsFiltered;
    }

    public virtual T GetDefaultLevel() {
        T levelReturn = new T();
        foreach (T level in GetAll()) {
            return level;
        }
        return levelReturn;
    }

    public virtual T GetDefaultLevelByPack(string packId) {
        T levelReturn = new T();
        foreach (T level in GetLevelsByPackId(packId)) {
            string code = (string)GetType().GetProperty("code").GetValue(level, null);
            if (code != "all") {
                return level;
            }
        }
        return levelReturn;
    }
}

public class BaseGameLevelKeys {
    public static string LEVEL_INITIAL_DIFFICULTY = "initial-diff";
    public static string LEVEL_SPONSOR_NAME = "sponsor";
    public static string LEVEL_SPONSOR_IMAGE = "sponsor-img";
}

public class BaseGameLevel : GameDataObject {

    // Attributes that are added or changed after launch should be like this to prevent
    // profile conversions.

    public BaseGameLevel() {
        Reset();
    }

    public override void Reset() {
        base.Reset();
    }

    public void Clone(BaseGameLevel toCopy) {
        base.Clone(toCopy);
    }

    // Attributes that are added or changed after launch should be like this to prevent
    // profile conversions.
}