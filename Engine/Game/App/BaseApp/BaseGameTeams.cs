using System;
using System.Collections.Generic;
using System.IO;

public class BaseGameTeams<T> : DataObjects<T> where T : DataObject, new() {
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

public class GameTeamDataItem : GameDataObject {

}

public class GameTeamData : GameDataObject {


    public virtual List<GameTeamDataItem> models {
        get {
            return Get<List<GameTeamDataItem>>(BaseDataObjectKeys.models);
        }
        
        set {
            Set<List<GameTeamDataItem>>(BaseDataObjectKeys.models, value);
        }
    } 

    public virtual List<GameTeamDataItem> color_presets {
        get {
            return Get<List<GameTeamDataItem>>(BaseDataObjectKeys.color_presets);
        }
        
        set {
            Set<List<GameTeamDataItem>>(BaseDataObjectKeys.color_presets, value);
        }
    } 

    public virtual List<GameTeamDataItem> texture_presets {
        get {
            return Get<List<GameTeamDataItem>>(BaseDataObjectKeys.texture_presets);
        }
        
        set {
            Set<List<GameTeamDataItem>>(BaseDataObjectKeys.texture_presets, value);
        }
    }  
}

public class BaseGameTeam : GameDataObject {
    
    // Attributes that are added or changed after launch should be like this to prevent
    // profile conversions.

    public virtual GameTeamData data {
        get {
            return Get<GameTeamData>(BaseDataObjectKeys.data);
        }
        
        set {
            Set<GameTeamData>(BaseDataObjectKeys.data, value);
        }
    }  

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