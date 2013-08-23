using System;
using System.Collections.Generic;
using System.IO;

using UnityEngine;

using Engine.Utility;

public class ARDataSetTrackers : BaseARDataSetTrackers<ARDataSetTracker>
{
	private static volatile ARDataSetTracker current;
	private static volatile ARDataSetTrackers instance;
	private static System.Object syncRoot = new System.Object();
		
	public List<ARDataSetTracker> currentTrackers;
	public List<ARDataSetTracker> currentViewableTrackers;
	public List<ARDataSetTracker> currentSelectedTrackers;
		
	public static string DATA_KEY = "ar-data-tracker-data";
		
	public static ARDataSetTracker Current {
	  get {
	     if (current == null) {
	        lock (syncRoot) {
	           	if (current == null) 
	            	current = new ARDataSetTracker();
	        	}
	     	}
	
	    	return current;
	  	}
		set {
			current = value;
		}
	}
		
	public static ARDataSetTrackers Instance {
	  get {
	     if (instance == null) {
	        lock (syncRoot) {
	           if (instance == null) 
	              instance = new ARDataSetTrackers(true);
	        }
	     }
	
	     return instance;
	  }
	}
	
	public ARDataSetTrackers() {
		Reset();
	}
	
	public override void Reset () {
		base.Reset ();
		
		currentSelectedTrackers = new List<ARDataSetTracker>();
		currentTrackers = new List<ARDataSetTracker>();
		currentViewableTrackers = new List<ARDataSetTracker>();
	}
	
	public ARDataSetTrackers(bool loadData) {
		Reset();
		path = "data/" + DATA_KEY + ".json";
		pathKey = DATA_KEY;
		LoadData();
	}
		
	public void ChangeCurrent(string code) {
		Current = GetById(code);
		LogUtil.Log("Changing :" + Current.GetType() + ": code:" + code);
		LogUtil.Log("Changing :" + Current.GetType() + ": name:" + Current.name);
	}
	
	public bool IsTrackerVisibleByCode(string trackerCode) {
		if(currentTrackers != null) {
			foreach(ARDataSetTracker tracker in currentTrackers) {
				if(tracker.code.ToLower() == trackerCode.ToLower()) {
					return true;
				}
			}
			
		}
		return false;
	}
	
	public bool IsTrackerVisibleByName(string trackerName) {
		if(currentTrackers != null) {
			foreach(ARDataSetTracker tracker in currentTrackers) {
				if(tracker.name.ToLower() == trackerName.ToLower()) {
					return true;
				}
			}			
		}
		return false;
	}
		
	public List<ARDataSetTracker> GetListByCodeAndPackCode(string trackerCode, string packCode) {
		List<ARDataSetTracker> filteredList = new List<ARDataSetTracker>();
		foreach(ARDataSetTracker tracker in GetListByPack(packCode)) {
			if(trackerCode.ToLower() == tracker.code.ToLower()) {
				filteredList.Add(tracker);
			}
		}
		
		return filteredList;
	}
	
	public void SetTrackerFound(string trackerName, string packCode) {
		// Find from vuforia tracker name
		foreach(ARDataSetTracker tracker in GetAll()) {
			if(tracker.name.ToLower() == trackerName.ToLower() 
				&& tracker.pack_code.ToLower() == packCode.ToLower()) {
				SetTrackerFound(tracker);
				////Debug.Log("SetTrackerLost: trackerName:" + trackerName);
				//break;
			}
		}
	}
	
	public void SetTrackerSelected(string trackerCode, string packCode) {
		ARDataSetTracker tracker = null;
		foreach(ARDataSetTracker t in GetListByCodeAndPackCode(trackerCode, packCode)) {
			tracker = t;
			break;
		}
			//ChangeCurrent(tracker.code);
		if(tracker != null) {
			if(!currentSelectedTrackers.Contains(tracker)) {
				currentSelectedTrackers.Add(tracker);
			}
		}
			
	}
	
