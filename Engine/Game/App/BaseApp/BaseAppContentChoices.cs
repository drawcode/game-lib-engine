using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Engine.Content;
using Engine.Game.Data;

namespace Engine.Game.App.BaseApp
{
    public class BaseAppContentChoices<T> : DataObjects<T> where T : DataObject, new()
    {
        private static T current;
        private static volatile BaseAppContentChoices<T> instance;
        private static object syncRoot = new Object();
        private string BASE_DATA_KEY = "app-content-choice-data";

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

        public static BaseAppContentChoices<T> BaseInstance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new BaseAppContentChoices<T>(true);
                    }
                }

                return instance;
            }
            set
            {
                instance = value;
            }
        }

        public BaseAppContentChoices()
        {
            Reset();
        }

        public BaseAppContentChoices(bool loadData)
        {
            Reset();
            path = "data/" + BASE_DATA_KEY + ".json";
            pathKey = BASE_DATA_KEY;
            LoadData();
        }

        public void ChangeState(string code)
        {
            if (AppContentChoices.Current.code != code)
            {
                AppContentChoices.Current = AppContentChoices.Instance.GetById(code);
            }
        }

        public List<AppContentChoice> GetListByCodeAndPackCode(string assetCode, string packCode)
        {
            List<AppContentChoice> filteredList = new List<AppContentChoice>();
            foreach (AppContentChoice obj in AppContentChoices.Instance.GetListByPack(packCode))
            {
                if (assetCode.ToLower() == obj.code.ToLower())
                {
                    filteredList.Add(obj);
                }
            }

            return filteredList;
        }

        public string GetAppContentChoiceContentPath(string packCode, string asset, bool versioned)
        {
            string packPath = PathUtil.Combine(
                ContentPaths.appCachePathSharedPacks,
                packCode);

            string packPathContent = PathUtil.Combine(
                packPath,
                ContentConfig.currentContentContent);

            string file = "";
            AppContentChoice appContentAsset = AppContentChoices.Instance.GetById(asset);
            if (appContentAsset != null)
            {
                file = appContentAsset.code
                    + "."
                    + appContentAsset.GetVersionFileExt();
            }

            string fullPath = PathUtil.Combine(packPathContent, file);
            if (versioned)
            {
                fullPath = Contents.GetFullPathVersioned(fullPath);
            }
            return fullPath;
        }

        public bool CheckChoiceUser(string code, List<AppContentChoiceItem> choices)
        {

            if (choices == null)
            {
                return false;
            }

            if (choices.Count > 0)
            {
                return false;
            }

            foreach (AppContentChoiceItem item in choices)
            {
                if (item.code.ToLower() == code.ToLower())
                {
                    return true;
                }
            }
            return false;
        }

        public bool CheckChoices(string choiceCode, List<AppContentChoiceItem> choices, bool correctOnly)
        {

            if (choices == null)
            {
                return false;
            }

            if (choices.Count > 0)
            {
                return false;
            }

            AppContentChoice choice = AppContentChoices.Instance.GetByCode(choiceCode);
            if (choice != null)
            {
                foreach (AppContentChoiceItem choiceItem in choice.choices)
                {

                    if (correctOnly)
                    {
                        if (choiceItem.type != AppContentChoiceType.correct)
                        {
                            return false;
                        }
                    }

                    if (!CheckChoiceUser(choiceItem.code, choices))
                    {
                        return false;
                    }
                }
            }
            else
            {
                return false;
            }

            if (choices.Count == choice.choices.Count)
            {
                return true;
            }

            return false;
        }
    }

    public class AppContentChoiceAttributes
    {
        public static string version_file_increment = "version_file_increment";
        public static string version = "version";
        public static string version_required_app = "version_required_app";
        public static string version_type = "version_type";
        public static string version_file_ext = "version_file_ext";
        public static string version_file_type = "version_file_type";
        /*
        *
             "version_file_increment":"1",
             "version":"1.0",
             "version_required_app":"1.0",
             "version_type":"itemized"
             */
    }

    public class AppContentChoiceAttributesFileType
    {
        public static string videoType = "video";
        public static string audioType = "audio";
        public static string imageType = "image";
        public static string assetBundleType = "assetBundle";
    }

    public class AppContentChoiceAttributesFileExt
    {
        public static string videoM4vExt = "m4v";
        public static string videoMp4Ext = "mp4";
        public static string audioMp3Ext = "mp3";
        public static string audioWavExt = "wav";
        public static string imagePngExt = "png";
        public static string imageJpgExt = "jpg";
        public static string assetBundleExt = "unity3d";
    }

    /*
        {
         "sort_order":0,
         "sort_order_type":0,
         "attributes":null,
         "game_id":"63a15c20-294f-11e1-9314-0800200c9a66",
         "type":"resource",
         "key":"question",
         "order_by":"",
         "code":"question-concussion-1",
         "display_name":"Concussions only happen if a player is knocked out.",
         "name":"",
         "description":"Concussions can happen whether a player is knocked out or not. It is important to pay attention to any symptoms.",
         "status":"default",
         "uuid":"33f4b660-2955-11e1-9314-0800200c9a61",
         "active":true,
         "keys":["concussion"],
         "responses": [
             {"name": "true", "display":"True", type":"incorrect"},
             {"name": "false", "display":"False", "type":"correct"}
         ]
     }
     */

    public class AppContentChoiceMessage
    {
        public string choiceItemType = AppContentChoiceType.incorrect;
        public string choiceItemCode = "";
        public string choiceCode = "";
    }

    public class AppContentChoiceMessages
    {
        public static string appContentChoiceItem = "app-content-choice-item";
    }

    public class AppContentChoiceType
    {
        public static string incorrect = "incorrect";
        public static string correct = "correct";
        public static string warp = "warp";
        public static string portal = "portal";
        public static string boost = "boost";
        public static string door = "door";
        public static string level = "level";
        public static string custom = "custom";
    }

    public class AppContentChoiceItemKeys
    {
        public static string code = "code";
        public static string display = "display";
        public static string type = "type";
    }

    public class AppContentChoiceItem : DataObject
    {


        public virtual string code
        {
            get
            {
                return Get<string>(AppContentChoiceItemKeys.code, "");
            }

            set
            {
                Set(AppContentChoiceItemKeys.code, value);
            }
        }

        public virtual string display
        {
            get
            {
                return Get<string>(AppContentChoiceItemKeys.display, "");
            }

            set
            {
                Set(AppContentChoiceItemKeys.display, value);
            }
        }

        public virtual string type
        {
            get
            {
                return Get<string>(AppContentChoiceItemKeys.type, "");
            }

            set
            {
                Set(AppContentChoiceItemKeys.type, value);
            }
        }

        public AppContentChoiceItem()
        {
            Reset();
        }

        public override void Reset()
        {
            code = "";
            display = "";
            type = AppContentChoiceType.incorrect;
        }

        public bool IsTypeCorrect()
        {
            return type == AppContentChoiceType.correct ? true : false;
        }

        public bool IsTypeIncorrect()
        {
            return type == AppContentChoiceType.incorrect ? true : false;
        }

        public bool IsTypeBoost()
        {
            return type == AppContentChoiceType.boost ? true : false;
        }

        public bool IsTypeDoor()
        {
            return type == AppContentChoiceType.door ? true : false;
        }

        public bool IsTypeLevel()
        {
            return type == AppContentChoiceType.level ? true : false;
        }

        public bool IsTypeCustom()
        {
            return type == AppContentChoiceType.custom ? true : false;
        }

        public bool IsTypePortal()
        {
            return type == AppContentChoiceType.portal ? true : false;
        }

        public bool IsTypeWarp()
        {
            return type == AppContentChoiceType.warp ? true : false;
        }
    }

    public class AppContentChoicesDataKeys
    {
        public static string choicesCorrect = "choicesCorrect";
        public static string choices = "choices";
    }

    public class AppContentChoicesData : DataObject
    {

        public virtual double choicesCorrect
        {
            get
            {
                return Get<double>(AppContentChoicesDataKeys.choicesCorrect, 0);
            }

            set
            {
                Set(AppContentChoicesDataKeys.choicesCorrect, value);
            }
        }

        public virtual Dictionary<string, AppContentChoiceData> choices
        {
            get
            {
                return Get<Dictionary<string, AppContentChoiceData>>(
                    AppContentChoicesDataKeys.choices);
            }

            set
            {
                Set(AppContentChoicesDataKeys.choices, value);
            }
        }

        public AppContentChoicesData()
        {
            Reset();
        }

        public int GetChoicesCount()
        {
            CheckDicts();
            return choices.Count;
        }

        public int GetChoicesCorrect()
        {
            CheckDicts();

            int correct = 0;

            foreach (KeyValuePair<string, AppContentChoiceData> choiceData in choices)
            {
                if (choiceData.Value.CheckChoices(true))
                {
                    correct += 1;
                }
            }

            return correct;
        }

        public string GetChoiceScorePercentage()
        {
            return GetChoicesScore().ToString("P0");
        }

        public float GetChoicesScore()
        {
            return GetChoicesCorrect() / GetChoicesCount();
        }

        public override void Reset()
        {
            choices = new Dictionary<string, AppContentChoiceData>();
        }

        public void CheckDicts()
        {
            if (choices == null)
            {
                choices = new Dictionary<string, AppContentChoiceData>();
            }
        }

        public Dictionary<string, AppContentChoiceData> GetChoices()
        {
            CheckDicts();

            return choices;
        }

        public AppContentChoiceData GetChoice(string code)
        {
            CheckDicts();

            if (choices.ContainsKey(code))
            {
                return choices[code];
            }

            return null;
        }

        public void SetChoice(AppContentChoiceData choiceData)
        {
            if (choiceData == null)
            {
                return;
            }

            CheckDicts();

            if (choices.ContainsKey(choiceData.choiceCode))
            {
                choices[choiceData.choiceCode] = choiceData;
            }
            else
            {
                choices.Add(choiceData.choiceCode, choiceData);
            }
        }

        public void UpdateValues()
        {
            //foreach(KeyValuePair<string, AppContentChoiceData> pair in choices) {

            //}
        }
    }

    public class AppContentChoiceDataKeys
    {
        public static string choiceCode = "choiceCode";
        public static string choiceData = "choiceData";
        public static string choices = "choices";
    }

    public class AppContentChoiceData : DataObject
    {

        public virtual string choiceCode
        {
            get
            {
                return Get<string>(AppContentChoiceDataKeys.choiceCode, "");
            }

            set
            {
                Set(AppContentChoiceDataKeys.choiceCode, value);
            }
        }

        public virtual string choiceData
        {
            get
            {
                return Get<string>(AppContentChoiceDataKeys.choiceData, "");
            }

            set
            {
                Set(AppContentChoiceDataKeys.choiceData, value);
            }
        }

        public virtual List<AppContentChoiceItem> choices
        {
            get
            {
                return Get<List<AppContentChoiceItem>>(
                    AppContentChoiceDataKeys.choices);
            }

            set
            {
                Set(AppContentChoiceDataKeys.choices, value);
            }
        }

        public AppContentChoiceData()
        {
            Reset();
        }

        public override void Reset()
        {
            choiceCode = "";
            choiceData = "";
            choices = new List<AppContentChoiceItem>();
        }

        public bool CheckChoices(bool correctOnly)
        {
            return AppContentChoices.Instance.CheckChoices(choiceCode, choices, correctOnly);
        }

        public AppContentChoiceItem GetChoice(string choiceCode)
        {
            foreach (AppContentChoiceItem item in choices)
            {
                if (choiceCode == item.code)
                {
                    return item;
                }
            }
            return null;
        }

        public bool HasChoice(AppContentChoiceItem choiceItem)
        {
            foreach (AppContentChoiceItem item in choices)
            {
                if (item.code == choiceItem.code
                    && item.type == choiceItem.type)
                {
                    return true;
                }
            }
            return false;
        }

        public void SetChoice(AppContentChoiceItem choiceItem)
        {
            if (HasChoice(choiceItem))
            {
                for (int i = 0; i < choices.Count; i++)
                {
                    choices[i] = choiceItem;
                }
            }
            else
            {
                choices.Add(choiceItem);
            }
        }
    }

    public class BaseAppContentChoiceKeys
    {
        public static string tags = "tags";
        public static string app_states = "app_states";
        public static string AppContentStates = "app_content_states";
        public static string required_assets = "required_assets";
        public static string keys = "keys";
        public static string content_attributes = "content_attributes";
        public static string choices = "choices";
    }

    public class BaseAppContentChoice : GameDataObject
    {

        public virtual List<AppContentChoiceItem> choices
        {
            get
            {
                return Get<List<AppContentChoiceItem>>(BaseAppContentChoiceKeys.choices);
            }

            set
            {
                Set(BaseAppContentChoiceKeys.choices, value);
            }
        }

        // types: tracker, pack, data, generic

        // Attributes that are added or changed after launch should be like this to prevent
        // profile conversions.

        public BaseAppContentChoice()
        {
            Reset();
        }

        public override void Reset()
        {
            base.Reset();
            app_states = new List<string>();
            app_content_states = new List<string>();
            required_assets = new Dictionary<string, List<string>>();
            keys = new List<string>();
            content_attributes = new Dictionary<string, object>();
            choices = new List<AppContentChoiceItem>();
        }

        public bool HasTypeCorrect()
        {
            foreach (AppContentChoiceItem item in choices)
            {
                if (item.type.ToLower() == AppContentChoiceType.correct)
                {
                    return true;
                }
            }
            return false;
        }

        public bool HasTypeCorrectMultiple()
        {
            int answers = 0;
            foreach (AppContentChoiceItem item in choices)
            {
                if (item.type.ToLower() == AppContentChoiceType.correct)
                {
                    answers += 1;
                }
            }
            return answers > 0 ? true : false;
        }

        public string GetContentString(string key)
        {
            string content = "";
            if (content_attributes.ContainsKey(key))
            {
                content = content_attributes[key].ToString();
            }
            return content;
        }

        public string GetVersion()
        {
            return GetContentString(AppContentChoiceAttributes.version);
        }

        public string GetVersionFileIncrement()
        {
            return GetContentString(AppContentChoiceAttributes.version_file_increment);
        }

        public string GetVersionRequiredApp()
        {
            return GetContentString(AppContentChoiceAttributes.version_required_app);
        }

        public string GetVersionType()
        {
            return GetContentString(AppContentChoiceAttributes.version_type);
        }

        public string GetVersionFileType()
        {
            return GetContentString(AppContentChoiceAttributes.version_file_type);
        }

        public string GetVersionFileExt()
        {
            return GetContentString(AppContentChoiceAttributes.version_file_ext);
        }
    }
}