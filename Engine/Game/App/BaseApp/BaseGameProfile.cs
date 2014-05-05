using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Engine.Data.Json;
using Engine.Events;
using Engine.Utility;

public class BaseGameProfileMessages {
    public static string ProfileShouldBeSaved = "profile-save";
}

public class BaseGameProfileAttributes {
    public static string ATT_CONTROL_HORIZON_TILT = "controls-horizon-tilt";
    public static string ATT_CONTROL_DRIVING_ASSIST = "controls-driving-assist";
    public static string ATT_CONTROL_INPUT_TOUCH = "controls-input";
    public static string ATT_CONTROL_HANDED = "controls-handed";
    public static string ATT_CONTROL_VIBRATE = "controls-vibrate";
    public static string ATT_AUDIO_MUSIC_VOLUME = "audio-music-volume";
    public static string ATT_AUDIO_EFFECTS_VOLUME = "audio-effects-volume";
    public static string ATT_AUDIO_VO_VOLUME = "audio-vo-volume";
    public static string ATT_THIRD_PARTY_NETWORK = "third-party-network";

    // RPG
    public static string ATT_PROGRESS_XP = "progress-xp";
    public static string ATT_CURRENCY = "currency";
    public static string ATT_PROGRESS_HEALTH = "progress-health";
    public static string ATT_PROGRESS_ENERGY = "progress-energy";
    public static string ATT_PROGRESS_LEVEL = "progress-level";

    // MODES

    public static string ATT_CURRENT_GAME_MODE = "game-mode";
    public static string ATT_CURRENT_CAMERA_MODE = "camera-mode";
    public static string ATT_CURRENT_APP_MODE = "app-mode";
    public static string ATT_CURRENT_APP_MODE_TYPE = "app-mode-type";
    public static string ATT_CURRENT_APP_STATE = "app-state";
    public static string ATT_CURRENT_APP_CONTENT_STATE = "app-content-state";

    // UI STATE
    public static string ATT_UI_HAS_SEEN_HELP = "ui-seen-help";
    public static string ATT_HELP_TIPS_SHOWN_DAY = "ui-help-tips-shown-day";
    public static string ATT_HELP_TIPS_SHOWN_DATE = "ui-help-tips-shown-date";

    // CUSTOM

    public static string ATT_CUSTOM_AUDIO = "custom-audio";
    public static string ATT_CUSTOM_COLORS = "custom-colors";
    public static string ATT_CUSTOM_COLORS_RUNNER = "custom-colors-runner";
    public static string ATT_CUSTOM_COLOR_ITEM = "custom-color-item";
    public static string ATT_CUSTOM_TEXTURE_ITEM = "custom-texture-item";
     
    public static string ATT_CUSTOM_COLOR_PRESET = "custom-color-preset";
    public static string ATT_CUSTOM_TEXTURE_PRESET = "custom-texture-preset";

    // SOCIAL
 
    public static string ATT_AUTH_SOCIAL_NETWORK_TYPE = "auth-social-network-type";
    public static string ATT_AUTH_SOCIAL_NETWORK_USERNAME = "auth-social-network-username";
    public static string ATT_AUTH_SOCIAL_NETWORK_NAME = "auth-social-network-name";
    public static string ATT_AUTH_SOCIAL_NETWORK_FNAME = "auth-social-network-fname";
    public static string ATT_AUTH_SOCIAL_NETWORK_LNAME = "auth-social-network-lname";
    public static string ATT_AUTH_SOCIAL_NETWORK_USERID = "auth-social-network-userid";
    public static string ATT_AUTH_SOCIAL_NETWORK_AUTHTOKEN_USER = "auth-social-network-authtoken-user";
    public static string ATT_AUTH_SOCIAL_NETWORK_AUTHTOKEN_APP = "auth-social-network-authtoken-app";
}

public class BaseGameProfileDataState {
    public static bool updatedEffectsVolume = false;
    public static double currentEffectsVolume = .5;
    public static bool updatedMusicVolume = false;
    public static double currentMusicVolume = .5;
    public static bool updatedVOVolume = false;
    public static double currentVOVolume = .5;
}

public enum ProfileControlHanded {
    RIGHT = 0,
    LEFT = 1
}

