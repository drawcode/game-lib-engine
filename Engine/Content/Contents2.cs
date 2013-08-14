using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Engine.Data.Json;
using Engine.Events;
using Engine.Game.Data;
using Engine.Networking;
using Engine.Utility;

namespace Engine.Content {
		
	public class ContentItemStatus {
		public double itemSize = 0;
		public double itemProgress = 0;
		public string url = "";
		
		public bool downloaded = false;
		
		public double percentageCompleted {
			get{ 
				return itemProgress;
			}
		}
		
		public bool completed {
			get {
				return percentageCompleted == 1 ? true : false;
			}
		}	
	}
	
	public class ContentItemAccess : DataObject {
		public bool globalItem = true;
		public string code = "";
		public string profileId = "";
		public string receipt = "";
		public string platform = "ios-storekit";
		public int quantity = 1;
		public string productCode = "";
	}
	
	public class ContentItemAccessDictionary : DataObject {
		public Dictionary<string, ContentItemAccess> accessItems = new Dictionary<string, ContentItemAccess>();
			
		public void CheckDictionary() {
			if(accessItems == null)
				accessItems = new Dictionary<string, ContentItemAccess>();
		}
		
		public bool CheckAccess(string key) {
			CheckDictionary();
			bool hasAccess = accessItems.ContainsKey(key);
			LogUtil.Log("CheckAccess:: key: " + key + " hasAccess: " + hasAccess);
			return hasAccess;
		}
		
		public ContentItemAccess GetContentAccess(string key) {
			if(CheckAccess(key)) {
				return accessItems[key];
			}
			return null;
		}
		
		public void SetContentAccess(string key) {		
			CheckDictionary();
			ContentItemAccess itemAccess;
			
			if(CheckAccess(key)) {
				itemAccess = GetContentAccess(key);
				itemAccess.code = key;
				itemAccess.globalItem = true;
				itemAccess.platform = "";
				itemAccess.productCode = key;
				itemAccess.profileId = "";
				itemAccess.quantity = 1;
				itemAccess.receipt = "";		
				SetContentAccess(itemAccess);
			}
			else {
				itemAccess = new ContentItemAccess();
				itemAccess.code = key;
				itemAccess.globalItem = true;
				itemAccess.platform = "";
				itemAccess.productCode = key;
				itemAccess.profileId = "";
				itemAccess.quantity = 1;
				itemAccess.receipt = "";
				SetContentAccess(itemAccess);
			}
		}
		
		public void SetContentAccess(ContentItemAccess itemAccess) {
					
			CheckDictionary();
			
			if(CheckAccess(itemAccess.code)) {
				accessItems[itemAccess.code] = itemAccess;
			}
			else {
				accessItems.Add(itemAccess.code, itemAccess);
			}
		}
		
		public void SetContentAccessTransaction(string key, string productId, string receipt, int quantity, bool save) {
			ContentItemAccess itemAccess = GetContentAccess(key);
			if(itemAccess != null) {
				itemAccess.receipt = receipt;
				itemAccess.productCode = productId;
				itemAccess.quantity = quantity;
				SetContentAccess(itemAccess);
				if(save) {
					Save();
				}
			}
		}
		
		public void Save() {
			CheckDictionary();
			string contentItemAccessString = "";
			string settingKey = "ssg-cal";
			contentItemAccessString = JsonMapper.ToJson(accessItems);
			LogUtil.Log("Save: access:" + contentItemAccessString);
			SystemPrefUtil.SetLocalSettingString(settingKey, contentItemAccessString);
			SystemPrefUtil.Save();
		}
		
		public void Load() {
			string settingKey = "ssg-cal";
			if(SystemPrefUtil.HasLocalSetting(settingKey)) {
				// Load from persistence
				string keyValue = SystemPrefUtil.GetLocalSettingString(settingKey);
				LogUtil.Log("Load: access:" + keyValue);
				accessItems = JsonMapper.ToObject<Dictionary<string, ContentItemAccess>>(keyValue);
				CheckDictionary();
			}
		}
						
	}
	
	
	//{"info": "", "status": "", "error": 0, "action": "sx-2012-pack-1", "message": "Success!", "data": {"download_urls": ["https://s3.amazonaws.com/game-supasupacross/1.1/ios/sx-2012-pack-1.unity3d?Signature=rJ%2Fe863up9wgAutleNY%2F%2B7OSy%2BU%3D&Expires=1332496714&AWSAccessKeyId=0YAPDVPCN85QV96YR382"], "access_allowed": true, "date_modified": "2012-03-21T10:58:34.919000", "udid": "[udid]", "tags": ["test", "fun"], "content": "this is \"real\"...", "url": "ffff", "version": "1.1", "increment": 1, "active": true, "date_created": "2012-03-21T10:58:34.919000", "type": "application/octet-stream"}}
	
