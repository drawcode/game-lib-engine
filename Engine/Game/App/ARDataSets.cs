using System;
using System.Collections.Generic;
using System.IO;

using Engine.Utility;

public class ARDataSets : BaseARDataSets<ARDataSet> {
    private static volatile ARDataSet current;
    private static volatile ARDataSets instance;
    private static object syncRoot = new System.Object();
    public static string DATA_KEY = "ar-data-set-data";
        
    public static ARDataSet Current {
        get {
            if (current == null) {
                lock (syncRoot) {
                    if (current == null) 
                        current = new ARDataSet();
                }
            }
    
            return current;
        }
        set {
            current = value;
        }
    }
        
    public static ARDataSets Instance {
        get {
            if (instance == null) {
                lock (syncRoot) {
                    if (instance == null) 
                        instance = new ARDataSets(true);
                }
            }
    
            return instance;
        }
    }
    
    public ARDataSets() {
        Reset();
    }
    
    public ARDataSets(bool loadData) {
        Reset();
        path = "data/" + DATA_KEY + ".json";
        pathKey = DATA_KEY;
        LoadData();
    }
    
    public void ChangeCurrent(string code) {
        Current = GetById(code);
        LogUtil.Log("Changing :" + Current.GetType() + ": code:" + code);
        LogUtil.Log("Changing :" + Current.GetType() + ": name:" + Current.name);
    }
    
    public void LoadDataSetFile(string setName) {
        
        //string pathToCheck = GetDataSetStoragePath(setName);
        
        if (CheckIfARDataSetAvailable(setName)) {
            
        }
    }
    
    public string GetDataSetStoragePath(string setName) {
        string downloadsStoragePath = PathUtil.AppContentPersistencePath;
        
        string pathToCheck = PathUtil.Combine(downloadsStoragePath, setName);
        
        return pathToCheck;
    }
    
    public bool CheckIfARDataSetAvailable(string setName) {
        
        // Check file path for dataset if file exists
        bool dataSetFileExists = false;
                
        string pathToCheck = GetDataSetStoragePath(setName);
        
        if (FileSystemUtil.CheckFileExists(pathToCheck)) {
            dataSetFileExists = true;
            //dataSetFileExists = DataSet.Exists(pathToCheck, DataSet.StorageType.STORAGE_ABSOLUTE);
        }
        
        // Check if ardataset has the dataset
        bool dataSetMetaExists = false;
        
        if (CheckById(setName)) {
            dataSetMetaExists = true;
        }
        
        // if file exists and dataset does not have the data fill it
        if (dataSetFileExists && !dataSetMetaExists) {
            ARDataSet newDataSet = new ARDataSet();
            newDataSet.active = true;
            newDataSet.attributes = new Dictionary<string, DataAttribute>();
            newDataSet.code = setName;
            newDataSet.description = setName;
            newDataSet.display_name = setName;
            newDataSet.game_id = setName;
            newDataSet.key = setName;
            newDataSet.name = setName;
            newDataSet.order_by = "";
            newDataSet.sort_order = 0;
            newDataSet.sort_order_type = 0;
            newDataSet.status = "";
            newDataSet.type = "dataset";
            newDataSet.uuid = setName;
            items.Add(newDataSet);
        }
        
        
        return true;
    }
}

public class ARDataSet : BaseARDataSet {    
    // Attributes that are added or changed after launch should be like this to prevent
    // profile conversions.
            
    public ARDataSet() {
        Reset();
    }
    
    public override void Reset() {
        base.Reset();
    }
}
