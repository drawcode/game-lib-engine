using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#if !UNITY_WEBPLAYER
using System.Reflection;
using System.IO;
#endif

using Engine.Events;
// using Engine.Data.Json;

public class DataKeyedObjects<T> {
    public List<T> items;
    public string pathKey = "";
    public string path = "";
    public List<string> packPaths;
    public List<string> packPathsVersioned;
    public DataObjectsStorage dataStorage = DataObjectsStorage.PERSISTENT;
    public List<string> historyLevelItems = new List<string>();

    public DataKeyedObjects() {
        Reset();
    }

    public virtual void LoadData() {
     
        dataStorage = AppConfigs.dataStorage;

        switch(dataStorage) {

        case DataObjectsStorage.SERVER:
            LogUtil.Log("LoadData:LoadDataFromServer:" + path + " " + pathKey);
            LoadDataFromServer();
            break;

        case DataObjectsStorage.RESOURCES:
            LogUtil.Log("LoadData:LoadDataFromResources:" + path + " " + pathKey);
            LoadDataFromResources();
            break;

        case DataObjectsStorage.PREFERENCES:
            LogUtil.Log("LoadData:LoadDataFromPrefs:" + path + " " + pathKey);
            LoadDataFromPrefs();
            break;

        default:
            LogUtil.Log("LoadData:LoadDataFromPersistent:" + path + " " + pathKey);
            LoadDataFromPersistent();
            break;

        }
    }

    public virtual void LoadDataFromPersistent() {
        LoadDataFromPersistent(path);
    }

    public virtual void LoadDataFromResources() {
        
        string pathResources = path;

        LogUtil.Log("LoadDataFromResources:pathResources:" + pathResources);
        LogUtil.Log("LoadDataFromResources:ContentsConfig.contentRootFolder:" + ContentsConfig.contentRootFolder);
        LogUtil.Log("LoadDataFromResources:ContentsConfig.contentAppFolder:" + ContentsConfig.contentAppFolder);
        LogUtil.Log("LoadDataFromResources:Application.persistentDataPath:" + Application.persistentDataPath);
        LogUtil.Log("LoadDataFromResources:Application.dataPath:" + Application.dataPath);

        if(!path.Contains(ContentsConfig.contentAppFolder)) {            

            if(Context.Current.isWebGL) {

                System.Text.StringBuilder sbPath = new System.Text.StringBuilder();

                sbPath.Append(ContentsConfig.contentRootFolder);
                sbPath.Append("/");
                sbPath.Append(ContentsConfig.contentAppFolder);
                sbPath.Append("/version/");
                sbPath.Append(pathResources);

                pathResources = sbPath.ToString();

                LogUtil.Log("LoadDataFromResources:web:pathResources:" + pathResources);
            }
            else {

                pathResources = pathResources.Replace("data/", "");

                pathResources =
                 PathUtil.Combine(ContentPaths.appCachePathData, pathResources)
                     .Replace(Application.persistentDataPath, "")
                     .Replace(Application.dataPath, "");
             
                if(pathResources.EndsWith(".json")) {
                    //pathResources += ".txt";
                }
    
                if(pathResources.StartsWith("/")) {
                    pathResources = pathResources.TrimStart('/');
                }
    
                if(pathResources.Contains("/" + ContentsConfig.contentVersion + "/")) {
                    pathResources = pathResources.Replace("/" + ContentsConfig.contentVersion + "/", "/version/");
                }
            }
        }
     
        LogUtil.Log("LoadDataFromResources:void:" + pathResources);
     
        string data = LoadDataFromResources(pathResources);
        LoadDataFromString(data);
    }

    public virtual void LoadDataFromPrefs() {
        string data = LoadDataFromPrefs(pathKey);

        LoadDataFromString(data);
    }

    public void LoadDataFromServer() {
        // TODO
        LoadDataFromServer(path);
    }

    public void LoadDataFromServer(string key) {
        // TODO
    }

    public string LoadDataFromPrefs(string key) {
        string data = "";

        if(!SystemPrefUtil.HasLocalSetting(key)) {
            data = SystemPrefUtil.GetLocalSettingString(key);
        }
        return data;
    }

    public string LoadDataFromPersistent(string path) {
        return LoadDataFromPersistent(path, true);
    }

