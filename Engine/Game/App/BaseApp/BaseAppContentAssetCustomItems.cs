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

public class AppContentAssetCustomItemProperty : DataObjectItem {
    public List<string> types { get; set; }
}

public class AppContentAssetCustomItemData {
    public List<AppContentAssetCustomItemProperty> properties { get; set; }
}

public class BaseAppContentAssetCustomItem : GameDataObject {

    public virtual AppContentAssetCustomItemData data {
        get {
            return Get<AppContentAssetCustomItemData>(BaseDataObjectKeys.data);
        }
        
        set {
            Set<AppContentAssetCustomItemData>(BaseDataObjectKeys.data, value);
        }
    }      

    // Attributes that are added or changed after launch should be like this to prevent
    // profile conversions.

    public BaseAppContentAssetCustomItem() {
        Reset();
    }

    public override void Reset() {
        base.Reset();
    }
}