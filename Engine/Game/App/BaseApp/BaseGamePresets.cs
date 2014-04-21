using System;
using System.Collections.Generic;
using System.IO;

public class GamePresetTypes {
    public static string terrain = "terrain";
    public static string item = "item";
}

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

    
    public virtual GamePresetItems data {
        get {
            return Get<GamePresetItems>(BaseDataObjectKeys.data);
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