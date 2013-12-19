using System;
using System.Collections.Generic;
using System.IO;
using Engine.Data.Json;
using Engine.Utility;

public class BaseGameAchievements<T> : DataObjects<T> where T : new() {
    private static T current;
    private static volatile BaseGameAchievements<T> instance;
    private static object syncRoot = new Object();
    public static string BASE_DATA_KEY = "game-achievement-data";

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

    public static BaseGameAchievements<T> BaseInstance {
        get {
            if (instance == null) {
                lock (syncRoot) {
                    if (instance == null)
                        instance = new BaseGameAchievements<T>(true);
                }
            }

            return instance;
        }
        set {
            instance = value;
        }
    }

    public BaseGameAchievements() {
        Reset();
    }

    public BaseGameAchievements(bool loadData) {
        Reset();
        path = "data/achievement-data.json";
        pathKey = BASE_DATA_KEY;
        LoadData();
    }
}

public class BaseGameAchievementKeys {

    public static string level = "level";
    public static string data = "data";
    public static string points = "points";
    public static string leaderboard = "leaderboard";
    public static string game_stat = "game_stat";
    public static string global = "global";
}

public class BaseGameAchievement : GameDataObject {


    public virtual string level {
        get {
            return Get<string>(BaseGameAchievementKeys.level);
        }
        
        set {
            Set<string>(BaseGameAchievementKeys.level, value);
        }
    }

    public virtual string data {
        get {
            return Get<string>(BaseGameAchievementKeys.data);
        }
        
        set {
            Set<string>(BaseGameAchievementKeys.data, value);
        }
    }
    
    public virtual int points {
        get {
            return Get<int>(BaseGameAchievementKeys.points);
        }
        
        set {
            Set<int>(BaseGameAchievementKeys.points, value);
        }
    }
    
    public virtual bool leaderboard {
        get {
            return Get<bool>(BaseGameAchievementKeys.leaderboard);
        }
        
        set {
            Set<bool>(BaseGameAchievementKeys.leaderboard, value);
        }
    }
    
    public virtual bool game_stat {
        get {
            return Get<bool>(BaseGameAchievementKeys.game_stat);
        }
        
        set {
            Set<bool>(BaseGameAchievementKeys.game_stat, value);
        }
    }

    public virtual bool global {
        get {
            return Get<bool>(BaseGameAchievementKeys.global);
        }
        
        set {
            Set<bool>(BaseGameAchievementKeys.global, value);
        }
    }

    public BaseGameAchievement() {
        Reset();
    }

    public override void Reset() {
        base.Reset();
    }
}