    public string LoadDataFromPersistent(string path, bool versioned) {
        string fileData = "";
                
        fileData = Contents.GetFileDataFromPersistentCache(path, versioned, false);

        LoadDataFromString(fileData);

        //if(!string.IsNullOrEmpty(fileData)) {
        //string log = fileData;
        //if(fileData.Length > 1000) {
        //      log = fileData.Substring(0, 990);
        //}
        ////LogUtil.Log("LoadDataFromPersistent:fileData:" + log + " " + pathKey);
        //}

        ////LogUtil.Log(">>>LoadDataFromPersistentPacks:" + path);
        LoadDataFromPersistentPacks(path);

        return fileData;
    }

    public bool SaveDataItemsToPersistent(string path) {
        bool saved = false;
        string fileData = "";
        string pathPart = path;

        path = PathUtil.Combine(ContentPaths.appCacheVersionPath, path.TrimStart('/'));
        string pathVersioned = Contents.GetFileVersioned(path);

        LogUtil.Log("LoadDataFromPersistent:path:" + path);
        LogUtil.Log("LoadDataFromPersistent:pathVersioned:" + pathVersioned + " " + pathKey);

        bool prepared = PreparePersistentFile(pathPart, pathVersioned);

        if(!prepared)
            return false;

        fileData = items.ToJson();//JsonMapper.ToJson(items);

        FileSystemUtil.WriteString(pathVersioned, fileData);

        LogUtil.Log("SaveDataItemsToPersistent:fileData:" + fileData + " " + pathKey);

        saved = true;

        return saved;
    }

    public bool SaveDataItemToPersistent(T obj, string path) {
        bool saved = false;
        string fileData = "";
        string pathPart = path;

        path = PathUtil.Combine(ContentPaths.appCacheVersionPath, path.TrimStart('/'));
        string pathVersioned = Contents.GetFileVersioned(path);

        LogUtil.Log("LoadDataFromPersistent:path:" + path);
        LogUtil.Log("LoadDataFromPersistent:pathVersioned:" + pathVersioned + " " + pathKey);

        bool prepared = PreparePersistentFile(pathPart, pathVersioned);

        if(!prepared)
            return false;

        fileData = obj.ToJson();//JsonMapper.ToJson(obj);

        FileSystemUtil.WriteString(pathVersioned, fileData);

        LogUtil.Log("SaveDataItemsToPersistent:fileData:" + fileData + " " + pathKey);

        saved = true;

        return saved;
    }

    public bool PreparePersistentFile(string pathPart, string pathVersioned) {
        bool prepared = true;

        if(!FileSystemUtil.CheckFileExists(pathVersioned)) {
            prepared = false;

            LogUtil.Log("LoadDataFromPersistent:pathVersioned not exist:" + pathVersioned + " " + pathKey);

            // copy from streaming assets
            string pathToCopy = PathUtil.Combine(ContentPaths.appCacheVersionPath, pathPart.TrimStart('/'));
            LogUtil.Log("LoadDataFromPersistent:pathToCopy:" + pathToCopy + " " + pathKey);
            if(FileSystemUtil.CheckFileExists(pathToCopy)) {
                FileSystemUtil.MoveFile(pathToCopy, pathVersioned);
                LogUtil.Log("LoadDataFromPersistent:file moved:" + pathToCopy + " " + pathKey);
                prepared = true;
            }
            else {
                LogUtil.Log("LoadDataFromPersistent:move not exist:" + pathToCopy + " " + pathKey);
                prepared = false;
            }
        }
        return prepared;
    }

