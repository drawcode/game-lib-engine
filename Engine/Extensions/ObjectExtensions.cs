using System;

using UnityEngine;

public static class ObjectExtensions {
    
    public static string ToJson(this object inst) {
        if(inst == null) {
            return null;
        }

        try {
            return Engine.Data.Json.JsonMapper.ToJson(inst);
        }
        catch(Exception e) {
            Debug.LogError("ToJson:FAILED:" + e);
            return null;
        }
    }

    public static string ToJson<T>(this T inst) {
        if(inst == null) {
            return null;
        }
        
        try {
            return Engine.Data.Json.JsonMapper.ToJson(inst);
        }
        catch(Exception e) {
            Debug.LogError("ToJson:FAILED:" + e);
            return null;
        }
    }
    
    public static T FromJson<T>(this string inst) {
        return Engine.Data.Json.JsonMapper.ToObject<T>(inst);
    }
    
    public static object FromJson(this string inst) {
        return Engine.Data.Json.JsonMapper.ToObject<object>(inst);
    }
    
    
}