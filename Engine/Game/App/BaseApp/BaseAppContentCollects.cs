using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class BaseAppContentCollects<T> : DataObjects<T> where T : DataObject, new() {
    private static T current;
    private static volatile BaseAppContentCollects<T> instance;
    private static object syncRoot = new Object();

    private string BASE_DATA_KEY = "app-content-collection-data";

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

    public static BaseAppContentCollects<T> BaseInstance {
        get {
            if (instance == null) {
                lock (syncRoot) {
                    if (instance == null)
                        instance = new BaseAppContentCollects<T>(true);
                }
            }

            return instance;
        }
        set {
            instance = value;
        }
    }


    public BaseAppContentCollects() {
        Reset();
    }

    public BaseAppContentCollects(bool loadData) {
        Reset();
        path = "data/" + BASE_DATA_KEY + ".json";
        pathKey = BASE_DATA_KEY;
        LoadData();
    }

    public void ChangeState(string code) {
        if(AppContentCollects.Current.code != code) {
            AppContentCollects.Current = AppContentCollects.Instance.GetById(code);
        }
    }

    public List<T> GetMissions() {
        return GetListByType(AppContentCollectType.mission);
    }

    
    public List<T> GetActions() {
        return GetListByType(AppContentCollectType.action);
    }

    
    public List<T> GetCollections() {
        return GetListByType(AppContentCollectType.collection);
    }

    public bool CheckCollects(string code, List<AppContentCollectItem> data) {

        if(data == null) {
            return false;
        }

        if(data.Count > 0) {
            return false;
        }

        AppContentCollect collect = AppContentCollects.Instance.GetByCode(code);

        if(collect != null) {
            //foreach(AppContentCollectItem collectItem in collect.data) {

                // TODO ensure collections are completed.

                ///if(!CheckChoiceUser(choiceItem.code, choices)) {
                //    return false;
                //}
            //}
        }
        else {
            return false;
        }

        if(data.Count == collect.data.Count) {
            return true;
        }

        return false;
    }
}


/*
    {
     "sort_order":0,
     "sort_order_type":0,
     "attributes":null,
     "game_id":"63a15c20-294f-11e1-9314-0800200c9a66",
     "type":"resource",
     "key":"question",
     "order_by":"",
     "code":"question-concussion-1",
     "display_name":"Concussions only happen if a player is knocked out.",
     "name":"",
     "description":"Concussions can happen whether a player is knocked out or not. It is important to pay attention to any symptoms.",
     "status":"default",
     "uuid":"33f4b660-2955-11e1-9314-0800200c9a61",
     "active":true,
     "keys":["concussion"],
     "responses": [
         {"name": "true", "display":"True", type":"incorrect"},
         {"name": "false", "display":"False", "type":"correct"}
     ]
 }
 */

public class AppContentCollectMessage {
    public string choiceItemType = AppContentCollectType.mission;
    public string choiceItemCode = "";
    public string choiceCode = "";
}

public class AppContentCollectMessages {
    public static string appContentCollectItem = "app-content-collect-item";
}

public class AppContentCollectType {
    public static string mission = "mission";
    public static string collection = "collection";
    public static string action = "action";
}

public class AppContentCollectItemKeys {
    public static string code = "code";
    public static string display = "display";
    public static string type = "type";
}

public class AppContentCollectItemMission : DataObject {
    
    public virtual string code {
        get {
            return Get<string>(BaseDataObjectKeys.code, "");
        }
        
        set {
            Set<string>(BaseDataObjectKeys.code, value);
        }
    }

    public virtual double amount {
        get {
            return Get<double>(BaseDataObjectKeys.amount, 1);
        }
        
        set {
            Set<double>(BaseDataObjectKeys.amount, value);
        }
    }
}

public class AppContentCollectItem : DataObject {

    public virtual string code {
        get {
            return Get<string>(BaseDataObjectKeys.code, "");
        }
        
        set {
            Set<string>(BaseDataObjectKeys.code, value);
        }
    }

    public virtual string type {
        get {
            return Get<string>(BaseDataObjectKeys.type, "");
        }
        
        set {
            Set<string>(BaseDataObjectKeys.type, value);
        }
    }

    public virtual DataObject data {
        get {
            return Get<DataObject>(BaseDataObjectKeys.data, new DataObject());
        }
        
        set {
            Set<DataObject>(BaseDataObjectKeys.data, value);
        }
    }

