using System;
using System.Collections.Generic;
using System.IO;
using Engine.Game.Data;
using Engine.Utility;

namespace Engine.Game.App.BaseApp
{
    public class BaseGameSkills<T> : DataObjects<T> where T : DataObject, new()
    {
        private static T current;
        private static volatile BaseGameSkills<T> instance;
        private static object syncRoot = new Object();
        public static string BASE_DATA_KEY = "game-skill-data";

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

        public static BaseGameSkills<T> BaseInstance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new BaseGameSkills<T>(true);
                    }
                }

                return instance;
            }
        }

        public BaseGameSkills()
        {
            Reset();
        }

        public BaseGameSkills(bool loadData)
        {
            Reset();
            path = "data/" + BASE_DATA_KEY + ".json";
            pathKey = BASE_DATA_KEY;
            LoadData();
        }
    }

    public class BaseGameSkill : GameDataObject
    {
        // Attributes that are added or changed after launch should be like this to prevent
        // profile conversions.

        public BaseGameSkill()
        {
            Reset();
        }

        public override void Reset()
        {
            base.Reset();
        }

        public void Clone(BaseGameSkill toCopy)
        {
            base.Clone(toCopy);
        }

        // Attributes that are added or changed after launch should be like this to prevent
        // profile conversions.
    }
}