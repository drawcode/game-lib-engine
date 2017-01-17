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
        else if (key.StartsWith(BaseDataObjectKeys.character)
                 || key.StartsWith(BaseDataObjectKeys.characters)) {                    
            path = ContentPaths.appCacheVersionSharedPrefabCharacters;
        }
        else if (key.StartsWith(BaseDataObjectKeys.weapon)
                 || key.StartsWith(BaseDataObjectKeys.weapons)) {                    
            path = ContentPaths.appCacheVersionSharedPrefabWeapons;
        }
        else if (key.StartsWith(BaseDataObjectKeys.world)
                 || key.StartsWith(BaseDataObjectKeys.worlds)) {                    
            path = ContentPaths.appCacheVersionSharedPrefabWorlds;
        }
        else if (key.StartsWith(BaseDataObjectKeys.vehicle)
                 || key.StartsWith(BaseDataObjectKeys.vehicles)) {                    
            path = ContentPaths.appCacheVersionSharedPrefabVehicles;
        }
        else if (key.StartsWith(BaseDataObjectKeys.effect)
                 || key.StartsWith(BaseDataObjectKeys.effects)) {                    
            path = ContentPaths.appCacheVersionSharedPrefabEffects;
        }
        else if (key.StartsWith(BaseDataObjectKeys.item)
                 || key.StartsWith(BaseDataObjectKeys.items)) {
            path = ContentPaths.appCacheVersionSharedPrefabLevelItems;
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
    
    public static GameObject LoadAsset(
        string code,
        Vector3 pos = default(Vector3),
        Quaternion rotate = default(Quaternion),
        bool pool = true) {

        return LoadAsset("", code,
                         pos, rotate, pool);
    }

    public static GameObject LoadAssetLevelAssets(
        string code,
        Vector3 pos = default(Vector3),
        Quaternion rotate = default(Quaternion),
        bool pool = true) {

        return LoadAsset(BaseDataObjectKeys.levelAssets, code,
                         pos, rotate, pool);
    }
    
    public static GameObject LoadAssetCharacters(
        string code,
        Vector3 pos = default(Vector3),
        Quaternion rotate = default(Quaternion),
        bool pool = true) {

        return LoadAsset(BaseDataObjectKeys.characters, code,
                         pos, rotate, pool);
    }

    public static GameObject LoadAssetItems(
        string code,
        Vector3 pos = default(Vector3),
        Quaternion rotate = default(Quaternion),
        bool pool = true) {

        return LoadAsset(BaseDataObjectKeys.items, code,
                         pos, rotate, pool);
    }

    public static GameObject LoadAssetWeapons(
        string code,
        Vector3 pos = default(Vector3),
        Quaternion rotate = default(Quaternion),
        bool pool = true) {

        return LoadAsset(BaseDataObjectKeys.weapons, code,
                         pos, rotate, pool);
    }

    public static GameObject LoadAssetEffects(
        string code,
        Vector3 pos = default(Vector3),
        Quaternion rotate = default(Quaternion),
        bool pool = true) {

        return LoadAsset(BaseDataObjectKeys.effects, code,
                         pos, rotate, pool);
    }
    
    public static GameObject LoadAssetWorlds(
        string code,
        Vector3 pos = default(Vector3),
        Quaternion rotate = default(Quaternion),
        bool pool = true) {

        return LoadAsset(BaseDataObjectKeys.worlds, code,
                         pos, rotate, pool);
    }
    
    public static GameObject LoadAssetVehicles(
        string code,
        Vector3 pos = default(Vector3),
        Quaternion rotate = default(Quaternion),
        bool pool = true) {

        return LoadAsset(BaseDataObjectKeys.vehicles, code,
                         pos, rotate, pool);
    }
    
    public static GameObject LoadAsset(
        string key, 
        string code,
        Vector3 pos = default(Vector3),
        Quaternion rotate = default(Quaternion),
        bool pool = true
        ) {
        
        LogUtil.Log("LoadAsset:" + " key:" + key + " code:" + code);
        
        GameObject prefabObject = LoadAssetPrefab(key, code);
        
        if (prefabObject == null) {
            return null;
        }
        
        GameObject go = GameObjectHelper.CreateGameObject(
            prefabObject, pos, rotate, 
            pool) as GameObject;
        
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

        Debug.Log("ERROR:LoadAssetPrefab:NOT FOUND IN ASSETS DATA: key:" + key + " code:" + code);
                
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