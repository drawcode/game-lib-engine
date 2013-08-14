using System;
using System.IO;
using UnityEngine;

public class PathUtil {

    /// <summary>
    /// Path to the applications root directory
    /// </summary>
    ///
    public static string AppRootPath {
        get {
            if (Application.platform == RuntimePlatform.IPhonePlayer)
                return GetAppRootPathIPhone();
            else
                return GetAppRootPath();
        }
    }

    /// <summary>
    /// Path to the applications data directory
    /// </summary>
    ///
    public static string AppDataPath {
        get {
            return Application.dataPath;
        }
    }

    /// <summary>
    /// Path to the applications content data directory
    /// </summary>
    ///
    public static Uri AppContentDataUri {
        get {
            if (Application.platform == RuntimePlatform.IPhonePlayer) {
                var uriBuilder = new UriBuilder();
                uriBuilder.Scheme = "file";
                uriBuilder.Path = Path.Combine(AppDataPath, "Raw");
                return uriBuilder.Uri;
            }
            else if (Application.platform == RuntimePlatform.Android) {
                return new Uri("jar:file://" + Application.dataPath + "!/assets");
            }
            else {
                var uriBuilder = new UriBuilder();
                uriBuilder.Scheme = "file";
                uriBuilder.Path = Path.Combine(AppDataPath, "StreamingAssets");
                return uriBuilder.Uri;
            }
        }
    }

    /// <summary>
    /// Path to the directory the application is allowed
    /// to save data in
    /// </summary>
    ///
    public static string AppPersistencePath {
        get {
            if (Application.platform == RuntimePlatform.IPhonePlayer)
                return GetAppPersistencePathIPhone();
            else
                return GetAppPersistencePath();
        }
    }

    /// <summary>
    /// Path to the directory the application is allowed
    /// to save content data in
    /// </summary>
    ///
    public static string AppContentPersistencePath {
        get {
            return Context.Current.ApplicationStreamingAssetsPath;
        }
    }

    /*
     * Standard path management functions which work on the following premise
     *
     * The app has read+write access to its install folders and therefore
     * can save data wherever it wants
     */

    /// <summary>
    /// The standard application root path
    /// </summary>
    ///
    public static string GetAppRootPath() {

        //Strip off the /Data from Application.dataPath
        return Path.GetFileName(Application.dataPath);
    }

    /// <summary>
    /// Returns the standard application persistence path
    /// </summary>
    ///
    public static string GetAppPersistencePath() {
        //return Context.Current.ApplicationPersistentPath;
		return Application.persistentDataPath;
    }

    /* iPhone path management functions which work on the following premise:
     *
     * The app has read+write access to
     *
     * 		/var/mobile/Applications/XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX/Documents
     *
     * Unity's Application.dataPath returns
     *
     * 		/var/mobile/Applications/XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX/myappname.app/Data
     */

    /// <summary>
    /// Returns the iPhone application's root path
    /// </summary>
    public static string GetAppRootPathIPhone() {

        // We need to strip "/Data" from Application.Data
        return Path.GetDirectoryName(Application.dataPath);
    }

    /// <summary>
    /// Returns the iPhone application's sandbox path
    /// (the path that the application is installed in)
    /// </summary>
    ///
    public static string GetAppSandboxPathIPhone() {

        // We need to strip "/myappname.app/Data"
        return Path.GetDirectoryName(Path.GetDirectoryName(Application.dataPath));
    }

    /// <summary>
    /// Returns the iPhone application's document path
    /// </summary>
    ///
    public static string GetAppPersistencePathIPhone() {
        return Path.Combine(GetAppSandboxPathIPhone(), "Documents");
    }
}