    public AppContentCollectItem() {
        Reset();
    }

    public override void Reset() {
        code = "";
        type = AppContentCollectType.mission;
    }

    public bool IsTypeAction() {
        return type == AppContentCollectType.action ? true : false;
    }

    public bool IsTypeCollection() {
        return type == AppContentCollectType.collection ? true : false;
    }

    public bool IsTypeMission() {
        return type == AppContentCollectType.mission ? true : false;
    }

    public AppContentCollectItemMission GetMission() {
        return (AppContentCollectItemMission)data;
    }
}

public class BaseAppContentCollect : GameDataObject {    
    
    public virtual List<string> tags {
        get {
            return Get<List<string>>(BaseDataObjectKeys.tags);
        }
        
        set {
            Set(BaseDataObjectKeys.tags, value);
        }
    }
    
    public virtual List<string> appStates {
        get {
            return Get<List<string>>(BaseDataObjectKeys.appStates);
        }
        
        set {
            Set(BaseDataObjectKeys.appStates, value);
        }
    }
    
    public virtual List<string> appContentStates {
        get {
            return Get<List<string>>(BaseDataObjectKeys.appContentStates);
        }
        
        set {
            Set(BaseDataObjectKeys.appContentStates, value);
        }
    }

    public virtual Dictionary<string, List<string>> requiredAssets {
        get {
            return Get<Dictionary<string, List<string>>>(BaseDataObjectKeys.requiredAssets);
        }
        
        set {
            Set(BaseDataObjectKeys.requiredAssets, value);
        }
    }

    public virtual List<string> keys {
        get {
            return Get<List<string>>(BaseDataObjectKeys.keys);
        }
        
        set {
            Set(BaseDataObjectKeys.keys, value);
        }
    }

    public virtual Dictionary<string, string> contentAttributes {
        get {
            return Get<Dictionary<string, string>>(BaseDataObjectKeys.contentAttributes);
        }
        
        set {
            Set(BaseDataObjectKeys.contentAttributes, value);
        }
    }

    public virtual List<AppContentCollectItem> data {
        get {
            return Get<List<AppContentCollectItem>>(BaseDataObjectKeys.data);
        }
        
        set {
            Set(BaseDataObjectKeys.data, value);
        }
    }

    // types: tracker, pack, data, generic

    // Attributes that are added or changed after launch should be like this to prevent
    // profile conversions.

    public BaseAppContentCollect() {
        Reset();
    }

    public override void Reset() {
        base.Reset();
        appStates = new List<string>();
        appContentStates = new List<string>();
        requiredAssets = new Dictionary<string, List<string>>();
        keys = new List<string>();
        contentAttributes = new Dictionary<string, string>();
        data = new List<AppContentCollectItem>();
    }

    public bool HasTypeMission() {
        foreach(AppContentCollectItem item in data) {
            if(item.type.ToLower() == AppContentCollectType.mission) {
                return true;
            }
        }
        return false;
    }

    public bool HasTypeCollection() {
        foreach(AppContentCollectItem item in data) {
            if(item.type.ToLower() == AppContentCollectType.collection) {
                return true;
            }
        }
        return false;
    }
        
    public bool HasTypeAction() {
        foreach(AppContentCollectItem item in data) {
            if(item.type.ToLower() == AppContentCollectType.action) {
                return true;
            }
        }
        return false;
    }

    public string GetContentString(string key) {
        string content = "";
        if(contentAttributes.ContainsKey(key)) {
            content = contentAttributes[key];
        }
        return content;
    }
    
    public string GetVersion() {
        return GetContentString(AppContentAssetAttributes.version);
    }
    
    public string GetVersionFileIncrement() {
        return GetContentString(AppContentAssetAttributes.version_file_increment);
    }
    
    public string GetVersionRequiredApp() {
        return GetContentString(AppContentAssetAttributes.version_required_app);
    }
    
    public string GetVersionType() {
        return GetContentString(AppContentAssetAttributes.version_type);
    }   
    
    public string GetVersionFileType() {
        return GetContentString(AppContentAssetAttributes.version_file_type);
    }
    
    public string GetVersionFileExt() {
        return GetContentString(AppContentAssetAttributes.version_file_ext);
    }
}