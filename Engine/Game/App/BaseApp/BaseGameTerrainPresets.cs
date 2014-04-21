using System;
using System.Collections.Generic;
using System.IO;

public class BaseGameTerrainPresets<T> : DataObjects<T> where T : DataObject, new() {
    private static T current;
    private static volatile BaseGameTerrainPresets<T> instance;
    private static object syncRoot = new Object();

    private string BASE_DATA_KEY = "game-terrain-preset-data";

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

    public static BaseGameTerrainPresets<T> BaseInstance {
        get {
            if (instance == null) {
                lock (syncRoot) {
                    if (instance == null)
                        instance = new BaseGameTerrainPresets<T>(true);
                }
            }

            return instance;
        }
        set {
            instance = value;
        }
    }

    public BaseGameTerrainPresets() {
        Reset();
    }

    public BaseGameTerrainPresets(bool loadData) {
        Reset();
        path = "data/" + BASE_DATA_KEY + ".json";
        pathKey = BASE_DATA_KEY;
        LoadData();
    }
}

public class GameTerrainPresetItems : GameDataObject {
    
    public virtual List<GameTerrainPresetItem> items {
        get {
            return Get<List<GameTerrainPresetItem>>(BaseDataObjectKeys.items);
        }
        
        set {
            Set<List<GameTerrainPresetItem>>(BaseDataObjectKeys.items, value);
        }
    }
}


public class GameTerrainPresetItem : GameDataObject {

}

public class BaseGameTerrainPreset : GameDataObject {

    // Attributes that are added or changed after launch should be like this to prevent
    // profile conversions.

    
    public virtual GameTerrainPresetItems data {
        get {
            return Get<GameTerrainPresetItems>(BaseDataObjectKeys.data);
        }
        
        set {
            Set(BaseDataObjectKeys.data, value);
        }
    }

    public BaseGameTerrainPreset() {
        Reset();
    }

    public override void Reset() {
        base.Reset();
    }


    // Attributes that are added or changed after launch should be like this to prevent
    // profile conversions.
}