using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Engine.Events;
using Engine.Data.Json;

public class BaseDataObjectKeys {  
    
    public static string none = "none";
    public static string id = "id";
    public static string uid = "uid";
    public static string profile = "profile";
    public static string profile_id = "profile_id";
    public static string game_id = "game_id";
    public static string uuid = "uuid";
    public static string code = "code";
    public static string name = "name";
    public static string data_type = "data_type";
    public static string compare_type = "compare_type";
    public static string display_type = "display_type";
    public static string description = "description";
    public static string display_name = "display_name";
    public static string action_description = "action_description";
    public static string action_display_name = "action_display_name";
    public static string attributes = "attributes";
    public static string url = "url";
    public static string path = "path";
    public static string content = "content";
    public static string contents = "contents";
    public static string upload = "upload";
    public static string hash = "hash";
    public static string file = "file";
    public static string files = "files";
    public static string host = "host";
    //
    public static string empty = "empty";
    public static string random = "random";
    //
    public static string complete = "complete";
    public static string incomplete = "incomplete";
    public static string star = "star";
    public static string stars = "stars";
    //
    public static string data = "data";
    public static string data_suffix_list = "data_suffix_list";
    public static string data_list = "data_list";
    public static string data_object = "data_object";
    public static string data_items = "data_items";
    public static string data_game_objects = "data_game_objects";
    //
    public static string level_num_suffix_list = "level_num_suffix_list";
    public static string world_num_list = "world_num_list";
    public static string level_list = "level_list";
    public static string world_list = "world_list";
    //
    public static string strings = "strings";
    public static string images = "images";
    public static string info = "info";
    public static string error = "error";
    public static string datatype = "datatype";
    public static string direction = "direction";
    //
    public static string network_id = "network_id";
    public static string network_username = "network_username";
    public static string network_name = "network_name";
    public static string network_first_name = "network_first_name";
    public static string network_last_name = "network_last_name";
    public static string network_type = "network_type";
    public static string network_token = "network_token";
    public static string network_data = "network_data";
    public static string network_items = "network_items";
    public static string network = "network";
    public static string networks = "networks";
    //    
    public static string level = "level";
    public static string levels = "levels";
    public static string level_num = "level_num";
    public static string level_code = "level_code";
    public static string level_data = "level_data";
    public static string level_items = "level_items";
    public static string level_type = "level_type";
    //
    public static string height = "height";
    public static string width = "width";
    public static string depth = "depth";
    //
    public static string levelAssets = "level-assets";
    //
    public static string world = "world";
    public static string worlds = "worlds";
    public static string world_data = "world_data";
    public static string world_num = "world_num";
    public static string world_code = "world_code";
    public static string world_type = "world_type";
    public static string world_items = "world_items";
    //
    public static string vehicle = "vehicle";
    public static string vehicles = "vehicles";
    public static string vehicle_data = "vehicle_data";
    public static string vehicle_num = "vehicle_num";
    public static string vehicle_code = "vehicle_code";
    public static string vehicle_type = "vehicle_type";
    public static string vehicle_items = "vehicle_items";
    //
    public static string mission = "mission";
    public static string missions = "missions";
    public static string mission_data = "mission_data";
    public static string mission_code = "mission_code";
    public static string mission_type = "mission_type";
    public static string mission_key = "mission_key";
    public static string mission_items = "mission_items";
    //    
    public static string app_content_collect = "app_content_collect";
    public static string app_content_collects = "app_content_collects";
    public static string app_content_collect_data = "app_content_collect_data";
    public static string app_content_collect_code = "app_content_collect_code";
    public static string app_content_collect_type = "app_content_collect_type";
    public static string app_content_collect_key = "app_content_collect_key";
    public static string app_content_collect_items = "app_content_collect_items";
    //
    public static string action = "action";
    public static string actions = "actions";
    public static string action_data = "action_data";
    public static string action_code = "action_code";
    public static string action_type = "action_type";
    public static string action_key = "action_key";
    public static string action_items = "action_items";
    //
    public static string collection_data = "collection_data";
    public static string collection_code = "collection_code";
    public static string collection_type = "collection_type";
    public static string collection_items = "collection_items";
    //
    public static string choice_data = "choice_data";
    public static string choice_code = "choice_code";
    public static string choice_type = "choice_type";
    public static string choice_items = "choice_items";
    //
    public static string broadcast_networks_record_levels = "broadcast_networks_record_levels";
    //
    public static string data_platform = "data_platform";
    public static string sort_order = "sort_order";
    public static string sort_order_type = "sort_order_type";
    public static string active = "active";
    public static string key = "key";
    public static string meta = "meta";
    public static string locale = "locale";
    public static string symbol = "symbol";
    public static string price = "price";
    public static string cost = "cost";
    public static string platform = "platform";
    public static string platforms = "platforms";
    public static string product_id = "product_id";
    public static string type = "type";
    public static string game = "game";
    public static string games = "games";
    public static string order_by = "order_by";
    public static string status = "status";
    //
    public static string pack_code = "pack_code";
    public static string pack_sort = "pack_sort";
    public static string date_created = "date_created";
    public static string date_modified = "date_modified";
    public static string email = "email";
    public static string username = "username";
    public static string udid = "udid";
    public static string file_name = "file_name";
    public static string file_path = "file_path";
    public static string file_full_path = "file_full_path";
    public static string keys = "keys";
    public static string properties = "properties";
    public static string types = "types";
    //
    // characters

