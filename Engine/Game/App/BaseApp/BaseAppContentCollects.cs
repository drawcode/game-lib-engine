using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class BaseAppContentCollects<T> : DataObjects<T> where T : DataObject, new() {
    private static T current;
    private static volatile BaseAppContentCollects<T> instance;
    private static object syncRoot = new Object();

    private string BASE_DATA_KEY = "app-content-collection-data";

    public static T BaseCurrent {
        get {
            if (current == null) {
                lock (syncRoot) {
                    if (current == null)
                        current = new T();
                }
            }

            return current;
        }
        set {
            current = value;
        }
    }

    public static BaseAppContentCollects<T> BaseInstance {
        get {
            if (instance == null) {
                lock (syncRoot) {
                    if (instance == null)
                        instance = new BaseAppContentCollects<T>(true);
                }
            }

            return instance;
        }
        set {
            instance = value;
        }
    }


    public BaseAppContentCollects() {
        Reset();
    }

    public BaseAppContentCollects(bool loadData) {
        Reset();
        path = "data/" + BASE_DATA_KEY + ".json";
        pathKey = BASE_DATA_KEY;
        LoadData();
    }

    public void ChangeState(string code) {
        if(AppContentCollects.Current.code != code) {
            AppContentCollects.Current = AppContentCollects.Instance.GetById(code);
        }
    }

    public List<AppContentCollect> GetListByCodeAndPackCode(string assetCode, string packCode) {
        List<AppContentCollect> filteredList = new List<AppContentCollect>();
        foreach(AppContentCollect obj in AppContentCollects.Instance.GetListByPack(packCode)) {
            if(assetCode.ToLower() == obj.code.ToLower()) {
                filteredList.Add(obj);
            }
        }

        return filteredList;
    }

    public string GetAppContentCollectContentPath(string packCode, string asset, bool versioned) {
        string packPath = PathUtil.Combine(
            ContentPaths.appCachePathSharedPacks,
            packCode);

        string packPathContent = PathUtil.Combine(
            packPath,
            ContentConfig.currentContentContent);
    
        string file = "";
        AppContentCollect appContentAsset = AppContentCollects.Instance.GetById(asset);
        if(appContentAsset != null) {
            file = appContentAsset.code
            + "."
            + appContentAsset.GetVersionFileExt();
        }
    
        string fullPath = PathUtil.Combine(packPathContent, file);
        if(versioned) {
            fullPath = Contents.GetFullPathVersioned(fullPath);
        }
        return fullPath;
    }

    public bool CheckChoiceUser(string code, List<AppContentCollectItem> choices) {

        if(choices == null) {
            return false;
        }

        if(choices.Count > 0) {
            return false;
        }

        foreach(AppContentCollectItem item in choices) {
            if(item.code.ToLower() == code.ToLower()) {
                return true;
            }
        }
        return false;
    }

    public bool CheckChoices(string choiceCode, List<AppContentCollectItem> choices, bool correctOnly) {

        if(choices == null) {
            return false;
        }

        if(choices.Count > 0) {
            return false;
        }

        AppContentCollect choice = AppContentCollects.Instance.GetByCode(choiceCode);
        if(choice != null) {
            foreach(AppContentCollectItem choiceItem in choice.choices) {

                if(correctOnly) {
                    if(choiceItem.type != AppContentCollectType.correct) {
                        return false;
                    }
                }

                if(!CheckChoiceUser(choiceItem.code, choices)) {
                    return false;
                }
            }
        }
        else {
            return false;
        }

        if(choices.Count == choice.choices.Count) {
            return true;
        }

        return false;
    }
}


public class AppContentCollectAttributes {
    public static string version_file_increment = "version_file_increment";
    public static string version = "version";
    public static string version_required_app = "version_required_app";
    public static string version_type = "version_type";
    public static string version_file_ext = "version_file_ext";
    public static string version_file_type = "version_file_type";
    /*
    *
         "version_file_increment":"1",
         "version":"1.0",
         "version_required_app":"1.0",
         "version_type":"itemized"
         */
}

