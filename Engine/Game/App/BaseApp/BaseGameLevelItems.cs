using System;
using System.Collections.Generic;
using System.IO;

using Engine.Data.Json;
using Engine.Utility;

using UnityEngine;

public class GameLevelItemAttributes {  
    public static string ATT_CURRENT_GAME_MODE_ARCADE_LEVEL = "game-mode-arcade-level"; 
}

public class BaseGameLevelItems<T> : DataObjects<T> where T : DataObject, new() {
    private static T current;
    private static volatile BaseGameLevelItems<T> instance;
    private static System.Object syncRoot = new System.Object();
    public static string BASE_DATA_KEY = "game-level-item-data";

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

    public static BaseGameLevelItems<T> BaseInstance {
        get {
            if (instance == null) {
                lock (syncRoot) {
                    if (instance == null)
                        instance = new BaseGameLevelItems<T>(true);
                }
            }

            return instance;
        }
        set {
            instance = value;
        }
    }

    public BaseGameLevelItems() {
        Reset();
    }

    public BaseGameLevelItems(bool loadData) {
        Reset();
        path = "data/" + BASE_DATA_KEY + ".json";
        pathKey = BASE_DATA_KEY;
        LoadData();
    }

    public void LoadDataItem(string code) {
    }

    public void SaveDataItem(string code) {
    }

    public virtual List<T> GetByPackId(string packId) {
        List<T> packLevels = new List<T>();
        foreach (T gameLevel in GetAll()) {
            List<string> packs = (List<string>)GetType().GetProperty("pack").GetValue(gameLevel, null);
            foreach (string pack in packs) {
                if (pack.ToLower() == packId.ToLower()) {
                    packLevels.Add(gameLevel);
                }
            }
        }
        return packLevels;
    }

    public virtual List<T> GetAllUnlocked() {
        // List<T> gameLevels = GetAll();
        List<T> gameLevelsFiltered = new List<T>();
        //foreach (T gameLevel in gameLevels) {

        //bool hasAccess = GameDatas.Instance.CheckIfHasAccessToLevel(gameLevel.code);
        //if(hasAccess) {
        //  gameLevelsFiltered.Add(gameLevel);
        //}
        // }
        return gameLevelsFiltered;
    }

    public virtual T GetDefault() {
        T levelReturn = new T();
        foreach (T level in GetAll()) {
            return level;
        }
        return levelReturn;
    }

    public virtual T GetDefaultByPack(string packId) {
        T levelReturn = new T();
        foreach (T level in GetByPackId(packId)) {
            string code = (string)GetType().GetProperty("code").GetValue(level, null);
            if (code != "all") {
                return level;
            }
        }
        return levelReturn;
    }

    //
    
    public virtual void Load(string code) {
        GameLevelItems.Current.code = code;        
        GameLevelItem currentItem = GameLevelItems.Instance.LoadItem(GameLevelItems.Instance.DATA_KEY, code);
        //GameLevelItem currentItem = GameLevelItems.Instance.LoadItem<GameLevelItem>(GameLevelItems.Instance.DATA_KEY, code);
        if (currentItem != null) {
            GameLevelItems.Current = currentItem;
        }
    }
    
    public virtual void Save() {
        GameLevelItems.Instance.Save(GameLevelItems.Current.code);
    }
    
    public virtual void Load() {
        GameLevelItems.Instance.Load(GameLevelItems.Current.code);
    }
    
    public virtual void Save(string code) {
        
        GameLevelItems.Current.code = code;
        
        if (string.IsNullOrEmpty(GameLevelItems.Current.code) 
            || GameLevelItems.Current.code == "changeme") {
            GameLevelItems.Current.code = GameLevels.Current.code;
        }
        
        if (!string.IsNullOrEmpty(GameLevelItems.Current.code)) {
            SaveItem(GameLevelItems.Instance.DATA_KEY, 
                     GameLevelItems.Current.code, GameLevelItems.Current);
        }
    }
    
    public virtual void ChangeCurrentAbsolute(string code) {
        GameLevelItems.Current.code = "changeme";
        GameLevelItems.Instance.ChangeCurrent(code);
    }

