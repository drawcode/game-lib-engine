using System;
using System.Collections.Generic;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

public class LogUtilKeyItem {
    public bool active = false;
    public string key = "";

    public LogUtilKeyItem(string keyValue, bool isActive) {
        key = keyValue;
        active = isActive;
    }
}

public class LogUtilKeys {
    public static LogUtilKeyItem keyAll = new LogUtilKeyItem("all", true);
    public static LogUtilKeyItem keyDefault = new LogUtilKeyItem("default", true);
    public static LogUtilKeyItem keyAlwaysLog = new LogUtilKeyItem("always", true);
    public static LogUtilKeyItem keyBikeAudio = new LogUtilKeyItem("bike-audio", true);
    public static LogUtilKeyItem keyProducts = new LogUtilKeyItem("products", true);
    public static LogUtilKeyItem keyAccess = new LogUtilKeyItem("access", true);
    public static LogUtilKeyItem keyStats = new LogUtilKeyItem("stats", true);
    public static LogUtilKeyItem keyAudio = new LogUtilKeyItem("audio", true);
    public static LogUtilKeyItem keyAds = new LogUtilKeyItem("ads", true);
    public static LogUtilKeyItem keyTools = new LogUtilKeyItem("tools", true);
    public static LogUtilKeyItem keyMode = new LogUtilKeyItem("mode", true);
}

public class LogUtil {
    public List<LogUtilKeyItem> logKeys = new List<LogUtilKeyItem>();

    private static volatile LogUtil instance;
    private static System.Object syncRoot = new System.Object();

    public static bool loggingEnabled = true;

    public static LogUtil Instance {
        get {
            if (instance == null) {
                lock (syncRoot) {
                    if (instance == null)
                        instance = new LogUtil();
                }
            }

            return instance;
        }
    }

    public LogUtil() {
    }

    public void LoadKeys() {
        if (logKeys == null) {
            logKeys = new List<LogUtilKeyItem>();
        }

        if (logKeys.Count == 0) {
            logKeys.Add(LogUtilKeys.keyAll);
            logKeys.Add(LogUtilKeys.keyDefault);
            logKeys.Add(LogUtilKeys.keyBikeAudio);
            logKeys.Add(LogUtilKeys.keyProducts);
            logKeys.Add(LogUtilKeys.keyStats);
            logKeys.Add(LogUtilKeys.keyAudio);
            logKeys.Add(LogUtilKeys.keyAlwaysLog);
            logKeys.Add(LogUtilKeys.keyAds);
            logKeys.Add(LogUtilKeys.keyTools);
            logKeys.Add(LogUtilKeys.keyMode);
            logKeys.Add(LogUtilKeys.keyAccess);
        }
    }

