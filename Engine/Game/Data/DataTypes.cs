using System;
using System.Collections;
using System.Collections.Generic;

/*
 * ## SSC DEFAULT
 bikeColor:
    ColorItem name:Default
        r:0.333333343267441
        g:0.764705896377563
        b:0.18823529779911
        a:1
 riderColor:
    ColorItem name:Default
        r:0.333333343267441
        g:0.764705896377563
        b:0.18823529779911
        a:1
 shirtColor:
    ColorItem name:Default
        r:0.423529446125031
        g:0.447058856487274
        b:0.450980424880981
        a:1
 skinColor:
    ColorItem name:Default
        r:0.803921639919281
        g:0.588235318660736
        b:0.474509835243225
        a:1
 bootsGlovesColor:
    ColorItem name:Default
        r:0.866666734218597
        g:0.866666734218597
        b:0.866666734218597
        a:1
 bootsSleevesPants:
    ColorItem name:Default
        r:0.184313729405403
        g:0.184313729405403
        b:0.184313729405403
        a:1
 */

using Engine.Data.Json;
using Engine.Events;
using Engine.Utility;
using UnityEngine;

public class VehicleStatsData {
    public bool GotHoleShot = false;
    public bool PrematureStart = false;

    public int BoostsHit = 0;
    public int MudPuddlesHit = 0;
    public int CollisionsFromOtherBikes = 0;
    public int CollisionsOnOtherBikes = 0;
    public int LapsInFirstPlace = 0;
    public int TimesIPassedSomeone = 0;
    public int TimesPassed = 0;

    public int TimesHitOnThisLap = 0;
    public int CleanLaps = 0; // didn't hit other bikes or get hit

    public float TimeInFirstPlace = 0.0f;
    public float MilesDriven = 0.0f;
}

public class CustomColors {

    public static CustomColorItem Green {
        get {
            CustomColorItem customColor = new CustomColorItem();
            customColor.colorCode = "Green";
            customColor.r = 0.333333343267441;
            customColor.g = 0.764705896377563;
            customColor.b = 0.18823529779911;
            customColor.a = 1;
            return customColor;
        }
    }

    public static CustomColorItem DefaultBike {
        get {
            CustomColorItem customColor = new CustomColorItem();
            customColor.colorCode = "DefaultBike";
            customColor.r = 0.333333343267441;
            customColor.g = 0.764705896377563;
            customColor.b = 0.18823529779911;
            customColor.a = 1;
            return customColor;
        }
    }

    public static CustomColorItem DefaultRider {
        get {
            CustomColorItem customColor = new CustomColorItem();
            customColor.colorCode = "DefaultRider";
            customColor.r = 0.333333343267441;
            customColor.g = 0.764705896377563;
            customColor.b = 0.18823529779911;
            customColor.a = 1;
            return customColor;
        }
    }

    public static CustomColorItem DefaultShirt {
        get {
            CustomColorItem customColor = new CustomColorItem();
            customColor.colorCode = "DefaultShirt";
            customColor.r = 0.423529446125031;
            customColor.g = 0.447058856487274;
            customColor.b = 0.450980424880981;
            customColor.a = 1;
            return customColor;
        }
    }

    public static CustomColorItem Grey {
        get {
            CustomColorItem customColor = new CustomColorItem();
            customColor.colorCode = "Grey";
            customColor.r = 0.423529446125031;
            customColor.g = 0.447058856487274;
            customColor.b = 0.450980424880981;
            customColor.a = 1;
            return customColor;
        }
    }

    public static CustomColorItem DefaultSkin {
        get {
            CustomColorItem customColor = new CustomColorItem();
            customColor.colorCode = "DefaultSkin";
            customColor.r = 0.803921639919281;
            customColor.g = 0.588235318660736;
            customColor.b = 0.474509835243225;
            customColor.a = 1;
            return customColor;
        }
    }

    public static CustomColorItem White {
        get {
            CustomColorItem customColor = new CustomColorItem();
            customColor.colorCode = "White";
            customColor.r = 0.866666734218597;
            customColor.g = 0.866666734218597;
            customColor.b = 0.866666734218597;
            customColor.a = 1;
            return customColor;
        }
    }

