using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class BaseGameConfigs<T> : DataObjects<T> where T : new() {
    private static T current;
    private static volatile BaseGameConfigs<T> instance;
    private static object syncRoot = new Object();

	public static string BASE_DATA_KEY = "game-config-data";

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

    // TODO: Common config actions, lookup, count, etc
}

public class BaseGameConfig : Config {

    public BaseGameConfig() {
        Reset();
    }

    public override void Reset() {
        base.Reset();
    }
}