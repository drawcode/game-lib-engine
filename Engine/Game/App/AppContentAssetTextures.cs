using System;
using System.Collections.Generic;
using System.IO;
using Engine.Data.Json;
using Engine.Utility;

public class AppContentAssetTextures : BaseAppContentAssetTextures<AppContentAssetTexture> {
    private static volatile AppContentAssetTexture current;
    private static volatile AppContentAssetTextures instance;
    private static System.Object syncRoot = new System.Object();
    private string DATA_KEY = "app-content-asset-texture-data";
    
    public static AppContentAssetTexture Current {
        get {
            if (current == null) {
                lock (syncRoot) {
                    if (current == null) 
                        current = new AppContentAssetTexture();
                }
            }
    
            return current;
        }
        set {
            current = value;
        }
    }
        
    public static AppContentAssetTextures Instance {
        get {
            if (instance == null) {
                lock (syncRoot) {
                    if (instance == null) 
                        instance = new AppContentAssetTextures(true);
                }
            }
    
            return instance;
        }
    }
    
    public AppContentAssetTextures() {
        Reset();
    }
    
    public AppContentAssetTextures(bool loadData) {
        Reset();
        path = "data/" + DATA_KEY + ".json";
        pathKey = DATA_KEY;
        LoadData();
    }   
}

public class AppContentAssetTexture : BaseAppContentAssetTexture {
        
    public AppContentAssetTexture() {
        Reset();
    }
    
    public override void Reset() {

    }
    
}
