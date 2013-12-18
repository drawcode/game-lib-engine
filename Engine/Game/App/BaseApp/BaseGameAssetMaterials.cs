using System;
using System.Collections.Generic;
using System.IO;

public class BaseGameAssetMaterials<T> : DataObjects<T> where T : new() {
    private static T current;
    private static volatile BaseGameAssetMaterials<T> instance;
    private static object syncRoot = new Object();

    private string BASE_DATA_KEY = "game-asset-material-data";

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

    public static BaseGameAssetMaterials<T> BaseInstance {
        get {
            if (instance == null) {
                lock (syncRoot) {
                    if (instance == null)
                        instance = new BaseGameAssetMaterials<T>(true);
                }
            }

            return instance;
        }
        set {
            instance = value;
        }
    }

    public BaseGameAssetMaterials() {
        Reset();
    }

    public BaseGameAssetMaterials(bool loadData) {
        Reset();
        path = "data/" + BASE_DATA_KEY + ".json";
        pathKey = BASE_DATA_KEY;
        LoadData();
    }
}

public class BaseGameAssetMaterial : GameDataObject {

    // Attributes that are added or changed after launch should be like this to prevent
    // profile conversions.

    public BaseGameAssetMaterial() {
        Reset();
    }

    public override void Reset() {
        base.Reset();
    }

    public void Clone(BaseGameAssetMaterial toCopy) {
        base.Clone(toCopy);
    }

    // Attributes that are added or changed after launch should be like this to prevent
    // profile conversions.
}