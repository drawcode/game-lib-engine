using System;
using System.Collections.Generic;
using System.IO;

public class BaseGameCharacterTypes<T> : DataObjects<T> where T : new() {
    private static T current;
    private static volatile BaseGameCharacterTypes<T> instance;
    private static object syncRoot = new Object();

    private string BASE_DATA_KEY = "game-character-type-data";

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

    public static BaseGameCharacterTypes<T> BaseInstance {
        get {
            if (instance == null) {
                lock (syncRoot) {
                    if (instance == null)
                        instance = new BaseGameCharacterTypes<T>(true);
                }
            }

            return instance;
        }
        set {
            instance = value;
        }
    }

    public BaseGameCharacterTypes() {
        Reset();
    }

    public BaseGameCharacterTypes(bool loadData) {
        Reset();
        path = "data/" + BASE_DATA_KEY + ".json";
        pathKey = BASE_DATA_KEY;
	}

}

public class BaseGameCharacterType : GameDataObject {

    // Attributes that are added or changed after launch should be like this to prevent
    // profile conversions.

    public BaseGameCharacterType() {
        Reset();
    }

    public override void Reset() {
        base.Reset();
    }

    public void Clone(BaseGameCharacterType toCopy) {
        base.Clone(toCopy);
    }

    // Attributes that are added or changed after launch should be like this to prevent
    // profile conversions.
}