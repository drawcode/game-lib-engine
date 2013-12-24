using System;
using System.Collections.Generic;
using System.IO;

public class BaseAppContentAssetCustomItems<T> : DataObjects<T> where T : new() {
    private static T current;
    private static volatile BaseAppContentAssetCustomItems<T> instance;
    private static object syncRoot = new Object();

    private string BASE_DATA_KEY = "app-content-asset-custom-item-data";

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

    public static BaseAppContentAssetCustomItems<T> BaseInstance {
        get {
            if (instance == null) {
                lock (syncRoot) {
                    if (instance == null)
                        instance = new BaseAppContentAssetCustomItems<T>(true);
                }
            }

            return instance;
        }
        set {
            instance = value;
        }
    }

    public BaseAppContentAssetCustomItems() {
        Reset();
    }

    public BaseAppContentAssetCustomItems(bool loadData) {
        Reset();
        path = "data/" + BASE_DATA_KEY + ".json";
        pathKey = BASE_DATA_KEY;
        LoadData();
    }
}


public class AppContentAssetCustomItemProperty {
    public List<string> types = new List<string>();
    public string code = "";
    /*
    public virtual List<string> types {
        get {
            return Get<List<string>>(BaseDataObjectKeys.types);
        }
        
        set {
            Set<List<string>>(BaseDataObjectKeys.types, value);
        }
    }  
    */
}

public class AppContentAssetCustomItemData {

   public List<AppContentAssetCustomItemProperty> properties = new List<AppContentAssetCustomItemProperty>();

    /*
    public virtual List<AppContentAssetCustomItemProperty> properties {
        get {
            return Get<List<AppContentAssetCustomItemProperty>>(BaseDataObjectKeys.properties);
        }
        
        set {
            Set<List<AppContentAssetCustomItemProperty>>(BaseDataObjectKeys.properties, value);
        }
    } 
    */
}

public class BaseAppContentAssetCustomItem : GameDataObject {  

    // Attributes that are added or changed after launch should be like this to prevent
    // profile conversions.    
    
    public virtual List<AppContentAssetCustomItemProperty> properties {
        get {
            return Get<List<AppContentAssetCustomItemProperty>>(BaseDataObjectKeys.properties);
        }
        
        set {
            Set<List<AppContentAssetCustomItemProperty>>(BaseDataObjectKeys.properties, value);
        }
    }  

    public BaseAppContentAssetCustomItem() {
        Reset();
    }

    public override void Reset() {
        base.Reset();
    }
}