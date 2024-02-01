using System;
using System.Collections.Generic;
using System.IO;
using Engine.Game.Data;

namespace Engine.Game.App.BaseApp
{
    public class BaseGameWeaponMaterials<T> : DataObjects<T> where T : DataObject, new()
    {
        private static T current;
        private static volatile BaseGameWeaponMaterials<T> instance;
        private static object syncRoot = new Object();
        private string BASE_DATA_KEY = "game-weapon-skin-data";

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

        public static BaseGameWeaponMaterials<T> BaseInstance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new BaseGameWeaponMaterials<T>(true);
                    }
                }

                return instance;
            }
            set
            {
                instance = value;
            }
        }

        public BaseGameWeaponMaterials()
        {
            Reset();
        }

        public BaseGameWeaponMaterials(bool loadData)
        {
            Reset();
            path = "data/" + BASE_DATA_KEY + ".json";
            pathKey = BASE_DATA_KEY;
            LoadData();
        }
    }

    public class BaseGameWeaponMaterial : GameDataObject
    {
        // Attributes that are added or changed after launch should be like this to prevent
        // profile conversions.

        public BaseGameWeaponMaterial()
        {
            Reset();
        }

        public override void Reset()
        {
            base.Reset();
        }

        public void Clone(BaseGameWeaponMaterial toCopy)
        {
            base.Clone(toCopy);
        }

        // Attributes that are added or changed after launch should be like this to prevent
        // profile conversions.
    }
}