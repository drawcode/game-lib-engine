using System;
using System.Collections.Generic;
using System.IO;

using UnityEngine;

public class BaseAppColors<T> : DataObjects<T> where T : new() {
    private static T current;
    private static volatile BaseAppColors<T> instance;
    private static System.Object syncRoot = new System.Object();

    private string BASE_DATA_KEY = "app-color-data";

    public static T BaseCurrent {
        get {
            if (current == null) {
                lock (syncRoot) {
                    if (current == null)
                        current = new T();
                }
            }

            return current;
        }
        set {
            current = value;
        }
    }

    public static BaseAppColors<T> BaseInstance {
        get {
            if (instance == null) {
                lock (syncRoot) {
                    if (instance == null)
                        instance = new BaseAppColors<T>(true);
                }
            }

            return instance;
        }
        set {
            instance = value;
        }
    }

    public BaseAppColors() {
        Reset();
    }

    public BaseAppColors(bool loadData) {
        Reset();
        path = "data/" + BASE_DATA_KEY + ".json";
        pathKey = BASE_DATA_KEY;
        LoadData();
    }
}


public class AppColorValue : DataObject {

    public virtual string type {
        get {
            return Get<string>(BaseDataObjectKeys.type, BaseAppColorKeys.rgba);
        }
        
        set {
            Set<string>(BaseDataObjectKeys.type, value);
        }
    }

    public virtual List<double> rgba {
        get {
            return Get<List<double>>(BaseAppColorKeys.rgba);
        }
        
        set {
            Set<List<double>>(BaseAppColorKeys.rgba, value);
        }
    }
    
    public AppColorValue() {
        Reset();
    }
    
    public void SetColor(float r, float g, float b, float a) {

        if(rgba == null) {            
            rgba = new List<double>();
            rgba.Add(r);
            rgba.Add(g);
            rgba.Add(b);
            rgba.Add(a);
        }
        else {
        
            rgba[0] = r;
            rgba[1] = g;
            rgba[2] = b;
            rgba[3] = a;
        }
    }

    public Color GetColor() {
        return ColorHelper.FromRGB(rgba);
    }
    
    public void Reset() {
        float r = 1;
        float g = 1;
        float b = 1;
        float a = 1;
        SetColor(r, g, b, a);
        type = BaseAppColorKeys.rgba;
    }
}

public class BaseAppColorKeys {
    public static string color = "color";
    public static string rgba = "rgba";
    public static string type = "type";
}

public class BaseAppColor : GameDataObject {

    // Attributes that are added or changed after launch should be like this to prevent
    // profile conversions.

    public virtual AppColorValue color {
        get {
            return Get<AppColorValue>(BaseAppColorKeys.color);
        }
        
        set {
            Set<AppColorValue>(BaseAppColorKeys.color, value);
        }
    }

    public BaseAppColor() {
        Reset();
    }

    public override void Reset() {
        base.Reset();
    }

    public void Clone(BaseAppColor toCopy) {
        base.Clone(toCopy);
    }     
    
    public Color GetColor() {
        return color.GetColor();
    }

    // Attributes that are added or changed after launch should be like this to prevent
    // profile conversions.
}