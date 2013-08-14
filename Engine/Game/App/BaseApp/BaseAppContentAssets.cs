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

public class BaseAppContentAsset : GameDataObject {
    public List<string> appStates;
    public List<string> appContentStates;
    public Dictionary<string, List<string>> requiredAssets;

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