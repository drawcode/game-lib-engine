using System;
using System.Collections.Generic;
using System.IO;

using UnityEngine;

using Engine.Data.Json;
using Engine.Utility;

public class BaseGameAchievements<T> : DataObjects<T> where T : DataObject, new() {
    private static T current;
    private static volatile BaseGameAchievements<T> instance;
    private static System.Object syncRoot = new System.Object();
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

// Used to determin if an achievement should also filter on other params such as
// type (action), pack, tracker, app_state, app_content_state and a custom string for an
// object or value...



public class GameAchievementData : GameDataObject {

    public virtual List<GameFilter> filters {
        get {
            return Get<List<GameFilter>>(BaseDataObjectKeys.filters);
        }
        
        set {
            Set(BaseDataObjectKeys.filters, value);
        }
    }

    public virtual List<GameNetworkData> networks {
        get {
            return Get<List<GameNetworkData>>(BaseDataObjectKeys.networks);
        }
        
        set {
            Set(BaseDataObjectKeys.networks, value);
        }
    }
        
    public virtual string level {
        get {
            return Get<string>(BaseDataObjectKeys.level);
        }
        
        set {
            Set<string>(BaseDataObjectKeys.level, value);
        }
    }
    
    public virtual int points {
        get {
            return Get<int>(BaseDataObjectKeys.points);
        }
        
        set {
            Set<int>(BaseDataObjectKeys.points, value);
        }
    }
    
    public virtual bool leaderboard {
        get {
            return Get<bool>(BaseDataObjectKeys.leaderboard);
        }
        
        set {
            Set<bool>(BaseDataObjectKeys.leaderboard, value);
        }
    }
    
    public virtual bool game_stat {
        get {
            return Get<bool>(BaseDataObjectKeys.game_stat);
        }
        
        set {
            Set<bool>(BaseDataObjectKeys.game_stat, value);
        }
    }
    
    public virtual bool global {
        get {
            return Get<bool>(BaseDataObjectKeys.global);
        }
        
        set {
            Set<bool>(BaseDataObjectKeys.global, value);
        }
    }
}

public class BaseGameAchievement : GameDataObject {


    public virtual GameAchievementData data {
        get {
            return Get<GameAchievementData>(BaseDataObjectKeys.data);
        }
        
        set {
            Set(BaseDataObjectKeys.data, value);
        }
    }

    public BaseGameAchievement() {
        Reset();
    }

    public override void Reset() {
        base.Reset();
    }
    
    public GameFilterBase GameFilterData(GameFilter filter) {
        if (filter != null) {
            GameFilterBase obj = filter.data;
            if (obj != null) {
                return obj;
            }
        }
        return null;
    }

    public List<GameFilter> GetGameFilters(string filterType) {
        List<GameFilter> filterList = new List<GameFilter>();
        if (data.filters != null) {
            foreach (GameFilter o in GetGameFilters()) {
                
                object val = o.type;//GetFieldValue(o, "type");
                if (val != null) {
                    if ((string)val == filterType) {
                        filterList.Add(o);
                    }
                }
            }
            return filterList;
        }
        return null;
    }
    
    public List<GameFilter> GetGameFilters() {
        if (data.filters != null) {
            return data.filters;
        }
        return null;
    }
    
    public List<GameFilterBase> GetGameFilter(string filterType) {
        List<GameFilter> objs = GetGameFilters(filterType);
        List<GameFilterBase> ts = new List<GameFilterBase>();
        if (objs != null) {
            ts = new List<GameFilterBase>();
            foreach (GameFilter o in objs) {
                ts.Add(o.data);
            }
        }
        return ts;
    }
    
    public List<GameFilterBase> GetFilterStatisticSingle() {
        return GetGameFilter(GameFilterType.statisticSingle);
    }
    
    public List<GameFilterBase> GetFilterStatisticSet() {
        return GetGameFilter(GameFilterType.statisticSet);
    }
    
    public List<GameFilterBase> GetFilterStatisticAll() {
        return GetGameFilter(GameFilterType.statisticAll);
    }
    
    public List<GameFilterBase> GetFilterStatisticLike() {
        return GetGameFilter(GameFilterType.statisticLike);
    }
    
    public List<GameFilterBase> GetFilterStatisticCompare() {
        return GetGameFilter(GameFilterType.statisticCompare);
    }
    //GameFilterStatisticCompare
    
    public List<GameFilterBase> GetFilterAchievementSet() {
        return GetGameFilter(GameFilterType.achievementSet);
    }
}