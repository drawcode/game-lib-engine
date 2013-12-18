using System;
using System.Collections.Generic;
using System.IO;

public class BaseGameAssetTextures<T> : DataObjects<T> where T : new() {
    private static T current;
    private static volatile BaseGameAssetTextures<T> instance;
    private static object syncRoot = new Object();

    private string BASE_DATA_KEY = "game-asset-texture-data";

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

    public static BaseGameAssetTextures<T> BaseInstance {
        get {
            if (instance == null) {
                lock (syncRoot) {
                    if (instance == null)
                        instance = new BaseGameAssetTextures<T>(true);
                }
            }

            return instance;
        }
        set {
            instance = value;
        }
    }

    public BaseGameAssetTextures() {
        Reset();
    }

    public BaseGameAssetTextures(bool loadData) {
        Reset();
        path = "data/" + BASE_DATA_KEY + ".json";
        pathKey = BASE_DATA_KEY;
        LoadData();
    }
}

public class BaseGameAssetTexture : GameDataObject {

    // Attributes that are added or changed after launch should be like this to prevent
    // profile conversions.

    public BaseGameAssetTexture() {
        Reset();
    }

    public override void Reset() {
        base.Reset();
    }

    public void Clone(BaseGameAssetTexture toCopy) {
        base.Clone(toCopy);
    }

    // Attributes that are added or changed after launch should be like this to prevent
    // profile conversions.
}