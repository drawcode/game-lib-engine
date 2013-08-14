using System;
using System.Collections.Generic;
using System.IO;

public class BaseARDataSets<T> : DataObjects<T> where T : new() {
    private static T current;
    private static volatile BaseARDataSets<T> instance;
    private static object syncRoot = new Object();

    private string BASE_DATA_KEY = "ar-data-set-data";

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

    public static BaseARDataSets<T> BaseInstance {
        get {
            if (instance == null) {
                lock (syncRoot) {
                    if (instance == null)
                        instance = new BaseARDataSets<T>(true);
                }
            }

            return instance;
        }
    }

    public BaseARDataSets() {
        Reset();
    }

    public BaseARDataSets(bool loadData) {
        Reset();
        path = "data/" + BASE_DATA_KEY + ".json";
        pathKey = BASE_DATA_KEY;
        LoadData();
    }
}

public class BaseARDataSet : GameDataObject {

    // Attributes that are added or changed after launch should be like this to prevent
    // profile conversions.

    public BaseARDataSet() {
        Reset();
    }

    public override void Reset() {
        base.Reset();
    }

    // Attributes that are added or changed after launch should be like this to prevent
    // profile conversions.
}