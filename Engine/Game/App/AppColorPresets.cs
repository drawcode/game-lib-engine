using System;
using System.Collections.Generic;
using System.IO;
using Engine.Data.Json;
using Engine.Utility;

using UnityEngine;

public class AppColorPresets : BaseAppColorPresets<AppColorPreset> {
    private static volatile AppColorPreset current;
    private static volatile AppColorPresets instance;
    private static System.Object syncRoot = new System.Object();
    private string DATA_KEY = "app-color-preset-data";
        
    public static Dictionary<string, Color> cachedColors = new Dictionary<string, Color>();
        
    public static AppColorPreset Current {
        get {
            if (current == null) {
                lock (syncRoot) {
                    if (current == null) 
                        current = new AppColorPreset();
                }
            }
    
            return current;
        }
        set {
            current = value;
        }
    }
        
    public static AppColorPresets Instance {
        get {
            if (instance == null) {
                lock (syncRoot) {
                    if (instance == null) 
                        instance = new AppColorPresets(true);
                }
            }
    
            return instance;
        }
    }
    
    public AppColorPresets() {
        Reset();
    }
    
    public AppColorPresets(bool loadData) {
        Reset();
        path = "data/" + DATA_KEY + ".json";
        pathKey = DATA_KEY;
        LoadData();
    }      

    public static Color GetColor(string code) {

        Color colorTo = Color.white;
                
        if(!cachedColors.ContainsKey(code)) {
         
            AppColor color = AppColors.Instance.GetByCode(code);

            if(color != null) {
                colorTo = color.GetColor();
            }

            cachedColors.Add(code, colorTo);
        }
        else {
            colorTo = cachedColors[code];
        }

        return colorTo;
    }

    public static string GetColorCodeByItemCode(string customItemCode) {
        return "";//List<AppColorPreset>
    }

    public static Color GetColorByItemCode(string customItemCode) { // helmet, jersey etc
        
        Color colorTo = Color.white;

        //if(AppContentAssetCustomItems.
        
        if(!cachedColors.ContainsKey(customItemCode)) {
            
            AppColor color = AppColors.Instance.GetByCode(customItemCode);
            
            if(color != null) {
                colorTo = color.GetColor();
            }
            
            cachedColors.Add(customItemCode, colorTo);
        }
        else {
            colorTo = cachedColors[customItemCode];
        }
        
        return colorTo;
    }
}

public class AppColorPreset : BaseAppColorPreset {
        
    public AppColorPreset() {
        Reset();
    }
    
    public override void Reset() {

    }
    
    /*
    public override void Clone(AppColorPreset toCopy) {
        
        colors = toCopy.colors;
    }
    */
    
}
