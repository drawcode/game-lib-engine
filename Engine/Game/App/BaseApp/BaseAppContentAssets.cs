using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class BaseAppContentAssets<T> : DataObjects<T> where T : new() {
    private static T current;
    private static volatile BaseAppContentAssets<T> instance;
    private static object syncRoot = new Object();

    private string BASE_DATA_KEY = "app-content-asset-data";

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

    public static BaseAppContentAssets<T> BaseInstance {
        get {
            if (instance == null) {
                lock (syncRoot) {
                    if (instance == null)
                        instance = new BaseAppContentAssets<T>(true);
                }
            }

            return instance;
        }
        set {
            instance = value;
        }
    }

    public BaseAppContentAssets() {
        Reset();
    }

    public BaseAppContentAssets(bool loadData) {
        Reset();
        path = "data/" + BASE_DATA_KEY + ".json";
        pathKey = BASE_DATA_KEY;
        LoadData();
    }
}

public class BaseAppContentAssetKeys {
    public static string appStates = "appStates";
    public static string appContentStates = "appContentStates";
    public static string requiredAssets = "requiredAssets";
}

public class BaseAppContentAsset : GameDataObject {

    public virtual List<string> appStates {
        get {
            return Get<List<string>>(BaseAppContentAssetKeys.appStates);
        }
        
        set {
            Set(BaseAppContentAssetKeys.appStates, value);
        }
    }

    public virtual List<string> appContentStates {
        get {
            return Get<List<string>>(BaseAppContentAssetKeys.appContentStates);
        }
        
        set {
            Set(BaseAppContentAssetKeys.appContentStates, value);
        }
    }

    public virtual Dictionary<string, List<string>> requiredAssets {
        get {
            return Get<Dictionary<string, List<string>>>(BaseAppContentAssetKeys.requiredAssets);
        }
        
        set {
            Set(BaseAppContentAssetKeys.requiredAssets, value);
        }
    }

    // types: tracker, pack, data, generic

    // Attributes that are added or changed after launch should be like this to prevent
    // profile conversions.

    public BaseAppContentAsset() {
        Reset();
    }

    public override void Reset() {
        base.Reset();
        appStates = new List<string>();
        appContentStates = new List<string>();
        requiredAssets = new Dictionary<string, List<string>>();
    }
}