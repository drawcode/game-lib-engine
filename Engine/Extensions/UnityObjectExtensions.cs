using System;
using System.Collections.Generic;
using UnityEngine;

public static class ObjectUtil {

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

    public static T FindObject<T>()
        where T : UnityEngine.Object {
        return UnityEngine.Object.FindObjectOfType(typeof(T)) as T;
    }

    public static T[] FindObjects<T>()
        where T : UnityEngine.Object {
        return UnityEngine.Object.FindObjectsOfType(typeof(T)) as T[];
    }
}

public static class ObjectExtensions {

    public static string ToJson(this object inst) {
        return Engine.Data.Json.JsonMapper.ToJson(inst);
    }

    public static T FromJson<T>(this string inst) {
        return Engine.Data.Json.JsonMapper.ToObject<T>(inst);
    }
        
    public static object FromJson(this string inst) {
        return Engine.Data.Json.JsonMapper.ToObject<object>(inst);
    }


}