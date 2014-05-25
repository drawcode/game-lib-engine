using System;
using System.Collections.Generic;
using System.IO;

public class GamePresetTypes {
    public static string item = "item";
    public static string character = "character";
    public static string terrain = "terrain";
}

public class GamePresetTypeDefault {
    public static string itemDefault = "item-default";
    public static string characterDefault = "character-default";
    public static string terrainDefault = "terrain-default";
}
//GamePresets.Instance.GetByCode("game-item-default");
public class BaseGamePresets<T> : DataObjects<T> where T : DataObject, new() {
    private static T current;
    private static volatile BaseGamePresets<T> instance;
    private static object syncRoot = new Object();

    private string BASE_DATA_KEY = "game-preset-data";

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

    public static BaseGamePresets<T> BaseInstance {
        get {
            if (instance == null) {
                lock (syncRoot) {
                    if (instance == null)
                        instance = new BaseGamePresets<T>(true);
                }
            }

            return instance;
        }
        set {
            instance = value;
        }
    }

    public BaseGamePresets() {
        Reset();
    }

    public BaseGamePresets(bool loadData) {
        Reset();
        path = "data/" + BASE_DATA_KEY + ".json";
        pathKey = BASE_DATA_KEY;
        LoadData();
    }
}

public class BaseGamePreset : GameDataObject {

    // Attributes that are added or changed after launch should be like this to prevent
    // profile conversions.

    
    public virtual GamePresetItems<GamePresetItem> data {
        get {
            return Get<GamePresetItems<GamePresetItem>>(BaseDataObjectKeys.data);
        }
        
        set {
            Set(BaseDataObjectKeys.data, value);
        }
    }

    public BaseGamePreset() {
        Reset();
    }

    public override void Reset() {
        base.Reset();
    }


    // Attributes that are added or changed after launch should be like this to prevent
    // profile conversions.
}