    public static string character = "character";
    public static string character_skin = "character_skin";
    public static string character_skin_variation = "character_skin_variation";
    public static string character_data = "character_data";
    public static string character_items = "character_items";
    public static string social_data = "social_data";
    public static string social_items = "social_items";
    //
    // weapons

    public static string weapon = "weapon";
    public static string weapon_skin = "weapon_skin";
    public static string weapon_skin_variation = "weapon_skin_variation";
    public static string weapon_data = "weapon_data";
    public static string weapon_items = "weapon_items";
    //
    // items
    
    public static string item = "item";
    public static string items = "items";
    public static string item_skin = "item_skin";
    public static string item_skin_variation = "item_skin_variation";
    public static string item_data = "item_data";
    public static string item_items = "item_items";
    //
    // locales
    
    public static string any = "any";
    public static string all = "all";
    public static string en = "en";
    public static string sp = "sp";
    public static string de = "de";
    public static string it = "it";
    public static string fr = "fr";
    public static string jp = "jp";
    public static string cn = "cn";
    public static string ko = "ko";
    //
    // custom

    public static string custom_items = "custom_items";
    public static string custom_materials = "custom_materials";
    public static string custom_textures = "custom_textures";
    public static string custom_models = "custom_models";
    public static string current_texture_preset = "current_texture_preset";
    public static string current_color_preset = "current_color_preset";
    public static string color_presets = "color_presets";
    public static string directors = "directors";
    public static string texture_presets = "texture_presets";
    public static string item_presets = "item_presets";
    public static string terrain_presets = "terrain_presets";
    public static string asset_presets = "asset_presets";
    public static string layout_presets = "layout_presets";
    public static string game_level_item_asset = "game_level_item_asset";
    public static string start_position = "start_position";
    public static string physics_type = "physics_type";
    public static string destructable = "destructable";
    public static string reactive = "reactive";
    public static string kinematic = "kinematic";
    public static string gravity = "gravity";
    public static string asset_scale = "range_scale";
    public static string asset_rotation = "range_rotation";
    public static string destroy_effect_code = "destroy_effect_code";
    public static string speed_rotation = "speed_rotation";
    public static string destroyed = "destroyed";
    public static string steps = "steps";
    public static string character_presets = "character_presets";
    public static string model = "model";
    public static string models = "models";
    public static string presets = "presets";
    public static string roles = "roles";
    public static string sound = "sound";
    public static string sounds = "sounds";
    public static string animation = "animation";
    public static string animations = "animations";
    public static string animator = "animator";
    public static string animation_state = "animation_state";
    public static string animation_type = "animation_type";
    public static string play_type = "play_type";
    public static string play_delay = "play_delay";
    public static string layer = "layer";
    public static string projectile = "projectile";
    public static string projectiles = "projectiles";
    public static string effect = "effect";
    public static string effects = "effects";