	public class DownloadableContentItem {
		public List<string> download_urls = new List<string>();
		//public DateTime date_modified = DateTime.Now;
		//public string udid = "";
		//public List<string> tags = new List<string>();
		//public string content = "";
		//public string url = "";
		//public string code = "";
		//public string version = "1.1";
		//public double increment = 3;
		//public bool active = true;
		//public DateTime date_created = DateTime.Now;
		//public string type = "application/octet-stream";	
		//public bool access_allowed = true;
	}
	
	public class DownloadableContentItemResponse : BaseObjectResponse {
		public DownloadableContentItem data = new DownloadableContentItem();
		
		public DownloadableContentItemResponse() {
			Reset();
		}
			
		public override void Reset() {
			base.Reset();
			data = new DownloadableContentItem();
		}
	}
	
	public class BaseObjectResponse {
		public string info = "";
		public string status = "";
		public string code = "0";
		public string action = "";
		public string message = "Success";
		
		public BaseObjectResponse() {
			Reset();
		}
		
		public virtual void Reset() {
			info = "";
			status = "";
			code = "0";
			action = "";
			message = "Success";
		}
	}
	
	//"info": "ssg_ssc_1_1", "status": "", "code": "0", "action": "sx-2012-pack-1", "message": "Success!", "data": {"download_urls": ["http://s3.amazonaws.com/game-supasupacross/1.1/ios/sx-2012-pack-1.unity3d?Signature=9VJYzvaLZjeVcakz4DBDDg51Fwo%3D&Expires=1332704684&AWSAccessKeyId=0YAPDVPCN85QV96YR382"]}
	
	public class ContentMessages {
		public static string ContentItemDownloadSuccess = "content-item-download-success";
		public static string ContentItemDownloadError = "content-item-download-error";
		public static string ContentItemDownloadStarted = "content-item-download-started";
		
		public static string ContentItemVerifySuccess = "content-item-verify-success";
		public static string ContentItemVerifyError = "content-item-verify-error";
		public static string ContentItemVerifyStarted = "content-item-verify-started";	
		
		public static string ContentItemLoadSuccess = "content-item-verify-success";
		public static string ContentItemLoadError = "content-item-verify-error";
		public static string ContentItemLoadStarted = "content-item-verify-started";
		
		public static string ContentItemProgress = "content-item-progress";
	}
	
	
	public class ContentEndpoints {
		public static string contentVerification = "http://dlc.supasupagames.com/api/v1/en/file/{0}/{1}/{2}/{3}"; // 0 = game, version, platform, pack
		
		public static string contentDownloadPrimary = "http://s3.amazonaws.com/static/{0}/{1}/{2}/{3}";
		public static string contentDownloadAmazon = "http://s3.amazonaws.com/{0}/{1}/{2}/{3}";
		public static string contentDownloadSupaSupaGames = "http://dlc.supasupagames.com/api/v1/en/file/{0}/{1}/{2}/{3}"; // 0 = game, version, platform, pack;
	}
	
	public class ContentItem {
		public string uid = "";
		public string name = "";
		public int version = 0;
		public AssetBundle bundle;
	}
	
	public class ContentItemError {
		public string uid = "";
		public string name = "";
		public string message = "";
		public ContentItem contentItem;
	}
	
	public class Contents {
		
		private static volatile Contents instance;
		private static System.Object syncRoot = new System.Object();
		
		public static Contents Instance
		{
		  get 
		  {
		     if (instance == null) 
		     {
		        lock (syncRoot) 
		        {
		           if (instance == null) 
		              instance = new Contents();
		        }
		     }
		
		     return instance;
		  }
		}
		
		//public static string contentUrlRoot = "http://dlc.supasupagames.com/";
		public string contentUrlRoot = "http://dlc.supasupagames.com/";
		
		public List<ContentItem> contentItemList = new List<ContentItem>();
		public ContentItem currentContentItem = new ContentItem();
		
		public AssetBundle bundle;
		
		public WWW downloader;
		public WWW verifier;
		
		public DownloadableContentItem dlcItem;
		public ContentItemStatus contentItemStatus;
		public bool downloadInProgress = false;
		
		public ContentItemAccessDictionary contentItemAccess;
		