    public static CustomColorItem DefaultGloves {
        get {
            CustomColorItem customColor = new CustomColorItem();
            customColor.colorCode = "DefaultGloves";
            customColor.r = 0.866666734218597;
            customColor.g = 0.866666734218597;
            customColor.b = 0.866666734218597;
            customColor.a = 1;
            return customColor;
        }
    }

    public static CustomColorItem DarkGrey {
        get {
            CustomColorItem customColor = new CustomColorItem();
            customColor.colorCode = "DarkGrey";
            customColor.r = 0.184313729405403;
            customColor.g = 0.184313729405403;
            customColor.b = 0.184313729405403;
            customColor.a = 1;
            return customColor;
        }
    }

    public static CustomColorItem DefaultSleevesPants {
        get {
            CustomColorItem customColor = new CustomColorItem();
            customColor.colorCode = "DefaultSleevesPants";
            customColor.r = 0.184313729405403;
            customColor.g = 0.184313729405403;
            customColor.b = 0.184313729405403;
            customColor.a = 1;
            return customColor;
        }
    }

    public static CustomColorItem Gold {
        get {
            CustomColorItem customColor = new CustomColorItem();
            customColor.colorCode = "Gold";
            customColor.r = 0.99;
            customColor.g = 0.898;
            customColor.b = 0.055;
            customColor.a = 1;
            return customColor;
        }
    }

    public static CustomColorItem GoldHighlight {
        get {
            CustomColorItem customColor = new CustomColorItem();
            customColor.colorCode = "Gold";
            customColor.r = 0.92;
            customColor.g = 0.56;
            customColor.b = 0.23;
            customColor.a = 1;
            return customColor;
        }
    }

    //1.000, 0.898, 0.055

    public static CustomColorItem Red {
        get { //1.000, 0.153, 0.067
            CustomColorItem customColor = new CustomColorItem();
            customColor.colorCode = "Red";
            customColor.r = 0.99;
            customColor.g = 0.153;
            customColor.b = 0.05;
            customColor.a = 1;
            return customColor;
        }
    }

    public static CustomColorItem Blue {
        get {
            CustomColorItem customColor = new CustomColorItem();
            customColor.colorCode = "Blue";
            customColor.r = 0.05;
            customColor.g = 0.99;
            customColor.b = 0.05;
            customColor.a = 1;
            return customColor;
        }
    }

    public static CustomColorItem GreenBright {
        get {
            CustomColorItem customColor = new CustomColorItem();
            customColor.colorCode = "GreenBright";
            customColor.r = 0.718;
            customColor.g = 1.0;
            customColor.b = 0.18;
            customColor.a = 1;
            return customColor;
        }
    }

    public static CustomColorItem SkinLight {
        get {
            CustomColorItem customColor = new CustomColorItem();
            customColor.colorCode = "SkinLight";
            customColor.r = 0.9;
            customColor.g = 0.9;
            customColor.b = 0.8;
            customColor.a = 1;
            return customColor;
        }
    }

    public static CustomPlayerColors GoldSet {
        get {
            CustomPlayerColors customColors = new CustomPlayerColors();
            customColors.bikeColor = Gold;
            customColors.bootsGlovesColor = White;
            customColors.shirtColor = Gold;
            customColors.riderColor = Gold;
            customColors.bootsSleevesPants = Gold;
            customColors.skinColor = DefaultSkin;
            return customColors;
        }
    }

    public static CustomPlayerColors GreenBrightSet {
        get {
            CustomPlayerColors customColors = new CustomPlayerColors();
            customColors.bikeColor = GreenBright;
            customColors.bootsGlovesColor = White;
            customColors.shirtColor = GreenBright;
            customColors.riderColor = GreenBright;
            customColors.bootsSleevesPants = DefaultSleevesPants;
            customColors.skinColor = DefaultSkin;
            return customColors;
        }
    }

    public static CustomPlayerColors GreySet {
        get {
            CustomPlayerColors customColors = new CustomPlayerColors();
            customColors.bikeColor = Grey;
            customColors.bootsGlovesColor = DarkGrey;
            customColors.shirtColor = DefaultShirt;
            customColors.riderColor = DarkGrey;
            customColors.bootsSleevesPants = DefaultSleevesPants;
            customColors.skinColor = DefaultSkin;
            return customColors;
        }
    }

