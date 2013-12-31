using System;
using System.Collections.Generic;
using System.IO;
using Engine.Data.Json;
using Engine.Utility;

public class BaseGameVehicles<T> : DataObjects<T> where T : DataObject, new() {
    private static T current;
    private static volatile BaseGameVehicles<T> instance;
    private static object syncRoot = new Object();

    public static string BASE_DATA_KEY = "game-vehicle-data";

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

    public static BaseGameVehicles<T> BaseInstance {
        get {
            if (instance == null) {
                lock (syncRoot) {
                    if (instance == null)
                        instance = new BaseGameVehicles<T>(true);
                }
            }

            return instance;
        }
    }

    public BaseGameVehicles() {
        Reset();
    }

    public BaseGameVehicles(bool loadData) {
        Reset();
        path = "data/" + BASE_DATA_KEY + ".json";
        pathKey = BASE_DATA_KEY;
        LoadData();
    }
}

public class BaseGameVehicle : GameDataObject {

    // Attributes that are added or changed after launch should be like this to prevent
    // profile conversions.

    public BaseGameVehicle() {
        Reset();
    }

    public override void Reset() {
        base.Reset();
    }

    public void Clone(BaseGameVehicle toCopy) {
        base.Clone(toCopy);
    }

    // Attributes that are added or changed after launch should be like this to prevent
    // profile conversions.
}