		public Contents() {
			contentItemAccess = new ContentItemAccessDictionary();		
			contentItemAccess.Load();
		}
	
		public bool CheckGlobalContentAccess(string pack) {
			if(contentItemAccess.CheckAccess(pack)) {
				return true;	
			}
			return false;
		}
		
		public void SaveGlobalContentAccess() {
			contentItemAccess.Save();
		}
		
		public void SetGlobalContentAccess(string pack) {
			contentItemAccess.SetContentAccess(pack);
			contentItemAccess.SetContentAccess(pack.Replace("-", "_"));
			contentItemAccess.SetContentAccess(pack.Replace("_", "-"));
			
			LogUtil.Log("GameStore::SetContentAccessPermissions pack :" + pack);
			LogUtil.Log("GameStore::SetContentAccessPermissions pack _ :" + pack.Replace("-", "_"));
			LogUtil.Log("GameStore::SetContentAccessPermissions pack - :" + pack.Replace("_", "-"));
			contentItemAccess.Save();
		}	
			
		void HandleDownloadableContentInfoCallback(WebRequest.ResponseObject response) {
			
			response = HandleResponseObject(response);
			
			bool serverError = false;
			
			if(response.validResponse) {
				
				LogUtil.Log("SUCCESSFUL DOWNLOAD");						
				
				string dataToParse = response.data;
							
				LogUtil.Log("dataToParse:" + dataToParse);
							
				if(!string.IsNullOrEmpty(dataToParse)) {	
					
					try {
						DownloadableContentItemResponse responseData = JsonMapper.ToObject<DownloadableContentItemResponse>(dataToParse);
						dlcItem = responseData.data;
					}
					catch(Exception e) {							
						serverError = true;
						LogUtil.Log("Parsing error:" + e.Message + e.StackTrace + e.Source);
					}				
					
					if(dlcItem != null) {
						
						List<string> downloadUrls = dlcItem.download_urls;
						
						foreach(string url in downloadUrls) {
							MessengerString<string>.Broadcast(ContentMessages.ContentItemVerifySuccess, "Content verified, downloading and loading pack." );
							CoroutineUtil.Start(Contents.Instance.SceneLoadFromCacheOrDownloadCo(url));
							break;
						}					
					}
					else {			
						serverError = true;
					}
				}
				else {			
					serverError = true;
				}
			}
			else {
				// There was a problem with the response.
				LogUtil.Log("NON-SUCCESSFUL DOWNLOAD");			
				serverError = true;
			}
			
			if(serverError) {
				Reset();
				MessengerString<string>.Broadcast(ContentMessages.ContentItemVerifyError, "Error on server, please try again.");
			}
		}
		
		public WebRequest.ResponseObject HandleResponseObject(WebRequest.ResponseObject responseObject) {		
			
			bool serverError = false;
			
			// Manages common response object parsing to get to object
			if(responseObject.www.text != null) {
				
				JsonData data = JsonMapper.ToObject(responseObject.www.text);
	
				if(data.IsObject) {
					
					string code = (string)data["code"];
					string message = (string)data["message"];
					/*
					JsonData dataValue = null;
					if(data["data"] != null) {
						if(data["data"].IsObject) {
							dataValue = data["data"];
						}
					}
					try{
						responseObject.error = Convert.ToInt32(code);
					}
					catch(Exception e) {
						responseObject.error = 1;
						LogUtil.Log("ERROR: " + e.Message + e.StackTrace + e.Source);
					}
					*/
					responseObject.message = message;
					responseObject.code = code;
	
					LogUtil.Log("STATUS/CODE:" + code);
					LogUtil.Log("STATUS/CODE MESSAGE:" + message);
	
					if(code == "0") {
						LogUtil.Log("STATUS/DATA NODE:" + data);
						
						LogUtil.Log("dataValue:" + data["data"]);
						LogUtil.Log("responseObject.www.text:" + responseObject.www.text);
						
						responseObject.data = responseObject.www.text;
						responseObject.dataValue = data["data"]; 
						responseObject.validResponse = true;
					}
					else {
						LogUtil.Log("ERROR - Good response but problem with data, see message.");
						serverError = true;
					}
				}
			}
			else {
				LogUtil.Log("ERROR - NO DATA");
				serverError = true;
			}
			
			if(serverError) {
				responseObject.validResponse = false;
				Reset();			
				MessengerString<string>.Broadcast(ContentMessages.ContentItemVerifyError, "Error receiving a server response, please try again." );		
			}
	
			return responseObject;
		}
		