    public static CustomPlayerColors RedSet {
        get {
            CustomPlayerColors customColors = new CustomPlayerColors();
            customColors.bikeColor = Red;
            customColors.bootsGlovesColor = Red;
            customColors.shirtColor = DefaultShirt;
            customColors.riderColor = Red;
            customColors.bootsSleevesPants = DefaultSleevesPants;
            customColors.skinColor = DefaultSkin;
            return customColors;
        }
    }

    public static CustomPlayerColors DefaultSet {
        get {
            CustomPlayerColors customColors = new CustomPlayerColors();
            customColors.bikeColor = DefaultBike;
            customColors.bootsGlovesColor = DefaultGloves;
            customColors.shirtColor = DefaultShirt;
            customColors.riderColor = DefaultRider;
            customColors.bootsSleevesPants = DefaultSleevesPants;
            customColors.skinColor = DefaultSkin;
            return customColors;
        }
    }
}

public class CustomColorItem : DataObject {
    public string colorCode;
    public double r;
    public double g;
    public double b;
    public double a;

    public CustomColorItem() {
        Reset();
    }

    public override void Reset() {
        base.Reset();
        colorCode = "default";
        r = 1.0f;
        g = 1.0f;
        b = 1.0f;
        a = 1.0f;
    }

    public void FromColor(UnityEngine.Color color) {
        r = color.r;
        g = color.g;
        b = color.b;
        a = color.a;
    }

    public UnityEngine.Color GetColor() {
        UnityEngine.Color color = new UnityEngine.Color();
        color.r = (float)r;
        color.g = (float)g;
        color.b = (float)b;
        color.a = (float)a;
        return color;
    }

    public override string ToString() {
        return String.Format("ColorItem colorCode:{0} r:{1} g:{2} b:{3} a:{4}", colorCode, r, g, b, a);
    }
}


public class CustomPlayerColorsRunner : DataObject {
    public string colorCode;
    public string colorDisplayName;

    public CustomColorItem helmetColor;
    public CustomColorItem helmetFacemaskColor;
    public CustomColorItem helmetHighlightColor;
    public CustomColorItem jerseyColor;
    public CustomColorItem jerseyHighlightColor;
    public CustomColorItem pantsColor;
	
    public CustomColorItem extra1Color;
    public CustomColorItem extra2Color;
    public CustomColorItem extra3Color;

    public CustomPlayerColorsRunner() {
        Reset();
    }

    public override void Reset() {
        base.Reset();

        colorCode = "default";
        colorDisplayName = "Default";
		//SetMaterialColor name:helmet-facemask color:RGBA(0.838, 1.000, 0.595, 1.000)
		//SetMaterialColor name:helmet-main color:RGBA(1.000, 0.189, 0.192, 1.000)


        helmetColor = new CustomColorItem();
		helmetColor.FromColor(new Color(0.979f, 0.943f, 0.938f, 1.000f));
		
        helmetFacemaskColor = new CustomColorItem();
		helmetFacemaskColor.FromColor(new Color(0.973f, 0.974f, 0.991f, 1.000f));
		
        helmetHighlightColor = new CustomColorItem();
		helmetHighlightColor.FromColor(new Color(0.442f, 0.114f, 0.067f, 1.000f));
		
        jerseyColor = new CustomColorItem();
		jerseyColor.FromColor(new Color(0.448f, 0.093f, 0.042f, 1.000f));
		
        jerseyHighlightColor = new CustomColorItem();
		jerseyHighlightColor.FromColor(new Color(0.974f, 0.955f, 0.952f, 1.000f));
		
        pantsColor = new CustomColorItem();
		pantsColor.FromColor(new Color(0.979f, 0.943f, 0.938f, 1.000f));
		
        extra1Color = new CustomColorItem();
		extra1Color.FromColor(new Color(1.000f, 0.189f, 0.192f, 1.000f));
		
        extra2Color = new CustomColorItem();
		extra2Color.FromColor(new Color(1.000f, 0.189f, 0.192f, 1.000f));
		
        extra3Color = new CustomColorItem();
		extra3Color.FromColor(new Color(1.000f, 0.189f, 0.192f, 1.000f));
    }

