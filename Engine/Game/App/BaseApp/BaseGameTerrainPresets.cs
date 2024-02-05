using System;
using System.Collections.Generic;
using System.IO;
using Engine.Game.Data;

namespace Engine.Game.App.BaseApp
{
    public class BaseGameTerrainPresets<T> : DataObjects<T> where T : DataObject, new()
    {
        private static T current;
        private static volatile BaseGameTerrainPresets<T> instance;
        private static object syncRoot = new Object();
        private string BASE_DATA_KEY = "game-terrain-preset-data";

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

        public static BaseGameTerrainPresets<T> BaseInstance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new BaseGameTerrainPresets<T>(true);
                    }
                }

                return instance;
            }
            set
            {
                instance = value;
            }
        }

        public BaseGameTerrainPresets()
        {
            Reset();
        }

        public BaseGameTerrainPresets(bool loadData)
        {
            Reset();
            path = "data/" + BASE_DATA_KEY + ".json";
            pathKey = BASE_DATA_KEY;
            LoadData();
        }
    }

    public class GameTerrainPresetItems : GamePresetItems<GameTerrainPresetItem>
    {

    }

    public class GameTerrainPresetItem : GamePresetItem
    {

    }

    public class BaseGameTerrainPreset : GameDataObject
    {
        // Attributes that are added or changed after launch should be like this to prevent
        // profile conversions.

        public virtual GamePresetItems<GameTerrainPresetItems> data
        {
            get
            {
                return Get<GamePresetItems<GameTerrainPresetItems>>(BaseDataObjectKeys.data);
            }

            set
            {
                Set(BaseDataObjectKeys.data, value);
            }
        }

        public BaseGameTerrainPreset()
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