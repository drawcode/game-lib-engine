using System;
using System.Collections.Generic;
using System.IO;

public class BaseGameRPGTypes<T> : DataObjects<T> where T : DataObject, new() {
    private static T current;
    private static volatile BaseGameRPGTypes<T> instance;
    private static object syncRoot = new Object();

    public static string BASE_DATA_KEY = "game-rpg-types-data";

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

    public static BaseGameRPGTypes<T> BaseInstance {
        get {
            if (instance == null) {
                lock (syncRoot) {
                    if (instance == null)
                        instance = new BaseGameRPGTypes<T>(true);
                }
            }

            return instance;
        }
        set {
            instance = value;
        }
    }

    public BaseGameRPGTypes() {
        Reset();
    }

    public BaseGameRPGTypes(bool loadData) {
        Reset();
        path = "data/" + BASE_DATA_KEY + ".json";
        pathKey = BASE_DATA_KEY;
        LoadData();
    }
}

public class BaseGameRPGType : GameDataObject {

    // Attributes that are added or changed after launch should be like this to prevent
    // profile conversions.

    public BaseGameRPGType() {
        Reset();
    }

    public override void Reset() {
        base.Reset();
    }
}