public class AppContentCollectAttributesFileType {
    public static string videoType = "video";
    public static string audioType = "audio";
    public static string imageType = "image";
    public static string assetBundleType = "assetBundle";
}

public class AppContentCollectAttributesFileExt {
    public static string videoM4vExt = "m4v";
    public static string videoMp4Ext = "mp4";

    public static string audioMp3Ext = "mp3";
    public static string audioWavExt = "wav";

    public static string imagePngExt = "png";
    public static string imageJpgExt = "jpg";

    public static string assetBundleExt = "unity3d";
}

/*
    {
     "sort_order":0,
     "sort_order_type":0,
     "attributes":null,
     "game_id":"63a15c20-294f-11e1-9314-0800200c9a66",
     "type":"resource",
     "key":"question",
     "order_by":"",
     "code":"question-concussion-1",
     "display_name":"Concussions only happen if a player is knocked out.",
     "name":"",
     "description":"Concussions can happen whether a player is knocked out or not. It is important to pay attention to any symptoms.",
     "status":"default",
     "uuid":"33f4b660-2955-11e1-9314-0800200c9a61",
     "active":true,
     "keys":["concussion"],
     "responses": [
         {"name": "true", "display":"True", type":"incorrect"},
         {"name": "false", "display":"False", "type":"correct"}
     ]
 }
 */

public class AppContentCollectMessage {
    public string choiceItemType = AppContentCollectType.incorrect;
    public string choiceItemCode = "";
    public string choiceCode = "";
}

public class AppContentCollectMessages {
    public static string appContentCollectItem = "app-content-choice-item";
}

public class AppContentCollectType {
    public static string incorrect = "incorrect";
    public static string correct = "correct";
    public static string warp = "warp";
    public static string portal = "portal";
    public static string boost = "boost";
    public static string door = "door";
    public static string level = "level";
    public static string custom = "custom";
}


public class AppContentCollectItemKeys {
    public static string code = "code";
    public static string display = "display";
    public static string type = "type";
}

public class AppContentCollectItem : DataObject {


    public virtual string code {
        get {
            return Get<string>(AppContentCollectItemKeys.code, "");
        }
        
        set {
            Set(AppContentCollectItemKeys.code, value);
        }
    }

    public virtual string display {
        get {
            return Get<string>(AppContentCollectItemKeys.display, "");
        }
        
        set {
            Set(AppContentCollectItemKeys.display, value);
        }
    }

    public virtual string type {
        get {
            return Get<string>(AppContentCollectItemKeys.type, "");
        }
        
        set {
            Set(AppContentCollectItemKeys.type, value);
        }
    }

    public AppContentCollectItem() {
        Reset();
    }

    public override void Reset() {
        code = "";
        display = "";
        type = AppContentCollectType.incorrect;
    }

    public bool IsTypeCorrect() {
        return type == AppContentCollectType.correct ? true : false;
    }

    public bool IsTypeIncorrect() {
        return type == AppContentCollectType.incorrect ? true : false;
    }

    public bool IsTypeBoost() {
        return type == AppContentCollectType.boost ? true : false;
    }

    public bool IsTypeDoor() {
        return type == AppContentCollectType.door ? true : false;
    }

    public bool IsTypeLevel() {
        return type == AppContentCollectType.level ? true : false;
    }

    public bool IsTypeCustom() {
        return type == AppContentCollectType.custom ? true : false;
    }

    public bool IsTypePortal() {
        return type == AppContentCollectType.portal ? true : false;
    }

    public bool IsTypeWarp() {
        return type == AppContentCollectType.warp ? true : false;
    }
}

public class AppContentCollectsDataKeys {
    public static string choicesCorrect = "choicesCorrect";
    public static string choices = "choices";
}

public class AppContentCollectsData : DataObject {
    
    public virtual double choicesCorrect {
        get {
            return Get<double>(AppContentCollectsDataKeys.choicesCorrect, 0);
        }
        
        set {
            Set(AppContentCollectsDataKeys.choicesCorrect, value);
        }
    }