    public void LoadDataFromPersistentPacks(string path) {

        // Append data from additional pack data files

        List<T> appendList = new List<T>();
        List<T> appendItemList = new List<T>();

        foreach(string packPath in ContentPaths.GetPackPathsVersioned()) {
            string data = "";
            string pathData = PathUtil.Combine(packPath, path.TrimStart('/'));
            FileSystemUtil.EnsureDirectory(pathData);

            string[] packDirs = packPath.TrimEnd('/').Replace("data", "").TrimEnd('/').Split('/');
            string packDirName = packDirs[packDirs.Length - 1];

            string fileVersioned = Contents.GetFullPathVersioned(pathData);
                        
            FileSystemUtil.EnsureDirectory(fileVersioned);

            if(FileSystemUtil.CheckFileExists(fileVersioned)) {
                data = Contents.GetFileDataFromPersistentCache(pathData, true, true);
            }

            if(!string.IsNullOrEmpty(data)) {
                List<T> objs = new List<T>();

                objs = LoadDataFromString(appendItemList, data);

                int i = 0;

                if(packDirName.Contains("app-viewer")) {
                }
                else {
                    i = 500;
                }

                for(int j = 0; j < objs.Count; j++) {
                    SetFieldValue(objs[j], "pack_code", packDirName);
                    SetFieldValue(objs[j], "pack_sort", i++);
                }

                if(objs != null && objs.Count > 0) {
                    appendList.AddRange(objs);
                }
            }
        }

        ////LogUtil.Log("!!!!!! PackPathsVersionShared:" + Contents.GetPackPathsVersionedShared().Count);

        foreach(string packPath in ContentPaths.GetPackPathsVersionedShared()) {
            string data = "";
            string pathData = PathUtil.Combine(packPath, path.TrimStart('/'));
            FileSystemUtil.EnsureDirectory(pathData);

            string[] packDirs = packPath.TrimEnd('/').Replace("data", "").TrimEnd('/').Split('/');
            string packDirName = packDirs[packDirs.Length - 1];

            string fileVersioned = Contents.GetFullPathVersioned(pathData);
            FileSystemUtil.EnsureDirectory(fileVersioned);

            if(FileSystemUtil.CheckFileExists(fileVersioned)) {
                data = Contents.GetFileDataFromPersistentCache(pathData, true, true);
            }

            if(!string.IsNullOrEmpty(data)) {
                List<T> objs = new List<T>();

                objs = LoadDataFromString(appendItemList, data);

                int i = 0;

                if(packDirName.Contains("app-viewer")) {
                }
                else {
                    i = 500;
                }

                for(int j = 0; j < objs.Count; j++) {
                    SetFieldValue(objs[j], "pack_code", packDirName);
                    SetFieldValue(objs[j], "pack_sort", i++);
                }

                if(objs != null && objs.Count > 0) {
                    appendList.AddRange(objs);
                }
            }
        }

        foreach(string packPath in ContentPaths.GetPackPathsNonVersioned()) {
            string data = "";
            string pathData = PathUtil.Combine(packPath, path.TrimStart('/'));
            FileSystemUtil.EnsureDirectory(pathData);

            string[] packDirs = packPath.TrimEnd('/').Replace("data", "").TrimEnd('/').Split('/');
            string packDirName = packDirs[packDirs.Length - 1];

            if(string.IsNullOrEmpty(packDirName)) {
                packDirName = "";
            }

            string fileVersioned = Contents.GetFullPathVersioned(pathData);
            FileSystemUtil.EnsureDirectory(fileVersioned);

            if(FileSystemUtil.CheckFileExists(fileVersioned)) {
                ////LogUtil.Log(">> PACK FILE EXISTS: " + pathData);
                data = Contents.GetFileDataFromPersistentCache(pathData, true, true);
                ////LogUtil.Log(">> PACK FILE DATA: " + data);
            }

            if(!string.IsNullOrEmpty(data)) {
                if(!string.IsNullOrEmpty(data)) {
                    List<T> objs = new List<T>();

                    objs = LoadDataFromString(appendItemList, data);

                    int i = 0;
                    if(packDirName.Contains("app-viewer")) {
                    }
                    else {
                        i = 500;
                    }

                    for(int j = 0; j < objs.Count; j++) {
                        SetFieldValue(objs[j], "pack_code", packDirName);
                        SetFieldValue(objs[j], "pack_sort", i++);
                    }

                    if(objs != null && objs.Count > 0) {
                        appendList.AddRange(objs);
                    }
                }
            }
        }

        items.AddRange(appendList);
    }

    public string LoadDataFromResources(string resourcesPath) {
        string fileData = "";

        LogUtil.Log("LoadDataFromResources:string:resourcesPath:" + resourcesPath + " " + pathKey);
                
        TextAsset textData = AssetUtil.LoadAsset<TextAsset>(path); 
        if(textData != null) {
            fileData = textData.text;
        }
        LoadDataFromString(fileData);

        LogUtil.Log("LoadDataFromResources:fileData:" + fileData + " " + pathKey);
        return fileData;
    }

    public virtual void LoadDataFromString(string data) {
        if(!string.IsNullOrEmpty(data)) {
            items.Clear();
            items = LoadDataFromString(items, data);
        }
    }

    public virtual List<T> LoadDataFromString(List<T> objs, string data) {
        if(!string.IsNullOrEmpty(data)) {
            objs = data.FromJson<List<T>>();

            //LogUtil.Log("T loaded:" + objs.Count);
            for(int j = 0; j < objs.Count; j++) {
                SetFieldValue(objs[j], "pack_code", "default");
            }
        }
        return objs;
    }

    public T GetByCode(string code) {

        LogUtil.Log("GetByCode:code:" + code);

        return GetByStringKey("code", code);
    }

