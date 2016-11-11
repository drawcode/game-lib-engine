using System;
using System.Collections;
using System.Collections.Generic;
using Engine.Data.Json;
using System.Net;
using System.Text;
using Engine.Utility;

using UnityEngine;

using Engine.Events;

public class WWWs : GameObjectBehavior {

    public Dictionary<string,WWW> wwws;

    public bool processing = false;
    public WWW lastWWW = null;

    public delegate void HandleResponseTextCallback(string data);

    public delegate void HandleResponseObjectCallback(ResponseObject responseObject);

    public delegate void HandleResponseBytesCallback(byte[] bytes);
    
    public delegate void HandleResponseErrorCallback(ResponseObject responseObject);

    public HandleResponseBytesCallback callbackBytes;

    public class RequestType {
        public static string post = "http-post";
        public static string get = "http-get";
        public static string put = "http-put";
        public static string del = "http-del";
    }

    public class ResponseObject {
        public bool validResponse;
        public string type;
        public WWW www;
        public int error;
        public string message;
        public string data;
        public string code;
        public string dataValueText;
        public JsonData dataValue;
    
        public ResponseObject() {
            validResponse = false;
            type = "";
            www = null;
            error = 0;
            message = "";
            code = "";
            dataValueText = "";
            data = "";
            dataValue = new JsonData();
        }
    }

    public class StatusMessages {
        public static string notStarted = "wwws-not-started";
        public static string started = "wwws-started";
        public static string success = "wwws-success";
        public static string error = "wwws-error";
    }
    
    public class DataType {
        public static string text = "text";
        public static string binary = "binary";
        public static string assetBundle = "asset-bundle";
        public static string responseObject = "response-object";

    }
    
    public class RequestItem {
        
        public string uuid = ""; // message name
        public string code = ""; // message name
        public string url = ""; // url to request
        public string channel = ""; // request channel - maps to new www class
        
        [JsonIgnore]
        public string status = StatusMessages.notStarted;
        public string dataType = DataType.text;

        public string requestType = RequestType.get;

        public Dictionary<string, object> paramHash = null;

        [JsonIgnore]
        public WWWForm form = null;
        
        [JsonIgnore]
        public bool processing = false;

        public object content = null;

        public RequestItem() {
            Reset();
        }
        
        public void Reset() {
            uuid = UniqueUtil.CreateUUID4();
            url = "";
            code = "profile";
            channel = "default";
            status = StatusMessages.notStarted;
            dataType = DataType.text;
            requestType = RequestType.get;
            paramHash = new Dictionary<string, object>();
            processing = false;
            form = null;
            content = null;
        }

        //

        public bool IsRequestType(string reqType) {
            return requestType == reqType ? true : false;
        }
        
        public void SetRequestType(string reqType) {
            requestType = reqType;
        }

        //
        
        [JsonIgnore]
        public bool POST {

            get {
                return IsRequestType(RequestType.post);
            }

            set {
                if(value) {
                    SetRequestType(RequestType.post);
                }
            }
        }
        
        [JsonIgnore]
        public bool GET {
            
            get {
                return IsRequestType(RequestType.get);
            }
            
            set {
                if(value) {
                    SetRequestType(RequestType.get);
                }
            }
        }

        public bool IsAction(string action) {
            return url.Contains(action);
        }

        // ACTIONS

        public void PrepareParams() {
            if(GET) {
                PrepareParamsGet();
            }
            else if(POST) {
                PrepareParamsPost();
            }
        }

        public void PrepareParamsGet() {

            if (paramHash == null) {
                return;
            }

            if (paramHash.Count > 0) {
                foreach (KeyValuePair<string, object> pair in paramHash) {
                    AddParam(pair.Key, pair.Value);
                }
            }
        }

        public void PrepareParamsPost() {
            
            if (paramHash == null) {
                return;
            }

            form = new WWWForm();

            if (paramHash != null) {
                if (paramHash.Count > 0) {
                    foreach (KeyValuePair<string, object> pair in paramHash) {
                        AddParam(pair.Key, pair.Value);
                    }
                }
            }
        }

        public void AddParam(string key, object value) {

            string val = "";

            if(value != null) {            
                if (typeof(string) == value.GetType()
                    || typeof(float) == value.GetType()
                    || typeof(double) == value.GetType()
                    || typeof(int) == value.GetType()
                    || typeof(DateTime) == value.GetType()) {
                    val = Convert.ToString(value);
                }
                else {
                    val = value.ToJson();
                }
            }

            if(POST) {
                if(form == null) {
                    form = new WWWForm();
                }

                if(val != null) {
                    form.AddField(key, val);
                }
            }
            else if(GET) {
                if(url.Contains(key + "=")) {
                    return; 
                }
                
                System.Text.StringBuilder paramValues = 
                    new System.Text.StringBuilder();

                if (!url.Contains("?")) {
                    paramValues.Append("?");    
                }
                else {
                    paramValues.Append("&");
                }
                
                if(val != null) {
                    paramValues.Append(key);
                    paramValues.Append("=");  
                    paramValues.Append(
                        Uri.EscapeDataString(val.ToString()));
                }
                
                string urlAppend = paramValues.ToString();
                url = url + urlAppend;
            }
        }
    }
// -------------------------------------------------------------------
// Singleton access

// Only one GameCloudSync can exist. We use a singleton pattern to enforce this.
    private static WWWs _instance = null;

