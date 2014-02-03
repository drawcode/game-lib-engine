using System;
using System.Collections.Generic;
using System.IO;
using Engine.Data.Json;
using Engine.Utility;


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


public class GameDataObject : DataObject {

    // Dataobject handles keys
        
    //[JsonIgnore(JsonIgnoreWhen.Deserializing)]
    public virtual string uuid {
        get { 
            return Get<string>(BaseDataObjectKeys.uuid, UniqueUtil.Instance.CreateUUID4());
        }
        
        set {
            Set<string>(BaseDataObjectKeys.uuid, value);
        }
    }
    
    //[JsonIgnore(JsonIgnoreWhen.Deserializing)]
    public virtual string code {
        get {
            return Get<string>(BaseDataObjectKeys.code, "");
        }
        
        set {
            Set<string>(BaseDataObjectKeys.code, value);
        }
    }
    
    //[JsonIgnore(JsonIgnoreWhen.Deserializing)]
    public virtual string display_name {
        get {
            return Get<string>(BaseDataObjectKeys.display_name, "");
        }
        
        set {
            Set<string>(BaseDataObjectKeys.display_name, value);
        }
    }    
    
    //[JsonIgnore(JsonIgnoreWhen.Deserializing)]
    public virtual string name {
        get {
            return Get<string>(BaseDataObjectKeys.name);
        }
        
        set {
            Set<string>(BaseDataObjectKeys.name, value);
        }
    }   
    
    //[JsonIgnore(JsonIgnoreWhen.Deserializing)]
    public virtual string description {
        get {
            return Get<string>(BaseDataObjectKeys.description);
        }
        
        set {
            Set<string>(BaseDataObjectKeys.description, value);
        }
    }     

    //[JsonIgnore(JsonIgnoreWhen.Deserializing)]
    public virtual string url {
        get {
            return Get<string>(BaseDataObjectKeys.url, "");
        }
        
        set {
            Set<string>(BaseDataObjectKeys.url, value);
        }
    }

    //[JsonIgnore(JsonIgnoreWhen.Deserializing)]
    public virtual string host {
        get {
            return Get<string>(BaseDataObjectKeys.host, "");
        }
        
        set {
            Set<string>(BaseDataObjectKeys.host, value);
        }
    }
    
    //[JsonIgnore(JsonIgnoreWhen.Deserializing)]
    /*
    public virtual object data {
        get {
            return Get<object>(BaseDataObjectKeys.data);
        }
        
        set {
            Set<object>(BaseDataObjectKeys.data, value);
        }
    }       
    */
    
    //[JsonIgnore(JsonIgnoreWhen.Deserializing)]
    public virtual int sort_order {
        get {
            return Get<int>(BaseDataObjectKeys.sort_order, 0);
        }
        
        set {
            Set<int>(BaseDataObjectKeys.sort_order, value);
        }
    }  
    
    //[JsonIgnore(JsonIgnoreWhen.Deserializing)]
    public virtual int sort_order_type {
        get {
            return Get<int>(BaseDataObjectKeys.sort_order_type, 0);
        }
        
        set {
            Set<int>(BaseDataObjectKeys.sort_order_type, value);
        }
    }  
    
    //[JsonIgnore(JsonIgnoreWhen.Deserializing)]
    public virtual bool active {
        get {
            return Get<bool>(BaseDataObjectKeys.active, true);
        }
        
        set {
            Set<bool>(BaseDataObjectKeys.active, value);
        }
    }      
    
    //[JsonIgnore(JsonIgnoreWhen.Deserializing)]
    public virtual string key {
        get {
            return Get<string>(BaseDataObjectKeys.key, "");
        }
        
        set {
            Set<string>(BaseDataObjectKeys.key, value);
        }
    }    
    
    //[JsonIgnore(JsonIgnoreWhen.Deserializing)]
    public virtual string game_id {
        get {
            return Get<string>(BaseDataObjectKeys.game_id, UniqueUtil.Instance.CreateUUID4());
        }
        
        set {
            Set<string>(BaseDataObjectKeys.game_id, value);
        }
    }    
    
    //[JsonIgnore(JsonIgnoreWhen.Deserializing)]
    public virtual string type {
        get {
            return Get<string>(BaseDataObjectKeys.type, "");
        }
        
        set {
            Set<string>(BaseDataObjectKeys.type, value);
        }
    }    
    
    //[JsonIgnore(JsonIgnoreWhen.Deserializing)]
    public virtual string order_by {
        get {
            return Get<string>(BaseDataObjectKeys.order_by, "");
        }
        
        set {
            Set<string>(BaseDataObjectKeys.order_by, value);
        }
    }      
    
    //[JsonIgnore(JsonIgnoreWhen.Deserializing)]
    public virtual string status {
        get {
            return Get<string>(BaseDataObjectKeys.status, "");
        }
        
        set {
            Set<string>(BaseDataObjectKeys.status, value);
        }
    }  
    
    //[JsonIgnore(JsonIgnoreWhen.Deserializing)]
    public virtual string pack_code {
        get {
            return Get<string>(BaseDataObjectKeys.pack_code, "");
        }
        
        set {
            Set<string>(BaseDataObjectKeys.pack_code, value);
        }
    }  
    
    //[JsonIgnore(JsonIgnoreWhen.Deserializing)]
    public virtual int pack_sort {
        get {
            return Get<int>(BaseDataObjectKeys.pack_sort, 0);
        }
        
        set {
            Set<int>(BaseDataObjectKeys.pack_sort, value);
        }
    } 
    
    //[JsonIgnore(JsonIgnoreWhen.Deserializing)]
    public virtual DateTime date_created {
        get {
            return Get<DateTime>(BaseDataObjectKeys.date_created, DateTime.Now);
        }
        
        set {
            Set<DateTime>(BaseDataObjectKeys.date_created, value);
        }
    } 
    
    //[JsonIgnore(JsonIgnoreWhen.Deserializing)]
    public virtual DateTime date_modified {
        get {
            return Get<DateTime>(BaseDataObjectKeys.date_modified, DateTime.Now);
        }
        
        set {
            Set<DateTime>(BaseDataObjectKeys.date_modified, value);
        }
    }
        
    public virtual string username {
        get {
            return Get<string>(BaseDataObjectKeys.username);
        }
        
        set {
            Set<string>(BaseDataObjectKeys.username, value);
        }
    }
    
    public virtual string udid {
        get {
            return Get<string>(BaseDataObjectKeys.udid);
        }
        
        set {
            Set<string>(BaseDataObjectKeys.udid, value);
        }
    }
    
    public virtual string file_name {
        get {
            return Get<string>(BaseDataObjectKeys.file_name);
        }
        
        set {
            Set<string>(BaseDataObjectKeys.file_name, value);
        }
    }
    
    public virtual string file_path {
        get {
            return Get<string>(BaseDataObjectKeys.file_path);
        }
        
        set {
            Set<string>(BaseDataObjectKeys.file_path, value);
        }
    }
    
    public virtual string file_full_path {
        get {
            return Get<string>(BaseDataObjectKeys.file_full_path);
        }
        
        set {
            Set<string>(BaseDataObjectKeys.file_full_path, value);
        }
    }

    // game

    public virtual string asset {
        get {
            return Get<string>(BaseDataObjectKeys.asset);
        }
        
        set {
            Set<string>(BaseDataObjectKeys.asset, value);
        }
    }

    public GameDataObject() {
        Reset();
    }

    public override void Reset() {
        base.Reset();
    }
}