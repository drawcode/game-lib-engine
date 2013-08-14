using System;
using Engine.Utility;
using UnityEngine;

public class SystemPrefUtil {

    public SystemPrefUtil() {
    }

    public static void Save() {
        PlayerPrefs.Save();
    }

    public static void SetLocalSettingString(string key, string value) {
        PlayerPrefs.SetString(key, value);
    }

    public static string GetLocalSettingString(string key) {
        string keyValue = "";
        if (SystemPrefUtil.HasLocalSetting(key)) {
            keyValue = PlayerPrefs.GetString(key);
            if (string.IsNullOrEmpty(keyValue))
                return "";
        }
        return keyValue;
    }

    public static bool HasLocalSetting(string key) {
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
}