    public override string ToString() {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        sb.Append(String.Format("CustomPlayerColors name: {0}", colorDisplayName));
        sb.Append(String.Format("\r\n\r\n helmetColor: {0}", helmetColor.ToString()));
        sb.Append(String.Format("\r\n\r\n helmetFacemaskColor: {0}", helmetFacemaskColor.ToString()));
        sb.Append(String.Format("\r\n\r\n helmetHighlightColor: {0}", helmetHighlightColor.ToString()));
        sb.Append(String.Format("\r\n\r\n jerseyColor: {0}", jerseyColor.ToString()));
        sb.Append(String.Format("\r\n\r\n jerseyHighlightColor: {0}", jerseyHighlightColor.ToString()));
        sb.Append(String.Format("\r\n\r\n pantsColor: {0}", pantsColor.ToString()));
        sb.Append(String.Format("\r\n\r\n extra1Color: {0}", extra1Color.ToString()));
        sb.Append(String.Format("\r\n\r\n extra2Color: {0}", extra2Color.ToString()));
        sb.Append(String.Format("\r\n\r\n extra3Color: {0}", extra3Color.ToString()));
        return sb.ToString();
    }
}

public class CustomPlayerColors : DataObject {
    public string colorCode;
    public string colorDisplayName;

    public CustomColorItem bikeColor;
    public CustomColorItem riderColor;
    public CustomColorItem shirtColor;
    public CustomColorItem skinColor;
    public CustomColorItem bootsGlovesColor;
    public CustomColorItem bootsSleevesPants;

    public CustomPlayerColors() {
        Reset();
    }

    public override void Reset() {
        base.Reset();

        colorCode = "default";
        colorDisplayName = "Default";

        bikeColor = new CustomColorItem();
        riderColor = new CustomColorItem();
        shirtColor = new CustomColorItem();
        skinColor = new CustomColorItem();
        bootsGlovesColor = new CustomColorItem();
        bootsSleevesPants = new CustomColorItem();
    }

    public override string ToString() {
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        sb.Append(String.Format("CustomPlayerColors name: {0}", colorDisplayName));
        sb.Append(String.Format("\r\n\r\nbikeColor: {0}", bikeColor.ToString()));
        sb.Append(String.Format("\r\n\r\nriderColor: {0}", riderColor.ToString()));
        sb.Append(String.Format("\r\n\r\nshirtColor: {0}", shirtColor.ToString()));
        sb.Append(String.Format("\r\n\r\nskinColor: {0}", skinColor.ToString()));
        sb.Append(String.Format("\r\n\r\nbootsGlovesColor: {0}", bootsGlovesColor.ToString()));
        sb.Append(String.Format("\r\n\r\nbootsSleevesPants: {0}", bootsSleevesPants.ToString()));
        return sb.ToString();
    }
}

public class CustomPlayerAudioItem : DataObject {
    public bool useCustom = false;
}

public class CustomPlayerAudioKeys {
    public static string audioBikeRevving = "audio-bike-revving";
    public static string audioBikeRacing = "audio-bike-racing";
    public static string audioBikeBoosting = "audio-bike-boosting";
    public static string audioCrowdCheer = "audio-crowd-cheer";
    public static string audioCrowdJump = "audio-crowd-jump";
    public static string audioCrowdBoo = "audio-crowd-boo";
}

public class CustomPlayerAudio : DataObject {
    public Dictionary<string, CustomPlayerAudioItem> audioItems = new Dictionary<string, CustomPlayerAudioItem>();

    public CustomPlayerAudioItem GetSoundBikeRevving() {
        return GetAudioItem(CustomPlayerAudioKeys.audioBikeRevving);
    }

    public CustomPlayerAudioItem GetSoundBikeRacing() {
        return GetAudioItem(CustomPlayerAudioKeys.audioBikeRacing);
    }

    public CustomPlayerAudioItem GetSoundBikeBoosting() {
        return GetAudioItem(CustomPlayerAudioKeys.audioBikeBoosting);
    }

