using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

using Engine.Data.Json;
using Engine.Utility;
using UnityEngine;

public class BaseGameLocalizationKeys {

    // APP

    public static string app_display_name = "app_display_name";
    public static string app_display_code = "app_display_code";
    public static string app_web_url = "app_web_url";
    public static string app_store_url_itunes = "app_store_url_itunes";
    public static string app_store_url_google_play = "app_store_url_google_play";
    public static string app_default_append = "app_default_append";

    // SOCIAL

    // -- TWITTER

    public static string social_twitter_post_message = "social_twitter_post_message";
    public static string social_twitter_disabled_title = "social_twitter_disabled_title";
    public static string social_twitter_disabled_message = "social_twitter_disabled_message";
    public static string social_twitter_upload_success_title = "social_twitter_upload_success_title";
    public static string social_twitter_upload_success_message = "social_twitter_upload_success_message";
    public static string social_twitter_upload_error_title = "social_twitter_upload_error_title";
    public static string social_twitter_upload_error_message = "social_twitter_upload_error_message";

    // -- FACEBOOK

    public static string social_facebook_post_message = "social_facebook_post_message";
    public static string social_facebook_upload_error_title = "social_facebook_upload_error_title";
    public static string social_facebook_upload_error_message = "social_facebook_upload_error_message";
    public static string social_facebook_upload_success_title = "social_facebook_upload_success_title";
    public static string social_facebook_upload_success_message = "social_facebook_upload_success_message";
    
    public static string social_facebook_game_action_append = "social_facebook_game_action_append";

    //social_facebook_game_action_append

    // -- PHOTO

    public static string social_photo_saved_photo_title = "social_photo_saved_photo_title";
    public static string social_photo_pending_creating_screenshot = "social_photo_pending_creating_screenshot";
    public static string social_photo_pending_uploading_post = "social_photo_pending_uploading_post";
    public static string social_photo_library_photo_saved_title = "social_photo_library_photo_saved_title";
    public static string social_photo_library_photo_saved_message = "social_photo_library_photo_saved_message";

    // -- GAME -- FACEBOOK

    public static string social_facebook_game_results_title = "social_facebook_game_results_title";
    public static string social_facebook_game_results_message = "social_facebook_game_results_message";
    public static string social_facebook_game_results_url = "social_facebook_game_results_url";
    public static string social_facebook_game_results_image_url = "social_facebook_game_results_image_url";
    public static string social_facebook_game_results_caption = "social_facebook_game_results_caption";
    
    // -- GAME -- TWITTER
    
    public static string social_twitter_game_results_message = "social_twitter_game_results_message";
    public static string social_twitter_game_results_image_url = "social_twitter_game_results_image_url";

    
    // -- GAME -- EVERYPLAY
    
    public static string social_everyplay_game_results_message = "social_everyplay_game_results_message";
    public static string social_everyplay_game_explore_message = "social_everyplay_game_explore_message";
    
    // -- GAME -- ADVERBS
    
    public static string game_action_adverb_1 = "game_action_adverb_1";
    public static string game_action_adverb_2 = "game_action_adverb_2";
    public static string game_action_adverb_3 = "game_action_adverb_3";
    public static string game_action_adverb_4 = "game_action_adverb_4";
    public static string game_action_adverb_5 = "game_action_adverb_5";
    public static string game_action_adverb_6 = "game_action_adverb_6";
    public static string game_action_adverb_7 = "game_action_adverb_7";

    
    public static string game_type_arcade = "game_type_arcade";
    public static string game_type_arcade_mode = "game_type_arcade_mode";

    
    public static string game_type_coins = "game_type_coins";
    public static string game_type_scores = "game_type_scores";
    public static string game_type_score = "game_type_score";
    
    public static string game_action_default_message = "game_action_default_message";
    public static string game_action_results_choice_quiz_message = "game_action_results_choice_quiz_message";    
    public static string game_action_results_arcade_message = "game_action_results_arcade_message";
    