	public void SetTrackerDeselect(string trackerCode) {
		ARDataSetTracker tracker = GetById(trackerCode);
		Current = null;
		if(tracker != null) {
			if(!currentSelectedTrackers.Contains(tracker)) {
				currentSelectedTrackers.RemoveAll(item => item == tracker);
			}
		}
	}
	
	public void SetTrackerLost(string trackerName, string packCode) {
		// Find from vuforia tracker name
		foreach(ARDataSetTracker tracker in GetAll()) {
			if(tracker.name.ToLower() == trackerName.ToLower() 
				&& tracker.pack_code.ToLower() == packCode.ToLower()) {
				SetTrackerLost(tracker);
				////Debug.Log("SetTrackerLost: trackerName:" + trackerName);
				//break;
			}
		}
	}
	
	public bool IfSelectedTrackersContains(string code) {
		foreach(ARDataSetTracker tracker in currentSelectedTrackers) {
			if(tracker.code.ToLower() == code.ToLower()) {
				return true;
			}
		}
		return false;
	}
	
	public bool IfCurrentTrackersContains(string code) {
		foreach(ARDataSetTracker tracker in currentTrackers) {
			if(tracker.code.ToLower() == code.ToLower()) {
				return true;
			}
		}
		return false;
	}
	
	public bool IfCurrentViewableTrackersContains(string code) {
		foreach(ARDataSetTracker tracker in currentViewableTrackers) {
			if(tracker.code.ToLower() == code.ToLower()) {
				return true;
			}
		}
		return false;
	}
	
	public void SetTrackerFound(ARDataSetTracker trackerTo) {
		
		Current = trackerTo;
		
		if(!IfSelectedTrackersContains(trackerTo.code)) {
			currentSelectedTrackers.Add(trackerTo);
		}
		
		if(!IfCurrentTrackersContains(trackerTo.code)) {
			currentTrackers.Add(trackerTo);
		}
		
		if(!IfCurrentViewableTrackersContains(trackerTo.code)) {
			currentViewableTrackers.Add(trackerTo);
		}
		
		
		//Debug.Log("SetTrackerFound:trackerName:" + trackerTo.name);
		//Debug.Log("SetTrackerFound:currentTrackers:" + currentTrackers.Count);
		
		//foreach(ARDataSetTracker tr in currentTrackers) {			
			//Debug.Log("SetTrackerFound:tracker:" + tr.name + " trackerCode:" + tr.code);
		//}
	}
	
	public void SetTrackerLost(ARDataSetTracker tracker) {
		if(IfCurrentTrackersContains(tracker.code)) {
			currentTrackers.RemoveAll(item => item.code.ToLower() == tracker.code.ToLower());
		}
		
		if(IfCurrentViewableTrackersContains(tracker.code)) {
			currentViewableTrackers.RemoveAll(item => item.code.ToLower() == tracker.code.ToLower());
		}
		
		if(IfSelectedTrackersContains(tracker.code)) {
			currentSelectedTrackers.RemoveAll(item => item.code.ToLower() == tracker.code.ToLower());
		}
		
		//Debug.Log("SetTrackerLost:trackerName:" + tracker.name);
		//Debug.Log("SetTrackerLost:currentTrackers:" + currentTrackers.Count);
		
		//foreach(ARDataSetTracker tr in currentTrackers) {			
			//Debug.Log("SetTrackerLost:tracker:" + tr.name + " trackerCode:" + tr.code);
		//}
	}
	
	public bool HasTrackersFound() {
		if(currentTrackers.Count > 0) {
			return true;
		}
		return false;
	}
	
	public List<ARDataSetTracker> GetByDataSetCode(string dataSetCode) {
		List<ARDataSetTracker> trackersByPack = new List<ARDataSetTracker>();
		foreach(ARDataSetTracker tracker in GetAll()) {
			if(tracker.ar_data_set_code.ToLower() == dataSetCode.ToLower()) {
				trackersByPack.Add(tracker);
			}			
		}
		return trackersByPack;
	}
	