    public CustomPlayerAudioItem GetSoundCrowdCheer() {
        return GetAudioItem(CustomPlayerAudioKeys.audioCrowdCheer);
    }

    public CustomPlayerAudioItem GetSoundCrowdJump() {
        return GetAudioItem(CustomPlayerAudioKeys.audioCrowdJump);
    }

    public CustomPlayerAudioItem GetSoundCrowdBoo() {
        return GetAudioItem(CustomPlayerAudioKeys.audioCrowdBoo);
    }

    public CustomPlayerAudioItem GetAudioItem(string key) {
        if (audioItems != null) {
            if (audioItems.ContainsKey(key)) {
                return audioItems[key];
            }
        }
        return new CustomPlayerAudioItem();
    }

    public void SetAudioItem(string key, CustomPlayerAudioItem audioItem) {
        if (audioItems == null) {
            audioItems = new Dictionary<string, CustomPlayerAudioItem>();
        }
        if (audioItems != null
           && !string.IsNullOrEmpty(key)) {
            if (audioItems.ContainsKey(key)) {
                audioItems[key] = audioItem;
            }
            else {
                audioItems.Add(key, audioItem);
            }
        }
    }
}

public class DisplayRecord {
    public string indexString = "";
    public string nameString = "";
    public string descriptionString = "";
    public string valueString = "";
    public bool isHeader = false;

    public DisplayRecord(string index, string name, string description, string value, bool isHeaderItem) {
        indexString = index;
        nameString = name;
        descriptionString = description;
        valueString = value;
        isHeader = isHeaderItem;
    }

    public DisplayRecord(string index, string name, string description, string value) {
        indexString = index;
        nameString = name;
        descriptionString = description;
        valueString = value;
        isHeader = false;
    }

    public DisplayRecord(string index, string name, bool isHeaderItem) {
        indexString = index;
        nameString = name;
        descriptionString = "--";
        valueString = "--";
        isHeader = isHeaderItem;
    }
}

public class DisplayRecordAchievement {
    public string indexString = "";
    public string nameString = "";
    public string descriptionString = "";
    public string valueString = "";
    public bool completedState = false;
    public bool isHeader = false;

    public DisplayRecordAchievement(string index, string name, string description, string value, bool completed, bool isHeaderItem) {
        indexString = index;
        nameString = name;
        descriptionString = description;
        valueString = value;
        completedState = completed;
        isHeader = isHeaderItem;
    }

    public DisplayRecordAchievement(string index, string name, string description, string value, bool completed) {
        indexString = index;
        nameString = name;
        descriptionString = description;
        valueString = value;
        completedState = completed;
        isHeader = false;
    }

    public DisplayRecordAchievement(string index, string name, bool isHeaderItem) {
        indexString = index;
        nameString = name;
        descriptionString = "--";
        valueString = "--";
        completedState = false;
        isHeader = isHeaderItem;
    }
}

public class BaseEntity : DataObject {

    public string status;

    public string uuid;

    public DateTime date_modified;

    public bool active;

    public DateTime date_created;

    public string type;

    public BaseEntity() {
        Reset();
    }

    public override void Reset() {
        base.Reset();
        status = "";
        uuid = "";
        date_modified = DateTime.Now;
        date_created = DateTime.Now;
        type = "";
        active = true;
    }
}

public class BaseResponse {

    public string message { get; set; }

    public string action { get; set; }

    public int error { get; set; }

    public string code { get; set; }

    public Dictionary<string, string> info { get; set; }

    public BaseResponse() {
        Reset();
    }

    public virtual void Reset() {
        message = "";
        action = "";
        error = 0;
        code = "";
        info = new Dictionary<string, string>();
    }
}

public class BaseMeta : BaseEntity {

    public string code;

    public string parent_code;

    public string display_name;

    public string name;

    public string description;

    public BaseMeta() {
        Reset();
    }

    public override void Reset() {
        base.Reset();
        parent_code = "";
        code = "";
        display_name = "";
        name = "";
        description = "";
    }
}

public class BaseSerializable<T> {

    public string fileName { get; set; }

    public string objectString { get; set; }

