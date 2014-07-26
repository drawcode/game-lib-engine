using System;
using System.Collections.Generic;
using System.Linq;
//using System.Web;
using System.Text.RegularExpressions;
    
public class Paths {
        
    /*
    public static string paramValueSeparatorPath = "/";
    public static string paramValueSeparatorPathFilter = "--";
        
    public static string urlSchemeAndHost {
        get {
            return HttpContext.Current.Request.Url.Scheme +
                System.Uri.SchemeDelimiter +
                HttpContext.Current.Request.Url.Host +
                (HttpContext.Current.Request.Url.IsDefaultPort ? "" : ":" + HttpContext.Current.Request.Url.Port);
        }
    }
        
    // Fixes case there Path.Combine has latter root path and overrides on occasion.
    public static string Combine(string path1, string path2) {
        if (path1.EndsWith("/")) {
            path1 = path1.TrimEnd('/');
        }
        if (path2.EndsWith("/")) {
            path2 = path2.TrimEnd('/');
        }
            
        return path1 + "/" + path2;
    }
        
    public static bool FilterUrl(string input, string pattern) {
        //^[A-Z][a-z]*( [A-Z][a-z]*)*$
            
        if (input.IsRegexMatch(pattern)) {
            return true;
        }
        return false;
    }
        
    public static string FilterToUrlFormat(string input) {
        //input = HttpUtility.UrlDecode(input);
        //input = input.StripToAlphanumerics();
        input = Regex.Replace(input, "_", "-");
        input = Regex.Replace(input, " ", "-");
        input = input.ToLower();
        return input;
    }
        
    public static string GetPathParamValue(string path, string key) {
        return GetPathParamValue(path, Paths.paramValueSeparatorPath, key);
    }
        
    public static string GetPathParamFilterValue(string path, string key) {
        return GetPathParamValue(path, Paths.paramValueSeparatorPathFilter, key);
    }
        
    public static string GetPathParamValue(string path, string separator, string key) {
        var value = "";
        if (path.IndexOf(key + separator) > -1) {
            var parts = GetPathParamValueParts(path, separator);
            for (var i = 0; i < parts.Length; i++) {
                if (parts[i] == key) {
                    if (parts[i + 1] != null) {
                        value = parts[i + 1];
                    }
                }
            }
        }
        return value;        
    }
        
    public static Dictionary<string, object> GetPathParamValues() {
        string path = HttpContext.Current.Request.RawUrl;
        return GetPathParamValues(path);
    }
        
    public static Dictionary<string, object> GetPathParamValuesKey(string key) {
            
        Dictionary<string, object> values = GetPathParamValues();
            
        if (values != null) {
            if (values.ContainsKey(key)) {
                return (Dictionary<string, object>)values[key]; 
            }
        }
            
        return new Dictionary<string, object>();
    }
        
    public static Dictionary<string, object> GetPathParamValues(string path, string separator = "/", string separatorFilter = "--") {
            
        Dictionary<string, object> values = new Dictionary<string, object>();
            
        values = GetPathParamValues(path, separator);
        foreach (KeyValuePair<string, object> pair in values) {
            string pathFilter = (string)pair.Value;
            values[pair.Key] = GetPathParamValues(path, separatorFilter);
        }
            
        return values;
    }
        
    public static Dictionary<string, object> GetPathParamValues(string path, string separator) {
        Dictionary<string, object> values = new Dictionary<string, object>();
        if (string.IsNullOrEmpty(path)) {
            return values;
        }
            
        var parts = GetPathParamValueParts(path, separator);
        for (var i = 0; i < parts.Length; i++) {
            if (((i % 2) == 0)) {
                string key = parts[i];
                if (!values.ContainsKey(key)) {
                    values.Add(key, "");
                }
                if (parts.Length > i + 1) {
                    if (parts[i + 1] != null) {
                        values[key] = parts[i + 1];
                    }
                }
            }
        }
        return values;
    }
        
    public static string[] GetPathParamValueParts(string path, string separator) {
        if (string.IsNullOrEmpty(path)) {
            return null;
        }
        return path.Split(new string[] { separator }, StringSplitOptions.None);
    }
        
    public static string ChangePathParamValue(string path, string param, string paramvalue) {
        return ChangePathParamValue(path, Paths.paramValueSeparatorPath, param, paramvalue);
    }
        
    public static string ChangePathParamFilterValue(string path, string param, string paramvalue) {
        return ChangePathParamValue(path, Paths.paramValueSeparatorPathFilter, param, paramvalue);
    }
        
    public static string ChangePathParamValue(string path, string separator, string param, string paramvalue) {
        var url = "";
        if (path.IndexOf(param) > -1) {
            // hash contains param, just update
            var parts = GetPathParamValueParts(path, separator);
            for (var i = 0; i < parts.Length; i++) {
                if (parts[i] == param) {
                    parts[i + 1] = paramvalue;
                }
            }
            url = string.Join(separator, parts);
        }
        else {
            // has not present, append it
            if (path.IndexOf(param + separator) == -1) {
                if (path.LastIndexOf(separator) == path.Length - 1) {
                    path = path.Substring(0, path.Length - 1);
                }
                url = path + separator + param + separator + paramvalue;
            }
        }
        return url;
    }
        
    public static string RemovePathParamValue(string path, string param) {
        return RemovePathParamValue(path, Paths.paramValueSeparatorPath, param);
    }
        
    public static string RemovePathParamFilterValue(string path, string param) {
        return RemovePathParamValue(path, Paths.paramValueSeparatorPathFilter, param);
    }
        
    public static string RemovePathParamValue(string path, string separator, string param) {
        var url = "";
        if (path.IndexOf(param) > -1) {
            // hash contains param, just update
            string[] parts = GetPathParamValueParts(path, separator);
            List<string> partsNew = new List<string>();
            for (var i = 0; i < parts.Length; i++) {
                if (parts[i] == param) {
                    i = i + 1;
                }
                else {
                    partsNew.Add(parts[i]);
                }
            }
            url = string.Join(separator, partsNew.ToArray());
        }
        return url;
    }
        
    public static string FilterUrlParam(string url, string param) {
        string paramValue = Paths.GetPathParamValue(url, param);
        if (!string.IsNullOrEmpty(paramValue)) {
            paramValue = paramValue.FilterToUrlFormat();
            url = url.ChangePathParamValue(param, paramValue);
        }
        return url;
    }
        
    public static bool IsAbsoluteUrl(string input) {
        string pattern = @"[-a-zA-Z0-9@:%_\+.~#?&//=]{2,256}\.[a-z]{2,4}\b(\/[-a-zA-Z0-9@:%_\+.~#?&//=]*)?";
        return input.IsRegexMatch(pattern);
    }
 */       
}