public class BaseGameProfiles {
    private static volatile BaseGameProfile current;
    private static volatile BaseGameProfiles instance;
    private static object syncRoot = new Object();
    public static string DEFAULT_USERNAME = "Player";

    public static BaseGameProfile Current {
        get {
            if (current == null) {
                lock (syncRoot) {
                    if (current == null)
                        current = new BaseGameProfile();
                }
            }

            return current;
        }
        set {
            current = value;
        }
    }

    public static BaseGameProfiles Instance {
        get {
            if (instance == null) {
                lock (syncRoot) {
                    if (instance == null)
                        instance = new BaseGameProfiles();
                }
            }

            return instance;
        }
    }

    // TODO: Common profile actions, lookup, count, etc
}

public class BaseGameProfile : Profile {

    // BE CAREFUL adding properties as they will cause a need for a profile conversion
    // Best way to add items to the profile is the GetAttribute and SetAttribute class as
    // that stores as a generic DataAttribute class.  Booleans, strings, objects, serialized json objects etc
    // all work well and cause no need to convert profile on updates.

    public BaseGameProfile() {
        Reset();
    }

    public override void Reset() {
        base.Reset();
        username = ProfileConfigs.defaultPlayerName;
    }

    // HELPERS
    // We want all properties to be list attributes instead of
    // class level properties since this is json serializable.

    // GAME MODES

    public virtual int GetCurrentGameMode() {
        return GetCurrentGameMode(0);
    }

    public virtual int GetCurrentGameMode(int defaultValue) {
        int attValue = defaultValue;
        if (CheckIfAttributeExists(BaseGameProfileAttributes.ATT_CURRENT_GAME_MODE))
            attValue = GetAttributeIntValue(BaseGameProfileAttributes.ATT_CURRENT_GAME_MODE);
        return attValue;
    }

    public virtual void SetCurrentGameMode(int attValue) {
        SetAttributeIntValue(BaseGameProfileAttributes.ATT_CURRENT_GAME_MODE, attValue);
    }

    // APP MODES

    public virtual string GetCurrentAppMode() {
        return GetCurrentAppMode("default");
    }

    public virtual string GetCurrentAppMode(string defaultValue) {
        string attValue = defaultValue;
        string key = BaseGameProfileAttributes.ATT_CURRENT_APP_MODE;
        if (CheckIfAttributeExists(key))
            attValue = GetAttributeStringValue(key);
        return attValue;
    }

    public virtual void SetCurrentAppMode(string attValue) {
        string key = BaseGameProfileAttributes.ATT_CURRENT_APP_MODE;
        SetAttributeStringValue(key, attValue);
    }

    // APP MODE TYPES

    public virtual string GetCurrentAppModeType() {
        return GetCurrentAppModeType("default");
    }

    public virtual string GetCurrentAppModeType(string defaultValue) {
        string attValue = defaultValue;
        string key = BaseGameProfileAttributes.ATT_CURRENT_APP_MODE_TYPE;
        if (CheckIfAttributeExists(key))
            attValue = GetAttributeStringValue(key);
        return attValue;
    }

    public virtual void SetCurrentAppModeType(string attValue) {
        string key = BaseGameProfileAttributes.ATT_CURRENT_APP_MODE_TYPE;
        SetAttributeStringValue(key, attValue);
    }

    // APP STATE

    public virtual string GetCurrentAppState() {
        return GetCurrentAppState("default");
    }

    public virtual string GetCurrentAppState(string defaultValue) {
        string attValue = defaultValue;
        string key = BaseGameProfileAttributes.ATT_CURRENT_APP_STATE;
        if (CheckIfAttributeExists(key))
            attValue = GetAttributeStringValue(key);
        return attValue;
    }

    public virtual void SetCurrentAppState(string attValue) {
        string key = BaseGameProfileAttributes.ATT_CURRENT_APP_STATE;
        SetAttributeStringValue(key, attValue);
    }

    // APP CONTENT STATE

    public virtual string GetCurrentAppContentState() {
        return GetCurrentAppContentState("default");
    }

    public virtual string GetCurrentAppContentState(string defaultValue) {
        string attValue = defaultValue;
        string key = BaseGameProfileAttributes.ATT_CURRENT_APP_CONTENT_STATE;
        if (CheckIfAttributeExists(key))
            attValue = GetAttributeStringValue(key);
        return attValue;
    }

