using System;
using System.Collections;
using System.IO;
using UnityEngine;

public static class AssetBundleRequestExtensions {

    public static UnityEngine.Object InstantiateAsset(this AssetBundleRequest inst) {
        return UnityEngine.Object.Instantiate(inst.asset);
    }

    public static UnityEngine.Object InstantiateAsset(this AssetBundleRequest inst, Vector3 position, Quaternion rotation) {
        return UnityEngine.Object.Instantiate(inst.asset, position, rotation);
    }

    public static T InstantiateAsset<T>(this AssetBundleRequest inst)
        where T : Component {
        return (UnityEngine.Object.Instantiate(inst.asset) as GameObject).GetComponent<T>();
    }

    public static T InstantiateAsset<T>(this AssetBundleRequest inst, Vector3 position, Quaternion rotation)
        where T : Component {
        return (UnityEngine.Object.Instantiate(inst.asset, position, rotation) as GameObject).GetComponent<T>();
    }
}