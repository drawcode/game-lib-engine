#define PROFILE_RESOURCES
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Threading;

using UnityEngine;

// using Engine.Data.Json;
using Engine.Events;
using Engine.Utility;

public class BaseGameState {
    
    //public GameConfig config;
    //public GameProfile profile;
    //public GameProfileStatistic profileStatistic;
    //public GameProfileAchievement profileAchievement;
    //public GameProfileCharacter profileCharacter;
    //public GameProfileCustomization profileCustomization;
    //public GameProfileMode profileMode;
    //public GameProfileProduct profileProduct;
    //public GameProfileRPG profileRPG;
    //public GameProfileTeam profileTeam;
    //public GameProfileVehicle profileVehicle;
    
    //public GameCareer career;
    public GameData gameData;
    //public GameLevels gameLevels;
    
    //public Gameverses.GameversesGameAPI gameversesAPI;
    
    private static volatile BaseGameState instance;
    private static System.Object syncRoot = new System.Object();
    //public Thread saveThread;
    public string KEY_CONFIG;
    public string KEY_PROFILE;
    public string KEY_CAREER;
    public string KEY_CAREER_LEGACY;
    public string KEY_GAME_DATA;
    
    public static BaseGameState BaseInstance {
        get {
            if (instance == null) {
                lock (syncRoot) {
                    if (instance == null) 
                        instance = new BaseGameState();
                }
            }   
            return instance;
        }
    }
    
    public BaseGameState() {
        
        if (Application.isEditor) {
            KEY_CONFIG = "config-DEV34";
            KEY_PROFILE = "profile-DEV34";
        }
        else {
            KEY_CONFIG = "config-v1_0_0";
            KEY_PROFILE = "profile-v1_0_0";
        }
        
        //KEY_CAREER = "career-v1";
        //KEY_CAREER_LEGACY = KEY_PROFILE + "career-v1";
        
        /*
        KEY_CONFIG = "config-v1111";
        KEY_PROFILE = "profile-v1111";
        KEY_CAREER = "career-v1111";
        KEY_CAREER_LEGACY = KEY_PROFILE + "career-v1111";
        */

        InitState();
    }
    
    public virtual void InitState() {

        //config = GameConfigs.Current;
        //profile = GameProfiles.Current;
        //profileStatistic = GameProfileStatistics.Current;
        //profileAchievement = GameProfileAchievements.Current;
        
        //profileCharacter = GameProfileCharacters.Current;
        //profileCustomization = GameProfileCustomizations.Current;
        //profileMode = GameProfileModes.Current;
        //profileProduct = GameProfileProducts.Current;
        //profileRPG = GameProfileRPGs.Current;
        //profileTeam = GameProfileTeams.Current;
        //profileVehicle = GameProfileVehicles.Current;

        gameData = GameDatas.Current;
        
        loadConfig();
        saveConfig();
        
        // Update to new profile naming to prevent overwriting bug
        //HandleLegacyProfileAndCareer();
        
        loadProfile();
        saveProfile();
    }

    // CONFIG
    
    public static void SaveConfig() {
        if (GameState.Instance != null) {
            GameState.Instance.saveConfig();
        }
    }
    
    public virtual void saveConfig() {
        string jsonString = GameConfigs.Current.ToJson();// JsonMapper.ToJson(config);
        SystemPrefUtil.SetLocalSettingString(KEY_CONFIG, jsonString);
        SystemPrefUtil.Save();
    }
    
    public static void LoadConfig() {
        if (GameState.Instance != null) {
            GameState.Instance.loadConfig();
        }
    }
    
    public virtual void loadConfig() {
        string data = SystemPrefUtil.GetLocalSettingString(KEY_CONFIG);
        if (!string.IsNullOrEmpty(data)) {
            GameConfigs.Current = data.FromJson<GameConfig>();//JsonMapper.ToObject<GameConfig>(data);
        }

        GameProfiles.Current.ChangeUser(GameConfigs.Current.lastLoggedOnUser);
        //career.ChangeUser(config.lastLoggedOnUser);
    }
    
