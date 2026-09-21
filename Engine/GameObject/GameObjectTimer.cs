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

    // last_time is rewritten on every timer fire -- on every actor, every frame. The base
    // GameDataObject property keeps it as a double in the string-keyed attribute dictionary,
    // so each write boxes the value (measured: 1 alloc / 24 B, on roughly 60% of calls).
    // GameObjectTimerData only ever lives in GameObjectTimer.timers; nothing serialises it
    // and nothing enumerates its attributes, so a plain backing field is safe here. The
    // public property is unchanged for any external caller, it just no longer touches the
    // dictionary.
    private double lastTimeValue = 0;

    public override double last_time {
        get {
            return lastTimeValue;
        }

        set {
            lastTimeValue = value;
        }
    }

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
        if (intervals == null || reset) {
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
        if (timers == null || reset) {
            timers = new Dictionary<string, GameObjectTimerData>();
        }
    }

    public void SetInterval(string key, float increment) {
        InitIntervals();

        intervals.Set(key, increment);
    }

    public float GetInterval(string key) {
        InitIntervals();

        float interval;

        // One hashed lookup instead of ContainsKey plus an indexer -- IsTimerPerf calls
        // this on every gate.
        if (intervals.TryGetValue(key, out interval)) {
            return interval;
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

        GameObjectTimerData obj;

        // One hashed lookup instead of Has plus Get (two ContainsKey calls and an indexer).
        // A missing key left obj null before and still does, so the create path below is
        // reached in exactly the same cases.
        if (!timers.TryGetValue(key, out obj)
            || obj == null) {

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

        if ((obj.last_time + (delta * modifier)) < Time.time) {
            obj.last_time = Time.time;
            // No Set here: GetTimer has already stored obj under this key and handed back
            // that same reference, so re-storing it was two more hashed lookups writing an
            // entry that is already correct.
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

    // FRAME-RATE INDEPENDENT THROTTLING (2026-09-20).
    //
    // This used to be GetFPSOffset() alone, i.e. desiredFPS / currentFPS, which SHRINKS the
    // interval as the framerate rises: at 120fps the gate's interval came out at ~8ms, so every
    // IsTimerPerf gate in the game passed on every frame and actors ticked 120 times a second
    // instead of the 30 the interval names. The cadence therefore scaled with the framerate --
    // the opposite of the intent, and it made a device's update rate depend on its hardware.
    //
    // Clamped at 1 the interval is a floor in REAL SECONDS: 1/30s means 30 ticks a second at any
    // framerate, and the offset still stretches it when the framerate falls below the target, so
    // the load-shedding behaviour that motivated the original formula is kept.
    //
    // Set frameRateIndependent = false to restore the old scaling for a product that relied on it.
    public static bool frameRateIndependent = true;

    public float currentModifier {
        get {

            float offset = GetFPSOffset();

            if (!frameRateIndependent) {
                return offset;
            }

            return offset < 1f ? 1f : offset;
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

