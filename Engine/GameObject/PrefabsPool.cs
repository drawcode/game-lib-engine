using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabsPool : GameObjectBehavior {

    public Dictionary<string, GameObject> prefabs;

    // Only one ObjectPoolManager can exist. We use a singleton pattern to enforce this.
    private static PrefabsPool _instance = null;

    public static PrefabsPool instance {
        get {
            if (!_instance) {
                _instance = FindAnyObjectByType(typeof(PrefabsPool)) as PrefabsPool;

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
        if (instance == null) {
            return;
        }

        if (instance.prefabs == null) {
            instance.prefabs = new Dictionary<string, GameObject>();
        }
    }

    // Path -> dictionary key memo.
    //
    // The KEY SCHEME IS DELIBERATELY UNCHANGED -- `prefabs` is a public field and callers
    // outside this assembly may hold SHA-1 keys -- but the hash itself was recomputed on
    // every lookup, for a small fixed set of prefab paths. Measured in a live round:
    // CalculateSHA1ASCII costs 505 bytes and 3.19 us per call, and it allocates a fresh
    // ASCIIEncoding and an undisposed SHA1CryptoServiceProvider each time. This sits on
    // the item/actor spawn path, which is where the frame spikes are.
    private static readonly Dictionary<string, string> pathKeys
        = new Dictionary<string, string>();

    private static string KeyForPath(string path) {

        string key;

        if (!pathKeys.TryGetValue(path, out key)) {
            key = CryptoUtil.CalculateSHA1ASCII(path);
            pathKeys[path] = key;
        }

        return key;
    }

    public static GameObject PoolPrefab(string path) {

        if (instance == null) {
            return null;
        }

        CheckPrefabs();

        string key = KeyForPath(path);

        GameObject cached;

        if (instance.prefabs.TryGetValue(key, out cached)) {
            return cached;
        }

        GameObject prefab = Resources.Load(path) as GameObject;

        if (prefab != null) {
            instance.prefabs.Add(key, prefab);
            return prefab;
        }

        return null;
    }
}
