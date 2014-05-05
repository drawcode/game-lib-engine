using System;
using System.Collections;
using System.Collections.Generic;

using System.Net;
using Engine.Data.Json;
using Engine.Utility;

using UnityEngine;

using Engine.Events;

namespace Engine.Networking {

public class WebRequests : GameObjectBehavior {
	
	public static WebRequests Instance;

	public bool processing = false;
	
	public WWW lastWWW = null;

	public delegate void HandleResponseTextCallback(string data);
	public delegate void HandleResponseObjectCallback(ResponseObject responseObject);
	public delegate void HandleResponseBytesCallback(byte[] bytes);
	//public delegate void HandleResponseAssetBundleCallback(AssetBundle assetBundle);
	//public delegate void HandleResponseAudioClipCallback(AudioClip assetBundle);
	//public delegate void HandleResponseTexture2DCallback(Texture2D texture2d);

	// TODO other types...
	//public delegate void HandleResponseBytesCallback(byte[] bytes);
	//public delegate void HandleResponseWWWCallback(WWW www);
	//public delegate void HandleResponseObjectCallback(ResponseObject responseObject);
	//public delegate void HandleResponseAssetBundleCallback(AssetBundle assetBundle);
	//public delegate void HandleResponseAudioClipCallback(AudioClip assetBundle);
	//public delegate void HandleResponseTexture2DCallback(Texture2D texture2d);
	//public delegate void HandleResponseMovieTextureCallback(MovieTexture movieTexture);
	//public delegate void HandleResponseAudioClipOggVorbisCallback(AudioClip oggVorbisClip);

	//public HandleResponseTextCallback callbackText;
	public HandleResponseBytesCallback callbackBytes;
	//public HandleResponseWWWCallback callbackWWW;
	//public HandleResponseAssetBundleCallback callbackAssetBundle;
	//public HandleResponseTexture2DCallback callbackTexture2D;
	//public HandleResponseAudioClipCallback callbackAudioClip;
	//public HandleResponseMovieTextureCallback callbackMovieTexture;
	//public HandleResponseAudioClipOggVorbisCallback callbackAudioClipOggVorbis;

	public enum RequestType {
		HTTP_POST,
		HTTP_GET,
		HTTP_PUT,
		HTTP_DELETE
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

	// -------------------------------------------------------------------
	// Singleton access

	private static WebRequests _instance = null;

