using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using Engine.Data.Json;
using Engine.Utility;

public class BaseGameProfileCharacterAttributes {	

	public static string ATT_CHARACTER_CODE = "att-character-code";
	public static string ATT_CHARACTER_COSTUME_CODE = "att-character-costume-code";
}	

public class BaseGameProfileCharacters {
	private static volatile BaseGameProfileCharacter current;
	private static volatile BaseGameProfileCharacters instance;
	private static object syncRoot = new Object();
	
	public static string DEFAULT_USERNAME = "Player";
	
	public static BaseGameProfileCharacter BaseCurrent {
		get {
	    	if (current == null) {
	        	lock (syncRoot) {
	           		if (current == null) 
	              		current = new BaseGameProfileCharacter();
	        	}
	     	}
	
	     	return current;
	  	}
		set {
			current = value;
		}
	}
		
	public static BaseGameProfileCharacters BaseInstance {
	  get {
	     if (instance == null) {
	        lock (syncRoot) {
	           if (instance == null) 
	              instance = new BaseGameProfileCharacters();
	        }
	     }
	
	     return instance;
	  }
	}
	
	// TODO: Common profile actions, lookup, count, etc
}

public class BaseGameProfileCharacter : Profile  {
	// BE CAREFUL adding properties as they will cause a need for a profile conversion
	// Best way to add items to the profile is the GetAttribute and SetAttribute class as 
	// that stores as a generic DataAttribute class.  Booleans, strings, objects, serialized json objects etc
	// all work well and cause no need to convert profile on updates. 
		
	public BaseGameProfileCharacter() {
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
		att.otype = "character";
		SetAttribute(att);
	}
	
	public virtual bool GetValue(string code) {
		bool currentValue = false;
		object objectValue = GetAttribute(code).val;
		if(objectValue != null) {
			currentValue = Convert.ToBoolean(objectValue);
		}
		
		return currentValue;
	}
		
	public virtual List<DataAttribute> GetAll() {
		return GetAttributesList("character");
	}
	
	
	// CHARACTER - Player specific
	
	public string GetCurrentCharacterCode(){
		return GetCurrentCharacterCode("default");
	}
	
	public string GetCurrentCharacterCode(string defaultValue){
		string attValue = defaultValue;
		if(CheckIfAttributeExists(BaseGameProfileCharacterAttributes.ATT_CHARACTER_CODE))
			attValue = GetAttributeStringValue(BaseGameProfileCharacterAttributes.ATT_CHARACTER_CODE);
		return attValue;
	}
	
	public void SetCurrentCharacterCode(string attValue) {
		SetAttributeStringValue(BaseGameProfileCharacterAttributes.ATT_CHARACTER_CODE, attValue);
	}	
	
	public string GetCurrentCharacterCostumeCode(){
		return GetCurrentCharacterCostumeCode("default");
	}
	
	public string GetCurrentCharacterCostumeCode(string defaultValue){
		string attValue = defaultValue;
		if(CheckIfAttributeExists(BaseGameProfileCharacterAttributes.ATT_CHARACTER_COSTUME_CODE))
			attValue = GetAttributeStringValue(BaseGameProfileCharacterAttributes.ATT_CHARACTER_COSTUME_CODE);
		return attValue;
	}
	
	public void SetCurrentCharacterCostumeCode(string attValue) {
		SetAttributeStringValue(BaseGameProfileCharacterAttributes.ATT_CHARACTER_COSTUME_CODE, attValue);
	}	
	
}


