using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using Engine.Data.Json;
using Engine.Utility;

public class BaseGameProfileCustomizationAttributes {
	
	public static string ATT_CHARACTERS = "att-characters";
	
	// CUSTOM
	
	public static string ATT_CUSTOM_AUDIO = "custom-audio";
	public static string ATT_CUSTOM_COLORS = "custom-colors";
	public static string ATT_CUSTOM_COLOR_BIKE = "custom-color-bike";
	public static string ATT_CUSTOM_COLOR_RIDER = "custom-color-rider";
	public static string ATT_CUSTOM_COLOR_SHIRT = "custom-color-shirt";
	public static string ATT_CUSTOM_COLOR_SKIN = "custom-color-skin";
	public static string ATT_CUSTOM_COLOR_BOOTSGLOVES = "custom-color-bootsgloves";
	public static string ATT_CUSTOM_COLOR_SLEEVESPANTS = "custom-color-sleevespants";
}

public class BaseGameProfileCustomizations {
	private static volatile BaseGameProfileCustomization current;
	private static volatile BaseGameProfileCustomizations instance;
	private static object syncRoot = new Object();
	
	public static string DEFAULT_USERNAME = "Player";
	
	public static BaseGameProfileCustomization BaseCurrent {
		get {
	    	if (current == null) {
	        	lock (syncRoot) {
	           		if (current == null) 
	              		current = new BaseGameProfileCustomization();
	        	}
	     	}
	
	     	return current;
	  	}
		set {
			current = value;
		}
	}
		
	public static BaseGameProfileCustomizations BaseInstance {
	  get {
	     if (instance == null) {
	        lock (syncRoot) {
	           if (instance == null) 
	              instance = new BaseGameProfileCustomizations();
	        }
	     }
	
	     return instance;
	  }
	}
	
	// TODO: Common profile actions, lookup, count, etc
}

public class BaseGameProfileCustomization : Profile  {
	// BE CAREFUL adding properties as they will cause a need for a profile conversion
	// Best way to add items to the profile is the GetAttribute and SetAttribute class as 
	// that stores as a generic DataAttribute class.  Booleans, strings, objects, serialized json objects etc
	// all work well and cause no need to convert profile on updates. 
		
	public BaseGameProfileCustomization() {
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
		att.otype = "customization";
		SetAttribute(att);
	}
		
	public virtual List<DataAttribute> GetList() {
		return GetAttributesList("customization");
	}	
}


