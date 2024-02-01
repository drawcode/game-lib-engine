using System;
using System.Collections.Generic;
using System.IO;
using Engine.Game.Data;

namespace Engine.Game.App.BaseApp
{
    public class BaseGameStatistics<T> : DataObjects<T> where T : DataObject, new()
    {
        private static T current;
        private static volatile BaseGameStatistics<T> instance;
        private static object syncRoot = new Object();
        public static string BASE_DATA_KEY = "game-statistics-data";

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

        public static BaseGameStatistics<T> BaseInstance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new BaseGameStatistics<T>(true);
                    }
                }

                return instance;
            }
        }

        public static string STAT_TOTAL_TIMES_PLAYED = "times-played";
        public static string STAT_TOTAL_TIME_PLAYED = "time-played";
        public static string STAT_TOTAL_WINS = "total-wins";
        public static string STAT_TOTAL_LOSSES = "total-losses";

        public BaseGameStatistics()
        {
            Reset();
        }

        public BaseGameStatistics(bool loadData)
        {
            Reset();
            path = "data/" + BASE_DATA_KEY + ".json";
            pathKey = BASE_DATA_KEY;
            LoadData();
        }

        public override void Reset()
        {
            base.Reset();
        }
    }

    public class BaseGameStatisticKeys
    {
        public static string order = "order";
        public static string store_count = "store_count";
    }

    public class BaseGameStatistic : GameDataObjectLocalized
    {
        public virtual string order
        {
            get
            {
                return Get<string>(BaseGameStatisticKeys.order);
            }

            set
            {
                Set(BaseGameStatisticKeys.order, value);
            }
        }

        public virtual int store_count
        {
            get
            {
                return Get<int>(BaseGameStatisticKeys.store_count);
            }

            set
            {
                Set(BaseGameStatisticKeys.store_count, value);
            }
        }

        // Attributes that are added or changed after launch should be like this to prevent
        // profile conversions.

        public BaseGameStatistic()
        {
            Reset();
        }

        public override void Reset()
        {
            base.Reset();
        }
    }
}