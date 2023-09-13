using System;
using System.Collections.Generic;
using UnityEngine;

public static class UnityObjectUtil {

    public static GameObject FindRequiredObject(string name) {
        var obj = GameObject.Find(name);
        if (obj == null)
            throw new InvalidOperationException(string.Format("unable to find required '{0}' game object", name));

        return obj;
    }

    public static T FindRequiredObject<T>(string name)
        where T : Component {
        foreach (var obj in FindObjects<T>()) {
            if (obj.gameObject.name == name)
                return obj;
        }

        throw new InvalidOperationException(string.Format("unable to find required '{0}' game object", name));
    }

    public static T FindRequiredObject<T>()
        where T : UnityEngine.Object {
        T obj = FindObject<T>();
        if (obj == null)
            throw new InvalidOperationException(string.Format("unable to find required '{0}' object", typeof(T).Name));
        return obj;
    }

    public static T FindObject<T>(
        FindObjectsInactive findObjectsInactive = FindObjectsInactive.Include, 
        FindObjectsSortMode findObjectsSortMode = FindObjectsSortMode.None)
        where T : UnityEngine.Object {

            foreach(T obj in FindObjects<T>(findObjectsInactive, findObjectsSortMode)) {
                return obj;
            }
            
        return default(T);
    }

    public static T[] FindObjects<T>(
        FindObjectsInactive findObjectsInactive = FindObjectsInactive.Include, 
        FindObjectsSortMode findObjectsSortMode = FindObjectsSortMode.None)
        where T : UnityEngine.Object {
        return UnityEngine.Object.FindObjectsByType(
            typeof(T), findObjectsInactive, findObjectsSortMode) as T[];
    }
}