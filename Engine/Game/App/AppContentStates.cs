using System;
using System.Collections.Generic;
using System.IO;

using Engine.Events;
using Engine.Data.Json;

public class AppContentStates : BaseAppContentStates<AppContentState>
{
	private static volatile AppContentState current;
	private static volatile AppContentStates instance;
	private static System.Object syncRoot = new System.Object();
	/*
	public static string APP_STATE_BOOKS = "app-state-books";
	public static string APP_STATE_CARDS = "app-state-cards";
	public static string APP_STATE_GAMES = "app-state-games";
	public static string APP_STATE_SETTINGS = "app-state-settings";
	public static string APP_STATE_TROPHIES = "app-state-trophies";
	*/
	
	public static string DATA_KEY = "app-content-state-data";
		
	public static AppContentState Current {
		get  {
			if (current == null) {
				lock (syncRoot)  {
				   if (current == null) 
				      current = new AppContentState();
				}
			}
			
			return current;
		}
		set {
			current = value;
		}
	}
		
	public static AppContentStates Instance {
		get  {
			if (instance == null) {
				lock (syncRoot)  {
				   if (instance == null) 
				      instance = new AppContentStates(true);
				}
			}
			
			return instance;
		}
		set {
			instance = value;
		}
	}
			
	public AppContentStates() {
		Reset();
		//ChangeState(APP_STATE_BOOKS);
	}
	
	public AppContentStates(bool loadData) {
		Reset();
		path = "data/" + DATA_KEY + ".json";
		pathKey = DATA_KEY;
		LoadData();
	}
	
	public void ChangeState(string code) {
		if(Current.code != code) {
			Current = GetById(code);
		}
	}
	
	/*
	public List<AppContentState> GetListByPack(string packCode) {
		List<AppContentState> filteredList = new List<AppContentState>();
		foreach(AppContentState obj in GetAll()) {
			if(packCode.ToLower() == obj.pack_code.ToLower()) {
				filteredList.Add(obj);
			}
		}
		
		return filteredList;
	}
	*/
	
	public List<AppContentState> GetListByCodeAndPackCode(string stateCode, string packCode) {
		List<AppContentState> filteredList = new List<AppContentState>();
		foreach(AppContentState obj in GetListByPack(packCode)) {
			if(stateCode.ToLower() == obj.code.ToLower()) {
				filteredList.Add(obj);
			}
		}
		
		return filteredList;
	}
	
	public List<AppContentState> GetByAppState(string appState) {
		Dictionary<string, int> packSorts = new Dictionary<string, int>();
		List<AppContentState> appContentStates = new List<AppContentState>();
		foreach(AppContentState state in GetAll()) {
			if(state.appStates.Contains(appState) 
				|| state.appStates.Contains("*")) {
				int sortOrder = 1;
				if(packSorts.ContainsKey(state.pack_code)) {
					sortOrder = packSorts[state.pack_code];
				}
				else {
					GamePack gamePack = GamePacks.Instance.GetById(state.pack_code);
					if(gamePack != null) {
						sortOrder = gamePack.sort_order;
						packSorts.Add(state.pack_code, sortOrder);
					}
				}
				state.sort_order = state.sort_order * sortOrder;
				appContentStates.Add(state);
			}			
		}
		appContentStates = SortList(appContentStates);
		
		return appContentStates;
	}
	
	
	/*
	public void ChangeState(AppState appStateTo) {
		if(lastAppState != appStateTo) {
			appState = appStateTo;
			HandleStateChange();
		}
	}
	
	public void HandleStateChange() {
		if(appState == AppState.StateNotSet) {
			OnAppStateNotStarted();
		}
		else if(appState == AppState.StateBook) {
			OnAppStateBooks();
		}
		else if(appState == AppState.StateCards) {
			OnAppStateCards();
		}
		else if(appState == AppState.StateGames) {
			OnAppStateGames();
		}
	}
	
	public void OnAppStateNotStarted() {
		appState = AppState.StateNotSet;
		appSubState = AppSubState.StateSubEnvironment;
		
		Messenger<AppState, AppSubState>.Broadcast(AppViewerAppControllerMessages.StateNotSet, appState, appSubState);
	}
	
	public void OnAppStateBooks() {
		appState = AppState.StateBook;
		appSubState = AppSubState.StateSubEnvironment;
			
		currentApp = new App();
		currentApp.appCode = "default";
		
		Messenger<AppState, AppSubState>.Broadcast(AppViewerAppControllerMessages.StateBooks, appState, appSubState);
	}
	
	public void OnAppStateCards() {
		appState = AppState.StateCards;
		appSubState = AppSubState.StateSubEnvironment;
			
		currentApp = new App();
		currentApp.appCode = "default";
		
		Messenger<AppState, AppSubState>.Broadcast(AppViewerAppControllerMessages.StateCards, appState, appSubState);
	}
	
	public void OnAppStateGames() {
		appState = AppState.StateCards;
		appSubState = AppSubState.StateSubEnvironment;
			
		currentApp = new App();
		currentApp.appCode = "default";
		
		Messenger<AppState, AppSubState>.Broadcast(AppViewerAppControllerMessages.StateGames, appState, appSubState);
	}
	*/
}

public class DataPlatformStoreMeta {
	public string platform = "";
	public string url = "";
	public string storeUrl = "";
	public string appId = "";
	public string locale = "en";
	public string price = "0";
}

public class AppContentState : BaseAppContentState 
{
		
	// Attributes that are added or changed after launch should be like this to prevent
	// profile conversions.
			
	public AppContentState () {
		Reset();
	}
	
	public override void Reset() {
		base.Reset();
	}
	
	public DataPlatformStoreMeta GetDataPlatformAttribute(string key) {
		key = GetPlatformAttributeKey(key);
		//UnityEngine.Debug.LogWarning("key:" + key);
		string json = GetAttributeStringValue(key);
		//UnityEngine.Debug.LogWarning("json:" + json);
		//UnityEngine.Debug.LogWarning("json:" + json.Replace("\\\"", "\""));
		if(!string.IsNullOrEmpty(json)) {
			try {
				return JsonMapper.ToObject<DataPlatformStoreMeta>(json);
			}
			catch (Exception e){
				UnityEngine.Debug.LogWarning("Error parsing DataPlatformStoreMeta" + e);
			}
		}
		return null;
	}
	
	public DataPlatformStoreMeta GetDataPlatformStoreMeta() {
		string key = "storemeta";
		return GetDataPlatformAttribute(key);
	}
	
	public bool IsExternalContent() {
		return GetDataPlatformStoreMeta() != null;
	}
	
}
