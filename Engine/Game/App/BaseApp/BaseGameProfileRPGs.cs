using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

using Engine.Data.Json;
using Engine.Utility;

public class BaseGameProfileRPGAttributes {
	
	// RPG	
	public static string ATT_PROGRESS_CURRENCY = "progress-currency";
	public static string ATT_PROGRESS_XP = "progress-xp";
	public static string ATT_PROGRESS_HEALTH = "progress-health";
	public static string ATT_PROGRESS_ENERGY = "progress-energy";
	public static string ATT_PROGRESS_LEVEL = "progress-level";
}

public class BaseGameProfileRPGs {
	private static volatile BaseGameProfileRPG current;
	private static volatile BaseGameProfileRPGs instance;
	private static object syncRoot = new Object();
	
	public static string DEFAULT_USERNAME = "Player";
	
	public static BaseGameProfileRPG BaseCurrent {
		get {
	    	if (current == null) {
	        	lock (syncRoot) {
	           		if (current == null) 
	              		current = new BaseGameProfileRPG();
	        	}
	     	}
	
	     	return current;
	  	}
		set {
			current = value;
		}
	}
		
	public static BaseGameProfileRPGs BaseInstance {
	  get {
	     if (instance == null) {
	        lock (syncRoot) {
	           if (instance == null) 
	              instance = new BaseGameProfileRPGs();
	        }
	     }
	
	     return instance;
	  }
	}
	
	// TODO: Common profile actions, lookup, count, etc
}

public class BaseGameProfileRPG : Profile  {
	// BE CAREFUL adding properties as they will cause a need for a profile conversion
	// Best way to add items to the profile is the GetAttribute and SetAttribute class as 
	// that stores as a generic DataAttribute class.  Booleans, strings, objects, serialized json objects etc
	// all work well and cause no need to convert profile on updates. 
		
	public BaseGameProfileRPG() {
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
		att.otype = "rpg";
		SetAttribute(att);
	}
		
	public virtual List<DataAttribute> GetList() {
		return GetAttributesList("rpg");
	}
	
	
	// CURRENCY
		
    public virtual double GetGamePlayerProgressCurrency() {
        return GetGamePlayerProgressCurrency(10.0);
    }

    public virtual double GetGamePlayerProgressCurrency(double defaultValue) {
        double attValue = defaultValue;
        if (CheckIfAttributeExists(BaseGameProfileRPGAttributes.ATT_PROGRESS_CURRENCY))
            attValue = GetAttributeDoubleValue(BaseGameProfileRPGAttributes.ATT_PROGRESS_CURRENCY);
        return attValue;
    }

    public virtual void SetGamePlayerProgressCurrency(double attValue) {
        SetAttributeDoubleValue(BaseGameProfileRPGAttributes.ATT_PROGRESS_CURRENCY, attValue);
    }
	
		// RPG - Player specific
	
	public double GetGamePlayerProgressXP(){
		return GetGamePlayerProgressXP(10.0);
	}
	
	public double GetGamePlayerProgressXP(double defaultValue){
		double attValue = defaultValue;
		if(CheckIfAttributeExists(BaseGameProfileRPGAttributes.ATT_PROGRESS_XP))
			attValue = GetAttributeDoubleValue(BaseGameProfileRPGAttributes.ATT_PROGRESS_XP);
		return attValue;
	}
	
	public void SetGamePlayerProgressXP(double attValue) {
		SetAttributeDoubleValue(BaseGameProfileRPGAttributes.ATT_PROGRESS_XP, attValue);
	}
	
	public double GetGamePlayerProgressLevel(){
		return GetGamePlayerProgressLevel(0.0);
	}
	
	public double GetGamePlayerProgressLevel(double defaultValue){
		
		double attValue = defaultValue;
		if(CheckIfAttributeExists(BaseGameProfileRPGAttributes.ATT_PROGRESS_LEVEL))
			attValue = GetAttributeDoubleValue(BaseGameProfileRPGAttributes.ATT_PROGRESS_LEVEL);
		return attValue;
	}
	
	public void SetGamePlayerProgressLevel(double attValue) {
		SetAttributeDoubleValue(BaseGameProfileRPGAttributes.ATT_PROGRESS_LEVEL, attValue);
	}	
	
	public double GetGamePlayerProgressHealth(){
		return GetGamePlayerProgressHealth(1.0);
	}
	
	public double GetGamePlayerProgressHealth(double defaultValue){
		
		double attValue = defaultValue;
		if(CheckIfAttributeExists(BaseGameProfileRPGAttributes.ATT_PROGRESS_HEALTH))
			attValue = GetAttributeDoubleValue(BaseGameProfileRPGAttributes.ATT_PROGRESS_HEALTH);
		return attValue;
	}
	
	public void SetGamePlayerProgressHealth(double attValue) {
		SetAttributeDoubleValue(BaseGameProfileRPGAttributes.ATT_PROGRESS_HEALTH, attValue);
	}	
	
	
	public double GetGamePlayerProgressEnergy(){
		return GetGamePlayerProgressEnergy(0.0);
	}
	
	public double GetGamePlayerProgressEnergy(double defaultValue){
		
		double attValue = defaultValue;
		if(CheckIfAttributeExists(BaseGameProfileRPGAttributes.ATT_PROGRESS_ENERGY))
			attValue = GetAttributeDoubleValue(BaseGameProfileRPGAttributes.ATT_PROGRESS_ENERGY);
		return attValue;
	}
	
	public void SetGamePlayerProgressEnergy(double attValue) {
		SetAttributeDoubleValue(BaseGameProfileRPGAttributes.ATT_PROGRESS_ENERGY, attValue);
	}
	
	
}


