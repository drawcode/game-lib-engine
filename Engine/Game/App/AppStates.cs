using System;
using System.Collections.Generic;
using System.IO;

public class AppStateMeta : BaseAppStateMeta {
    //public static string appModeTypeGameDefault = "app-mode-game-default";
}

public class AppStates : BaseAppStates<AppState>
{
	private static volatile AppState current;
	private static volatile AppStates instance;
	private static object syncRoot = new System.Object();

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
	}
	
	public AppStates(bool loadData) {
		Reset();
		path = "data/" + DATA_KEY + ".json";
		pathKey = DATA_KEY;
		LoadData();
	}
	
	/*
	public void ChangeState(AppState app_stateTo) {
		if(lastAppState != app_stateTo) {
			app_state = app_stateTo;
			HandleStateChange();
		}
	}
	
	public void HandleStateChange() {
		if(app_state == AppState.StateNotSet) {
			OnAppStateNotStarted();
		}
		else if(app_state == AppState.StateBook) {
			OnAppStateBooks();
		}
		else if(app_state == AppState.StateCards) {
			OnAppStateCards();
		}
		else if(app_state == AppState.StateGames) {
			OnAppStateGames();
		}
	}
	
	public void OnAppStateNotStarted() {
		app_state = AppState.StateNotSet;
		appSubState = AppSubState.StateSubEnvironment;
		
		Messenger<AppState, AppSubState>.Broadcast(AppViewerAppControllerMessages.StateNotSet, app_state, appSubState);
	}
	
	public void OnAppStateBooks() {
		app_state = AppState.StateBook;
		appSubState = AppSubState.StateSubEnvironment;
			
		currentApp = new App();
		currentApp.appCode = "default";
		
		Messenger<AppState, AppSubState>.Broadcast(AppViewerAppControllerMessages.StateBooks, app_state, appSubState);
	}
	
	public void OnAppStateCards() {
		app_state = AppState.StateCards;
		appSubState = AppSubState.StateSubEnvironment;
			
		currentApp = new App();
		currentApp.appCode = "default";
		
		Messenger<AppState, AppSubState>.Broadcast(AppViewerAppControllerMessages.StateCards, app_state, appSubState);
	}
	
	public void OnAppStateGames() {
		app_state = AppState.StateCards;
		appSubState = AppSubState.StateSubEnvironment;
			
		currentApp = new App();
		currentApp.appCode = "default";
		
		Messenger<AppState, AppSubState>.Broadcast(AppViewerAppControllerMessages.StateGames, app_state, appSubState);
	}
	*/
}

public class AppState : BaseAppState {
	
	// Attributes that are added or changed after launch should be like this to prevent
	// profile conversions.
			
	public AppState () {
		Reset();
	}
	
	public override void Reset() {
		base.Reset();
	}
}
