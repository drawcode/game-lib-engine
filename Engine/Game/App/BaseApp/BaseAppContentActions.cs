using System;
using System.Collections.Generic;
using System.IO;

public class BaseAppContentActions<T> : DataObjects<T> where T : new() {
    private static T current;
    private static volatile BaseAppContentActions<T> instance;
    private static object syncRoot = new Object();

    private string BASE_DATA_KEY = "app-content-action-data";

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

    public static BaseAppContentActions<T> BaseInstance {
        get {
            if (instance == null) {
                lock (syncRoot) {
                    if (instance == null)
                        instance = new BaseAppContentActions<T>(true);
                }
            }

            return instance;
        }
        set {
            instance = value;
        }
    }

    public BaseAppContentActions() {
        Reset();
    }

    public BaseAppContentActions(bool loadData) {
        Reset();
        path = "data/" + BASE_DATA_KEY + ".json";
        pathKey = BASE_DATA_KEY;
        LoadData();
    }
}

public class BaseAppContentAction : GameDataObject {

    // type

    // Attributes that are added or changed after launch should be like this to prevent
    // profile conversions.

    public BaseAppContentAction() {
        Reset();
    }

    public override void Reset() {
        base.Reset();
    }
}