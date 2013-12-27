using System;
using System.Collections.Generic;
using System.IO;

public class BaseAppContentAssetTexturePresets<T> : DataObjects<T> where T : new() {
    private static T current;
    private static volatile BaseAppContentAssetTexturePresets<T> instance;
    private static object syncRoot = new Object();

    private string BASE_DATA_KEY = "app-content-asset-texture-preset-data";

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

    public static BaseAppContentAssetTexturePresets<T> BaseInstance {
        get {
            if (instance == null) {
                lock (syncRoot) {
                    if (instance == null)
                        instance = new BaseAppContentAssetTexturePresets<T>(true);
                }
            }

            return instance;
        }
        set {
            instance = value;
        }
    }

    public BaseAppContentAssetTexturePresets() {
        Reset();
    }

    public BaseAppContentAssetTexturePresets(bool loadData) {
        Reset();
        path = "data/" + BASE_DATA_KEY + ".json";
        pathKey = BASE_DATA_KEY;
        LoadData();
    }
}

public class BaseAppContentAssetTexturePreset : GameDataObject {

    // Attributes that are added or changed after launch should be like this to prevent
    // profile conversions.

    
    public virtual Dictionary<string, string> data {
        get {
            return Get<Dictionary<string, string>>(BaseDataObjectKeys.data);
        }
        
        set {
            Set(BaseDataObjectKeys.data, value);
        }
    }

    public BaseAppContentAssetTexturePreset() {
        Reset();
    }

    public override void Reset() {
        base.Reset();
    }


    // Attributes that are added or changed after launch should be like this to prevent
    // profile conversions.
}