	public List<ARDataSetTracker> GetInPack(string packCode) {
		List<ARDataSetTracker> trackersByPack = new List<ARDataSetTracker>();
		foreach(ARDataSetTracker tracker in GetAll()) {
			if(tracker.pack_code.ToLower() == packCode.ToLower()) {
				trackersByPack.Add(tracker);
			}			
		}
		return trackersByPack;
	}
	
	public ARDataSetTracker GetByCodeAndPack(string code, string packCode) {
		foreach(ARDataSetTracker tracker in GetAll()) {
			if(tracker.code.ToLower() == code.ToLower()
				&& tracker.pack_code.ToLower() == packCode.ToLower()) {
				return tracker;
			}			
		}
		return null;
	}
	
	public ARDataSetTracker GetByNameAndPack(string name, string packCode) {
		foreach(ARDataSetTracker tracker in GetAll()) {
			if(tracker.name.ToLower() == name.ToLower()
				&& tracker.pack_code.ToLower() == packCode.ToLower()) {
				return tracker;
			}			
		}
		return null;
	}
	
	public void ClearAllTrackers() {
		if(currentTrackers != null) {
			currentTrackers.Clear();
		}
		
		if(currentViewableTrackers != null) {
			currentViewableTrackers.Clear();
		}
		
		if(currentSelectedTrackers != null) {
			currentSelectedTrackers.Clear();
		}
	}
}
public class ARDataSetTrackerAttributes {
	public static string image_width = "image_width";
	public static string image_height = "image_height";
	public static string tracker_scale_x = "tracker_scale_x";
	public static string tracker_scale_y = "tracker_scale_y";
	public static string tracker_scale_z = "tracker_scale_z";
	public static string tracker_local_position_x = "tracker_local_position_x";
	public static string tracker_local_position_y = "tracker_local_position_y";
	public static string tracker_local_position_z = "tracker_local_position_z";
}

public class ARDataSetTracker : BaseARDataSetTracker 
{
	
	public string ar_data_set_code = "";
	public Dictionary<string, object> content_attributes;	
	
	// Attributes that are added or changed after launch should be like this to prevent
	// profile conversions.
			
	public ARDataSetTracker () {
		Reset();
	}
	
	public override void Reset() {
		base.Reset();
		content_attributes = new Dictionary<string, object>();
	}
		
	public int GetAttributeInt(string key) {
		int content = 0;
		if(content_attributes.ContainsKey(key)) {
			content = (int)content_attributes[key];
		}
		return content;
	}
	
	public double GetAttributeDouble(string key) {
		double content = 1;
		if(content_attributes.ContainsKey(key)) {
			content = (double)content_attributes[key];
		}
		return content;
	}
	
	public Vector3Data GetTrackerScale() {
		Vector3Data data = new Vector3Data();
		data.x = GetAttributeDouble(ARDataSetTrackerAttributes.tracker_scale_x);
		data.y = GetAttributeDouble(ARDataSetTrackerAttributes.tracker_scale_y);
		data.z = GetAttributeDouble(ARDataSetTrackerAttributes.tracker_scale_z);		
		return data;
	}
	
	public Vector3Data GetTrackerLocalPosition() {
		Vector3Data data = new Vector3Data();
		data.x = GetAttributeDouble(ARDataSetTrackerAttributes.tracker_local_position_x);
		data.y = GetAttributeDouble(ARDataSetTrackerAttributes.tracker_local_position_y);
		data.z = GetAttributeDouble(ARDataSetTrackerAttributes.tracker_local_position_z);		
		return data;
	}
	
	public object GetAttributeObject(string key) {
		object content = null;
		if(content_attributes.ContainsKey(key)) {
			content = content_attributes[key];
		}
		return content;
	}
	
	public Vector2 GetImageSize() {
		Vector2 vec = Vector2.zero;
		vec.x = GetAttributeInt(ARDataSetTrackerAttributes.image_width);
		vec.y = GetAttributeInt(ARDataSetTrackerAttributes.image_height);
		return vec;
	}
}