    public override void ChangeCurrent(string code) {
        base.ChangeCurrent(code);     
        //GameLevelItems.Instance.Save(Current.code);       
        GameLevelItems.Instance.Load(code);
    }    
}

public class GameLevelItemAssetStep : GameDataObject {
    
    public virtual Dictionary<string,object> data { 
        get {
            return Get<Dictionary<string,object>>(BaseDataObjectKeys.data);
        }
        
        set {
            Set<Dictionary<string,object>>(BaseDataObjectKeys.data, value);
        }
    }
    
    public GameLevelItemAssetStep() {
        Reset();
    }
    
    public override void Reset() {
        base.Reset();
        
        delay = 0.0;
        time = 1.0;
        ease_type = "linear"; // linear, easeInOut, easeIn, easeOut
        position_data = new Vector3Data();
        rotation_data = new Vector3Data();
        scale_data = new Vector3Data(1f, 1f, 1f);
        data = new Dictionary<string, object>();
    }
}

public class GameObjectProperties : GameDataObject {
    
    // position 
    // rotation
    // scale 
}

public class GameLevelItemAssetPhysicsType {
    public static string physicsStatic = "physics-static";
    public static string physicsOnCollide = "physics-oncollide";
    public static string physicsOnStart = "physics-onstart";
}

public class GameLevelItemAssetData : GameDataObject {
    
    // code asset code
    // limit
    
    public virtual GameLevelItemAsset game_level_item_asset {
        get {
            return Get<GameLevelItemAsset>(BaseDataObjectKeys.game_level_item_asset);
        }
        
        set {
            Set<GameLevelItemAsset>(BaseDataObjectKeys.game_level_item_asset, value);
        }
    }
        
    public virtual string physics_type {
        get {
            return Get<string>(BaseDataObjectKeys.physics_type);
        }
        
        set {
            Set<string>(BaseDataObjectKeys.physics_type, value);
        }
    }
    
    public virtual bool destructable {
        get {
            return Get<bool>(BaseDataObjectKeys.destructable);
        }
        
        set {
            Set<bool>(BaseDataObjectKeys.destructable, value);
        }
    }
    
    public virtual bool reactive {
        get {
            return Get<bool>(BaseDataObjectKeys.reactive);
        }
        
        set {
            Set<bool>(BaseDataObjectKeys.reactive, value);
        }
    }
    
    public virtual bool kinematic {
        get {
            return Get<bool>(BaseDataObjectKeys.kinematic);
        }
        
        set {
            Set<bool>(BaseDataObjectKeys.kinematic, value);
        }
    }
    
    public virtual bool gravity {
        get {
            return Get<bool>(BaseDataObjectKeys.gravity);
        }
        
        set {
            Set<bool>(BaseDataObjectKeys.gravity, value);
        }
    }
    
    public GameLevelItemAssetData() {
        Reset();
    }

    // scale range
    
    public void SetAssetScaleRange(float min, float max) {
        scale_data = new Vector3Data(GetAssetScaleRange(min, max));
    }
        
    // set
    
    // x
    
    public void SetAssetScaleRangeX(float val) {
        scale_data = new Vector3Data(GetAssetScaleRangeX(val));
    }
    
    public void SetAssetScaleRangeX(float min, float max) {
        scale_data = new Vector3Data(GetAssetScaleRangeX(min, max));
    }
    
    // y
    
    public void SetAssetScaleRangeY(float val) {
        scale_data = new Vector3Data(GetAssetScaleRangeY(val));
    }
    
    public void SetAssetScaleRangeY(float min, float max) {
        scale_data = new Vector3Data(GetAssetScaleRangeY(min, max));
    }
    
    // z
    
    public void SetAssetScaleRangeZ(float val) {
        scale_data = new Vector3Data(GetAssetScaleRangeZ(val));
    }
    
    public void SetAssetScaleRangeZ(float min, float max) {
        scale_data = new Vector3Data(GetAssetScaleRangeZ(min, max));
    }
    
    // get
    
    public Vector3 GetAssetScaleRange(float min, float max) {
        float range = UnityEngine.Random.Range(min, max);
        return Vector3.zero.WithX(range).WithY(range).WithZ(range);
    }
    
