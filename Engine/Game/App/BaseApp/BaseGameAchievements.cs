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

public class BaseGameAchievementKeys {

    public static string level = "level";
    public static string data = "data";
    public static string points = "points";
    public static string leaderboard = "leaderboard";
    public static string game_stat = "game_stat";
    public static string global = "global";
}
public class GameAchievementFilterType {
    public static string statisticSingle = "statistic-single";
    public static string statisticAll = "statistic-all";
    public static string statisticLike = "statistic-like";
    public static string statisticCompare = "statistic-compare";
    public static string statisticSet = "statistic-set";
    public static string achievementSet = "achievement-set";
}

public class GameAchievementFilterIncludeType {
    public static string none = "none";
    public static string current = "current";
    public static string all = "all";
}

public class GameAchievementCodeType {
    public static string like = "like";
    public static string equal = "equal";
    public static string startsWith = "startsWith";
    public static string endsWith = "endsWith";
    public static string all = "all";
}

// Used to determin if an achievement should also filter on other params such as
// type (action), pack, tracker, appState, appContentState and a custom string for an
// object or value...
public class GameAchievementFilterIncludeKeys {
    public string defaultKey = GameAchievementFilterIncludeType.none;
    public string pack = GameAchievementFilterIncludeType.none;
    public string tracker = GameAchievementFilterIncludeType.none;
    public string type = GameAchievementFilterIncludeType.none;
    public string action = GameAchievementFilterIncludeType.none;
    public string appState = GameAchievementFilterIncludeType.none;
    public string appContentState = GameAchievementFilterIncludeType.none;
    public string custom = GameAchievementFilterIncludeType.none;
}

public class GameAchievementFilterBase {    
    public List<string> codes = new List<string>();
    public string codeType = GameAchievementCodeType.equal;
    public string compareType = StatEqualityTypeString.STAT_GREATER_THAN;
    public double compareValue = 1.0;
    public GameAchievementFilterIncludeKeys includeKeys = new GameAchievementFilterIncludeKeys();
}

public class GameAchievementFilterStatisticSingle : GameAchievementFilterBase {
}

public class GameAchievementFilterStatisticSet : GameAchievementFilterBase {
    
}

public class GameAchievementFilterStatisticLike : GameAchievementFilterBase {
    public string codeLike = "";
}

public class GameAchievementFilterStatisticCompare : GameAchievementFilterBase {
    public string codeCompareTo = "";
}

public class GameAchievementFilterStatisticAll : GameAchievementFilterBase {
}

public class GameAchievementFilterAchievementSet : GameAchievementFilterBase {
}

public class GameAchievementFilter {
    public string type = "";
    public string data = "";
}


public class GameAchievementData : GameDataObject {

    public virtual List<GameNetworkData> networks {
        get {
            return Get<List<GameNetworkData>>(BaseDataObjectKeys.networks);
        }
        
        set {
            Set(BaseDataObjectKeys.networks, value);
        }
    }
}

public class BaseGameAchievement : GameDataObject {
    
    public List<GameAchievementFilter> filters;

    public virtual GameAchievementData data {
        get {
            return Get<GameAchievementData>(BaseDataObjectKeys.data);
        }
        
        set {
            Set(BaseDataObjectKeys.data, value);
        }
    }

    public virtual string level {
        get {
            return Get<string>(BaseGameAchievementKeys.level);
        }
        
        set {
            Set<string>(BaseGameAchievementKeys.level, value);
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
        
        filters = new List<GameAchievementFilter>();
    }

    
    public object GetAchievementFilterData(GameAchievementFilter filter) {
        if (filter != null) {
            object obj = filter.data;
            if (obj != null) {
                return obj;
            }
        }
        return null;
    }
    
    public List<GameAchievementFilter> GetAchievementFilters(string filterType) {
        List<GameAchievementFilter> filterList = new List<GameAchievementFilter>();
        if (filters != null) {
            foreach (GameAchievementFilter o in GetAchievementFilters()) {
                
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
    
    public List<GameAchievementFilter> GetAchievementFilters() {
        if (filters != null) {
            return filters;
        }
        return null;
    }
    
    public List<T> GetAchievementFilter<T>(string filterType) {
        List<GameAchievementFilter> objs = GetAchievementFilters(filterType);
        List<T> ts = new List<T>();
        if (objs != null) {
            ts = new List<T>();
            foreach (GameAchievementFilter o in objs) {
                string jsonData = "";
                try {
                    jsonData = o.data.Replace("\\\"", "\"");
                    ts.Add(JsonMapper.ToObject<T>(jsonData));
                }
                catch (Exception e) {
                    Debug.Log("ERROR converting achievement filter: " + e + " ::: " + jsonData);
                }
            }
        }
        return ts;
    }
    
    public List<GameAchievementFilterStatisticSingle> GetFilterStatisticSingle() {
        return GetAchievementFilter<GameAchievementFilterStatisticSingle>(GameAchievementFilterType.statisticSingle);
    }
    
    public List<GameAchievementFilterStatisticSet> GetFilterStatisticSet() {
        return GetAchievementFilter<GameAchievementFilterStatisticSet>(GameAchievementFilterType.statisticSet);
    }
    
    public List<GameAchievementFilterStatisticAll> GetFilterStatisticAll() {
        return GetAchievementFilter<GameAchievementFilterStatisticAll>(GameAchievementFilterType.statisticAll);
    }
    
    public List<GameAchievementFilterStatisticLike> GetFilterStatisticLike() {
        return GetAchievementFilter<GameAchievementFilterStatisticLike>(GameAchievementFilterType.statisticLike);
    }
    
    public List<GameAchievementFilterStatisticCompare> GetFilterStatisticCompare() {
        return GetAchievementFilter<GameAchievementFilterStatisticCompare>(GameAchievementFilterType.statisticCompare);
    }
    //GameAchievementFilterStatisticCompare
    
    public List<GameAchievementFilterAchievementSet> GetFilterAchievementSet() {
        return GetAchievementFilter<GameAchievementFilterAchievementSet>(GameAchievementFilterType.achievementSet);
    }
}