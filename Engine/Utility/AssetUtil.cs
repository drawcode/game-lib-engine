using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using UnityEngine;

public static class AssetUtil {

    public static T LoadAsset<T>(string path) where T : UnityEngine.Object {
        return LoadAssetFromResources<T>(path);
    }

    public static T LoadAssetFromResources<T>(string path) where T : UnityEngine.Object {

        T obj = Resources.Load(path) as T;

        return obj;

        /*
        ResourceRequest resourceRequest = 
            Resources.LoadAsync<T> (path);
        while (!resourceRequest.isDone) {
            //continue;
        }

        return resourceRequest.asset as T;
        */
    }

    /*
    public static T LoadAssetFromBundle<T>(string path) where T : UnityEngine.Object {
        
        AssetBundleRequest resourceRequest = 
            AssetBundleRequest. Resources.LoadAsync<T> (path);
        while (!resourceRequest.isDone) {
            continue;
        }
        
        return resourceRequest.asset as T;
    }
    */
}