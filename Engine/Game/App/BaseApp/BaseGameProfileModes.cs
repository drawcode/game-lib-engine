using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Engine.Data.Json;
using Engine.Events;
using Engine.Utility;

public class BaseGameProfileModeAttributes {
        
    // MODES    
    public static string ATT_CURRENT_CAMERA_MODE = "camera-mode";
}

public class GameProfileContentCollectItem : GameDataObject {

    // action code
    // level code
    // mission code
    // world code

    //public List<AppContentCollectItem> data;
    
    public GameProfileContentCollectItem() {
        Reset();
    }
    
    public void Reset() {
        //data = new List<AppContentCollectItem>();
    } 

}

public class GameProfileContentCollectItems {
    
    public List<GameProfileContentCollectItem> items;
    
    public GameProfileContentCollectItems() {
        Reset();
    }
    
    public void Reset() {
        items = new List<GameProfileContentCollectItem>();
    }

    // COLLECT ITEM KEYS
            
    public GameProfileContentCollectItem GetItemByTypeAndKey(
        string type, string key) {
        
        if (items == null) {
            return null;
        }
        
        foreach (GameProfileContentCollectItem item in items) {
            if (item.key.ToLower() == key.ToLower()
                && item.type.ToLower() == type.ToLower()) {
                return item;
            }
        }
        return null;
    }
        
    public void SetItemByTypeAndKey(GameProfileContentCollectItem item) {

        bool found = false;
        
        for (int i = 0; i < items.Count; i++) {
            if (items[i].key.ToLower() == item.key.ToLower()
                && items[i].type.ToLower() == item.type.ToLower()) {
                items[i] = item;
                found = true;
                break;
            }
        }
        
        if (!found) {
            items.Add(item);
        }
    }    
}

public class BaseGameProfileModes {
    private static volatile BaseGameProfileMode current;
    private static volatile BaseGameProfileModes instance;
    private static object syncRoot = new Object();
    public static string DEFAULT_USERNAME = "Player";
    
    public static BaseGameProfileMode BaseCurrent {
        get {
            if (current == null) {
                lock (syncRoot) {
                    if (current == null) 
                        current = new BaseGameProfileMode();
                }
            }
    
            return current;
        }
        set {
            current = value;
        }
    }

    
    //string appContentState = AppContentStates.Current.code;
    //string appState = AppStates.Current.code;
        
    public static BaseGameProfileModes BaseInstance {
        get {
            if (instance == null) {
                lock (syncRoot) {
                    if (instance == null) 
                        instance = new BaseGameProfileModes();
                }
            }
    
            return instance;
        }
    }

    public static string GetAppContentCollectItemKey(
        string appState,
        string appContentState,
        string worldCode, 
        string levelCode, 
        string appContentCollectCode, 
        string actionCode) {

        StringBuilder sb = new StringBuilder();

        sb.Append(":");
        sb.Append(BaseDataObjectKeys.app_state);
        sb.Append(":");
        sb.Append(appState);
        
        sb.Append(":");
        sb.Append(BaseDataObjectKeys.app_content_state);
        sb.Append(":");
        sb.Append(appContentState);
        
        sb.Append(":");
        sb.Append(BaseDataObjectKeys.world_code);
        sb.Append(":");
        sb.Append(worldCode);
        
        sb.Append(":");
        sb.Append(BaseDataObjectKeys.level_code);
        sb.Append(":");
        sb.Append(levelCode);
        
        sb.Append(":");
        sb.Append(BaseDataObjectKeys.app_content_collect_code);
        sb.Append(":");
        sb.Append(appContentCollectCode);
        
        sb.Append(":");
        sb.Append(BaseDataObjectKeys.action_code);
        sb.Append(":");
        sb.Append(actionCode);

        return sb.ToString();
    }

    public static string GetAppContentCollectItemKey(
        string missionCode) {
        
        string appState = AppStates.Current.code;
        string appContentState = AppContentStates.Current.code;
        string worldCode = GameWorlds.Current.code;
        string levelCode = "all";
                        
        return 
            GetAppContentCollectItemKey(
                appState, appContentState, worldCode, 
                levelCode, missionCode, "default");
    }

