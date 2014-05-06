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
    
    public static GameObject LoadAssetPrefab(string code) {
        return LoadAssetPrefab("", code);
    }

    public static GameObject LoadAssetPrefab(string key, string code) {
        
        LogUtil.Log("LoadAsset:" + " key:" + key + " code:" + code);
        //LogUtil.Log("LoadAsset:" + " code:" + code);
        
        foreach (AppContentAsset asset in AppContentAssets.Instance.GetAll()) {
            
            if (asset.code == code 
                && (asset.key == key || string.IsNullOrEmpty(key))) {
                
                LogUtil.Log("LoadAsset2:" + " key:" + key + " code:" + code);
                
                if (asset != null) {
                    
                    string assetCode = asset.code;
                    string path = "";
                    
                    LogUtil.Log("LoadAsset:" + " assetCode:" + assetCode + " asset.type:" + asset.type);
                    
                    if (asset.type == "resource") {
                        // Load from resources
                        
                        if (asset.key.StartsWith("level")) {                    
                            path = ContentPaths.appCacheVersionSharedPrefabLevelAssets;
                        }
                        else if (asset.key.StartsWith("character")) {                    
                            path = ContentPaths.appCacheVersionSharedPrefabCharacters;
                        }
                        else if (asset.key.StartsWith("weapon")) {                    
                            path = ContentPaths.appCacheVersionSharedPrefabWeapons;
                        }
                        else if (asset.key.StartsWith("world")) {                    
                            path = ContentPaths.appCacheVersionSharedPrefabWorlds;
                        }
                        else if (asset.key.StartsWith("vehicle")) {                    
                            path = ContentPaths.appCacheVersionSharedPrefabVehicles;
                        }
                        
                        path += assetCode;
                        
                        LogUtil.Log("LoadAsset:" + " path:" + path);
                        
                        GameObject prefabObject = PrefabsPool.PoolPrefab(path);
                        
                        LogUtil.Log("LoadAsset:" + " prefabObject:" + prefabObject != null);
                        
                        return prefabObject;
                        
                    }
                    else if (asset.type == "streaming") {
                        // Load from resources
                    }
                    else if (asset.type == "server") {
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

    public static GameObject LoadAsset(string key, string code) {
        
        LogUtil.Log("LoadAsset:" + " key:" + key + " code:" + code);
        //LogUtil.Log("LoadAsset:" + " code:" + code);

        foreach (AppContentAsset asset in AppContentAssets.Instance.GetAll()) {

            if (asset.code == code 
                && (asset.key == key || string.IsNullOrEmpty(key))) {
                
                LogUtil.Log("LoadAsset2:" + " key:" + key + " code:" + code);

                if (asset != null) {

                    string assetCode = asset.code;
                    string path = "";
                    
                    LogUtil.Log("LoadAsset:" + " assetCode:" + assetCode + " asset.type:" + asset.type);

                    if (asset.type == "resource") {
                        // Load from resources

                        if (asset.key.StartsWith("level")) {                    
                            path = ContentPaths.appCacheVersionSharedPrefabLevelAssets;
                        }
                        else if (asset.key.StartsWith("character")) {                    
                            path = ContentPaths.appCacheVersionSharedPrefabCharacters;
                        }
                        else if (asset.key.StartsWith("weapon")) {                    
                            path = ContentPaths.appCacheVersionSharedPrefabWeapons;
                        }
                        else if (asset.key.StartsWith("world")) {                    
                            path = ContentPaths.appCacheVersionSharedPrefabWorlds;
                        }
                        else if (asset.key.StartsWith("vehicle")) {                    
                            path = ContentPaths.appCacheVersionSharedPrefabVehicles;
                        }
                        
                        path += assetCode;
                                                
                        LogUtil.Log("LoadAsset:" + " path:" + path);
                        
                        GameObject prefabObject = PrefabsPool.PoolPrefab(path);
                        GameObject go = GameObjectHelper.CreateGameObject(
                            prefabObject, Vector3.zero, Quaternion.identity, 
                             true) as GameObject;
                        
                        LogUtil.Log("LoadAsset:" + " go:" + go != null);

                        return go;

                    }
                    else if (asset.type == "streaming") {
                        // Load from resources
                    }
                    else if (asset.type == "server") {
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

public class AppContentAssetAttributes {
    public static string version_file_increment = "version_file_increment";
    public static string version = "version";
    public static string version_required_app = "version_required_app";
    public static string version_type = "version_type";
    public static string version_file_ext = "version_file_ext";
    public static string version_file_type = "version_file_type";
    /*
     * 
            "version_file_increment":"1",
            "version":"1.0",
            "version_required_app":"1.0",
            "version_type":"itemized"
            */
}

public class AppContentAssetAttributesFileType {
    public static string videoType = "video";
    public static string audioType = "audio";
    public static string imageType = "image";
    public static string assetBundleType = "assetBundle";
}

public class AppContentAssetAttributesFileExt {
    public static string videoM4vExt = "m4v";
    public static string videoMp4Ext = "mp4";
    public static string audioMp3Ext = "mp3";
    public static string audioWavExt = "wav";
    public static string imagePngExt = "png";
    public static string imageJpgExt = "jpg";
    public static string assetBundleExt = "unity3d";
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