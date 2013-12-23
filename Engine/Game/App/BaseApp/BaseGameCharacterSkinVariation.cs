using System;
using System.Collections.Generic;
using System.IO;

public class BaseGameCharacterSkinVariations<T> : DataObjects<T> where T : new() {
    private static T current;
    private static volatile BaseGameCharacterSkinVariations<T> instance;
    private static object syncRoot = new Object();

    private string BASE_DATA_KEY = "game-character-skin-variation-data";

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

    public static BaseGameCharacterSkinVariations<T> BaseInstance {
        get {
            if (instance == null) {
                lock (syncRoot) {
                    if (instance == null)
                        instance = new BaseGameCharacterSkinVariations<T>(true);
                }
            }

            return instance;
        }
        set {
            instance = value;
        }
    }

    public BaseGameCharacterSkinVariations() {
        Reset();
    }

    public BaseGameCharacterSkinVariations(bool loadData) {
        Reset();
        path = "data/" + BASE_DATA_KEY + ".json";
        pathKey = BASE_DATA_KEY;
        LoadData();
    }
}

public class BaseGameCharacterSkinVariation : GameDataObject {

    // Attributes that are added or changed after launch should be like this to prevent
    // profile conversions.

    public BaseGameCharacterSkinVariation() {
        Reset();
    }

    public override void Reset() {
        base.Reset();
    }

    public void Clone(BaseGameCharacterSkinVariation toCopy) {
        base.Clone(toCopy);
    }

    // Attributes that are added or changed after launch should be like this to prevent
    // profile conversions.
}