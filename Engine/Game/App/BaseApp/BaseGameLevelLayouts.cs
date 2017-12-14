using System;
using System.Collections.Generic;
using System.IO;

// using Engine.Data.Json;
using Engine.Utility;

using UnityEngine;

public class BaseGameLevelLayouts<T> : DataObjects<T> where T : DataObject, new() {
    private static T current;
    private static volatile BaseGameLevelLayouts<T> instance;
    private static System.Object syncRoot = new System.Object();
    public static string BASE_DATA_KEY = "game-level-layout-data";

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

    public static BaseGameLevelLayouts<T> BaseInstance {
        get {
            if (instance == null) {
                lock (syncRoot) {
                    if (instance == null)
                        instance = new BaseGameLevelLayouts<T>(true);
                }
            }

            return instance;
        }
        set {
            instance = value;
        }
    }

    public BaseGameLevelLayouts() {
        Reset();
    }

    public BaseGameLevelLayouts(bool loadData) {
        Reset();
        path = "data/" + BASE_DATA_KEY + ".json";
        pathKey = BASE_DATA_KEY;
        LoadData();
    }

    public virtual T GetDefaultLevelLayout() {
        T levelReturn = new T();
        foreach (T level in GetAll()) {
            return level;
        }
        return levelReturn;
    }

    public void SetGameLevelLayout(GameLevelLayout gameLevelLayout) {
        bool found = false;
        
        for (int i = 0; i < items.Count; i++) {
            if (GameLevelLayouts.Instance.items[i].code.ToLower() == gameLevelLayout.code.ToLower()) {
                GameLevelLayouts.Instance.items[i] = gameLevelLayout;
                found = true;
                break;
            }
        }
        
        if (!found) {
            GameLevelLayouts.Instance.items.Add(gameLevelLayout);
        }
    }

    public List<GameLevelLayout> GetByWorldId(string worldCode) {
        List<GameLevelLayout> filteredLevelLayouts = new List<GameLevelLayout>();
        foreach (GameLevelLayout levelLayout in GameLevelLayouts.Instance.GetAll()) {
            if (levelLayout.world_code == worldCode) {
                filteredLevelLayouts.Add(levelLayout);
            }
        }
        return filteredLevelLayouts;
    }

    public void ChangeCurrentAbsolute(string code) {
        GameLevelLayouts.Current.code = "changeme";
        ChangeCurrent(code);
    }
    
    public override void ChangeCurrent(string code) {
        base.ChangeCurrent(code);

        if (GameLevelLayouts.Current.code != code) {

            GameLevelLayouts.Current = GameLevelLayouts.Instance.GetById(code);

            string originalCode = code;
            if (string.IsNullOrEmpty(GameLevelLayouts.Current.code)) {
                //code = "level-" + code;
                GameLevelLayouts.Current = GameLevelLayouts.Instance.GetById(code);
            }
            
            if (string.IsNullOrEmpty(GameLevelLayouts.Current.code)) {
                // TODO not found add?
                GameLevelLayout obj = new GameLevelLayout();
                obj.code = code;
                obj.date_created = DateTime.Now;
                obj.date_modified = DateTime.Now;
                obj.description = originalCode;
                obj.display_name = code;
                obj.name = originalCode;
                obj.game_id = GameversesConfig.apiGameId;
                obj.key = originalCode;
                GameLevelLayouts.Instance.items.Add(obj);
            }
            
            if (string.IsNullOrEmpty(GameLevelLayouts.Current.code)) {
                GameLevelLayouts.Current = GameLevelLayouts.Instance.GetById(code);
            }

            LogUtil.Log("Changing LevelLayout: code:" + code);    
        }
    }
}

public class GameLevelLayoutType {
    public static string layoutGridList = "layout-grid-list";
    public static string layoutTextKeys = "layout-text-keys";
}

public class GameLevelLayoutDisplayType {
    public static string layoutCentered = "centered";
    public static string layoutExplicit = "explicit";
}

/*
"asset_data" : [{
               "code": "wall-1", 
               "position_data": {"x":1, "y":1, "z": 0}, 
               "local_position_data": {"x":0, "y":0, "z": 0}, 
               "rotation_data": {"x":0, "y":90, "z": 0}, 
               "scale_data": {"x":1, "y":1, "z": 1}
            },
            {
               "code": "wall-1", 
               "position_data": {"x":1, "y":1, "z": 0}, 
               "local_position_data": {"x":0, "y":0, "z": 0}, 
               "rotation_data": {"x":0, "y":9, "z": 0}, 
               "scale_data": {"x":1, "y":1, "z": 1}
            }
         ]


"display_type": "centered",
         "data_keys": {
            "-", {
               "code": "wall-1", 
               "local_position_data": {"x":0, "y":0, "z": 0}, 
               "rotation_data": {"x":0, "y":90, "z": 0}, 
               "scale_data": {"x":1, "y":1, "z": 1}
            }
            "|", {
               "code": "wall-1", 
               "local_position_data": {"x":0, "y":0, "z": 0}, 
               "rotation_data": {"x":0, "y":0, "z": 0}, 
               "scale_data": {"x":1, "y":1, "z": 1}
            }
            "c", {
               "code": "item-coin", 
               "local_position_data": {"x":0, "y":0, "z": 0}, 
               "rotation_data": {"x":0, "y":0, "z": 0}, 
               "scale_data": {"x":1, "y":1, "z": 1}
            }
         }
         "layout_data" : [
            "|---------  |",
            "| c c c c c |",
            "|           |",
            "| c c c c c |",
            "|           |",
            "| c c c c c |",
            "|  ---------|"
         ]

*/


