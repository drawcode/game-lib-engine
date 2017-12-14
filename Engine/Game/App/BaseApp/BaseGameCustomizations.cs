using System;
using System.Collections.Generic;
using System.IO;
// using Engine.Data.Json;
using Engine.Utility;

public class BaseGameCustomizations<T> : DataObjects<T> where T : DataObject, new() {
    private static T current;
    private static volatile BaseGameCustomizations<T> instance;
    private static object syncRoot = new Object();

    public static string BASE_DATA_KEY = "game-customization-data";

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

    public static BaseGameCustomizations<T> BaseInstance {
        get {
            if (instance == null) {
                lock (syncRoot) {
                    if (instance == null)
                        instance = new BaseGameCustomizations<T>(true);
                }
            }

            return instance;
        }
        set {
            instance = value;
        }
    }

    public BaseGameCustomizations() {
        Reset();
    }

    public BaseGameCustomizations(bool loadData) {
        Reset();
        path = "data/" + BASE_DATA_KEY + ".json";
        pathKey = BASE_DATA_KEY;
        LoadData();
    }
}

public class BaseGameCustomization : DataObject {
    public List<CustomPlayerColors> colors;

    public BaseGameCustomization() {
        Reset();
    }

    public override void Reset() {
        base.Reset();
        colors = new List<CustomPlayerColors>();
    }

    public void Clone(BaseGameCustomization toCopy) {
        colors = toCopy.colors;
    }
}