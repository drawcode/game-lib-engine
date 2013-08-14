using System;
using System.Collections.Generic;
using System.IO;

public class BaseAppContentStates<T> : DataObjects<T> where T : new() {
    private static T current;
    private static volatile BaseAppContentStates<T> instance;
    private static object syncRoot = new Object();

    private string BASE_DATA_KEY = "app-content-state-data";

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

    public static BaseAppContentStates<T> BaseInstance {
        get {
            if (instance == null) {
                lock (syncRoot) {
                    if (instance == null)
                        instance = new BaseAppContentStates<T>(true);
                }
            }

            return instance;
        }
        set {
            instance = value;
        }
    }

    public BaseAppContentStates() {
        Reset();
    }

    public BaseAppContentStates(bool loadData) {
        Reset();
        path = "data/" + BASE_DATA_KEY + ".json";
        pathKey = BASE_DATA_KEY;
        LoadData();
    }
}

public class BaseAppContentState : GameDataObject {
    public List<string> appStates;
    public List<string> requiredPacks;
    public List<string> requiredTrackers;

    // type

    // Attributes that are added or changed after launch should be like this to prevent
    // profile conversions.

    public BaseAppContentState() {
        Reset();
    }

    public override void Reset() {
        base.Reset();
        appStates = new List<string>();
        requiredPacks = new List<string>();
        requiredTrackers = new List<string>();
    }
}