    public static string game_action_panel_character_customize_message = "game_action_panel_character_customize_message";
    public static string game_action_panel_character_colors_message = "game_action_panel_character_colors_message";
    public static string game_action_panel_character_rpg_message = "game_action_panel_character_rpg_message";
    public static string game_action_panel_achievements_message = "game_action_panel_achievements_message";
    public static string game_action_panel_statistics_message = "game_action_panel_statistics_message";

}

public enum GameLocalizationDataItemType {
    strings,
    images
}

public class BaseGameLocalizations<T> : DataObjects<T> where T : DataObject, new() {
    private static T current;
    private static volatile BaseGameLocalizations<T> instance;
    private static System.Object syncRoot = new System.Object();
    public static string currentLocale = ""; //
    public static string defaultLocale = "en"; //

    public static string BASE_DATA_KEY = "game-localization-data";

    public static T BaseCurrent {
        get {
            if (current == null) {
                lock (syncRoot) {
                    if (current == null)
                        current = new T();
                }
            }

            return current;
        }
        set {
            current = value;
        }
    }

    public static BaseGameLocalizations<T> BaseInstance {
        get {
            if (instance == null) {
                lock (syncRoot) {
                    if (instance == null)
                        instance = new BaseGameLocalizations<T>(true);
                }
            }

            return instance;
        }
        set {
            instance = value;
        }
    }

    public BaseGameLocalizations() {
        Reset();
    }

    public BaseGameLocalizations(bool loadData) {
        //Reset();
        //LoadLocale(defaultLocale);
    }

    public void LoadLocale(string localeCode) {

        Reset();
                
        string localeSystem = Application.systemLanguage.ToString();
        
        Debug.Log("GameLocalizations:LoadLocale:" + " localeSystem:" + localeSystem);

        if (string.IsNullOrEmpty(localeCode)) {
            localeCode = defaultLocale;
        }
        
        Debug.Log("GameLocalizations:LoadLocale:" + " localeCode:" + localeCode);
                
        currentLocale = localeCode;

        path = "data/" + BASE_DATA_KEY + "-" + localeCode + ".json";
        
        Debug.Log("GameLocalizations:LoadLocale:" + " path:" + path);

        pathKey = BASE_DATA_KEY;
        LoadData();
    }

    public string ReplaceLocalized(string content) {
        
        //if (!GameConfigs.globalReady) {
        //    return content;
        //}
    
        //\{\{\s+(.*?)\s+\}\}
    
        // If string contains mustache/handlebars {{ [code] }} then get all 
        // matches and replace with localized content

        string regexTemplate = @"\{\{[ ]*\^[ ]*(.*?)[ ]*\}\}";
        
        if (content.RegexIsMatch(regexTemplate)) {
            
            MatchCollection matches = content.RegexMatches(regexTemplate);
            
            foreach (Match match in matches) {

                string valCodeMatch = match.Value;
                string valCodeGroup = match.Value;

                foreach (Group group in match.Groups) {
                    valCodeGroup = group.Value;
                }

                GameLocalization localization = GameLocalizations.Current;

                GameLocalizationData data = localization.data;

                if (data != null) {

                    GameLocalizationDataItem dataItem = null;

                    if (data.strings != null) {
                        if (data.strings.ContainsKey(valCodeGroup)) {
                            dataItem = data.strings.Get(valCodeGroup);
                        }
                    }
                    
                    string regexCode = @"(\{\{[ ]*\^[ ]*" + valCodeGroup + @"[ ]*\}\})"; //{{[ ]*(.*?)[ ]*}} //({{[ ]*app_display_name[ ]*}}.)
                    string replaceText = valCodeMatch; // replace it if not found to prevent recursion

                    if (dataItem != null) {
                        if (!string.IsNullOrEmpty(dataItem.valString)) { 
                            replaceText = dataItem.valString;
                        }
                    }                                        
                    
                    content = content.RegexMatchesReplace(regexCode, replaceText);

                }
            }
            
            // recurse
            content = ReplaceLocalized(content);
        }
        
        return content;

    }

    //

