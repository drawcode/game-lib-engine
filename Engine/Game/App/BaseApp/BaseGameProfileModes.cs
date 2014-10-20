using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using Engine.Data.Json;
using Engine.Events;
using Engine.Utility;

public class BaseGameProfileModeAttributes {
		
	// MODES	
	
	public static string ATT_CURRENT_RACE_MODE = "race-mode";
	public static string ATT_CURRENT_CAMERA_MODE = "camera-mode";
	public static string ATT_CURRENT_RACE_MODE_ARCADE_LEVEL = "race-mode-arcade-level";	
	public static string ATT_CURRENT_RACE_MODE_ARCADE_DIFFICULTY_VALUE = "race-mode-arcade-difficulty-value";
	
	public static string ATT_CURRENT_RACE_MODE_ENDLESS_LEVEL = "race-mode-endless-level";
	public static string ATT_CURRENT_RACE_MODE_ENDLESS_ITEM = "race-mode-endless-item";
	public static string ATT_CURRENT_RACE_MODE_ENDLESS_PACK = "race-mode-endless-pack";
	public static string ATT_CURRENT_RACE_MODE_ENDLESS_RESULT_SET = "race-mode-endless-result-set";
	public static string ATT_CURRENT_RACE_MODE_ENDLESS_RESULT = "race-mode-endless-result";
	
	public static string ATT_CURRENT_RACE_MODE_SERIES_PACK = "race-mode-series-pack";
	public static string ATT_CURRENT_RACE_MODE_SERIES_LEVEL = "race-mode-series-level";
	public static string ATT_CURRENT_RACE_MODE_SERIES_INDEX = "race-mode-series-index";		
	public static string ATT_CURRENT_RACE_MODE_SERIES_DIFFICULTY_TYPE = "race-mode-series-difficulty-type";
	public static string ATT_CURRENT_RACE_MODE_SERIES_CONTAINER_INDEX = "race-mode-series-container-index";
	public static string ATT_CURRENT_RACE_MODE_SERIES_LEVEL_INDEX = "race-mode-series-level-index";
	public static string ATT_CURRENT_RACE_MODE_SERIES_RESULT_SET = "race-mode-endless-result-set";
}


public class GameProfileMissionItem : GameDataObject {
    
}

public class GameProfileMissionItems {
    
    public List<GameProfileMissionItem> items;
    
    public GameProfileMissionItems() {
        Reset();
    }
    
    public void Reset() {
        items = new List<GameProfileMissionItem>();
    }
    
    public GameProfileMissionItem GetItem(string missionCode) {
        //if(items == null) {
        //     items = new List<GameProfileCharacterItem>();
        //
        //    GameProfileCharacterItem item = new GameProfileCharacterItem();
        //    items.Add(item);
        //}
        
        if (items == null) {
            return null;
        }
        
        foreach (GameProfileMissionItem item in items) {
            if (item.mission_code.ToLower() == missionCode.ToLower()) {
                return item;
            }
        }
        return null;
    }
    
    public void SetItem(string missionCode, GameProfileMissionItem item) {
        bool found = false;
        
        for (int i = 0; i < items.Count; i++) {
            if (items[i].mission_code.ToLower() == missionCode.ToLower()) {
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
	
	
	// TODO: Common profile actions, lookup, count, etc
}

public class BaseGameProfileMode : Profile  {
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
	
	// GAME MODES DATA


    
    // SOCIAL
    
    // auth/social
    
    
    public virtual GameProfileMissionItems mission_items {
        get {
            return Get<GameProfileMissionItems>(BaseDataObjectKeys.mission_items);
        }
        
        set {
            Set(BaseDataObjectKeys.mission_items, value);
        }
    }
    
    public virtual void SetMissions(GameProfileMissionItems obj) {
        
        mission_items = obj;
        
        Messenger.Broadcast(BaseGameProfileMessages.ProfileShouldBeSaved);
    }
    
    public virtual GameProfileMissionItems GetMissions() {       
        GameProfileMissionItems obj = new GameProfileMissionItems();
        
        obj = mission_items;
        
        if (obj == null) {
            obj = new GameProfileMissionItems();
        }
        
        return obj;
    }
    
    public T GetMissionValue<T>(string missionCode, string key) {
        
        GameProfileMissionItem item = GetMission(missionCode);
        
        if (item == null) {
            return default(T);
        }
        
        return item.Get<T>(key);
    }
    
    public string GetMissionValue(string missionCode, string key) {
        
        return GetMissionValue<string>(missionCode, key);
    }
    
    public void GetMissionValue(string missionCode, string key, object val) {
        
        GameProfileMissionItem item = GetMission(missionCode);
        
        if (item == null) {
            item = new GameProfileMissionItem();
            item.mission_code = missionCode;
        }
        
        item.Set(key, val);
        
        SetMission(missionCode, item);
    }
        
    public GameProfileMissionItem GetMission(string missionCode) {
        
        GameProfileMissionItem item = GetMissions().GetItem(missionCode);
        
        return item;
    }
    
    public void SetMission(string missionCode, GameProfileMissionItem item) {
        SetMission(missionCode, item, true);
    }
    
    public void SetMission(string missionCode, GameProfileMissionItem item, bool setAsCurrent) {
        
        GameProfileMissionItems items = GetMissions();
        
        if (setAsCurrent) {
            //BaseGameProfileCharacters.currentCharacter = item;
            //GameProfileCharacters.Current.SetCurrentCharacterProfileCode(code);
        }
        
        items.SetItem(missionCode, item);
        SetMissions(items);
    }

    // ACTIONS

    // COLLECTIONS



	
}