    // KEYS

    public string key {
        get {
            return getProfileKey(GameProfiles.Current.username);
        }
    }
    
    public string keyAchievement {
        get {
            return getProfileAchievementKey(GameProfiles.Current.username);
        }
    }
    
    public string keyStatistic {
        get {
            return getProfileStatisticKey(GameProfiles.Current.username);
        }
    }
    
    public string keyCharacter {
        get {
            return getProfileCharacterKey(GameProfiles.Current.username);
        }
    }

    public string keyCustomization {
        get {
            return getProfileCustomizationKey(GameProfiles.Current.username);
        }
    }

    public string keyMode {
        get {
            return getProfileModeKey(GameProfiles.Current.username);
        }
    }

    public string keyProduct {
        get {
            return getProfileProductKey(GameProfiles.Current.username);
        }
    }

    public string keyRPG {
        get {
            return getProfileRPGKey(GameProfiles.Current.username);
        }
    }

    public string keyTeam {
        get {
            return getProfileTeamKey(GameProfiles.Current.username);
        }
    }

    public string keyVehicle {
        get {
            return getProfileVehicleKey(GameProfiles.Current.username);
        }
    }
    
    public virtual string getKey(string username, string type) {
        return KEY_PROFILE + "-" + type + "-" + System.Uri.EscapeUriString(username).ToLower();
    }
    
    public virtual string getProfileKey(string username) {
        return KEY_PROFILE + "-" + System.Uri.EscapeUriString(username).ToLower();
    }
    
    public virtual string getProfileStatisticKey(string username) {
        return getKey(username, "statistic");
    }
    
    public virtual string getProfileAchievementKey(string username) {
        return getKey(username, "achievement");
    }
    
    public virtual string getProfileCharacterKey(string username) {
        return getKey(username, "character-1-1");
    }
    
    public virtual string getProfileCustomizationKey(string username) {
        return getKey(username, "customize");
    }
    
    public virtual string getProfileModeKey(string username) {
        return getKey(username, "mode");
    }
    
    public virtual string getProfileProductKey(string username) {
        return getKey(username, "product");
    }
    
    public virtual string getProfileRPGKey(string username) {
        return getKey(username, "rpg");
    }
    
    public virtual string getProfileTeamKey(string username) {
        return getKey(username, "team");
    }
    
    public virtual string getProfileVehicleKey(string username) {
        return getKey(username, "vehicle");
    }
    
    public virtual string getProfileKeyLegacy() {
        return KEY_PROFILE;
    }

    // PROFILE
    
    public virtual void contentSave(string key, string data) {
        
        #if UNITY_WEBPLAYER || PROFILE_RESOURCES    
        SystemPrefUtil.SetLocalSettingString(key, data);
        SystemPrefUtil.Save();
        
        #else
        string path = Path.Combine(
            ContentPaths.appCachePathAllSharedUserData,
            key + ".json"); 
        FileSystemUtil.WriteString(path, data);
        #endif
    }
    
    public virtual T contentLoad<T>(string key, T obj) where T : new() {
        
        string data = prepareLoad(key);
        
        if (!string.IsNullOrEmpty(data)) {
            try {
                obj = data.FromJson<T>();// JsonMapper.ToObject<T>(data);
            }
            catch (Exception e) {
                Debug.LogError("Error content load:" + key + " " + obj.ToJson() + " e:" + e.ToJson());
            }
        }
        
        return obj;
    }
    
    public virtual string prepareSave(string keyTo, object obj) {
        string data = obj.ToJson();//JsonMapper.ToJson(obj);
        
        if (ProfileConfigs.useStorageEncryption) {
            data = data.ToEncrypted();
        }
        
        if (ProfileConfigs.useStorageCompression) {
            data = data.ToCompressed();
        }

        if(keyTo == key) {
            //Debug.Log("GameState::prepareSave data....");
            //Debug.Log(data);
        }

        return data;
    }
    
