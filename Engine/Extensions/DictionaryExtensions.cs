using System;
using System.Collections.Generic;

public static class DictionaryExtensions {

    public static bool ChangeKey<TKey, TValue>(
        this IDictionary<TKey, TValue> dict,
        TKey oldKey, TKey newKey) {

        TValue value;
        if (!dict.TryGetValue(oldKey, out value))
            return false;

        dict.Remove(oldKey);  // do not change order
        dict[newKey] = value;  // or dict.Add(newKey, value) depending on ur comfort
        return true;
    }

    public static void Merge<TKey, TValue>(
        this IDictionary<TKey, TValue> dict, IDictionary<TKey, TValue> dictNew) {
        if (dict != null && dictNew != null) {
            foreach (KeyValuePair<TKey, TValue> pair in dictNew) {
                dict.Set(pair.Key, pair.Value);
            }
        }
    }

    public static void Merge(
        this IDictionary<string, object> dict, IDictionary<string, object> dictNew) {

        if (dict != null && dictNew != null) {
            foreach (KeyValuePair<string, object> pair in dictNew) {
                dict.Set(pair.Key, pair.Value);
            }
        }
    }

    public static void Set(
        this IDictionary<string, object> dict, string key, object val) {

        if (dict != null) {
            if (dict.ContainsKey(key)) {
                dict[key] = val;
            }
            else {
                dict.Add(key, val);
            }
        }
    }

    public static void Set<TKey, TValue>(
        this IDictionary<TKey, TValue> dict, TKey key, TValue val) {

        if (dict != null) {
            if (dict.ContainsKey(key)) {
                dict[key] = val;
            }
            else {
                dict.Add(key, val);
            }
        }
    }

    public static object Get(this IDictionary<string, object> dict, string key) {
        return dict.Get<object>(key);
    }

    public static T Get<T>(this IDictionary<string, object> dict, string key) {
        if (dict != null) {

            if (dict.ContainsKey(key)) {
                return (T)dict[key];
            }
            if (dict.ContainsKey(key.ToLower())) {
                return (T)dict[key.ToLower()];
            }
        }

        return default(T);
    }

    public static T Get<T>(this IDictionary<string, T> dict, string key) {
        if (dict != null) {
            if (dict.ContainsKey(key)) {
                return (T)(dict[key]);
            }
        }

        return default(T);
    }

    public static TValue Get<TKey, TValue>(this IDictionary<TKey, TValue> dict, TKey key) {
        if (dict != null) {
            if (dict.ContainsKey(key)) {
                return dict[key];
            }
        }

        return default(TValue);
    }

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

    public static List<Dictionary<string, object>> RemoveAllKeyStartsWith(
        this List<Dictionary<string, object>> dicts, string keyStartsWith) {

        if (dicts != null) {
            foreach (Dictionary<string, object> dict in dicts) {

                List<string> toRemove = new List<string>();

                foreach (KeyValuePair<string, object> pair in dict) {
                    if (pair.Key.StartsWith(keyStartsWith)) {
                        toRemove.Add(pair.Key);
                    }
                }

                foreach (string key in toRemove) {
                    dict.Remove(key);
                }
            }
        }

        return dicts;
    }

    /*
    public static IList<IDictionary<string, TValue>> RemoveAllKeyStartsWith<TValue>(
        this IList<IDictionary<string, TValue>> dicts, string keyStartsWith) {

        if (dicts != null) {
            foreach (IDictionary<string, TValue> dict in dicts) {

                List<string> toRemove = new List<string>();

                foreach (KeyValuePair<string, TValue> pair in dict) {
                    if (pair.Key.StartsWith(keyStartsWith)) {
                        toRemove.Add(pair.Key);
                    }
                }

                foreach (string key in toRemove) {
                    dict.Remove(key);
                }
            }
        }

        return dicts;
    }
     * */
     
    public static IDictionary<string, TValue> RemoveAllKeyStartsWith<TValue>(
        this IDictionary<string, TValue> dict, string keyStartsWith) {
        if (dict != null) {

            List<string> toRemove = new List<string>();

            foreach (KeyValuePair<string, TValue> pair in dict) {
                if (pair.Key.StartsWith(keyStartsWith)) {
                    toRemove.Add(pair.Key);
                }
            }

            foreach (string key in toRemove) {
                dict.Remove(key);
            }
        }

        return dict;
    }

    public static IDictionary<string, TValue> RemoveAllKeyContains<TValue>(
        this IDictionary<string, TValue> dict, string keyContains) {
        if (dict != null) {

            List<string> toRemove = new List<string>();

            foreach (KeyValuePair<string, TValue> pair in dict) {
                if (pair.Key.Contains(keyContains)) {
                    toRemove.Add(pair.Key);
                }
            }

            foreach (string key in toRemove) {
                dict.Remove(key);
            }
        }

        return dict;
    }

    public static Dictionary<TKey, TValue> CloneDictionaryWithValues<TKey, TValue>(
        this Dictionary<TKey, TValue> original) {

        Dictionary<TKey, TValue> ret =
            new Dictionary<TKey, TValue>(
                original.Count, original.Comparer);

        foreach (KeyValuePair<TKey, TValue> entry in original) {
            ret.Add(entry.Key, entry.Value);
        }

        return ret;
    }
}