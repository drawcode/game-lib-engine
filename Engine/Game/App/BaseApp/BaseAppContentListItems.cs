using System;
using System.Collections.Generic;
using System.IO;
using Engine.Game.Data;

namespace Engine.Game.App.BaseApp
{
    public class BaseAppContentListItems<T> : DataObjects<T> where T : DataObject, new()
    {
        private static T current;
        private static volatile BaseAppContentListItems<T> instance;
        private static object syncRoot = new Object();

        private string BASE_DATA_KEY = "app-content-list-item-data";

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

        public static BaseAppContentListItems<T> BaseInstance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new BaseAppContentListItems<T>(true);
                    }
                }

                return instance;
            }
            set
            {
                instance = value;
            }
        }

        public BaseAppContentListItems()
        {
            Reset();
        }

        public BaseAppContentListItems(bool loadData)
        {
            Reset();
            path = "data/" + BASE_DATA_KEY + ".json";
            pathKey = BASE_DATA_KEY;
            LoadData();
        }
    }

    public class BaseAppContentListItem : GameDataObject
    {
        // type

        // Attributes that are added or changed after launch should be like this to prevent
        // profile conversions.

        public BaseAppContentListItem()
        {
            Reset();
        }

        public override void Reset()
        {
            base.Reset();
        }
    }
}