    //roles

    public static string color = "color";
    public static string colors = "colors";
    public static string asset = "asset";
    public static string assets = "assets";
    public static string val = "val";
    public static string vals = "vals";
    public static string last_update = "last_update";
    public static string last_time = "last_time";
    public static string delta = "delta";
    public static string modifier = "modifier";
    public static string texture = "texture";
    public static string textures = "textures";
    public static string material = "material";
    public static string materials = "materials";
    //
    // asset items
    public static string delay = "delay";
    public static string time = "time";
    public static string ease_type = "ease_type";
    public static string local_position_data = "local_position_data";
    public static string local_rotation_data = "local_rotation_data";
    public static string local_scale_data = "local_scale_data";
    public static string position_data = "position_data";
    public static string grid_data = "grid_data";
    public static string rotation_data = "rotation_data";
    public static string scale_data = "scale_data";
    //delay
    //
    // RPG
    
    public static string rpg = "rpg";
    public static string rpgs = "rpgs";
    public static string reward = "reward";
    public static string rewards = "rewards";
    public static string duration = "duration";
    public static string limit = "limit";
    public static string max = "max";
    public static string min = "min";
    public static string frequency = "frequency";
    public static string probability = "probability";
    public static string scale = "scale";
    public static string speed = "speed";
    public static string attack = "attack";
    public static string defense = "defense";
    public static string health = "health";
    public static string energy = "energy";
    public static string jump = "jump";
    public static string fly = "fly";
    public static string boost = "boost";
    public static string attack_speed = "attack_speed";
    public static string recharge_speed = "recharge_speed";
    public static string upgrades_applied = "upgrades_applied";
    public static string upgrades = "upgrades";
    public static string xp = "xp";
    public static string power = "power";
    public static string currency = "currency";
    //
    // content
    
    public static string tags = "tags";
    public static string app_states = "app_states";
    public static string app_content_states = "app_content_states";
    public static string required_assets = "required_assets";
    public static string required_packs = "required_packs";
    public static string required_trackers = "required_trackers";
    public static string content_attributes = "content_attributes";
    public static string choices = "choices";
    //
    // missions

    public static string amount = "amount";
    public static string run = "run";
    //
    // achievements
    
    public static string filter = "filter";
    public static string filters = "filters";
    public static string defaultKey = "defaultKey";
    public static string pack = "pack";
    public static string tracker = "tracker";
    public static string app_state = "app_state";
    public static string app_content_state = "app_content_state";
    public static string custom = "custom";
    public static string codes = "codes";
    public static string codeType = "codeType";
    public static string compareType = "compareType";
    public static string compareValue = "compareValue";
    public static string includeKeys = "includeKeys";
    public static string codeCompareTo = "codeCompareTo";
    public static string codeLike = "codeLike";
    public static string points = "points";
    public static string leaderboard = "leaderboard";
    public static string game_stat = "game_stat";
    public static string global = "global";

}

public class BaseDataObject : Dictionary<string, object> {  

    //public Dictionary<string, DataAttribute> attributes;

    public BaseDataObject() {
        
    }
    
    // VALUES
    // use keyed dictionaries for all data objects to prevent
    // serialize and deserialize issues with keys on versions.    

    //[JsonIgnore(JsonIgnoreWhen.Deserializing)]
    public virtual Dictionary<string, DataAttribute> attributes {
        get {
            return Get<Dictionary<string, DataAttribute>>(BaseDataObjectKeys.attributes);
        }
        
        set {
            Set(BaseDataObjectKeys.attributes, value);
        }
    } 

    // -----------------------------------------------------------------------
    // VALUE ACCESSORS

    // generics

    public virtual T Get<T>(string code) {
        return Get<T>(code, default(T));
    }
    
