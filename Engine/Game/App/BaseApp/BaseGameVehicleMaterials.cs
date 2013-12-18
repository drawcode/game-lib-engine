using System;
using System.Collections.Generic;
using System.IO;

public class BaseGameVehicleMaterials<T> : DataObjects<T> where T : new() {
    private static T current;
    private static volatile BaseGameVehicleMaterials<T> instance;
    private static object syncRoot = new Object();

    private string BASE_DATA_KEY = "game-vehicle-material-data";

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

    public static BaseGameVehicleMaterials<T> BaseInstance {
        get {
            if (instance == null) {
                lock (syncRoot) {
                    if (instance == null)
                        instance = new BaseGameVehicleMaterials<T>(true);
                }
            }

            return instance;
        }
        set {
            instance = value;
        }
    }

    public BaseGameVehicleMaterials() {
        Reset();
    }

    public BaseGameVehicleMaterials(bool loadData) {
        Reset();
        path = "data/" + BASE_DATA_KEY + ".json";
        pathKey = BASE_DATA_KEY;
        LoadData();
    }
}

public class BaseGameVehicleMaterial : GameDataObject {

    // Attributes that are added or changed after launch should be like this to prevent
    // profile conversions.

    public BaseGameVehicleMaterial() {
        Reset();
    }

    public override void Reset() {
        base.Reset();
    }

    public void Clone(BaseGameVehicleMaterial toCopy) {
        base.Clone(toCopy);
    }

    // Attributes that are added or changed after launch should be like this to prevent
    // profile conversions.
}