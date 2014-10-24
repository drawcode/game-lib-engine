using System;
using System.Collections.Generic;
using System.IO;

public class BaseAppContentAssetCustomItems<T> : DataObjects<T> where T : DataObject, new() {
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

    public AppContentAssetCustomItem GetByCodeAndType(string code, string type) {
        return AppContentAssetCustomItems.Instance.GetAll().Find(
            u => u.code == code && u.type == type);
    }
}

public class AppContentAssetCustomItemProperty : GameDataObject {
    //public List<string> types = new List<string>();
    //public string code = "";

  
}

public class AppContentAssetCustomItemData : GameDataObject {

   //public List<AppContentAssetCustomItemProperty> properties = new List<AppContentAssetCustomItemProperty>();


    public virtual List<AppContentAssetCustomItemProperty> properties {
        get {
            return Get<List<AppContentAssetCustomItemProperty>>(BaseDataObjectKeys.properties);
        }
        
        set {
            Set<List<AppContentAssetCustomItemProperty>>(BaseDataObjectKeys.properties, value);
        }
    } 

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