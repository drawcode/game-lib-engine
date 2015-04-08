using System;
using System.Collections.Generic;

public static class DictionaryExtensions {


    public static T Get<T>(this Dictionary<string, T> dict, string code) {
        return dict.Get<string, T>(code, default(T));
    }

    public static object Get(this Dictionary<string, object> dict, string code) {
        return dict.Get<string, object>(code, null);
    }

    public static TVal Get<TKey, TVal>(this Dictionary<TKey, TVal> dict, TKey code) {
        return dict.Get<TKey, TVal>(code, default(TVal));
    }
    
    public static TVal Get<TKey, TVal>(this Dictionary<TKey, TVal> dict, TKey code, TVal defaultValue) {  
        if (dict == null) {
            return default(TVal);
        }

        try {
            if (dict.ContainsKey(code)) {
                return (TVal)dict[code];
            }
            return defaultValue;
        }
        catch (Exception e) {
            LogUtil.Log(e);
            return default(TVal);
        }
    }

    public static void Set<T>(this Dictionary<string, T> dict, string code, T val) {
        dict.Set<string, T>(code, val);
    }
    
    public static void Set(this Dictionary<string, object> dict, string code, object val) {
        dict.Set<string, object>(code, val);
    }

    public static void Set<TKey, TVal>(this Dictionary<TKey, TVal> dict, TKey code, TVal val) {
        if (dict == null) {
            return;
        }
        
        if (dict.ContainsKey(code)) {
            dict[code] = val;
        }
        else {
            dict.Add(code, val);
        }
    }

    public static bool Has<T>(this Dictionary<string, T> dict, string code) {
        return dict.Has<string, T>(code);
    }

    public static bool Has(this Dictionary<string, object> dict, string code) {
        return dict.Has<string, object>(code);
    }

    public static bool Has<TKey, TVal>(this Dictionary<TKey, TVal> dict, TKey code) {
        if (dict == null) {
            return false;
        }
        
        if (dict.ContainsKey(code)) {
            return true;
        }

        return false;
    }
}