    public T GetById(string id) {
        return GetByStringKey("code", id);
    }

    public T GetByPackCode(string packCode) {
        return GetByStringKey("pack_code", packCode);
    }

    public bool CheckById(string id) {
        return CheckByStringKey("code", id);
    }

    public bool CheckByCode(string code) {
        return CheckByStringKey("code", code);
    }

    public T GetByUuid(string id) {
        return GetByStringKey("uuid", id);
    }

    public bool CheckByUuid(string id) {
        return CheckByStringKey("uuid", id);
    }

    public T GetByName(string id) {
        return GetByStringKey("name", id);
    }

    public bool CheckByName(string id) {
        return CheckByStringKey("name", id);
    }

    public T GetByStringKey(string key, string keyValue) {
        foreach(T obj in GetAll()) {
            try {
                bool found = false;
                foreach(System.Reflection.FieldInfo fieldInfo in obj.GetType().GetFields()) {
                    if(fieldInfo.Name == key) {
                        found = true;
                        break;
                    }
                }
                if(found) {
                    string codeValue = (string)obj.GetType().GetField(key).GetValue(obj);
                    if(codeValue.ToLower() == keyValue.ToLower()) {
                        return obj;
                    }
                }
                else {

                    foreach(System.Reflection.PropertyInfo propInfo in obj.GetType().GetProperties()) {
                        if(propInfo.Name == key) {
                            found = true;
                            break;
                        }
                    }

                    if(found) {
                        string codeValue = (string)obj.GetType().GetProperty(key).GetValue(obj, null);
                        if(codeValue.ToLower() == keyValue.ToLower()) {
                            return obj;
                        }
                    }
                }
            }
            catch(Exception e) {
                LogUtil.Log("GetByStringKey warning no key:" + e);
                return default(T);
            }
        }

        return default(T);
    }

    /*
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
            foreach(PropertyInfo prop in obj.GetType().GetProperties()) {
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

        foreach(FieldInfo field in fieldName.Split('.').Select(s => obj.GetType().GetField(s))) {
            if(field != null) {
                field.SetValue(obj, fieldValue);

                //hasSet = true;
            }
        }

        //if(!hasSet) {
        foreach(PropertyInfo prop in obj.GetType().GetProperties()) {
            if(prop.Name == fieldName) {
                prop.SetValue(obj, fieldValue, null);
            }
        }

        //}
    }
    */


    public object GetFieldValue(object obj, string fieldName) {
        LogUtil.Log("GetFieldValue:obj.GetType():" + obj.GetType());

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
        LogUtil.Log("SetFieldValue:obj.GetType():" + obj.GetType());

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

    public bool CheckByStringKey(string key, string keyValue) {

        foreach(T obj in GetAll()) {
            try {
                bool found = false;
                foreach(System.Reflection.FieldInfo fieldInfo in obj.GetType().GetFields()) {
                    if(fieldInfo.Name == key) {
                        found = true;
                        break;
                    }
                }
                if(found) {
                    string codeValue = (string)obj.GetType().GetField(key).GetValue(obj);
                    if(codeValue.ToLower() == keyValue.ToLower()) {
                        return true;
                    }
                }
            }
            catch(Exception e) {
                LogUtil.Log("GetByStringKey warning no key:" + e);
                return false;
            }
        }

        return false;
    }

    public List<T> GetList(string key, object val) {
        //LogUtil.Log("GetList:" + " key:" + key + " val:" + val);
        List<T> list = new List<T>();
        foreach(T t in GetAll()) {
            object obj = GetFieldValue(t, key);
            //LogUtil.Log("GetList:" + " obj:" + obj);
            if(obj != null) {
                if(obj.Equals(val)) {
                    //LogUtil.Log("GetList: adding t:" + t);
                    list.Add(t);
                }
            }
        }
        return list;
    }
     
    public List<T> GetListPack(string key, object val, bool all) {
        //LogUtil.Log("GetList:" + " key:" + key + " val:" + val);
        List<T> list = new List<T>();
        foreach(T t in GetAll()) {
            object obj = GetFieldValue(t, key);
            string strObj = "";
            if(obj != null) { 
                strObj = obj.ToString();
                if(strObj != null) {
                    strObj = strObj.ToLower();
                }
            }
            string strVal = "";
            if(val != null) { 
                strVal = val.ToString();
                if(strVal != null) {
                    strVal = strVal.ToLower();
                }
            }
            //LogUtil.Log("GetList:" + " obj:" + obj);
            if(obj != null) {
                if((obj.Equals(val) 
                 || strObj == strVal)
                 || (all 
                     && (
                     obj.Equals("*")
                     || obj.Equals("all")
                     || obj.Equals("default")
                     || obj.Equals("app-state-all")
                     || obj.Equals("app-pack-all")
                     )
                 )) {
                    
                    //LogUtil.Log("GetList: adding t:" + t);
                    list.Add(t);
                }
            }
        }
        return list;
    }