    public static WWWs Instance {
        get {
            if (!_instance) {
            
                // check if an ObjectPoolManager is already available in the scene graph
                _instance = FindObjectOfType(typeof(WWWs)) as WWWs;
            
                // nope, create a new one
                if (!_instance) {
                    var obj = new GameObject("_WWWs");
                    _instance = obj.AddComponent<WWWs>();
                }
            }
        
            return _instance;
        }
    }

    public void Awake() {
    
        if (Instance != null && this != Instance) {
            //There is already a copy of this script running
            Destroy(this);
            return;
        }
    }

    public void Init() {
        LogUtil.Log("WWWs Init");

        wwws = new Dictionary<string, WWW>();
    }

    void Start() {
        Init();
        LogUtil.Log("WWWs Start");
    }

    void OnApplicationQuit() {
        _instance = null;
    }

// -------------------------------------------------------------------
// CUSTOM Methods

// Core Request Helpers

    public string EnsureUrlUniqueByTime(string url) {
        if (url.IndexOf('?') > -1) {
            url = url + "&timestamp=" + System.DateTime.Now.Ticks.ToString();
        }
        else {
            url = url + "?timestamp=" + System.DateTime.Now.Ticks.ToString();
        }
        return url;
    }

    // -----------------------------------------------------
    // MESSAGE BASED
    
    public void BroadcastStarted(RequestItem requestItem) {
        
        requestItem.status = StatusMessages.started;
        Messenger<RequestItem>.Broadcast(StatusMessages.started, requestItem);        
    }
    
    public void BroadcastSuccess(RequestItem requestItem) {
        
        requestItem.status = StatusMessages.success;
        Messenger<RequestItem>.Broadcast(StatusMessages.success, requestItem);        
    }

    public void BroadcastError(RequestItem requestItem, string error) {
        
        requestItem.status = StatusMessages.error;
        Messenger<RequestItem,string>.Broadcast(StatusMessages.error, requestItem, error);        
    }
    
    public static void Request(RequestItem requestItem) {
        Instance.request(requestItem);
    }

    public void Request(
        string requestType, 
        string url, 
        Dictionary<string, object> paramHash) {

        RequestItem requestItem = new RequestItem();
        requestItem.url = url;
        requestItem.requestType = requestType;
        requestItem.paramHash = paramHash;
        request(requestItem);
    }

    public void request(RequestItem requestItem) {
    
        if(requestItem == null) {
            return;
        }
                
        requestItem.PrepareParams();

        RequestStart(requestItem);
    }

    public void RequestStart(RequestItem requestItem) {
    
        if (Context.Current.hasNetworkAccess) {
            BroadcastStarted(requestItem);
            StartCoroutine(WaitForResponse(requestItem));
        }
        else {
            BroadcastError(requestItem, "Lost or no network connection");
        }
    }

    public IEnumerator WaitForResponse(RequestItem requestItem) {
    
        WWW www = null;

        requestItem.url = EnsureUrlUniqueByTime(requestItem.url);

        if (requestItem.form != null) {
            www = new WWW(requestItem.url, requestItem.form);
        }
        else {
            www = new WWW(requestItem.url);
        }

        wwws.Set(requestItem.channel, www);

        requestItem.processing = true;
    
        yield return www;    
                                
        if (www.error != null) {
            Debug.LogError("WWW Error:" + www.error + " requestItem.url:" + requestItem.url);
            requestItem.content = www.error;
            requestItem.content += www.text;
            requestItem.processing = false;
            BroadcastError(requestItem, "Error on sync");
            yield break;
        }

        if(requestItem.dataType == DataType.text) {
            
            string text = www.text;

            requestItem.content = text;

            BroadcastSuccess(requestItem);
        }
        else if(requestItem.dataType == DataType.binary) {
            
            byte[] bytes = www.bytes;

            requestItem.content = bytes;
            
            BroadcastSuccess(requestItem);
        }
        else if(requestItem.dataType == DataType.responseObject) {

            ResponseObject responseObject = new ResponseObject();
            responseObject.www = www;
            responseObject.validResponse = true;
            responseObject.type = "";
            
            if (www.error != null) {
                LogUtil.LogError("WWW Error:" + www.error + " requestItem.url:" + requestItem.url);
                responseObject.validResponse = false;
                responseObject.message = www.error;
                responseObject.dataValueText = "";
            }
            
            try {
                responseObject.dataValueText = www.text;
                LogUtil.Log("WWW Text Output:" + www.text);
            }
            catch (System.Exception e) {            
                responseObject.validResponse = false;
                responseObject.message = www.error;
                responseObject.dataValueText = "";
                LogUtil.Log("ERROR:" + e);
            }
                        
            requestItem.content = responseObject;
            
            BroadcastSuccess(requestItem);
        }
    
        requestItem.processing = false;
    }

    void Update() {
    
        //ProgressStatus();
    }

    /*
    public ContentItemStatus contentItemStatus = new ContentItemStatus();

    public ContentItemStatus ProgressStatus() {
    
        if (lastWWW != null && processing) {
            if (lastWWW.isDone) {
                contentItemStatus.downloaded = true;
                contentItemStatus.itemSize = lastWWW.size;
            }       
        
            LogUtil.Log("progress:" + lastWWW.progress);
        
            contentItemStatus.itemProgress = lastWWW.progress;
            contentItemStatus.url = lastWWW.url;
        
            Messenger<ContentItemStatus>.Broadcast("content-item-status", contentItemStatus);           
        }
    
        return contentItemStatus;
    }
    */

}