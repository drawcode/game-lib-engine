using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Engine.Game.App;
using UnityEngine;


public class GameObjectTimerKeys {
    public static string gameUpdateAll = "game-update-all";
    public static string gameUpdateAlways = "game-update-always";
    public static string gameUpdatePhysics = "game-update-physics";
    
    public static string gameLateUpdateAll = "game-late-update-all";
    public static string gameLateUpdateAlways = "game-late-update-always";
    public static string gameLateUpdatePhysics = "game-late-update-physics";
    
    public static string gameFixedUpdateAll = "game-fixed-update-all";
    public static string gameFixedUpdateAlways = "game-fixed-update-always";
    public static string gameFixedUpdatePhysics = "game-fixed-update-physics";
}

public class GameObjectTimerData : GameDataObject {

    // key
    // delta
    // last_time

}

public class GameObjectTimer {

    public Dictionary<string, GameObjectTimerData> timers;
    public Dictionary<string, float> intervals;

    private static volatile GameObjectTimer instance;
    private static System.Object syncRoot = new System.Object();
        
    public static GameObjectTimer Instance {
        get {
            if (instance == null) {
                lock (syncRoot) {
                    if (instance == null)
                        instance = new GameObjectTimer();
                }
            }
            
            return instance;
        }
        set {
            instance = value;
        }
    }

    public GameObjectTimer() {
        Reset();
    }

    public void Reset() {
        InitTimers(true);
        InitIntervals(true);
    }

    public void InitIntervals(bool reset = false) {
        if(intervals == null || reset) {            
            intervals = new Dictionary<string, float>();
            
            SetInterval(GameObjectTimerKeys.gameUpdateAll, defaultInterval);
            SetInterval(GameObjectTimerKeys.gameUpdateAlways, defaultInterval);
            SetInterval(GameObjectTimerKeys.gameUpdatePhysics, defaultInterval);

            SetInterval(GameObjectTimerKeys.gameLateUpdateAll, defaultInterval);
            SetInterval(GameObjectTimerKeys.gameLateUpdateAlways, defaultInterval);
            SetInterval(GameObjectTimerKeys.gameLateUpdatePhysics, defaultInterval);

            SetInterval(GameObjectTimerKeys.gameFixedUpdateAll, defaultInterval);
            SetInterval(GameObjectTimerKeys.gameFixedUpdateAlways, defaultInterval);
            SetInterval(GameObjectTimerKeys.gameFixedUpdatePhysics, defaultInterval);
        }

    }

    public void InitTimers(bool reset = false) {
        if(timers == null || reset) {            
            timers = new Dictionary<string, GameObjectTimerData>();
        }
    }

    public void SetInterval(string key, float increment) {
        InitIntervals();

        intervals.Set(key, increment);
    }

    public float GetInterval(string key) {
        InitIntervals();

        if(intervals.ContainsKey(key)) {
            return intervals[key];
        }

        return defaultInterval;
    }

    public GameObjectTimerData SetTimer(string key, float delta = 0.033f, float modifier = 1f) {

        InitTimers();

        GameObjectTimerData obj = GetTimer(key, delta, modifier);
        
        obj.key = key;
        obj.delta = delta;
        obj.last_time = Time.time;
        obj.modifier = modifier;
        
        timers.Set<string, GameObjectTimerData>(key, obj);

        return obj;
    }
    
    public GameObjectTimerData GetTimer(string key, float delta = 0.033f, float modifier = 1f) {

        InitTimers();

        GameObjectTimerData obj = null;

        if(timers.Has(key)) {
            obj = timers.Get(key);    
        }
        
        if(obj == null) {
            obj = new GameObjectTimerData();            
            obj.key = key;
            obj.delta = delta;
            obj.last_time = Time.time;
            obj.modifier = modifier;

            timers.Set<string, GameObjectTimerData>(key, obj);
        }

        return obj;
    }

    public bool IsTimer(string key, float delta = .033f, float modifier = 1f) {

        InitTimers();

        GameObjectTimerData obj = GetTimer(key, delta, modifier);

        if((obj.last_time + (delta * modifier)) < Time.time) {
            obj.last_time = Time.time;
            timers.Set<GameObjectTimerData>(key, obj);
            //Debug.Log("GameObjectTimer:" + key + " last_time:" + obj.last_time);
            return true;
        }
        
        return false;
    }

    public bool IsTimerPerfRelative(string key, float delta = .033f) {
        return IsTimer(key, delta, currentModifier);
    }

    public bool IsTimerPerfRelativeHalved(string key, float delta = .033f) {
        return IsTimer(key, delta, currentModifier * 2);
    }

    public bool IsTimerPerf(string key, float modifier = 1f) {
        return IsTimerPerfRelative(
            key, 
            GetInterval(key) * modifier);
    }

    public bool IsFPSLessThan(float val) {
#if USE_GAME_LIB_GAMES
        return FPSDisplay.IsFPSLessThan(val);
#else
        return false;
#endif
    }

    public float GetFPSOffset(float desiredFPS = 30f) {
        return desiredFPS / currentFPS;
    }

    public float currentFPS {
        get {
#if USE_GAME_LIB_GAMES
            return FPSDisplay.GetCurrentFPS();
#else
            return 30;
#endif
        }
    }

    public float currentModifier {
        get {
            return GetFPSOffset();
        }
    }
   
    public float defaultInterval {
        get {
            return 1f / 30f;
        }
    }

    public float defaultIntervalDouble {
        get {
            return 2f / 30f;
        }
    }
        
    public float defaultIntervalQuarter {
        get {
            return 4f / 30f;
        }
    }

}

