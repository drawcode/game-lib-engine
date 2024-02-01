using System;
using System.Collections.Generic;
using System.IO;
using Engine.Utility;
using Engine.Game.Data;

using UnityEngine;

namespace Engine.Game.App.BaseApp
{
    public class BaseGameLevels<T> : DataObjects<T> where T : DataObject, new()
    {
        private static T current;
        private static volatile BaseGameLevels<T> instance;
        private static System.Object syncRoot = new System.Object();
        public static string BASE_DATA_KEY = "game-level-data";
        //
        //public static GameLevelData defaultLevelData;
        //public static GameLevelData currentLevelData;
        //
        public static T BaseCurrent
        {
            get
            {
                if (current == null)
                {
                    lock (syncRoot)
                    {
                        if (current == null)
                            current = new T();
                    }
                }

                return current;
            }
            set
            {
                current = value;
            }
        }

        public static BaseGameLevels<T> BaseInstance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new BaseGameLevels<T>(true);
                    }
                }

                return instance;
            }
            set
            {
                instance = value;
            }
        }

        public BaseGameLevels()
        {
            Reset();
        }

        public BaseGameLevels(bool loadData)
        {
            Reset();
            path = "data/" + BASE_DATA_KEY + ".json";
            pathKey = BASE_DATA_KEY;
            LoadData();

        }

        public override void Reset()
        {
            base.Reset();

            //defaultLevelData = new GameLevelData();
            //currentLevelData = new GameLevelData();
        }

        public virtual T GetDefaultLevel()
        {
            T levelReturn = new T();
            foreach (T level in GetAll())
            {
                return level;
            }
            return levelReturn;
        }

        public void PrepareDefaultData()
        {

            /*
            items = new List<GameLevel>();

            SetGameLevel(
                "airship", 
                "Airship", 
                "Airship",
                "The airship acts as the game’s central hub.", 
                "airship",
                0,
                0);

            LogUtil.Log("GameLevels:" + JsonMapper.ToJson(items));
            */
        }

        public void SetGameLevel(string code, string name, string displayName,
                                 string description, string type, int sortIndex, int typeSortIndex)
        {
            bool found = false;

            for (int i = 0; i < GameLevels.Instance.items.Count; i++)
            {
                if (GameLevels.Instance.items[i].code.ToLower() == code.ToLower())
                {
                    GameLevels.Instance.items[i].code = code;
                    GameLevels.Instance.items[i].name = name;
                    GameLevels.Instance.items[i].display_name = displayName;
                    GameLevels.Instance.items[i].description = description;
                    GameLevels.Instance.items[i].type = type;
                    GameLevels.Instance.items[i].sort_order = sortIndex;
                    GameLevels.Instance.items[i].sort_order_type = typeSortIndex;
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                GameLevel obj = new GameLevel();
                obj.active = true;
                obj.SetAttributeStringValue("default", "default");

                obj.code = code;
                obj.description = description;
                obj.display_name = displayName;
                obj.game_id = "11111111-1111-1111-1111-111111111111";
                obj.key = code;
                obj.name = name;
                obj.order_by = "";
                obj.sort_order = sortIndex;
                obj.sort_order_type = typeSortIndex;
                obj.status = "";
                obj.type = "default";
                obj.uuid = UniqueUtil.CreateUUID4();
                GameLevels.Instance.items.Add(obj);
            }
        }

        public void SetGameLevel(GameLevel gameLevel)
        {
            bool found = false;

            for (int i = 0; i < items.Count; i++)
            {
                if (GameLevels.Instance.items[i].code.ToLower() == gameLevel.code.ToLower())
                {
                    GameLevels.Instance.items[i] = gameLevel;
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                GameLevels.Instance.items.Add(gameLevel);
            }
        }

        /*
        public override GameLevel GetById(string levelCode) {
            foreach(GameLevel level in GetAll()) {
                if(level.code == levelCode) {
                    return level;
                }
            }
            return null;
        }
        */

        public List<GameLevel> GetByWorldId(string worldCode)
        {
            List<GameLevel> filteredLevels = new List<GameLevel>();
            foreach (GameLevel level in GameLevels.Instance.GetAll())
            {
                if (level.world_code == worldCode)
                {
                    filteredLevels.Add(level);
                }
            }
            return filteredLevels;
        }

        public void ReloadLevel()
        {
            ReloadLevel(GameLevels.Current.code);
        }

        public void ReloadLevel(string levelCode)
        {
            GameLevelItems.Instance.Load(levelCode);
        }

        public void ChangeCurrentAbsolute(string code)
        {
            //GameLevels.Current.code = "changeme";
            ChangeCurrent(code);
        }

        public override void ChangeCurrent(string code)
        {
            base.ChangeCurrent(code);

            Debug.Log("GameLevels:ChangeCurrent:" + " code:" + code);

            if (GameLevels.Current.code != code)
            {
                GameLevels.Current = GameLevels.Instance.GetById(code);

                string originalCode = code;

                if (string.IsNullOrEmpty(GameLevels.Current.code))
                {
                    //code = "level-" + code;
                    GameLevels.Current = GameLevels.Instance.GetById(code);
                }

                if (string.IsNullOrEmpty(GameLevels.Current.code))
                {
                    // TODO not found add?
                    GameLevel gameLevel = new GameLevel();
                    gameLevel.code = code;
                    gameLevel.date_created = DateTime.Now;
                    gameLevel.date_modified = DateTime.Now;
                    gameLevel.description = originalCode;
                    gameLevel.display_name = code;
                    gameLevel.name = originalCode;

#if USE_GAME_LIB_CONTENT
                    gameLevel.game_id = ContentsConfig.contentAppFolder;
#endif
                    gameLevel.key = originalCode;
                    gameLevel.world_code = GameWorlds.Current.code;
                    gameLevel.data = new GameLevelDataObjectItem();
                    GameLevels.Instance.items.Add(gameLevel);
                }

                if (string.IsNullOrEmpty(GameLevels.Current.code))
                {
                    GameLevels.Current = GameLevels.Instance.GetById(code);
                }

                //

                if (GameLevels.Current.data != null)
                {

                    GameLevelData levelData = GameLevels.Current.data.level_data;

                    if (levelData != null)
                    {
                        GameLevels.Current.data.level_data.Copy(levelData);
                        //currentLevelData.Copy(levelData);
                    }
                    else
                    {

                        //currentLevelData.Copy(defaultLevelData);
                        GameLevels.Current.data.level_data.Copy(new GameLevelData());
                    }
                }

                // Update World

                if (!string.IsNullOrEmpty(GameLevels.Current.world_code))
                {
                    GameWorlds.Instance.ChangeCurrent(GameLevels.Current.world_code);
                }
            }

            Debug.Log("GameLevels:ChangeCurrent:" + " GameLevels.Current.code:" + GameLevels.Current.code);
        }

        //

#if USE_GAME_LIB_GAMES

        public static GameLevelGridData GetLevelGridTerrains(
            GameLevelGridData dataItems, List<GameDataTerrainPreset> presets)
        {

            foreach (GameDataTerrainPreset terrainDataItem in presets)
            {

                GamePreset terrainPreset = GamePresets.Instance.GetById(terrainDataItem.code);

                if (terrainPreset != null)
                {

                    GamePresetItem terrainPresetItem =
                        terrainPreset.GetItemRandomByProbability(terrainPreset.data.items);

                    float offsetX = 0;
                    float offsetY = 0;
                    float offsetZ = 0;

                    if (GameLevels.Current.data.level_data.grid_centered_x)
                    {
                        offsetX = (float)(GameLevels.Current.data.level_data.grid_width / 2);
                    }

                    if (GameLevels.Current.data.level_data.grid_centered_y)
                    {
                        offsetY = (float)(GameLevels.Current.data.level_data.grid_height / 2);
                    }

                    if (GameLevels.Current.data.level_data.grid_centered_z)
                    {
                        offsetZ = (float)(GameLevels.Current.data.level_data.grid_depth / 2);
                    }

                    float gridX = offsetX;// + (float)(layoutObjectItem.grid_data.x);
                    float gridY = offsetY;// + (float)(layoutObjectItem.grid_data.y);
                    float gridZ = offsetZ;// + (float)(layoutObjectItem.grid_data.z);

                    Vector3 gridPos = Vector3.zero.WithX(gridX).WithY(gridY).WithZ(gridZ);
                    Vector3 gridScale = Vector3.one;
                    Vector3 gridRotation = Vector3.zero;

                    string assetCode = terrainPresetItem.code;
                    string assetType = terrainPresetItem.type;
                    string assetDataType = terrainPresetItem.data_type;
                    string assetDisplayType = terrainPresetItem.display_type;

                    dataItems.SetAssetsInAssetMap(
                        assetCode,
                        assetType,
                        assetDataType,
                        assetDisplayType,
                        gridPos,
                        gridScale,
                        gridRotation);

                    //if(terrainPresetItem != null) {
                    //    dataItems = GameLevelGridData.AddAssets(dataItems, terrainPresetItem.code, 1);
                    //}
                }
            }

            return dataItems;
        }

#endif

        // -----------------------------------------------------------------------
        // GRID LEVEL LAYOUTS

        public static string GetLevelLayoutCode(GameDataLayoutPreset dataItem)
        {

            string layoutCode = dataItem.code;

            if (dataItem.data_type.IsEqualLowercase(BaseDataObjectKeys.preset))
            {

                // This is a nested preset, get preset and set code

                GamePreset preset = GamePresets.Instance.GetByCode(layoutCode);

                if (preset != null)
                {

                    GamePresetItem presetItem = preset.GetItemRandomByProbability(preset.data.items);

                    if (presetItem != null)
                    {
                        //int amount = 1;
                        //dataItems = GameLevelGridData.AddAssets(dataItems, presetItem.code, amount);
                        layoutCode = presetItem.code;
                    }

                }
            }

            return layoutCode;
        }

        public static bool IsGameLevelLayoutType(string dataType, string filterType)
        {

            if (dataType.IsNullOrEmpty()
                   && filterType.IsEqualLowercase(BaseDataObjectKeys.defaultKey))
            {
                return true;
            }

            if (!dataType.IsEqualLowercase(filterType))
            {
                return false;
            }

            return true;
        }

        public static GameLevelLayout GetGameLevelLayoutFromPresets(
            List<GameDataLayoutPreset> presets, string loadTypeFilter = "default")
        {

            // Get a part from the current presets from level or world to use
            // as load_type ="dynamic"

            // TODO make all or one select, if not preset then collect all and randomize
            // Currently expects dynamic types to be preset data_type to select by proability

            foreach (GameDataLayoutPreset dataItem in presets)
            {

                // If is is not a dynamic load type load on start, else skip load

                if (!IsGameLevelLayoutType(dataItem.load_type, loadTypeFilter))
                {
                    continue;
                }

                // If the data_type is preset then get the preset from probability

                string layoutCode = GetLevelLayoutCode(dataItem);

                // Handle loading the preset assets meta information info data

                return GameLevelLayouts.Instance.GetById(layoutCode);
            }

            return null;
        }

        public static List<GameDataObject> GetGameLevelLayoutObjects(
            GameLevelLayout gameLevelLayout)
        {

            if (gameLevelLayout == null || gameLevelLayout.data == null)
            {
                return null;
            }

            List<GameDataObject> layoutObjects = gameLevelLayout.data.GetLayoutAssets();

            if (layoutObjects == null)
            {
                return null;
            }

            return layoutObjects;
        }

#if USE_GAME_LIB_GAMES
        public static GameLevelGridData GetLevelGridLayoutParts(
            GameLevelGridData dataItems, List<GameDataLayoutPreset> presets,
            string loadTypeFilter = "default")
        {

            foreach (GameDataLayoutPreset dataItem in presets)
            {

                // If is is not a dynamic load type load on start, else skip load

                if (!IsGameLevelLayoutType(dataItem.load_type, loadTypeFilter))
                {
                    continue;
                }

                // If the data_type is preset then get the preset from probability

                string layoutCode = GetLevelLayoutCode(dataItem);

                // Handle loading the preset assets meta informatino info data

                GameLevelLayout gameLevelLayout = GameLevelLayouts.Instance.GetById(layoutCode);

                if (gameLevelLayout == null || gameLevelLayout.data == null)
                {
                    continue;
                }

                List<GameDataObject> layoutObjects = gameLevelLayout.data.GetLayoutAssets();

                if (layoutObjects == null)
                {
                    continue;
                }

                /*

                List<GameDataObject> layoutObjects = gameLevelLayout.data.GetLayoutAssets();

                if (layoutObjects == null) {
                    continue;
                }

                Vector3 size = Vector3.zero;

                if (gameLevelLayout.data.position_data == null) {
                    gameLevelLayout.data.position_data = new Vector3Data();
                }

                if (dataItem.display_type == GameLevelLayoutDisplayType.layoutCentered) {
                    size = gameLevelLayout.data.position_data.GetVector3();
                }

                //Debug.Log("layoutObjects.Count:" + layoutObjects.Count);

                float offsetX = 0;
                float offsetZ = 0;
                float offsetY = 0;// TODO 2d/3d type ((int)GameLevels.gridHeight / 2) + 3;

                float offsetPlayerX = 3;
                float offsetPlayerY = 3;
                float offsetPlayerZ = 0;

                foreach (GameDataObject layoutObjectItem in layoutObjects) {

                    if (layoutObjectItem.position_data == null) {
                        layoutObjectItem.position_data = new Vector3Data();
                    }

                    if (layoutObjectItem.local_position_data == null) {
                        layoutObjectItem.local_position_data = new Vector3Data();
                    }

                    if (layoutObjectItem.grid_data == null) {
                        layoutObjectItem.grid_data = new Vector3Data();
                    }

                    if (dataItem.grid_data == null) {
                        dataItem.grid_data = new Vector3Data();
                    }

                    if (dataItem.type == GameLevelLayoutDisplayType.layoutCentered) {

                        if (GameLevels.currentLevelData.grid_centered_x) {
                            offsetX =
                                (float)((GameLevels.currentLevelData.grid_width / 2) + offsetPlayerX) -
                                    ((float)size.x / 2);
                        }

                        if (GameLevels.currentLevelData.grid_centered_y) {
                            offsetY =
                                (float)((GameLevels.currentLevelData.grid_height / 2) + offsetPlayerY) -
                                    ((float)size.y / 2);
                        }

                        if (GameLevels.currentLevelData.grid_centered_z) {
                            offsetZ =
                                (float)((GameLevels.currentLevelData.grid_depth / 2) + offsetPlayerZ) -
                                    ((float)size.z / 2);
                        }
                    }

                    float gridX = offsetX + (float)(layoutObjectItem.grid_data.x) + (float)(dataItem.grid_data.x);
                    float gridY = offsetY + (float)(layoutObjectItem.grid_data.y) + (float)(dataItem.grid_data.y);
                    float gridZ = offsetZ + (float)(layoutObjectItem.grid_data.z) + (float)(dataItem.grid_data.z);

                    Vector3 gridPos = Vector3.zero.WithX(gridX).WithY(gridY).WithZ(gridZ);
                    Vector3 gridScale = layoutObjectItem.scale_data.GetVector3();
                    Vector3 gridRotation = layoutObjectItem.local_rotation_data.GetVector3();

                    Vector3 localPosition = layoutObjectItem.local_position_data.GetVector3();

                    string assetCode = layoutObjectItem.code;
                    string assetType = layoutObjectItem.type;
                    string assetDataType = layoutObjectItem.data_type;
                    string assetDisplayType = layoutObjectItem.display_type;

                    if (assetCode != BaseDataObjectKeys.empty) {
                        Debug.Log("layoutObjectItem:" + " assetCode:" + assetCode + " gridPos:" + gridPos
                            + " gridScale:" + gridScale + " gridRotation:" + gridRotation
                            + " gridX:" + gridX + " gridY:" + gridY + " gridZ:" + gridZ
                            + " layoutObjectItem.grid_data:" + layoutObjectItem.grid_data.GetVector3()
                        );
                    }

                    dataItems.SetAssetsInAssetMap(
                        assetCode,
                        assetType,
                        assetDataType,
                        assetDisplayType,
                        gridPos,
                        gridScale,
                        gridRotation,
                        localPosition);
                }
                */

            }

            return dataItems;
        }
#endif


#if USE_GAME_LIB_GAMES
        public static GameLevelGridData GetLevelGridLayouts(
            GameLevelGridData dataItems, List<GameDataLayoutPreset> presets, string loadTypeFilter = "default")
        {

            foreach (GameDataLayoutPreset dataItem in presets)
            {

                // If is is not a dynamic load type load on start, else skip load

                if (!IsGameLevelLayoutType(dataItem.load_type, loadTypeFilter))
                {
                    continue;
                }

                // If the data_type is preset then get the preset from probability

                string layoutCode = GetLevelLayoutCode(dataItem);

                // Handle loading the preset assets meta informatino info data

                GameLevelLayout gameLevelLayout = GameLevelLayouts.Instance.GetById(layoutCode);

                if (gameLevelLayout == null || gameLevelLayout.data == null)
                {
                    continue;
                }

                List<GameDataObject> layoutObjects = gameLevelLayout.data.GetLayoutAssets();

                if (layoutObjects == null)
                {
                    continue;
                }

                Vector3 size = Vector3.zero;

                if (gameLevelLayout.data.position_data == null)
                {
                    gameLevelLayout.data.position_data = new Vector3Data();
                }

                if (dataItem.display_type == GameLevelLayoutDisplayType.layoutCentered)
                {
                    size = gameLevelLayout.data.position_data.GetVector3();
                }

                //Debug.Log("layoutObjects.Count:" + layoutObjects.Count);

                float offsetX = 0;
                float offsetZ = 0;
                float offsetY = 0;// TODO 2d/3d type ((int)GameLevels.gridHeight / 2) + 3;

                float offsetPlayerX = 3;
                float offsetPlayerY = 3;
                float offsetPlayerZ = 0;

                foreach (GameDataObject layoutObjectItem in layoutObjects)
                {

                    if (layoutObjectItem.position_data == null)
                    {
                        layoutObjectItem.position_data = new Vector3Data();
                    }

                    if (layoutObjectItem.local_position_data == null)
                    {
                        layoutObjectItem.local_position_data = new Vector3Data();
                    }

                    if (layoutObjectItem.grid_data == null)
                    {
                        layoutObjectItem.grid_data = new Vector3Data();
                    }

                    if (dataItem.grid_data == null)
                    {
                        dataItem.grid_data = new Vector3Data();
                    }

                    if (dataItem.type == GameLevelLayoutDisplayType.layoutCentered)
                    {

                        if (GameLevels.Current.data.level_data.grid_centered_x)
                        {
                            offsetX =
                                (float)((GameLevels.Current.data.level_data.grid_width / 2) + offsetPlayerX) -
                                    ((float)size.x / 2);
                        }

                        if (GameLevels.Current.data.level_data.grid_centered_y)
                        {
                            offsetY =
                                (float)((GameLevels.Current.data.level_data.grid_height / 2) + offsetPlayerY) -
                                    ((float)size.y / 2);
                        }

                        if (GameLevels.Current.data.level_data.grid_centered_z)
                        {
                            offsetZ =
                                (float)((GameLevels.Current.data.level_data.grid_depth / 2) + offsetPlayerZ) -
                                    ((float)size.z / 2);
                        }
                    }

                    float gridX = offsetX + (float)(layoutObjectItem.grid_data.x) + (float)(dataItem.grid_data.x);
                    float gridY = offsetY + (float)(layoutObjectItem.grid_data.y) + (float)(dataItem.grid_data.y);
                    float gridZ = offsetZ + (float)(layoutObjectItem.grid_data.z) + (float)(dataItem.grid_data.z);

                    double scaleMin = dataItem.scale_min;
                    double scaleMax = dataItem.scale_max;

                    //Vector3 gridScaleRandom = MathUtil.RandomRangeConstrain((float)scaleMin, (float)scaleMax);

                    Vector3 gridPos = Vector3.zero.WithX(gridX).WithY(gridY).WithZ(gridZ);
                    Vector3 gridScale = layoutObjectItem.scale_data.GetVector3();
                    Vector3 gridRotation = layoutObjectItem.local_rotation_data.GetVector3();

                    Vector3 localPosition = layoutObjectItem.local_position_data.GetVector3();

                    string assetCode = layoutObjectItem.code;
                    string assetType = layoutObjectItem.type;
                    string assetDataType = layoutObjectItem.data_type;
                    string assetDisplayType = layoutObjectItem.display_type;

                    if (assetCode != BaseDataObjectKeys.empty
                        && assetCode.IsNotNullOrEmpty())
                    {

                        Debug.Log("layoutObjectItem:" + " assetCode:" + assetCode + " gridPos:" + gridPos
                            + " gridScale:" + gridScale + " gridRotation:" + gridRotation
                            + " gridX:" + gridX + " gridY:" + gridY + " gridZ:" + gridZ
                            + " layoutObjectItem.grid_data:" + layoutObjectItem.grid_data.GetVector3()
                        );
                    }

                    dataItems.SetAssetsInAssetMap(
                        assetCode,
                        assetType,
                        assetDataType,
                        assetDisplayType,
                        gridPos,
                        gridScale,
                        gridRotation,
                        localPosition);
                }

            }

            /*
            dataItems.SetAssetsInAssetMap("wall-1", Vector3.zero.WithX(offsetX+2).WithY(1).WithZ(offsetZ+2), scale, Vector3.zero.WithY(90));
            dataItems.SetAssetsInAssetMap("wall-1", Vector3.zero.WithX(offsetX+2).WithY(1).WithZ(offsetZ+4), scale, Vector3.zero.WithY(90));
            dataItems.SetAssetsInAssetMap("wall-1", Vector3.zero.WithX(offsetX+2).WithY(1).WithZ(offsetZ+6), scale, Vector3.zero.WithY(90));
            dataItems.SetAssetsInAssetMap("wall-1", Vector3.zero.WithX(offsetX+2).WithY(1).WithZ(offsetZ+8), scale, Vector3.zero.WithY(90));
            dataItems.SetAssetsInAssetMap("wall-1", Vector3.zero.WithX(offsetX+4).WithY(1).WithZ(offsetZ+8), scale, Vector3.zero.WithY(0));
            dataItems.SetAssetsInAssetMap("wall-1", Vector3.zero.WithX(offsetX+6).WithY(1).WithZ(offsetZ+8), scale, Vector3.zero.WithY(0));
            dataItems.SetAssetsInAssetMap("wall-1", Vector3.zero.WithX(offsetX+8).WithY(1).WithZ(offsetZ+8), scale, Vector3.zero.WithY(0));
    */
            return dataItems;
        }
#endif

        // -----------------------------------------------------------------------
        // GAME LEVEL ASSETS

#if USE_GAME_LIB_GAMES
        public static GameLevelGridData GetLevelGridAssets(
            GameLevelGridData dataItems, List<GameDataAssetPreset> presets, string loadTypeFilter = "default")
        {

            foreach (GameDataAssetPreset assetDataItem in presets)
            {

                double scaleMin = assetDataItem.scale_min;
                double scaleMax = assetDataItem.scale_max;

                Vector3 gridScaleRandom = MathUtil.RandomRangeConstrain((float)scaleMin, (float)scaleMax);

                // PRELOAD

                if (IsGameLevelLayoutType(assetDataItem.load_type, BaseDataObjectKeys.dynamicKey))
                {
                    GameController.PreloadLevelAssetPreset(assetDataItem);
                }

                // USE

                if (!IsGameLevelLayoutType(assetDataItem.load_type, loadTypeFilter))
                {
                    continue;
                }

                int minAssetLimit = (int)assetDataItem.min;
                int maxAssetLimit = (int)assetDataItem.max;

                int randomAssetLimit = UnityEngine.Random.Range(minAssetLimit, maxAssetLimit);

                int totalAssetLimit = 0;

                bool isNestedLimitsType = assetDataItem.Get<string>(BaseDataObjectKeys.data_type) == "nested_limits" ? true : false;

                GamePreset assetPreset = GamePresets.Instance.GetById(assetDataItem.code);

                if (assetPreset != null)
                {

                    if (!isNestedLimitsType)
                    {

                        for (int i = 0; i < randomAssetLimit; i++)
                        {

                            GamePresetItem presetItem = assetPreset.GetItemRandomByProbability(assetPreset.data.items);

                            if (presetItem != null)
                            {
                                int amount = 1;
                                dataItems = GameLevelGridData.AddAssets(dataItems, presetItem.code, amount);
                                totalAssetLimit += amount;
                            }
                        }
                    }
                    else
                    {

                        foreach (GamePresetItem presetItem in assetPreset.data.items)
                        {

                            int amount = UnityEngine.Random.Range((int)presetItem.min, (int)presetItem.max);
                            totalAssetLimit += amount;

                            dataItems = GameLevelGridData.AddAssets(dataItems, presetItem.code, amount);

                            if (totalAssetLimit > maxAssetLimit)
                            {
                                // Too many for this set to add more...
                                break;
                            }
                        }
                    }
                }
            }

            return dataItems;
        }
#endif

        public static List<GameDataLayoutPreset> GetLevelLayoutPresets(string loadTypeFilter = "default")
        {
            return GetLevelLayoutPresets(GameLevels.Current.code, loadTypeFilter);
        }

        public static List<GameDataLayoutPreset> GetLevelLayoutPresets(string levelCode, string loadTypeFilter = "default")
        {

            List<GameDataLayoutPreset> filteredItems = new List<GameDataLayoutPreset>();

            GameLevel gameLevel = GameLevels.Instance.GetByCode(levelCode);

            if (gameLevel == null)
            {
                return filteredItems;
            }

            List<GameDataLayoutPreset> dataItems = gameLevel.data.layout_presets;

            foreach (GameDataLayoutPreset dataItem in dataItems)
            {

                if (!IsGameLevelLayoutType(dataItem.load_type, loadTypeFilter))
                {
                    continue;
                }

                //if (dataItem.load_type.IsEqualLowercase(loadTypeFilter)
                //    || (loadTypeFilter == BaseDataObjectKeys.defaultKey 
                //        && dataItem.load_type.IsNullOrEmpty())) {
                filteredItems.Add(dataItem);
                //}
            }

            return filteredItems;
        }


    }


    public class BaseGameLevelKeys
    {
        public static string LEVEL_INITIAL_DIFFICULTY = "initial-diff";
        public static string LEVEL_SPONSOR_NAME = "sponsor";
        public static string LEVEL_SPONSOR_IMAGE = "sponsor-img";
    }

    public class GameLevelRenderType
    {
        public static string type_3d = "3d";
        public static string type_2d = "2d";
    }

    public class GameLevelData : DataObject
    {

        /*
         * 
        public static float gridHeight = 1f;
        public static float gridWidth = 240f;
        public static float gridDepth = 240f;
        public static float gridBoxSize = 4f;
        public static bool centeredX = true;
        public static bool centeredY = false;
        public static bool centeredZ = true;

        public static string grid_height = "grid_height";
        public static string grid_width = "grid_width";
        public static string grid_depth = "grid_depth";
        public static string grid_box_size = "grid_box_size";
        public static string grid_centered_x = "grid_centered_x";
        public static string grid_centered_y = "grid_centered_y";
        public static string grid_centered_z = "grid_centered_z";

             "level_data": {
                "grid_centered_x": true,
                "grid_centered_y": false,
                "grid_centered_z": true,
                "grid_box_size": 4,
                "grid_width": 240,
                "grid_height": 1,
                "grid_depth": 240,
                "type": "3d"
             },

        */

        public virtual string type
        {
            get
            {
                return Get<string>(BaseDataObjectKeys.type, GameLevelRenderType.type_3d);
            }

            set
            {
                Set<string>(BaseDataObjectKeys.type, value);
            }
        }

        public virtual bool grid_centered_x
        {
            get
            {
                return Get<bool>(BaseDataObjectKeys.grid_centered_x, true);
            }

            set
            {
                Set<bool>(BaseDataObjectKeys.grid_centered_x, value);
            }
        }

        public virtual bool grid_centered_y
        {
            get
            {
                return Get<bool>(BaseDataObjectKeys.grid_centered_y, false);
            }

            set
            {
                Set<bool>(BaseDataObjectKeys.grid_centered_y, value);
            }
        }

        public virtual bool grid_centered_z
        {
            get
            {
                return Get<bool>(BaseDataObjectKeys.grid_centered_z, true);
            }

            set
            {
                Set<bool>(BaseDataObjectKeys.grid_centered_z, value);
            }
        }

        public virtual double grid_box_size
        {
            get
            {
                return Get<double>(BaseDataObjectKeys.grid_box_size, 4);
            }

            set
            {
                Set<double>(BaseDataObjectKeys.grid_box_size, value);
            }
        }

        public virtual double grid_depth
        {
            get
            {
                return Get<double>(BaseDataObjectKeys.grid_depth, 240);
            }

            set
            {
                Set<double>(BaseDataObjectKeys.grid_depth, value);
            }
        }

        public virtual double grid_width
        {
            get
            {
                return Get<double>(BaseDataObjectKeys.grid_width, 240);
            }

            set
            {
                Set<double>(BaseDataObjectKeys.grid_width, value);
            }
        }

        public virtual double grid_height
        {
            get
            {
                return Get<double>(BaseDataObjectKeys.grid_height, 1);
            }

            set
            {
                Set<double>(BaseDataObjectKeys.grid_height, value);
            }
        }

        public void Copy(GameLevelData from)
        {

            grid_depth = from.grid_depth;
            grid_height = from.grid_height;
            grid_width = from.grid_width;
            //
            grid_box_size = from.grid_box_size;
            //
            grid_centered_x = from.grid_centered_x;
            grid_centered_y = from.grid_centered_y;
            grid_centered_z = from.grid_centered_z;
        }
    }

    public class GameLevelDataObjectItem : GameDataObjectItem
    {

        public virtual GameLevelData level_data
        {
            get
            {
                return Get<GameLevelData>(BaseDataObjectKeys.level_data, new GameLevelData());
            }

            set
            {
                Set<GameLevelData>(BaseDataObjectKeys.level_data, value);
            }
        }
    }

    public class BaseGameLevel : GameDataObject
    {

        public virtual GameLevelDataObjectItem data
        {
            get
            {
                return Get<GameLevelDataObjectItem>(BaseDataObjectKeys.data);
            }

            set
            {
                Set<GameLevelDataObjectItem>(BaseDataObjectKeys.data, value);
            }
        }

        public BaseGameLevel()
        {
            Reset();
        }

        public override void Reset()
        {
            base.Reset();
        }

        // Attributes that are added or changed after launch should be like this to prevent
        // profile conversions.
    }
}