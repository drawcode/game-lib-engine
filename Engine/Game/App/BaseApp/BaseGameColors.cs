using System;
using System.Collections.Generic;
using System.IO;

using UnityEngine;

public class BaseGameColors<T> : DataObjects<T> where T : new() {
    private static T current;
    private static volatile BaseGameColors<T> instance;
    private static System.Object syncRoot = new System.Object();

    private string BASE_DATA_KEY = "game-color-data";

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

    public static BaseGameColors<T> BaseInstance {
        get {
            if (instance == null) {
                lock (syncRoot) {
                    if (instance == null)
                        instance = new BaseGameColors<T>(true);
                }
            }

            return instance;
        }
        set {
            instance = value;
        }
    }

    public BaseGameColors() {
        Reset();
    }

    public BaseGameColors(bool loadData) {
        Reset();
        path = "data/" + BASE_DATA_KEY + ".json";
        pathKey = BASE_DATA_KEY;
        LoadData();
    }
}


public class GameColorValue : DataObject {

    public virtual string type {
        get {
            return Get<string>(BaseDataObjectKeys.type, BaseGameColorKeys.rgba);
        }
        
        set {
            Set<string>(BaseDataObjectKeys.type, value);
        }
    }

    public virtual List<double> rgba {
        get {
            return Get<List<double>>(BaseGameColorKeys.rgba);
        }
        
        set {
            Set<List<double>>(BaseGameColorKeys.rgba, value);
        }
    }
    
    public GameColorValue() {
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
        type = BaseGameColorKeys.rgba;
    }
}

public class BaseGameColorKeys {
    public static string color = "color";
    public static string rgba = "rgba";
    public static string type = "type";
}

public class BaseGameColor : GameDataObject {

    // Attributes that are added or changed after launch should be like this to prevent
    // profile conversions.

    public virtual GameColorValue color {
        get {
            return Get<GameColorValue>(BaseGameColorKeys.color);
        }
        
        set {
            Set<GameColorValue>(BaseGameColorKeys.color, value);
        }
    }

    public BaseGameColor() {
        Reset();
    }

    public override void Reset() {
        base.Reset();
    }

    public void Clone(BaseGameColor toCopy) {
        base.Clone(toCopy);
    }     
    
    public Color GetColor() {
        return color.GetColor();
    }

    // Attributes that are added or changed after launch should be like this to prevent
    // profile conversions.
}