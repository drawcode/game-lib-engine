using System;
using System.Collections.Generic;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

public class UnityUtil {

    public static WWWForm WWWFormFromDictionary(Dictionary<string, object> parameters) {
        WWWForm form = new WWWForm();
        if (parameters != null) {
            if (parameters.Count > 0) {
                foreach (KeyValuePair<string, object> pair in parameters) {
                    form.AddField(pair.Key, pair.Value.ToString());
                }
            }
        }
        return form;
    }

    public static WWWForm WWWFormFromDictionary(Dictionary<string, string> parameters) {
        WWWForm form = new WWWForm();
        if (parameters != null) {
            if (parameters.Count > 0) {
                foreach (KeyValuePair<string, string> pair in parameters) {
                    form.AddField(pair.Key, pair.Value.ToString());
                }
            }
        }
        return form;
    }

    public static string GetPlatformAppDataFolder() {
        return Application.dataPath;
    }

    public static string GetPlatformAppPersistenceFolder() {
        if (Application.platform == RuntimePlatform.IPhonePlayer) {
            return GetDeviceDocumentsFolder();
        }
        else {
            return Application.persistentDataPath;
        }
    }

    public static string GetPlatformAppStorageLocalFolder() {
        if (Application.platform == RuntimePlatform.IPhonePlayer) {
            return GetPlatformAppFolder();
        }
        else {
            return GetPlatformAppDataFolder();
        }
    }

    public static string GetPlatformAppFolder() {
        // Your game has read+write access to /var/mobile/Applications/XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX/Documents 
        // Application.dataPath returns              
        // /var/mobile/Applications/XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX/myappname.app/Data 
        // Strip "/Data" from path 
        string path = GetPlatformAppDataFolder().Substring(0, Application.dataPath.Length - 5);
        return path;
    }

    public static string GetPlatformAppRootFolder() {
        // Your game has read+write access to /var/mobile/Applications/XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX/Documents 
        // Application.dataPath returns              
        // /var/mobile/Applications/XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX/myappname.app/Data 
        // Strip "/Data" from path 
        string path = GetPlatformAppDataFolder().Substring(0, Application.dataPath.Length - 5);
        // Strip application name 
        path = path.Substring(0, path.LastIndexOf('/'));
        return path;
    }

    public static string GetDeviceDocumentsFolder() {
        // Your game has read+write access to /var/mobile/Applications/XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX/Documents 
        // Application.dataPath returns              
        // /var/mobile/Applications/XXXXXXXX-XXXX-XXXX-XXXX-XXXXXXXXXXXX/myappname.app/Data 
        // Strip "/Data" from path 
        string path = GetPlatformAppRootFolder();
        return path + "/Documents";
    }

    //

    //public static void FileCopyToPersistence(string dataFilePath, string persistenceFilePath) {

    //    UnityWebRequest www = new UnityWebRequest();
    //    www.url = dataFilePath;

    //    UnityWebRequestAsyncOperation asyncOp = www.SendWebRequest();

    //    //WWW file = new WWW(dataFilePath);

    //    float currentTime = Time.time;
    //    float endTime = currentTime + 6f; // only allow some seconds for file check

    //    while (!asyncOp.isDone && currentTime < endTime) {
    //        currentTime = Time.time;
    //    };

    //    if (www.isNetworkError || www.isHttpError) {
    //        Debug.LogWarning($"Network error whilst downloading [{dataFilePath}] Error: [{www.error}]");
    //        //Debug.Log(www.error);
    //    }
    //    else {
    //        //int length = file.bytes.Length;
    //        int length = www.downloadHandler.data.Length;

    //        Debug.Log("CopyFile: Android: dataFilePath:" + dataFilePath);
    //        Debug.Log("CopyFile: Android: persistenceFilePath:" + persistenceFilePath);
    //        Debug.Log("CopyFile: Android: file.bytes.length:" + length);

    //        //if(file.bytes.Length > 0) {
    //        //    // Save file contents to new location                   
    //        //    FileSystemUtil.WriteAllBytes(persistenceFilePath, file.bytes);
    //        //}

    //        if (www.downloadHandler.data.Length > 0) {
    //            // Save file contents to new location                   
    //            FileSystemUtil.WriteAllBytes(persistenceFilePath, www.downloadHandler.data);
    //        }
    //    }
    //}
}