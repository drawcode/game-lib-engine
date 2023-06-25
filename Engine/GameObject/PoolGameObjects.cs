using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolGameObjects : GameObjectBehavior {    
    
    public Dictionary<string, GameObject> prefabs;

    // Only one ObjectPoolManager can exist. We use a singleton pattern to enforce this.
    private static PoolGameObjects _instance = null;
    
    public static PoolGameObjects instance {
        get {
            if (!_instance) {
                _instance = FindAnyObjectByType(typeof(PoolGameObjects)) as PoolGameObjects;

                if (!_instance) {
                    var obj = new GameObject("_PoolGameObjects");
                    _instance = obj.AddComponent<PoolGameObjects>();
                }
            }
            
            return _instance;
        }
    }
    
    private void OnApplicationQuit() {
        _instance = null;
    }

    //

    public static void Check() {
        if(instance == null) {
            return;
        }

        if(instance.prefabs == null) {
            instance.prefabs = new Dictionary<string, GameObject>();
        }
    }

    public static GameObject PoolPrefab(string path) {
        
        if(instance == null) {
            return null;
        }

        Check();
        
        string key = CryptoUtil.CalculateSHA1ASCII(path);
        
        if(!instance.prefabs.ContainsKey(key)) {

            GameObject prefab = AssetUtil.LoadAsset<GameObject>(path); 
            if(prefab != null) {
                instance.prefabs.Add(key, Resources.Load(path) as GameObject);
            }
        }
        
        if(instance.prefabs.ContainsKey(key)) {
            return instance.prefabs[key];
        }
        
        return null;
    }
}
