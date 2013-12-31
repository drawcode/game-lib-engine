using System;
using System.Collections.Generic;
using System.IO;

public class BaseGameCharacterSkins<T> : DataObjects<T> where T : DataObject, new() {
    private static T current;
    private static volatile BaseGameCharacterSkins<T> instance;
    private static object syncRoot = new Object();

    private string BASE_DATA_KEY = "game-character-skin-data";

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

    public static BaseGameCharacterSkins<T> BaseInstance {
        get {
            if (instance == null) {
                lock (syncRoot) {
                    if (instance == null)
                        instance = new BaseGameCharacterSkins<T>(true);
                }
            }

            return instance;
        }
        set {
            instance = value;
        }
    }

    public BaseGameCharacterSkins() {
        Reset();
    }

    public BaseGameCharacterSkins(bool loadData) {
        Reset();
        path = "data/" + BASE_DATA_KEY + ".json";
        pathKey = BASE_DATA_KEY;
        LoadData();
    }
}

public class BaseGameCharacterSkin : GameDataObject {

    // Attributes that are added or changed after launch should be like this to prevent
    // profile conversions.

    public BaseGameCharacterSkin() {
        Reset();
    }

    public override void Reset() {
        base.Reset();
    }

    public void Clone(BaseGameCharacterSkin toCopy) {
        base.Clone(toCopy);
    }

    // Attributes that are added or changed after launch should be like this to prevent
    // profile conversions.
}