    public BaseSerializable() {
        Reset();
    }

    public void Reset() {
        fileName = "";
        objectString = "";
    }

    public void Save(string fileName) {
        string objectString = JsonMapper.ToJson(this);
#if !UNITY_WEBPLAYER
        FileSystemUtil.WriteString(fileName, objectString);
#endif
    }

    public void Load(string fileName) {
#if !UNITY_WEBPLAYER
        string objectString = FileSystemUtil.ReadString(fileName);
        Debug.Log(objectString);

        //this = JsonMapper.ToObject<T>(objectString);
#endif
    }
}

public class AchievementMeta : GameDataObject {

    public int sort { get; set; }

    public bool game_stat { get; set; }

    public string level { get; set; }

    public string data { get; set; }

    public bool global { get; set; }

    public int points { get; set; }

    public bool leaderboard { get; set; }

    public AchievementMeta() {
        Reset();
    }

    public override void Reset() {
        base.Reset();
        sort = 0;
        game_stat = true;
        level = "";
        data = "";
        global = true;
        points = 0;
        key = "";
        game_id = "";
        type = "";
        leaderboard = true;
    }
}

public class AchievementMetaResponse : BaseResponse {

    public List<AchievementMeta> data { get; set; }

    public AchievementMetaResponse() {
        Reset();
    }

    public override void Reset() {
        base.Reset();
        data = new List<AchievementMeta>();
    }
}

public class Achievement {
    public bool value = false;
    public string key = "";

    public Achievement() {
        Reset();
    }

    public virtual void Reset() {
        value = false;
        key = "";
    }
}

public class Statistic {
    public string key = "";
    public double value = 0.0001;

    public Statistic() {
        Reset();
    }

    public virtual void Reset() {
        key = "";
        value = 0.0001;
    }
}

public class StatisticMeta : GameDataObject {

    public int sort { get; set; }

    public int store_count { get; set; }

    public string data { get; set; }

    public string order { get; set; }

    public StatisticMeta() {
        Reset();
    }

    public override void Reset() {
        base.Reset();
        sort = 0;
        store_count = 0;
        data = "";
        key = "";
        game_id = "";
        type = "";
        order = "";
    }
}

public class StatisticMetaResponse : BaseResponse {

    public List<StatisticMeta> data { get; set; }

    public StatisticMetaResponse() {
        Reset();
    }

    public override void Reset() {
        base.Reset();
        data = new List<StatisticMeta>();
    }
}

public class Level : GameDataObject {

    public int sort { get; set; }

    public string data { get; set; }

    public string order { get; set; }

    public float value = 0.0f;

    public Level() {
        Reset();
    }

    public override void Reset() {
        base.Reset();
        sort = 0;
        data = "";
        key = "";
        game_id = "";
        type = "";
        order = "";
    }
}

public class LevelResponse : BaseResponse {

    public List<Level> data { get; set; }

    public LevelResponse() {
        Reset();
    }

    public override void Reset() {
        base.Reset();
        data = new List<Level>();
    }
}

public class KeyMeta : BaseMeta {

    public int sort { get; set; }

    public int store_count { get; set; }

    public string level { get; set; }

    public string data { get; set; }

    public string key_level { get; set; }

    public string key_stat { get; set; }

    public string key { get; set; }

    public string game_id { get; set; }

    public string order { get; set; }

    public KeyMeta() {
        Reset();
    }

    public override void Reset() {
        base.Reset();
        sort = 0;
        store_count = 0;
        level = "";
        data = "";
        key_level = "";
        key_stat = "";
        key = "";
        game_id = "";
        type = "";
        order = "";
    }
}

public class KeyMetaResponse : BaseResponse {

    public List<KeyMeta> data { get; set; }

    public KeyMetaResponse() {
        Reset();
    }

    public override void Reset() {
        base.Reset();
        data = new List<KeyMeta>();
    }
}

public class StatisticLeaderboard : BaseEntity {

    public string username { get; set; }

    public int rank_change { get; set; }

    public float timestamp { get; set; }

    public string level { get; set; }

    public string stat_value_formatted { get; set; }

    public string profile_id { get; set; }

    public int rank_total_count { get; set; }