    public static string Babel(string stringToTranslate) {
        if (GameLocalizations.Instance != null) {
            //string locale = Application.systemLanguage.ToString();
            // TODO translate
        }
        return stringToTranslate;
    }
    
    public static string Get(string key) {
        return GetString(key);
    }

    public static string Get(string locale, string key) {
        return GetString(locale, key);
    }
    
    public static string GetString(string key) {
        return GetString(currentLocale, key);
    }

    public static string GetString(string locale, string key) {

        GameLocalizationDataItem item = GetDataItem(
            GameLocalizationDataItemType.strings, locale, key);

        if (item != null) {
            return GameLocalizations.Instance.ReplaceLocalized(item.valString);
        }

        return null;
    }

    public static string GetImage(string locale, string key) {
        
        GameLocalizationDataItem item = GetDataItem(
            GameLocalizationDataItemType.images, locale, key);
        
        if (item != null) {
            return item.valString;
        }
        
        return null;
    }

    public static GameLocalizationDataItem GetDataItem(
        GameLocalizationDataItemType itemType, string locale, string key) {

        //if(locale != currentLocale) {
        // check locale
        GameLocalizations.Instance.ChangeCurrent(locale);
        //}
                        
        GameLocalization localeObject = GameLocalizations.Current;
                
        if (localeObject != null) {
                
            GameLocalizationData localeData = localeObject.data;
                
            if (localeData != null) {

                if (itemType == GameLocalizationDataItemType.strings) {

                    return localeData.GetItemString(key);
                }
                else if (itemType == GameLocalizationDataItemType.images) {
                    return localeData.GetItemImage(key);
                }
            }
        }
        
        return null;
    }

    public void ChangeCurrent(string code) {

        //Debug.Log("ChangeCurrent:" + 
        //          " currentLocale:" + currentLocale +
        //          " GameLocalizations.Current.code:" + GameLocalizations.Current.code +
        //          " code:" + code +
        //          " IsLoaded:" + IsLoaded +
        //          " HasLoadedStrings:" + HasLoadedStrings
        //          );

        if (code != currentLocale
            || !IsLoaded
            || !HasLoadedStrings) {

            LoadLocale(code);

            GameLocalization obj = GameLocalizations.Instance.GetById(code);

            if (obj != null) {
                
                GameLocalizations.Current = obj;
            }  
        } 
    }

    public virtual bool HasLoadedStrings {
        get {

            if (GameLocalizations.Current.data == null) {
                return false;
            }

            if (GameLocalizations.Current.data.strings == null) {
                return false;
            }

            return GameLocalizations.Current.data.strings.Count > 0 ? true : false;
        }
    }
}

public class GameLocalizationDataItem : GameDataObject {
    // val - content
    // type - string, image, etc - default type
    // data_type - string, int, etc - default string

    public GameLocalizationDataItem() {
        val = "";
        type = "strings";
        val = "string";
    }
}

public class GameLocalizationData : GameDataObject {

    public virtual Dictionary<string, GameLocalizationDataItem> strings {
        get {
            return Get<Dictionary<string, GameLocalizationDataItem>>(BaseDataObjectKeys.strings);
        }

        set {
            Set(BaseDataObjectKeys.strings, value);
        }
    }

    public virtual Dictionary<string, GameLocalizationDataItem> images {
        get {
            return Get<Dictionary<string, GameLocalizationDataItem>>(BaseDataObjectKeys.images);
        }
        
        set {
            Set(BaseDataObjectKeys.images, value);
        }
    }

    public GameLocalizationDataItem GetItemString(string key) {

        if (strings != null) {
        
            return strings.Get(key);
        }

        return null;
    }

    public GameLocalizationDataItem GetItemImage(string key) {
        if (images != null) {
            return images.Get(key);
        }
        
        return null;
    }
}

public class BaseGameLocalization : GameDataObject {

    public virtual GameLocalizationData data {
        get {
            return Get<GameLocalizationData>(BaseDataObjectKeys.data);
        }
            
        set {
            Set(BaseDataObjectKeys.data, value);
        }
    }

    public BaseGameLocalization() {
        Reset();
    }

    public override void Reset() {

    }

}