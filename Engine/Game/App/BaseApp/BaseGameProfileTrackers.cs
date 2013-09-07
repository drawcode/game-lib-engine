using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class BaseGameProfileTrackers {
	private static volatile BaseGameProfileTracker current;
	private static volatile BaseGameProfileTrackers instance;
	private static object syncRoot = new Object();
	
	public static string DEFAULT_USERNAME = "Player";
	
	public static BaseGameProfileTracker Current {
		get {
	    	if (current == null) {
	        	lock (syncRoot) {
	           		if (current == null) 
	              		current = new BaseGameProfileTracker();
	        	}
	     	}
	
	     	return current;
	  	}
		set {
			current = value;
		}
	}
		
	public static BaseGameProfileTrackers Instance {
	  get {
	     if (instance == null) {
	        lock (syncRoot) {
	           if (instance == null) 
	              instance = new BaseGameProfileTrackers();
	        }
	     }
	
	     return instance;
	  }
	}
	
	// TODO: Common profile actions, lookup, count, etc
}

public class BaseGameProfileTracker : Profile  {
	// BE CAREFUL adding properties as they will cause a need for a profile conversion
	// Best way to add items to the profile is the GetAttribute and SetAttribute class as 
	// that stores as a generic DataAttribute class.  Booleans, strings, objects, serialized json objects etc
	// all work well and cause no need to convert profile on updates. 
		
	public BaseGameProfileTracker() {
		Reset();
	}
	
	public override void Reset() {
		base.Reset();
		username = "Player";// + UnityEngine.Random.Range(1, 9999999);
	}
		
	// ACHIEVEMENTS		
	
	public virtual void SetTrackerValue(string code, object value) {
		DataAttribute att = new DataAttribute();
		att.val = value;
		att.code = code;
		att.name = "";
		att.type = "string";
		att.otype = "tracker";
		SetAttribute(att);
	}
	
	public virtual string GetTrackerValue(string code) {
		string currentValue = "";
		object objectValue = GetAttribute(code).val;
		if(objectValue != null) {
			currentValue = Convert.ToString(objectValue);
		}
		
		return currentValue;
	}
		
	public virtual List<DataAttribute> GetAchievements() {
		return GetAttributesList("tracker");
	}
	
}

