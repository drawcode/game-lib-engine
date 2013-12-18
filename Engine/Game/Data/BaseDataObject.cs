using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Engine.Events;
using Engine.Data.Json;

public class BaseDataObjectKeys { 
    
    public static string uuid = "uuid";
    public static string code = "code";
    public static string name = "name";
    public static string description = "description";
    public static string display_name = "display_name";
    public static string attributes = "attributes";
    public static string data = "data";

    public static string sort_order = "sort_order";
    public static string sort_order_type = "sort_order_type";
    public static string active = "active";
    public static string key = "key";
    public static string game_id = "game_id";
    public static string type = "type";
    public static string order_by = "order_by";
    public static string status = "status";
    public static string data_items = "data_items";
    
    public static string pack_code = "pack_code";
    public static string pack_sort = "pack_sort";
    
    public static string date_created = "date_created";
    public static string date_modified = "date_modified";
}

public class BaseDataObject : Dictionary<string, object> {  

    public BaseDataObject() {
        
    }    
    
    // VALUES
    // use keyed dictionaries for all data objects to prevent
    // serialize and deserialize issues with keys on versions.    

    //[JsonIgnore(JsonIgnoreWhen.Deserializing)]
    public virtual string uuid {
        get { 
            return Get<string>(BaseDataObjectKeys.uuid);
        }
        
        set {
            Set(BaseDataObjectKeys.uuid, value);
        }
    }
    
    //[JsonIgnore(JsonIgnoreWhen.Deserializing)]
    public virtual string code {
        get {
            return Get<string>(BaseDataObjectKeys.code);
        }
        
        set {
            Set(BaseDataObjectKeys.code, value);
        }
    }
    
