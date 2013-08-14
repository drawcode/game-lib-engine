using UnityEngine;
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
			if(Application.internetReachability != NetworkReachability.NotReachable) {
				return true;
			}	
			return false;
		}		
	}
	
	public bool hasConnectionViaLocalNetwork {
		get {
			if(Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork) {
				return true;
			}	
			return false;
		}		
	}

	public bool hasConnectionViaCarrierNetwork {
		get {
			if(Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork) {
				return true;
			}	
			return false;
		}		
	}
	
	// PLATFORM
	
	public bool isWeb {
		get {
			return isWebMac || isWebWindows;
		}
	}
	
	public bool isWebMac {
		get {
			return Application.platform == RuntimePlatform.OSXWebPlayer;
		}
	}
	
	public bool isWebWindows {
		get {
			return Application.platform == RuntimePlatform.WindowsWebPlayer;
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
	
	public bool isEditorWindows{
		get {
			return Application.platform == RuntimePlatform.WindowsEditor;
		}
	}
	
	public bool isGoogleNativeClient {
		get {
			return Application.platform == RuntimePlatform.NaCl;
		}
	}
	
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
			return isConsolePS3  || isConsoleXbox360 || isConsoleWii;
		}
	}
	
	public bool isConsolePS3 {
		get {
			return Application.platform == RuntimePlatform.PS3;
		}
	}
	
	public bool isConsoleXbox360 {
		get {
			return Application.platform == RuntimePlatform.XBOX360;
		}
	}
	
	public bool isConsoleWii {
		get {
			return Application.platform == RuntimePlatform.WiiPlayer;
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
			
			//return Path.Combine(AppPersistencePath, "StreamingAssets");
			*/
			
		}
	}
	
	public string ApplicationUrl() {
		if(isWeb) {
			return Application.absoluteURL;
		}
		else {
			return "";
		}
	}

	public int ApplicationTotalLevels() {
		return Application.levelCount;
	}

	public int ApplicationLoadedLevelNumber() {
		return Application.loadedLevel;
	}

	public string ApplicationLoadedLevelName() {
		return Application.loadedLevelName;
	}

	public void ApplicationLoadLevelByNumber(int levelNumber) {
		Application.LoadLevel(levelNumber);
	}

	public void ApplicationLoadLevelByName(string levelName) {
		Application.LoadLevel(levelName);
	}

	public void ApplicationStreamLevelByNumber(int levelNumber) {
		Application.LoadLevelAdditive(levelNumber);
	}

	public void ApplicationStreamLevelByName(string levelName) {
		Application.LoadLevelAdditive(levelName);
	}

	public void ApplicationOpenURL(string url) {
		Application.OpenURL(url);
	}

	public int ApplicationStreamedBytes() {
		return Application.streamedBytes;
	}
}