    public virtual Dictionary<string, AppContentCollectData> choices {
        get {
            return Get<Dictionary<string, AppContentCollectData>>(
                AppContentCollectsDataKeys.choices);
        }
        
        set {
            Set(AppContentCollectsDataKeys.choices, value);
        }
    }

    public AppContentCollectsData() {
        Reset();
    }

    public int GetChoicesCount() {
        CheckDicts();
        return choices.Count;
    }

    public int GetChoicesCorrect() {
        CheckDicts();

        int correct = 0;

        foreach(KeyValuePair<string, AppContentCollectData> choiceData in choices) {
            if(choiceData.Value.CheckChoices(true)) {
                correct += 1;
            }
        }

        return correct;
    }

    public string GetChoiceScorePercentage() {
        return GetChoicesScore().ToString("P0");
    }

    public float GetChoicesScore() {
        return GetChoicesCorrect() / GetChoicesCount();
    }

    public override void Reset() {
        choices = new Dictionary<string, AppContentCollectData>();
    }

    public void CheckDicts() {
        if(choices == null) {
            choices = new Dictionary<string, AppContentCollectData>();
        }
    }

    public Dictionary<string, AppContentCollectData> GetChoices() {
        CheckDicts();

        return choices;
    }

    public AppContentCollectData GetChoice(string code) {
        CheckDicts();

        if(choices.ContainsKey(code)) {
            return choices[code];
        }

        return null;
    }

    public void SetChoice(AppContentCollectData choiceData) {
        if(choiceData == null) {
            return;
        }

        CheckDicts();

        if(choices.ContainsKey(choiceData.choiceCode)) {
            choices[choiceData.choiceCode] = choiceData;
        }
        else {
            choices.Add(choiceData.choiceCode, choiceData);
        }
    }

    public void UpdateValues() {
        //foreach(KeyValuePair<string, AppContentCollectData> pair in choices) {

        //}
    }
}

public class AppContentCollectDataKeys {
    public static string choiceCode = "choiceCode";
    public static string choiceData = "choiceData";
    public static string choices = "choices";
}

public class AppContentCollectData : DataObject {
        
    public virtual string choiceCode {
        get {
            return Get<string>(AppContentCollectDataKeys.choiceCode, "");
        }
        
        set {
            Set(AppContentCollectDataKeys.choiceCode, value);
        }
    }
    
    public virtual string choiceData {
        get {
            return Get<string>(AppContentCollectDataKeys.choiceData, "");
        }
        
        set {
            Set(AppContentCollectDataKeys.choiceData, value);
        }
    }
    
    public virtual List<AppContentCollectItem> choices {
        get {
            return Get<List<AppContentCollectItem>>(
                AppContentCollectDataKeys.choices);
        }
        
        set {
            Set(AppContentCollectDataKeys.choices, value);
        }
    }

    public AppContentCollectData() {
        Reset();
    }

    public override void Reset() {
        choiceCode = "";
        choiceData = "";
        choices = new List<AppContentCollectItem>();
    }

    public bool CheckChoices(bool correctOnly) {
        return AppContentCollects.Instance.CheckChoices(choiceCode, choices, correctOnly);
    }

    public AppContentCollectItem GetChoice(string choiceCode) {
        foreach(AppContentCollectItem item in choices) {
            if(choiceCode == item.code) {
                return item;
            }
        }
        return null;
    }

    public bool HasChoice(AppContentCollectItem choiceItem) {
        foreach(AppContentCollectItem item in choices) {
            if(item.code == choiceItem.code
                && item.type == choiceItem.type) {
                return true;
            }
        }
        return false;
    }

    public void SetChoice(AppContentCollectItem choiceItem) {
        if(HasChoice(choiceItem)) {
            for(int i = 0; i < choices.Count; i++) {
                choices[i] = choiceItem;
            }
        }
        else {
            choices.Add(choiceItem);
        }
    }
}

