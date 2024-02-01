using System;
using System.Collections.Generic;
using System.IO;
using Engine.Game.Data;
using Engine.Utility;

using UnityEngine;

namespace Engine.Game.App.BaseApp
{

    public class BaseAppColorPresets<T> : DataObjects<T> where T : DataObject, new()
    {
        private static T current;
        private static volatile BaseAppColorPresets<T> instance;
        private static System.Object syncRoot = new System.Object();

        private string BASE_DATA_KEY = "app-color-preset-data";

        public static Dictionary<string, Color> cachedColors = new Dictionary<string, Color>();

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

        public static BaseAppColorPresets<T> BaseInstance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new BaseAppColorPresets<T>(true);
                    }
                }

                return instance;
            }
            set
            {
                instance = value;
            }
        }

        public BaseAppColorPresets()
        {
            Reset();
        }

        public BaseAppColorPresets(bool loadData)
        {
            Reset();
            path = "data/" + BASE_DATA_KEY + ".json";
            pathKey = BASE_DATA_KEY;
            LoadData();
        }


        public static Color GetColor(string code)
        {

            Color colorTo = Color.white;

            //if(GameConfigs.globalReady) {

            if (!cachedColors.ContainsKey(code))
            {

                AppColor color = AppColors.Instance.GetByCode(code);

                if (color != null)
                {
                    colorTo = color.GetColor();
                }

                cachedColors.Add(code, colorTo);
            }
            else
            {
                colorTo = cachedColors[code];
            }
            //}

            return colorTo;
        }

        public static string GetColorCodeByItemCode(string customItemCode)
        {
            return "";//List<AppColorPreset>
        }

        public static Color GetColorByItemCode(string customItemCode)
        { // helmet, jersey etc

            Color colorTo = Color.white;

            //if (GameConfigs.globalReady)
            //{

            if (!cachedColors.ContainsKey(customItemCode))
            {

                AppColor color = AppColors.Instance.GetByCode(customItemCode);

                if (color != null)
                {
                    colorTo = color.GetColor();
                }

                cachedColors.Add(customItemCode, colorTo);
            }
            else
            {
                colorTo = cachedColors[customItemCode];
            }
            //}

            return colorTo;
        }
    }

    public class BaseAppColorPreset : GameDataObject
    {

        // Attributes that are added or changed after launch should be like this to prevent
        // profile conversions.


        public virtual Dictionary<string, string> data
        {
            get
            {
                return Get<Dictionary<string, string>>(BaseDataObjectKeys.data);
            }

            set
            {
                Set(BaseDataObjectKeys.data, value);
            }
        }

        public BaseAppColorPreset()
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