    public static string GetAppContentCollectItemKey(
        string missionCode, string actionUid) {
        
        string appState = AppStates.Current.code;
        string appContentState = AppContentStates.Current.code;
        string worldCode = GameWorlds.Current.code;
        string levelCode = "all";
        
        return 
            GetAppContentCollectItemKey(
                appState, appContentState, worldCode, 
                levelCode, missionCode, actionUid);
    }
        
    // TODO: Common profile actions, lookup, count, etc
}

public class BaseGameProfileMode : Profile {
    // BE CAREFUL adding properties as they will cause a need for a profile conversion
    // Best way to add items to the profile is the GetAttribute and SetAttribute class as 
    // that stores as a generic DataAttribute class.  Booleans, strings, objects, serialized json objects etc
    // all work well and cause no need to convert profile on updates. 
        
    public BaseGameProfileMode() {
        Reset();
    }
    
    public override void Reset() {
        base.Reset();
        username = ProfileConfigs.defaultPlayerName;
    }
        
    // customizations       
    
    public virtual void SetValue(string code, object value) {
        DataAttribute att = new DataAttribute();
        att.val = value;
        att.code = code;
        att.name = "";
        att.type = "bool";
        att.otype = "mode";
        SetAttribute(att);
    }
        
    public virtual List<DataAttribute> GetList() {
        return GetAttributesList("mode");
    }   
        
    // -----------------------------------------------------------------
    // GAME MODES DATA
    
    // -----------------------------------------------------------------
    // COLLECTIONS - ACTIONS


    // -----------------------------------------------------------------
    // COLLECTIONS MISSIONS 

    // Missions are
    
    public virtual GameProfileContentCollectItems app_content_collect_items {
        get {
            return Get<GameProfileContentCollectItems>(BaseDataObjectKeys.app_content_collect_items);
        }
        
        set {
            Set(BaseDataObjectKeys.app_content_collect_items, value);
        }
    }
    
    public virtual void SetContentCollectItems(GameProfileContentCollectItems obj) {
        
        app_content_collect_items = obj;
        
        Messenger.Broadcast(BaseGameProfileMessages.ProfileShouldBeSaved);
    }

    public virtual GameProfileContentCollectItems GetContentCollectItems() {       
        GameProfileContentCollectItems obj = new GameProfileContentCollectItems();
        
        obj = app_content_collect_items;
        
        if (obj == null) {
            obj = new GameProfileContentCollectItems();
        }
        
        return obj;
    }
    
    public T GetContentCollectValue<T>(string type, string typeKey, string key) {
        
        GameProfileContentCollectItem item = GetContentCollectItem(type, typeKey);
        
        if (item == null) {
            return default(T);
        }
        
        return item.Get<T>(key);
    }
    
    public string GetContentCollectValue(string type, string typeKey, string key) {
        
        return GetContentCollectValue<string>(type, typeKey, key);
    }

    /*
    public void SetContentCollectItems(string type, string typeKey, List<AppContentCollectItem> items) {
        
        GameProfileContentCollectItem item = GetContentCollectItem(type, typeKey);
        
        if (item == null) {
            item = new GameProfileContentCollectItem();
        }

        item.data = items;
        
        SetContentCollectItem(item);
    }
    */
    
    public void SetContentCollectValue(string type, string typeKey, string key, object val) {
        
        GameProfileContentCollectItem item = GetContentCollectItem(type, typeKey);
        
        if (item == null) {
            item = new GameProfileContentCollectItem();
        }
        
        item.type = type;
        item.key = typeKey;

        item.Set(key, val);
        
        SetContentCollectItem(item);
    }
        
    public GameProfileContentCollectItem GetContentCollectItem(string type, string typeKey) {

        GameProfileContentCollectItems objs = GetContentCollectItems();

        return objs.GetItemByTypeAndKey(type, typeKey);
    }
    
    public void SetContentCollectItem(GameProfileContentCollectItem item) {
        SetContentCollectItem(item, true);
    }
    
    public void SetContentCollectItem(GameProfileContentCollectItem item, bool setAsCurrent) {
        
        GameProfileContentCollectItems items = GetContentCollectItems();
        
        if (setAsCurrent) {
            //BaseGameProfileCharacters.currentCharacter = item;
            //GameProfileCharacters.Current.SetCurrentCharacterProfileCode(code);
        }
        
        items.SetItemByTypeAndKey(item);

        SetContentCollectItems(items);
    }

    
}


