using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

using Engine.Events;

public class BaseAppContentCollects<T> : DataObjects<T> where T : GameDataObject, new() {
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

    public static void ChangeCurrent(string code) {
        if (AppContentCollects.Current.code != code) {
            AppContentCollects.Current = AppContentCollects.Instance.GetById(code);
        }
    }
    
    // -------------------------------------------------------
    // UPDATE DISPLAYS
    
    public static List<AppContentCollect> UpdateCollectItemData(List<AppContentCollect> list) {
        return AppContentCollects.Instance.updateCollectItemData(list);
    }

    public List<AppContentCollect> updateCollectItemData(List<AppContentCollect> list) {
    
        foreach (AppContentCollect obj in list) {
            foreach (AppContentCollectItem item in obj.data) {
                if (item.data != null) {                    
                    if (item.IsType(AppContentCollectType.action)) {
                        item.UpdateDisplayValues();
                    }
                }
            }
        }

        return list;
    }
    
    // TYPE
    
    public static List<AppContentCollect> GetByType(string type) {
        return AppContentCollects.Instance.getByType(type);
    }
    
    public List<T> getByType(string type) {

        List<T> objs = GetAll().FindAll(
            u => 
            u.type == type);

        // Handle


        return objs;
    }
    
    // CODE
    
    public static AppContentCollect GetByTypeAndCode(string type, string code) {
        return AppContentCollects.Instance.getByTypeAndCode(type, code);
    }
    
    public T getByTypeAndCode(string type, string code) {
        return GetAll().Find(
            u => 
            u.type == type 
            && u.code == code);
    }

    // WORLD
    
    public static List<AppContentCollect> GetByTypeAndWorld(string type, string code) {
        return AppContentCollects.Instance.getByTypeAndWorld(type, code);
    }
    
    public List<T> getByTypeAndWorld(string type, string code) {
        return GetAll().FindAll(
            u => 
            u.type == type 
            && (u.world_code == code || u.HasTag("all")));
    }
    
    // ---------------------------------------------------------
    // MISSIONS

    public static List<AppContentCollect> GetMissions() {
        return AppContentCollects.Instance.getMissions();
    }
    
    public List<AppContentCollect> getMissions() {
        return GetByType(AppContentCollectType.mission);
    }

    // MISSION SETS CODE
    
    public static AppContentCollect GetMission(string code) {
        return AppContentCollects.Instance.getMission(code);
    }
    
    public AppContentCollect getMission(string code) {
        return GetByTypeAndCode(AppContentCollectType.mission, code);
    }

    // MISSION SET BY WORLD
    
    public static List<AppContentCollect> GetMissionsByWorld(string code) {
        return AppContentCollects.Instance.getMissionsByWorld(code);
    }
    
    public List<AppContentCollect> getMissionsByWorld(string code) {
        return GetByTypeAndWorld(AppContentCollectType.mission, code);
    }

    // MISSION SET DEFAULT AND BY WORLD
    
    //public static List<AppContentCollect> GetMissionsByWorld(string code) {
    //    return AppContentCollects.Instance.getMissionsByWorld(code);
    //}
    
    //public List<AppContentCollect> getMissionsByWorld(string code) {
    //   return GetByTypeAndWorld(AppContentCollectType.mission, code);
    //}

    // ---------------------------------------------------------
    // ACTIONS
    
    public static List<AppContentCollect> GetActions() {
        return AppContentCollects.Instance.getActions();
    }

    public List<AppContentCollect> getActions() {
        return GetByType(AppContentCollectType.action);
    }
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

         {
            "code":"action-rescue", 
            "val": 3, 
            "type": "action"
         },         
         {
            "code":"action-win", 
             "val": 1, 
             "type": "action"
         },         
         {
            "code":"action-collect", 
            "val": 3, 
            "type": "action", 
            "data_code": "item-coin", 
            "data_type": "item"
         }


 */

public class AppContentCollectMessage {
    public string choiceItemType = AppContentCollectType.mission;
    public string choiceItemCode = "";
    public string choiceCode = "";
}

public class AppContentCollectMessages {
    public static string appContentCollectItem = "app-content-collect-item";
}

public class AppContentCollectType {
    public static string mission = "mission";
    public static string action = "action";
}

public class AppContentCollectActionType {
    public static string actionRescue = "action-rescue";
    public static string actionWin = "action-win";
    public static string actionCollect = "action-collect";
    public static string actionDefend = "action-defend";
    public static string actionAttack = "action-attack";
    public static string actionKill = "action-kill";    
}

public class AppContentCollectActionDataType {
    public static string itemType = "item";
    public static string statisticType = "statistic";
    public static string achievementType = "achievement";
    public static string characterType = "character";
}

public class AppContentCollectActionDataCode {
    public static string itemCoin = "item-coin";
}

public class AppContentCollectItemKeys {
    public static string code = "code";
    public static string display = "display";
    public static string type = "type";
}

public class AppContentCollectItemMission : GameDataObject {

    public AppContentCollectItemMission() {
       
    }
}

public class AppContentCollectItemAction : GameDataObject {
    
    public AppContentCollectItemAction() {
        
    }
}

public class AppContentCollectItem : GameDataObject {

