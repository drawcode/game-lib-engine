using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;


public class BaseGameConfigs<T> : DataObjects<T> where T : DataObject, new() {
    private static T current;
    private static volatile BaseGameConfigs<T> instance;
    private static object syncRoot = new Object();

	public static string BASE_DATA_KEY = "game-config-data";

    public static string MULTIPLAYER_GAME_CODE = "default";
    public static string MULTIPLAYER_GAME_TYPE = "default";

	public static bool usePooledGamePlayers = true;
	public static bool usePooledIndicators = false;
	public static bool usePooledProjectiles = true;
    public static bool usePooledItems = true;
    public static bool usePooledLevelItems = true;

    public static bool globalReady = false;

    public static bool useCoinRewardsForAchievements = true;
    public static double coinRewardAchievementPoint = 50;

    public static bool useNetworking = false;

	public static T BaseCurrent {
        get {
            if (current == null) {
                lock (syncRoot) {
                    if (current == null)
                        current = new T();
                }
            }

            return current;
        }
        set {
            current = value;
        }
    }

    public static BaseGameConfigs<T> BaseInstance {
        get {
            if (instance == null) {
                lock (syncRoot) {
                    if (instance == null)
                        instance = new BaseGameConfigs<T>(true);
                }
            }

            return instance;
        }
        set {
            instance = value;
        }
    }

    public BaseGameConfigs() {
        Reset();
    }

    public BaseGameConfigs(bool loadData) {
        Reset();
        path = "data/" + BASE_DATA_KEY + ".json";
        pathKey = BASE_DATA_KEY;
        LoadData();
    }
    
    public static bool isGameRunning {
        get {
            if (GameController.IsGameRunning
                && !isUIRunning) {
                return true;
            }
            return false;
        }
    }
    
    public static bool isUIRunning {
        get {
            if(GameUIController.Instance == null) {
                return false;
            }
            if (GameUIController.Instance.uiVisible) {
                return true;
            }
            return false;
        }
    }
}

public class BaseGameConfig : Config {

    public BaseGameConfig() {
        Reset();
    }

    public override void Reset() {
        base.Reset();
    }
}