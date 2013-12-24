using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabsPool : MonoBehaviour {    
    
    public Dictionary<string, GameObject> prefabs;

    // Only one ObjectPoolManager can exist. We use a singleton pattern to enforce this.
    private static PrefabsPool _instance = null;
    
    public static PrefabsPool instance {
        get {
            if (!_instance) {
                _instance = FindObjectOfType(typeof(PrefabsPool)) as PrefabsPool;

                if (!_instance) {
                    var obj = new GameObject("_PrefabsPool");
                    _instance = obj.AddComponent<PrefabsPool>();
                }
            }
            
            return _instance;
        }
    }
    
    private void OnApplicationQuit() {
        _instance = null;
    }

    public static void CheckPrefabs() {
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

        CheckPrefabs();
        
        string key = CryptoUtil.CalculateSHA1ASCII(path);
        
        if(!instance.prefabs.ContainsKey(key)) {
            GameObject prefab = Resources.Load(path) as GameObject;
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
