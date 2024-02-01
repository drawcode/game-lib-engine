using System;
using System.Collections.Generic;
using System.IO;
using Engine.Game.Data;

namespace Engine.Game.App.BaseApp
{
    public class BaseGameCharacterPresets<T> : DataObjects<T> where T : DataObject, new()
    {
        private static T current;
        private static volatile BaseGameCharacterPresets<T> instance;
        private static object syncRoot = new Object();
        private string BASE_DATA_KEY = "game-character-preset-data";

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

        public static BaseGameCharacterPresets<T> BaseInstance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new BaseGameCharacterPresets<T>(true);
                    }
                }

                return instance;
            }
            set
            {
                instance = value;
            }
        }

        public BaseGameCharacterPresets()
        {
            Reset();
        }

        public BaseGameCharacterPresets(bool loadData)
        {
            Reset();
            path = "data/" + BASE_DATA_KEY + ".json";
            pathKey = BASE_DATA_KEY;
            LoadData();
        }
    }

    public class GameCharacterPresetItems : GamePresetItems<GameCharacterPresetItem>
    {

    }

    public class GameCharacterPresetItem : GamePresetItem
    {

    }

    /*
    public class GameCharacterPresetItems : GameDataObject {

        public virtual List<GameCharacterPresetItem> items {
            get {
                return Get<List<GameCharacterPresetItem>>(BaseDataObjectKeys.items);
            }

            set {
                Set<List<GameCharacterPresetItem>>(BaseDataObjectKeys.items, value);
            }
        }
    }


    public class GameCharacterPresetItem : GameDataObject {

    }
    */

    public class BaseGameCharacterPreset : GameDataObject
    {
        // Attributes that are added or changed after launch should be like this to prevent
        // profile conversions.

        public virtual GameCharacterPresetItems data
        {
            get
            {
                return Get<GameCharacterPresetItems>(BaseDataObjectKeys.data);
            }

            set
            {
                Set(BaseDataObjectKeys.data, value);
            }
        }

        public BaseGameCharacterPreset()
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