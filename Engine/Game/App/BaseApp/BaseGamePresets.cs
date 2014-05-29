using System;
using System.Collections.Generic;
using System.IO;

public class GamePresetTypes {
    public static string item = "item";
    public static string character = "character";
    public static string terrain = "terrain";
}

public class GamePresetTypeDefault {
    public static string itemDefault = "item-default";
    public static string characterDefault = "character-default";
    public static string terrainDefault = "terrain-default";
}
//GamePresets.Instance.GetByCode("game-item-default");
public class BaseGamePresets<T> : DataObjects<T> where T : DataObject, new() {
    private static T current;
    private static volatile BaseGamePresets<T> instance;
    private static object syncRoot = new Object();

    private string BASE_DATA_KEY = "game-preset-data";
    
    public Dictionary<string,string> currentPresets;

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

    public static BaseGamePresets<T> BaseInstance {
        get {
            if (instance == null) {
                lock (syncRoot) {
                    if (instance == null)
                        instance = new BaseGamePresets<T>(true);
                }
            }

            return instance;
        }
        set {
            instance = value;
        }
    }

    public BaseGamePresets() {
        Reset();
    }

    public BaseGamePresets(bool loadData) {
        Reset();
        path = "data/" + BASE_DATA_KEY + ".json";
        pathKey = BASE_DATA_KEY;
        LoadData();
    }    
    
    public static List<GamePreset> GetAllItems() {
        return GamePresets.Instance.GetAll();
    }
    
    public static GamePreset Get(string code) {
        return GamePresets.Instance.GetByCode(code);
    }

    public void LoadCurrentPresets() {
    
        if(currentPresets == null) {
            currentPresets = new Dictionary<string,string>();
        }

        if(!currentPresets.ContainsKey(GamePresetTypes.character)) {
            currentPresets.Set(GamePresetTypes.character, GamePresetTypeDefault.characterDefault);
        }

        if(!currentPresets.ContainsKey(GamePresetTypes.item)) {
            currentPresets.Set(GamePresetTypes.item, GamePresetTypeDefault.itemDefault);
        }

        if(!currentPresets.ContainsKey(GamePresetTypes.terrain)) {
            currentPresets.Set(GamePresetTypes.terrain, GamePresetTypeDefault.terrainDefault);
        }
    }
        
    public void ChangeCurrentPreset(string presetType, string presetCode) {

        LoadCurrentPresets();
        
        currentPresets.Set(presetType, presetCode);
    }

    public string GetCurrentPreset(string presetType) {

        LoadCurrentPresets();

        if(currentPresets.ContainsKey(presetType)) {
            return currentPresets[presetType];
        }

        return null;
    }

    public GamePreset GetCurrentPresetData(string presetType) {
        string presetCode = GetCurrentPreset(presetType);

        if(!string.IsNullOrEmpty(presetCode)) {
            return GamePresets.Instance.GetById(presetCode); 
        }

        return null;
    }
    
    public GamePreset GetCurrentPresetDataCharacter() {
        return GetCurrentPresetData(GamePresetTypes.character);
    }
    
    public GamePreset GetCurrentPresetDataItem() {
        return GetCurrentPresetData(GamePresetTypes.item);
    }
    
    public GamePreset GetCurrentPresetDataTerrain() {
        return GetCurrentPresetData(GamePresetTypes.terrain);
    }
}

public class BaseGamePreset : GameDataObject {

    // Attributes that are added or changed after launch should be like this to prevent
    // profile conversions.

    
    public virtual GamePresetItems<GamePresetItem> data {
        get {
            return Get<GamePresetItems<GamePresetItem>>(BaseDataObjectKeys.data);
        }
        
        set {
            Set(BaseDataObjectKeys.data, value);
        }
    }

    public BaseGamePreset() {
        Reset();
    }

    public override void Reset() {
        base.Reset();
    }


    // Attributes that are added or changed after launch should be like this to prevent
    // profile conversions.
}