using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Engine.Content;


#if !UNITY_WEBPLAYER
using System.Reflection;
using System.IO;

#endif
using Engine.Events;

namespace Engine.Game.Data
{
    public enum DataObjectsStorage
    {
        RESOURCES,
        PERSISTENT,
        PREFERENCES,
        SERVER
    }

    public enum DataObjectsType
    {
        LIST,
        OBJECT
    }


    public class DataObjectsMessages
    {
        public static string dataObjectLoaded = "data-object-loaded";
    }

    public class DataObjects<T> where T : DataObject, new()
    {

        public List<T> _items;
        public Dictionary<string, int> _lookupCode;

        //private static object syncRoot = new System.Object();

        public string pathKey = "";
        public string path = "";

        public string currentStateCode = null;

        public List<string> packPaths;
        public List<string> packPathsVersioned;
        public DataObjectsStorage dataStorage = DataObjectsStorage.PERSISTENT;
        public DataObjectsType dataType = DataObjectsType.LIST;
        public List<string> historyLevelItems = new List<string>();

        // If data is stored as a list json dataset

        public List<T> items
        {
            get
            {
                if (_items == null)
                {
                    _items = new List<T>();
                }
                return _items;
            }
            set
            {
                _items = value;
            }
        }

        // If data is stored as a list json dataset

        public Dictionary<string, int> lookupCode
        {
            get
            {
                if (_lookupCode == null)
                {
                    _lookupCode = new Dictionary<string, int>();
                }
                return _lookupCode;
            }
            set
            {
                _lookupCode = value;
            }
        }

        //private static volatile U current;
        //private static volatile U instance;
        //private static object syncRoot = new System.Object();

        /*
         * 
        public static string DATA_KEY = "data-object";

        public static T Current {
            get  {
                if (current == null) {
                    lock (syncRoot)  {
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

        public static AppContentActions Instance {
            get  {
                if (instance == null) {
                    lock (syncRoot)  {
                        if (instance == null) 
                            instance = new AppContentActions(true);
                    }
                }

                return instance;
            }
            set {
                instance = value;
            }
        }

        */

        public DataObjects()
        {
            Reset();

            //Messenger<string>.AddListener(DataObjectsMessages.dataObjectLoaded, OnDataLoaded);
        }