    //[JsonIgnore(JsonIgnoreWhen.Deserializing)]
    public virtual string display_name {
        get {
            return Get<string>(BaseDataObjectKeys.display_name);
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
    public virtual Dictionary<string, DataAttribute> attributes {
        get {
            return Get<Dictionary<string, DataAttribute>>(BaseDataObjectKeys.attributes);
        }
        
        set {
            Set(BaseDataObjectKeys.attributes, value);
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
            return Get<int>(BaseDataObjectKeys.sort_order);
        }
        
        set {
            Set(BaseDataObjectKeys.sort_order, value);
        }
    }  
    
    //[JsonIgnore(JsonIgnoreWhen.Deserializing)]
    public virtual int sort_order_type {
        get {
            return Get<int>(BaseDataObjectKeys.sort_order_type);
        }
        
        set {
            Set(BaseDataObjectKeys.sort_order_type, value);
        }
    }  
    
    //[JsonIgnore(JsonIgnoreWhen.Deserializing)]
    public virtual bool active {
        get {
            return Get<bool>(BaseDataObjectKeys.active);
        }
        
        set {
            Set(BaseDataObjectKeys.active, value);
        }
    }      
    
    //[JsonIgnore(JsonIgnoreWhen.Deserializing)]
    public virtual string key {
        get {
            return Get<string>(BaseDataObjectKeys.key);
        }
        
        set {
            Set(BaseDataObjectKeys.key, value);
        }
    }    
    
    //[JsonIgnore(JsonIgnoreWhen.Deserializing)]
    public virtual string game_id {
        get {
            return Get<string>(BaseDataObjectKeys.game_id);
        }
        
        set {
            Set(BaseDataObjectKeys.game_id, value);
        }
    }    
    
    //[JsonIgnore(JsonIgnoreWhen.Deserializing)]
    public virtual string type {
        get {
            return Get<string>(BaseDataObjectKeys.type);
        }
        
        set {
            Set(BaseDataObjectKeys.type, value);
        }
    }    
    
    //[JsonIgnore(JsonIgnoreWhen.Deserializing)]
    public virtual string order_by {
        get {
            return Get<string>(BaseDataObjectKeys.order_by);
        }
        
        set {
            Set(BaseDataObjectKeys.order_by, value);
        }
    }      
    
    //[JsonIgnore(JsonIgnoreWhen.Deserializing)]
    public virtual string status {
        get {
            return Get<string>(BaseDataObjectKeys.status);
        }
        
        set {
            Set(BaseDataObjectKeys.status, value);
        }
    }  
    
    //[JsonIgnore(JsonIgnoreWhen.Deserializing)]
    public virtual string pack_code {
        get {
            return Get<string>(BaseDataObjectKeys.pack_code);
        }
        
        set {
            Set(BaseDataObjectKeys.pack_code, value);
        }
    }  
    
    //[JsonIgnore(JsonIgnoreWhen.Deserializing)]
    public virtual int pack_sort {
        get {
            return Get<int>(BaseDataObjectKeys.pack_sort);
        }
        
        set {
            Set(BaseDataObjectKeys.pack_sort, value);
        }
    } 
    
    //[JsonIgnore(JsonIgnoreWhen.Deserializing)]
    public virtual DateTime date_created {
        get {
            return Get<DateTime>(BaseDataObjectKeys.date_created);
        }
        
        set {
            Set(BaseDataObjectKeys.date_created, value);
        }
    } 
    
    //[JsonIgnore(JsonIgnoreWhen.Deserializing)]
    public virtual DateTime date_modified {
        get {
            return Get<DateTime>(BaseDataObjectKeys.date_modified);
        }
        
        set {
            Set(BaseDataObjectKeys.date_modified, value);
        }
    } 

    // -----------------------------------------------------------------------
    // VALUE ACCESSORS

    // generics

    public virtual T Get<T>(string code) {
        return Get<T>(code, null);
    } 
    
    public virtual T Get<T>(string code, object defaultValue) {                
        try {
            return (T)Get(code, defaultValue);
        }
        catch(Exception e) {
            return default(T);
        }
    }

    // typed gets

    public virtual object Get(string code) {
        return Get<object>(code, null);
    }    
    
    public virtual object Get(string code, object defaultValue) {
        if(ContainsKey(code)) {
            return this[code];
        }
        return defaultValue;
    }

    // sets
    
    public virtual void Set(string code, object val) {
        if(ContainsKey(code)) {
            this[code] = val;
        }
        else {
            Add(code, val);
        }
    }
    
    public virtual void Set(string code, DataAttribute val) {
        if(attributes == null) {
            attributes = new Dictionary<string, DataAttribute>();                        
        }


        if(attributes.ContainsKey(code)) {
            attributes[code] = val;
        }
        else {
            attributes.Add(code, val);
        }
    }

    //
    
    public virtual void Clone(DataObject toCopy) {
        attributes = toCopy.attributes;
    }
    
    // -----------------------------------------------------------------------
    // LOADING/SAVING
    
    public string LoadDataFromResources(string resourcesFilePath) {
        string fileData = "";
        
        TextAsset textData = Resources.Load(resourcesFilePath, typeof(TextAsset)) as TextAsset;          
        if(textData != null) {
            fileData = textData.text;
        }
        
        return fileData;
    }
    
    public string LoadDataFromPrefs(string key) {
        string data = "";
        
        if(!SystemPrefUtil.HasLocalSetting(key)) {
            data = SystemPrefUtil.GetLocalSettingString(key);
        }
        return data;
    }
    
    public string LoadData(string fileFullPath) {
        string fileData = "";
        
        #if !UNITY_WEBPLAYER 
        if(FileSystemUtil.CheckFileExists(fileFullPath)) {       
            fileData = FileSystemUtil.ReadString(fileFullPath);
        }        
        #endif       
        return fileData;
    }
    
    public T LoadData<T>(string folderPath, string fileKey) {
        string fileData = "";
        #if !UNITY_WEBPLAYER
        string path = PathUtil.Combine(folderPath, (fileKey + ".json").TrimStart('/'));
        if(FileSystemUtil.CheckFileExists(path)) {       
            fileData = FileSystemUtil.ReadString(path);
        }        
        #endif   
        if(!string.IsNullOrEmpty(fileData)) {
            return JsonMapper.ToObject<T>(fileData);
        }
        
        return default(T);
    }
    
    public void SaveData(string folderPath, string fileKey, object obj) {
        string data = JsonMapper.ToJson(obj);
        string path = PathUtil.Combine(folderPath, (fileKey + ".json").TrimStart('/'));
        SaveData(path, data);
    }
    
    public void SaveData(string fileFullPath, string data) {
        #if !UNITY_WEBPLAYER
        
        if(fileFullPath.Contains(Application.dataPath)
           || fileFullPath.Contains(Application.persistentDataPath)) {
            
            FileSystemUtil.WriteString(fileFullPath, data);
        }
        #endif
    }
    
    // -----------------------------------------------------------------------
    // HELPERS, REFLECT
    
    public object GetFieldValue(object obj, string fieldName) {
        ////Debug.Log("GetFieldValue:obj.GetType():" + obj.GetType());
        
        bool hasGet = false;
        
        foreach(var prop in fieldName.Split('.').Select(s => obj.GetType().GetField(s))) {
            if(obj != null) {
                obj = prop.GetValue(obj);
                hasGet = true;
            }
        }
        
        if(!hasGet) {
            foreach(System.Reflection.PropertyInfo prop in obj.GetType().GetProperties()) {
                if(prop.Name == fieldName) {
                    obj = prop.GetValue(obj, null);
                }
            }
        }
        
        return obj;
    }
    
    public void SetFieldValue(object obj, string fieldName, object fieldValue) {
        ////Debug.Log("SetFieldValue:obj.GetType():" + obj.GetType());
        
        //bool hasSet = false;
        
        foreach(System.Reflection.FieldInfo field in fieldName.Split('.').Select(s => obj.GetType().GetField(s))) {
            if(field != null) {
                field.SetValue(obj, fieldValue);
                
                //hasSet = true;
            }
        }
        
        //if(!hasSet) {
        foreach(System.Reflection.PropertyInfo prop in obj.GetType().GetProperties()) {
            if(prop.Name == fieldName) {
                prop.SetValue(obj, fieldValue, null);
            }
        }
        
        //}
    }
    
    public virtual void Reset() {
        //attributes = new Dictionary<string, DataAttribute>();
    }
    
    // -----------------------------------------------------------------------
    // ATTRIBUTES
    
    public void SetAttribute(DataAttribute attribute) {

        Set(BaseDataObjectKeys.attributes, attribute);
    }
    
    public bool CheckIfAttributeExists(string code) {
        if(Get(BaseDataObjectKeys.attributes) != null) {
            //code = UniqueUtil.Instance.GetStringHash(code);
            if(attributes.ContainsKey(code)) {
                return true;
            }
        }
        return false;
    }
    
    public DataAttribute GetAttribute(string code) {
        DataAttribute attribute = new DataAttribute();
        
        //code = UniqueUtil.Instance.GetStringHash(code);
        
        if(CheckIfAttributeExists(code)) {
            attribute = attributes[code];
        }
        
        return attribute;
    }
        
    public List<DataAttribute> GetAttributesList() {
        List<DataAttribute> attributesList = new List<DataAttribute>();
        foreach(DataAttribute attribute in attributes.Values) {
            attributesList.Add(attribute);
        }
        return attributesList;
    }
    
    public List<DataAttribute> GetAttributesList(string objectType) {
        List<DataAttribute> attributesFiltered = new List<DataAttribute>();
        foreach(DataAttribute attribute in attributes.Values) {
            if(attribute.otype == objectType) {
                attributesFiltered.Add(attribute);
            }
        }
        return attributesFiltered;
    }
    
    public Dictionary<string, DataAttribute> GetAttributesDictionary() {
        return attributes;   
    }
    
    public Dictionary<string, DataAttribute> GetAttributesDictionary(string objectType) {
        Dictionary<string, DataAttribute> attributesFiltered = new Dictionary<string, DataAttribute>();
        foreach(KeyValuePair<string, DataAttribute> pair in attributes) {
            if(pair.Value.otype == objectType) {
                attributesFiltered.Add(pair.Value.code, pair.Value);
            }
        }
        return attributesFiltered;
    }
    
    public void SetAttributeBoolValue(string code, bool val) {
        DataAttribute att = new DataAttribute();
        att.val = val;
        att.code = code;
        att.name = code;
        att.type = "bool";
        att.otype = "attribute";
        SetAttribute(att);
    }
    
    public bool GetAttributeBoolValue(string code) {
        bool currentValue = false;
        object objectValue = GetAttribute(code).val;
        if(objectValue != null) {
            currentValue = Convert.ToBoolean(objectValue);
        }
        
        return currentValue;
    }
    
    public void SetAttributeStringValue(string code, string val) {
        DataAttribute att = new DataAttribute();
        att.val = val;
        att.code = code;
        att.name = code;
        att.type = "string";
        att.otype = "attribute";
        SetAttribute(att);
    }
    
    public string GetAttributeStringValue(string code) {
        string currentValue = "";
        object objectValue = GetAttribute(code).val;
        if(objectValue != null) {
            currentValue = Convert.ToString(objectValue);
        }
        
        return currentValue;
    }
    
    public void SetAttributeDoubleValue(string code, double val) {
        DataAttribute att = new DataAttribute();
        att.val = val;
        att.code = code;
        att.name = code;
        att.type = "double";
        att.otype = "attribute";
        SetAttribute(att);
    }
    
    public double GetAttributeDoubleValue(string code) {
        double currentValue = 0.0;
        object objectValue = GetAttribute(code).val;
        if(objectValue != null) {
            currentValue = Convert.ToDouble(objectValue);
        }
        
        return currentValue;
    }
    
    public void SetAttributeIntValue(string code, int val) {
        DataAttribute att = new DataAttribute();
        att.val = val;
        att.code = code;
        att.name = code;
        att.type = "int";
        att.otype = "attribute";
        SetAttribute(att);
    }
    
    public int GetAttributeIntValue(string code) {
        int currentValue = 0;
        object objectValue = GetAttribute(code).val;
        if(objectValue != null) {
            currentValue = Convert.ToInt32(objectValue);
        }
        
        return currentValue;
    }
    
    public void SetAttributeObjectValue(string code, object val) {
        DataAttribute att = new DataAttribute();
        att.val = val;
        att.code = code;
        att.name = code;
        att.type = "object";
        att.otype = "attribute";
        SetAttribute(att);
    }
    
    public object GetAttributeObjectValue(string code) {
        object objectValue = GetAttribute(code).val;
        return objectValue;
    }
    
    public string GetPlatformAttributeKey(string key) {
        string keyto = key;
        
        #if UNITY_IPHONE
        keyto = "platform-ios-" + keyto;    
        #endif
        #if UNITY_ANDROID
        keyto = "platform-android-" + keyto;
        #endif
        return keyto;
    }
    
}
