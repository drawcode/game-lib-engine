using System;
using System.Collections.Generic;
using System.IO;
// using Engine.Data.Json;
using Engine.Utility;

public class BaseGameLeaderboards<T> : DataObjects<T> where T : DataObject, new() {
    private static T current;
    private static volatile BaseGameLeaderboards<T> instance;
    private static object syncRoot = new Object();

    public static string BASE_DATA_KEY = "game-leaderboard-data";

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

    public static BaseGameLeaderboards<T> BaseInstance {
        get {
            if (instance == null) {
                lock (syncRoot) {
                    if (instance == null)
                        instance = new BaseGameLeaderboards<T>(true);
                }
            }

            return instance;
        }
        set {
            instance = value;
        }
    }

    public BaseGameLeaderboards() {
        Reset();
    }

    public BaseGameLeaderboards(bool loadData) {
        Reset();
        path = "data/" + BASE_DATA_KEY + ".json";
        pathKey = BASE_DATA_KEY;
        LoadData();
    }
}

public class GameNetworkData : GameDataObject {
    
}

public class GameLeaderboardData : GameDataObject {

    public virtual string datatype {
        get {
            return Get<string>(BaseDataObjectKeys.datatype);
        }
        
        set {
            Set(BaseDataObjectKeys.datatype, value);
        }
    }

    public virtual string direction {
        get {
            return Get<string>(BaseDataObjectKeys.direction);
        }
        
        set {
            Set(BaseDataObjectKeys.direction, value);
        }
    }
    
    public virtual List<GameNetworkData> networks {
        get {
            return Get<List<GameNetworkData>>(BaseDataObjectKeys.networks);
        }
        
        set {
            Set(BaseDataObjectKeys.networks, value);
        }
    }
}

public class BaseGameLeaderboard : GameDataObject {

    // Attributes that are added or changed after launch should be like this to prevent
    // profile conversions.
    
    public virtual GameLeaderboardData data {
        get {
            return Get<GameLeaderboardData>(BaseDataObjectKeys.data);
        }
        
        set {
            Set(BaseDataObjectKeys.data, value);
        }
    }


    public BaseGameLeaderboard() {
        Reset();
    }

    public override void Reset() {
        base.Reset();
    }

    public void Clone(BaseGameLeaderboard toCopy) {
        base.Clone(toCopy);
    }

    // Attributes that are added or changed after launch should be like this to prevent
    // profile conversions.
}