    // code 
    // type 

    public virtual GameDataObject data {
        get {
            return Get<GameDataObject>(BaseDataObjectKeys.data, new GameDataObject());
        }
        
        set {
            Set<DataObject>(BaseDataObjectKeys.data, value);
        }
    }

    public AppContentCollectItem() {
        Reset();
    }

    public override void Reset() {
        code = "";
        type = AppContentCollectType.mission;
    }

    public bool IsTypeAction() {
        return IsType(AppContentCollectType.action);
    }

    public bool IsTypeMission() {
        return IsType(AppContentCollectType.mission);
    }

    public void UpdateDisplayValues() {
        // get the action and object
        // format using collect item meta

        AppContentCollect collectItem = 
                AppContentCollects.GetByTypeAndCode(type, code);

        if (collectItem != null) {

            string itemDisplayName = collectItem.display_name;
            string itemDescription = collectItem.description;

            string dataType = data.type;
            string dataCode = data.code;

            if(dataType == AppContentCollectActionDataType.itemType) {
            
                GameItem obj = GameItems.Instance.GetById(dataCode);

                if(obj != null) {
                    data.action_display_name = obj.display_name;
                    data.action_description = obj.description;
                }
            }
            else if(dataType == AppContentCollectActionDataType.statisticType) {
                
                GameStatistic obj = GameStatistics.Instance.GetById(dataCode);
                
                if(obj != null) {
                    data.action_display_name = obj.display_name;
                    data.action_description = obj.description;
                }
            }
            else if(dataType == AppContentCollectActionDataType.characterType) {
                
                GameCharacter obj = GameCharacters.Instance.GetById(dataCode);
                
                if(obj != null) {
                    data.action_display_name = obj.display_name;
                    data.action_description = obj.description;
                }
            }

            data.display_name = ReplaceTemplated(itemDisplayName);
            data.description = ReplaceTemplated(itemDescription);
                
        }
    }
    
    public string ReplaceTemplated(string content) {

        // If string contains mustache/handlebars {{ [code] }} then get all 
        // matches and replace with localized content

        string regexTemplate = @"\{\{[ ]*(.*?)[ ]*\}\}";
        
        if (content.RegexIsMatch(regexTemplate)) {
            
            MatchCollection matches = content.RegexMatches(regexTemplate);
            
            foreach (Match match in matches) {
                
                //string valCodeMatch = match.Value;
                string valCodeGroup = match.Value;

                foreach (Group group in match.Groups) {
                    valCodeGroup = group.Value;
                }
                                
                if (data != null) {

                    string regexCode = @"(\{\{[ ]*" + valCodeGroup + @"[ ]*\}\})"; 
                    object replaceObj = data.Get<object>(valCodeGroup);

                    if(replaceObj != null) {
                        string replaceText = replaceObj.ToString();
                        content = content.RegexMatchesReplace(regexCode, replaceText);
                    }
                }
            }
        }
        
        return content;        
    }


    //public AppContentCollectItemMission GetMission() {
    //    return (AppContentCollectItemMission)data;
    //}

    //public AppContentCollectItemAction GetAction() {
    //    return (AppContentCollectItemAction)data;
    //}
}

public class BaseAppContentCollect : GameDataObject {    

    public virtual List<AppContentCollectItem> data {
        get {
            return Get<List<AppContentCollectItem>>(BaseDataObjectKeys.data);
        }
        
        set {
            Set(BaseDataObjectKeys.data, value);
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
        data = new List<AppContentCollectItem>();
    }

    public bool HasTypeMission() {
        foreach (AppContentCollectItem item in data) {
            if (item.type.ToLower() == AppContentCollectType.mission) {
                return true;
            }
        }
        return false;
    }
        
    public bool HasTypeAction() {
        foreach (AppContentCollectItem item in data) {
            if (item.type.ToLower() == AppContentCollectType.action) {
                return true;
            }
        }
        return false;
    }

    public string GetContentString(string key) {
        string content = "";
        if (content_attributes.ContainsKey(key)) {
            content = content_attributes[key].ToString();
        }
        return content;
    }

    public void UpdateCollectItemData() {
        
            foreach (AppContentCollectItem item in data) {
                if (item.data != null) {                    
                    if (item.IsType(AppContentCollectType.action)) {
                        item.UpdateDisplayValues();
                    }
                }
            }
    }

    public List<AppContentCollectItem> GetItemsData() {
        UpdateCollectItemData();
        return data;
    }

    
    public string GetVersion() {
        return GetContentString(AppContentAssetAttributes.version);
    }
    
    public string GetVersionFileIncrement() {
        return GetContentString(AppContentAssetAttributes.version_file_increment);
    }
    
    public string GetVersionRequiredApp() {
        return GetContentString(AppContentAssetAttributes.version_required_app);
    }
    
    public string GetVersionType() {
        return GetContentString(AppContentAssetAttributes.version_type);
    }
    
    public string GetVersionFileType() {
        return GetContentString(AppContentAssetAttributes.version_file_type);
    }
    
    public string GetVersionFileExt() {
        return GetContentString(AppContentAssetAttributes.version_file_ext);
    }
}