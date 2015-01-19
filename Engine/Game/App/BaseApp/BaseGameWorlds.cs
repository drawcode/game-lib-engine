using System;
using System.Collections.Generic;
using System.IO;
using Engine.Data.Json;
using Engine.Utility;

public class BaseGameWorlds<T> : DataObjects<T> where T : DataObject, new() {
    private static T current;
    private static volatile BaseGameWorlds<T> instance;
    private static object syncRoot = new Object();

    public static string BASE_DATA_KEY = "game-world-data";

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

    public static BaseGameWorlds<T> BaseInstance {
        get {
            if (instance == null) {
                lock (syncRoot) {
                    if (instance == null)
                        instance = new BaseGameWorlds<T>(true);
                }
            }

            return instance;
        }
    }

    public BaseGameWorlds() {
        Reset();
    }

    public BaseGameWorlds(bool loadData) {
        Reset();
        path = "data/" + BASE_DATA_KEY + ".json";
        pathKey = BASE_DATA_KEY;
        LoadData();
    }
    
    public void ChangeCurrentAbsolute(string code) {
        GameWorlds.Current.code = "changeme";
        ChangeCurrent(code);
    }
    
    public void ChangeCurrent(string code) {
        if (GameWorlds.Current.code != code) {
            GameWorlds.Current = GameWorlds.Instance.GetById(code);
            //string originalCode = code;
            if (string.IsNullOrEmpty(GameWorlds.Current.code)) {
                GameWorlds.Current = GameWorlds.Instance.GetById(code);
            }   
        }
    }     
}

public class BaseGameWorld : GameDataObject {

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

    public BaseGameWorld() {
        Reset();
    }

    public override void Reset() {
        base.Reset();
    }

    public void Clone(BaseGameWorld toCopy) {
        base.Clone(toCopy);
    }

    // Attributes that are added or changed after launch should be like this to prevent
    // profile conversions.
}