public class BaseAppContentCollectKeys {
    public static string tags = "tags";
    public static string appStates = "appStates";
    public static string appContentStates = "appContentStates";
    public static string requiredAssets = "requiredAssets";
    public static string keys = "keys";
    public static string contentAttributes = "contentAttributes";
    public static string choices = "choices";
}

public class BaseAppContentCollect : GameDataObject {    
    
    public virtual List<string> tags {
        get {
            return Get<List<string>>(BaseAppContentCollectKeys.tags);
        }
        
        set {
            Set(BaseAppContentCollectKeys.tags, value);
        }
    }
    
    public virtual List<string> appStates {
        get {
            return Get<List<string>>(BaseAppContentCollectKeys.appStates);
        }
        
        set {
            Set(BaseAppContentCollectKeys.appStates, value);
        }
    }
    
    public virtual List<string> appContentStates {
        get {
            return Get<List<string>>(BaseAppContentCollectKeys.appContentStates);
        }
        
        set {
            Set(BaseAppContentCollectKeys.appContentStates, value);
        }
    }

    public virtual Dictionary<string, List<string>> requiredAssets {
        get {
            return Get<Dictionary<string, List<string>>>(BaseAppContentCollectKeys.requiredAssets);
        }
        
        set {
            Set(BaseAppContentCollectKeys.requiredAssets, value);
        }
    }

    public virtual List<string> keys {
        get {
            return Get<List<string>>(BaseAppContentCollectKeys.keys);
        }
        
        set {
            Set(BaseAppContentCollectKeys.keys, value);
        }
    }

    public virtual Dictionary<string, string> contentAttributes {
        get {
            return Get<Dictionary<string, string>>(BaseAppContentCollectKeys.contentAttributes);
        }
        
        set {
            Set(BaseAppContentCollectKeys.contentAttributes, value);
        }
    }

    public virtual List<AppContentCollectItem> choices {
        get {
            return Get<List<AppContentCollectItem>>(BaseAppContentCollectKeys.choices);
        }
        
        set {
            Set(BaseAppContentCollectKeys.choices, value);
        }
    }

    // types: tracker, pack, data, generic

    // Attributes that are added or changed after launch should be like this to prevent
    // profile conversions.

    public BaseAppContentCollect() {
        Reset();
    }

    public override void Reset() {
        base.Reset();
        appStates = new List<string>();
        appContentStates = new List<string>();
        requiredAssets = new Dictionary<string, List<string>>();
        keys = new List<string>();
        contentAttributes = new Dictionary<string, string>();
        choices = new List<AppContentCollectItem>();
    }

    public bool HasTypeCorrect() {
        foreach(AppContentCollectItem item in choices) {
            if(item.type.ToLower() == AppContentCollectType.correct) {
                return true;
            }
        }
        return false;
    }

    public bool HasTypeCorrectMultiple() {
        int answers = 0;
        foreach(AppContentCollectItem item in choices) {
            if(item.type.ToLower() == AppContentCollectType.correct) {
                answers += 1;
            }
        }
        return answers > 0 ? true : false;
    }
 
    public string GetContentString(string key) {
        string content = "";
        if(contentAttributes.ContainsKey(key)) {
            content = contentAttributes[key];
        }
        return content;
    }
    
    public string GetVersion() {
        return GetContentString(AppContentCollectAttributes.version);
    }
    
    public string GetVersionFileIncrement() {
        return GetContentString(AppContentCollectAttributes.version_file_increment);
    }
    
    public string GetVersionRequiredApp() {
        return GetContentString(AppContentCollectAttributes.version_required_app);
    }
    
    public string GetVersionType() {
        return GetContentString(AppContentCollectAttributes.version_type);
    }   
    
    public string GetVersionFileType() {
        return GetContentString(AppContentCollectAttributes.version_file_type);
    }
    
    public string GetVersionFileExt() {
        return GetContentString(AppContentCollectAttributes.version_file_ext);
    }
}