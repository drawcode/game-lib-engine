using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
#if !UNITY_WEBPLAYER
using System.IO;
#endif
using UnityEngine;

using Engine.Events;
// using Engine.Data.Json;
using Engine.Networking;

public class ContentConfig {

    public static string contentCacheAssetBundles = "bundles";
    public static string contentCacheVersion = "version";
    public static string contentCacheData = "data";
    public static string contentCacheUserData = "userdata";
    public static string contentCacheShared = "shared";
    public static string contentCacheAll = "all";
    public static string contentCacheTrackers = "trackers";
    public static string contentCacheScenes = "scenes";
    public static string contentCachePacks = "packs";
    public static string currentContentPackCode = "default";
    public static string currentContentContent = "content";

}

public class ContentProgressItemStatus {

    public Dictionary<string, List<string>> contentItems = new Dictionary<string, List<string>>();
    public string contentMessage = "";
}

public class ContentItemStatus {
    public double itemSize = 0;
    public double itemProgress = 0;
    public string url = "";

    public bool downloaded = false;

    public double percentageCompleted {
        get {
            return itemProgress;
        }
    }

    public bool completed {
        get {
            return percentageCompleted == 1 ? true : false;
        }
    }
}

public class ContentItemAccess : DataObjectItem {
    public bool globalItem = true;
    public string code = "";
    public string profileId = "";
    public string receipt = "";
    public string platform = "ios-storekit";
    public int quantity = 1;
    public string productCode = "";
}

public class ContentItemAccessDictionary : DataObjectItem {
    public Dictionary<string, ContentItemAccess> accessItems = new Dictionary<string, ContentItemAccess>();

    public void CheckDictionary() {
        if (accessItems == null)
            accessItems = new Dictionary<string, ContentItemAccess>();
    }

    public bool CheckAccess(string key) {
        CheckDictionary();
        bool hasAccess = accessItems.ContainsKey(key);
        if (key.ToLower() == "default") {
            //|| !GameProducts.enableProductLocks) {
            hasAccess = true;
        }
        LogUtil.LogAccess("CheckAccess:: key: " + key + " hasAccess: " + hasAccess);
        return hasAccess;
    }

    public ContentItemAccess GetContentAccess(string key) {
        CheckDictionary();
        if (CheckAccess(key)) {
            if (accessItems != null) {
                if (accessItems.ContainsKey(key)) {
                    return accessItems[key];
                }
            }
        }
        return null;
    }

    public void SetContentAccess(string key) {
        CheckDictionary();
        ContentItemAccess itemAccess;

        if (CheckAccess(key) && accessItems.ContainsKey(key)) {
            itemAccess = GetContentAccess(key);
            itemAccess.code = key;
            itemAccess.globalItem = true;
            itemAccess.platform = "";//GamePacks.currentPacksPlatform;
            itemAccess.productCode = key;
            itemAccess.profileId = "";//GameProfiles.Current.username;
            itemAccess.quantity = 1;
            itemAccess.receipt = "";
            SetContentAccess(itemAccess);
        }
        else {
            itemAccess = new ContentItemAccess();
            itemAccess.code = key;
            itemAccess.globalItem = true;
            itemAccess.platform = "";//GamePacks.currentPacksPlatform;
            itemAccess.productCode = key;
            itemAccess.profileId = "";//GameProfiles.Current.username;
            itemAccess.quantity = 1;
            itemAccess.receipt = "";
            SetContentAccess(itemAccess);
        }
    }

    public void SetContentAccess(ContentItemAccess itemAccess) {

        CheckDictionary();

        if (CheckAccess(itemAccess.code)) {
            accessItems[itemAccess.code] = itemAccess;
        }
        else {
            accessItems.Add(itemAccess.code, itemAccess);
        }
    }

    public void SetContentAccessTransaction(string key, string productId, string receipt, int quantity, bool save) {
        ContentItemAccess itemAccess = GetContentAccess(key);
        if (itemAccess != null) {
            itemAccess.receipt = receipt;
            itemAccess.productCode = productId;
            itemAccess.quantity = quantity;
            SetContentAccess(itemAccess);
            if (save) {
                Save();
            }
        }
    }

    public void Save() {
        CheckDictionary();
        string contentItemAccessString = "";
        string settingKey = "ssg-cal";
        contentItemAccessString = accessItems.ToJson();
        LogUtil.LogAccess("Save: access:" + contentItemAccessString);
        SystemPrefUtil.SetLocalSettingString(settingKey, contentItemAccessString);
        SystemPrefUtil.Save();
    }

    public void Load() {
        string settingKey = "ssg-cal";
        if (SystemPrefUtil.HasLocalSetting(settingKey)) {
            // Load from persistence
            string keyValue = SystemPrefUtil.GetLocalSettingString(settingKey);
            LogUtil.LogAccess("Load: access:" + keyValue);
            accessItems = keyValue.FromJson<Dictionary<string, ContentItemAccess>>();
            CheckDictionary();
        }
    }

}

// RESPONSES

//{"info": "", "status": "", "error": 0, "action": "sx-2012-pack-1", "message": "Success!", "data": 
// {"download_urls": ["https://s3.amazonaws.com/game-ssc/1.1/ios/sx-2012-pack-1.unity3d?Signature=rJ%2Fe863up9wgAutleNY%2F%2B7OSy%2BU%3D&Expires=1332496714&AWSAccessKeyId=0YAPDVPCN85QV96YR382"], "access_allowed": true, "date_modified": "2012-03-21T10:58:34.919000", "udid": "[udid]", "tags": ["test", "fun"], "content": "this is \"real\"...", "url": "ffff", "version": "1.1", "increment": 1, "active": true, "date_created": "2012-03-21T10:58:34.919000", "type": "application/octet-stream"}}

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

public class DownloadableContentItemList {
    public Dictionary<string, DownloadableContentUrlObject> url_objs
        = new Dictionary<string, DownloadableContentUrlObject>();
}

public class DownloadableContentUrlObject {
    public string file_key = ""; // hashed url part
    public string url = ""; // amazon url
    public string path = ""; // path for lookup in local content list
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

public class DownloadableContentItemListResponse : BaseObjectResponse {
    public DownloadableContentItemList data
        = new DownloadableContentItemList();

    public DownloadableContentItemListResponse() {
        Reset();
    }

