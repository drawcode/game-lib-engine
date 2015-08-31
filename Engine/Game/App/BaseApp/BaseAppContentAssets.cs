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

    public static string GetLoadAssetPath(string key, string assetCode, string type) {

        string path = "";
        
        LogUtil.Log("LoadAsset:" + " assetCode:" + assetCode + " type:" + type);

        if (key.StartsWith(BaseDataObjectKeys.level) 
            || key.StartsWith(BaseDataObjectKeys.levelAssets)) {                    
            path = ContentPaths.appCacheVersionSharedPrefabLevelAssets;
        }
        else if (key.StartsWith(BaseDataObjectKeys.character)) {                    
            path = ContentPaths.appCacheVersionSharedPrefabCharacters;
        }
        else if (key.StartsWith(BaseDataObjectKeys.weapon)) {                    
            path = ContentPaths.appCacheVersionSharedPrefabWeapons;
        }
        else if (key.StartsWith(BaseDataObjectKeys.world)) {                    
            path = ContentPaths.appCacheVersionSharedPrefabWorlds;
        }
        else if (key.StartsWith(BaseDataObjectKeys.vehicle)) {                    
            path = ContentPaths.appCacheVersionSharedPrefabVehicles;
        }
        
        path += assetCode;
        
        LogUtil.Log("LoadAsset:" + " path:" + path);
        
        if (type == "resource") {
            // Load from resources                        
        }
        else if (type == "streaming") {
            // TODO update path for streaming folder
        }
        else if (type == "server") {
            // TODO udpate path for download and process
        }
        else {
            // Load from other
        }

        return path;
    }

    // LOAD ASSET
    
    public static GameObject LoadAsset(string code) {
        return LoadAsset("", code);
    }

    public static GameObject LoadAssetLevelAssets(string code) {
        return LoadAsset(BaseDataObjectKeys.levelAssets, code);
    }
    
    public static GameObject LoadAssetCharacter(string code) {
        return LoadAsset(BaseDataObjectKeys.character, code);
    }
    
    public static GameObject LoadAssetWeapon(string code) {
        return LoadAsset(BaseDataObjectKeys.weapon, code);
    }
    
    public static GameObject LoadAssetWorld(string code) {
        return LoadAsset(BaseDataObjectKeys.world, code);
    }
    
    public static GameObject LoadAssetVehicle(string code) {
        return LoadAsset(BaseDataObjectKeys.vehicle, code);
    }
    
    public static GameObject LoadAsset(string key, string code) {
        
        LogUtil.Log("LoadAsset:" + " key:" + key + " code:" + code);
        
        GameObject prefabObject = LoadAssetPrefab(key, code);
        
        if (prefabObject == null) {
            return null;
        }
        
        GameObject go = GameObjectHelper.CreateGameObject(
            prefabObject, Vector3.zero, Quaternion.identity, 
            true) as GameObject;
        
        LogUtil.Log("LoadAsset:" + " go:" + go != null);
        
        return go;
    }

    // LOAD ASSET PREFAB
    
    public static GameObject LoadAssetPrefab(string code) {
        return LoadAssetPrefab("", code);
    }

    public static GameObject LoadAssetPrefab(string key, string code) {
        
        LogUtil.Log("LoadAssetPrefab:" + " key:" + key + " code:" + code);
        //LogUtil.Log("LoadAsset:" + " code:" + code);
        
        foreach (AppContentAsset asset in AppContentAssets.Instance.GetAll()) {
            
            if (asset.code == code 
                && (asset.key == key || string.IsNullOrEmpty(key))) {
                
                LogUtil.Log("LoadAssetPrefab2:" + " key:" + key + " code:" + code);
                
                if (asset != null) {

                    string path = GetLoadAssetPath(asset.key, asset.code, asset.type);                    

                    if (string.IsNullOrEmpty(path)) {
                        return null;
                    }

                    LogUtil.Log("LoadAssetPrefab:" + " path:" + path);
                        
                    GameObject prefabObject = PrefabsPool.PoolPrefab(path);
                        
                    LogUtil.Log("LoadAssetPrefab:" + " prefabObject:" + prefabObject != null);
                        
                    return prefabObject;
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
    public static string app_states = "app_states";
    public static string AppContentStates = "app_content_states";
    public static string required_assets = "required_assets";
}

public class BaseAppContentAsset : GameDataObject {
    // types: tracker, pack, data, generic

    // Attributes that are added or changed after launch should be like this to prevent
    // profile conversions.

    public BaseAppContentAsset() {
        Reset();
    }

    public override void Reset() {
        base.Reset();
        app_states = new List<string>();
        app_content_states = new List<string>();
        required_assets = new Dictionary<string, List<string>>();
    }
}