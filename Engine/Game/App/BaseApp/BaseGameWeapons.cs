using System;
using System.Collections.Generic;
using System.IO;
// using Engine.Data.Json;
using Engine.Utility;

public class BaseGameWeapons<T> : DataObjects<T> where T : DataObject, new() {
    private static T current;
    private static volatile BaseGameWeapons<T> instance;
    private static object syncRoot = new Object();
    public static string BASE_DATA_KEY = "game-weapon-data";

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

    public static BaseGameWeapons<T> BaseInstance {
        get {
            if (instance == null) {
                lock (syncRoot) {
                    if (instance == null)
                        instance = new BaseGameWeapons<T>(true);
                }
            }

            return instance;
        }
    }

    public BaseGameWeapons() {
        Reset();
    }

    public BaseGameWeapons(bool loadData) {
        Reset();
        path = "data/" + BASE_DATA_KEY + ".json";
        pathKey = BASE_DATA_KEY;
        LoadData();
    }
}

public class BaseGameWeapon : GameDataObject {

    // Attributes that are added or changed after launch should be like this to prevent
    // profile conversions.

    public virtual GameDataObjectItem data {
        get {
            return Get<GameDataObjectItem>(BaseDataObjectKeys.data);
        }
        
        set {
            Set<GameDataObjectItem>(BaseDataObjectKeys.data, value);
        }
    }

    public BaseGameWeapon() {
        Reset();
    }

    public override void Reset() {
        base.Reset();
    }

    public void Clone(BaseGameWeapon toCopy) {
        base.Clone(toCopy);
    }

    // Attributes that are added or changed after launch should be like this to prevent
    // profile conversions.
}