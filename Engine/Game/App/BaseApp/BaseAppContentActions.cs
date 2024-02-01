using System;
using System.Collections.Generic;
using System.IO;
using Engine.Game.Data;

namespace Engine.Game.App.BaseApp
{
    public class BaseAppContentActions<T> : DataObjects<T> where T : DataObject, new()
    {
        private static T current;
        private static volatile BaseAppContentActions<T> instance;
        private static object syncRoot = new Object();

        private string BASE_DATA_KEY = "app-content-action-data";

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

        public static BaseAppContentActions<T> BaseInstance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new BaseAppContentActions<T>(true);
                    }
                }

                return instance;
            }
            set
            {
                instance = value;
            }
        }

        public BaseAppContentActions()
        {
            Reset();
        }

        public BaseAppContentActions(bool loadData)
        {
            Reset();
            path = "data/" + BASE_DATA_KEY + ".json";
            pathKey = BASE_DATA_KEY;
            LoadData();
        }
    }

    public class BaseAppContentAction : GameDataObject
    {

        // type

        // Attributes that are added or changed after launch should be like this to prevent
        // profile conversions.

        public BaseAppContentAction()
        {
            Reset();
        }

        public override void Reset()
        {
            base.Reset();
        }
    }

    // ----------------------------------------------------------------------------
    // OVERRIDE TO CUSTOMIZE 

    public partial class AppContentAction : BaseAppContentAction
    {
        //public Dictionary<string, List<string>> content_attributes;   
        public Dictionary<string, AppContentSet> content_sets = new Dictionary<string, AppContentSet>();
        public List<AppToolTip> content_tooltips = new List<AppToolTip>();
        public List<AppDataDisplay> content_data = new List<AppDataDisplay>();
        public List<AppContentActionEvent> content_events = new List<AppContentActionEvent>();

        //  "app_states":["all"],
        // "actionStates":["all"],
        // "appTrackers":["rocket-kid"],
        //  "required_assets":["shuttle"]

        // Attributes that are added or changed after launch should be like this to prevent
        // profile conversions.

        public AppContentAction()
        {
            Reset();
        }

        public override void Reset()
        {
            base.Reset();
            //content_attributes = new Dictionary<string, List<string>>();
            if (content_data == null)
            {
                content_data = new List<AppDataDisplay>();
            }
            else
            {
                content_data.Clear();
            }

            if (content_tooltips == null)
            {
                content_tooltips = new List<AppToolTip>();
            }
            else
            {
                content_tooltips.Clear();
            }

            if (content_sets == null)
            {
                content_sets = new Dictionary<string, AppContentSet>();
            }
            else
            {
                content_sets.Clear();
            }

            if (content_events == null)
            {
                content_events = new List<AppContentActionEvent>();
            }
            else
            {
                content_events.Clear();
            }
        }

        /*
        public List<AppToolTip> GetContentTips(string key) {
            List<AppToolTip> items = new List<AppToolTip>();
            if(content_tooltips != null) {
                items = content_tooltips[key] as List<AppToolTip>;
            }
            return items;
        }


        public List<string> GetContentTips(string key) {
            List<string> items = new List<string>();
            if(content_attributes.ContainsKey(key)) {
                items = content_attributes[key];
            }
            return items;
        }
        */
        public AppContentSet GetCurrentContentSet()
        {
            return GetContentSet(AppStates.Current.code);
        }

        public AppContentSet GetContentSet(string app_state)
        {
            if (!string.IsNullOrEmpty(app_state))
            {
                if (content_sets != null)
                {
                    if (content_sets.Count > 0)
                    {

                        if (content_sets.ContainsKey(app_state))
                        {
                            if (content_sets[app_state] != null)
                            {
                                return content_sets[app_state];
                            }
                        }
                        else if (content_sets.ContainsKey("all"))
                        {
                            if (content_sets["all"] != null)
                            {
                                return content_sets["all"];
                            }
                        }
                        else if (content_sets.ContainsKey("*"))
                        {
                            if (content_sets["*"] != null)
                            {
                                return content_sets["*"];
                            }
                        }
                        else if (content_sets.ContainsKey("app-state-all"))
                        {
                            if (content_sets["app-state-all"] != null)
                            {
                                return content_sets["app-state-all"];
                            }
                        }
                        else
                        {
                            foreach (KeyValuePair<string, AppContentSet> contentSet in content_sets)
                            {
                                return contentSet.Value;
                            }
                        }
                    }
                }
            }
            return null;
        }
        /*  
            public List<string> GetContentList(string key) {
                List<string> items = new List<string>();
                if(content_attributes.ContainsKey(key)) {
                    items = content_attributes[key];
                }
                return items;
            }
                 */

        public List<AppToolTip> GetContentTipsList()
        {
            List<AppToolTip> items = new List<AppToolTip>();
            if (content_tooltips != null)
            {
                if (content_tooltips.Count > 0)
                {
                    items = content_tooltips;
                }
            }
            return items;
        }

        public List<AppDataDisplay> GetContentDataList()
        {
            List<AppDataDisplay> items = new List<AppDataDisplay>();
            if (content_data != null)
            {
                if (content_data.Count > 0)
                {
                    items = content_data;
                }
            }
            return items;
        }

        public object GetAppContentActionEventData(AppContentActionEvent filter)
        {
            if (filter != null)
            {
                object obj = filter.obj;
                if (obj != null)
                {
                    return obj;
                }
            }
            return null;
        }

        public List<AppContentActionEvent> GetAppContentActionEvents(string filterType)
        {
            List<AppContentActionEvent> filterList = new List<AppContentActionEvent>();
            if (content_events != null)
            {
                foreach (AppContentActionEvent o in GetAppContentActionEvents())
                {

                    object val = GetFieldValue(o, "type");
                    if (val != null)
                    {
                        if ((string)val == filterType)
                        {
                            filterList.Add(o);
                        }
                    }
                }
                return filterList;
            }
            return null;
        }

        public List<AppContentActionEvent> GetAppContentActionEvents()
        {
            if (content_events != null)
            {
                return content_events;
            }
            return null;
        }

        public List<T> GetAppContentActionEvent<T>(string filterType)
        {
            List<AppContentActionEvent> objs = GetAppContentActionEvents(filterType);
            List<T> ts = new List<T>();
            if (objs != null)
            {
                ts = new List<T>();
                foreach (AppContentActionEvent o in objs)
                {
                    string jsonData = "";
                    try
                    {
                        jsonData = o.obj.Replace("\\\"", "\"");
                        ts.Add(jsonData.FromJson<T>());
                    }
                    catch (Exception e)
                    {
                        LogUtil.Log("ERROR converting achievement filter: " + e + " ::: " + jsonData);
                    }
                }
            }
            return ts;
        }

        /*
        public List<T> GetAppContentActionEventTypes<T>(string type) {
            List<AppContentActionEventBase> items 
                = GetAppContentActionEvent<AppContentActionEventBase>(type);
            List<T> returnItems = new List<T>();
            foreach(AppContentActionEventBase item in items) {
                T returnItem = new T();
                returnItem.Fill(item);
                returnItems.Add(returnItem);
            }

            return returnItems;
        }
        */

        public List<AppContentActionEventPoint> GetAppContentActionEventPoints()
        {

            List<AppContentActionEventBase> items
                = GetAppContentActionEvent<AppContentActionEventBase>(
                    AppContentActionEventType.eventPoint);
            List<AppContentActionEventPoint> returnItems
                = new List<AppContentActionEventPoint>();
            foreach (AppContentActionEventBase item in items)
            {
                AppContentActionEventPoint returnItem = new AppContentActionEventPoint();
                returnItem.Fill(item);
                returnItems.Add(returnItem);
            }

            return returnItems;
        }

        public List<AppContentActionEventAudio> GetAppContentActionEventAudios()
        {
            List<AppContentActionEventBase> items
                = GetAppContentActionEvent<AppContentActionEventBase>(
                    AppContentActionEventType.eventAudio);
            List<AppContentActionEventAudio> returnItems
                = new List<AppContentActionEventAudio>();
            foreach (AppContentActionEventBase item in items)
            {
                AppContentActionEventAudio returnItem = new AppContentActionEventAudio();
                returnItem.Fill(item);
                returnItems.Add(returnItem);
            }

            return returnItems;
        }

        public List<AppContentActionEventVideo> GetAppContentActionEventVideos()
        {
            List<AppContentActionEventBase> items
                = GetAppContentActionEvent<AppContentActionEventBase>(
                    AppContentActionEventType.eventVideo);
            List<AppContentActionEventVideo> returnItems
                = new List<AppContentActionEventVideo>();
            foreach (AppContentActionEventBase item in items)
            {
                AppContentActionEventVideo returnItem = new AppContentActionEventVideo();
                returnItem.Fill(item);
                returnItems.Add(returnItem);
            }

            return returnItems;
        }

        public List<AppContentActionEventLink> GetAppContentActionEventLinks()
        {
            List<AppContentActionEventBase> items
                = GetAppContentActionEvent<AppContentActionEventBase>(
                    AppContentActionEventType.eventLink);
            List<AppContentActionEventLink> returnItems
                = new List<AppContentActionEventLink>();
            foreach (AppContentActionEventBase item in items)
            {
                AppContentActionEventLink returnItem = new AppContentActionEventLink();
                returnItem.Fill(item);
                returnItems.Add(returnItem);
            }

            return returnItems;
        }

        public List<AppContentActionEventTip> GetAppContentActionEventTips()
        {
            List<AppContentActionEventBase> items
                = GetAppContentActionEvent<AppContentActionEventBase>(
                    AppContentActionEventType.eventTip);
            List<AppContentActionEventTip> returnItems
                = new List<AppContentActionEventTip>();
            foreach (AppContentActionEventBase item in items)
            {
                AppContentActionEventTip returnItem = new AppContentActionEventTip();
                returnItem.Fill(item);
                returnItems.Add(returnItem);
            }

            return returnItems;
        }

        /*
        public List<string> GetAppStates() {
            return GetContentSet(AppContentStates.Current.code)(AppContentActionAttributes.app_states);
        }

        public string GetInitialAppState() {
            List<string> states = new List<string>();
            string initial = "";
            states = GetContentList(AppContentActionAttributes.app_states);
            if(states.Count > 0) {
                initial = states[0];
            }

            return initial;
        }   

        public List<string> GetAppContentStates() {
            return GetContentList(AppContentActionAttributes.app_content_states);
        }

        public string GetInitialAppContentState() {
            List<string> states = new List<string>();
            string initial = "";
            states = GetContentList(AppContentActionAttributes.app_content_states);
            if(states.Count > 0) {
                initial = states[0];
            }

            return initial;
        }

        public List<string> GetAppTrackers() {
            return GetContentList(AppContentActionAttributes.appTrackers);
        }

        public string GetInitialTracker() {
            List<string> states = new List<string>();
            string initial = "";
            states = GetContentList(AppContentActionAttributes.appTrackers);
            if(states.Count > 0) {
                initial = states[0];
            }

            return initial;
        }

        public bool IsInternalActionType() {
            return GetActionTypes().Contains("internal");
        }

        public bool IsGenericInternalActionType() {
            return GetActionTypes().Contains("generic-internal");
        }

        public List<string> GetActionTypes() {
            return GetContentList(AppContentActionAttributes.actionTypes);
        }

        public string GetInitialActionType() {
            List<string> states = new List<string>();
            string initial = "";
            states = GetContentList(AppContentActionAttributes.actionTypes);
            if(states.Count > 0) {
                initial = states[0];
            }

            return initial;
        }


        public List<string> GetActionStates() {
            return GetContentList(AppContentActionAttributes.actionStates);
        }

        public string GetInitialActionState() {
            List<string> states = new List<string>();
            string initial = "";
            states = GetContentList(AppContentActionAttributes.actionStates);
            if(states.Count > 0) {
                initial = states[0];
            }

            return initial;
        }

        public List<string> GetRequiredAssets() {
            return GetContentList(AppContentActionAttributes.required_assets);
        }

        public string GetInitialRequiredAsset() {
            List<string> states = new List<string>();
            string initial = "";
            states = GetContentList(AppContentActionAttributes.required_assets);
            if(states.Count > 0) {
                initial = states[0];
            }

            return initial;
        }


        public List<string> GetRequiredPacks() {
            List<string> required_packs = new List<string>();
            object obj = GetAttributeObjectValue(AppContentActionAttributes.required_packs);
            if(obj != null) {
                try {
                    required_packs = (List<string>)obj;
                }
                catch (Exception e) {
                    //LogUtil.Log("Attribute object conversion error:" + e);
                }
            }
            return required_packs;
        }
        */
    }
}