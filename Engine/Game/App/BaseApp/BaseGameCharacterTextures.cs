using System;
using System.Collections.Generic;
using System.IO;

public class BaseGameCharacterTextures<T> : DataObjects<T> where T : new() {
    private static T current;
    private static volatile BaseGameCharacterTextures<T> instance;
    private static object syncRoot = new Object();

    private string BASE_DATA_KEY = "game-character-texture-data";

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

    public static BaseGameCharacterTextures<T> BaseInstance {
        get {
            if (instance == null) {
                lock (syncRoot) {
                    if (instance == null)
                        instance = new BaseGameCharacterTextures<T>(true);
                }
            }

            return instance;
        }
        set {
            instance = value;
        }
    }

    public BaseGameCharacterTextures() {
        Reset();
    }

    public BaseGameCharacterTextures(bool loadData) {
        Reset();
        path = "data/" + BASE_DATA_KEY + ".json";
        pathKey = BASE_DATA_KEY;
        LoadData();
    }
}

public class BaseGameCharacterTexture : GameDataObject {

    // Attributes that are added or changed after launch should be like this to prevent
    // profile conversions.

    public BaseGameCharacterTexture() {
        Reset();
    }

    public override void Reset() {
        base.Reset();
    }

    public void Clone(BaseGameCharacterTexture toCopy) {
        base.Clone(toCopy);
    }

    // Attributes that are added or changed after launch should be like this to prevent
    // profile conversions.
}