    public virtual void SetCurrentAppContentState(string attValue) {
        string key = BaseGameProfileAttributes.ATT_CURRENT_APP_CONTENT_STATE;
        SetAttributeStringValue(key, attValue);
    }

    // CAMERA MODES

    public virtual int GetCurrentCameraMode() {
        return GetCurrentCameraMode(0);
    }

    public virtual int GetCurrentCameraMode(int defaultValue) {
        int attValue = defaultValue;
        if (CheckIfAttributeExists(BaseGameProfileAttributes.ATT_CURRENT_CAMERA_MODE))
            attValue = GetAttributeIntValue(BaseGameProfileAttributes.ATT_CURRENT_CAMERA_MODE);
        return attValue;
    }

    public virtual void SetCurrentCameraMode(int attValue) {
        SetAttributeIntValue(BaseGameProfileAttributes.ATT_CURRENT_CAMERA_MODE, attValue);
    }

    // HELP TIPS SHOWN DAY

    public virtual int GetHelpTipsShownDay() {
        return GetHelpTipsShownDay(0);
    }

    public virtual int GetHelpTipsShownDay(int defaultValue) {
        int attValue = defaultValue;
        if (CheckIfAttributeExists(BaseGameProfileAttributes.ATT_HELP_TIPS_SHOWN_DAY))
            attValue = GetAttributeIntValue(BaseGameProfileAttributes.ATT_HELP_TIPS_SHOWN_DAY);
        return attValue;
    }

    public virtual void SetHelpTipsShownDay(int attValue) {
        SetAttributeIntValue(BaseGameProfileAttributes.ATT_HELP_TIPS_SHOWN_DAY, attValue);
    }

    public virtual double GetHelpTipsShownDate() {
        return GetHelpTipsShownDate(0);
    }

    public virtual double GetHelpTipsShownDate(double defaultValue) {
        double attValue = defaultValue;
        if (CheckIfAttributeExists(BaseGameProfileAttributes.ATT_HELP_TIPS_SHOWN_DATE))
            attValue = GetAttributeDoubleValue(BaseGameProfileAttributes.ATT_HELP_TIPS_SHOWN_DATE);
        return attValue;
    }

    public virtual void SetHelpTipsShownDate(double attValue) {
        SetAttributeDoubleValue(BaseGameProfileAttributes.ATT_HELP_TIPS_SHOWN_DATE, attValue);
    }
 

    /*
    // CUSTOMIZATION
    public virtual void SetCustomColorsRunner(CustomPlayerColorsRunner colors) {
        string colorsText = JsonMapper.ToJson(colors);
        LogUtil.Log("SetCustomColorsRunner: " + colorsText);
        SetAttributeStringValue(BaseGameProfileAttributes.ATT_CUSTOM_COLORS_RUNNER, colorsText);
    }

    public virtual CustomPlayerColorsRunner GetCustomColorsRunner() {
        CustomPlayerColorsRunner colors = new CustomPlayerColorsRunner();

        string key = BaseGameProfileAttributes.ATT_CUSTOM_COLORS_RUNNER;

        if(!CheckIfAttributeExists(key)) {

            // add default colors
            SetCustomColorsRunner(new CustomPlayerColorsRunner());
            Messenger.Broadcast(BaseGameProfileMessages.ProfileShouldBeSaved);
        }

        string json = GetAttributeStringValue(key);
        if(!string.IsNullOrEmpty(json)) {
            try {
                //LogUtil.Log("GetCustomColors: " + json);
                colors = JsonMapper.ToObject<CustomPlayerColorsRunner>(json);
            }
            catch(Exception e) {
                colors = new CustomPlayerColorsRunner();
                LogUtil.Log(e);
            }
        }
        return colors;
    }

 

    // CUSTOMIZATION
    public virtual void SetCustomColors(CustomPlayerColors colors) {
        string colorsText = JsonMapper.ToJson(colors);
        LogUtil.Log("SetCustomColors: " + colorsText);
        SetAttributeStringValue(BaseGameProfileAttributes.ATT_CUSTOM_COLORS, colorsText);
    }

    public virtual CustomPlayerColors GetCustomColors() {
        CustomPlayerColors colors = new CustomPlayerColors();

        string key = BaseGameProfileAttributes.ATT_CUSTOM_COLORS;

        if(!CheckIfAttributeExists(key)) {

            // add default colors
            SetCustomColors(CustomColors.DefaultSet);
            Messenger.Broadcast(BaseGameProfileMessages.ProfileShouldBeSaved);
        }

        string json = GetAttributeStringValue(key);
        if(!string.IsNullOrEmpty(json)) {
            try {
                LogUtil.Log("GetCustomColors: " + json);
                colors = JsonMapper.ToObject<CustomPlayerColors>(json);
            }
            catch(Exception e) {
                colors = new CustomPlayerColors();
                LogUtil.Log(e);
            }
        }
        return colors;
    }

    */