    public virtual void save(string key, object obj, bool setSync = false) {        
        string jsonString = prepareSave(key, obj);
        contentSave(key, jsonString);
        //LogUtil.Log("GameState::SaveProfile jsonString...." + jsonString);

        if (setSync) {
            sync(key, jsonString);
        }
    }

    public virtual void sync(string key, string data) {        
        if (AppConfigs.gameCloudSyncEnabled) {
            GameSync.SetProfileSyncContent(key, key, data);
        }
    }
    
    public virtual string prepareLoad(string keyTo) {
        
        string data = "";
        
        #if UNITY_WEBPLAYER || PROFILE_RESOURCES 
        
        data = SystemPrefUtil.GetLocalSettingString(key);//.ToDecompressed();
        
        #else
        data = readProfileFile(key);//.ToDecompressed();
        #endif
                
        if (ProfileConfigs.useStorageCompression) {// || data.IsCompressed()) {
            data = data.ToDecompressed();
        }
        
        if (ProfileConfigs.useStorageEncryption) {
            data = data.ToDecrypted();
        }

        if(keyTo == key) {
            //Debug.Log("GameState::prepareLoad data....");
            //Debug.Log(data);
        }

        return data;
    }
    
    public virtual string readProfileFile(string key) {
        
        string data = "";
        
        string persistentPath = Path.Combine(
            ContentPaths.appCachePathAllSharedUserData,
            key + ".json"); 
        data = FileSystemUtil.ReadString(persistentPath);
        
        return data;
    }

    public static void SyncProfile() {
        if (GameState.Instance != null) {
            GameState.Instance.syncProfile();
        }
    }
    
    public virtual void syncProfile() {

        if(AppConfigs.gameCloudSyncEnabled) {
            CoroutineUtil.Start(syncProfileCo());
        }
    }

    public IEnumerator syncProfileCo() {

        yield return new WaitForEndOfFrame();

        Debug.Log("syncProfileCo");

        GameSync.ResetProfileSyncObject();
        
        save(key, GameProfiles.Current, true);
        save(keyAchievement, GameProfileAchievements.Current, true);
        save(keyStatistic, GameProfileStatistics.Current, true);
        save(keyCharacter, GameProfileCharacters.Current, true);
        save(keyCustomization, GameProfileCustomizations.Current, true);
        save(keyMode, GameProfileModes.Current, true);
        save(keyProduct, GameProfileProducts.Current, true);
        save(keyRPG, GameProfileRPGs.Current, true);
        save(keyTeam, GameProfileTeams.Current, true);
        save(keyVehicle, GameProfileVehicles.Current, true);

        // prepare and upload any files changed

        GameSync.SyncProfile();

        // validate on server

        // download new profile files
        
        yield return new WaitForEndOfFrame();
    }
    
    public static void SaveProfile() {
        if (GameState.Instance != null) {
            GameState.Instance.saveProfile();
        }
    }
    
    public virtual void saveProfile(GameProfile profile) {
        if (profile != null) {
            //LogUtil.Log("SaveProfile: username: " + profile.username);            
            //LogUtil.Log("SaveProfile: key: " + key);
            //LogUtil.Log("SaveProfile: keyAchievement: " + keyAchievement);
            //LogUtil.Log("SaveProfile: keyStatistic: " + keyStatistic);

            Debug.Log("BaseGameState::saveProfile username:" + profile.username);

            save(key, GameProfiles.Current);
            save(keyAchievement, GameProfileAchievements.Current);
            save(keyStatistic, GameProfileStatistics.Current);
            save(keyCharacter, GameProfileCharacters.Current);
            save(keyCustomization, GameProfileCustomizations.Current);
            save(keyMode, GameProfileModes.Current);
            save(keyProduct, GameProfileProducts.Current);
            save(keyRPG, GameProfileRPGs.Current);
            save(keyTeam, GameProfileTeams.Current);
            save(keyVehicle, GameProfileVehicles.Current);

            //CoroutineUtil.Start(SaveProfileCo(profile));
        }
    }
    
