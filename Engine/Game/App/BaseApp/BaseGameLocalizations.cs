using System;
using System.Collections.Generic;
using System.IO;
using Engine.Data.Json;
using Engine.Utility;
using UnityEngine;

public class BaseGameLocalizations<T> : DataObjects<T> where T : new() {
    private static T current;
    private static volatile BaseGameLocalizations<T> instance;
    private static System.Object syncRoot = new System.Object();

    public static string currentLocale = "en"; //

    public string BASE_DATA_KEY = "game-localization-data";

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

    public static BaseGameLocalizations<T> BaseInstance {
        get {
            if (instance == null) {
                lock (syncRoot) {
                    if (instance == null)
                        instance = new BaseGameLocalizations<T>(true);
                }
            }

            return instance;
        }
        set {
            instance = value;
        }
    }

    public BaseGameLocalizations() {
        Reset();
    }

    public BaseGameLocalizations(bool loadData) {
        Reset();
        path = "data/" + BASE_DATA_KEY + ".json";
        pathKey = BASE_DATA_KEY;
        LoadData();
    }
}

public class BaseGameLocalization : DataObject {
    public string name = "default";
    public string code = "default";

    public BaseGameLocalization() {
        Reset();
    }

    public override void Reset() {
        name = "default";
        code = "default";
    }

    public void Clone(BaseGameLocalization toCopy) {
        name = toCopy.name;
        code = toCopy.code;
    }
}