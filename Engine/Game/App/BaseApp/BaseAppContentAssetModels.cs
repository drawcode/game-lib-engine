using System;
using System.Collections.Generic;
using System.IO;

public class BaseAppContentAssetModels<T> : DataObjects<T> where T : new() {
    private static T current;
    private static volatile BaseAppContentAssetModels<T> instance;
    private static object syncRoot = new Object();

    private string BASE_DATA_KEY = "app-content-asset-model-data";

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

    public static BaseAppContentAssetModels<T> BaseInstance {
        get {
            if (instance == null) {
                lock (syncRoot) {
                    if (instance == null)
                        instance = new BaseAppContentAssetModels<T>(true);
                }
            }

            return instance;
        }
        set {
            instance = value;
        }
    }

    public BaseAppContentAssetModels() {
        Reset();
    }

    public BaseAppContentAssetModels(bool loadData) {
        Reset();
        path = "data/" + BASE_DATA_KEY + ".json";
        pathKey = BASE_DATA_KEY;
        LoadData();
    }
}

public class BaseAppContentAssetModel : GameDataObject {

    // Attributes that are added or changed after launch should be like this to prevent
    // profile conversions.

    public BaseAppContentAssetModel() {
        Reset();
    }

    public override void Reset() {
        base.Reset();
    }

    public void Clone(BaseAppContentAssetModel toCopy) {
        base.Clone(toCopy);
    }

    // Attributes that are added or changed after launch should be like this to prevent
    // profile conversions.
}