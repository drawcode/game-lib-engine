using System;
using System.Collections.Generic;
using System.IO;

using UnityEngine;

public class GameDataModel : GameDataObject {
    
    public virtual string textures {
        get {
            return Get<string>(BaseDataObjectKeys.textures);
        }
        
        set {
            Set<string>(BaseDataObjectKeys.textures, value);
        }
    } 
    
    public virtual string colors {
        get {
            return Get<string>(BaseDataObjectKeys.colors);
        }
        
        set {
            Set<string>(BaseDataObjectKeys.colors, value);
        }
    }
}



public class GameDataItemColorPreset : GameDataObject {
    
}

public class GameDataItemTexturePreset : GameDataObject {
    
}

public class GameDataItemSound : GameDataObject {
    
}


public class GameDataObjectItem : GameDataObject {
    
    public virtual List<string> roles {
        get {
            return Get<List<string>>(BaseDataObjectKeys.roles);
        }
        
        set {
            Set<List<string>>(BaseDataObjectKeys.roles, value);
        }
    } 

    public virtual List<GameDataModel> models {
        get {
            return Get<List<GameDataModel>>(BaseDataObjectKeys.models);
        }
        
        set {
            Set<List<GameDataModel>>(BaseDataObjectKeys.models, value);
        }
    } 
    
    public virtual List<GameDataItemColorPreset> color_presets {
        get {
            return Get<List<GameDataItemColorPreset>>(BaseDataObjectKeys.color_presets);
        }
        
        set {
            Set<List<GameDataItemColorPreset>>(BaseDataObjectKeys.color_presets, value);
        }
    } 
    
    public virtual List<GameDataItemTexturePreset> texture_presets {
        get {
            return Get<List<GameDataItemTexturePreset>>(BaseDataObjectKeys.texture_presets);
        }
        
        set {
            Set<List<GameDataItemTexturePreset>>(BaseDataObjectKeys.texture_presets, value);
        }
    } 
        
    public virtual List<GameDataItemSound> sounds {
        get {
            return Get<List<GameDataItemSound>>(BaseDataObjectKeys.sounds);
        }
        
        set {
            Set<List<GameDataItemSound>>(BaseDataObjectKeys.sounds, value);
        }
    } 
    
    public virtual List<GameItemRPG> rpgs {
        get {
            return Get<List<GameItemRPG>>(BaseDataObjectKeys.rpgs);
        }
        
        set {
            Set<List<GameItemRPG>>(BaseDataObjectKeys.rpgs, value);
        }
    } 
    
    // color presets
    
    public GameDataModel GetModel() {
        return GetModel("default");
    }
    
    public GameDataModel GetModel(string code) {
        return GetItem<GameDataModel>(models, code);
    }

    // color presets
    
    public GameDataItemColorPreset GetColorPreset() {
        return GetColorPreset("default");
    }
    
    public GameDataItemColorPreset GetColorPreset(string code) {
        return GetItem<GameDataItemColorPreset>(color_presets, code);
    }

    // texture presets
    
    public GameDataItemTexturePreset GetTexturePreset() {
        return GetTexturePreset("default");
    }
        
    public GameDataItemTexturePreset GetTexturePreset(string code) {
        return GetItem<GameDataItemTexturePreset>(texture_presets, code);
    }

    // rpgs
    
    public GameItemRPG GetRPG() {
        return GetRPG("default");
    }

    public GameItemRPG GetRPG(string code) {
        return GetItem<GameItemRPG>(rpgs, code);
    }

    public T GetItem<T>(List<T> list, string code) where T : GameDataObject {
        
        if(list == null) 
            return default(T);
        
        if(list.Count > 0) {
            foreach(T item in list) {
                if(item.code == code) {
                    return item;
                }
            }
            
            foreach(T item in list) {
                return item;
            }
        }
        
        return null;
    }
}

public class BaseAppContentAssetModels<T> : DataObjects<T> where T : DataObject, new() {
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