    public virtual T Get<T>(string code, T defaultValue) {                
        try {
            if (ContainsKey(code)) {
                return (T)this[code];
            }
            return defaultValue;
        }
        catch (Exception e) {
            LogUtil.Log(e);
            return default(T);
        }
    }

    // typed gets

    /*
    public virtual object Get(string code) {
        return Get<object>(code, null);
    }
    
    public virtual object Get(string code, object defaultValue) {
        return Get<object>(code, defaultValue);;
    }
    */

    // sets
    
    public virtual void Set<T>(string code, T val) {
        if (ContainsKey(code)) {
            this[code] = val;
        }
        else {
            Add(code, val);
        }
    }

    public virtual void Set(string code, object val) {
        if (ContainsKey(code)) {
            this[code] = val;
        }
        else {
            Add(code, val);
        }
    }
    
    public virtual void Set(string code, DataAttribute val) {
        if (attributes == null) {
            attributes = new Dictionary<string, DataAttribute>();                        
        }


        if (attributes.ContainsKey(code)) {
            attributes[code] = val;
        }
        else {
            attributes.Add(code, val);
        }
    }

    //
    
    public virtual void Clone(DataObject toCopy) {
        attributes = toCopy.attributes;
    }
    
    // -----------------------------------------------------------------------
    // LOADING/SAVING

    public string DataPrepareSave(string dataValue) {
        
        #if !UNITY_WEBPLAYER
        if (AppConfigs.useStorageEncryption) {
            dataValue = dataValue.ToEncrypted();
        }
        
        if (AppConfigs.useStorageCompression) {
            dataValue = dataValue.ToCompressed();
        }
        #endif

        return dataValue;
    }

    public string DataPrepareLoad(string dataValue) {
        
        #if !UNITY_WEBPLAYER
        if (AppConfigs.useStorageCompression) { /// || data.IsCompressed()) {
            dataValue = dataValue.ToDecompressed();
        }
        
        if (AppConfigs.useStorageEncryption) {
            dataValue = dataValue.ToDecrypted();
        }
        #endif

        return dataValue;
    }

    public string LoadDataFromResources(string path) {

        string dataValue = "";
        
        TextAsset textData = AssetUtil.LoadAsset<TextAsset>(path);      
        if (textData != null) {
            dataValue = textData.text;
        }
        
        dataValue = DataPrepareLoad(dataValue);

        return dataValue;
    }
    
    public string LoadDataFromPrefs(string key) {

        string dataValue = "";
        
        if (!SystemPrefUtil.HasLocalSetting(key)) {
            dataValue = SystemPrefUtil.GetLocalSettingString(key);
        }
        
        dataValue = DataPrepareLoad(dataValue);

        return dataValue;
    }

    public string GetWebPath(string path) {

        if (!string.IsNullOrEmpty(Application.dataPath)) {
            path = path.Replace(Application.dataPath, "");
        }

        if (!string.IsNullOrEmpty(Application.persistentDataPath)) {
            path = path.Replace(Application.persistentDataPath, "");
        }

        return path;
    }
    
    public string LoadData(string fileFullPath) {

        string dataValue = "";
        
        #if !UNITY_WEBPLAYER 
        if (FileSystemUtil.CheckFileExists(fileFullPath)) {       
            dataValue = FileSystemUtil.ReadString(fileFullPath);
        }
        #else
        string fileWebPath = GetWebPath(fileFullPath);
        dataValue = SystemPrefUtil.GetLocalSettingString(fileWebPath);
        #endif  
        
        dataValue = DataPrepareLoad(dataValue);

        return dataValue;
    }
    
    public T LoadData<T>(string fileFullPath, string fileKey) {
        string dataValue = "";
        #if !UNITY_WEBPLAYER
        string path = PathUtil.Combine(fileFullPath, (fileKey + ".json").TrimStart('/'));
        if (FileSystemUtil.CheckFileExists(path)) {       
            dataValue = FileSystemUtil.ReadString(path);
        }
        #else
        string fileWebPath = GetWebPath(fileFullPath);
        dataValue = SystemPrefUtil.GetLocalSettingString(fileWebPath);
        #endif 

        if (!string.IsNullOrEmpty(dataValue)) {
            dataValue = DataPrepareLoad(dataValue);
            return JsonMapper.ToObject<T>(dataValue);
        }
        
        return default(T);
    }
    