    // x
    
    public Vector3 GetAssetScaleRangeX(float val) {
        return GetAssetScaleRange(val, val, 0, 0, 0, 0);
    }
    
    public Vector3 GetAssetScaleRangeX(float min, float max) {
        return GetAssetScaleRange(min, max, 0, 0, 0, 0);
    }
    
    // y
    
    public Vector3 GetAssetScaleRangeY(float val) {
        return GetAssetScaleRange(0, 0, val, val, 0, 0);
    }
    
    public Vector3 GetAssetScaleRangeY(float min, float max) {
        return GetAssetScaleRange(0, 0, min, max, 0, 0);
    }
        
    // z
    
    public Vector3 GetAssetScaleRangeZ(float val) {
        return GetAssetScaleRange(0, 0, 0, 0, val, val);
    }
    
    public Vector3 GetAssetScaleRangeZ(float min, float max) {
        return GetAssetScaleRange(0, 0, 0, 0, min, max);
    }
    
    // all
    
    public Vector3 GetAssetScaleRange(
        float minX, float maxX,
        float minY, float maxY,
        float minZ, float maxZ) {
        
        float range_x = UnityEngine.Random.Range(minX, maxX);
        float range_y = UnityEngine.Random.Range(minY, maxY);
        float range_z = UnityEngine.Random.Range(minZ, maxZ);
        
        return Vector3.zero.WithX(range_x).WithY(range_y).WithZ(range_z);
    }

    // rotation range

    // set

    // x

    public void SetAssetRotationRangeX(float val) {
        rotation_data = new Vector3Data(GetAssetRotationRangeX(val));
    }

    public void SetAssetRotationRangeX(float min, float max) {
        rotation_data = new Vector3Data(GetAssetRotationRangeX(min, max));
    }
    
    // y
    
    public void SetAssetRotationRangeY(float val) {
        rotation_data = new Vector3Data(GetAssetRotationRangeY(val));
    }
    
    public void SetAssetRotationRangeY(float min, float max) {
        rotation_data = new Vector3Data(GetAssetRotationRangeY(min, max));
    }
    
    // z
    
    public void SetAssetRotationRangeZ(float val) {
        rotation_data = new Vector3Data(GetAssetRotationRangeZ(val));
    }
    
    public void SetAssetRotationRangeZ(float min, float max) {
        rotation_data = new Vector3Data(GetAssetRotationRangeZ(min, max));
    }

    // get

    public Vector3 GetAssetRotationRange(float min, float max) {
        return GetAssetRotationRange(min, max, min, max, min, max);
    }

    // x
    
    public Vector3 GetAssetRotationRangeX(float val) {
        return GetAssetRotationRange(val, val, 0, 0, 0, 0);
    }
    
    public Vector3 GetAssetRotationRangeX(float min, float max) {
        return GetAssetRotationRange(min, max, 0, 0, 0, 0);
    }
    
    // y
    
    public Vector3 GetAssetRotationRangeY(float val) {
        return GetAssetRotationRange(0, 0, val, val, 0, 0);
    }
    
    public Vector3 GetAssetRotationRangeY(float min, float max) {
        return GetAssetRotationRange(0, 0, min, max, 0, 0);
    }
    
    
    // z
    
    public Vector3 GetAssetRotationRangeZ(float val) {
        return GetAssetRotationRange(0, 0, 0, 0, val, val);
    }
    
    public Vector3 GetAssetRotationRangeZ(float min, float max) {
        return GetAssetRotationRange(0, 0, 0, 0, min, max);
    }

    // all
    
    public Vector3 GetAssetRotationRange(
        float minX = 0, float maxX = 0,
        float minY = 0, float maxY = 0,
        float minZ = 0, float maxZ = 0) {
        
        float range_rotation_x = UnityEngine.Random.Range(minX, maxX);
        float range_rotation_y = UnityEngine.Random.Range(minY, maxY);
        float range_rotation_z = UnityEngine.Random.Range(minZ, maxZ);
        
        return Vector3.zero.WithX(range_rotation_x).WithY(range_rotation_y).WithZ(range_rotation_z);
    }

    //
    
