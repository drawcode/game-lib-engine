using System;
using System.Collections.Generic;
using System.IO;

public class BaseGameVehicleSkins<T> : DataObjects<T> where T : DataObject, new() {
    private static T current;
    private static volatile BaseGameVehicleSkins<T> instance;
    private static object syncRoot = new Object();
    private string BASE_DATA_KEY = "game-vehicle-skin-data";

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

    public static BaseGameVehicleSkins<T> BaseInstance {
        get {
            if (instance == null) {
                lock (syncRoot) {
                    if (instance == null)
                        instance = new BaseGameVehicleSkins<T>(true);
                }
            }

            return instance;
        }
        set {
            instance = value;
        }
    }

    public BaseGameVehicleSkins() {
        Reset();
    }

    public BaseGameVehicleSkins(bool loadData) {
        Reset();
        path = "data/" + BASE_DATA_KEY + ".json";
        pathKey = BASE_DATA_KEY;
        LoadData();
    }
}

public class BaseGameVehicleSkin : GameDataObject {

    // Attributes that are added or changed after launch should be like this to prevent
    // profile conversions.

    public BaseGameVehicleSkin() {
        Reset();
    }

    public override void Reset() {
        base.Reset();
    }

    public void Clone(BaseGameVehicleSkin toCopy) {
        base.Clone(toCopy);
    }

    // Attributes that are added or changed after launch should be like this to prevent
    // profile conversions.
}