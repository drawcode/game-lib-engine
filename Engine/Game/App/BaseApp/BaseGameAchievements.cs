using System;
using System.Collections.Generic;
using System.IO;

using UnityEngine;

// using Engine.Data.Json;
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


    /*
    public List<GameAchievement> GetListByPack(string packCode) {
        return GetAll().FindAll(
            item => 
                item.pack_code.ToLower() == packCode.ToLower()
            );
    }
    */

    public GameAchievement GetByCodeAndPack(string code, string packCode) {
        List<GameAchievement> items = GetListByPack(packCode) as List<GameAchievement>;
        if (items != null) {
            items = items.FindAll(
                item =>
                    item.code == code
            );

            if (items != null) {
                foreach (GameAchievement item in items) {
                    return item;
                }
            }
        }

        return null;
    }

    public string FormatAchievementTags(AppState app_state, AppContentState app_content_state, string textToRender) {

        if (!string.IsNullOrEmpty(textToRender)) {
            textToRender = textToRender.Replace(
                "___app_content_state___",
                app_content_state.display_name
            );
            textToRender = textToRender.Replace(
                "___app_state___",
                app_state.display_name
            );
        }

        return textToRender;
    }

    public string FormatAchievementTags(string textToRender) {

        return FormatAchievementTags(AppStates.Current, AppContentStates.Current, textToRender);
    }

    public GameAchievement GetByCodeAndPack(string code, string packCode, string app_content_state) {
        List<GameAchievement> items = GetListByPackByAppContentState(packCode, app_content_state);
        if (items != null) {
            items = items.FindAll(
                item =>
                    item.code == code
            );

            if (items != null) {
                foreach (GameAchievement item in items) {
                    return item;
                }
            }
        }

        return null;
    }

    /*
    public string FormatAchievementTags(string app_state, string app_content_state, string textToRender) {
        AppState app_stateTo = AppStates.Instance.GetById(app_state);
        AppContentState app_content_stateTo = AppContentStates.Instance.GetById(app_content_state);     
        return FormatAchievementTags(app_stateTo, app_content_stateTo, textToRender);
    }
    
    public string FormatAchievementTags(AppState app_state, AppContentState app_content_state, string textToRender) {
        if(app_state == null) 
            return textToRender;
        
        if(app_content_state == null) 
            return textToRender;
        
        if(!string.IsNullOrEmpty(textToRender)){
            textToRender = textToRender.Replace(
                "___app_content_state___", 
                app_content_state.display_name
            );
            textToRender = textToRender.Replace(
                "___app_state___", 
                app_state.display_name
            );
        }
        
        return textToRender;
    }
    */

    public List<GameAchievement> GetListByPackByAppContentState(string appPackCode, string app_content_state) {
        List<GameAchievement> achievements = new List<GameAchievement>();
        //foreach(GameAchievement achievement in GetListByPack(appPackCode)) {
        // TODO FILTER
        //foreach(GameFilter filter in achievement.filters) {
        //  string filteredAppContentState = filter.app_content_state;
        //  if(achievement.active 
        //      && (filteredAppContentState == app_content_state
        //      || filteredAppContentState == "all"
        //      || filteredAppContentState == "*"
        //      || filteredAppContentState == "default")) {
        //      achievements.Add(achievement);
        //  }
        //}
        //}
        return achievements;
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

public class BaseGameAchievement : GameDataObjectLocalized {


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