    public virtual void saveProfile() {
        
        //LogUtil.Log("SaveProfile");
        
        if (GameProfiles.Current != null) {
            saveProfile(GameProfiles.Current);
        }
    }
    
    public IEnumerator saveProfileCo(GameProfile profile) {        
        
        saveProfile(profile);

        yield return null;
    }
    
    public static void LoadProfile() {
        if (GameState.Instance != null) {
            GameState.Instance.loadProfile();
        }
    }
    
    public virtual void loadProfile() {
        
        if (GameProfiles.Current != null) {
            
            LogUtil.Log("LoadProfile: username: " + GameProfiles.Current.username);
            //LogUtil.Log("LoadProfile: key: " + key);
            //profile.LoadData(Application.persistentDataPath + "/" + key);

            GameProfiles.Current = contentLoad<GameProfile>(key, GameProfiles.Current);

            GameProfiles.Current.login_count++;
            GameProfiles.Current.SyncAccessPermissions();

            GameProfileAchievements.Current = contentLoad<GameProfileAchievement>(keyAchievement, GameProfileAchievements.Current);
            GameProfileStatistics.Current = contentLoad<GameProfileStatistic>(keyStatistic, GameProfileStatistics.Current);
            GameProfileCharacters.Current = contentLoad<GameProfileCharacter>(keyCharacter, GameProfileCharacters.Current);
            GameProfileCustomizations.Current = contentLoad<GameProfileCustomization>(keyCustomization, GameProfileCustomizations.Current);
            GameProfileModes.Current = contentLoad<GameProfileMode>(keyMode, GameProfileModes.Current);
            GameProfileProducts.Current = contentLoad<GameProfileProduct>(keyProduct, GameProfileProducts.Current);
            GameProfileRPGs.Current = contentLoad<GameProfileRPG>(keyRPG, GameProfileRPGs.Current);
            GameProfileTeams.Current = contentLoad<GameProfileTeam>(keyTeam, GameProfileTeams.Current);
            GameProfileVehicles.Current = contentLoad<GameProfileVehicle>(keyVehicle, GameProfileVehicles.Current);

            //profileAchievement = contentLoad<GameProfileAchievement>(keyAchievement, profileAchievement);
            //profileStatistic = contentLoad<GameProfileStatistic>(keyStatistic, profileStatistic);
            //profileCharacter = contentLoad<GameProfileCharacter>(keyCharacter, profileCharacter);
            //profileCustomization = contentLoad<GameProfileCustomization>(keyCustomization, profileCustomization);
            //profileMode = contentLoad<GameProfileMode>(keyMode, profileMode);
            //profileProduct = contentLoad<GameProfileProduct>(keyProduct, profileProduct);
            //profileRPG = contentLoad<GameProfileRPG>(keyRPG, profileRPG);
            //profileTeam = contentLoad<GameProfileTeam>(keyTeam, profileTeam);
            //profileVehicle = contentLoad<GameProfileVehicle>(keyVehicle, profileVehicle);

            //GameProfiles.Current = profile;
            //GameProfileAchievements.Current = profileAchievement;
            //GameProfileStatistics.Current = profileStatistic;

            //GameProfileCharacters.Current = profileCharacter;
            //GameProfileCustomizations.Current = profileCustomization;
            //GameProfileModes.Current = profileMode;
            //GameProfileProducts.Current = profileProduct;
            //GameProfileRPGs.Current = profileRPG;
            //GameProfileTeams.Current = profileTeam;
            //GameProfileVehicles.Current = profileVehicle;
        }
    }
    