    public int rank { get; set; }

    public string key { get; set; }

    public string game_id { get; set; }

    public string data { get; set; }

    public float stat_value { get; set; }

    public StatisticLeaderboard() {
        Reset();
    }

    public override void Reset() {
        base.Reset();
        username = "";
        rank_change = 0;
        timestamp = 0.0f;
        level = "";
        stat_value_formatted = "";
        profile_id = "";
        rank_total_count = 0;
        rank = 0;
        key = "";
        type = "";
        game_id = "";
        data = "";
        stat_value = 0.0f;
    }
}

// ---------------------------------------------------------
// PROFILE REMOTE

public class ProfileCheck {
    public string uuid = "";
    public string username = "";
    public string hash = "";
    public bool exists = false;

    public ProfileCheck() {
        Reset();
    }

    public virtual void Reset() {
        uuid = "";
        username = "";
        hash = "";
        exists = false;
    }
}

public class ProfileDevice {
    public string udid = "";
    public string uuid = "";
    public string profile_id = "";
    public string hash = "";
    public DateTime date_modified;
    public DateTime date_created;
    public string status = "";
    public bool active = false;

    public ProfileDevice() {
        Reset();
    }

    public virtual void Reset() {
        udid = "";
        uuid = "";
        profile_id = "";
        hash = "";
        date_modified = DateTime.Now;
        date_created = DateTime.Now;
        status = "";
        active = false;
    }
}

public class ProfileCheckResponse : BaseResponse {

    public ProfileCheck data { get; set; }

    public ProfileCheckResponse() {
        Reset();
    }

    public override void Reset() {
        base.Reset();
        data = new ProfileCheck();
    }
}

public class ProfileDeviceResponse : BaseResponse {

    public List<ProfileDevice> data { get; set; }

    public ProfileDeviceResponse() {
        Reset();
    }

    public override void Reset() {
        base.Reset();
        data = new List<ProfileDevice>();
    }
}

public class ProfileSet {
    public ProfileData profile;
    public bool successfulSet = false;

    public ProfileSet() {
        Reset();
    }

    public virtual void Reset() {
        profile = new ProfileData();
        successfulSet = false;
    }
}

public class ProfileSetResponse : BaseResponse {

    public ProfileSet data { get; set; }

    public ProfileSetResponse() {
        Reset();
    }

    public override void Reset() {
        base.Reset();
        data = new ProfileSet();
    }
}

public class ProfileData : BaseEntity {

    // Used for sets if user wants to change anything

    public string username { get; set; }

    public string city { get; set; }

    public string first_name { get; set; }

    public string last_name { get; set; }

    public string zip { get; set; }

    public string twittername { get; set; }

    public System.DateTime dob { get; set; }

    public string address1 { get; set; }

    public string address2 { get; set; }

    public string ip { get; set; }

    public string gender { get; set; }

    public string state_province { get; set; }

    public string country { get; set; }

    public string password { get; set; }

    public string email { get; set; }

    public ProfileData() {
        Reset();
    }

    public void HashPasswordSHA1OneWay() {
        password = CryptoUtil.CalculateBase64SHA1TrimASCII(password);
    }

    public override void Reset() {
        base.Reset();
        username = "";
        last_name = "";
        address1 = "";
        address2 = "";
        password = "";
        state_province = "";
        city = "";
        first_name = "";
        uuid = "";
        zip = "";
        dob = DateTime.Now;
        gender = "m";
        twittername = "";
        ip = "";
        country = "US";
        email = "";
    }
}

public class ProfileResponse : BaseResponse {

    public ProfileData data { get; set; }

    public ProfileResponse() {
        Reset();
    }

    public override void Reset() {
        base.Reset();
        data = new ProfileData();
    }
}

public class ProfilePublic {

    // Used for gets, only shows public info
    // TODO add profile attributes udid, ip etc to make private/public
    public ProfilePublic() {
        Reset();
    }

    public string uuid { get; set; }

    public string username { get; set; }

    public string city { get; set; }

    public string first_name { get; set; }

    public string last_name { get; set; }

    public string zip { get; set; }

    public string twittername { get; set; }

    public string gender { get; set; }

