using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using UnityEngine;

public class BaseAppContentAssets<T> : DataObjects<T> where T : DataObject, new() {
    private static T current;
    private static volatile BaseAppContentAssets<T> instance;
    private static System.Object syncRoot = new System.Object();

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

    
    public static GameObject LoadAsset(string code) {
        return LoadAsset("", code);
    }

    public static GameObject LoadAsset(string key, string code) {

        foreach(AppContentAsset asset in AppContentAssets.Instance.GetAll()) {

            if(asset.code == code 
               && (asset.key == key || string.IsNullOrEmpty(key))) {

                if(asset != null) {

                    string assetCode = asset.code;
                    string path = "";

                    if(asset.type == "resource") {
                        // Load from resources

                        if(asset.key.StartsWith("level")) {                    
                            path = ContentPaths.appCacheVersionSharedPrefabLevelAssets;
                        }
                        else if(asset.key.StartsWith("character")) {                    
                            path = ContentPaths.appCacheVersionSharedPrefabCharacters;
                        }
                        
                        path += assetCode;
                        
                        GameObject prefabObject = PrefabsPool.PoolPrefab(path);
                        GameObject go = GameObjectHelper.CreateGameObject(
                            prefabObject, Vector3.zero, Quaternion.identity, 
                             true) as GameObject;
                        return go;

                    }
                    else if(asset.type == "streaming") {
                        // Load from resources
                    }
                    else if(asset.type == "server") {
                        // Load from resources
                    }
                    else {
                        // Load from other
                    }
                }
            }
        }
        
        return null;
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