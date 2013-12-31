using System;
using System.Collections.Generic;
using System.IO;

using UnityEngine;

public class BaseGameCharacters<T> : DataObjects<T> where T : DataObject, new() {
    private static T current;
    private static volatile BaseGameCharacters<T> instance;
    private static System.Object syncRoot = new System.Object();

    private string BASE_DATA_KEY = "game-character-data";

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

    public static BaseGameCharacters<T> BaseInstance {
        get {
            if (instance == null) {
                lock (syncRoot) {
                    if (instance == null)
                        instance = new BaseGameCharacters<T>(true);
                }
            }

            return instance;
        }
        set {
            instance = value;
        }
    }

    public BaseGameCharacters() {
        Reset();
    }

    public BaseGameCharacters(bool loadData) {
        Reset();
        path = "data/" + BASE_DATA_KEY + ".json";
        pathKey = BASE_DATA_KEY;
        LoadData();
    }
    
    public static GameObject Load(string code) {
        return AppContentAssetModels.LoadModel(code);
    }
}

public class GameDataCharacterModel : GameDataObject {
    
    public virtual string textures {
        get {
            return Get<string>(BaseDataObjectKeys.textures);
        }
        
        set {
            Set<string>(BaseDataObjectKeys.textures, value);
        }
    } 
    
    public virtual string colors {
        get {
            return Get<string>(BaseDataObjectKeys.colors);
        }
        
        set {
            Set<string>(BaseDataObjectKeys.colors, value);
        }
    } 
}

public class GameDataCharacter : GameDataObject {

    public virtual List<string> roles {
        get {
            return Get<List<string>>(BaseDataObjectKeys.roles);
        }
        
        set {
            Set<List<string>>(BaseDataObjectKeys.roles, value);
        }
    } 
    
    public virtual List<GameDataCharacterModel> models {
        get {
            return Get<List<GameDataCharacterModel>>(BaseDataObjectKeys.models);
        }
        
        set {
            Set<List<GameDataCharacterModel>>(BaseDataObjectKeys.models, value);
        }
    } 
}

/*
"character_data": {
            "roles": ["hero","enemy","sidekick"],
            "models" : [
                {
                    "type": "character",
                    "code": "character-boy-1",
                    "textures": "character-boy1-default",
                    "colors":"game-college-baylor-bears-home"
                }
            ]
        }
}
*/

public class BaseGameCharacter : GameDataObject {

    // Attributes that are added or changed after launch should be like this to prevent
    // profile conversions.

    public virtual GameDataCharacter character_data {
        get {
            return Get<GameDataCharacter>(BaseDataObjectKeys.character_data);
        }
        
        set {
            Set<GameDataCharacter>(BaseDataObjectKeys.character_data, value);
        }
    }  

    public BaseGameCharacter() {
        Reset();
    }

    public override void Reset() {
        base.Reset();
    }

    public void Clone(BaseGameCharacter toCopy) {
        base.Clone(toCopy);
    }
    
    public GameObject Load() {
        foreach(GameDataCharacterModel model in character_data.models) {
            return GameCharacters.Load(model.code);
        }
        return null;
    }

    // Attributes that are added or changed after launch should be like this to prevent
    // profile conversions.
}