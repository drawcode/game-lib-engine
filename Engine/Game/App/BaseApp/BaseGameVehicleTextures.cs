using System;
using System.Collections.Generic;
using System.IO;
using Engine.Game.Data;

namespace Engine.Game.App.BaseApp
{
    public class BaseGameVehicleTextures<T> : DataObjects<T> where T : DataObject, new()
    {
        private static T current;
        private static volatile BaseGameVehicleTextures<T> instance;
        private static object syncRoot = new Object();
        private string BASE_DATA_KEY = "game-vehicle-texture-data";

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

        public static BaseGameVehicleTextures<T> BaseInstance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new BaseGameVehicleTextures<T>(true);
                    }
                }

                return instance;
            }
            set
            {
                instance = value;
            }
        }

        public BaseGameVehicleTextures()
        {
            Reset();
        }

        public BaseGameVehicleTextures(bool loadData)
        {
            Reset();
            path = "data/" + BASE_DATA_KEY + ".json";
            pathKey = BASE_DATA_KEY;
            LoadData();
        }
    }

    public class BaseGameVehicleTexture : GameDataObject
    {
        // Attributes that are added or changed after launch should be like this to prevent
        // profile conversions.

        public BaseGameVehicleTexture()
        {
            Reset();
        }

        public override void Reset()
        {
            base.Reset();
        }

        public void Clone(BaseGameVehicleTexture toCopy)
        {
            base.Clone(toCopy);
        }

        // Attributes that are added or changed after launch should be like this to prevent
        // profile conversions.
    }
}