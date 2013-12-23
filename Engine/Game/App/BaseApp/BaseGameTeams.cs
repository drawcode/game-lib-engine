using System;
using System.Collections.Generic;
using System.IO;

public class BaseGameTeams<T> : DataObjects<T> where T : new() {
    private static T current;
    private static volatile BaseGameTeams<T> instance;
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
    
    public static BaseGameTeams<T> BaseInstance {
        get {
            if (instance == null) {
                lock (syncRoot) {
                    if (instance == null)
                        instance = new BaseGameTeams<T>(true);
                }
            }
            
            return instance;
        }
        set {
            instance = value;
        }
    }
    
    public BaseGameTeams() {
        Reset();
    }
    
    public BaseGameTeams(bool loadData) {
        Reset();
        path = "data/" + BASE_DATA_KEY + ".json";
        pathKey = BASE_DATA_KEY;
        LoadData();
    }
}

public class BaseGameTeam : GameDataObject {
    
    // Attributes that are added or changed after launch should be like this to prevent
    // profile conversions.
    
    public BaseGameTeam() {
        Reset();
    }
    
    public override void Reset() {
        base.Reset();
    }
    
    public void Clone(BaseGameTeam toCopy) {
        base.Clone(toCopy);
    }
    
    // Attributes that are added or changed after launch should be like this to prevent
    // profile conversions.
}