    public virtual void SetCustomAudio(CustomPlayerAudio audio) {
        string audioText = JsonMapper.ToJson(audio);
        LogUtil.Log("SetCustomAudio: " + audioText);
        SetAttributeStringValue(BaseGameProfileAttributes.ATT_CUSTOM_AUDIO, audioText);
    }

    public virtual CustomPlayerAudio GetCustomAudio() {
        CustomPlayerAudio audio = new CustomPlayerAudio();
        string json = GetAttributeStringValue(BaseGameProfileAttributes.ATT_CUSTOM_AUDIO);
        if (!string.IsNullOrEmpty(json)) {
            try {
                LogUtil.Log("GetCustomAudio: " + json);
                audio = JsonMapper.ToObject<CustomPlayerAudio>(json);
            }
            catch (Exception e) {
                audio = new CustomPlayerAudio();
                LogUtil.Log(e);
            }
        }
        return audio;
    }

    // CONTROLS

    // HORIZON TILT

    public virtual double GetControlHorizonTilt() {
        return GetControlHorizonTilt(1.0);
    }

    public virtual double GetControlHorizonTilt(double defaultValue) {
        double attValue = defaultValue;
        if (CheckIfAttributeExists(BaseGameProfileAttributes.ATT_CONTROL_HORIZON_TILT))
            attValue = GetAttributeDoubleValue(BaseGameProfileAttributes.ATT_CONTROL_HORIZON_TILT);
        return attValue;
    }

    public virtual void SetControlHorizonTilt(double attValue) {
        SetAttributeDoubleValue(BaseGameProfileAttributes.ATT_CONTROL_HORIZON_TILT, attValue);
    }

    // CONTROL HANDED (LEFT/RIGHT)

    public virtual ProfileControlHanded GetControlHanded() {
        return GetControlHanded(ProfileControlHanded.RIGHT);
    }

    public virtual ProfileControlHanded GetControlHanded(ProfileControlHanded defaultValue) {
        int attValue = (int)defaultValue;
        if (CheckIfAttributeExists(BaseGameProfileAttributes.ATT_CONTROL_HANDED))
            attValue = GetAttributeIntValue(BaseGameProfileAttributes.ATT_CONTROL_HANDED);
        ProfileControlHanded controlHanded = (ProfileControlHanded)attValue;
        return controlHanded;
    }

    public virtual void SetControlHanded(ProfileControlHanded attValue) {
        SetAttributeIntValue(BaseGameProfileAttributes.ATT_CONTROL_HANDED, (int)attValue);
    }

    // CONTROL VIBRATE

    public virtual bool GetControlVibrate() {
        return GetControlVibrate(true);
    }

    public virtual bool GetControlVibrate(bool defaultValue) {
        bool attValue = defaultValue;
        if (CheckIfAttributeExists(BaseGameProfileAttributes.ATT_CONTROL_VIBRATE))
            attValue = GetAttributeBoolValue(BaseGameProfileAttributes.ATT_CONTROL_VIBRATE);
        return attValue;
    }

    public virtual void SetControlVibrate(bool attValue) {
        SetAttributeBoolValue(BaseGameProfileAttributes.ATT_CONTROL_VIBRATE, attValue);
    }


    // HELP/TIPS

    public virtual bool GetHasSeenHelp() {
        return GetHasSeenHelp(false);
    }

    public virtual bool GetHasSeenHelp(bool defaultValue) {
        bool attValue = defaultValue;
        if (CheckIfAttributeExists(BaseGameProfileAttributes.ATT_UI_HAS_SEEN_HELP))
            attValue = GetAttributeBoolValue(BaseGameProfileAttributes.ATT_UI_HAS_SEEN_HELP);
        return attValue;
    }