	public static WebRequests instance {
		get {
			if(_instance == null) {
				_instance = FindObjectOfType(typeof(WebRequests)) as WebRequests;
				if(_instance == null)
					LogUtil.LogError("Could not locate an WebRequests object. You have to have exactly one WebRequests in the scene.");
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
		
        Instance = this;
	}

	public void Init() {
		LogUtil.Log("WebRequests Init");
	}

	void Start() {
		Init();
		LogUtil.Log("WebRequests Start");
	}

	void OnApplicationQuit() {
		_instance = null;
	}

	// -------------------------------------------------------------------
	// CUSTOM Methods

	public void ServiceResponseObjectCallback(WebRequests.ResponseObject responseObject) {
		LogUtil.Log("ServiceResponseTextCallback:" + responseObject.dataValueText);
	}

	//public void RequestText(string url, HandleResponseTextCallback callback) {
	//    HandleResponseObjectCallback callbackInternal = ServiceResponseObjectCallback;
	//    Request(url, callbackInternal);
	//    callbackText = callback;
	//}

	//public void RequestTextWithParams(RequestType requestType, string url, Dictionary<string, string> paramHash, HandleResponseWWWCallback callback) {
	//    HandleResponseObjectCallback callbackInternal = ServiceResponseObjectCallback;
	//    Request(requestType, url, paramHash, callbackInternal);
	//    callbackWWW = callback;
	//}

	//public void RequestTexture(string url, HandleResponseTexture2DCallback callback) {
	//    HandleResponseObjectCallback callbackInternal = ServiceResponseObjectCallback;
	//    Request(url, callbackInternal);
	//    callbackTexture2D = callback;
	//}

	//public void RequestBytes(string url, HandleResponseBytesCallback callback) {
	//    HandleResponseObjectCallback callbackInternal = ServiceResponseObjectCallback;
	//    Request(url, callbackInternal);
	//    callbackBytes = callback;
	//}

	//public void RequestAudioClip(string url, HandleResponseAudioClipCallback callback) {
	//    HandleResponseObjectCallback callbackInternal = ServiceResponseObjectCallback;
	//    Request(url, callbackInternal);
	//    callbackAudioClip = callback;
	//}

	//public void RequestAssetBundle(string url, HandleResponseAssetBundleCallback callback) {
	//    HandleResponseObjectCallback callbackInternal = ServiceResponseObjectCallback;
	//    Request(url, callbackInternal);
	//    callbackAssetBundle = callback;
	//}

	//public void RequestMovieTexture(string url, HandleResponseMovieTextureCallback callback) {
	//    HandleResponseObjectCallback callbackInternal = ServiceResponseObjectCallback;
	//    Request(url, callbackInternal);
	//    callbackMovieTexture = callback;
	//}

	//public void RequestAudioClipOggVorbis(string url, HandleResponseAudioClipOggVorbisCallback callback) {
	//    HandleResponseObjectCallback callbackInternal = ServiceResponseObjectCallback;
	//    Request(url, callbackInternal);
	//    callbackAudioClipOggVorbis = callback;
	//}

	// Core Request Helpers

	public string EnsureUrlUniqueByTime(string url) {
		if(url.IndexOf('?') > -1) {
			url = url + "&timestamp=" + System.DateTime.Now.Ticks.ToString();
		}
		else {
			url = url + "?timestamp=" + System.DateTime.Now.Ticks.ToString();
		}
		return url;
	}
	
	// DELEGATE BASED
	
	// -------------------------------------
	// RESPONSE OBJECTS
	
	public void Request(string url, HandleResponseObjectCallback callback) {
		Request(RequestType.HTTP_GET, url, null, callback);
	}

	public void Request(RequestType requestType, string url, HandleResponseObjectCallback callback) {
		Request(requestType, url, null, callback);
	}
	
	public void Request(RequestType requestType, string url, Dictionary<string, object> paramHash, HandleResponseObjectCallback callback) {
		if(requestType == RequestType.HTTP_POST) {
			RequestPost(url, paramHash, callback);
		}
		else {
			RequestGet(url, paramHash, callback);
		}
	}

	public void RequestGet(string url, Dictionary<string, object> paramHash, HandleResponseObjectCallback callback) {
		System.Text.StringBuilder paramValues = new System.Text.StringBuilder();
		if(paramHash != null) {
			if(paramHash.Count > 0) {
				// append param types
				foreach(KeyValuePair<string, object> pair in paramHash) {
					if(paramValues.Length == 0 
						&& !url.Contains("?")) {
						paramValues.Append("?");	
					}
					else {
						paramValues.Append("&");
					}
					
					paramValues.Append(pair.Key);
					paramValues.Append("=");					
					paramValues.Append(Uri.EscapeDataString(pair.Value.ToString()));
				}
			}
		}
		
		string urlAppend = paramValues.ToString();
		url = url + urlAppend;
		
		//LogUtil.Log("url:" + url);
		
		RequestStartCoroutineEnumerator(url, callback);
	}

	public void RequestPost(string url, Dictionary<string, object> paramHash, HandleResponseObjectCallback callback) {
		WWWForm form = new WWWForm();
		if(paramHash != null) {
			if(paramHash.Count > 0) {
				foreach(KeyValuePair<string, object> pair in paramHash) {
					form.AddField(pair.Key, pair.Value.ToString());
				}
			}
		}
		RequestStartCoroutineEnumerator(url, callback, form);
	}

	public void RequestStartCoroutineEnumerator(string url, HandleResponseObjectCallback callback) {
		RequestStartCoroutineEnumerator(url, callback, null);
	}

	public void RequestStartCoroutineEnumerator(string url, HandleResponseObjectCallback callback, WWWForm form) {
		
		if(Context.Current.hasNetworkAccess) {
			//LogUtil.Log("WebRequests: url:" + url);
			StartCoroutine(WaitForResponse(url, form, callback));
		}
		else {			
			ResponseObject responseObject = new ResponseObject();
			responseObject.type = "";
			responseObject.dataValueText = "";
			responseObject.validResponse = false;
			responseObject.message = "No internet access";
			responseObject.error = 99;
			callback(responseObject);
		}
	}
	
	public IEnumerator WaitForResponse(string url, WWWForm form, HandleResponseObjectCallback callback) {
		
		WWW www = null;
		if(form != null) {
			www = new WWW(url, form);
		}
		else {
			www = new WWW(url);
		}
		
		lastWWW = www;
		
		processing = true;
		yield return www;

		ResponseObject responseObject = new ResponseObject();
		responseObject.www = www;
		responseObject.validResponse = true;
		responseObject.type = "";

		if(www.error != null) {
			LogUtil.LogError("WWW Error:" + www.error + " url:" + url);
			responseObject.validResponse = false;
			responseObject.message = www.error;
			responseObject.dataValueText = "";
		}
				
		try {
			responseObject.dataValueText = www.text;
			LogUtil.Log("WWW Text Output:" + www.text);
		}
		catch(System.Exception e) {			
			responseObject.validResponse = false;
			responseObject.message = www.error;
			responseObject.dataValueText = "";
			LogUtil.Log("ERROR:" + e);
		}
		// Call callback after yield
		callback(responseObject);
		//callback = null;
		processing = false;
	}
	
	
	// -----------------------------------------------------
	// BYTES
	
	public void RequestBytes(string url, HandleResponseBytesCallback callback) {
		RequestBytes(RequestType.HTTP_GET, url, null, callback);
	}

	public void RequestBytes(RequestType requestType, string url, HandleResponseBytesCallback callback) {
		RequestBytes(requestType, url, null, callback);
	}
	
	public void RequestBytes(RequestType requestType, string url, Dictionary<string, object> paramHash, HandleResponseBytesCallback callback) {
		if(requestType == RequestType.HTTP_POST) {
			RequestPostBytes(url, paramHash, callback);
		}
		else {
			RequestGetBytes(url, paramHash, callback);
		}
	}

	public void RequestGetBytes(string url, Dictionary<string, object> paramHash, HandleResponseBytesCallback callback) {
		System.Text.StringBuilder paramValues = new System.Text.StringBuilder();
		if(paramHash != null) {
			if(paramHash.Count > 0) {
				// append param types
				foreach(KeyValuePair<string, object> pair in paramHash) {
					if(paramValues.Length == 0 
						&& !url.Contains("?")) {
						paramValues.Append("?");	
					}
					else {
						paramValues.Append("&");
					}
					
					paramValues.Append(pair.Key);
					paramValues.Append("=");					
					paramValues.Append(Uri.EscapeDataString(pair.Value.ToString()));
				}
			}
		}
		
		string urlAppend = paramValues.ToString();
		url = url + urlAppend;
		
		//LogUtil.Log("url:" + url);
		
		RequestStartBytesCoroutineEnumerator(url, callback);
	}
	
	public void RequestPostBytes(string url, Dictionary<string, object> paramHash, HandleResponseBytesCallback callback) {
		WWWForm form = new WWWForm();
		if(paramHash != null) {
			if(paramHash.Count > 0) {
				foreach(KeyValuePair<string, object> pair in paramHash) {
					form.AddField(pair.Key, pair.Value.ToString());
				}
			}
		}
		RequestStartBytesCoroutineEnumerator(url, callback, form);
	}
	
	public void RequestStartBytesCoroutineEnumerator(string url, HandleResponseBytesCallback callback) {
		RequestStartBytesCoroutineEnumerator(url, callback, null);
	}
	public void RequestStartBytesCoroutineEnumerator(string url, HandleResponseBytesCallback callback, WWWForm form) {
		
		if(Context.Current.hasNetworkAccess) {
			//LogUtil.Log("WebRequests: url:" + url);
			StartCoroutine(WaitForResponseBytes(url, form, callback));
		}
		else {			
			byte[] bytes = new byte[0];
			callback(bytes);
		}
	}	
	
	public IEnumerator WaitForResponseBytes(string url, WWWForm form, HandleResponseBytesCallback callback) {
		
		WWW www = null;
		if(form != null) {
			www = new WWW(url, form);
		}
		else {
			www = new WWW(url);
		}
		lastWWW = www;
		
		processing = true;
		yield return www;
		
		byte[] bytes = www.bytes;

		if(www.error != null) {
			LogUtil.LogError("WWW Error:" + www.error + " url:" + url);
		}
		// Call callback after yield
		callback(bytes);
			
		//callback = null;
		processing = false;
	}
	
	
	// -----------------------------------------------------
	// TEXT
	
	public void RequestText(string url, HandleResponseTextCallback callback) {
		RequestText(RequestType.HTTP_GET, url, null, callback);
	}

	public void RequestText(RequestType requestType, string url, HandleResponseTextCallback callback) {
		RequestText(requestType, url, null, callback);
	}
	
	public void RequestText(RequestType requestType, string url, Dictionary<string, object> paramHash, HandleResponseTextCallback callback) {
		if(requestType == RequestType.HTTP_POST) {
			RequestPostText(url, paramHash, callback);
		}
		else {
			RequestGetText(url, paramHash, callback);
		}
	}

	public void RequestGetText(string url, Dictionary<string, object> paramHash, HandleResponseTextCallback callback) {
		System.Text.StringBuilder paramValues = new System.Text.StringBuilder();
		if(paramHash != null) {
			if(paramHash.Count > 0) {
				// append param types
				foreach(KeyValuePair<string, object> pair in paramHash) {
					if(paramValues.Length == 0 
						&& !url.Contains("?")) {
						paramValues.Append("?");	
					}
					else {
						paramValues.Append("&");
					}
					
					paramValues.Append(pair.Key);
					paramValues.Append("=");					
					paramValues.Append(Uri.EscapeDataString(pair.Value.ToString()));
				}
			}
		}
		
		string urlAppend = paramValues.ToString();
		url = url + urlAppend;
		
		//LogUtil.Log("url:" + url);
		
		RequestStartTextCoroutineEnumerator(url, callback);
	}
	
	public void RequestPostText(string url, Dictionary<string, object> paramHash, HandleResponseTextCallback callback) {
		WWWForm form = new WWWForm();
		if(paramHash != null) {
			if(paramHash.Count > 0) {
				foreach(KeyValuePair<string, object> pair in paramHash) {
					form.AddField(pair.Key, pair.Value.ToString());
				}
			}
		}
		RequestStartTextCoroutineEnumerator(url, callback, form);
	}
	
	public void RequestStartTextCoroutineEnumerator(string url, HandleResponseTextCallback callback) {
		RequestStartTextCoroutineEnumerator(url, callback, null);
	}
	public void RequestStartTextCoroutineEnumerator(string url, HandleResponseTextCallback callback, WWWForm form) {
		
		if(Context.Current.hasNetworkAccess) {
			//LogUtil.Log("WebRequests: url:" + url);
			StartCoroutine(WaitForResponseText(url, form, callback));
		}
		else {			
			string text = "";
			callback(text);
		}
	}
	
	public IEnumerator WaitForResponseText(string url, WWWForm form, HandleResponseTextCallback callback) {
		
		WWW www = null;
		if(form != null) {
			www = new WWW(url, form);
		}
		else {
			www = new WWW(url);
		}
		lastWWW = www;
		
		processing = true;
		yield return www;
		
		string text = www.text;

		if(www.error != null) {
			LogUtil.LogError("WWW Error:" + www.error + " url:" + url);
		}
		// Call callback after yield
		callback(text);
			
		//callback = null;
		processing = false;
	}
	
	void Update() {
		
		ProgressStatus();
	}
	
	public ContentItemStatus contentItemStatus = new ContentItemStatus();
	
	public ContentItemStatus ProgressStatus() {
				
		if(lastWWW != null && processing) {
			if(lastWWW.isDone) {
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

}

}