    public bool IsKeyActive(string key) {
        LoadKeys();
        if (LogUtilKeys.keyAll.active || key == LogUtilKeys.keyAlwaysLog.key) {
            foreach (LogUtilKeyItem item in logKeys) {
                if (item.key.ToLower() == key.ToLower()) {
                    if (item.active) {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    public void LogProductKeyInternal(object message) {
        LogInternal(LogUtilKeys.keyProducts.key, message);
    }

    public void LogAudioKeyInternal(object message) {
        LogInternal(LogUtilKeys.keyAudio.key, message);
    }

    public void LogBikeAudioKeyInternal(object message) {
        LogInternal(LogUtilKeys.keyBikeAudio.key, message);
    }

    public void LogDefaultKeyInternal(object message) {
        LogInternal(LogUtilKeys.keyDefault.key, message);
    }

    public void LogStatsKeyInternal(object message) {
        LogInternal(LogUtilKeys.keyStats.key, message);
    }

    public void LogAlwaysKeyInternal(object message) {
        LogInternal(LogUtilKeys.keyAlwaysLog.key, message);
    }

    public void LogAdsKeyInternal(object message) {
        LogInternal(LogUtilKeys.keyAds.key, message);
    }

    public void LogToolsKeyInternal(object message) {
        LogInternal(LogUtilKeys.keyTools.key, message);
    }

    public void LogModeKeyInternal(object message) {
        LogInternal(LogUtilKeys.keyMode.key, message);
    }

    public void LogAccessKeyInternal(object message) {
        LogInternal(LogUtilKeys.keyAccess.key, message);
    }

    public void LogInternal(string key, object message) {
        if (IsKeyActive(key)) {
            //Debug.Log(key + "::" + "\r\n\r\n" + message + "\r\n\r\n\r\n");
            Debug.Log(message);

            // Output to Firebug or inspectors as avail in the browser.
            if (Application.platform == RuntimePlatform.OSXWebPlayer
                || Application.platform == RuntimePlatform.WindowsWebPlayer) {
                if (message.GetType() == typeof(String))
                    Application.ExternalCall("if(console)console.log", message);
                else
                    Application.ExternalCall("if(console)console.log", message);
            }
        }
    }

    public void LogInternal(object message) {

        // Log default
        LogDefaultKeyInternal(message);
    }

    public void LogErrorInternal(object message) {
        Debug.LogError(message + "\r\n\r\n\r\n");

        // Output to Firebug or inspectors as avail in the browser.
        if (Application.platform == RuntimePlatform.OSXWebPlayer
            || Application.platform == RuntimePlatform.WindowsWebPlayer) {
            if (message.GetType() == typeof(String))
                Application.ExternalCall("if(console)console.error", message);
            else
                Application.ExternalCall("if(console)console.error", message);
        }
    }

    public static void Log(string key, object message) {

        if(!loggingEnabled) return;

        LogUtil.Instance.LogInternal(key + ":" + message);
    }

    public static void Log(object message) {
        
        if(!loggingEnabled) return;

        LogUtil.Instance.LogInternal(message);
    }

    public static void LogProduct(object message) {
        
        if(!loggingEnabled) return;

        LogUtil.Instance.LogProductKeyInternal(message);
    }

    public static void LogAudio(object message) {

        if(!loggingEnabled) return;

        LogUtil.Instance.LogAudioKeyInternal(message);
    }

    public static void LogBikeAudio(object message) {
        
        if(!loggingEnabled) return;

        LogUtil.Instance.LogBikeAudioKeyInternal(message);
    }

    public static void LogDefault(object message) {
        
        if(!loggingEnabled) return;

        LogUtil.Instance.LogDefaultKeyInternal(message);
    }

    public static void LogStats(object message) {
        
        if(!loggingEnabled) return;

        LogUtil.Instance.LogStatsKeyInternal(message);
    }

    public static void LogAlways(object message) {
        
        if(!loggingEnabled) return;

        LogUtil.Instance.LogAlwaysKeyInternal(message);
    }

    public static void LogAds(object message) {
        
        if(!loggingEnabled) return;

        LogUtil.Instance.LogAdsKeyInternal(message);
    }

    public static void LogError(object message) {
        
        if(!loggingEnabled) return;

        LogUtil.Instance.LogErrorInternal(message);
    }

    public static void LogTools(object message) {
        
        if(!loggingEnabled) return;

        LogUtil.Instance.LogToolsKeyInternal(message);
    }

    public static void LogMode(object message) {
        
        if(!loggingEnabled) return;

        LogUtil.Instance.LogModeKeyInternal(message);
    }

    public static void LogAccess(object message) {
        
        if(!loggingEnabled) return;

        LogUtil.Instance.LogAccessKeyInternal(message);
    }

    public void ListLoadedTextures() {
        UnityEngine.Object[] textures = Resources.FindObjectsOfTypeAll(typeof(Texture));

        string list = string.Empty;

        for (int i = 0; i < textures.Length; i++) {
            if (textures[i].name == string.Empty) {
                continue;
            }

            list += (i.ToString() + ". " + textures[i].name + "\n");

            if (i == 500) {
                Debug.Log(list);
                list = string.Empty;
            }
        }

        Debug.Log(list);
    }

    private void ListLoadedAudio() {
        UnityEngine.Object[] sounds = Resources.FindObjectsOfTypeAll(typeof(AudioClip));

        string list = string.Empty;

        for (int i = 0; i < sounds.Length; i++) {
            if (sounds[i].name == string.Empty) {
                continue;
            }
            list += (i.ToString() + ". " + sounds[i].name + "\n");
        }

        Debug.Log(list);
    }

    private void ListLoadedGameObjects() {
        UnityEngine.Object[] gos = Resources.FindObjectsOfTypeAll(typeof(GameObject));

        string list = string.Empty;

        for (int i = 0; i < gos.Length; i++) {
            if (gos[i].name == string.Empty) {
                continue;
            }
            list += (i.ToString() + ". " + gos[i].name + "\n");
        }

        Debug.Log(list);
    }
}