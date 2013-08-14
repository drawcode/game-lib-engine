using System;
using System.Collections.Generic;
using System.IO;

public class BaseARDataSetTrackers<T> : DataObjects<T> where T : new() {
    private static T current;
    private static volatile BaseARDataSetTrackers<T> instance;
    private static object syncRoot = new Object();

    private string BASE_DATA_KEY = "ar-data-tracker-data";

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

    public static BaseARDataSetTrackers<T> BaseInstance {
        get {
            if (instance == null) {
                lock (syncRoot) {
                    if (instance == null)
                        instance = new BaseARDataSetTrackers<T>(true);
                }
            }

            return instance;
        }
    }

    public BaseARDataSetTrackers() {
        Reset();
    }

    public BaseARDataSetTrackers(bool loadData) {
        Reset();
        path = "data/" + BASE_DATA_KEY + ".json";
        pathKey = BASE_DATA_KEY;
        LoadData();
    }
}

public class BaseARDataSetTracker : GameDataObject {

    // Attributes that are added or changed after launch should be like this to prevent
    // profile conversions.

    public BaseARDataSetTracker() {
        Reset();
    }

    public override void Reset() {
        base.Reset();
    }

    // Attributes that are added or changed after launch should be like this to prevent
    // profile conversions.
}