public class GameLevelLayoutData : GameDataObject {
        
    // code
    // type

    public GameLevelLayoutData() {
        Reset();

    }

    public override void Reset() {
        base.Reset();

        data_list = new List<string>();
        data_object = new Dictionary<string, GameDataObject>();
        data_game_objects = new List<GameDataObject>();

        position_data = new Vector3Data();
    }

    public List<GameDataObject> GetLayoutAssets() {
        ProcessLayout();

        return data_game_objects;
    }

    public GameDataObject GetGameDataObjectKeyed(string assetKey) {

        if(data_object == null) {
            return null;
        }

        if(data_object.ContainsKey(assetKey)) {
            return data_object.Get(assetKey);
        }

        return null;
    }

    public void SetGameDataObject(GameDataObject gameDataObject) {
        if(data_game_objects == null) {
            return;
        }

        data_game_objects.Add(gameDataObject);
    }

    public void ProcessLayout() {
        // Get the list based level layout

        if(data_list == null) {
            return;
        }

        // If has list, clear the data objects already parsed.
        if(data_game_objects != null) {
            data_game_objects.Clear();
        }

        int x = 0;
        int y = 0;
        int z = data_list.Count;
       
        foreach(string row in data_list) {

            x = 0;

            char[] rowAssets = row.ToCharArray();

            if(rowAssets == null) {
                continue;
            }

            foreach(char c in rowAssets) {

                string key = c.ToString();

                if(!string.IsNullOrEmpty(key)) {

                    GameDataObject assetObject = GetGameDataObjectKeyed(key).ToDataObject<GameDataObject>();

                    if(assetObject != null) {

                        //if(assetObject.code == "wall-1") {
                        //    Debug.Log("ProcessLayout:" + " code:" + assetObject.code + " data:" + assetObject.ToJson());
                        //}

                        // add the game object in place of the text character 

                        if(assetObject.local_position_data == null) {
                            assetObject.local_position_data = new Vector3Data();
                        }

                        if(assetObject.local_rotation_data == null) {
                            assetObject.local_rotation_data = new Vector3Data();
                        }
                                            
                        if(assetObject.rotation_data == null) {
                            assetObject.rotation_data = new Vector3Data();
                        }

                        if(assetObject.position_data == null) {
                            assetObject.position_data = new Vector3Data();
                        }

                        if(assetObject.scale_data == null) {
                            assetObject.scale_data = new Vector3Data(1, 1, 1);
                        }

                        if(assetObject.grid_data == null) {
                            assetObject.grid_data = new Vector3Data(x, y, z);
                        }

                        //Debug.Log("ProcessLayout:" + " x:" + x + " y:" + y + " z:" + z + assetObject.grid_data.GetVector3());

                        if(assetObject.code == BaseDataObjectKeys.random) {
                            // skip and later randomize
                        }
                        else {
                            SetGameDataObject(assetObject);                        
                        }
                    }  
                }

                x++;
            }
            z--;
        }

        position_data = new Vector3Data(x, y, z);

        // Get they keyed asset dictionary to lookup assets to load

        // Fill data_game_objects with the list
    }
}

public class BaseGameLevelLayout : GameDataObject {

    public virtual GameLevelLayoutData data {
        get {
            return Get<GameLevelLayoutData>(BaseDataObjectKeys.data);
        }
        
        set {
            Set<GameLevelLayoutData>(BaseDataObjectKeys.data, value);
        }
    } 

    public BaseGameLevelLayout() {
        Reset();
    }

    public override void Reset() {
        base.Reset();
    }

    public int GetGridHeight() {

        if (data == null) {
            return 0;
        }

        if (data.data_list == null) {
            return 0;
        }

        return data.data_list.Count;
    }

    public int GetGridWidth() {

        if (data == null) {
            return 0;
        }

        if (data.data_list == null) {
            return 0;
        }

        if (data.data_list.Count == 0) {
            return 0;
        }

        return data.data_list[0].Length;
    }




    // Attributes that are added or changed after launch should be like this to prevent
    // profile conversions.
}