    public override void Reset() {
        base.Reset();
        data
            = new DownloadableContentItemList();
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

// CONTENT SYSTEM

//"info": "ssg_ssc_1_1", "status": "", "code": "0", "action": "pack-1", "message": "Success!", "data": {"download_urls": ["http://s3.amazonaws.com/game-[app]/1.1/ios/sx-2012-pack-1.unity3d?Signature=9VJYzvaLZjeVcakz4DBDDg51Fwo%3D&Expires=1332704684&AWSAccessKeyId=0YAPDVPCN85QV96YR382"]}

public class ContentMessages {
    public static string ContentFileDownloadSuccess = "content-file-download-success";
    public static string ContentFileDownloadError = "content-file-download-error";
    public static string ContentFileDownloadStarted = "content-file-download-started";



    public static string ContentSetSuccess = "content-set-success";
    public static string ContentSetError = "content-set-error";
    public static string ContentSetStarted = "content-set-started";

    public static string ContentSetDownloadSuccess = "content-set-download-success";
    public static string ContentSetDownloadError = "content-set-download-error";
    public static string ContentSetDownloadStarted = "content-set-download-started";

    public static string ContentItemDownloadSuccess = "content-item-download-success";
    public static string ContentItemDownloadError = "content-item-download-error";
    public static string ContentItemDownloadStarted = "content-item-download-started";

    public static string ContentItemVerifySuccess = "content-item-verify-success";
    public static string ContentItemVerifyError = "content-item-verify-error";
    public static string ContentItemVerifyStarted = "content-item-verify-started";

    public static string ContentItemPrepareSuccess = "content-item-prepare-success";
    public static string ContentItemPrepareError = "content-item-prepare-error";
    public static string ContentItemPrepareStarted = "content-item-prepare-started";

    public static string ContentItemLoadSuccess = "content-item-load-success";
    public static string ContentItemLoadError = "content-item-load-error";
    public static string ContentItemLoadStarted = "content-item-load-started";

    public static string ContentItemProgress = "content-item-progress";
    public static string ContentProgressStatus = "content-progress-status";
    public static string ContentProgressMessage = "content-progress-message";

    public static string ContentSyncShipContentSuccess = "content-sync-ship-content-success";
    public static string ContentSyncShipContentError = "content-sync-ship-content-error";
    public static string ContentSyncShipContentStarted = "content-sync-ship-content-started";

    public static string ContentSyncInitialSuccess = "content-sync-initial-success";
    public static string ContentSyncInitialError = "content-sync-initial-error";
    public static string ContentSyncInitialStarted = "content-sync-initial-started";

    public static string ContentSyncPackSuccess = "content-sync-pack-success";
    public static string ContentSyncPackError = "content-sync-pack-error";
    public static string ContentSyncPackStarted = "content-sync-pack-started";

    public static string ContentSyncFullSuccess = "content-sync-full-success";
    public static string ContentSyncFullError = "content-sync-full-error";
    public static string ContentSyncFullStarted = "content-sync-full-started";
    public static string ContentSyncFullPrepare = "content-sync-full-prepare";

    // download of initial app-content-list-item files for gatherning new files.
    public static string ContentAppContentListSyncSuccess = "content-app-content-list-sync-success";
    public static string ContentAppContentListSyncError = "content-app-content-list-sync-error";
    public static string ContentAppContentListSyncStarted = "content-app-content-list-sync-started";

    // download of content list ofr verified files from passed in app-content-list-item paths (replacing /version/ with correct version).
    public static string ContentAppContentListFileDownloadSuccess = "content-app-content-list-file-download-success";
    public static string ContentAppContentListFileDownloadError = "content-app-content-list-file-download-error";
    public static string ContentAppContentListFileDownloadStarted = "content-app-content-list-file-download-started";

    // download of content list ofr verified files from passed in app-content-list-item paths (replacing /version/ with correct version).
    public static string ContentAppContentListFilesSuccess = "content-app-content-list-files-success";
    public static string ContentAppContentListFilesError = "content-app-content-list-files-error";
    public static string ContentAppContentListFilesStarted = "content-app-content-list-files-started";

    // download of actual files from download list completed.
    public static string ContentAppContentListFilesDownloadSuccess = "content-app-content-list-files-download-success";
    public static string ContentAppContentListFilesDownloadError = "content-app-content-list-files-download-error";
    public static string ContentAppContentListFilesDownloadStarted = "content-app-content-list-files-download-started";

}


public class ContentEndpoints {
    public static string contentVerification = ContentsConfig.contentEndpoint + "api/v1/en/file/{0}/{1}/{2}/{3}"; // 0 = game, version, platform, pack

    public static string contentDownloadPrimary = "http://s3.amazonaws.com/static/{0}/{1}/{2}/{3}";
    public static string contentDownloadAmazon = "http://s3.amazonaws.com/{0}/{1}/{2}/{3}";
    public static string contentDownloadFileAsset = ContentsConfig.contentEndpoint + "api/v1/en/file/{0}/{1}/{2}/{3}"; // 0 = game, version, platform, pack;
    public static string contentSyncContentSet = ContentsConfig.contentEndpoint + "api/v1/sync/en/content-list/{0}/{1}/{2}/{3}"; // 0 = game, version, platform, pack;
    public static string contentDownloadAppContentListFiles = ContentsConfig.contentEndpoint + "api/v1/en/app-content-list/file/{0}/{1}/{2}/"; // 0 = game, version, platform, pack;
                                                                                                                                               //http://content1.host.com/api/v1/en/app-content-list/file/app-viewer/1.0/ios/?paths=1.0/data/app-content-list-item-data-1-0-5.json&app_id=85366ecb7429c19839e6900a1cfcedc18342f775

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

public enum ContentSyncState {
    SyncNotStarted,
    SyncStarted,
    SyncCompleted,
    SyncPrepare,
    SyncError,
    SyncProcessShipFiles,
    SyncProcessContentList,
    SyncProcessDownloadFiles,
    SyncProcessVerify,
    SyncDefaultStarted,
    SyncDefaultCompleted,
    SyncPackStarted,
    SyncPackCompleted
}

public enum ContentSyncDisplayState {
    SyncNotStarted = 0,
    SyncPreparing = 1,
    SyncShip = 2,
    SyncContentListsDefault = 3,
    SyncContentListsDefaultDownload = 4,
    SyncContentListsPack = 5,
    SyncContentListsPackDownload = 6,
    SyncContentListsValidation = 7,
    SynContentListsCompleted = 8,
    StateCount = 9
}

public enum ContentVersionSyncEnum {
    NonVersioned, // plain file
    Versioned, // just version and increment
    VersionedSync // hashed with checksum
}

public class ContentPaths {

    public static string persistenceFolder = "";
    public static string streamingAssetsFolder = "";

    public static string appCachePath = "";
    public static string appCachePathAssetBundles = "";
    public static string appCachePathTrackers = "";
    public static string appCachePathPacks = "";
    public static string appCachePathShared = "";
    public static string appCachePathAll = "";
    public static string appCachePathAllShared = "";
    public static string appCachePathAllSharedUserData = "";
    public static string appCachePathSharedPacks = "";
    public static string appCachePathSharedTrackers = "";
    public static string appCachePathAllSharedTrackers = "";
    public static string appCachePathAllPlatform = "";
    public static string appCachePathAllPlatformPacks = "";
    public static string appCachePathAllPlatformData = "";
    public static string appCachePathData = "";
    public static string appCacheVersionPath = "";
    public static string appCachePlatformPath = "";

    public static string appShipCachePath = "";
    public static string appShipCachePathAssetBundles = "";
    public static string appShipCachePathTrackers = "";
    public static string appShipCachePathPacks = "";
    public static string appShipCachePathShared = "";
    public static string appShipCachePathAll = "";
    public static string appShipCachePathData = "";
    public static string appShipCacheVersionPath = "";
    public static string appShipCachePlatformPath = "";
    public static string appShipCachePathPlatformPath = "";

    public static string currentPlatformCode = "ios";

    public static List<string> packPaths = new List<string>();
    public static List<string> packPathsVersionedShared = new List<string>();
    public static List<string> packPathsVersioned = new List<string>();

    public static string appCacheVersion {
        get {
            return ContentsConfig.contentRootFolder + "/"
                + ContentsConfig.contentAppFolder + "/version/";
        }
    }
    public static string appCacheVersionShared {
        get {
            return appCacheVersion + "shared/";
        }
    }

    public static string appCacheVersionSharedAudio {
        get {
            return appCacheVersionShared + "audio/";
        }
    }

    public static string appCacheVersionSharedPrefab {
        get {
            return appCacheVersionShared + "prefab/";
        }
    }

    public static string appCacheVersionSharedMaterials {
        get {
            return appCacheVersionShared + "materials/";
        }
    }

    public static string appCacheVersionSharedPrefabCharacters {
        get {
            return appCacheVersionSharedPrefab + "characters/";
        }
    }

    public static string appCacheVersionSharedPrefabLevelAssets {
        get {
            return appCacheVersionSharedPrefab + "levelassets/";
        }
    }

    public static string appCacheVersionSharedPrefabLevelItems {
        get {
            return appCacheVersionSharedPrefab + "items/";
        }
    }

    public static string appCacheVersionSharedPrefabNetwork {
        get {
            return appCacheVersionSharedPrefab + "network/";
        }
    }

    public static string appCacheVersionSharedPrefabLevelUI {
        get {
            return appCacheVersionSharedPrefab + "ui/";
        }
    }

    public static string appCacheVersionSharedPrefabWeapons {
        get {
            return appCacheVersionSharedPrefab + "weapons/";
        }
    }

    public static string appCacheVersionSharedPrefabEffects {
        get {
            return appCacheVersionSharedPrefab + "effects/";
        }
    }

    public static string appCacheVersionSharedPrefabWorlds {
        get {
            return appCacheVersionSharedPrefab + "worlds/";
        }
    }

    public static string appCacheVersionSharedPrefabVehicles {
        get {
            return appCacheVersionSharedPrefab + "vehicles/";
        }
    }
    
    public static string GetCurrentPlatformCode() {

#if UNITY_IPHONE
        return "ios";
#elif UNITY_ANDROID
        return "android";
#else
        return "desktop";
#endif
    }

    public static void ResetPaths() {
        persistenceFolder = "";
        streamingAssetsFolder = "";

        appCachePath = "";
        appCachePathAssetBundles = "";
        appCachePathTrackers = "";
        appCachePathPacks = "";
        appCachePathShared = "";
        appCachePathAll = "";
        appCachePathAllShared = "";
        appCachePathAllSharedUserData = "";
        appCachePathSharedPacks = "";
        appCachePathSharedTrackers = "";
        appCachePathAllSharedTrackers = "";
        appCachePathAllPlatform = "";
        appCachePathAllPlatformPacks = "";
        appCachePathAllPlatformData = "";
        appCachePathData = "";
        appCacheVersionPath = "";
        appCachePlatformPath = "";

        appShipCachePath = "";
        appShipCachePathAssetBundles = "";
        appShipCachePathTrackers = "";
        appShipCachePathPacks = "";
        appShipCachePathShared = "";
        appShipCachePathAll = "";
        appShipCachePathData = "";
        appShipCacheVersionPath = "";
        appShipCachePlatformPath = "";
        appShipCachePathPlatformPath = "";

        currentPlatformCode = GetCurrentPlatformCode();

        packPaths = new List<string>();
        packPathsVersioned = new List<string>();
        packPathsVersionedShared = new List<string>();

    }

    public static bool pathsCreated {
        get {
            if (string.IsNullOrEmpty(appCachePath)) {
                return false;
            }
            return true;
        }
    }

    public static bool CheckPathsCreated(bool create) {
        if (!pathsCreated) {
            if (create) {
                CreateCachePaths();
            }
            else {
                return false;
            }
        }
        return true;
    }


    public static void CreateCachePaths() {

        if (CheckPathsCreated(false)) {
            return;
        }

        ContentPaths.persistenceFolder = PathUtil.AppPersistencePath;
        ContentPaths.streamingAssetsFolder = Application.streamingAssetsPath;

        LogUtil.Log("Contents::persistenceFolder: " + ContentPaths.persistenceFolder);
        LogUtil.Log("Contents::streamingAssetsFolder: " + ContentPaths.streamingAssetsFolder);

        string pathRoot = PathUtil.Combine(ContentPaths.persistenceFolder, ContentsConfig.contentRootFolder);
        string pathShipRoot = PathUtil.Combine(ContentPaths.streamingAssetsFolder, ContentsConfig.contentRootFolder);

        FileSystemUtil.EnsureDirectory(pathRoot, false);
        FileSystemUtil.EnsureDirectory(pathShipRoot, false);

        string pathRootAppend = PathUtil.Combine(pathRoot, ContentsConfig.contentAppFolder);
        string pathShipRootAppend = PathUtil.Combine(pathShipRoot, ContentsConfig.contentAppFolder);

        FileSystemUtil.EnsureDirectory(pathRootAppend, false);
        FileSystemUtil.EnsureDirectory(pathShipRootAppend, false);

        ContentPaths.appCachePath = pathRootAppend;
        ContentPaths.appShipCachePath = pathShipRootAppend;

        FileSystemUtil.EnsureDirectory(ContentPaths.appCachePath, false);
        FileSystemUtil.EnsureDirectory(ContentPaths.appShipCachePath, false);

        ContentPaths.appCacheVersionPath = PathUtil.Combine(ContentPaths.appCachePath, ContentsConfig.contentVersion);
        ContentPaths.appShipCacheVersionPath = PathUtil.Combine(ContentPaths.appShipCachePath, ContentConfig.contentCacheVersion);

        FileSystemUtil.EnsureDirectory(ContentPaths.appCacheVersionPath, false);
        FileSystemUtil.EnsureDirectory(ContentPaths.appShipCacheVersionPath, false);

        ContentPaths.appCachePathAll = PathUtil.Combine(ContentPaths.appCachePath, ContentConfig.contentCacheAll);
        ContentPaths.appShipCachePathAll = PathUtil.Combine(ContentPaths.appShipCachePath, ContentConfig.contentCacheAll);
        ContentPaths.appCachePathAllShared = PathUtil.Combine(ContentPaths.appCachePathAll, ContentConfig.contentCacheShared);
        ContentPaths.appCachePathAllSharedTrackers = PathUtil.Combine(ContentPaths.appCachePathAllShared, ContentConfig.contentCacheTrackers);
        ContentPaths.appCachePathAllSharedUserData = PathUtil.Combine(ContentPaths.appCachePathAllShared, ContentConfig.contentCacheUserData);

        FileSystemUtil.EnsureDirectory(ContentPaths.appCachePathAll, false);
        FileSystemUtil.EnsureDirectory(ContentPaths.appShipCachePathAll, false);
        FileSystemUtil.EnsureDirectory(ContentPaths.appCachePathAllShared, false);
        FileSystemUtil.EnsureDirectory(ContentPaths.appCachePathAllSharedTrackers, false);
        FileSystemUtil.EnsureDirectory(ContentPaths.appCachePathAllSharedUserData, false);

        ContentPaths.appCachePathAllPlatform = PathUtil.Combine(ContentPaths.appCachePathAll, GetCurrentPlatformCode());
        ContentPaths.appCachePathAllPlatformPacks = PathUtil.Combine(ContentPaths.appCachePathAllPlatform, ContentConfig.contentCachePacks);
        ContentPaths.appCachePathAllPlatformData = PathUtil.Combine(ContentPaths.appCachePathAllPlatform, ContentConfig.contentCacheData);

        FileSystemUtil.EnsureDirectory(ContentPaths.appCachePathAllPlatform, false);
        FileSystemUtil.EnsureDirectory(ContentPaths.appCachePathAllPlatformPacks, false);
        FileSystemUtil.EnsureDirectory(ContentPaths.appCachePathAllPlatformData, false);

        ContentPaths.appCachePlatformPath = PathUtil.Combine(ContentPaths.appCacheVersionPath, GetCurrentPlatformCode());
        ContentPaths.appShipCachePlatformPath = PathUtil.Combine(ContentPaths.appShipCacheVersionPath, GetCurrentPlatformCode());

        FileSystemUtil.EnsureDirectory(ContentPaths.appCachePlatformPath, false);
        FileSystemUtil.EnsureDirectory(ContentPaths.appShipCachePlatformPath, false);

        appCachePathData = PathUtil.Combine(
            appCacheVersionPath, ContentConfig.contentCacheData);

        appCachePathShared = PathUtil.Combine(
            appCacheVersionPath, ContentConfig.contentCacheShared);

        appCachePathSharedPacks = PathUtil.Combine(
            appCachePathShared, ContentConfig.contentCachePacks);

        appCachePathSharedTrackers = PathUtil.Combine(
            appCachePathShared, ContentConfig.contentCacheTrackers);

        appCachePathPacks = PathUtil.Combine(
            appCachePlatformPath, ContentConfig.contentCachePacks);

        FileSystemUtil.EnsureDirectory(ContentPaths.appCachePathData, false);
        FileSystemUtil.EnsureDirectory(ContentPaths.appCachePathShared, false);
        FileSystemUtil.EnsureDirectory(ContentPaths.appCachePathSharedPacks, false);
        FileSystemUtil.EnsureDirectory(ContentPaths.appCachePathSharedTrackers, false);
        FileSystemUtil.EnsureDirectory(ContentPaths.appCachePathPacks, false);

        ContentPaths.appShipCachePathData = PathUtil.Combine(
            ContentPaths.appShipCacheVersionPath, ContentConfig.contentCacheData);

        ContentPaths.appShipCachePathShared = PathUtil.Combine(
            ContentPaths.appShipCacheVersionPath, ContentConfig.contentCacheShared);

        FileSystemUtil.EnsureDirectory(ContentPaths.appShipCachePathData, false);
        FileSystemUtil.EnsureDirectory(ContentPaths.appShipCachePathShared, false);

    }

    public static List<string> GetPackPathsNonVersioned() {
        LoadPackPaths();
        return ContentPaths.packPaths;
    }

    public static List<string> GetPackPathsVersioned() {
        LoadPackPaths();
        return ContentPaths.packPathsVersioned;
    }

    public static List<string> GetPackPathsVersionedShared() {
        LoadPackPaths();
        return ContentPaths.packPathsVersionedShared;
    }

    public static void LoadPackPaths() {

        if (string.IsNullOrEmpty(ContentPaths.appCachePath)) {
            CreateCachePaths();
        }

        ////LogUtil.Log("LoadPackPaths:appCachePathPacks:" + appCachePathPacks);
        ////LogUtil.Log("LoadPackPaths:appCachePathAllPlatformPacks:" + appCachePathAllPlatformPacks);

        if (ContentPaths.packPaths.Count == 0) {
            //LogUtil.Log("Loading packPathsNONVersioned: " + appCachePathPacks);

            if (!string.IsNullOrEmpty(appCachePathPacks)) {
#if !UNITY_WEBPLAYER
                foreach (string path in Directory.GetDirectories(ContentPaths.appCachePathPacks)) {
                    string pathToAdd = PathUtil.Combine(appCachePathPacks, path);
                    if (!string.IsNullOrEmpty(pathToAdd)) {
                        if (!ContentPaths.packPaths.Contains(pathToAdd)) {
                            ContentPaths.packPaths.Add(pathToAdd);
                            //LogUtil.Log("Adding packPathsNONVersioned: pathToAdd:" + pathToAdd);
                        }
                    }
                }
#endif
            }
        }

        if (packPathsVersionedShared.Count == 0) {
            //LogUtil.Log("Loading packPathsVersionedShared: " + appCachePathSharedPacks);

            if (!string.IsNullOrEmpty(appCachePathSharedPacks)) {
#if !UNITY_WEBPLAYER
                foreach (string path in Directory.GetDirectories(appCachePathSharedPacks)) {
                    string pathToAdd = PathUtil.Combine(appCachePathSharedPacks, path);
                    if (!string.IsNullOrEmpty(pathToAdd)) {
                        if (!packPathsVersionedShared.Contains(pathToAdd)) {
                            packPathsVersionedShared.Add(pathToAdd);
                            //LogUtil.Log("Adding packPathsVersionedShared: pathToAdd:" + pathToAdd);
                        }
                    }
                }
#endif
            }
        }

        if (packPathsVersioned.Count == 0) {
            //LogUtil.Log("Loading packPathsVersioned: " + appCachePathAllPlatformPacks);
            if (!string.IsNullOrEmpty(appCachePathAllPlatformPacks)) {
#if !UNITY_WEBPLAYER
                foreach (string path in Directory.GetDirectories(appCachePathAllPlatformPacks)) {
                    string pathToAdd = PathUtil.Combine(appCachePathAllPlatformPacks, path);
                    if (!string.IsNullOrEmpty(pathToAdd)) {
                        if (!packPathsVersioned.Contains(pathToAdd)) {
                            packPathsVersioned.Add(pathToAdd);
                            //LogUtil.Log("Adding packPathsVersioned: pathToAdd:" + pathToAdd);
                        }
                    }
                }
#endif
            }
        }
    }

}

public class Contents : GameObjectBehavior {

    /*
	private static volatile Contents instance;
	private static System.Object syncRoot = new System.Object();
	
	public static Contents Instance {
		get {
			if (instance == null) { 
				lock (syncRoot) {
					if (instance == null) 
						instance = new Contents();
	        	}
	     	}	
	     return instance;
	  }
	}
	*/

    public static Contents Instance;

    public void Awake() {

        if(Instance != null && this != Instance) {
            //There is already a copy of this script running
            Destroy(this);
            return;
        }

        Instance = this;

        // Init();

    }

    public bool isReady {
        get {
            if(!string.IsNullOrEmpty(ContentPaths.appCachePath)
                && !string.IsNullOrEmpty(ContentPaths.appShipCachePath)) {
                return true;
            }
            return false;
        }
    }

    public static bool isInst {
        get {
            return Instance != null ? true : false;
        }
    }

    DownloadableContentUrlObject currentUrlObject = null;

    float incrementDownload = 0;
    float countsDownload = 0;

    public bool runtimeUpdate = false;

    public ContentSyncState syncState = ContentSyncState.SyncNotStarted;

    public ContentSyncDisplayState displayState = ContentSyncDisplayState.SyncNotStarted;
    public int displayStateCount = (int)ContentSyncDisplayState.StateCount;

    public string contentUrlRoot = ContentsConfig.contentEndpoint;
    public string contentUrlCDN = "http://s3.amazonaws.com/";

    public List<ContentItem> contentItemList = new List<ContentItem>();
    public ContentItem currentContentItem = new ContentItem();

    public Queue<DownloadableContentUrlObject> downloadUrlObjects = new Queue<DownloadableContentUrlObject>();
    public Queue<DownloadableContentUrlObject> processUrlObjects = new Queue<DownloadableContentUrlObject>();

    public static AssetBundle bundle;

    public static WWW downloader;
    public static WWW verifier;

    public static DownloadableContentItem dlcItem;
    public static ContentItemStatus contentItemStatus;
    public static bool downloadInProgress = false;

    public static ContentItemAccessDictionary contentItemAccess;

    public static Dictionary<string, string> fileHashLookup = null;

    public static bool initialSyncCompleted = false;
    public static bool initialDownload = false;
    public static string currentPackCodeSync = "default";

    void Start() {

        ContentPaths.CreateCachePaths();

        contentItemAccess = new ContentItemAccessDictionary();
        contentItemAccess.Load();

        //InitCache();

        fileHashLookup = new Dictionary<string, string>();

        // Content Sync Full

        Messenger<object>.AddListener(
            ContentMessages.ContentSyncFullStarted,
            OnContentSyncFullStarted);

        Messenger<object>.AddListener(
            ContentMessages.ContentSyncFullError,
            OnContentSyncFullError);

        Messenger<object>.AddListener(
            ContentMessages.ContentSyncFullSuccess,
            OnContentSyncFullSuccess);

        Messenger<object>.AddListener(
            ContentMessages.ContentSyncFullPrepare,
            OnContentSyncFullPrepare);
        
        // Content Sync Initial

        Messenger<object>.AddListener(
            ContentMessages.ContentSyncInitialStarted,
            OnContentSyncInitialStarted);

        Messenger<object>.AddListener(
            ContentMessages.ContentSyncInitialError,
            OnContentSyncInitialError);

        Messenger<object>.AddListener(
            ContentMessages.ContentSyncInitialSuccess,
            OnContentSyncInitialSuccess);

        // Content Sync Pack

        Messenger<object>.AddListener(
            ContentMessages.ContentSyncPackStarted,
            OnContentSyncPackStarted);

        Messenger<object>.AddListener(
            ContentMessages.ContentSyncPackError,
            OnContentSyncPackError);

        Messenger<object>.AddListener(
            ContentMessages.ContentSyncPackSuccess,
            OnContentSyncPackSuccess);
        
        //Messenger<object>.AddListener(
        //	ContentMessages.ContentAppContentListFileDownloadError, 
        //	OnContentAppContentListFileSuccess);

        //Messenger<object>.AddListener(
        //	ContentMessages.ContentAppContentListFileStarted, 
        //	OnContentAppContentListFileStarted);

        //Messenger<object>.AddListener(
        //	ContentMessages.ContentAppContentListFileError, 
        //	OnContentAppContentListFileError);
        
        // Content List Sync Pack

        Messenger<object>.AddListener(
            ContentMessages.ContentAppContentListSyncStarted,
            OnContentAppContentListSyncStarted);

        Messenger<object>.AddListener(
            ContentMessages.ContentAppContentListSyncError,
            OnContentAppContentListSyncError);

        Messenger<object>.AddListener(
            ContentMessages.ContentAppContentListSyncSuccess,
            OnContentAppContentListSyncSuccess);
        
        // Has content list

        Messenger<object>.AddListener(
            ContentMessages.ContentAppContentListFilesSuccess,
            OnContentAppContentListFilesSuccess);

        Messenger<object>.AddListener(
            ContentMessages.ContentAppContentListFilesStarted,
            OnContentAppContentListFilesStarted);

        Messenger<object>.AddListener(
            ContentMessages.ContentAppContentListFilesError,
            OnContentAppContentListFilesError);
        
        // Has content list

        Messenger<object>.AddListener(
            ContentMessages.ContentAppContentListFileDownloadSuccess,
            OnContentAppContentListFileDownloadSuccess);

        Messenger<object>.AddListener(
            ContentMessages.ContentAppContentListFileDownloadStarted,
            OnContentAppContentListFileDownloadStarted);

        Messenger<object>.AddListener(
            ContentMessages.ContentAppContentListFileDownloadError,
            OnContentAppContentListFileDownloadError);


    }

    // EVENTS SYNC

    // FULL

    public void OnContentSyncFullStarted(object obj) {

        LogUtil.Log("OnContentSyncFullStarted:", obj);

        ChangeSyncState(ContentSyncState.SyncProcessContentList);
        displayState = ContentSyncDisplayState.SyncPreparing;
    }

    public void OnContentSyncFullPrepare(object obj) {

        LogUtil.Log("OnContentSyncFullPrepare:", obj);

        //AppContentListItems.Instance.LoadData();
        displayState = ContentSyncDisplayState.SyncContentListsValidation;

        ChangeSyncState(ContentSyncState.SyncCompleted);
    }

    public void OnContentSyncFullSuccess(object obj) {

        LogUtil.Log("OnContentSyncFullSuccess:", obj);

        displayState = ContentSyncDisplayState.SynContentListsCompleted;
    }

    public void OnContentSyncFullError(object obj) {

        LogUtil.Log("OnContentSyncFullError:", obj);
        displayState = ContentSyncDisplayState.SynContentListsCompleted;
    }

    // INITIAL

    public void OnContentSyncInitialStarted(object obj) {

        LogUtil.Log("OnContentSyncInitialStarted:", obj);

        displayState = ContentSyncDisplayState.SyncContentListsDefault;

        ChangeSyncState(ContentSyncState.SyncProcessShipFiles);
    }

    public void OnContentSyncInitialSuccess(object obj) {

        LogUtil.Log("OnContentSyncInitialSuccess:", obj);

        ChangeSyncState(ContentSyncState.SyncPrepare);
    }

    public void OnContentSyncInitialError(object obj) {

        LogUtil.Log("OnContentSyncInitialError:", obj);

        ChangeSyncState(ContentSyncState.SyncError);
    }

    // PACK

    public void OnContentSyncPackStarted(object obj) {

        LogUtil.Log("OnContentSyncPackStarted:", obj);

        ChangeSyncState(ContentSyncState.SyncPackStarted);

        displayState = ContentSyncDisplayState.SyncContentListsPack;
    }

    public void OnContentSyncPackSuccess(object obj) {

        LogUtil.Log("OnContentSyncPackSuccess:", obj);

        ChangeSyncState(ContentSyncState.SyncPackCompleted);
    }

    public void OnContentSyncPackError(object obj) {

        LogUtil.Log("OnContentSyncPackError:", obj);

        ChangeSyncState(ContentSyncState.SyncError);
    }

    // EVENTS CONTENT LIST

    public void OnContentAppContentListSyncStarted(object obj) {

        LogUtil.Log("OnContentAppContentListSyncStarted:", obj);

    }

    public void OnContentAppContentListSyncError(object obj) {

        LogUtil.Log("OnContentAppContentListSyncError:", obj);

        ChangeSyncState(ContentSyncState.SyncError);
    }

    public void OnContentAppContentListSyncSuccess(object obj) {

        LogUtil.Log("OnContentAppContentListSyncSuccess:", obj);

        ChangeSyncState(ContentSyncState.SyncProcessDownloadFiles);

        if(displayState == ContentSyncDisplayState.SyncContentListsPack) {
            displayState = ContentSyncDisplayState.SyncContentListsPackDownload;
        }
        else if(displayState == ContentSyncDisplayState.SyncContentListsDefault) {
            displayState = ContentSyncDisplayState.SyncContentListsDefaultDownload;
        }

        ProcessDownloadableContentUrlQueue();
    }

    // SYNC ACTUAL FILES OR PACK DOWNLAOD

    public void OnContentAppContentListFilesStarted(object obj) {

        LogUtil.Log("OnContentAppContentListFilesStarted:", obj);

        //ChangeSyncState(ContentSyncState.SyncProcessDownloadFiles);
    }

    public void OnContentAppContentListFilesError(object obj) {

        LogUtil.Log("OnContentAppContentListFilesError:", obj);

        ChangeSyncState(ContentSyncState.SyncError);
    }

    public void OnContentAppContentListFilesSuccess(object obj) {

        LogUtil.Log("OnContentAppContentListFilesSuccess:", obj);

        // Start downloading the new files

        ProcessDownloadableContentUrlQueue();
    }

    public void OnContentAppContentListFileStarted(object obj) {

        LogUtil.Log("OnContentAppContentListFileStarted:", obj);

        ChangeSyncState(ContentSyncState.SyncProcessDownloadFiles);
    }

    public void OnContentAppContentListFileError(object obj) {

        LogUtil.Log("OnContentAppContentListFileError:", obj);

        ChangeSyncState(ContentSyncState.SyncError);
    }

    public void OnContentAppContentListFileSuccess(object obj) {

        LogUtil.Log("OnContentAppContentListFileSuccess:", obj);

        // Start downloading the new files

        if(displayState == ContentSyncDisplayState.SyncContentListsPack) {
            displayState = ContentSyncDisplayState.SyncContentListsPackDownload;
        }
        else if(displayState == ContentSyncDisplayState.SyncContentListsDefault) {
            displayState = ContentSyncDisplayState.SyncContentListsDefaultDownload;
        }

        ProcessDownloadableContentUrlQueue();

    }

    public void OnContentAppContentListFileDownloadStarted(object obj) {

        ChangeSyncState(ContentSyncState.SyncProcessDownloadFiles);
    }

    public void OnContentAppContentListFileDownloadError(object obj) {

        ChangeSyncState(ContentSyncState.SyncError);
    }

    public void OnContentAppContentListFileDownloadSuccess(object obj) {

        LogUtil.Log("OnContentAppContentListFileDownloadSuccess:", obj);

        // Start downloading the new files

        if(displayState == ContentSyncDisplayState.SyncContentListsPack) {
            displayState = ContentSyncDisplayState.SyncContentListsPackDownload;
        }
        else if(displayState == ContentSyncDisplayState.SyncContentListsDefault) {
            displayState = ContentSyncDisplayState.SyncContentListsDefaultDownload;
        }

        ProcessDownloadableContentUrlQueue();

    }

    // -----------------------------------------------------------------------

    public static void ChangeSyncState(ContentSyncState syncStateTo) {
        if(isInst) {
            Instance.changeSyncState(syncStateTo);
        }
    }

    public void changeSyncState(ContentSyncState syncStateTo) {
        if(syncState != syncStateTo) {
            syncState = syncStateTo;
            if(syncState == ContentSyncState.SyncNotStarted) {

            }
            else if(syncState == ContentSyncState.SyncStarted) {

                incrementDownload = 0;
                countsDownload = downloadUrlObjects.Count;
                Messenger<object>.Broadcast(
                    ContentMessages.ContentSyncFullStarted,
                    "Content Sync Started");
            }
            else if(syncState == ContentSyncState.SyncCompleted) {

                Messenger<object>.Broadcast(
                    ContentMessages.ContentSyncFullSuccess,
                    "Content Sync Completed Successfully");
            }
            else if(syncState == ContentSyncState.SyncPrepare) {

                Messenger<object>.Broadcast(
                    ContentMessages.ContentSyncFullPrepare,
                    "Content Sync Preparing");
            }
            else if(syncState == ContentSyncState.SyncError) {

                Messenger<object>.Broadcast(
                    ContentMessages.ContentSyncFullError,
                    "Content Sync Completed with Errors");
            }
            else if(syncState == ContentSyncState.SyncProcessContentList) {

                Messenger<object>.Broadcast(
                    ContentMessages.ContentAppContentListSyncStarted
                    , "Content Sync Started");
            }
            else if(syncState == ContentSyncState.SyncProcessDownloadFiles) {

                //Messenger<object>.Broadcast(
                //	ContentMessages.ContentAppContentListSyncStarted
                //	,"Content Sync Started");
            }
        }
    }

    // -----------------------------------------------------------------------

    public static void ProcessDownloadableContentUrlQueue() {
        if(isInst) {
            Instance.processDownloadableContentUrlQueue();
        }
    }

    public void processDownloadableContentUrlQueue() {
        StartCoroutine(processDownloadableContentUrlQueueCo());
    }

    public IEnumerator processDownloadableContentUrlQueueCo() {
        if(downloadUrlObjects != null) {
            if(downloadUrlObjects.Count > 0) {
                currentUrlObject = downloadUrlObjects.Dequeue();
                if(currentUrlObject != null) {
                    // download file...

                    if(countsDownload > 0) {

                        broadcastProgressMessage("Downloading Content",
                            getUnversionedDisplayFile(currentUrlObject.path), incrementDownload++ / countsDownload);

                        LogUtil.Log("Downloading Content:"
                            , " incrementDownload:" + incrementDownload
                            + " countsDownload:" + countsDownload);
                    }

                    yield return new WaitForEndOfFrame();

                    requestDownloadBytes(currentUrlObject.url);
                }
            }
            else {

                yield return StartCoroutine(processSyncUpdateCo());

                if(!initialSyncCompleted) {
                    // Kick off actual file process
                    processAppContentList(currentPackCodeSync);
                }
                else {
                    // Kick off prepare and load initial content state
                    changeSyncState(ContentSyncState.SyncPrepare);
                }
            }
        }
    }
    // -----------------------------------------------------------------------

    public static void ChangePackAndLoadMainScene(string pack) {
        if(isInst) {
            Instance.changePackAndLoadMainScene(pack);
        }
    }

    public void changePackAndLoadMainScene(string pack) {
        GamePacks.Instance.ChangeCurrentGamePack(pack);
        //GameLevels.Instance.ChangeCurrentGameLevel(pack + "-main");
        // scene bundle based with unity caching
        Contents.LoadSceneOrDownloadScenePackAndLoad(GamePacks.Current.code);
    }
    // -----------------------------------------------------------------------


    public static bool CheckGlobalContentAccess(string pack) {
        if(isInst) {
            return Instance.checkGlobalContentAccess(pack);
        }
        return false;
    }

    public bool checkGlobalContentAccess(string pack) {
        if(contentItemAccess.CheckAccess(pack)) {
            return true;
        }
        return false;
    }

    public static void SaveGlobalContentAccess() {
        if(isInst) {
            Instance.saveGlobalContentAccess();
        }
    }

    public void saveGlobalContentAccess() {
        contentItemAccess.Save();
    }

    public static void SetGlobalContentAccess(string pack) {
        if(isInst) {
            Instance.setGlobalContentAccess(pack);
        }
    }

    public void setGlobalContentAccess(string pack) {
        pack = pack.Replace(GamePacks.currentGameBundle + ".", "");
        contentItemAccess.SetContentAccess(pack);
        contentItemAccess.SetContentAccess(pack.Replace("-", "_"));
        contentItemAccess.SetContentAccess(pack.Replace("_", "-"));

        LogUtil.LogAccess("GameStore::SetContentAccessPermissions pack :" + pack);
        LogUtil.LogAccess("GameStore::SetContentAccessPermissions pack _ :" + pack.Replace("-", "_"));
        LogUtil.LogAccess("GameStore::SetContentAccessPermissions pack - :" + pack.Replace("_", "-"));
        LogUtil.LogAccess("GameStore::SetContentAccessPermissions pack - :" + pack.Replace("_", "-"));
        contentItemAccess.Save();
    }

    public static void SetContentAccessTransaction(string key, string productId, string receipt, int quantity, bool save) {
        if(isInst) {
            Instance.setContentAccessTransaction(key, productId, receipt, quantity, save);
        }
    }

    public void setContentAccessTransaction(string key, string productId, string receipt, int quantity, bool save) {
        contentItemAccess.SetContentAccessTransaction(key, productId, receipt, quantity, save);
    }

    // -----------------------------------------------------------------------

    public static void ProcessLoad(bool runtime) {
        if(isInst) {
            Instance.processLoad(runtime);
        }
    }

    public void processLoad(bool runtime) {
        runtimeUpdate = runtime;
        //AppViewerUIController.Instance.ShowUI();
        if(runtimeUpdate) {
            //UINotificationDisplayContent.Instance.HideDialog();
        }
        StartCoroutine(processLoadCo());
    }

    IEnumerator processLoadCo() {

        currentPackCodeSync = "default";

        ChangeSyncState(ContentSyncState.SyncStarted);

        ChangeSyncState(ContentSyncState.SyncProcessShipFiles);

        displayState = ContentSyncDisplayState.SyncPreparing;

        // On initial load handle cache

        yield return StartCoroutine(initCacheCo(true, true));

        displayState = ContentSyncDisplayState.SyncContentListsDefault;

        ChangeSyncState(ContentSyncState.SyncProcessContentList);

        // Download content list from root for ui and initial load
        ProcessAppContentListSync(currentPackCodeSync);
    }

    public static void ProcessPackLoad(string packCode, bool runtime) {
        if(isInst) {
            Instance.processPackLoad(packCode, runtime);
        }
    }

    public void processPackLoad(string packCode, bool runtime) {
        runtimeUpdate = true;
        processPackLoad(packCode);
    }

    public static void ProcessPackLoad(string packCode) {
        if(isInst) {
            Instance.processPackLoad(packCode);
        }
    }

    public void processPackLoad(string packCode) {
        runtimeUpdate = false;
        StartCoroutine(processPackLoadCo(packCode));
    }

    IEnumerator processPackLoadCo(string packCode) {
        // Process pack initial startup, sync and download any new files

        currentPackCodeSync = packCode;

        ChangeSyncState(ContentSyncState.SyncStarted);

        ChangeSyncState(ContentSyncState.SyncProcessContentList);

        displayState = ContentSyncDisplayState.SyncContentListsPack;

        yield return new WaitForEndOfFrame();

        ProcessAppContentList(packCode);

    }

    public static void ProcessAppContentListSync(string packCode) {
        if(isInst) {
            Instance.processAppContentList(packCode);
        }
    }

    public void processAppContentListSync(string packCode) {

        initialSyncCompleted = false;

        resetQueues();

        List<string> paths = collectAppContentListSync(packCode);
        if(paths.Count > 0) {
            requestDownloadableAppContentListSync(paths);
        }
        else {

            Messenger<object>.Broadcast(
                ContentMessages.ContentAppContentListSyncSuccess,
                    "Success, no files to sync");
        }
    }

    /*
	public void ProcessSyncUpdate() {
		if(processUrlObjects != null) {
			ProcessSyncedFiles();
		}
	}
	*/

    IEnumerator processSyncUpdateCo() {
        if(processUrlObjects != null) {
            yield return StartCoroutine(processSyncedFilesCo());
        }
    }

    public static void ProcessAppContentList(string packCode) {
        if(isInst) {
            Instance.processAppContentList(packCode);
        }
    }

    public void processAppContentList(string packCode) {

        initialSyncCompleted = true;

        ChangeSyncState(ContentSyncState.SyncProcessDownloadFiles);

        resetQueues();

        List<string> paths = collectAppContentListFiles(packCode);
        if(paths.Count > 0) {
            requestDownloadableAppContentListFiles(paths);
        }
        else {

            Messenger<object>.Broadcast(
                ContentMessages.ContentAppContentListFilesSuccess,
                    "Success, no files to sync");
        }
    }

    public static bool IsDefault(string code) {
        if(isInst) {
            return Instance.isDefault(code);
        }
        return true;
    }

    public bool isDefault(string code) {
        if(!string.IsNullOrEmpty(code)) {
            if(code.ToLower() == "default"
                    && code.ToLower() == "*"
                    && code.ToLower() == "all") {
                return true;
            }
        }
        return false;
    }

    public static bool CheckHashVerified(string pathVersioned, string hashData) {
        if(isInst) {
            return Instance.checkHashVerified(pathVersioned, hashData);
        }
        return false;
    }

    public bool checkHashVerified(string pathVersioned, string hashData) {
        string currentHash = ChecksumHash(pathVersioned);
        string dataHash = hashData;
        bool hashVerified = currentHash.ToLower() == dataHash.ToLower() ? true : false;
        return hashVerified;
    }

    public static List<string> CollectAppContentListFiles(string packCode) {
        if(isInst) {
            return Instance.collectAppContentListFiles(packCode);
        }
        return new System.Collections.Generic.List<string>();
    }

    public List<string> collectAppContentListFiles(string packCode) {

        AppContentListItems.Instance.LoadData(); // load latest

        List<AppContentListItem> appContentListItems
            = AppContentListItems.Instance.GetAll();//packCode);

        List<string> paths = new List<string>();

        foreach(AppContentListItem item in appContentListItems) {
            string path = PathUtil.Combine(item.data.directoryFull, item.data.fileName);
            string pathVersioned = getFullPathVersioned(path);
            string pathHashed = getFileVersioned(path, item.data.hash);
            bool fileExists = FileSystemUtil.CheckFileExists(pathVersioned);

            // TODO DEV CONTENT
            bool shouldBeUpdated = false;
            bool hashVerified = false;

            if(!fileExists) {
                pathVersioned = PathUtil.Combine(ContentPaths.appCachePath, pathVersioned);
                pathVersioned = GetPathUpdatedVersion(pathVersioned);
                fileExists = FileSystemUtil.CheckFileExists(pathVersioned);
            }

            if(pathVersioned.Contains("/icon")
                || pathVersioned.Contains("/featured-item")
                || pathVersioned.Contains("/featured")
                || pathVersioned.Contains("/" + AppContentStates.DATA_KEY)
                || pathVersioned.Contains("/" + AppContentActions.DATA_KEY)
                || pathVersioned.Contains("/" + AppContentAssets.DATA_KEY)
                || pathVersioned.Contains("/" + AppStates.DATA_KEY)
                //|| pathVersioned.Contains("/" + BaseGameStatistics.DATA_KEY)
                //|| pathVersioned.Contains("/" + BaseGameAchievements.DATA_KEY)
#if ENABLE_FEATURE_AR
                || pathVersioned.Contains("/" + ARDataSets.DATA_KEY)
				|| pathVersioned.Contains("/" + ARDataSetTrackers.DATA_KEY)
#endif
                ) {

                hashVerified = CheckHashVerified(pathVersioned, item.data.hash);

                if(hashVerified) {
                    shouldBeUpdated = false;
                }
                else {
                    shouldBeUpdated = true;
                }
            }

            if(fileExists
                && (packCode.ToLower() == item.pack_code.ToLower()
                || isDefault(packCode))
                ) {

                hashVerified = checkHashVerified(pathVersioned, item.data.hash);

                if(hashVerified) {
                    shouldBeUpdated = false;
                }
                else {
                    shouldBeUpdated = true;
                }
            }
            else if(!fileExists
                && (packCode.ToLower() == item.pack_code.ToLower()
                || isDefault(packCode))
                ) {
                shouldBeUpdated = true;
            }

            if(shouldBeUpdated) {
                pathHashed = getPathUpdatedVersion(pathHashed);
                paths.Add(pathHashed);
            }

        }

        return paths;
    }

    public static string CollectAppContentListSharedPacksPathData(string packCode, string key, string ext, bool versioned, bool synced) {
        if(isInst) {
            return Instance.collectAppContentListPlatformPacksPathData(packCode, key, ext, versioned, synced);
        }
        return "";
    }

    public string collectAppContentListSharedPacksPathData(string packCode, string key, string ext, bool versioned, bool synced) {

        string pathPack = "";

        // app content list data
        pathPack = ContentsConfig.contentVersion +
            "/shared/packs/" +
                packCode +
                "/data/" +
                key +
                "." + ext;


        if(synced) {
            pathPack = getFullPathVersionedSync(pathPack);
        }
        else if(versioned && !synced) {
            pathPack = getFileVersioned(pathPack);
        }

        return pathPack;
    }

    public static string CollectAppContentListPlatformPacksPathData(string packCode, string key, string ext, bool versioned, bool synced) {
        if(isInst) {
            return Instance.collectAppContentListPlatformPacksPathData(packCode, key, ext, versioned, synced);
        }
        return "";
    }

    public string collectAppContentListPlatformPacksPathData(string packCode, string key, string ext, bool versioned, bool synced) {

        string pathPack = "";

        // app content list data
        pathPack = ContentsConfig.contentVersion +
            "/" + ContentPaths.GetCurrentPlatformCode() + "/packs/" +
                packCode +
                "/data/" +
                key +
                "." + ext;


        if(synced) {
            pathPack = GetFullPathVersionedSync(pathPack);
        }
        else if(versioned && !synced) {
            pathPack = GetFileVersioned(pathPack);
        }

        return pathPack;
    }
    public static string CollectAppContentListSharedPacksPathContent(string packCode, string key, string ext, bool versioned, bool synced) {
        if(isInst) {
            return Instance.collectAppContentListSharedPacksPathContent(packCode, key, ext, versioned, synced);
        }
        return "";
    }

    public string collectAppContentListSharedPacksPathContent(string packCode, string key, string ext, bool versioned, bool synced) {

        string pathPack = "";

        // app content list data
        pathPack = ContentsConfig.contentVersion +
            "/shared/packs/" +
                packCode +
                "/content/" +
                key +
                "." + ext;

        if(synced) {
            pathPack = GetFullPathVersionedSync(pathPack);
        }
        else if(versioned && !synced) {
            pathPack = GetFileVersioned(pathPack);
        }

        return pathPack;
    }

    public static string CollectAppContentListSharedPacksPath(string packCode, string key, string ext, bool versioned, bool synced) {
        if(isInst) {
            return Instance.collectAppContentListSharedPacksPath(packCode, key, ext, versioned, synced);
        }
        return "";
    }

    public string collectAppContentListSharedPacksPath(string packCode, string key, string ext, bool versioned, bool synced) {

        string pathPack = "";

        // app content list data
        pathPack = ContentsConfig.contentVersion +
            "/shared/packs/" +
                packCode +
                "/" +
                key +
                "." + ext;

        if(synced) {
            pathPack = GetFullPathVersionedSync(pathPack);
        }
        else if(versioned && !synced) {
            pathPack = GetFileVersioned(pathPack);
        }

        return pathPack;
    }

    public static List<string> CollectAppContentListSync(string packCode) {
        if(isInst) {
            return Instance.collectAppContentListSync(packCode);
        }
        return new List<string>();
    }

    public List<string> collectAppContentListSync(string packCode) {

        AppContentListItems.Instance.LoadData(); // load latest

        List<string> paths = new List<string>();

        string pathRoot = ContentsConfig.contentVersion + "/data/" + AppContentListItems.DATA_KEY + ".json";
        string pathRootVersioned = GetFileVersioned(pathRoot);
        //string pathRootSync = GetFullPathVersionedSync(pathRoot); 

        paths.Add(pathRootVersioned);

        foreach(GamePack pack in GamePacks.Instance.GetAll()) {

            // content list items

            paths.Add(
                CollectAppContentListSharedPacksPathData(
                pack.code,
                AppContentListItems.DATA_KEY,
                "json",
                true,
                false));

            paths.Add(
                CollectAppContentListPlatformPacksPathData(
                pack.code,
                AppContentListItems.DATA_KEY,
                "json",
                true,
                false));
        }

        return paths;
    }

    // -----------------------------------------------------------------------

    public static void ResetQueues() {
        if(isInst) {
            Instance.resetQueues();
        }
    }

    public void resetQueues() {

        if(downloadUrlObjects == null) {
            downloadUrlObjects = new Queue<DownloadableContentUrlObject>();
        }

        downloadUrlObjects.Clear();

        if(processUrlObjects == null) {
            processUrlObjects = new Queue<DownloadableContentUrlObject>();
        }
        processUrlObjects.Clear();
    }

    public static string GetPathUpdatedVersion(string url) {
        if(isInst) {
            return Instance.getPathUpdatedVersion(url);
        }
        return url;
    }

    public string getPathUpdatedVersion(string url) {
        if(url.Contains("version/")) {
            url = url.Replace("version/", ContentsConfig.contentVersion + "/");
        }
        return url;
    }

    // ----------------------------------------------------------------------------------
    // HANDLERS	

    void handleDownloadableAppContentListFilesCallback(Engine.Networking.WebRequests.ResponseObject response) {

        response = handleResponseObject(response);

        bool serverError = false;

        if(response.validResponse) {

            LogUtil.LogAccess("Successful HandleDownloadableAppContentListFilesCallback verfication download...");

            string dataToParse = response.data;

            LogUtil.LogAccess("dataToParse:" + dataToParse);

            if(!string.IsNullOrEmpty(dataToParse)) {

                try {
                    DownloadableContentItemListResponse responseData
                        = dataToParse.FromJson<DownloadableContentItemListResponse>();

                    foreach(KeyValuePair<string, DownloadableContentUrlObject> item
                        in responseData.data.url_objs) {
                        DownloadableContentUrlObject urlData = item.Value;
                        downloadUrlObjects.Enqueue(urlData);
                    }

                    countsDownload = downloadUrlObjects.Count;
                    incrementDownload = 0;

                    Messenger<object>.Broadcast(
                        ContentMessages.ContentAppContentListFilesSuccess,
                        "Content verified, downloading and loading pack.");
                }
                catch(Exception e) {
                    serverError = true;
                    LogUtil.LogAccess("Parsing error:"
                        + e.Message + e.StackTrace + e.Source);
                }
            }
            else {
                serverError = true;
            }
        }
        else {
            // There was a problem with the response.
            LogUtil.LogAccess("HandleDownloadableAppContentListFilesCallback NON-SUCCESSFUL DOWNLOAD");
            serverError = true;
        }

        if(serverError) {
            reset();
            Messenger<object>.Broadcast(
                ContentMessages.ContentAppContentListFilesError,
                "Error on server, please try again.");
        }
    }

    void handleDownloadableAppContentListSyncCallback(Engine.Networking.WebRequests.ResponseObject response) {

        response = handleResponseObject(response);

        bool serverError = false;

        if(response.validResponse) {

            LogUtil.LogAccess("Successful HandleDownloadableAppContentListSyncCallback verfication download...");

            string dataToParse = response.data;

            LogUtil.LogAccess("dataToParse:" + dataToParse);

            if(!string.IsNullOrEmpty(dataToParse)) {

                try {
                    DownloadableContentItemListResponse responseData
                        = dataToParse.FromJson<DownloadableContentItemListResponse>();

                    foreach(KeyValuePair<string, DownloadableContentUrlObject> item
                        in responseData.data.url_objs) {
                        DownloadableContentUrlObject urlData = item.Value;
                        downloadUrlObjects.Enqueue(urlData);
                    }

                    countsDownload = downloadUrlObjects.Count;
                    incrementDownload = 0;

                    Messenger<object>.Broadcast(
                        ContentMessages.ContentAppContentListSyncSuccess,
                        "Content list verfication success.");
                }
                catch(Exception e) {
                    serverError = true;
                    LogUtil.LogAccess("Parsing error:"
                        + e.Message + e.StackTrace + e.Source);
                }
            }
            else {
                serverError = true;
            }
        }
        else {
            // There was a problem with the response.
            LogUtil.LogAccess("HandleDownloadableAppContentListSyncCallback NON-SUCCESSFUL DOWNLOAD");
            serverError = true;
        }

        if(serverError) {
            reset();
            Messenger<object>.Broadcast(
                ContentMessages.ContentAppContentListSyncError,
                "Error on server, please try again.");
        }
    }

    // ----------------------------------------------------------------------------------
    // HANDLERS - DEFAULT

    void handleDownloadAssetBundleCallback(Engine.Networking.WebRequests.ResponseObject response) {

        /*
		 * 
		response = handleResponseObject(response);
		
		if(response.validResponse) {
			
			LogUtil.LogAccess("SUCCESSFUL DOWNLOAD");						
			
			
			
			
			string dataToParse = response.data;
						
			LogUtil.LogAccess("dataToParse:" + dataToParse);
						
			if(!string.IsNullOrEmpty(dataToParse)) {	
				
				try {
					DownloadableContentItemResponse responseData 
					= JsonMapper.ToObject<DownloadableContentItemResponse>(dataToParse);
					
					dlcItem = responseData.data;
				}
				catch(Exception e) {							
					serverError = true;
					LogUtil.LogAccess("Parsing error:" 
					+ e.Message + e.StackTrace + e.Source);
				}				
				
				if(dlcItem != null) {
					
					List<string> downloadUrls = dlcItem.download_urls;
					
					foreach(string url in downloadUrls) {
						Messenger<string>.Broadcast(
							ContentMessages.ContentItemVerifySuccess, 
							"Content verified, downloading and loading pack." );
						
						StartCoroutine(
							Contents.SceneLoadFromCacheOrDownloadCo(url));
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
			LogUtil.LogAccess("NON-SUCCESSFUL DOWNLOAD");			
			serverError = true;
		}
		
		if(serverError) {
			reset();
			Messenger<string>.Broadcast(
			ContentMessages.ContentItemVerifyError, 
			"Error on server, please try again.");
		}
		*/
    }

    void handleDownloadableContentInfoCallback(Engine.Networking.WebRequests.ResponseObject response) {

        response = handleResponseObject(response);

        bool serverError = false;

        if(response.validResponse) {

            LogUtil.LogAccess("SUCCESSFUL DOWNLOAD");

            string dataToParse = response.data;

            LogUtil.LogAccess("dataToParse:" + dataToParse);

            if(!string.IsNullOrEmpty(dataToParse)) {

                try {
                    DownloadableContentItemResponse responseData
                        = dataToParse.FromJson<DownloadableContentItemResponse>();
                    dlcItem = responseData.data;
                }
                catch(Exception e) {
                    serverError = true;
                    LogUtil.LogAccess("Parsing error:"
                        + e.Message + e.StackTrace + e.Source);
                }

                if(dlcItem != null) {

                    List<string> downloadUrls = dlcItem.download_urls;

                    foreach(string url in downloadUrls) {
                        Messenger<string>.Broadcast(
                            ContentMessages.ContentItemVerifySuccess,
                            "Content verified, downloading and loading pack.");

                        StartCoroutine(
                            sceneLoadFromCacheOrDownloadCo(url));
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
            LogUtil.LogAccess("NON-SUCCESSFUL DOWNLOAD");
            serverError = true;
        }

        if(serverError) {
            reset();
            Messenger<string>.Broadcast(
                ContentMessages.ContentItemVerifyError,
                "Error on server, please try again.");
        }
    }

    //HandleDownloadableContentSetSyncCallback

    void handleDownloadableContentSetSyncCallback(
        Engine.Networking.WebRequests.ResponseObject response) {

        response = handleResponseObject(response);

        bool serverError = false;

        if(response.validResponse) {

            LogUtil.LogAccess("HandleDownloadableContentSetSyncCallback valid");

            string dataToParse = response.data;

            LogUtil.LogAccess("dataToParse:" + dataToParse);

            if(!string.IsNullOrEmpty(dataToParse)) {

                try {
                    DownloadableContentItemResponse responseData
                        = dataToParse.FromJson<DownloadableContentItemResponse>();

                    dlcItem = responseData.data;
                }
                catch(Exception e) {
                    serverError = true;
                    LogUtil.LogAccess("Parsing error:"
                        + e.Message + e.StackTrace + e.Source);
                }

                if(dlcItem != null) {

                    List<string> downloadUrls = dlcItem.download_urls;

                    foreach(string url in downloadUrls) {
                        Messenger<string>.Broadcast(
                            ContentMessages.ContentItemVerifySuccess,
                            "Content verified, downloading and loading pack.");

                        StartCoroutine(
                            sceneLoadFromCacheOrDownloadCo(url));
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
            LogUtil.LogAccess("NON-SUCCESSFUL DOWNLOAD");
            serverError = true;
        }

        if(serverError) {
            reset();
            Messenger<string>.Broadcast(
                ContentMessages.ContentItemVerifyError,
                "Error on server, please try again.");
        }
    }

    void handleDownloadableFileCallback(Engine.Networking.WebRequests.ResponseObject response) {

        response = handleResponseObject(response);

        bool serverError = false;

        if(response.validResponse) {

            LogUtil.LogAccess("Successful verfication download...");

            string dataToParse = response.data;

            LogUtil.LogAccess("dataToParse:" + dataToParse);

            if(!string.IsNullOrEmpty(dataToParse)) {

                try {
                    DownloadableContentItemResponse responseData
                        = dataToParse.FromJson<DownloadableContentItemResponse>();

                    dlcItem = responseData.data;
                }
                catch(Exception e) {
                    serverError = true;
                    LogUtil.LogAccess("Parsing error:"
                        + e.Message + e.StackTrace + e.Source);
                }

                if(dlcItem != null) {

                    List<string> downloadUrls = dlcItem.download_urls;

                    if(downloadUrls.Count > 0) {
                        Messenger<string>.Broadcast(
                            ContentMessages.ContentItemVerifySuccess,
                            "Content verified, downloading and loading pack.");
                        //LogUtil.Log("url:" + url);
                        //StartCoroutine(Contents.SceneLoadFromCacheOrDownloadCo(url));
                        //WebRequests.Instance.Request(
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
            LogUtil.LogAccess("NON-SUCCESSFUL DOWNLOAD");
            serverError = true;
        }

        if(serverError) {
            reset();
            Messenger<string>.Broadcast(
                ContentMessages.ContentItemVerifyError,
                "Error on server, please try again.");
        }
    }

    //public WebRequests.ResponseObject HandleResponseObjectAssetBundle(WebRequests.ResponseObject responseObject) {		

    //}

    public static Engine.Networking.WebRequests.ResponseObject HandleResponseObject(
        Engine.Networking.WebRequests.ResponseObject responseObject) {

        if(isInst) {
            return Instance.handleResponseObject(responseObject);
        }

        return null;
    }

    public Engine.Networking.WebRequests.ResponseObject handleResponseObject(
        Engine.Networking.WebRequests.ResponseObject responseObject) {

        bool serverError = false;

        // Manages common response object parsing to get to object
        if(responseObject.dataValueText != null) {

            Dictionary<string, object> data =
                responseObject.dataValueText.FromJsonToDict();

            if(data != null && data.Count > 0) {

                string code = "9999";

                try {
                    code = data.Get<string>("code");
                }
                catch(Exception e) {
                    responseObject.error = 1;
                    LogUtil.Log("ERROR: " +
                                e.Message + e.StackTrace + e.Source);
                }

                string message = "Failure to parse response.";

                try {
                    message = data.Get<string>("message");
                }
                catch(Exception e) {
                    responseObject.error = 1;
                    LogUtil.Log("ERROR: " +
                                e.Message + e.StackTrace + e.Source);
                }

                /*
				
				JsonData dataValue = null;
				
				try {
					if(data["data"] != null) {
						if(data["data"].IsObject) {
							dataValue = data["data"];
						}
					}				
				}
				catch(Exception e) {
					responseObject.error = 1;
					LogUtil.Log("ERROR: " + e.Message + e.StackTrace + e.Source);
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

                LogUtil.LogAccess("STATUS/CODE:" + code);
                LogUtil.LogAccess("STATUS/CODE MESSAGE:" + message);

                if(code == "0") {
                    LogUtil.LogAccess("STATUS/DATA NODE:" + data);

                    LogUtil.LogAccess("dataValue:" + data["data"]);
                    LogUtil.LogAccess("responseObject.dataValueText:"
                        + responseObject.dataValueText);

                    responseObject.data = responseObject.dataValueText;
                    responseObject.dataValue = data["data"];
                    responseObject.validResponse = true;
                }
                else {
                    LogUtil.Log(
                        "ERROR - Good response but problem with data, see message.");

                    serverError = true;
                }
            }
        }
        else {
            LogUtil.LogAccess("ERROR - NO DATA");
            serverError = true;
        }

        if(serverError) {
            responseObject.validResponse = false;
            reset();
            Messenger<string>.Broadcast(
                ContentMessages.ContentItemVerifyError,
                "Error receiving a server response, please try again.");
        }

        return responseObject;
    }

    // ----------------------------------------------------------------------------------
    // REQUESTS		

    public static void RequestDownloadableContent(string pack) {
        if(isInst) {
            Instance.requestDownloadableContent(pack);
        }
    }

    public void requestDownloadableContent(string pack) {
        RequestDownloadableContent(
            GamePacks.currentPacksGame,
            GamePacks.currentPacksVersion,
            GamePacks.currentPacksPlatform,
            pack);
    }

    public static void RequestDownloadableContent(
        string game, string version, string platform, string pack) {

        if(isInst) {
            Instance.requestDownloadableContent(game, version, platform, pack);
        }
    }

    public void requestDownloadableContent(
        string game, string version, string platform, string pack) {
        //glob.ShowLoadingIndicator();

        Dictionary<string, object> data = new Dictionary<string, object>();
        string udid = UniqueUtil.Instance.currentUniqueId;

        data.Add("device_id", udid);
        data.Add("app_id", ContentsConfig.contentApiKey);

        downloadInProgress = true;

        string url = getDownloadContentItemUrl(
            game, version, platform, pack);

        WebRequests.Instance.Request(
            WebRequests.RequestType.HTTP_POST, url, data,
            handleDownloadableContentInfoCallback);

        contentItemStatus = new ContentItemStatus();

        Messenger<string>.Broadcast(
            ContentMessages.ContentItemVerifyStarted,
            "Verifying content access...");
    }

    public static void RequestDownloadableContentSetSync(
        string game, string version, string platform) {
        if(isInst) {
            Instance.requestDownloadableContentSetSync(game, version, platform);
        }
    }

    public void requestDownloadableContentSetSync(
        string game, string version, string platform) {

        downloadInProgress = true;

        string url = getContentSetUrl(game, version, platform);
        WebRequests.Instance.Request(
            WebRequests.RequestType.HTTP_GET, url,
            handleDownloadableContentSetSyncCallback);

        Messenger<string>.Broadcast(
            ContentMessages.ContentSetDownloadStarted,
            "Getting downloadable content access...");
    }

    public static void RequestDownloadableFile(string url) {
        if(isInst) {
            Instance.requestDownloadableFile(url);
        }
    }

    public void requestDownloadableFile(string url) {

        downloadInProgress = true;

        WebRequests.Instance.Request(
            WebRequests.RequestType.HTTP_GET, url,
            handleDownloadableFileCallback);

        Messenger<string>.Broadcast(
            ContentMessages.ContentFileDownloadStarted,
            "Started downloading..." + url);
    }

    public static Dictionary<string, object> GetDefaultPostParams() {
        if(isInst) {
            return Instance.getDefaultPostParams();
        }
        return null;
    }

    public Dictionary<string, object> getDefaultPostParams() {
        Dictionary<string, object> data = new Dictionary<string, object>();
        string udid = UniqueUtil.Instance.currentUniqueId;
        data.Add("device_id", udid);
        data.Add("app_id", ContentsConfig.contentApiKey);
        return data;
    }

    public static void RequestDownloadableAppContentListSync(List<string> paths) {
        if(isInst) {
            Instance.requestDownloadableAppContentListSync(paths);
        }
    }

    public void requestDownloadableAppContentListSync(List<string> paths) {

        downloadInProgress = true;

        string game = ContentsConfig.contentAppFolder;
        string platform = ContentPaths.GetCurrentPlatformCode();
        string version = ContentsConfig.contentVersion;

        string url = GetDownloadAppContentListFilesUrl(game, version, platform);

        Dictionary<string, object> data = GetDefaultPostParams();

        string stringPaths = string.Join(",", paths.ToArray());

        data.Add("paths", stringPaths);

        downloadUrlObjects.Clear();

        WebRequests.Instance.Request(
            WebRequests.RequestType.HTTP_POST, url, data,
            handleDownloadableAppContentListSyncCallback);

        ChangeSyncState(ContentSyncState.SyncProcessContentList);
    }

    public static void RequestDownloadableAppContentListFiles(List<string> paths) {
        if(isInst) {
            Instance.requestDownloadableAppContentListFiles(paths);
        }
    }

    public void requestDownloadableAppContentListFiles(List<string> paths) {

        downloadInProgress = true;

        string game = ContentsConfig.contentAppFolder;
        string platform = ContentPaths.GetCurrentPlatformCode();
        string version = ContentsConfig.contentVersion;

        string url = GetDownloadAppContentListFilesUrl(game, version, platform);

        Dictionary<string, object> data = GetDefaultPostParams();

        string stringPaths = string.Join(",", paths.ToArray());

        data.Add("paths", stringPaths);

        downloadUrlObjects.Clear();

        WebRequests.Instance.Request(
            WebRequests.RequestType.HTTP_POST, url, data,
            handleDownloadableAppContentListFilesCallback);

        Messenger<object>.Broadcast(
            ContentMessages.ContentAppContentListFilesStarted,
            "Started downloading..." + url);
    }


    // ----------------------------------------------------------------------------------
    // HELPERS		

    public static string GetDownloadContentItemUrl(
        string game, string buildVersion, string platform, string pack) {
        if(isInst) {
            return Instance.getDownloadContentItemUrl(
                game, buildVersion, platform, pack);
        }
        return "";
    }

    public string getDownloadContentItemUrl(
        string game, string buildVersion, string platform, string pack) {
        // add increment to the pack name
        pack = pack + "-" + Convert.ToString(GamePacks.currentPacksIncrement);
        return String.Format(
            ContentEndpoints.contentDownloadFileAsset,
            game, buildVersion, platform, pack);
    }

    public static string GetDownloadAppContentListFilesUrl(
        string game, string buildVersion, string platform) {
        if(isInst) {
            return Instance.getDownloadAppContentListFilesUrl(
                game, buildVersion, platform);
        }
        return "";
    }

    //contentDownloadAppContentListFiles
    public string getDownloadAppContentListFilesUrl(
        string game, string buildVersion, string platform) {
        return String.Format(
            ContentEndpoints.contentDownloadAppContentListFiles,
            game, buildVersion, platform);
    }

    public static string GetContentSetUrl(
        string game, string buildVersion, string platform) {
        if(isInst) {
            return Instance.getContentSetUrl(game, buildVersion, platform);
        }
        return "";
    }

    public string getContentSetUrl(
        string game, string buildVersion, string platform) {
        // add increment to the pack name
        //pack = pack + "-" + Convert.ToString(GamePacks.currentPacksIncrement);
        return String.Format(
            ContentEndpoints.contentSyncContentSet,
            game, buildVersion, platform);
    }

    public static void LoadSceneOrDownloadScenePackAndLoad(string pack) {
        if(isInst) {
            Instance.loadSceneOrDownloadScenePackAndLoad(pack);
        }
    }

    public void loadSceneOrDownloadScenePackAndLoad(string pack) {
        loadSceneOrDownloadScenePackAndLoad(
            GamePacks.currentPacksGame,
            GamePacks.currentPacksVersion,
            GamePacks.currentPacksPlatform,
            pack);
    }

    public static void LoadSceneOrDownloadScenePackAndLoad(
        string game, string buildVersion, string platform, string pack) {
        if(isInst) {
            Instance.loadSceneOrDownloadScenePackAndLoad(
                game, buildVersion, platform, pack);
        }
    }

    public void loadSceneOrDownloadScenePackAndLoad(
        string game, string buildVersion, string platform, string pack) {

        bool isDownloadableContent = IsDownloadableContent(pack);
        LogUtil.LogAccess("isDownloadableContent:" + isDownloadableContent);
        int version = GamePacks.currentPacksIncrement;

        //string url = GetDownloadContentItemUrl(game, buildVersion, platform, pack);

        string lastPackUrlValue = GetLastPackState(pack);

        if(Caching.IsVersionCached(lastPackUrlValue, Hash128.Parse(version.ToString()))
            && !string.IsNullOrEmpty(lastPackUrlValue)) {
            // Just load from the saved url
            StartCoroutine(
                sceneLoadFromCacheOrDownloadCo(lastPackUrlValue));
        }
        else {
            // Do download verification and download
            requestDownloadableContent(game, buildVersion, platform, pack);
        }
    }

    public static bool IsDownloadableContent(string pack) {
        if(isInst) {
            return Instance.isDownloadableContent(pack);
        }
        return false;
    }

    public bool isDownloadableContent(string pack) {
        //if(pack.ToLower() == GamePacks.PACK_BOOK_DEFAULT.ToLower()) {
        //	return true;
        //}
        return false;
    }

    public static void SetLastPackState(string packName, string url) {
        if(isInst) {
            Instance.setLastPackState(packName, url);
        }
    }

    public void setLastPackState(string packName, string url) {

        if(IsDownloadableContent(packName)) {
            string lastPackUrlKey = "last-pack-" + packName;
            string lastPackUrlValue = url;

            if(!string.IsNullOrEmpty(lastPackUrlValue)) {
                SystemPrefUtil.SetLocalSettingString(lastPackUrlKey, lastPackUrlValue);
                SystemPrefUtil.Save();
            }
        }
    }

    public static string GetLastPackState(string packName) {
        if(isInst) {
            return Instance.getLastPackState(packName);
        }
        return "";
    }

    public string getLastPackState(string packName) {
        if(IsDownloadableContent(packName)) {
            string lastPackUrlKey = "last-pack-" + packName;
            if(SystemPrefUtil.HasLocalSetting(lastPackUrlKey)) {
                return SystemPrefUtil.GetLocalSettingString(lastPackUrlKey);
            }
        }
        return "";
    }


    // ----------------------------------------------------------------------------------
    // FILE LOADING

    // Individual file downloading

    public static string GetHashCodeFromFile(string url) {
        if(isInst) {
            return Instance.getHashCodeFromFile(url);
        }
        return "";
    }

    public string getHashCodeFromFile(string url) {
        string hash = "";
        if(!url.Contains(ContentPaths.appCachePath)) {
            url = PathUtil.Combine(ContentPaths.appCachePath, url);
        }
        hash = ChecksumHash(url);
        return hash;
    }

    public static string GetHashCodeFromFilePath(string url) {
        if(isInst) {
            return Instance.getHashCodeFromFilePath(url);
        }
        return url;
    }

    public string getHashCodeFromFilePath(string url) {
        string hash = "";

        string text = url;
        string pat = @"([^-]*)\.*$";

        Regex r = new Regex(pat, RegexOptions.IgnoreCase);

        Match m = r.Match(text);

        //int matchCount = 0;

        if(m.Success) {
            hash = m.Value;
            hash = hash.Split('.')[0];
        }

        return hash;
        /*
		while (m.Success) {
			LogUtil.Log("Match", (++matchCount));
			for (int i = 1; i <= 2; i++) {
				Group g = m.Groups[i];
				LogUtil.Log("Group"+i+"='" + g + "'","");
				CaptureCollection cc = g.Captures;
				for (int j = 0; j < cc.Count; j++) {
					Capture c = cc[j];
					LogUtil.Log("Capture"+j+"='" + c + "', Position="+c.Index);
				}
			}
			m = m.NextMatch();
		}
		*/
    }

    // BINARY SAVING	

    public static void HandleSyncedFileBinary(byte[] bytes, DownloadableContentUrlObject urlObject) {
        if(isInst) {
            Instance.handleSyncedFileBinary(bytes, urlObject);
        }
    }

    public void handleSyncedFileBinary(byte[] bytes, DownloadableContentUrlObject urlObject) {
        if(bytes != null) {

            string path = urlObject.path;
            string pathSave = path;
            string pathCache = ContentPaths.appCachePath;

            if(!pathSave.Contains(pathCache)) {
                pathSave = PathUtil.Combine(pathCache, path);
            }

            string pathBase = ContentsConfig.contentRootFolder + "/";
            pathBase += ContentsConfig.contentAppFolder + "/";

            if(pathSave.Contains(pathBase + pathBase)) {
                pathSave = pathSave.Replace(pathBase + pathBase, pathBase);
            }

            if(pathSave.Contains("/" + AppContentListItems.DATA_KEY)) {
                // If this is a content list, hash it before saving


                string pathSaveHashedTemp = pathSave + processMarker;
                FileSystemUtil.WriteAllBytes(pathSaveHashedTemp, bytes);

                //string hash = ChecksumHash(pathSaveHashedTemp);
                //string pathUnversioned = GetFileUnversioned(pathSaveHashedTemp);
                //string pathSaveHashed = GetFileVersioned(pathUnversioned, hash);
                //pathSaveHashed = pathSaveHashed.Replace("-process","");
            }
            else {

                // Save file to check against before copying over			
                FileSystemUtil.WriteAllBytes(pathSave, bytes);

            }

            processUrlObjects.Enqueue(urlObject);
        }
    }

    IEnumerator processSyncedFilesCo() {

        AppContentListItems.Instance.LoadData();

        if(processUrlObjects != null) {

            validatingTotal = processUrlObjects.Count;
            validatingInc = 0;

            yield return StartCoroutine(processSyncedFilesRecursiveCo());
        }

        AppContentListItems.Instance.LoadData();
    }

    /*
	public void ProcessSyncedFilesRecursive() {
		if(processUrlObjects != null) {		
			if(processUrlObjects.Count > 0) {
				DownloadableContentUrlObject urlObject = processUrlObjects.Dequeue();
				ProcessSyncedFile(urlObject);
				ProcessSyncedFilesRecursive();
			}
		} 
	}
	*/

    public static string GetUnversionedDisplayFile(string val) {
        if(isInst) {
            return Instance.getUnversionedDisplayFile(val);
        }
        return "";
    }

    public string getUnversionedDisplayFile(string val) {
#if !UNITY_WEBPLAYER
        val = Path.GetFileName(val);
#endif
        val = GetDisplayFileUnversioned(val);

        /*
		AppContentListItem item = AppContentListItems.Instance.GetAll();
		if(item.data.fileName.Contains(val)) {
		
		}
		*/

        if(val.Contains(AppContentListItems.DATA_KEY)) {
            val = "Checking For Awesome New Content";
        }
        //else if(val.Contains(BaseGameStatistics.DATA_KEY)) {
        //	val = "Challenging Statistics";
        //}
        //else if(val.Contains(BaseGameAchievements.DATA_KEY)) {
        //	val = "Achievements To Earn";
        //}
        else if(val.Contains(AppContentStates.DATA_KEY)) {
            val = "3D Content";
        }
        else if(val.Contains(AppStates.DATA_KEY)) {
            val = "Virtual Aisles + Categories";
        }
        else if(val.Contains(AppContentAssets.DATA_KEY)) {
            val = "3D Objects, Audio + Video";
        }
        else if(val.Contains(".m4v") || val.Contains(".mp4")) {
            val = "Videos";
        }
        else if(val.Contains(".mp3") || val.Contains(".mp3")) {
            val = "Audio + Effects";
        }
        else if(val.Contains("icon") || val.Contains("feature")) {
            val = "Icons + Feature Images";
        }
        else if(val.Contains(".png") || val.Contains(".png")) {
            val = "Images";
        }

        return val;
    }

    float validatingTotal = 0;
    float validatingInc = 0;

    IEnumerator processSyncedFilesRecursiveCo() {
        if(processUrlObjects != null) {
            if(validatingTotal > 0) {
                DownloadableContentUrlObject urlObject = processUrlObjects.Dequeue();
                broadcastProgressMessage("Validating Files", GetUnversionedDisplayFile(urlObject.path), validatingInc++ / validatingTotal);
                processSyncedFile(urlObject);

                yield return new WaitForEndOfFrame();
                yield return StartCoroutine(processSyncedFilesRecursiveCo());
            }
        }
    }


    public static string processMarker = "____process";

    public static void ProcessSyncedFile(DownloadableContentUrlObject urlObject) {
        if(isInst) {
            Instance.processSyncedFile(urlObject);
        }
    }

    public void processSyncedFile(DownloadableContentUrlObject urlObject) {

        string path = urlObject.path;
        string pathSave = path;
        string pathCache = ContentPaths.appCachePath;

        if(!pathSave.Contains(pathCache)) {
            pathSave = PathUtil.Combine(pathCache, path);
        }

        bool isContentList = pathSave.Contains(
            "/" + AppContentListItems.DATA_KEY);

        if(isContentList) {
            pathSave = pathSave + processMarker;
        }

        // Parse out hash and test hash

        // Check filename hash
        string hashNewFile = GetHashCodeFromFile(pathSave);

        // Check actual file contents hash
        string hashNew = GetHashCodeFromFilePath(pathSave);

        if(isContentList) {
            hashNew = hashNewFile;
        }

        // Make sure file is valid checksum in filename matches file contents
        // to verify it is a complete and valid downloaded file.
        bool isFileValid = hashNew.ToLower() == hashNewFile.ToLower() ? true : false;

        string pathNoHashNew = path.Replace("-" + hashNew, "");

        string pathNoHashCurrent = pathSave.Replace("-" + hashNew, "");
        string hashCurrent = GetHashCodeFromFile(pathNoHashCurrent);

        string pathVersionedSync = ContentsConfig.contentRootFolder + "/";
        pathVersionedSync += ContentsConfig.contentAppFolder + "/";
        pathVersionedSync = PathUtil.Combine(pathVersionedSync, path);

        string pathVersioned = ContentsConfig.contentRootFolder + "/";
        pathVersioned += ContentsConfig.contentAppFolder + "/";
        pathVersioned = PathUtil.Combine(pathVersioned, pathNoHashNew);

        bool updateFile = false;
        //List<AppContentListItem> appContentListItems = null;
        bool isAllType = pathVersionedSync.Contains(
            ContentsConfig.contentAppFolder + "/all/");

        if(isFileValid) {
            // Hash matches from content list and downloaded file

            // If download matches hash in data and from file then update
            // if has new doesn't match hash current then update

            //if(Application.isEditor) {
            //	updateFile = true;
            //}
            //else {
            updateFile = false;
            //}

            if(hashNew != hashCurrent) {
                updateFile = true;
            }

            if(updateFile || isContentList) {

                if(isContentList) {

                    pathNoHashCurrent = pathNoHashCurrent.Replace(processMarker, "");

                    FileSystemUtil.CopyFile(
                        pathSave,
                        pathNoHashCurrent, true);
                    FileSystemUtil.RemoveFile(pathSave);
                }
                else {
                    if(updateFile) {

                        if(isAllType) {
                            //string pathSaveTo = 
                            //	appContentListItem.data.GetFilePaths().
                            //	pathNonVersionedSystem;
                            FileSystemUtil.CopyFile(
                                pathSave,
                                GetFileUnversioned(pathNoHashCurrent)
                                , true);
                        }
                        else {
                            //string pathSaveTo = 
                            //	appContentListItem.data.GetFilePaths().
                            //	pathVersionedSystem;
                            FileSystemUtil.CopyFile(
                                pathSave,
                                pathNoHashCurrent, true);
                        }

                        FileSystemUtil.RemoveFile(pathSave);
                    }


                    /*
					if(isAllType) {
						appContentListItems
							 = AppContentListItems.Instance.GetListByPackCodeAndPathVersioned(
									GamePacks.Current.code, 
									pathVersioned);	
					}
					else {
						
						appContentListItems
							 = AppContentListItems.Instance.GetListByPackCodeAndPathVersioned(
									GamePacks.Current.code, 
									pathVersioned);	
					}
						
					foreach(AppContentListItem appContentListItem in appContentListItems) {
						
						// Check if file should be overwritten
						// Check if file has for new matches file
						// If so copy over current
										
						string hashCurrentContentList = appContentListItem.data.hash;
						if(hashCurrentContentList.ToLower() == hashNew.ToLower()
							&& isFileValid) {
						
							// Hash matches from content list and downloaded file
							
							// If download matches hash in data and from file then update
							// if has new doesn't match hash current then update
							// TODO DEV trigger
							
					if(updateFile) {
						
						if(isAllType) {
							string pathSaveTo = 
								appContentListItem.data.GetFilePaths().
								pathNonVersionedSystem;
							FileSystemUtil.CopyFile(
								pathSave, 
								pathSaveTo, true);
						}
						else {
							string pathSaveTo = 
								appContentListItem.data.GetFilePaths().
								pathVersionedSystem;
							FileSystemUtil.CopyFile(
								pathSave, 
								pathSaveTo, true);									
						}
						
						FileSystemUtil.RemoveFile(pathSave);
					}										
						//}
					//}		
					*/
                }
            }
        }
        else {
            FileSystemUtil.RemoveFile(pathSave);
        }
    }

    void handleRequestDownloadBytesCallback(
        byte[] responseBytes) {

        //bool serverError = false;

        if(responseBytes != null) {

            handleSyncedFileBinary(responseBytes, currentUrlObject);

            Messenger<object>.Broadcast(
                ContentMessages.ContentAppContentListFileDownloadSuccess,
                    "Download Success " + currentUrlObject.file_key);

        }
        else {
            //serverError = true;			

            Messenger<object>.Broadcast(
                ContentMessages.ContentAppContentListFileDownloadError,
                    "Download Error");
        }
    }

    public static void RequestDownloadBytes(string url) {
        if(isInst) {
            Instance.requestDownloadBytes(url);
        }
    }

    public void requestDownloadBytes(string url) {

        Messenger<object>.Broadcast(
            ContentMessages.ContentAppContentListFileDownloadStarted,
                "Download Started");
        WebRequests.Instance.RequestBytes(url, handleRequestDownloadBytesCallback);
    }

    // ----------------------------------------------------------------------------------
    // FILE SETS


    // public void StartContentSystem() {
    //
    // }

    //public void LoadContentSet() {
    //LogUtil.Log("Contents::LoadContentSet");
    //}       
    // INIT and PREPARE

    //public void InitCache() {
    //    SyncFolders();
    //}

    public static void BroadcastProgressMessage(string title, string description, float progress) {
        if(isInst) {
            Instance.broadcastProgressMessage(title, description, progress);
        }
    }

    public void broadcastProgressMessage(string title, string description, float progress) {
        Messenger<string, string, float>.Broadcast(ContentMessages.ContentProgressMessage,
            title,
            description,
            progress);

        LogUtil.Log("BroadcastProgressMessage:"
            , " progress:" + progress.ToString("P0")
            + " title:" + title
            + " description:" + description);

        //AppViewerUIPanelLoading.ShowProgress(title, description, progress);
    }

    public static void InitCache(bool syncFolders, bool syncServer) {
        if(isInst) {
            Instance.initCache(syncFolders, syncServer);
        }
    }
    public void initCache(bool syncFolders, bool syncServer) {
        StartCoroutine(initCacheCo(syncFolders, syncServer));
    }

    //public static IEnumerator InitCacheCo(bool syncFolders, bool syncServer) {
    //	if(isInst) {
    //		yield return Instance.StartCoroutine(Instance.initCacheCo(syncFolders, syncServer));
    //	}
    //}

    public IEnumerator initCacheCo(bool syncFolders, bool syncServer) {

        LogUtil.Log("initCacheCo:", " syncFolders:" + syncFolders + " syncServer:" + syncServer);

        broadcastProgressMessage(
            "Loading Content",
            "Syncing initial content...",
            1f);

        // Initial cache
        yield return StartCoroutine(syncFoldersCo(syncFolders, syncServer));

        // Get latest main content list from server

        // Sync any files missing from latest

        // Check that all content states have icons downloaded

        //yield break;
    }

    IEnumerator downloadLatestContentList() {

        // Get and check the md5 hash of main content list		

        yield break;
    }

    public static string GetFileDataFromPersistentCache(
        string path, bool versioned, bool absolute) {
        if(isInst) {
            return Instance.getFileDataFromPersistentCache(path, versioned, absolute);
        }
        return "";
    }

    public string getFileDataFromPersistentCache(
        string path, bool versioned, bool absolute) {

        string fileData = "";
        string pathPart = path;

        string pathToCopy = "";
        pathToCopy = PathUtil.Combine(
            ContentPaths.appShipCacheVersionPath, pathPart);

        if(!absolute) {
            path = PathUtil.Combine(
                ContentPaths.appCacheVersionPath, path);
            //shipPath = PathUtil.Combine(ContentPaths.appShipCacheVersionPath, path);
        }
        string pathVersioned = path;

        if(versioned) {
            pathVersioned = Contents.GetFullPathVersioned(pathVersioned);
        }

        //LogUtil.Log("GetFileDataFromPersistentCache:path:" + path);
        //LogUtil.Log("GetFileDataFromPersistentCache:pathVersioned:" + pathVersioned );
        //LogUtil.Log("GetFileDataFromPersistentCache:pathToCopy:" + pathToCopy );
        //LogUtil.Log("GetFileDataFromPersistentCache:ContentPaths.appShipCacheVersionPath:" + ContentPaths.appShipCacheVersionPath );

        //LogUtil.Log("GetFileDataFromPersistentCache:ContentPaths.appCacheVersionPath:" + ContentPaths.appCacheVersionPath );

        bool versionedExists = FileSystemUtil.CheckFileExists(pathVersioned);

        //LogUtil.Log("GetFileDataFromPersistentCache:versionedExists:" + versionedExists.ToString());
        //LogUtil.Log("GetFileDataFromPersistentCache:absolute:" + absolute);

        if(!versionedExists && !absolute) {
            // copy from streaming assets	
            bool shipExists = FileSystemUtil.CheckFileExists(pathToCopy);
            if(shipExists) {
                FileSystemUtil.CopyFile(pathToCopy, pathVersioned);
            }
            else {
                return "";
            }
        }
        fileData = FileSystemUtil.ReadString(pathVersioned);
        return fileData;
    }

    public enum ContentStorageLocation {
        RESOURCES,
        STREAMING_ASSETS,
        PERSISTENT,
        WEB_SERVICE
    }

    public static IEnumerator SyncContentListItemDataCo(
        ContentStorageLocation locationFrom,
        ContentStorageLocation locationTo) {
        if(isInst) {
            yield return Instance.StartCoroutine(Instance.syncContentListItemDataCo(locationFrom, locationTo));
        }
        else {
            yield break;
        }
    }

    public IEnumerator syncContentListItemDataCo(
        ContentStorageLocation locationFrom,
        ContentStorageLocation locationTo) {
        // Syncs content list item data from streaming assets to persistent

        LogUtil.Log("SyncContentListItemData: locationFrom" +
            locationFrom.ToString() + " locationTo:" + locationTo.ToString());

        List<AppContentListItem> contentListItems = AppContentListItems.Instance.GetAll();
        int totalItems = contentListItems.Count;
        float inc = 0f;

        yield return new WaitForEndOfFrame();

        foreach(AppContentListItem contentItem in contentListItems) {

            string fileFrom = PathUtil.Combine(
                Application.streamingAssetsPath,
                contentItem.data.filePathNonVersioned);

            LogUtil.Log("SyncContentListItemData: " +
                "\r\n  fileFrom:" + fileFrom);

            float progressCount = inc++ / totalItems;

            broadcastProgressMessage(
                "Preparing Content",
                "Syncing file:" + getUnversionedDisplayFile(contentItem.data.fileNoExtension),
                progressCount);

            yield return new WaitForEndOfFrame();

            if(FileSystemUtil.CheckFileExists(fileFrom)) {
                string fileTo = PathUtil.Combine(
                    Application.persistentDataPath,
                    contentItem.data.filePathVersioned);

                //if(contentItem.data.versioned) {
                //	fileTo = Contents.GetFileVersioned(fileTo);
                //}

                LogUtil.Log("SyncContentListItemData: " +
                    "\r\n  fileFrom:" + fileFrom +
                    "\r\n  fileTo:" + fileTo);

                broadcastProgressMessage(
                    "Preparing Content",
                    "Syncing file:" + getUnversionedDisplayFile(contentItem.data.fileNoExtension),
                    progressCount);

                yield return new WaitForEndOfFrame();

                if(!FileSystemUtil.CheckFileExists(fileTo)) {
                    LogUtil.Log("SyncContentListItemData: Copying File: fileTo:" + fileTo);

                    broadcastProgressMessage(
                        "Preparing Content",
                        "Copying file:" + getUnversionedDisplayFile(contentItem.data.fileNoExtension),
                        progressCount);

                    yield return new WaitForEndOfFrame();

                    FileSystemUtil.CopyFile(fileFrom, fileTo);
                }
            }

        }

        broadcastProgressMessage(
            "Preparing Content",
            "Sync Complete",
            1f);

        yield return new WaitForEndOfFrame();
    }


    public static IEnumerator DirectoryCopyCo(
        string sourceDirName, string destDirName,
        bool copySubDirs, bool versioned) {
        if(isInst) {
            yield return Instance.StartCoroutine(Instance.directoryCopyCo(
                sourceDirName, destDirName, copySubDirs, versioned));
        }
        else {
            yield break;
        }

    }

    public IEnumerator directoryCopyCo(
        string sourceDirName, string destDirName,
        bool copySubDirs, bool versioned) {


#if !UNITY_WEBPLAYER
        LogUtil.Log("DirectoryCopy:" +
            " sourceDirName:" + sourceDirName +
            " destDirName:" + destDirName +
            " copySubDirs:" + copySubDirs +
            " versioned:" + versioned);

        FileSystemUtil.EnsureDirectory(sourceDirName, false);
        FileSystemUtil.EnsureDirectory(destDirName, false);

        FileSystemUtil.CreateDirectoryIfNeededAndAllowed(sourceDirName);

        bool sourceDirExists = Directory.Exists(sourceDirName);
        //bool sourceDirExists2 = Directory.Exists(sourceDirName.Replace("jar:",""));
        //bool sourceDirExists3 = Directory.Exists(sourceDirName.Replace("jar:file://",""));

        //LogUtil.Log("DirectoryCopy:" + 
        //	" sourceDirExists:" + sourceDirExists.ToString() + 
        //	" sourceDirExists2:" + sourceDirExists2.ToString() + 
        //	" sourceDirExists3:" + sourceDirExists3.ToString());

        if(sourceDirExists) {

            DirectoryInfo dir = new DirectoryInfo(sourceDirName);
            DirectoryInfo[] dirs = dir.GetDirectories();

            if(!dir.Exists) {
                throw new DirectoryNotFoundException(
                    "Source directory does not exist or could not be found: "
                    + sourceDirName);
            }

            FileSystemUtil.CreateDirectoryIfNeededAndAllowed(destDirName);

            broadcastProgressMessage(
                "Preparing Content",
                "Syncing directory: " + getUnversionedDisplayFile(destDirName),
                1f);

            FileInfo[] files = dir.GetFiles();

            broadcastProgressMessage(
                "Preparing Content",
                "Syncing files in: " + getUnversionedDisplayFile(destDirName),
                0f);


            //LogUtil.Log(">>>> Directory Files: directory: " + destDirName);
            //LogUtil.Log(">>>> files.Count:", files.Length);

            int curr = 0;

            foreach(FileInfo file in files) {
                if(file.Extension != ".meta"
                    && file.Extension != ".DS_Store") {

                    string temppath = PathUtil.Combine(destDirName, file.Name);

                    if(versioned) {
                        temppath = getFullPathVersioned(temppath);
                    }

                    if(!FileSystemUtil.CheckFileExists(temppath) || Application.isEditor) {

                        LogUtil.Log("--copying ship file: " + file.FullName);
                        LogUtil.Log("--copying ship file to cache: " + temppath);

                        FileSystemUtil.CopyFile(file.FullName, temppath);
                    }

                    yield return new WaitForEndOfFrame();

                    broadcastProgressMessage(
                        "Preparing Content",
                        "Syncing files - " + getUnversionedDisplayFile(file.Name),
                        curr++ / files.Length);
                }

            }

            if(copySubDirs) {

                foreach(DirectoryInfo subdir in dirs) {
                    string temppath = PathUtil.Combine(destDirName, subdir.Name);
                    LogUtil.Log("Copying Directory: " + temppath);
                    yield return StartCoroutine(directoryCopyCo(subdir.FullName, temppath, copySubDirs, versioned));
                }
            }
        }
#else
		yield break;
#endif
    }

    //public static IEnumerator SyncFoldersCo(bool syncFolders, bool syncServer) {
    //	if(isInst) {
    //		yield return Instance.StartCoroutine(Instance.syncFoldersCo(syncFolders, syncServer));
    //	}
    //	else {
    //		yield break;
    //	}
    //}

    public IEnumerator syncFoldersCo(bool syncFolders, bool syncServer) {


        LogUtil.Log("Contents::syncFoldersCo:", " syncFolders:" + syncFolders + " syncServer:" + syncServer);

        yield return new WaitForEndOfFrame();

        Messenger<object>.Broadcast(ContentMessages.ContentSyncShipContentStarted, "started");

        LogUtil.Log("Contents::SyncFolders");

        ContentPaths.CreateCachePaths();

        LogUtil.Log("Contents::syncFoldersCo:Folders Created:", " syncFolders:" + syncFolders + " syncServer:" + syncServer);

        if(syncFolders) {
            yield return StartCoroutine(directoryCopyCo(ContentPaths.appShipCachePlatformPath, ContentPaths.appCachePlatformPath, true, true));
            //DirectoryCopy(appShipCachePathPacks, ContentPaths.appCachePathPacks, true);
            yield return StartCoroutine(directoryCopyCo(ContentPaths.appShipCachePathData, ContentPaths.appCachePathData, true, true));
            yield return StartCoroutine(directoryCopyCo(ContentPaths.appShipCachePathShared, ContentPaths.appCachePathShared, true, true));
            yield return StartCoroutine(directoryCopyCo(ContentPaths.appShipCachePathAll, ContentPaths.appCachePathAll, true, false));  // files in all/shared are not versioned...
        }

        if(syncServer) {
            yield return StartCoroutine(syncContentListItemDataCo(
                ContentStorageLocation.STREAMING_ASSETS,
                ContentStorageLocation.PERSISTENT));
        }

        broadcastProgressMessage(
            "Preparing Content",
            "Ship files sync complete",
            1f);

        Messenger<object>.Broadcast(ContentMessages.ContentSyncShipContentSuccess, "success");

        ////
    }

    public static string GetFullPathVersioned(string fullPath) {
        if(isInst) {
            return Instance.getFullPathVersioned(fullPath);
        }
        return fullPath;
    }

    public string getFullPathVersioned(string fullPath) {
        //string fileHash = ChecksumHash(fullPath);
        //return GetFileVersioned(fullPath, fileHash);
        return getFileVersioned(fullPath, null);
    }

    //public string GetFullPathVersioned(string hashPath, string pathToVersion) {
    //if(FileSystemUtil.CheckFileExists(hashPath)) {
    //	string fileHash = ChecksumHash(hashPath);
    //	return GetFileVersioned(pathToVersion, fileHash);
    //}
    //else {
    //	return hashPath;
    //}
    //return GetFileVersioned(pathToVersion, null);
    //}

    public static string GetFullPathVersionedSync(string fullPath) {
        if(isInst) {
            return Instance.getFullPathVersionedSync(fullPath);
        }
        return fullPath;
    }

    public string getFullPathVersionedSync(string fullPath) {
        string fileHash = ChecksumHash(fullPath);
        return GetFileVersioned(fullPath, fileHash);
    }

    public static string ChecksumHash(string fullPath) {
        if(isInst) {
            return Instance.checksumHash(fullPath);
        }
        return "";
    }

    public string checksumHash(string fullPath) {
        return CryptoUtil.CalculateMD5HashFromFile(fullPath);
    }

    public static string GetFullPathVersionedSync(string hashPath, string pathToVersion) {
        if(isInst) {
            return Instance.getFullPathVersionedSync(hashPath, pathToVersion);
        }
        return "";
    }

    public string getFullPathVersionedSync(string hashPath, string pathToVersion) {
        if(FileSystemUtil.CheckFileExists(hashPath)) {
            string fileHash = ChecksumHash(hashPath);
            return GetFileVersioned(pathToVersion, fileHash);
        }
        else {
            return hashPath;
        }
    }

    public static string GetFileVersioned(string path) {
        if(isInst) {
            return Instance.getFileVersioned(path);
        }
        return path;
    }

    public string getFileVersioned(string path) {
        return GetFileVersioned(path, null);
    }

    public static string GetFileVersioned(string path, string hash) {
        if(isInst) {
            return Instance.getFileVersioned(path, hash);
        }
        return path;
    }

    public string getFileVersioned(string path, string hash) {
        string fileVersioned = "";
        if(!string.IsNullOrEmpty(path)) {
            string[] arrpath = path.Split('/');

            fileVersioned = path;

            if(arrpath != null) {
                string filepart = arrpath[arrpath.Length - 1];
                string arttpathrest = path.Replace(filepart, "");
                string[] arrfilepart = filepart.Split('.');
                string ext = arrfilepart[arrfilepart.Length - 1];
                string filepartbare = filepart.Replace("." + ext, "");

                string appVersion = ContentsConfig.contentVersion.Replace(".", "-");
                string appIncrement = ContentsConfig.contentIncrement.ToString();

                if(!string.IsNullOrEmpty(hash)) {
                    fileVersioned = PathUtil.Combine(arttpathrest, filepartbare
                        + "-" + appVersion
                        + "-"
                        + appIncrement
                        + "-" + hash
                        + "." + ext);
                }
                else {
                    fileVersioned = PathUtil.Combine(arttpathrest, filepartbare
                        + "-" + appVersion
                        + "-"
                        + appIncrement
                        + "." + ext);
                }
            }
        }

        return fileVersioned;
    }

    public static string GetDisplayFileUnversioned(string path) {
        if(isInst) {
            return Instance.getDisplayFileUnversioned(path);
        }
        return path;
    }

    public string getDisplayFileUnversioned(string path) {
        string fileVersioned = path;
        if(!string.IsNullOrEmpty(path)) {
            string[] arrpath = path.Split('/');

            fileVersioned = path;

            if(arrpath != null) {

                string appVersion = ContentsConfig.contentVersion.Replace(".", "-");
                string appIncrement = ContentsConfig.contentIncrement.ToString();
                string versionAppend = "-" + appVersion
                        + "-"
                        + appIncrement;
                if(fileVersioned.Contains(versionAppend)) {
                    fileVersioned = fileVersioned.Substring(0, fileVersioned.IndexOf(versionAppend));
                }
            }
        }

        return fileVersioned;
    }

    public static string GetFileUnversioned(string path) {
        if(isInst) {
            return Instance.getFileUnversioned(path);
        }
        return path;
    }

    public string getFileUnversioned(string path) {
        return GetFileUnversioned(path, null);
    }

    public static string GetFileUnversioned(string path, string hash) {
        if(isInst) {
            return Instance.getFileUnversioned(path, hash);
        }
        return path;
    }

    public string getFileUnversioned(string path, string hash) {
        string fileVersioned = path;
        if(!string.IsNullOrEmpty(path)) {
            string[] arrpath = path.Split('/');

            fileVersioned = path;

            if(arrpath != null) {

                string appVersion = ContentsConfig.contentVersion.Replace(".", "-");
                string appIncrement = ContentsConfig.contentIncrement.ToString();

                if(!string.IsNullOrEmpty(hash)) {

                    fileVersioned = fileVersioned.Replace(
                        "-" + appVersion
                        + "-"
                        + appIncrement
                        + "-" + hash, "");
                }
                else {
                    fileVersioned = fileVersioned.Replace(
                        "-" + appVersion
                        + "-"
                        + appIncrement, "");
                }
            }
        }

        return fileVersioned;
    }

    // SCENE / CONTENT SET FILES

    public static void CheckContentSetFiles() {
        if(isInst) {
            Instance.checkContentSetFiles();
        }
    }

    public void checkContentSetFiles() {
        //LogUtil.Log("Contents::CheckContentSetFiles");            

        //SyncFolders();
    }

    //public static IEnumerator SceneLoadFromCacheOrDownloadCo(string url) {
    //	if(isInst) {
    //		yield return Instance.StartCoroutine(Instance.sceneLoadFromCacheOrDownloadCo(url));
    //	}
    //	else {
    //		yield break;
    //	}
    //}

    public IEnumerator sceneLoadFromCacheOrDownloadCo(string url) {

        unloadLevelBundle();

        int version = GamePacks.currentPacksIncrement;
        string packName = "";//GamePacks.PACK_BOOK_DEFAULT;
        string sceneName = "";//GameLevels.Current.name;

        LogUtil.Log("SceneLoadFromCacheOrDownloadCo: packName:" + packName);
        LogUtil.Log("SceneLoadFromCacheOrDownloadCo: sceneName:" + sceneName);
        LogUtil.Log("SceneLoadFromCacheOrDownloadCo: version:" + version);

        ContentItem contentItem = new ContentItem();

        contentItem.uid = sceneName; // hash this
        contentItem.name = sceneName;
        contentItem.version = version;

        //bool isDlc = false;
        bool ready = true;

        if(IsDownloadableContent(packName)) {

            LogUtil.Log("SceneLoadFromCacheOrDownloadCo: " + packName);

            LogUtil.Log("SceneLoadFromCacheOrDownloadCo: " + url);

            Messenger<string>.Broadcast(
                ContentMessages.ContentItemDownloadStarted, url);

            downloadInProgress = true;

            downloader = WWW.LoadFromCacheOrDownload(url, version);

            LogUtil.Log("downloader.progress: " + downloader.progress);

            yield return downloader;

            LogUtil.Log("downloader.progress2: " + downloader.progress);

            // Handle error
            if(downloader.error != null) {
                LogUtil.LogError("Error downloading");
                LogUtil.LogError(downloader.error);
                LogUtil.LogError(url);
                ready = false;
                reset();
                Messenger<string>.Broadcast(
                    ContentMessages.ContentItemDownloadError,
                    downloader.error);
            }
            else {

                // In order to make the scene available from LoadLevel, we have to load the asset bundle.
                // The AssetBundle class also lets you force unload all assets and file storage once it 
                // is no longer needed.

                Messenger<string>.Broadcast(
                    ContentMessages.ContentItemPrepareStarted,
                    "Content preparing...");

                UnloadLevelBundle();

                bundle = downloader.assetBundle;

                LogUtil.Log("LoadLevel" + sceneName);

                SetLastPackState(packName, url);

                Messenger<string>.Broadcast(
                    ContentMessages.ContentItemPrepareSuccess,
                    "Content prepared...");
                // Load the level we have just downloaded
            }
        }

        if(ready) {
            //GameLoadingObject.Instance.LoadLevelHandler();
            reset();
        }
        else {
            // Show download error...
            Messenger<string>.Broadcast(
                ContentMessages.ContentItemDownloadError,
                "Error unloading pack, please try again.");
            reset();
        }
    }

    public static void LoadLevelBundle(string pack, int increment) {
        if(isInst) {
            Instance.loadLevelBundle(pack, increment);
        }
    }

    public void loadLevelBundle(string pack, int increment) {
        string pathPack = PathUtil.Combine(ContentPaths.appCachePathAllPlatformPacks, pack);
        pathPack = PathUtil.Combine(pathPack, ContentConfig.contentCacheScenes);

        GamePacks.Instance.ChangeCurrentGamePack(pack);
#if !UNITY_WEBPLAYER
        if(Directory.Exists(pathPack)) {
            string pathUrl = PathUtil.Combine(
                pathPack, pack + "-" + increment.ToString() + ".unity3d");

            if(FileSystemUtil.CheckFileExists(pathUrl)) {
                LoadLevelBundle("file://" + pathUrl);
            }
            else {
                //LogUtil.Log("Pack file does not exist: " + pathUrl);
            }
        }
        else {
            //LogUtil.Log("Pack does not exist:" + pathPack);
        }
#endif
    }

    public static void LoadLevelBundle(string sceneUrl) {
        if(isInst) {
            Instance.loadLevelBundle(sceneUrl);
        }
    }

    public void loadLevelBundle(string sceneUrl) {
        StartCoroutine(loadLevelBundleCo(sceneUrl));
    }

    //public static IEnumerator LoadLevelBundleCo(string sceneUrl) {
    //	if(isInst) {
    //		yield return Instance.StartCoroutine(Instance.loadLevelBundleCo(sceneUrl));
    //	}
    //	else {
    //		yield break;
    //	}
    //}

    public IEnumerator loadLevelBundleCo(string sceneUrl) {

        bool ready = true;

        downloader = new WWW(sceneUrl);

        downloadInProgress = true;

        LogUtil.Log("downloader.progress: " + downloader.progress);

        yield return downloader;

        LogUtil.Log("downloader.progress2: " + downloader.progress);

        // Handle error
        if(downloader.error != null) {
            LogUtil.LogError("Error downloading");
            LogUtil.LogError(downloader.error);
            LogUtil.LogError(sceneUrl);
            ready = false;
            Reset();
            Messenger<string>.Broadcast(
                ContentMessages.ContentItemDownloadError,
                downloader.error);
        }
        else {

            // In order to make the scene available from LoadLevel, we have to load the asset bundle.
            // The AssetBundle class also lets you force unload all assets and file storage once it 
            // is no longer needed.

            Messenger<string>.Broadcast(
                ContentMessages.ContentItemPrepareStarted,
                "Content preparing...");

            UnloadLevelBundle();

            bundle = downloader.assetBundle;

            //LogUtil.Log("LoadLevel" + sceneName);

            SetLastPackState(GamePacks.Current.code, sceneUrl);

            downloadInProgress = false;
            string sceneName = GamePacks.Current.code + "-main";

            //AsyncOperation asyncLoad = Application.LoadLevelAsync(sceneName);
            Context.Current.ApplicationLoadLevelByName(sceneName);

            Messenger<string>.Broadcast(
                ContentMessages.ContentItemPrepareSuccess,
                "Content prepared...");
        }

        LogUtil.Log("ready:" + ready);
    }

    public static void UnloadLevelBundle(bool unloadAll) {
        if(isInst) {
            Instance.unloadLevelBundle(unloadAll);
        }
    }

    public void unloadLevelBundle(bool unloadAll) {
        if(bundle != null) {
            bundle.Unload(unloadAll);
        }
    }

    public static void UnloadLevelBundle() {
        if(isInst) {
            Instance.unloadLevelBundle();
        }
    }

    public void unloadLevelBundle() {
        unloadLevelBundle(false);
    }

    public static void Reset() {
        if(isInst) {
            Instance.reset();
        }
    }

    public void reset() {
        downloader = null;
        contentItemStatus = new ContentItemStatus();
        downloadInProgress = false;
    }

    public static ContentItemStatus ProgressStatus() {
        if(isInst) {
            return Instance.progressStatus();
        }
        return null;
    }

    public ContentItemStatus progressStatus() {

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
        public static IEnumerator SceneLoadFromCacheOrDownloadCo(
        string packName, string sceneName) {

            int version = GamePacks.currentPacksIncrement;
            string url = "https://s3.amazonaws.com/game-[app]/1.1/ios/" + packName + ".unity3d";

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