using System;
using System.Collections.Generic;
using System.IO;
// using Engine.Data.Json;
using Engine.Utility;

public class AppContentAssetModels : BaseAppContentAssetModels<AppContentAssetModel> {
    private static volatile AppContentAssetModel current;
    private static volatile AppContentAssetModels instance;
    private static System.Object syncRoot = new System.Object();
    private string DATA_KEY = "app-content-asset-model-data";
    
    public static AppContentAssetModel Current {
        get {
            if (current == null) {
                lock (syncRoot) {
                    if (current == null) 
                        current = new AppContentAssetModel();
                }
            }
    
            return current;
        }
        set {
            current = value;
        }
    }
        
    public static AppContentAssetModels Instance {
        get {
            if (instance == null) {
                lock (syncRoot) {
                    if (instance == null) 
                        instance = new AppContentAssetModels(true);
                }
            }
    
            return instance;
        }
    }
    
    public AppContentAssetModels() {
        Reset();
    }
    
    public AppContentAssetModels(bool loadData) {
        Reset();
        path = "data/" + DATA_KEY + ".json";
        pathKey = DATA_KEY;
        LoadData();
    }   
}

public class AppContentAssetModel : BaseAppContentAssetModel {
        
    public AppContentAssetModel() {
        Reset();
    }
    
    public override void Reset() {

    }
    
}