    public static void ChangeUser(string username) {
        if (GameState.Instance != null) {
            GameState.Instance.changeUser(username);
        }
    }
    
    public static void ChangeUser(string username, bool keepExisting) {
        if (GameState.Instance != null) {
            GameState.Instance.changeUser(username, keepExisting);
        }
    }
    
    public virtual void changeUser(string username) {
        changeUser(username, false);
    }
    
    public virtual void changeUser(string username, bool keepExisting) {
        LogUtil.Log("ChangeUser: username: " + username);
        LogUtil.Log("ChangeUser: key: " + getProfileKey(username));
        
        if (GameProfiles.Current.username != username) {

            GameConfigs.Current.lastLoggedOnUser = username;
            saveConfig();
            
            string originalProfileUser = GameProfiles.Current.username.ToLower();
            
            if (originalProfileUser.ToLower() == "player"
                || keepExisting) {
                // Keep all progress from defualt player if they decide to log into gamecenter
                // Has cheating problems but can be resolved after bug.
                GameProfiles.Current.ChangeUserNoReset(username);
            }
            else {
                GameProfiles.Current.ChangeUser(username);
            }
            
            loadProfile();
            saveProfile();
        }
        
        //if (profileAchievement != null) {
        //    profileAchievement.username = profile.username;
        //}
        
        //if (profileStatistic != null) {
        //    profileStatistic.username = profile.username;
        //}       
        
        //if (profileCharacter != null) {
        //    profileCharacter.username = profile.username;
        //}
        
        //if (profileCustomization != null) {
        //    profileCustomization.username = profile.username;
        //}
        
        //if (profileMode != null) {
        //    profileMode.username = profile.username;
        //}
        
        //if (profileProduct != null) {
        //    profileProduct.username = profile.username;
        //}
        
        //if (profileRPG != null) {
        //    profileRPG.username = profile.username;
        //}
        
        //if (profileTeam != null) {
        //    profileTeam.username = profile.username;
        //}
        
        //if (profileVehicle != null) {
        //    profileVehicle.username = profile.username;
        //}
    }
}


/*
    public void HandleLegacyProfileAndCareer() {    
        
        bool hasOldData = false;
        bool hasNewData = false;
        
        string profileCheck = SystemPrefUtil.GetLocalSettingString(GetProfileKeyLegacy()); 
        if(!string.IsNullOrEmpty(profileCheck)) {
            // Has old data
            if(profileCheck.Length > 10) {
                hasOldData = true;
            }
        }
        
        if(hasOldData) {
            LogUtil.Log("HandleLegacyProfileAndCareer: hasOldData: " + hasOldData);

            string lastLoggedInProfileUsername = config.lastLoggedOnUser;
            
            LogUtil.Log("HandleLegacyProfileAndCareer: key: " + lastLoggedInProfileUsername);
            
            string profileCheckNew = SystemPrefUtil.GetLocalSettingString(GetProfileKey(lastLoggedInProfileUsername));
        
            if(!string.IsNullOrEmpty(profileCheckNew)) {
                // Has new data
                if(profileCheckNew.Length > 10) {
                    hasNewData = true;
                }
            }           
            
            // If new key doesn't exist and old key does, 
            if(!hasNewData) {
                LogUtil.Log("HandleLegacyProfileAndCareer: hasNewData: " + hasNewData);
                // Create new data
                // Copy old key to new and save to remove old key
                LoadProfileLegacy();
                SaveProfile();
                                
                LoadCareerLegacy();
                SaveCareer();
            }
            
            // If new key exists already, remove old
            // Set legacy key to nothing
            SystemPrefUtil.SetLocalSettingString(GetProfileKeyLegacy(), "");
            SystemPrefUtil.SetLocalSettingString(GetCareerKeyLegacy(profile), "");
            SystemPrefUtil.Save();
        }
    }
    */