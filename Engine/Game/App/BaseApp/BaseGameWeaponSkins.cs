using System;
using System.Collections.Generic;
using System.IO;

public class BaseGameWeaponSkins<T> : DataObjects<T> where T : DataObject, new() {
    private static T current;
    private static volatile BaseGameWeaponSkins<T> instance;
    private static object syncRoot = new Object();
    private string BASE_DATA_KEY = "game-weapon-skin-data";

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

    public static BaseGameWeaponSkins<T> BaseInstance {
        get {
            if (instance == null) {
                lock (syncRoot) {
                    if (instance == null)
                        instance = new BaseGameWeaponSkins<T>(true);
                }
            }

            return instance;
        }
        set {
            instance = value;
        }
    }

    public BaseGameWeaponSkins() {
        Reset();
    }

    public BaseGameWeaponSkins(bool loadData) {
        Reset();
        path = "data/" + BASE_DATA_KEY + ".json";
        pathKey = BASE_DATA_KEY;
        LoadData();
    }
}

public class BaseGameWeaponSkin : GameDataObject {

    // Attributes that are added or changed after launch should be like this to prevent
    // profile conversions.

    public BaseGameWeaponSkin() {
        Reset();
    }

    public override void Reset() {
        base.Reset();
    }

    public void Clone(BaseGameWeaponSkin toCopy) {
        base.Clone(toCopy);
    }

    // Attributes that are added or changed after launch should be like this to prevent
    // profile conversions.
}