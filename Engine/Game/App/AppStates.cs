using System;
using System.Collections.Generic;
using System.IO;

public class AppStates : BaseAppStates<AppState>
{
	private static volatile AppState current;
	private static volatile AppStates instance;
	private static System.Object syncRoot = new System.Object();
	
	public static string APP_STATE_BOOKS = "app-state-books";
	public static string APP_STATE_CARDS = "app-state-cards";
	public static string APP_STATE_GAMES = "app-state-games";
	public static string APP_STATE_SETTINGS = "app-state-settings";
	public static string APP_STATE_TROPHIES = "app-state-trophies";
		
	public static string DATA_KEY = "app-state-data";
		
	public static AppState Current {
		get  {
			if (current == null) {
				lock (syncRoot)  {
				   if (current == null) 
				      current = new AppState();
				}
			}
			
			return current;
		}
		set {
			current = value;
		}
	}
		
	public static AppStates Instance {
		get  {
			if (instance == null) {
				lock (syncRoot)  {
				   if (instance == null) 
				      instance = new AppStates(true);
				}
			}
			
			return instance;
		}
		set {
			instance = value;
		}
	}
			
	public AppStates() {
		Reset();
		ChangeState(APP_STATE_BOOKS);
	}
	
	public AppStates(bool loadData) {
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

public class AppState : BaseAppState 
{
	
	// Attributes that are added or changed after launch should be like this to prevent
	// profile conversions.
			
	public AppState () {
		Reset();
	}
	
	public override void Reset() {
		base.Reset();
	}
}
