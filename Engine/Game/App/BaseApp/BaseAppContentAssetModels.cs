using System;
using System.Collections.Generic;
using System.IO;

using UnityEngine;

public class BaseAppContentAssetModels<T> : DataObjects<T> where T : new() {
    private static T current;
    private static volatile BaseAppContentAssetModels<T> instance;
    private static System.Object syncRoot = new System.Object();

    private string BASE_DATA_KEY = "app-content-asset-model-data";

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

    public static BaseAppContentAssetModels<T> BaseInstance {
        get {
            if (instance == null) {
                lock (syncRoot) {
                    if (instance == null)
                        instance = new BaseAppContentAssetModels<T>(true);
                }
            }

            return instance;
        }
        set {
            instance = value;
        }
    }

    public BaseAppContentAssetModels() {
        Reset();
    }

    public BaseAppContentAssetModels(bool loadData) {
        Reset();
        path = "data/" + BASE_DATA_KEY + ".json";
        pathKey = BASE_DATA_KEY;
        LoadData();
    }

    public static GameObject LoadModel(string code) {
        AppContentAssetModel model = AppContentAssetModels.Instance.GetByCode(code);
        if(model != null) {
            string assetCode = model.asset;
            return AppContentAssets.LoadAsset(assetCode);
        }
        return null;
    }
}

public class BaseAppContentAssetModel : GameDataObject {

    // Attributes that are added or changed after launch should be like this to prevent
    // profile conversions.

    
    public virtual List<AppContentAssetCustomItemProperty> custom_materials {
        get {
            return Get<List<AppContentAssetCustomItemProperty>>(BaseDataObjectKeys.custom_materials);
        }
        
        set {
            Set(BaseDataObjectKeys.custom_materials, value);
        }
    }
        
    public virtual string custom_items {
        get {
            return Get<string>(BaseDataObjectKeys.custom_items);
        }
        
        set {
            Set(BaseDataObjectKeys.custom_items, value);
        }
    }

    public BaseAppContentAssetModel() {
        Reset();
    }

    public override void Reset() {
        base.Reset();
    }

    public void Clone(BaseAppContentAssetModel toCopy) {
        base.Clone(toCopy);
    }
    
    
    public GameObject LoadModel() {
        return AppContentAssetModels.LoadModel(code);
    }

    public virtual List<AppContentAssetCustomItemProperty> GetCustomMaterials() {
        return custom_materials;
    }    
    
    public virtual AppContentAssetCustomItem GetCustomItems() {
        return AppContentAssetCustomItems.Instance.GetByCode(custom_items);
    }

    // Attributes that are added or changed after launch should be like this to prevent
    // profile conversions.
}