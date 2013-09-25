using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class BaseAppContentChoices<T> : DataObjects<T> where T : new() {
    private static T current;
    private static volatile BaseAppContentChoices<T> instance;
    private static object syncRoot = new Object();

    private string BASE_DATA_KEY = "app-content-choice-data";

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

    public static BaseAppContentChoices<T> BaseInstance {
        get {
            if (instance == null) {
                lock (syncRoot) {
                    if (instance == null)
                        instance = new BaseAppContentChoices<T>(true);
                }
            }

            return instance;
        }
        set {
            instance = value;
        }
    }

    public BaseAppContentChoices() {
        Reset();
    }

    public BaseAppContentChoices(bool loadData) {
        Reset();
        path = "data/" + BASE_DATA_KEY + ".json";
        pathKey = BASE_DATA_KEY;
        LoadData();
    }
}

public class BaseAppContentChoice : GameDataObject {
    public List<string> appStates;
    public List<string> appContentStates;
    public Dictionary<string, List<string>> requiredAssets;

    // types: tracker, pack, data, generic

    // Attributes that are added or changed after launch should be like this to prevent
    // profile conversions.

    public BaseAppContentChoice() {
        Reset();
    }

    public override void Reset() {
        base.Reset();
        appStates = new List<string>();
        appContentStates = new List<string>();
        requiredAssets = new Dictionary<string, List<string>>();
    }
}