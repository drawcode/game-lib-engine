using System;
using System.Collections.Generic;
using System.IO;
using Engine.Data.Json;
using Engine.Utility;

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
    public virtual object data {
        get {
            return Get<object>(BaseDataObjectKeys.data);
        }
        
        set {
            Set<object>(BaseDataObjectKeys.data, value);
        }
    }       
    
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