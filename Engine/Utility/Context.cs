using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class Context {

    private static volatile Context instance;
    private static System.Object syncRoot = new System.Object();

    public static Context Current {
        get {
            if (instance == null) {
                lock (syncRoot) {
                    if (instance == null)
                        instance = new Context();
                }
            }
            return instance;
        }
    }

    // NETWORK

    public bool hasNetworkAccess {
        get {
            if (Application.internetReachability != NetworkReachability.NotReachable) {
                return true;
            }
            return false;
        }
    }

    public bool hasConnectionViaLocalNetwork {
        get {
            if (Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork) {
                return true;
            }
            return false;
        }
    }

    public bool hasConnectionViaCarrierNetwork {
        get {
            if (Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork) {
                return true;
            }
            return false;
        }
    }

    // PLATFORM

    public bool isWeb {
        get {
#if UNITY_WEBPLAYER
            return true;
#else
            return false;
#endif
        }
    }

    public bool isWebGL {
        get {
            return Application.platform == RuntimePlatform.WebGLPlayer;
        }
    }

    public bool isMobile {
        get {
            return isMobileiOS || isMobileAndroid;
        }
    }

    public bool isMobileAndroid {
        get {
            return Application.platform == RuntimePlatform.Android;
        }
    }

    public bool isMobileiOS {
        get {
            return Application.platform == RuntimePlatform.IPhonePlayer;
        }
    }

    public bool isEditor {
        get {
            return isEditorMac || isEditorWindows;
        }
    }

    public bool isEditorMac {
        get {
            return Application.platform == RuntimePlatform.OSXEditor;
        }
    }

    public bool isEditorWindows {
        get {
            return Application.platform == RuntimePlatform.WindowsEditor;
        }
    }

    //public bool isGoogleNativeClient {
    //	get {
    //		return Application.platform == RuntimePlatform.NaCl;
    //	}
    //}

    public bool isDesktop {
        get {
            return isDesktopMac || isDesktopWindows || isDesktopLinux;
        }
    }

    public bool isDesktopMac {
        get {
            return Application.platform == RuntimePlatform.OSXPlayer;
        }
    }

    public bool isDesktopWindows {
        get {
            return Application.platform == RuntimePlatform.WindowsPlayer;
        }
    }

    public bool isDesktopLinux {
        get {
            return Application.platform == RuntimePlatform.LinuxPlayer;
        }
    }

    public bool isConsole {
        get {
            return isConsolePS4 || isConsoleXboxOne;
        }
    }

    public bool isConsolePS4 {
        get {
            return Application.platform == RuntimePlatform.PS4;
        }
    }

    public bool isConsoleXboxOne {
        get {
            return Application.platform == RuntimePlatform.XboxOne;
        }
    }

    public RuntimePlatform ApplicationPlatform() {
        //if (Application.platform == RuntimePlatform.WindowsPlayer)
        return Application.platform;
    }

    public string ApplicationDataPath() {
        return Application.dataPath;
    }

    public string ApplicationStreamingAssetsPath {
        get {

            return Application.streamingAssetsPath;


            // OLD SKOOL
            /*
			  path = = Application.dataPath + "/StreamingAssets";
			 On iOS, you should use:-
			
			  path = Application.dataPath + "/Raw";
			...while on Android, you should use:-
			
			  path = "jar:file://" + Application.dataPath + "!/assets/";
			string path = Application.dataPath + "/StreamingAssets";
			
			if(Context.Current.isMobileiOS) {
				path = Application.dataPath + "/Raw";
			}
			else if(Context.Current.isMobileAndroid) {
				path = "jar:file://" + Application.dataPath + "!/assets/";
			}
			
			return path;
			
			//return PathUtil.Combine(AppPersistencePath, "StreamingAssets");
			*/

        }
    }

    public string ApplicationUrl() {
        if (isWeb) {
            return Application.absoluteURL;
        }
        else {
            return "";
        }
    }

    public int ApplicationTotalLevels() {
        return UnityEngine.SceneManagement.SceneManager.sceneCount;
    }

    public int ApplicationLoadedLevelNumber() {
        return UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex;
    }

    public string ApplicationLoadedLevelName() {
        return UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
    }

    public void ApplicationLoadLevelByNumber(int levelNumber) {
        UnityEngine.SceneManagement.SceneManager.LoadScene(levelNumber);
    }

    public void ApplicationLoadLevelByName(string levelName) {
        UnityEngine.SceneManagement.SceneManager.LoadScene(levelName);
    }

    public void ApplicationStreamLevelByNumber(int levelNumber) {
        UnityEngine.SceneManagement.SceneManager.LoadScene(levelNumber, LoadSceneMode.Additive);
    }

    public void ApplicationStreamLevelByName(string levelName) {
        UnityEngine.SceneManagement.SceneManager.LoadScene(levelName, LoadSceneMode.Additive);
    }

    public void ApplicationOpenURL(string url) {
        Application.OpenURL(url);
    }

    //[System.Obsolete]
    //public int ApplicationStreamedBytes() {
    //	return Application.streamedBytes;
    //}
}