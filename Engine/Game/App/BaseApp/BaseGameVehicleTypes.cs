using System;
using System.Collections.Generic;
using System.IO;

public class BaseGameVehicleTypes<T> : DataObjects<T> where T : DataObject, new() {
    private static T current;
    private static volatile BaseGameVehicleTypes<T> instance;
    private static object syncRoot = new Object();
    private string BASE_DATA_KEY = "game-vehicle-type-data";

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

    public static BaseGameVehicleTypes<T> BaseInstance {
        get {
            if (instance == null) {
                lock (syncRoot) {
                    if (instance == null)
                        instance = new BaseGameVehicleTypes<T>(true);
                }
            }

            return instance;
        }
        set {
            instance = value;
        }
    }

    public BaseGameVehicleTypes() {
        Reset();
    }

    public BaseGameVehicleTypes(bool loadData) {
        Reset();
        path = "data/" + BASE_DATA_KEY + ".json";
        pathKey = BASE_DATA_KEY;
    }

}

public class BaseGameVehicleType : GameDataObject {

    // Attributes that are added or changed after launch should be like this to prevent
    // profile conversions.

    public BaseGameVehicleType() {
        Reset();
    }

    public override void Reset() {
        base.Reset();
    }

    public void Clone(BaseGameVehicleType toCopy) {
        base.Clone(toCopy);
    }

    // Attributes that are added or changed after launch should be like this to prevent
    // profile conversions.
}