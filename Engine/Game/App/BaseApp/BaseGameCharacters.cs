using System;
using System.Collections.Generic;
using System.IO;

public class BaseGameCharacters<T> : DataObjects<T> where T : new() {
    private static T current;
    private static volatile BaseGameCharacters<T> instance;
    private static object syncRoot = new Object();

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
}


public class GameCharacterModel : GameDataObject {

}

public class GameCharacterPreset : GameDataObject {

    public string textures { get; set; }
    public string colors { get; set; }
}

public class GameCharacterData : GameDataObject {

    public string character_role { get; set; }
    public List<GameCharacterModel> models { get; set; }
    public List<GameCharacterPreset> presets { get; set; }
}

public class BaseGameCharacter : GameDataObject {

    // Attributes that are added or changed after launch should be like this to prevent
    // profile conversions.

    public virtual GameCharacterData character_data {
        get {
            return Get<GameCharacterData>(BaseDataObjectKeys.character_data);
        }
        
        set {
            Set<GameCharacterData>(BaseDataObjectKeys.character_data, value);
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

    // Attributes that are added or changed after launch should be like this to prevent
    // profile conversions.
}