    public List<T> GetListByUuid(string val) {
        return GetList("uuid", val);
    }

    public List<T> GetListByCode(string val) {
        return GetList("code", val);
    }
 
    public List<T> GetListByPack(string val) {
        return GetListPack(val, true);
    }

    public List<T> GetListByParentCode(string val) {
        return GetList("parent_code", val);
    }
 
    public List<T> GetListPack(object val, bool all) {
        return GetListPack("pack_code", val, all);
    }
 
    public List<T> GetListByPackExplicit(string val) {
        return GetListPack(val, false);
    }

    public List<T> GetListByType(string val) {
        return GetList("type", val);
    }
 
    public List<T> SortList() {
        if(items == null) {
            return null;
        }
        items.Sort(
                delegate(T c1, T c2) {
            //LogUtil.Log("sorting:c1:", c1);
            //LogUtil.Log("sorting:c2:", c2);
            if(GetFieldValue(c1, "sort_order") != null) {
                int sort1 = (int)GetFieldValue(c1, "sort_order");
                int sort2 = (int)GetFieldValue(c2, "sort_order");
                //LogUtil.Log("sorting:sort1:", sort1);
                //LogUtil.Log("sorting:sort2:", sort2);
                return sort1.CompareTo(sort2);
            }
            else {
                return -1;
            }
        }
            );
        return items;
    }
 
    public List<T> SortList(List<T> listItems) {
        if(listItems == null) {
            return null;
        }
        listItems.Sort(
            delegate(T c1, T c2) {
            //LogUtil.Log("sorting:c1:", c1);
            //LogUtil.Log("sorting:c2:", c2);
            if(GetFieldValue(c1, "sort_order") != null) {
                int sort1 = (int)GetFieldValue(c1, "sort_order");
                int sort2 = (int)GetFieldValue(c2, "sort_order");
                //LogUtil.Log("sorting:sort1:", sort1);
                //LogUtil.Log("sorting:sort2:", sort2);
                return sort1.CompareTo(sort2);
            }
            else {
                return -1;
            }
        }
            );
        return listItems;
    }

    public List<T> GetAll() {
        //LogUtil.Log("GetAll:IsLoaded:", IsLoaded);
        if(!IsLoaded) {
            LoadData();
            items = SortList();
        }
        return items;
    }

    public virtual bool IsLoaded {
        get {
            return items.Count > 0 ? true : false;
        }
    }

    public virtual void Reset() {
        items = new List<T>();
        packPaths = new List<string>();
    }
        
    public string GetPathItem(string key, string code, string folder) {
        string path = PathUtil.Combine(ContentPaths.appCachePathData, folder.TrimStart('/'));
        return GetPathItem(key, code, folder, path);
    }

    public string GetPathItem(string key, string code, string folder, string fullPath) {
        string pathCode = key + "-" + code + ".json";

        FileSystemUtil.CreateDirectoryIfNeededAndAllowed(fullPath);

        fullPath = PathUtil.Combine(fullPath, pathCode.TrimStart('/'));
        return fullPath;
    }
        
    public T LoadItem<U>(string key, string code) {
        // Load from file individually not a list

        string path = GetPathItem(key, code, "items");
        path = Contents.GetFileVersioned(path);

        DataObject item = new DataObject();

        if(!FileSystemUtil.CheckFileExists(path)) {
            SaveItem(key, code, item);
        }

        historyLevelItems.Clear();

        string jsonData = item.LoadData(path);
        T itemReturn = jsonData.FromJson<T>();
        if(itemReturn != null) {
            return itemReturn;
        }
        else {
            return default(T);
        }
    }

    public void SaveItem(string key, string code, DataObject obj) {
        // Load from file individually not a list

        string path = GetPathItem(key, code, "items");
        path = Contents.GetFileVersioned(path);

        string jsonData = obj.ToJson();

        historyLevelItems.Insert(0, jsonData);
        if(historyLevelItems.Count > 20) {
            historyLevelItems.RemoveAt(historyLevelItems.Count);
        }

        obj.SaveData(path, jsonData);

    }       
}