    public virtual void SetHasSeenHelp(bool attValue) {
        SetAttributeBoolValue(BaseGameProfileAttributes.ATT_UI_HAS_SEEN_HELP, attValue);
    }

    // INPUT

    public virtual bool GetControlTouch() {
        return GetControlTouch(true);
    }

    public virtual bool GetControlTouch(bool defaultValue) {
        bool attValue = defaultValue;
        if (CheckIfAttributeExists(BaseGameProfileAttributes.ATT_CONTROL_INPUT_TOUCH))
            attValue = GetAttributeBoolValue(BaseGameProfileAttributes.ATT_CONTROL_INPUT_TOUCH);
        return attValue;
    }

    public virtual void SetControlTouch(bool attValue) {
        SetAttributeBoolValue(BaseGameProfileAttributes.ATT_CONTROL_INPUT_TOUCH, attValue);
    }

    // NETWORK USER STATE

    public virtual bool IsThirdPartyNetworkUser() {
        return GetThirdPartyNetworkUser(false);
    }

    public virtual bool GetThirdPartyNetworkUser(bool defaultValue) {
        bool attValue = defaultValue;
        if (CheckIfAttributeExists(BaseGameProfileAttributes.ATT_THIRD_PARTY_NETWORK))
            attValue = GetAttributeBoolValue(BaseGameProfileAttributes.ATT_THIRD_PARTY_NETWORK);
        return attValue;
    }

    public virtual void SetThirdPartyNetworkUser(bool attValue) {
        SetAttributeBoolValue(BaseGameProfileAttributes.ATT_THIRD_PARTY_NETWORK, attValue);
    }

    // AUDIO

    public virtual double GetAudioMusicVolume() {
        return GetAudioMusicVolume(0.5);
    }

    public virtual double GetAudioMusicVolume(double defaultValue) {
        double attValue = defaultValue;

        attValue = BaseGameProfileDataState.currentMusicVolume;

        //UnityEngine.//LogUtil.Log("GetAudioMusicVolume ");
        //UnityEngine.//LogUtil.Log("GetAudioMusicVolume BaseGameProfileDataState.updatedMusicVolume:" + BaseGameProfileDataState.updatedMusicVolume);

        //if(BaseGameProfileDataState.updatedMusicVolume) {
        //   BaseGameProfileDataState.updatedMusicVolume = false;
        //UnityEngine.//LogUtil.Log("GetAudioMusicVolume checking attribute exists:" + BaseGameProfileAttributes.ATT_AUDIO_MUSIC_VOLUME);

        if (CheckIfAttributeExists(BaseGameProfileAttributes.ATT_AUDIO_MUSIC_VOLUME)) {

            //UnityEngine.//LogUtil.Log("GetAudioMusicVolume attribute exists:" + BaseGameProfileAttributes.ATT_AUDIO_MUSIC_VOLUME);

            attValue = GetAttributeDoubleValue(BaseGameProfileAttributes.ATT_AUDIO_MUSIC_VOLUME);

            //UnityEngine.//LogUtil.Log("GetAudioMusicVolume attValue:" + attValue);
            BaseGameProfileDataState.currentMusicVolume = attValue;
        }

        //}
        return attValue;
    }

    public virtual void SetAudioMusicVolume(double attValue) {
        BaseGameProfileDataState.updatedMusicVolume = true;
        SetAttributeDoubleValue(BaseGameProfileAttributes.ATT_AUDIO_MUSIC_VOLUME, attValue);
    }

    // AUDIO VO

    public virtual double GetAudioVOVolume() {
        return GetAudioVOVolume(0.7);
    }

    public virtual double GetAudioVOVolume(double defaultValue) {
        double attValue = defaultValue;

        attValue = BaseGameProfileDataState.currentVOVolume;

        //if(BaseGameProfileDataState.updatedVOVolume) {
        //   BaseGameProfileDataState.updatedVOVolume = false;

        if (CheckIfAttributeExists(BaseGameProfileAttributes.ATT_AUDIO_VO_VOLUME)) {
            attValue = GetAttributeDoubleValue(BaseGameProfileAttributes.ATT_AUDIO_VO_VOLUME);
            BaseGameProfileDataState.currentVOVolume = attValue;
        }

        //}
        return attValue;
    }