    public override void Reset() {
        game_level_item_asset = new GameLevelItemAsset();
        position_data = new Vector3Data(Vector3.zero);
        limit = 0;
        physics_type = GameLevelItemAssetPhysicsType.physicsStatic;
        destructable = false;
        reactive = false;
        kinematic = true;
        gravity = true;
        scale_data = new Vector3Data(Vector3.one);
        rotation_data = new Vector3Data(Vector3.zero); // range of rotation start stop
    }
}

public class GameLevelItemAsset : GameDataObject {
    
    // code
    //
    
    
    public virtual string physics_type {
        get {
            return Get<string>(BaseDataObjectKeys.physics_type);
        }
        
        set {
            Set<string>(BaseDataObjectKeys.physics_type, value);
        }
    }
    
    public virtual bool destructable {
        get {
            return Get<bool>(BaseDataObjectKeys.destructable);
        }
        
        set {
            Set<bool>(BaseDataObjectKeys.destructable, value);
        }
    }
    
    public virtual bool reactive {
        get {
            return Get<bool>(BaseDataObjectKeys.reactive);
        }
        
        set {
            Set<bool>(BaseDataObjectKeys.reactive, value);
        }
    }
    
    public virtual bool kinematic {
        get {
            return Get<bool>(BaseDataObjectKeys.kinematic);
        }
        
        set {
            Set<bool>(BaseDataObjectKeys.kinematic, value);
        }
    }
    
    public virtual bool gravity {
        get {
            return Get<bool>(BaseDataObjectKeys.gravity);
        }
        
        set {
            Set<bool>(BaseDataObjectKeys.gravity, value);
        }
    }

    public virtual List<GameLevelItemAssetStep> steps {
        get {
            return Get<List<GameLevelItemAssetStep>>(BaseDataObjectKeys.steps);
        }
        
        set {
            Set<List<GameLevelItemAssetStep>>(BaseDataObjectKeys.steps, value);
        }
    }
    
    public virtual Dictionary<string,object> data {
        get {
            return Get<Dictionary<string,object>>(BaseDataObjectKeys.data);
        }
        
        set {
            Set<Dictionary<string,object>>(BaseDataObjectKeys.data, value);
        }
    }
    
    // uuid 
    
    public virtual string destroy_effect_code {
        get {
            return Get<string>(BaseDataObjectKeys.destroy_effect_code);
        }
        
        set {
            Set<string>(BaseDataObjectKeys.destroy_effect_code, value);
        }
    }
    
    public virtual bool destroyed {
        get {
            return Get<bool>(BaseDataObjectKeys.destroyed, false);
        }
        
        set {
            Set<bool>(BaseDataObjectKeys.destroyed, value);
        }
    }
    
    public virtual Vector3Data speed_rotation {
        get {
            return Get<Vector3Data>(BaseDataObjectKeys.speed_rotation);
        }
        
        set {
            Set<Vector3Data>(BaseDataObjectKeys.speed_rotation, value);
        }
    }
    
    public GameLevelItemAsset() {
        Reset();
    }
    
    public override void Reset() {
        base.Reset();
        
        code = "";
        destroy_effect_code = "";
        physics_type = GameLevelItemAssetPhysicsType.physicsStatic;
        steps = new List<GameLevelItemAssetStep>();
        uuid = UniqueUtil.CreateUUID4();
        data = new Dictionary<string, object>();
        
        destructable = true;
        destroyed = false;
        kinematic = false;
        gravity = false;
        reactive = false;
        
        speed_rotation = new Vector3Data();

        local_position_data = new Vector3Data(0, 0, 0);
        local_rotation_data = new Vector3Data(0, 0, 0);
        scale_data = new Vector3Data(1, 1, 1);
    }
}

public class BaseGameLevelItem : GameDataObject {

    // Attributes that are added or changed after launch should be like this to prevent
    // profile conversions.

    public BaseGameLevelItem() {
        Reset();
    }

    public override void Reset() {
        base.Reset();
    }

    public void Clone(BaseGameLevelItem toCopy) {
        base.Clone(toCopy);
    }

    // Attributes that are added or changed after launch should be like this to prevent
    // profile conversions.
}