using System;
using System.Collections.Generic;
using System.IO;
using Engine.Data.Json;
using Engine.Utility;

public class GamePacks : DataObjects<GamePack>
{
	private static volatile GamePack current;
	private static volatile GamePacks instance;
	private static object syncRoot = new System.Object();
		
	public static string DATA_KEY = "pack-data";
	
#if UNITY_IPHONE	
	public static string currentPacksPlatform = "ios";
#elif UNITY_ANDROID	
	public static string currentPacksPlatform = "android";
#else
	public static string currentPacksPlatform = "web";
#endif
	
#if UNITY_IPHONE	
	public static int currentPacksIncrement = 3007; // Used for unity cache
#elif UNITY_ANDROID	
	public static int currentPacksIncrement = 1001; // Used for unity cache
#else
	public static int currentPacksIncrement = 50; // Used for unity cache
#endif
	
	public static string currentPacksVersion = "1.1"; // used for actual version and on dlc storage
	public static string currentPacksGame = "game-supasupacross";
	public static string currentGameBundle = "com.supasupagames.supasupacross";
		
	public static GamePack Current
	{
	  get 
	  {
	     if (current == null) 
	     {
	        lock (syncRoot) 
	        {
	           if (current == null) 
	              current = new GamePack();
	        }
	     }
	
	     return current;
	  }
		set {
			current = value;
		}
	}
		
	public static GamePacks Instance
	{
	  get 
	  {
	     if (instance == null) 
	     {
	        lock (syncRoot) 
	        {
	           if (instance == null) 
	              instance = new GamePacks(true);
	        }
	     }
	
	     return instance;
	  }
	}
	
	public static string PACK_DEFAULT = "default";	
	public static string PACK_SX_2012_1 = "sx-2012-pack-1";	
	public static string PACK_SX_2012_2 = "sx-2012-pack-2";
	
	public GamePacks() {
		Reset();
	}
	
	public GamePacks(bool loadData) {
		Reset();		
		path = "data/" + DATA_KEY +".json";
		pathKey = DATA_KEY;
		LoadData();
	}
	
	public void ChangeCurrentGamePack(string code) {
		Current = GetById(code);
		LogUtil.Log("Changing Pack: code:" + code);
		LogUtil.Log("Changing Pack: name:" + Current.name);
	}
	
	public bool OwnsAllPacks() {
		foreach(GamePack pack in GetAll()) {
			//if(pack.code != GameProducts.ITEM_PACK_SX_2012_ALL
			//	&& pack.code != GamePacks.PACK_DEFAULT) {
				if(!Contents.CheckGlobalContentAccess(pack.code)) {
					return false;
				}
			//}
		}
		return true;
	}
	
	public bool OwnsAtLeastOnePack() {
		foreach(GamePack pack in GetAll()) {
			//if(pack.code != GameProducts.ITEM_PACK_SX_2012_ALL
			//	&& pack.code != GamePacks.PACK_DEFAULT) {
				if(Contents.CheckGlobalContentAccess(pack.code)) {
					return true;
				}
			//}
		}
		return false;
	}
}


public class GamePack : DataObject 
{
	
	// Attributes that are added or changed after launch should be like this to prevent
	// profile conversions.
	
	public int sort_order = 0;
	public int sort_order_type = 0;
	public string type;
	public string key;
	public string game_id;
	public string order_by;
	public string code;
	public string display_name;
	public string name;
	public string description;
	public string status;
	public string url;
	public string host;
	public string uuid;
	public bool active;	
		
	public GamePack () {
		Reset();
	}
	
	public override void Reset() {
		base.Reset();
		
		sort_order = 0;
		sort_order_type = 0;
		type = "default";
		key = "default";
		game_id = "default";
		order_by = "default";
		code = "default";
		display_name = "default";
		name = "default";
		description = "default";
		status = "default";
		url = "default";
		host = "dlc.supasupagames.com";
		uuid = "default";
		active = true;	
	}
	
	public void Clone(GamePack toCopy) {
		
		base.Clone(toCopy);
		
		active = toCopy.active;
		code = toCopy.code;
		description = toCopy.description;
		display_name = toCopy.display_name;
		game_id = toCopy.game_id;
		key = toCopy.key;
		name = toCopy.name;
		order_by = toCopy.order_by;
		sort_order = toCopy.sort_order;
		sort_order_type = toCopy.sort_order_type;
		url = toCopy.url;
		host = toCopy.host;
		status = toCopy.status;
		type = toCopy.type;
		uuid = toCopy.uuid;
	}
	
	// Attributes that are added or changed after launch should be like this to prevent
	// profile conversions.

	/*
	public double GetInitialDifficulty() {
		return GetAttributeDoubleValue(GameLevelKeys.LEVEL_INITIAL_DIFFICULTY);
	}
	
	public void SetInitialDifficulty(double val) {
		SetAttributeDoubleValue(GameLevelKeys.LEVEL_INITIAL_DIFFICULTY, val);
	}
	*/
	
}
