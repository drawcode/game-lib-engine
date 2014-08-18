
using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Engine.Events;
using Engine.Data.Json;

public class DataObjectItem {  
    
    public Dictionary<string, DataAttribute> attributes;
    
    public DataObjectItem() {
        Reset();
    }
    
    public virtual void Clone(DataObject toCopy) {
        attributes = toCopy.attributes;
    }
    
    public string LoadDataFromResources(string path) {
        string fileData = "";

        TextAsset textData = AssetUtil.LoadAsset<TextAsset>(path); 
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
    
    public object GetFieldValue(object obj, string fieldName) {
        ////LogUtil.Log("GetFieldValue:obj.GetType():" + obj.GetType());
        
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
        ////LogUtil.Log("SetFieldValue:obj.GetType():" + obj.GetType());
        
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
        attributes = new Dictionary<string, DataAttribute>();
    }
    
    
    // ATTRIBUTES
    
    public void SetAttribute(DataAttribute attribute) {
        
        string code = attribute.code;//UniqueUtil.Instance.GetStringHash(attribute.code);
        
        if(attributes == null) {
            attributes = new Dictionary<string, DataAttribute>();
        }
        
        // UPSERT        
        if(CheckIfAttributeExists(code)) {
            // UPDATE
            attributes[code] = attribute;
        }
        else {
            // INSERT
            attributes.Add(code, attribute);
        }
    }
    
    public bool CheckIfAttributeExists(string code) {
        if(attributes != null) {
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
    
    
    // ATTRIBUTES
    
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