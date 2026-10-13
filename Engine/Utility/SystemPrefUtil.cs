using System;
using Agnostic.Core;
using Engine.AgnosticHost;
using Engine.Utility;
using UnityEngine;

public class SystemPrefUtil {

    // Kill switch. On: the string, has, delete and save calls go through the agnostic storage seam
    // (AgnosticStorage.storage -> UnityStorage -> PlayerPrefs, so the same keys and values). Flip it at
    // this source default; a runtime flip only affects later calls. Int, Float, DateTime and DeleteAll
    // stay on PlayerPrefs: their values are native typed entries that the string kv cannot read.
    // See contexts/gamedev/agnostic/context-agnostic-storage.md (S5).
    public static bool inward = true;

    public SystemPrefUtil() {

    }

    public static void Save() {

        if (inward) {
            AgnosticStorage.storage.KvFlush();
            return;
        }

        PlayerPrefs.Save();
    }

    public static void SetLocalSettingString(string key, string value) {

        if (inward && AgnosticStorage.storage.KvSet(key, value) == HostStatus.ok) {
            return;
        }

        PlayerPrefs.SetString(key, value);
    }

    public static string GetLocalSettingString(string key) {

        if (inward && !string.IsNullOrEmpty(key)) {
            string stored;
            return AgnosticStorage.storage.KvGet(key, out stored) && !string.IsNullOrEmpty(stored) ? stored : "";
        }

        string keyValue = "";
        if (SystemPrefUtil.HasLocalSetting(key)) {
            keyValue = PlayerPrefs.GetString(key);
            if (string.IsNullOrEmpty(keyValue))
                return "";
        }
        return keyValue;
    }

    public static bool HasLocalSetting(string key) {

        if (inward && !string.IsNullOrEmpty(key)) {
            string stored;
            return AgnosticStorage.storage.KvGet(key, out stored);
        }

        bool hasKey = false;
        hasKey = PlayerPrefs.HasKey(key);
        return hasKey;
    }

    public static void SetLocalSettingFloat(string key, float value) {
        PlayerPrefs.SetFloat(key, value);
    }

    public static float GetLocalSettingFloat(string key) {
        float value = PlayerPrefs.GetFloat(key);
        return value;
    }

    public static void SetLocalSettingInt(string key, int value) {
        PlayerPrefs.SetInt(key, value);
    }

    public static int GetLocalSettingInt(string key) {
        int value = PlayerPrefs.GetInt(key);
        return value;
    }

    public static void SetLocalSettingDateTime(string key, DateTime value) {
        string dateTimeString = Convert.ToString(value.ToUniversalTime());
        PlayerPrefs.SetString(key, dateTimeString);
    }

    public static DateTime GetLocalSettingDateTime(string key) {
        string value = PlayerPrefs.GetString(key);
        if (string.IsNullOrEmpty(value)) {
            return DateTime.MinValue;
        }
        DateTime dt = DateTime.MinValue;
        bool validDate = DateTime.TryParse(value, out dt);
        if (!validDate) {
            dt = DateTime.MinValue;
        }
        return dt;
    }

    public static void DeleteKey(string key) {

        if (inward && !string.IsNullOrEmpty(key)) {
            AgnosticStorage.storage.KvDelete(key);
            return;
        }

        PlayerPrefs.DeleteKey(key);
    }

    public static void DeleteAll() {
        PlayerPrefs.DeleteAll();
    }
}