        public virtual void LoadData()
        {

            dataStorage = AppConfigs.dataStorage;

            switch (dataStorage)
            {

                case DataObjectsStorage.SERVER:
                    LogUtil.Log("LoadData:LoadDataFromServer:" + path + " " + pathKey);
                    LoadDataFromServer();
                    break;

                case DataObjectsStorage.RESOURCES:
                    Debug.Log("LoadData:LoadDataFromResources:" + path + " " + pathKey);
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

            //BroadcastDataLoaded();
        }

        public virtual void BroadcastDataLoaded()
        {

            Messenger<string>.Broadcast(DataObjectsMessages.dataObjectLoaded, pathKey);
        }

        public void OnDataLoaded(string key)
        {

            if (key.IsEqualLowercase(pathKey))
            {
                LoadState();
            }
        }

        public virtual void LoadState()
        {

            string val = GetStateCode();

            if (val.IsNullOrEmpty())
            {
                return;
            }

            SetStateCode(val);
        }

        public virtual string GetStateCode()
        {

            string val = GameProfiles.Current.GetGameDataState(pathKey);

            if (val.IsNullOrEmpty()
                || val.IsEqualLowercase(BaseDataObjectKeys.defaultKey))
            {
                return null;
            }

            return val;
        }

        public virtual void SetStateCode(string val)
        {

            if (val.IsNullOrEmpty()
                || val.IsEqualLowercase(BaseDataObjectKeys.defaultKey))
            {
                return;
            }

            GameProfiles.Current.SetGameDataState(pathKey, val);

            currentStateCode = val;
        }

        public virtual void ChangeCurrent(string codeTo)
        {
            SetStateCode(codeTo);
        }

        public virtual void LoadDataFromPersistent()
        {
            LoadDataFromPersistent(path);
        }

        public virtual void LoadDataFromResources()
        {

            string pathResources = path;

            if (!path.Contains(ContentsConfig.contentAppFolder))
            {

                //Debug.Log("LoadDataFromResources:pathResources:" + pathResources);
                //Debug.Log("LoadDataFromResources:ContentsConfig.contentRootFolder:" + ContentsConfig.contentRootFolder);
                //Debug.Log("LoadDataFromResources:ContentsConfig.contentAppFolder:" + ContentsConfig.contentAppFolder);
                //Debug.Log("LoadDataFromResources:Application.persistentDataPath:" + Application.persistentDataPath);
                //Debug.Log("LoadDataFromResources:Application.dataPath:" + Application.dataPath);

                /*
                if (Application.isWebPlayer) {

                    System.Text.StringBuilder sbPath = new System.Text.StringBuilder();

                    sbPath.Append(ContentsConfig.contentRootFolder);
                    sbPath.Append("/");
                    sbPath.Append(ContentsConfig.contentAppFolder);
                    sbPath.Append("/version/");
                    sbPath.Append(pathResources);

                    pathResources = sbPath.ToString();

                    Debug.Log("LoadDataFromResources:web:pathResources:" + pathResources);
                }
                else {


                    sbPath.Append(ContentsConfig.contentRootFolder);
                    sbPath.Append("/");
                    sbPath.Append(ContentsConfig.contentAppFolder);
                    sbPath.Append("/version/");
                    sbPath.Append(pathResources);

                    pathResources = sbPath.ToString();
                    pathResources = pathResources.Replace("data/", "");

                    pathResources =
                     PathUtil.Combine(ContentPaths.appCachePathData, pathResources)
                         .Replace(Application.persistentDataPath, "")
                         .Replace(Application.dataPath, "");

                    if (pathResources.EndsWith(".json")) {
                        //pathResources += ".txt";
                    }

                    if (pathResources.StartsWith("/")) {
                        pathResources = pathResources.TrimStart('/');
                    }

                    if (pathResources.Contains("/" + ContentsConfig.contentVersion + "/")) {
                        pathResources = pathResources.Replace("/" + ContentsConfig.contentVersion + "/", "/version/");
                    }

                }
                */


                System.Text.StringBuilder sbPath = new System.Text.StringBuilder();

                sbPath.Append(ContentsConfig.contentRootFolder);
                sbPath.Append("/");
                sbPath.Append(ContentsConfig.contentAppFolder);
                sbPath.Append("/version/");
                sbPath.Append(pathResources);

                pathResources = sbPath.ToString();
            }

            Debug.Log("LoadDataFromResources:pathResources:" + pathResources);

            string data = LoadDataFromResources(pathResources);
            LoadDataFromString(data);
        }

        public virtual string DataToJson()
        {

            if (_items != null)
            {
                return items.ToJson();
            }

            return null;
        }

        public virtual bool SaveDataItemsToResources()
        {
            return SaveDataItemsToResources(path);
        }

        public virtual bool SaveDataItemsToResources(string path)
        {
            bool saved = false;
            string fileData = "";
            //string pathPart = path;

            path =
                Path.Combine(
                Application.dataPath,
                path);

            Debug.Log("SaveDataItemsToResources:path:" + path);

            if (_items != null)
            {
                fileData = items.ToJson();
            }

            if (path.EndsWith(".json"))
            {
                path = path + ".txt";
            }

            FileSystemUtil.WriteString(path, fileData);

            Debug.Log("SaveDataItemsToResources:fileData:" + fileData + " " + pathKey);

            saved = true;

            return saved;
        }

        //

        public virtual void LoadDataFromPrefs()
        {
            string data = LoadDataFromPrefs(pathKey);

            LoadDataFromString(data);
        }

        public void LoadDataFromServer()
        {
            // TODO
            LoadDataFromServer(path);
        }

        public void LoadDataFromServer(string key)
        {
            // TODO
        }

        public string LoadDataFromPrefs(string key)
        {
            string data = "";

            if (!SystemPrefUtil.HasLocalSetting(key))
            {
                data = SystemPrefUtil.GetLocalSettingString(key);
            }
            return data;
        }

        public string LoadDataFromPersistent(string path)
        {
            return LoadDataFromPersistent(path, true);
        }

        public string LoadDataFromPersistent(string path, bool versioned)
        {
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

        public bool SaveDataItemsToPersistent(string path)
        {
            bool saved = false;
            string fileData = "";
            string pathPart = path;

            path = PathUtil.Combine(ContentPaths.appCacheVersionPath, path.TrimStart('/'));
            string pathVersioned = Contents.GetFileVersioned(path);

            //LogUtil.Log("LoadDataFromPersistent:path:" + path);
            //LogUtil.Log("LoadDataFromPersistent:pathVersioned:" + pathVersioned + " " + pathKey);

            bool prepared = PreparePersistentFile(pathPart, pathVersioned);

            if (!prepared)
                return false;

            fileData = DataToJson();

            FileSystemUtil.WriteString(pathVersioned, fileData);

            //LogUtil.Log("SaveDataItemsToPersistent:fileData:" + fileData + " " + pathKey);

            saved = true;

            return saved;
        }

        public bool SaveDataItemToPersistent(T obj, string path)
        {
            bool saved = false;
            string fileData = "";
            string pathPart = path;

            path = PathUtil.Combine(ContentPaths.appCacheVersionPath, path.TrimStart('/'));
            string pathVersioned = Contents.GetFileVersioned(path);

            //LogUtil.Log("LoadDataFromPersistent:path:" + path);
            //LogUtil.Log("LoadDataFromPersistent:pathVersioned:" + pathVersioned + " " + pathKey);

            bool prepared = PreparePersistentFile(pathPart, pathVersioned);

            if (!prepared)
                return false;

            fileData = obj.ToJson();

            FileSystemUtil.WriteString(pathVersioned, fileData);

            //LogUtil.Log("SaveDataItemsToPersistent:fileData:" + fileData + " " + pathKey);

            saved = true;

            return saved;
        }

        public bool PreparePersistentFile(string pathPart, string pathVersioned)
        {
            bool prepared = true;

            if (!FileSystemUtil.CheckFileExists(pathVersioned))
            {
                prepared = false;

                //LogUtil.Log("LoadDataFromPersistent:pathVersioned not exist:" + pathVersioned + " " + pathKey);

                // copy from streaming assets
                string pathToCopy = PathUtil.Combine(ContentPaths.appCacheVersionPath, pathPart.TrimStart('/'));
                //LogUtil.Log("LoadDataFromPersistent:pathToCopy:" + pathToCopy + " " + pathKey);
                if (FileSystemUtil.CheckFileExists(pathToCopy))
                {
                    FileSystemUtil.MoveFile(pathToCopy, pathVersioned);
                    //LogUtil.Log("LoadDataFromPersistent:file moved:" + pathToCopy + " " + pathKey);
                    prepared = true;
                }
                else
                {
                    //LogUtil.Log("LoadDataFromPersistent:move not exist:" + pathToCopy + " " + pathKey);
                    prepared = false;
                }
            }
            return prepared;
        }

        public void LoadDataFromPersistentPacks(string path)
        {

            // Append data from additional pack data files

            List<T> appendList = new List<T>();
            List<T> appendItemList = new List<T>();

            foreach (string packPath in ContentPaths.GetPackPathsVersioned())
            {
                string data = "";
                string pathData = PathUtil.Combine(packPath, path.TrimStart('/'));
                FileSystemUtil.EnsureDirectory(pathData);

                string[] packDirs = packPath.TrimEnd('/').Replace("data", "").TrimEnd('/').Split('/');
                string packDirName = packDirs[packDirs.Length - 1];

                string fileVersioned = Contents.GetFullPathVersioned(pathData);

                FileSystemUtil.EnsureDirectory(fileVersioned);

                if (FileSystemUtil.CheckFileExists(fileVersioned))
                {
                    data = Contents.GetFileDataFromPersistentCache(pathData, true, true);
                }

                if (!string.IsNullOrEmpty(data))
                {
                    List<T> objs = new List<T>();

                    objs = LoadDataFromString(appendItemList, data);

                    int i = 0;

                    for (int j = 0; j < objs.Count; j++)
                    {
                        SetFieldValue(objs[j], "pack_code", packDirName);
                        SetFieldValue(objs[j], "pack_sort", i++);
                    }

                    if (objs != null && objs.Count > 0)
                    {
                        appendList.AddRange(objs);
                    }
                }
            }

            ////LogUtil.Log("!!!!!! PackPathsVersionShared:" + Contents.GetPackPathsVersionedShared().Count);

            foreach (string packPath in ContentPaths.GetPackPathsVersionedShared())
            {
                string data = "";
                string pathData = PathUtil.Combine(packPath, path.TrimStart('/'));
                FileSystemUtil.EnsureDirectory(pathData);

                string[] packDirs = packPath.TrimEnd('/').Replace("data", "").TrimEnd('/').Split('/');
                string packDirName = packDirs[packDirs.Length - 1];

                string fileVersioned = Contents.GetFullPathVersioned(pathData);
                FileSystemUtil.EnsureDirectory(fileVersioned);

                if (FileSystemUtil.CheckFileExists(fileVersioned))
                {
                    data = Contents.GetFileDataFromPersistentCache(pathData, true, true);
                }

                if (!string.IsNullOrEmpty(data))
                {
                    List<T> objs = new List<T>();

                    objs = LoadDataFromString(appendItemList, data);

                    int i = 0;

                    for (int j = 0; j < objs.Count; j++)
                    {
                        SetFieldValue(objs[j], "pack_code", packDirName);
                        SetFieldValue(objs[j], "pack_sort", i++);
                    }

                    if (objs != null && objs.Count > 0)
                    {
                        appendList.AddRange(objs);
                    }
                }
            }

            foreach (string packPath in ContentPaths.GetPackPathsNonVersioned())
            {
                string data = "";
                string pathData = PathUtil.Combine(packPath, path.TrimStart('/'));
                FileSystemUtil.EnsureDirectory(pathData);

                string[] packDirs = packPath.TrimEnd('/').Replace("data", "").TrimEnd('/').Split('/');
                string packDirName = packDirs[packDirs.Length - 1];

                if (string.IsNullOrEmpty(packDirName))
                {
                    packDirName = "";
                }

                string fileVersioned = Contents.GetFullPathVersioned(pathData);
                FileSystemUtil.EnsureDirectory(fileVersioned);

                if (FileSystemUtil.CheckFileExists(fileVersioned))
                {
                    ////LogUtil.Log(">> PACK FILE EXISTS: " + pathData);
                    data = Contents.GetFileDataFromPersistentCache(pathData, true, true);
                    ////LogUtil.Log(">> PACK FILE DATA: " + data);
                }

                if (!string.IsNullOrEmpty(data))
                {
                    if (!string.IsNullOrEmpty(data))
                    {
                        List<T> objs = new List<T>();

                        objs = LoadDataFromString(appendItemList, data);

                        int i = 0;

                        for (int j = 0; j < objs.Count; j++)
                        {
                            SetFieldValue(objs[j], "pack_code", packDirName);
                            SetFieldValue(objs[j], "pack_sort", i++);
                        }

                        if (objs != null && objs.Count > 0)
                        {
                            appendList.AddRange(objs);
                        }
                    }
                }
            }

            // TODO update dict lookups

            items.AddRange(appendList);

        }

        public string LoadDataFromResources(string resourcesPath)
        {
            string fileData = "";

            //Debug.Log("LoadDataFromResources:string:resourcesPath:" + resourcesPath + " " + pathKey);

            TextAsset textData = Resources.Load(resourcesPath, typeof(TextAsset)) as TextAsset;

            if (textData != null)
            {
                //Debug.Log("LoadDataFromResources:textData.text:" + textData.text);
                fileData = textData.text;

                //using(StreamReader sr = new StreamReader(new MemoryStream(level.bytes))) {
                //
                //}
            }

            //Debug.Log("LoadDataFromResources:fileData:" + fileData + " " + pathKey);

            return fileData;
        }

        public virtual void LoadDataFromString(string data)
        {

            //Debug.Log("LoadDataFromString:hasData:" + !data.IsNullOrEmpty());

            if (!string.IsNullOrEmpty(data))
            {

                items.Clear();
                items = LoadDataFromString(items, data);

                //Debug.Log("LoadDataFromString:items.Count:" + items.Count);

                UpdateLookups();
            }
        }

        public virtual Dictionary<string, T> LoadDataFromString(Dictionary<string, T> objs, string data)
        {

            if (!string.IsNullOrEmpty(data))
            {

                try
                {
                    objs = data.FromJson<Dictionary<string, T>>();
                }
                catch (Exception e)
                {
                    Debug.Log("LoadDataFromString:" + " e:" + e.Message + " data:" + data + " objs:" + objs.ToJson());
                }

                Debug.Log(objs.GetType().ToString() + " T loaded:" + objs.Count);

                //for (int j = 0; j < objs.Count; j++) {
                //    SetFieldValue(objs[j], "pack_code", "default");
                //}
            }

            return objs;
        }

        public virtual List<T> LoadDataFromString(List<T> objs, string data)
        {

            if (!string.IsNullOrEmpty(data))
            {

                try
                {
                    objs = data.FromJson<List<T>>();
                }
                catch (Exception e)
                {
                    Debug.Log("LoadDataFromString:" + " e:" + e.Message + " data:" + data + " objs:" + objs.ToJson());
                }

                Debug.Log(objs.GetType().ToString() + " T loaded:" + objs.Count);

                for (int j = 0; j < objs.Count; j++)
                {
                    SetFieldValue(objs[j], "pack_code", "default");
                }
            }
            return objs;
        }

        public T GetByCode(string code)
        {
            return GetByStringKey("code", code);
        }

        public T GetById(string id)
        {
            return GetByStringKey("code", id);
        }

        public T GetByPackCode(string packCode)
        {
            return GetByStringKey("pack_code", packCode);
        }

        public bool CheckById(string id)
        {
            return CheckByStringKey("code", id);
        }

        public bool CheckByCode(string code)
        {
            return CheckByStringKey("code", code);
        }

        public T GetByUuid(string id)
        {
            return GetByStringKey("uuid", id);
        }

        public bool CheckByUuid(string id)
        {
            return CheckByStringKey("uuid", id);
        }

        public T GetByName(string id)
        {
            return GetByStringKey("name", id);
        }

        public bool CheckByName(string id)
        {
            return CheckByStringKey("name", id);
        }

        public Dictionary<string, string> attributes;

        public void Inspect<U>(U obj) where U : BaseDataObject
        {

            // harvest the properties, fields and keys if not already done.

            if (attributes == null)
            {

                attributes = new Dictionary<string, string>();

                if (typeof(BaseDataObject).IsAssignableFrom(obj.GetType()))
                {

                    foreach (string key in ((BaseDataObject)obj).Keys)
                    {

                        if (attributes.ContainsKey(key))
                        {
                            attributes[key] = "dict";
                        }
                        else
                        {
                            attributes.Add(key, "dict");
                        }
                    }
                }

                // add fields
                foreach (System.Reflection.FieldInfo fieldInfo in obj.GetType().GetFields())
                {

                    string key = fieldInfo.Name;

                    if (attributes.ContainsKey(key))
                    {
                        attributes[key] = "field";
                    }
                    else
                    {
                        attributes.Add(key, "field");
                    }
                }

                // add properties
                foreach (System.Reflection.PropertyInfo propInfo in obj.GetType().GetProperties())
                {

                    string key = propInfo.Name;

                    if (attributes.ContainsKey(key))
                    {
                        attributes[key] = "property";
                    }
                    else
                    {
                        attributes.Add(key, "property");
                    }
                }

                // TODO dictionary keys...
            }
        }

        public T GetByStringKey(string key, string keyValue)
        {

            if (keyValue.IsNullOrEmpty())
            {
                return default(T);
            }

            LoadAll();

            if (key == "code")
            {

                if (lookupCode.ContainsKey(keyValue))
                {

                    int index = lookupCode.Get<int>(keyValue);
                    T t = GetAll()[index];
                    if (t != default(T))
                    {
                        return t;
                    }
                }
                else
                {
                    return default(T);
                }
            }

            foreach (T obj in GetAll())
            {

                if (Has(obj, key))
                {

                    if (keyValue == GetFieldValue<string>(obj, key))
                    {
                        return obj;
                    }
                }
                else
                {
                    return default(T);
                }
            }

            return default(T);
        }

        public bool Has(T obj, string key)
        {

            Inspect<T>(obj);
            if (attributes.ContainsKey(key))
            {
                return true;
            }
            return false;
        }

        public U GetFieldValue<U>(T obj, string fieldName)
        {

            object val = null;

            if (Has(obj, fieldName))
            {

                string type = attributes[fieldName];

                if (type == "dict")
                {
                    if (typeof(BaseDataObject).IsAssignableFrom(obj.GetType()))
                    {
                        if (((BaseDataObject)obj).ContainsKey(fieldName))
                        {
                            val = ((BaseDataObject)obj)[fieldName];
                        }
                    }
                }
                else if (type == "field")
                {
                    System.Reflection.FieldInfo field = obj.GetType().GetField(fieldName);
                    if (field != null)
                    {
                        val = field.GetValue(obj);
                    }
                }
                else if (type == "property")
                {
                    System.Reflection.PropertyInfo prop = obj.GetType().GetProperty(fieldName);
                    if (prop != null)
                    {
                        if (prop.Name == fieldName)
                        {
                            val = prop.GetValue(obj, null);
                        }
                    }
                }
            }

            try
            {
                return (U)val;
            }
            catch (Exception e)
            {
                LogUtil.Log(e);
                return default(U);
            }
        }

        public void SetFieldValue(T obj, string fieldName, object fieldValue)
        {
            //bool hasSet = false;

            if (Has(obj, fieldName))
            {
                string type = attributes[fieldName];
                if (type == "dict")
                {
                    if (typeof(BaseDataObject).IsAssignableFrom(obj.GetType()))
                    {
                        if (((BaseDataObject)obj).ContainsKey(fieldName))
                        {
                            ((BaseDataObject)obj)[fieldName] = fieldValue;
                        }
                    }
                }
                else if (type == "field")
                {
                    System.Reflection.FieldInfo field = obj.GetType().GetField(fieldName);
                    if (field != null)
                    {
                        field.SetValue(obj, fieldValue);
                    }
                }
                else if (type == "property")
                {
                    System.Reflection.PropertyInfo prop = obj.GetType().GetProperty(fieldName);
                    if (prop != null)
                    {
                        if (prop.Name == fieldName)
                        {
                            prop.SetValue(obj, fieldValue, null);
                        }
                    }
                }
            }
        }

        public bool CheckByStringKey(string key, string keyValue)
        {

            foreach (T obj in GetAll())
            {

                Inspect<T>(obj);

                if (attributes.ContainsKey(key))
                {
                    string val = GetFieldValue<string>(obj, key);
                    if (val != keyValue && !string.IsNullOrEmpty(val))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public List<T> GetList(string key, object val)
        {
            //LogUtil.Log("GetList:" + " key:" + key + " val:" + val);
            List<T> list = new List<T>();
            foreach (T t in GetAll())
            {
                object obj = GetFieldValue<object>(t, key);
                //LogUtil.Log("GetList:" + " obj:" + obj);
                if (obj != null)
                {
                    if (obj.Equals(val))
                    {
                        //LogUtil.Log("GetList: adding t:" + t);
                        list.Add(t);
                    }
                }
            }
            return list;
        }

        public List<T> GetListLike(string key, object val)
        {
            //LogUtil.Log("GetList:" + " key:" + key + " val:" + val);
            List<T> list = new List<T>();
            foreach (T t in GetAll())
            {
                object obj = GetFieldValue<object>(t, key);
                //LogUtil.Log("GetList:" + " obj:" + obj);
                if (obj != null)
                {
                    if (obj.GetType() == typeof(string))
                    {
                        string objString = Convert.ToString(obj);
                        string valString = Convert.ToString(val);
                        if (objString.Contains(valString))
                        {
                            //LogUtil.Log("GetList: adding t:" + t);
                            list.Add(t);
                        }
                    }
                }
            }
            return list;
        }

        public List<T> GetListPack(string key, object val, bool all)
        {
            //LogUtil.Log("GetList:" + " key:" + key + " val:" + val);
            List<T> list = new List<T>();
            foreach (T t in GetAll())
            {
                object obj = GetFieldValue<object>(t, key);
                string strObj = "";
                if (obj != null)
                {
                    strObj = obj.ToString();
                    if (strObj != null)
                    {
                        strObj = strObj.ToLower();
                    }
                }
                string strVal = "";
                if (val != null)
                {
                    strVal = val.ToString();
                    if (strVal != null)
                    {
                        strVal = strVal.ToLower();
                    }
                }
                //LogUtil.Log("GetList:" + " obj:" + obj);
                if (obj != null)
                {
                    if ((obj.Equals(val)
                        || strObj == strVal)
                        || (all
                        && (
                         obj.Equals("*")
                        || obj.Equals("all")
                        || obj.Equals("default")
                        || obj.Equals("app-state-all")
                        || obj.Equals("app-pack-all")
                         )
                     ))
                    {

                        //LogUtil.Log("GetList: adding t:" + t);
                        list.Add(t);
                    }
                }
            }
            return list;
        }

        public List<T> GetListByUuid(string val)
        {
            return GetList("uuid", val);
        }

        public List<T> GetListByCode(string val)
        {
            return GetList("code", val);
        }

        public List<T> GetListByPack(string val)
        {
            return GetListPack(val, true);
        }

        public List<T> GetListByParentCode(string val)
        {
            return GetList("parent_code", val);
        }

        public List<T> GetListPack(object val, bool all)
        {
            return GetListPack("pack_code", val, all);
        }

        public List<T> GetListByPackExplicit(string val)
        {
            return GetListPack(val, false);
        }

        public List<T> GetListByType(string val)
        {
            return GetList("type", val);
        }

        //
        //  

        public List<T> GetList(Predicate<T> match)
        {
            return items.FindAll(match);
        }

        public List<T> GetList(List<T> fromList, Predicate<T> match)
        {
            return fromList.FindAll(match);
        }

        //

        public List<T> SortList()
        {
            return SortList(items);
        }

        public List<T> SortList(List<T> listItems)
        {

            if (listItems == null)
            {
                return null;
            }

            listItems.Sort(
                delegate (T c1, T c2)
                {
                    //LogUtil.Log("sorting:c1:", c1);
                    //LogUtil.Log("sorting:c2:", c2);
                    if (GetFieldValue<object>(c1, "sort_order") != null)
                    {
                        int sort1 = (int)GetFieldValue<int>(c1, "sort_order");
                        int sort2 = (int)GetFieldValue<int>(c2, "sort_order");
                        //LogUtil.Log("sorting:sort1:", sort1);
                        //LogUtil.Log("sorting:sort2:", sort2);
                        return sort1.CompareTo(sort2);
                    }
                    else
                    {
                        return -1;
                    }
                }
            );

            return listItems;
        }

        public void UpdateLookups()
        {

            lookupCode.Clear();

            for (int i = 0; i < items.Count(); i++)
            {
                string _code = GetFieldValue<string>(items[i], "code");
                lookupCode.Set<int>(_code, i);
            }
        }

        public void LoadAll()
        {

            //LogUtil.Log("GetAll:IsLoaded:", IsLoaded);

            if (!IsLoaded)
            {

                LoadData();

                List<T> itemsActive = new List<T>();

                foreach (T t in items)
                {
                    bool active = GetFieldValue<bool>(t, "active");
                    if (active)
                    {
                        itemsActive.Add(t);
                    }
                }

                items.Clear();

                foreach (T t in itemsActive)
                {
                    items.Add(t);
                }

                items = SortList(items);

                itemsActive.Clear();
                itemsActive = null;

                UpdateLookups();
            }

            if (lookupCode.Count != items.Count)
            {
                UpdateLookups();
            }
        }

        public List<T> GetAll()
        {

            //LogUtil.Log("GetAll:IsLoaded:", IsLoaded);

            LoadAll();

            return items;
        }

        public int CountAll()
        {

            LoadAll();

            if (_items != null)
            {

                return items.Count;
            }

            return 0;
        }

        public virtual bool IsLoaded
        {
            get
            {
                //if (list == null && dict == null) {
                //    return false;
                //}

                if (_items != null)
                {
                    return items.Count > 0 ? true : false;
                }

                return false;
            }
        }

        public virtual bool HasData
        {
            get
            {
                return CountAll() > 0 ? true : false;
            }
        }

        public virtual void Reset()
        {

            if (_lookupCode != null)
            {

                lookupCode.Clear();
            }

            if (_items != null)
            {

                items.Clear();
            }

            packPaths = new List<string>();
        }

        public string GetPathItem(string key, string code, string folder)
        {
            string path = PathUtil.Combine(ContentPaths.appCachePathData, folder.TrimStart('/'));
            return GetPathItem(key, code, folder, path);
        }

        public string GetPathItem(string key, string code, string folder, string fullPath)
        {
            string pathCode = key + "-" + code + ".json";

            FileSystemUtil.CreateDirectoryIfNeededAndAllowed(fullPath);

            fullPath = PathUtil.Combine(fullPath, pathCode.TrimStart('/'));
            return fullPath;
        }

        //public T LoadItem<U>(string key, string code) {
        public T LoadItem(string key, string code)
        {
            // Load from file individually not a list

            string path = GetPathItem(key, code, "items");
            path = Contents.GetFileVersioned(path);

            DataObject item = new DataObject();

            if (!FileSystemUtil.CheckFileExists(path))
            {
                SaveItem(key, code, item);
            }

            historyLevelItems.Clear();

            string jsonData = item.LoadData(path);
            T itemReturn = jsonData.FromJson<T>();
            if (itemReturn != null)
            {
                return itemReturn;
            }
            else
            {
                return default(T);
            }
        }

        public void SaveItem(string key, string code, DataObject obj)
        {
            // Load from file individually not a list

            string path = GetPathItem(key, code, "items");
            path = Contents.GetFileVersioned(path);

            string jsonData = obj.ToJson();

            historyLevelItems.Insert(0, jsonData);
            if (historyLevelItems.Count > 20)
            {
                historyLevelItems.RemoveAt(historyLevelItems.Count);
            }

            obj.SaveData(path, jsonData);

        }
    }
}