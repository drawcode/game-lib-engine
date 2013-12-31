using System;
using System.Collections.Generic;
using System.IO;

public class BaseGameCharacterMaterials<T> : DataObjects<T> where T : DataObject, new() {
    private static T current;
    private static volatile BaseGameCharacterMaterials<T> instance;
    private static object syncRoot = new Object();

    private string BASE_DATA_KEY = "game-character-material-data";

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

    public static BaseGameCharacterMaterials<T> BaseInstance {
        get {
            if (instance == null) {
                lock (syncRoot) {
                    if (instance == null)
                        instance = new BaseGameCharacterMaterials<T>(true);
                }
            }

            return instance;
        }
        set {
            instance = value;
        }
    }

    public BaseGameCharacterMaterials() {
        Reset();
    }

    public BaseGameCharacterMaterials(bool loadData) {
        Reset();
        path = "data/" + BASE_DATA_KEY + ".json";
        pathKey = BASE_DATA_KEY;
        LoadData();
    }
}

public class BaseGameCharacterMaterial : GameDataObject {

    // Attributes that are added or changed after launch should be like this to prevent
    // profile conversions.

    public BaseGameCharacterMaterial() {
        Reset();
    }

    public override void Reset() {
        base.Reset();
    }

    public void Clone(BaseGameCharacterMaterial toCopy) {
        base.Clone(toCopy);
    }

    // Attributes that are added or changed after launch should be like this to prevent
    // profile conversions.
}