    public virtual void SetAudioVOVolume(double attValue) {
        BaseGameProfileDataState.updatedVOVolume = true;
        SetAttributeDoubleValue(BaseGameProfileAttributes.ATT_AUDIO_VO_VOLUME, attValue);
    }

    // AUDIO EFFECTS

    public virtual double GetAudioEffectsVolume() {
        return GetAudioEffectsVolume(0.9);
    }

    public virtual double GetAudioEffectsVolume(double defaultValue) {
        double attValue = defaultValue;

        attValue = BaseGameProfileDataState.currentEffectsVolume;

        //if(BaseGameProfileDataState.updatedEffectsVolume) {
        //   BaseGameProfileDataState.updatedEffectsVolume = false;

        if (CheckIfAttributeExists(BaseGameProfileAttributes.ATT_AUDIO_EFFECTS_VOLUME)) {
            attValue = GetAttributeDoubleValue(BaseGameProfileAttributes.ATT_AUDIO_EFFECTS_VOLUME);
            BaseGameProfileDataState.currentEffectsVolume = attValue;
        }

        //}

        return attValue;
    }

    public virtual void SetAudioEffectsVolume(double attValue) {
        BaseGameProfileDataState.updatedEffectsVolume = true;
        SetAttributeDoubleValue(BaseGameProfileAttributes.ATT_AUDIO_EFFECTS_VOLUME, attValue);
    }

    // CONTENT

    public virtual void SyncAccessPermissions() {

        //Contents.AccessPermissions.UnionWith(GetAccessPermissions());
    }

    // Profile specific content access, use Contents for global access
    // saved outside of the profile.

    // Attribute based unlocks to switch to for profile based unlocks.
    public virtual List<string> GetAccessPermissions() {
        List<string> permissions = new List<string>();
        DataAttribute attribute = GetAttribute("access-permissions");
        if (attribute != null) {
            permissions = attribute.val as List<string>;
            if (permissions != null)
                return permissions;
            else
                permissions = new List<string>();
        }
        return permissions;
    }

    public virtual bool CheckIfAccessPermissionExists(string permission) {
        List<string> permissions = GetAccessPermissions();
        if (permissions.Contains(permission))
            return true;
        return false;
    }

    public virtual void SetAccessPermissions(List<string> permissions) {
        DataAttribute attribute = new DataAttribute();
        attribute.code = "access-permissions";
        attribute.val = permissions;
        SetAttribute(attribute);
    }

    public virtual void SetAccessPermission(string permission) {
        List<string> permissions = GetAccessPermissions();
        if (!permissions.Contains(permission)) {
            permissions.Add(permission);
        }
        SetAccessPermissions(permissions);
    }

    // GAME SETTINGS

    public virtual void SetGameSettingValue(string code, object value) {
        DataAttribute att = new DataAttribute();
        att.val = value;
        att.code = code;
        att.name = code;
        att.type = "string";
        att.otype = "gameSetting";
        SetAttribute(att);
    }

    public virtual string GetGameSettingValue(string code) {
        string currentValue = "";
        object objectValue = GetAttribute(code).val;
        if (objectValue != null) {
            currentValue = Convert.ToString(objectValue);
        }

        return currentValue;
    }
 
    // SOCIAL
 
    // auth/social
        
    public string GetSocialNetworkKey(string type, string val) {
        return val + "-" + type;
    }
        
    public bool IsSocialNetworkUserFound() {
        string userId = GetSocialNetworkUserId();
        if (!string.IsNullOrEmpty(userId)) {
            return true;
        }
        return false;
    }
        
    public string GetSocialNetworkType() {
        return GetAttributeStringValue(BaseGameProfileAttributes.ATT_AUTH_SOCIAL_NETWORK_TYPE);
    }
        
    public void SetSocialNetworkProfileState(string type, string userId) {
        SetSocialNetworkType(type);
        SetSocialNetworkUserId(userId);
    }
        
    public void SetSocialNetworkType(string type) {
        SetAttributeStringValue(BaseGameProfileAttributes.ATT_AUTH_SOCIAL_NETWORK_TYPE, type);
    }
        
    // user id
        
    public string GetSocialNetworkUserId() {
        string type = GetSocialNetworkType();
        string userId = GetAttributeStringValue(
                        GetSocialNetworkKey(
                                type, 
                                BaseGameProfileAttributes.ATT_AUTH_SOCIAL_NETWORK_USERID));         
        return userId;
    }
        
