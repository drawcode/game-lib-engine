using System;
using System.Collections.Generic;
using System.IO;

public class BaseAppContentAssetTextures<T> : DataObjects<T> where T : DataObject, new() {
    private static T current;
    private static volatile BaseAppContentAssetTextures<T> instance;
    private static object syncRoot = new Object();
    private string BASE_DATA_KEY = "app-content-asset-texture-data";

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

    public static BaseAppContentAssetTextures<T> BaseInstance {
        get {
            if (instance == null) {
                lock (syncRoot) {
                    if (instance == null)
                        instance = new BaseAppContentAssetTextures<T>(true);
                }
            }

            return instance;
        }
        set {
            instance = value;
        }
    }

    public BaseAppContentAssetTextures() {
        Reset();
    }

    public BaseAppContentAssetTextures(bool loadData) {
        Reset();
        path = "data/" + BASE_DATA_KEY + ".json";
        pathKey = BASE_DATA_KEY;
        LoadData();
    }
}

public class BaseAppContentAssetTexture : GameDataObject {

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

    public BaseAppContentAssetTexture() {
        Reset();
    }

    public override void Reset() {
        base.Reset();
    }

    public void Clone(BaseAppContentAssetTexture toCopy) {
        base.Clone(toCopy);
    }

    // Attributes that are added or changed after launch should be like this to prevent
    // profile conversions.
}