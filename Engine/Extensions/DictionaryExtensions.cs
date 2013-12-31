using System;
using System.Collections.Generic;

public static class DictionaryExtensions {

    /*
    // generics
    
    public virtual T Get<T>(this Dictionary<T, U> dict, string code) {
        return Get<T>(code, default(T));
    }
    
    public virtual T Get<T>(this Dictionary<U, T> dict, string code, T defaultValue) {                
        try {
            if (ContainsKey(code)) {
                return (T)this[code];
            }
            return defaultValue;
        }
        catch (Exception e) {
            return default(T);
        }
    }

    
    public virtual void Set<T>(string code, T val) {
        if (ContainsKey(code)) {
            this[code] = val;
        }
        else {
            Add(code, val);
        }
    }
    
    public virtual void Set(string code, object val) {
        if (ContainsKey(code)) {
            this[code] = val;
        }
        else {
            Add(code, val);
        }
    }
    
    public virtual void Set(string code, DataAttribute val) {
        if (attributes == null) {
            attributes = new Dictionary<string, DataAttribute>();                        
        }
        
        
        if (attributes.ContainsKey(code)) {
            attributes[code] = val;
        }
        else {
            attributes.Add(code, val);
        }
    }
    */
    /*
    public static bool ContainsKey(this IList<T> list, string key) {
        if(list == null) {
            return false;
        }

        foreach
    }
    */
}