    public void SaveData(string folderPath, string fileKey, object obj) {
        string dataValue = JsonMapper.ToJson(obj);
        string path = PathUtil.Combine(folderPath, (fileKey + ".json").TrimStart('/'));
        SaveData(path, dataValue);
    }
    
    public void SaveData(string fileFullPath, string dataValue) {
        #if !UNITY_WEBPLAYER
        
        if (fileFullPath.Contains(Application.dataPath)
            || fileFullPath.Contains(Application.persistentDataPath)) {

            dataValue = DataPrepareSave(dataValue);

            FileSystemUtil.WriteString(fileFullPath, dataValue);
        }
        #else
        
        string fileWebPath = GetWebPath(fileFullPath);
        SystemPrefUtil.SetLocalSettingString(fileWebPath, dataValue);
        #endif
    }
    
    // -----------------------------------------------------------------------
    // HELPERS, REFLECT
    
    public object GetFieldValue(object obj, string fieldName) {
        ////LogUtil.Log("GetFieldValue:obj.GetType():" + obj.GetType());
        
        bool hasGet = false;

        if (obj == null) {
            return obj;
        }
        
        foreach (var prop in fieldName.Split('.').Select(s => obj.GetType().GetField(s))) {
            obj = prop.GetValue(obj);
            hasGet = true;
        }
        
        if (!hasGet) {
            foreach (System.Reflection.PropertyInfo prop in obj.GetType().GetProperties()) {
                if (prop.Name == fieldName) {
                    obj = prop.GetValue(obj, null);
                }
            }
        }
        
        return obj;
    }
    
    public void SetFieldValue(object obj, string fieldName, object fieldValue) {
        ////LogUtil.Log("SetFieldValue:obj.GetType():" + obj.GetType());
        
        //bool hasSet = false;
                
        if (obj == null) {
            return;
        }
        
        foreach (System.Reflection.FieldInfo field in fieldName.Split('.').Select(s => obj.GetType().GetField(s))) {
            if (field != null) {
                field.SetValue(obj, fieldValue);
                
                //hasSet = true;
            }
        }
        
        //if(!hasSet) {
        foreach (System.Reflection.PropertyInfo prop in obj.GetType().GetProperties()) {
            if (prop.Name == fieldName) {
                prop.SetValue(obj, fieldValue, null);
            }
        }
        
        //}
    }
    
    public virtual void Reset() {
        attributes = new Dictionary<string, DataAttribute>();
    }
    
    // -----------------------------------------------------------------------
    // ATTRIBUTES
    
    public void SetAttribute(DataAttribute attribute) {
        
        if (CheckIfAttributeExists(attribute.code)) {
            attributes[attribute.code] = attribute;
        }
        else {
            attributes.Add(attribute.code, attribute);
        }
        Set(BaseDataObjectKeys.attributes, attributes);
    }
    
    public bool CheckIfAttributeExists(string code) {
        if (attributes != null) {
            //code = UniqueUtil.Instance.GetStringHash(code);
            if (attributes.ContainsKey(code)) {
                return true;
            }
        }
        return false;
    }
    
    public DataAttribute GetAttribute(string code) {
        DataAttribute attribute = new DataAttribute();
        
        //code = UniqueUtil.Instance.GetStringHash(code);
        
        if (CheckIfAttributeExists(code)) {
            attribute = attributes[code];
        }
        
        return attribute;
    }
        
    public List<DataAttribute> GetAttributesList() {
        List<DataAttribute> attributesList = new List<DataAttribute>();
        foreach (DataAttribute attribute in attributes.Values) {
            attributesList.Add(attribute);
        }
        return attributesList;
    }
    
