using System;
using System.Collections.Generic;
using System.IO;

public class BaseGameItemPresets<T> : DataObjects<T> where T : DataObject, new() {
    private static T current;
    private static volatile BaseGameItemPresets<T> instance;
    private static object syncRoot = new Object();

    private string BASE_DATA_KEY = "game-item-preset-data";

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

    public static BaseGameItemPresets<T> BaseInstance {
        get {
            if (instance == null) {
                lock (syncRoot) {
                    if (instance == null)
                        instance = new BaseGameItemPresets<T>(true);
                }
            }

            return instance;
        }
        set {
            instance = value;
        }
    }

    public BaseGameItemPresets() {
        Reset();
    }

    public BaseGameItemPresets(bool loadData) {
        Reset();
        path = "data/" + BASE_DATA_KEY + ".json";
        pathKey = BASE_DATA_KEY;
        LoadData();
    }
}


public class GameItemPresetItems : GamePresetItems {
    
}

public class GameItemPresetItem : GamePresetItem {
    
}

/*
public class GameItemPresetItems : GameDataObject {
    
    public virtual List<GameItemPresetItem> items {
        get {
            return Get<List<GameItemPresetItem>>(BaseDataObjectKeys.items);
        }
        
        set {
            Set<List<GameItemPresetItem>>(BaseDataObjectKeys.items, value);
        }
    }
}


public class GameItemPresetItem : GameDataObject {

}
*/

public class BaseGameItemPreset : GameDataObject {

    // Attributes that are added or changed after launch should be like this to prevent
    // profile conversions.

    
    public virtual GameItemPresetItems data {
        get {
            return Get<GameItemPresetItems>(BaseDataObjectKeys.data);
        }
        
        set {
            Set(BaseDataObjectKeys.data, value);
        }
    }

    public BaseGameItemPreset() {
        Reset();
    }

    public override void Reset() {
        base.Reset();
    }


    // Attributes that are added or changed after launch should be like this to prevent
    // profile conversions.
}