using System;
using System.Collections.Generic;
using System.IO;
using Engine.Game.Data;

namespace Engine.Game.App.BaseApp
{
    public class BaseGameWeaponTypes<T> : DataObjects<T> where T : DataObject, new()
    {
        private static T current;
        private static volatile BaseGameWeaponTypes<T> instance;
        private static object syncRoot = new Object();
        private string BASE_DATA_KEY = "game-weapon-type-data";

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

        public static BaseGameWeaponTypes<T> BaseInstance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new BaseGameWeaponTypes<T>(true);
                    }
                }

                return instance;
            }
            set
            {
                instance = value;
            }
        }

        public BaseGameWeaponTypes()
        {
            Reset();
        }

        public BaseGameWeaponTypes(bool loadData)
        {
            Reset();
            path = "data/" + BASE_DATA_KEY + ".json";
            pathKey = BASE_DATA_KEY;
        }

    }

    public class BaseGameWeaponType : GameDataObject
    {
        // Attributes that are added or changed after launch should be like this to prevent
        // profile conversions.

        public BaseGameWeaponType()
        {
            Reset();
        }

        public override void Reset()
        {
            base.Reset();
        }

        public void Clone(BaseGameWeaponType toCopy)
        {
            base.Clone(toCopy);
        }

        // Attributes that are added or changed after launch should be like this to prevent
        // profile conversions.
    }
}