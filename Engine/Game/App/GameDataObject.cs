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
            Set(BaseDataObjectKeys.uuid, value);
        }
    }
    
    //[JsonIgnore(JsonIgnoreWhen.Deserializing)]
    public virtual string code {
        get {
            return Get<string>(BaseDataObjectKeys.code, "");
        }
        
        set {
            Set(BaseDataObjectKeys.code, value);
        }
    }
    
    //[JsonIgnore(JsonIgnoreWhen.Deserializing)]
    public virtual string display_name {
        get {
            return Get<string>(BaseDataObjectKeys.display_name, "");
        }
        
        set {
            Set(BaseDataObjectKeys.display_name, value);
        }
    }    
    
    //[JsonIgnore(JsonIgnoreWhen.Deserializing)]
    public virtual string name {
        get {
            return Get<string>(BaseDataObjectKeys.name);
        }
        
        set {
            Set(BaseDataObjectKeys.name, value);
        }
    }   
    
    //[JsonIgnore(JsonIgnoreWhen.Deserializing)]
    public virtual string description {
        get {
            return Get<string>(BaseDataObjectKeys.description);
        }
        
        set {
            Set(BaseDataObjectKeys.description, value);
        }
    }     
    
    //[JsonIgnore(JsonIgnoreWhen.Deserializing)]
    public virtual object data {
        get {
            return Get(BaseDataObjectKeys.data);
        }
        
        set {
            Set(BaseDataObjectKeys.data, value);
        }
    }       
    
    //[JsonIgnore(JsonIgnoreWhen.Deserializing)]
    public virtual int sort_order {
        get {
            return Get<int>(BaseDataObjectKeys.sort_order, 0);
        }
        
        set {
            Set(BaseDataObjectKeys.sort_order, value);
        }
    }  
    
    //[JsonIgnore(JsonIgnoreWhen.Deserializing)]
    public virtual int sort_order_type {
        get {
            return Get<int>(BaseDataObjectKeys.sort_order_type, 0);
        }
        
        set {
            Set(BaseDataObjectKeys.sort_order_type, value);
        }
    }  
    
    //[JsonIgnore(JsonIgnoreWhen.Deserializing)]
    public virtual bool active {
        get {
            return Get<bool>(BaseDataObjectKeys.active, true);
        }
        
        set {
            Set(BaseDataObjectKeys.active, value);
        }
    }      
    
    //[JsonIgnore(JsonIgnoreWhen.Deserializing)]
    public virtual string key {
        get {
            return Get<string>(BaseDataObjectKeys.key, "");
        }
        
        set {
            Set(BaseDataObjectKeys.key, value);
        }
    }    
    
    //[JsonIgnore(JsonIgnoreWhen.Deserializing)]
    public virtual string game_id {
        get {
            return Get<string>(BaseDataObjectKeys.game_id, UniqueUtil.Instance.CreateUUID4());
        }
        
        set {
            Set(BaseDataObjectKeys.game_id, value);
        }
    }    
    
    //[JsonIgnore(JsonIgnoreWhen.Deserializing)]
    public virtual string type {
        get {
            return Get<string>(BaseDataObjectKeys.type, "");
        }
        
        set {
            Set(BaseDataObjectKeys.type, value);
        }
    }    
    
    //[JsonIgnore(JsonIgnoreWhen.Deserializing)]
    public virtual string order_by {
        get {
            return Get<string>(BaseDataObjectKeys.order_by, "");
        }
        
        set {
            Set(BaseDataObjectKeys.order_by, value);
        }
    }      
    
    //[JsonIgnore(JsonIgnoreWhen.Deserializing)]
    public virtual string status {
        get {
            return Get<string>(BaseDataObjectKeys.status, "");
        }
        
        set {
            Set(BaseDataObjectKeys.status, value);
        }
    }  
    
    //[JsonIgnore(JsonIgnoreWhen.Deserializing)]
    public virtual string pack_code {
        get {
            return Get<string>(BaseDataObjectKeys.pack_code, "");
        }
        
        set {
            Set(BaseDataObjectKeys.pack_code, value);
        }
    }  
    
    //[JsonIgnore(JsonIgnoreWhen.Deserializing)]
    public virtual int pack_sort {
        get {
            return Get<int>(BaseDataObjectKeys.pack_sort, "");
        }
        
        set {
            Set(BaseDataObjectKeys.pack_sort, value);
        }
    } 
    
    //[JsonIgnore(JsonIgnoreWhen.Deserializing)]
    public virtual DateTime date_created {
        get {
            return Get<DateTime>(BaseDataObjectKeys.date_created, DateTime.Now);
        }
        
        set {
            Set(BaseDataObjectKeys.date_created, value);
        }
    } 
    
    //[JsonIgnore(JsonIgnoreWhen.Deserializing)]
    public virtual DateTime date_modified {
        get {
            return Get<DateTime>(BaseDataObjectKeys.date_modified, DateTime.Now);
        }
        
        set {
            Set(BaseDataObjectKeys.date_modified, value);
        }
    } 

    public GameDataObject() {
        Reset();
    }

    public override void Reset() {
        base.Reset();
    }
}