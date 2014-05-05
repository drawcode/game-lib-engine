using System;
using System.Collections.Generic;
using System.IO;
using Engine.Data.Json;
using Engine.Utility;

using UnityEngine;

public class AppColors : BaseAppColors<AppColor> {
    private static volatile AppColor current;
    private static volatile AppColors instance;
    private static System.Object syncRoot = new System.Object();
    private string DATA_KEY = "app-color-data";

    public static AppColor Current {
        get {
            if (current == null) {
                lock (syncRoot) {
                    if (current == null) 
                        current = new AppColor();
                }
            }
    
            return current;
        }
        set {
            current = value;
        }
    }
        
    public static AppColors Instance {
        get {
            if (instance == null) {
                lock (syncRoot) {
                    if (instance == null) 
                        instance = new AppColors(true);
                }
            }
    
            return instance;
        }
    }
    
    public AppColors() {
        Reset();
    }
    
    public AppColors(bool loadData) {
        Reset();
        path = "data/" + DATA_KEY + ".json";
        pathKey = DATA_KEY;
        LoadData();
    }      
    
    public static Dictionary<string, Color> cachedColors = new Dictionary<string, Color>();

    public static Color GetColor(string code) {

        Color colorTo = Color.white;

        if(GameConfigs.globalReady) {
                        
            if(!cachedColors.ContainsKey(code)) {

                //if(GameGlobal.Instance != null) {
             
                    AppColor color = AppColors.Instance.GetByCode(code);

                    if(color != null) {
                        colorTo = color.GetColor();
                    }

                    if(colorTo != Color.white) {
                        cachedColors.Add(code, colorTo);            
                    }
                //}

            }
            else {
                colorTo = cachedColors[code];
            }
        }

        return colorTo;
    }
}

public class AppColor : BaseAppColor {
        
    public AppColor() {
        Reset();
    }
    
    public override void Reset() {

    }
    
    /*
    public override void Clone(AppColor toCopy) {
        
        colors = toCopy.colors;
    }
    */
    
}
