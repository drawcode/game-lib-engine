using System;
using System.Collections.Generic;
using System.IO;
using Engine.Game.Data;

using UnityEngine;

namespace Engine.Game.App.BaseApp
{
    public class BaseGameMinis<T> : DataObjects<T> where T : DataObject, new()
    {
        private static T current;
        private static volatile BaseGameMinis<T> instance;
        private static System.Object syncRoot = new System.Object();

        private string BASE_DATA_KEY = "game-mini-data";

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

        public static BaseGameMinis<T> BaseInstance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new BaseGameMinis<T>(true);
                    }
                }

                return instance;
            }
            set
            {
                instance = value;
            }
        }

        public BaseGameMinis()
        {
            Reset();
        }

        public BaseGameMinis(bool loadData)
        {
            Reset();
            path = "data/" + BASE_DATA_KEY + ".json";
            pathKey = BASE_DATA_KEY;
            LoadData();
        }

        public static GameObject Load(string code)
        {
            return AppContentAssets.LoadAsset(code);
            //return AppContentAssetModels.LoadModel(code);
        }

        public static GameObject LoadPrefab(string code)
        {
            return AppContentAssets.LoadAssetPrefab(code);
            //return AppContentAssetModels.LoadPrefab(code);
        }
    }

    public class GameMini : GameDataObject
    {
        //public virtual List<string> roles {
        //    get {
        //        return Get<List<string>>(BaseDataObjectKeys.roles);
        //    }

        //    set {
        //        Set<List<string>>(BaseDataObjectKeys.roles, value);
        //    }
        //} 

        //public virtual List<GameDataModel> models {
        //    get {
        //        return Get<List<GameDataModel>>(BaseDataObjectKeys.models);
        //    }

        //    set {
        //        Set<List<GameDataModel>>(BaseDataObjectKeys.models, value);
        //    }
        //} 
    }

    /*
    "character_data": {
                "roles": ["hero","enemy","sidekick"],
                "models" : [
                    {
                        "type": "character",
                        "code": "character-boy-1",
                        "textures": "character-boy1-default",
                        "colors":"game-college-baylor-bears-home"
                    }
                ]
            }
    }
    */

    public class BaseGameMini : GameDataObjectMeta
    {
        // Attributes that are added or changed after launch should be like this to prevent
        // profile conversions.

        public BaseGameMini()
        {
            Reset();
        }

        public override void Reset()
        {
            base.Reset();
        }

        public void Clone(BaseGameMini toCopy)
        {
            base.Clone(toCopy);
        }

        public GameObject Load()
        {
            //foreach(GameDataModel model in data.models) {
            //    return GameCharacters.Load(model.code);
            //}
            return null;
        }

        public GameObject LoadPrefab()
        {
            //foreach(GameDataModel model in data.models) {
            //    return GameCharacters.LoadPrefab(model.code);
            //}
            return null;
        }

        // Attributes that are added or changed after launch should be like this to prevent
        // profile conversions.
    }
}