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
// type (action), pack, tracker, appState, appContentState and a custom string for an
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

    
    public object GameFilterData(GameFilter filter) {
        if (filter != null) {
            object obj = filter.data;
            if (obj != null) {
                return obj;
            }
        }
        return null;
    }

    public List<GameFilter> GameFilters(string filterType) {
        List<GameFilter> filterList = new List<GameFilter>();
        if (data.filters != null) {
            foreach (GameFilter o in GameFilters()) {
                
                object val = GetFieldValue(o, "type");
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
    
    public List<GameFilter> GameFilters() {
        if (data.filters != null) {
            return data.filters;
        }
        return null;
    }
    
    public List<T> GameFilter<T>(string filterType) where T : GameFilterBase {
        List<GameFilter> objs = GameFilters(filterType);
        List<T> ts = new List<T>();
        if (objs != null) {
            ts = new List<T>();
            foreach (GameFilter o in objs) {
                ts.Add((T)o.data);
            }
        }
        return ts;
    }
    
    public List<GameFilterStatisticSingle> GetFilterStatisticSingle() {
        return GameFilter<GameFilterStatisticSingle>(GameFilterType.statisticSingle);
    }
    
    public List<GameFilterStatisticSet> GetFilterStatisticSet() {
        return GameFilter<GameFilterStatisticSet>(GameFilterType.statisticSet);
    }
    
    public List<GameFilterStatisticAll> GetFilterStatisticAll() {
        return GameFilter<GameFilterStatisticAll>(GameFilterType.statisticAll);
    }
    
    public List<GameFilterStatisticLike> GetFilterStatisticLike() {
        return GameFilter<GameFilterStatisticLike>(GameFilterType.statisticLike);
    }
    
    public List<GameFilterStatisticCompare> GetFilterStatisticCompare() {
        return GameFilter<GameFilterStatisticCompare>(GameFilterType.statisticCompare);
    }
    //GameFilterStatisticCompare
    
    public List<GameFilterAchievementSet> GetFilterAchievementSet() {
        return GameFilter<GameFilterAchievementSet>(GameFilterType.achievementSet);
    }
}