    public List<DataAttribute> GetAttributesList(string objectType) {
        List<DataAttribute> attributesFiltered = new List<DataAttribute>();
        foreach (DataAttribute attribute in attributes.Values) {
            if (attribute.otype == objectType) {
                attributesFiltered.Add(attribute);
            }
        }
        return attributesFiltered;
    }
    
    public Dictionary<string, DataAttribute> GetAttributesDictionary() {
        return attributes;   
    }
    
    public Dictionary<string, DataAttribute> GetAttributesDictionary(string objectType) {
        Dictionary<string, DataAttribute> attributesFiltered = new Dictionary<string, DataAttribute>();
        foreach (KeyValuePair<string, DataAttribute> pair in attributes) {
            if (pair.Value.otype == objectType) {
                attributesFiltered.Add(pair.Value.code, pair.Value);
            }
        }
        return attributesFiltered;
    }
    
    public void SetAttributeBoolValue(string code, bool val) {
        DataAttribute att = new DataAttribute();
        att.val = val;
        att.code = code;
        att.name = code;
        att.type = "bool";
        att.otype = "attribute";
        SetAttribute(att);
    }
    
    public bool GetAttributeBoolValue(string code) {
        bool currentValue = false;
        object objectValue = GetAttribute(code).val;
        if (objectValue != null) {
            currentValue = Convert.ToBoolean(objectValue);
        }
        
        return currentValue;
    }
    
    public void SetAttributeStringValue(string code, string val) {
        DataAttribute att = new DataAttribute();
        att.val = val;
        att.code = code;
        att.name = code;
        att.type = "string";
        att.otype = "attribute";
        SetAttribute(att);
    }
    
    public string GetAttributeStringValue(string code) {
        string currentValue = "";
        object objectValue = GetAttribute(code).val;
        if (objectValue != null) {
            currentValue = Convert.ToString(objectValue);
        }
        
        return currentValue;
    }
    
    public void SetAttributeDoubleValue(string code, double val) {
        DataAttribute att = new DataAttribute();
        att.val = val;
        att.code = code;
        att.name = code;
        att.type = "double";
        att.otype = "attribute";
        SetAttribute(att);
    }
    
    public double GetAttributeDoubleValue(string code) {
        double currentValue = 0.0;
        object objectValue = GetAttribute(code).val;
        if (objectValue != null) {
            currentValue = Convert.ToDouble(objectValue);
        }
        
        return currentValue;
    }
    
    public void SetAttributeIntValue(string code, int val) {
        DataAttribute att = new DataAttribute();
        att.val = val;
        att.code = code;
        att.name = code;
        att.type = "int";
        att.otype = "attribute";
        SetAttribute(att);
    }
    
    public int GetAttributeIntValue(string code) {
        int currentValue = 0;
        object objectValue = GetAttribute(code).val;
        if (objectValue != null) {
            currentValue = Convert.ToInt32(objectValue);
        }
        
        return currentValue;
    }
    
    public void SetAttributeObjectValue(string code, object val) {
        DataAttribute att = new DataAttribute();
        att.val = val;
        att.code = code;
        att.name = code;
        att.type = "object";
        att.otype = "attribute";
        SetAttribute(att);
    }

    public T GetAttributeObjectValue<T>(string code) {
        object objectValue = GetAttribute(code).val;
        if (objectValue != null) {
            try {
                return (T)objectValue;
            }
            catch (Exception e) {
                LogUtil.Log(e);
                LogUtil.Log("ERROR:GetAttributeObjectValue:code:" + code);
            }
        }
        
        return default(T);
    }

    public object GetAttributeObjectValue(string code) {
        object objectValue = GetAttribute(code).val;
        return objectValue;
    }
    
    public string GetPlatformAttributeKey(string key) {
        string keyto = key;
        
        #if UNITY_IPHONE
        keyto = "platform-ios-" + keyto;    
        #endif
        #if UNITY_ANDROID
        keyto = "platform-android-" + keyto;
        #endif
        return keyto;
    }
    
}
