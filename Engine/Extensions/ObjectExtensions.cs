using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

using Engine.Data.Json;

using UnityEngine;
using System.Text;

public enum JsonUtilType {
    JsonMapper,
    UnityJsonUtility
}

public static class ObjectExtensions {

    public static string ToJson(
        this object inst,
        bool prettyPrint = true,
        bool filter = true,
        JsonUtilType jsonUtilType = JsonUtilType.JsonMapper) {

        if (inst == null) {
            return null;
        }

        try {

            if (jsonUtilType == JsonUtilType.UnityJsonUtility) {
                if (filter) {
                    return JsonUtility.ToJson(inst, prettyPrint).FilterJson();
                }
                else {
                    return JsonUtility.ToJson(inst, prettyPrint);
                }
            }
            else {

                StringBuilder val = new StringBuilder();
                
                JsonWriter writer = new JsonWriter(val);
                writer.PrettyPrint = prettyPrint; 
                writer.IndentValue = 2;
                
                JsonMapper.ToJson(inst, writer);

                string valString = val.ToString();

                if (filter) {
                    return valString.FilterJson();
                }
                else {
                    return valString;
                }
            }
        }
        catch (Exception e) {
            Debug.LogError("ToJson:FAILED:" + e);
            return null;
        }
    }

    //public static string ToJson<T>(this T inst) {
    //    if (inst == null) {
    //        return null;
    //    }

    //    try {
    //        return JsonMapper.ToJson(inst).FilterJson();
    //        //return JsonUtility.ToJson(inst).FilterJson();
    //    }
    //    catch (Exception e) {
    //        LogUtil.LogError("ToJson:FAILED:" + e);
    //        return null;
    //    }
    //}

    public static Dictionary<string, object> FromJsonToDict(
        this string inst,
        bool filter = true,
        JsonUtilType jsonUtilType = JsonUtilType.JsonMapper) {

        if (jsonUtilType == JsonUtilType.UnityJsonUtility) {

            if (filter) {
                return JsonUtility.FromJson<Dictionary<string, object>>(
                    inst.FilterJson());
            }
            else {
                return JsonUtility.FromJson<Dictionary<string, object>>(
                    inst);
            }
        }
        else {

            if (filter) {
                return JsonMapper.ToObject<Dictionary<string, object>>(
                    inst.FilterJson());
            }
            else {
                return JsonMapper.ToObject<Dictionary<string, object>>(
                    inst);
            }
        }
    }

    public static List<Dictionary<string, object>> FromJsonToDictList(
        this string inst,
        bool filter = true,
        JsonUtilType jsonUtilType = JsonUtilType.JsonMapper) {

        if (jsonUtilType == JsonUtilType.UnityJsonUtility) {

            if (filter) {
                return JsonUtility.FromJson<List<Dictionary<string, object>>>(
                    inst.FilterJson());
            }
            else {
                return JsonUtility.FromJson<List<Dictionary<string, object>>>(
                    inst);
            }
        }
        else {

            if (filter) {
                return JsonMapper.ToObject<List<Dictionary<string, object>>>(
                    inst.FilterJson());
            }
            else {
                return JsonMapper.ToObject<List<Dictionary<string, object>>>(
                    inst);
            }
        }
    }

    public static T FromJson<T>(
        this string inst,
        bool filter = true,
        JsonUtilType jsonUtilType = JsonUtilType.JsonMapper) {

        if (jsonUtilType == JsonUtilType.UnityJsonUtility) {

            if (filter) {
                return JsonUtility.FromJson<T>(inst.FilterJson());
            }
            else {
                return JsonUtility.FromJson<T>(inst);
            }
        }
        else {

            if (filter) {
                return JsonMapper.ToObject<T>(inst.FilterJson());
            }
            else {
                return JsonMapper.ToObject<T>(inst);
            }
        }
    }

    public static object FromJson(
        this string inst,
        bool filter = true,
        JsonUtilType jsonUtilType = JsonUtilType.JsonMapper) {

        if (jsonUtilType == JsonUtilType.UnityJsonUtility) {

            if (filter) {
                return JsonUtility.FromJson<object>(inst.FilterJson());
            }
            else {
                return JsonUtility.FromJson<object>(inst);
            }
        }
        else {
            if (filter) {
                return JsonMapper.ToObject<object>(inst.FilterJson());
            }
            else {
                return JsonMapper.ToObject<object>(inst);
            }
        }
    }

    //

    public static T ToDataObject<T>(
        this object val,
        bool filter = true,
        JsonUtilType jsonUtilType = JsonUtilType.JsonMapper) {

        if (val == null) {
            return default(T);
        }

        return val.ToJson(true, filter, jsonUtilType).FromJson<T>(filter, jsonUtilType);
    }

    public static string FilterJson(this string val) {
        if (string.IsNullOrEmpty(val))
            return val;
        return val
            .Replace("\\\"", "\"")
            //.Replace("\\\"", "\"")
            //.Replace(":\"{", ":{")
            //.Replace("}\",", "},")
            ////.Replace("}\"", "}")
            //.Replace(":\"[", ":[")
            //.Replace("]\",", "],")
            ////.Replace("]\"", "]")
            .TrimStart('"').TrimEnd('"');
    }

    public static object ConvertJson(this string val) {
        if (val.StartsWith("{") || val.StartsWith("[")) {
            try {

                if (val.TrimStart().StartsWith("{")) {
                    return val.FilterJson().FromJson<Dictionary<string, object>>();
                }
                else if (val.TrimStart().StartsWith("[")) {
                    return val.FilterJson().FromJson<List<object>>();
                }

            }
            catch (Exception e) {
                UnityEngine.Debug.Log("ERROR parsing attribute:" + e);
            }
        }

        return val;
    }

    /*
    public static int GetObjectSize(this object val) {
        if (val == null) {
            return 0;
        }

        BinaryFormatter bf = new BinaryFormatter();
        MemoryStream ms = new MemoryStream();
        byte[] Array;
        bf.Serialize(ms, val.GetType());
        Array = ms.ToArray();
        return Array.Length;
    }
    */

    public static long GetObjectSize(this object o) {

        long size = 0;
        using (Stream s = new MemoryStream()) {
            BinaryFormatter formatter = new BinaryFormatter();
            formatter.Serialize(s, o);
            size = s.Length;
        }

        return size;
    }
}