    public string state_province { get; set; }

    public string country { get; set; }

    public virtual void Reset() {
        uuid = "";
        username = "";
        last_name = "";
        state_province = "";
        city = "";
        first_name = "";
        uuid = "";
        zip = "";
        gender = "m";
        twittername = "";
        country = "US";
    }
}

public class ProfilePublicResponse : BaseResponse {

    public ProfilePublic data { get; set; }

    public ProfilePublicResponse() {
        Reset();
    }

    public override void Reset() {
        base.Reset();
        data = new ProfilePublic();
    }
}

public class ProfileLocal {

    public ProfileLocal() {
        Reset();
    }

    public string username { get; set; }

    public string authhash { get; set; }

    public string uuid { get; set; }

    public bool registered { get; set; }

    public bool registrationStarted { get; set; }

    public void Reset() {
        uuid = UniqueUtil.Instance.currentUniqueId;
        username = "player-" + uuid;
        authhash = "1111";
        registered = false;
        registrationStarted = false;
    }
}

public class LocalConfig {

    public LocalConfig() {
        Reset();
    }

    public string profileCurrentUsername { get; set; }

    public string profileCurrentUuid { get; set; }

    public void Reset() {
        profileCurrentUuid = UniqueUtil.Instance.currentUniqueId;
        profileCurrentUsername = "player-" + profileCurrentUuid;
    }
}

public enum StatEqualityTypeEnum {
    STAT_GREATER_THAN_OR_EQUAL_TO, 	// "greater-than-or-equal-to"
    STAT_LESS_THAN_OR_EQUAL_TO,		// "less-than-or-equal-to"
    STAT_GREATER_THAN,				// "greater-than"
    STAT_LESS_THAN,					// "less-than"
    EQUAL_TO						// "equal-to"
}

public class StatEqualityTypeString {
    public static string STAT_GREATER_THAN_OR_EQUAL_TO = "greater-than-or-equal-to";
    public static string STAT_LESS_THAN_OR_EQUAL_TO = "less-than-or-equal-to";
    public static string STAT_GREATER_THAN = "greater-than";
    public static string STAT_LESS_THAN = "less-than";
    public static string EQUAL_TO = "equal-to";

    public static StatEqualityTypeEnum GetEnum(string statEqualityType) {
        if (statEqualityType == EQUAL_TO) {
            return StatEqualityTypeEnum.EQUAL_TO;
        }
        else if (statEqualityType == STAT_LESS_THAN) {
            return StatEqualityTypeEnum.STAT_LESS_THAN;
        }
        else if (statEqualityType == STAT_GREATER_THAN) {
            return StatEqualityTypeEnum.STAT_GREATER_THAN;
        }
        else if (statEqualityType == STAT_LESS_THAN_OR_EQUAL_TO) {
            return StatEqualityTypeEnum.STAT_LESS_THAN_OR_EQUAL_TO;
        }
        else if (statEqualityType == STAT_GREATER_THAN_OR_EQUAL_TO) {
            return StatEqualityTypeEnum.STAT_GREATER_THAN_OR_EQUAL_TO;
        }
        else {
            return StatEqualityTypeEnum.STAT_GREATER_THAN_OR_EQUAL_TO;
        }
    }

    public string GetStatEqualityTypeString(StatEqualityTypeEnum statEqualityType) {
        if (statEqualityType == StatEqualityTypeEnum.EQUAL_TO) {
            return EQUAL_TO;
        }
        else if (statEqualityType == StatEqualityTypeEnum.STAT_LESS_THAN) {
            return STAT_LESS_THAN;
        }
        else if (statEqualityType == StatEqualityTypeEnum.STAT_GREATER_THAN) {
            return STAT_GREATER_THAN;
        }
        else if (statEqualityType == StatEqualityTypeEnum.STAT_LESS_THAN_OR_EQUAL_TO) {
            return STAT_LESS_THAN_OR_EQUAL_TO;
        }
        else if (statEqualityType == StatEqualityTypeEnum.STAT_GREATER_THAN_OR_EQUAL_TO) {
            return STAT_GREATER_THAN_OR_EQUAL_TO;
        }
        else {
            return "";
        }
    }
}