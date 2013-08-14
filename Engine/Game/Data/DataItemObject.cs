using System;
using System.Collections.Generic;
using System.IO;
using Engine.Data.Json;
using Engine.Utility;
using UnityEngine;

public class DataItemObject : DataObject {
    public string uuid = "";
    public string udid = "";

    public string fileName = "";
    public string filePath = "";
    public string fileFullPath = "";
    public string serverUrl = "";

    public string itemCode = "";
    public string parentCode = "";

    public DataItemObject() {
        Reset();
    }

    public void Change(string code) {
        Reset();

        itemCode = code;
    }

    public void ChangeNoReset(string code) {
        itemCode = code;
    }

    /*
    public void Change(string code, bool keepExisting) {
        LogUtil.Log("Change: code: " + code);
        LogUtil.Log("Change: key: " + GetCodeKey(username));

        if(itemCode != code) {
            config.lastLoggedOnUser = username;
            SaveConfig();

            string originalProfileUser = profile.username.ToLower();

            if(originalProfileUser == profile.GetDefaultPlayer().ToLower()
               || keepExisting) {

                // Keep all progress from defualt player if they decide to log into gamecenter
                // Has cheating problems but can be resolved after bug.
                profile.ChangeUserNoReset(username);
            }
            else {
                profile.ChangeUser(username);
            }

            LoadProfile();
            SaveProfile();
        }

        if(profileAchievement != null) {
            profileAchievement.username = profile.username;
        }

        if(profileStatistic != null) {
            profileStatistic.username = profile.username;
        }
    }

    public string GetCodeKey(string code) {
        return "item-" + System.Uri.EscapeUriString(code).ToLower();
    }

    public void LoadItem(string path, string code) {
        string key = GetCodeKey(code);

        LogUtil.Log("LoadItem: code: " + code);
        LogUtil.Log("LoadProfile: key: " + key);

        //profile.LoadData(Application.persistentDataPath + "/" + key);

        string data = "";
#if UNITY_WEBPLAYER
        data = SystemPrefUtil.GetLocalSettingString(key);
#else

        // general profile
        string persistentPath = Path.Combine(
            path,
            key + ".json");
        data = FileSystemUtil.ReadString(persistentPath);

#endif

        //if(!string.IsNullOrEmpty(data)) {
        //	this = JsonMapper.ToObject<DataItemObject>(data);
        //}
    }
    */

    public virtual void SetSettingValue(string code, object value) {
        DataAttribute att = new DataAttribute();
        att.val = value;
        att.code = code;
        att.name = code;
        att.type = "string";
        att.otype = "setting";
        SetAttribute(att);
    }

    public virtual string GetSettingValue(string code) {
        string currentValue = "";
        object objectValue = GetAttribute(code).val;
        if (objectValue != null) {
            currentValue = Convert.ToString(objectValue);
        }

        return currentValue;
    }

    public override void Reset() {
        base.Reset();

        itemCode = "";
        parentCode = "";
        uuid = "";
        udid = "";
    }
}