		public void RequestDownloadableContent(string pack) {
			RequestDownloadableContent(
				"",
				"", 
				"",
				pack);
		}
		
		public void RequestDownloadableContent(string game, string version, string platform, string pack) {
			//glob.ShowLoadingIndicator();
			
			Dictionary<string, string> data = new Dictionary<string, string>();
			string udid = SystemInfo.deviceUniqueIdentifier;
	
			data.Add("device_id", udid);
			data.Add("ssg_app_id", "ssg_ssc_1_1");
			
			downloadInProgress = true;
			
			string url = String.Format(ContentEndpoints.contentDownloadSupaSupaGames, game, version, platform, pack);
			WebRequest.Instance.Request(WebRequest.RequestType.HTTP_POST, url, data, HandleDownloadableContentInfoCallback);
			contentItemStatus = new ContentItemStatus();
			
			MessengerString<string>.Broadcast(ContentMessages.ContentItemVerifyStarted, "Verifying content access..." );
		}
		
		public string GetDownloadContentItemUrl(string game, string buildVersion, string platform, string pack) {
			return String.Format(ContentEndpoints.contentDownloadSupaSupaGames, game, buildVersion, platform, pack);
		}
		
		public void LoadSceneOrDownloadScenePackAndLoad(string pack) {
			LoadSceneOrDownloadScenePackAndLoad(
				"",
				"", 
				"",
				pack);
		}
		
		public void LoadSceneOrDownloadScenePackAndLoad(string game, string buildVersion, string platform, string pack) {
			
			bool isDownloadableContent = IsDownloadableContent(pack);
			LogUtil.Log("isDownloadableContent:" + isDownloadableContent);
			int version = 1;
			
			string url = GetDownloadContentItemUrl(game, buildVersion, platform, pack);
		
			string lastPackUrlKey = "last-pack-" + pack;
			string lastPackUrlValue = url;
			
			if(SystemPrefUtil.HasLocalSetting(lastPackUrlKey)) {
				lastPackUrlValue = SystemPrefUtil.GetLocalSettingString(lastPackUrlKey);
			}
			
			if(Caching.IsVersionCached(lastPackUrlValue, version)) {
				// Just load from the saved url
				CoroutineUtil.Start(SceneLoadFromCacheOrDownloadCo(lastPackUrlValue));			
			}
			else {
				// Do download verification and download
				RequestDownloadableContent(game, buildVersion, platform, pack);
			}
		}
		
		public bool IsDownloadableContent(string pack) {
			//if(pack.ToLower() == GamePacks.PACK_SX_2012_1.ToLower()
			//	|| pack.ToLower() == GamePacks.PACK_SX_2012_2.ToLower()) {
			//	return true;
			//}
			return false;
		}	
			
		
		public IEnumerator SceneLoadFromCacheOrDownloadCo(string url) {
					
			int version = 1;
			string packName = "";
			string sceneName = "";
				
			LogUtil.Log("SceneLoadFromCacheOrDownloadCo: packName:" + packName);
			LogUtil.Log("SceneLoadFromCacheOrDownloadCo: sceneName:" + sceneName);
			LogUtil.Log("SceneLoadFromCacheOrDownloadCo: version:" + version);
			
			ContentItem contentItem = new ContentItem();
			
			contentItem.uid = sceneName; // hash this
			contentItem.name = sceneName;
			contentItem.version = version;
			
			//bool isDlc = false;
			bool ready = true;
			
			//if(packName.ToLower() == GamePacks.PACK_SX_2012_1
			//	|| packName.ToLower() == GamePacks.PACK_SX_2012_2) {
				
				LogUtil.Log("SceneLoadFromCacheOrDownloadCo: " + packName);
						    
				LogUtil.Log("SceneLoadFromCacheOrDownloadCo: " + url);
				
				MessengerString<string>.Broadcast(ContentMessages.ContentItemDownloadStarted, url);
				
				downloadInProgress = true;
				
				downloader = WWW.LoadFromCacheOrDownload(url, version);
				
				
				LogUtil.Log("downloader.progress: " + downloader.progress);
					    
				yield return downloader;
				
				LogUtil.Log("downloader.progress2: " + downloader.progress);
			    
			    // Handle error
			    if (downloader.error != null) {			
					LogUtil.LogError("Error downloading");
					LogUtil.LogError(downloader.error);
					LogUtil.LogError(url);
					ready = false;
					Reset();
					MessengerString<string>.Broadcast(ContentMessages.ContentItemDownloadError, downloader.error);
			    }
				else {
			    
				    // In order to make the scene available from LoadLevel, we have to load the asset bundle.
				    // The AssetBundle class also lets you force unload all assets and file storage once it 
					// is no longer needed.
					
					UnloadLevelBundle();
					
				    bundle = downloader.assetBundle;
					
					LogUtil.Log("LoadLevel" + sceneName);
					
				    // Load the level we have just downloaded
				}
			//}
			
			if(ready) {
				LoadLevelHandler();
			}
			else {
				// Show download error...
				MessengerString<string>.Broadcast(ContentMessages.ContentItemDownloadError, "Error unloading pack, please try again.");
				Reset();
			}
		}
		