    public void SetSocialNetworkUserId(string userId) {
        SetAttributeStringValue(
                        GetSocialNetworkKey(
                                GetSocialNetworkType(), 
                                BaseGameProfileAttributes.ATT_AUTH_SOCIAL_NETWORK_USERID), userId);         
    }
        
    // username
        
    public string GetSocialNetworkUserName() {
        string type = GetSocialNetworkType();
        string val = GetAttributeStringValue(
                        GetSocialNetworkKey(
                                type, 
                                BaseGameProfileAttributes.ATT_AUTH_SOCIAL_NETWORK_USERNAME));               
        return val;
    }
        
    public void SetSocialNetworkUserName(string userName) {
        SetAttributeStringValue(
                        GetSocialNetworkKey(
                                GetSocialNetworkType(), 
                                BaseGameProfileAttributes.ATT_AUTH_SOCIAL_NETWORK_USERNAME), userName);             
    }
        
    // Name
        
    public string GetSocialNetworkName() {
        string type = GetSocialNetworkType();
        string val = GetAttributeStringValue(
                        GetSocialNetworkKey(
                                type, 
                                BaseGameProfileAttributes.ATT_AUTH_SOCIAL_NETWORK_NAME));           
        return val;
    }
        
    public void SetSocialNetworkName(string name) {
        SetAttributeStringValue(
                        GetSocialNetworkKey(
                                GetSocialNetworkType(), 
                                BaseGameProfileAttributes.ATT_AUTH_SOCIAL_NETWORK_NAME), name);             
    }
        
    public string GetSocialNetworkFirstName() {
        string type = GetSocialNetworkType();
        string val = GetAttributeStringValue(
                        GetSocialNetworkKey(
                                type, 
                                BaseGameProfileAttributes.ATT_AUTH_SOCIAL_NETWORK_FNAME));          
        return val;
    }
        
    public void SetSocialNetworkFirstName(string name) {
        SetAttributeStringValue(
                        GetSocialNetworkKey(
                                GetSocialNetworkType(), 
                                BaseGameProfileAttributes.ATT_AUTH_SOCIAL_NETWORK_FNAME), name);            
    }
        
    // auth token user
        
    public string GetSocialNetworkAuthTokenUser() {
        string type = GetSocialNetworkType();
        string val = GetAttributeStringValue(
                        GetSocialNetworkKey(
                                type, 
                                BaseGameProfileAttributes.ATT_AUTH_SOCIAL_NETWORK_AUTHTOKEN_USER));         
        return val;
    }
        
    public void SetSocialNetworkAuthTokenUser(string token) {
        SetAttributeStringValue(
                        GetSocialNetworkKey(
                                GetSocialNetworkType(), 
                                BaseGameProfileAttributes.ATT_AUTH_SOCIAL_NETWORK_AUTHTOKEN_USER), token);          
    }

    /*

    // STATISTICS

    public virtual void SetStatisticValue(string code, object value) {
        double convertedValue = 0;
        if(value != null)
            convertedValue = Convert.ToDouble(value);
        DataAttribute att = new DataAttribute();
        att.val = convertedValue;
        att.code = code;
        att.name = "";
        att.type = "double";
        att.otype = "statistic";
        SetAttribute(att);
    }

    public virtual double GetStatisticValue(string code) {
        double currentValue = 0;
        object objectValue = GetAttribute(code).val;
        if(objectValue != null) {
            currentValue = Convert.ToDouble(objectValue);
            if(currentValue < 0) {
                currentValue = 0;
            }
        }

        return currentValue;
    }

    // ACHIEVEMENTS

    public virtual void SetAchievementValue(string code, object value) {
        DataAttribute att = new DataAttribute();
        att.val = value;
        att.code = code;
        att.name = code;
        att.type = "bool";
        att.otype = "achievement";
        SetAttribute(att);
    }

    public virtual bool GetAchievementValue(string code) {
        bool currentValue = false;
        object objectValue = GetAttribute(code).val;
        if(objectValue != null) {
            currentValue = Convert.ToBoolean(objectValue);
        }

        return currentValue;
    }

    public virtual List<DataAttribute> GetAchievements() {
        return GetAttributesList("achievements");
    }

    */
}