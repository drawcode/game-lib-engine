using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using Engine.Data.Json;
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
		username = "Player";// + UnityEngine.Random.Range(1, 9999999);
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
	
	// GAME MODES

	/*
	public int GetCurrentGameMode(){
		return GetCurrentGameMode((int)GameRaceMode.GAME_RACE_MODE_SERIES);
	}
	
	public int GetCurrentGameMode(int defaultValue){
		
		int attValue = defaultValue;
		if(CheckIfAttributeExists(GameProfileModeAttributes.ATT_CURRENT_RACE_MODE))
			attValue = GetAttributeIntValue(GameProfileModeAttributes.ATT_CURRENT_RACE_MODE);
		return attValue;
	}
	
	public void SetCurrentGameMode(int attValue) {
		SetAttributeIntValue(GameProfileModeAttributes.ATT_CURRENT_RACE_MODE, attValue);
	}
	*/
	
}