		public void LoadLevelHandler() {
			
			string sceneName = "";//GameLevels.Current.name;
			Application.LoadLevel (sceneName);
				
			//if(GameLoadingObject.Instance != null) {
			//	GameLoadingObject.Instance.HideLoadingHelp();
			//	GameLoadingObject.Instance.ShowReadyDelayed();
			//}
			
			LogUtil.Log("LoadLevelOrDownload: Complete");
			Reset();
			MessengerString<string>.Broadcast("","");//UIMessages.EventLevelLoaded, GameLevels.Current.name);
		}
		
		public void UnloadLevelBundle(bool unloadAll) {		
			if(bundle != null) {
				bundle.Unload(unloadAll);
			}
		}
		
		public void UnloadLevelBundle() {		
			UnloadLevelBundle(true);
		}
		
		public void Reset() {
			downloader = null;
			contentItemStatus = new ContentItemStatus();
			downloadInProgress = false;
		}
		
		public ContentItemStatus ProgressStatus() {
						
			if(downloader != null && downloadInProgress) {
				if(downloader.isDone) {
					contentItemStatus.downloaded = true;
				}		
				
				LogUtil.Log("progress:" + downloader.progress);
				
				contentItemStatus.itemProgress = downloader.progress;
				contentItemStatus.url = downloader.url;
			}
			
			return contentItemStatus;
		}
		
		
		/*
		public static IEnumerator SceneLoadFromCacheOrDownloadCo(string packName, string sceneName) {
			
			int version = GamePacks.currentPacksIncrement;
			string url = "https://s3.amazonaws.com/game-supasupacross/1.1/ios/" + packName + ".unity3d";
		    
			LogUtil.Log("SceneLoadFromCacheOrDownloadCo: " + url);
			
			var downloadProgress = WWW.LoadFromCacheOrDownload(url, version);
			
		    
			yield return downloadProgress;
		    
		    // Handle error
		    if (downloadProgress.error != null)
		    {
				//Messenger<ContentItemError>.Broadcast(ContentMessages.ContentItemDownloadError, 
				                                      //contentItemError);
				
				LogUtil.LogError("Error downloading");
		    }
			else {
		    
			    // In order to make the scene available from LoadLevel, we have to load the asset bundle.
			    // The AssetBundle class also lets you force unload all assets and file storage once it 
				// is no longer needed.
			    Contents.lastBundle = downloadProgress.assetBundle;
				//contentItem.bundle = bundle;
				
				//contentItemList.Add(contentItem);
			    
			    // Load the level we have just downloaded
			    Application.LoadLevel (sceneName);
				
			}
		}
			
			
			
		public IEnumerator PrepareSceneLoadFromCacheOrDownloadCoroutine(string sceneName, int version) {
			
			ContentItem contentItem = new ContentItem();
			
			contentItem.uid = sceneName; // hash this
			contentItem.name = sceneName;
			contentItem.version = version;
	
		    var downloadProgress = WWW.LoadFromCacheOrDownload("Streamed-" + sceneName + ".unity3d", version);
		    
			yield return downloadProgress;
		    
		    // Handle error
		    if (downloadProgress.error != null)
		    {
				ContentItemError contentItemError = new ContentItemError();
				contentItemError.contentItem = contentItem;
				contentItemError.name = sceneName;
				contentItemError.message = downloadProgress.error;
				Messenger<ContentItemError>.Broadcast(ContentMessages.ContentItemDownloadError, 
				                                      contentItemError);
		    }
			else {
		    
			    // In order to make the scene available from LoadLevel, we have to load the asset bundle.
			    // The AssetBundle class also lets you force unload all assets and file storage once it 
				// is no longer needed.
			    AssetBundle bundle = downloadProgress.assetBundle;
				contentItem.bundle = bundle;
				
				contentItemList.Add(contentItem);
			    
			    // Load the level we have just downloaded
			    //Application.LoadLevel ("Level1");
			}
		}
		*/
	}
		
}
