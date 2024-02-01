using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Engine.Game.App.BaseApp;
using Engine.Game.Data;


#if !UNITY_WEBPLAYER
using System.Reflection;
using System.IO;

#endif
using Engine.Events;

namespace Engine.Game.App
{
    public class AppContentActions : BaseAppContentActions<AppContentAction>
    {
        private static volatile AppContentAction current;
        private static volatile AppContentActions instance;
        private static object syncRoot = new System.Object();

        public static string DATA_KEY = "app-content-action-data";

        public static AppContentAction Current
        {
            get
            {
                if (current == null)
                {
                    lock (syncRoot)
                    {
                        if (current == null)
                            current = new AppContentAction();
                    }
                }

                return current;
            }
            set
            {
                current = value;
            }
        }

        public static AppContentActions Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new AppContentActions(true);
                    }
                }

                return instance;
            }
            set
            {
                instance = value;
            }
        }

        public AppContentActions()
        {
            Reset();
            //ChangeState(APP_STATE_BOOKS);
        }

        public AppContentActions(bool loadData)
        {
            Reset();
            path = "data/" + DATA_KEY + ".json";
            pathKey = DATA_KEY;
            LoadData();
        }

        public void ChangeState(string code, string packCode)
        {
            if (Current.code != code)
            {
                foreach (AppContentAction action in GetListByCodeAndPackCode(code, packCode))
                {
                    Current = action;
                    break;
                }
            }
        }

        public AppContentSet GetContentSet(AppContentAction action, string app_state)
        {
            AppContentSet contentSet = null;
            if (action != null)
            {
                contentSet = action.GetContentSet("all");
                if (contentSet == null)
                {
                    contentSet = action.GetContentSet("app-state-all");
                }
                if (contentSet == null)
                {
                    contentSet = action.GetContentSet("*");
                }
                if (contentSet == null)
                {
                    contentSet = action.GetContentSet(app_state);
                }
            }
            return contentSet;
        }

        public AppContentActionEventPointInfo GetAppContentActionEventPointInfo(
            string appPack, string app_state, string app_content_state)
        {
            AppContentActionEventPointInfo eventInfo = new AppContentActionEventPointInfo();

            List<AppContentAction> actions = GetListByPackAndStateAndContentState(
                appPack, app_state, app_content_state);

            foreach (AppContentAction action in actions)
            {

                List<AppContentActionEventPoint> points = action.GetAppContentActionEventPoints();

                foreach (AppContentActionEventPoint point in points)
                {
                    double pointValue = point.points;
                    eventInfo.totalEvents += pointValue;

                    /*
                    GamePlayerProgressPointData pointData 
                        = GameProfileRPGs.Current.GetGamePlayerProgressPointData(point.uuid);

                    if(pointData.collected) {
                        eventInfo.totalEventsCompleted += pointValue;
                    }
                    */
                }
            }

            return eventInfo;
        }

        public List<AppContentActionEvent> GetActionEvents(
            string packCode, string app_state, string app_content_state)
        {

            List<AppContentActionEvent> eventList = new List<AppContentActionEvent>();

            List<AppContentAction> actionList = GetListByPackAndStateAndContentState(
                packCode, app_state, app_content_state);

            foreach (AppContentAction action in actionList)
            {
                List<AppContentActionEvent> actionEvents = action.GetAppContentActionEvents();
                eventList.AddRange(actionEvents);
            }

            return eventList;
        }

        public List<AppContentActionEvent> GetActionEventPoints(
            string packCode, string app_state, string app_content_state)
        {

            List<AppContentActionEvent> eventList = new List<AppContentActionEvent>();

            List<AppContentAction> actionList = GetListByPackAndStateAndContentState(
                packCode, app_state, app_content_state);

            foreach (AppContentAction action in actionList)
            {
                List<AppContentActionEvent> actionEvents
                    = action.GetAppContentActionEvents(AppContentActionEventType.eventPoint);
                eventList.AddRange(actionEvents);
            }

            return eventList;
        }

        public List<AppContentAction> GetListByPackAndState(string packCode, string app_state)
        {
            List<AppContentAction> filteredList = new List<AppContentAction>();
            List<AppContentAction> actions = GetListByPack(packCode);
            if (actions != null)
            {
                foreach (AppContentAction obj in actions)
                {
                    if (obj != null)
                    {
                        AppContentSet contentSet = GetContentSet(obj, app_state);

                        if (contentSet != null)
                        {
                            if (packCode.ToLower() == obj.pack_code.ToLower()
                                && (contentSet.GetAppStates().Contains(app_state.ToLower())
                                || contentSet.GetAppStates().Contains("*")
                                || contentSet.GetAppStates().Contains("all")
                                || contentSet.GetInitialAppState().IndexOf("all") > -1))
                            {
                                filteredList.Add(obj);
                            }
                        }
                    }
                }
            }
            return filteredList;
        }

        public List<AppContentAction> GetListByPackAndStateAndContentState(
            string packCode, string app_state, string app_content_state)
        {
            List<AppContentAction> filteredList = new List<AppContentAction>();
            List<AppContentAction> actions = GetListByPack(packCode);
            if (actions != null)
            {
                foreach (AppContentAction obj in actions)
                {
                    if (obj != null)
                    {
                        AppContentSet contentSet = GetContentSet(obj, app_state);

                        if (contentSet != null)
                        {

                            List<string> app_states = contentSet.GetAppStates();
                            List<string> app_content_states = contentSet.GetAppContentStates();

                            if (packCode.ToLower() == obj.pack_code.ToLower()
                                && (app_states.Contains(app_state.ToLower())
                                || app_states.Contains("*")
                                || app_states.Contains("all")
                                || contentSet.GetInitialAppState().IndexOf("all") > -1))
                            {

                                if (app_content_states.Contains(app_content_state)
                                    || app_content_states.Contains("*")
                                    || app_content_states.Contains("all"))
                                {
                                    filteredList.Add(obj);
                                }
                            }
                        }
                    }
                }
            }
            return filteredList;
        }

        /*
        public List<AppContentAction> GetListByPackAndContentState(string packCode, string app_content_state) {
            List<AppContentAction> filteredList = new List<AppContentAction>();
            foreach(AppContentAction obj in GetAll()) {
                if(packCode.ToLower() == obj.pack_code.ToLower()
                    && obj.GetCurrentContentSet().GetAppContentStates().Contains(app_content_state.ToLower())) {
                    filteredList.Add(obj);
                }
            }

            return filteredList;
        }
        */

        public List<string> GetListTrackerCodesByPackAndAppState(string packCode, string app_state)
        {

            //LogUtil.Log("GetListTrackerCodesByPackAndState:" + " packCode:" + packCode + " app_state:" + app_state);

            List<string> filteredList = new List<string>();
            foreach (AppContentAction obj in GetListByPackAndState(packCode, app_state))
            {
                //LogUtil.Log("GetListTrackerCodesByPackAndState:" + " obj.code:" + obj.code + " objGetAppTrackers:" + obj.GetAppTrackers()[0]);

                AppContentSet contentSet = GetContentSet(obj, app_state);

                if (contentSet != null)
                {
                    foreach (string s in contentSet.GetAppTrackers())
                    {
                        if (!filteredList.Contains(s))
                        {
                            filteredList.Add(s);
                        }
                    }
                }
            }

            return filteredList;
        }

        public List<string> GetListTrackerCodesByPackAndAppContentState(
            string packCode, string app_state, string app_content_state)
        {

            //LogUtil.Log("GetListTrackerCodesByPackAndState:" + " packCode:" + packCode + " app_state:" + app_state);

            List<string> filteredList = new List<string>();
            foreach (AppContentAction obj in GetListByPackAndStateAndContentState(
                packCode, app_state, app_content_state))
            {
                //LogUtil.Log("GetListTrackerCodesByPackAndState:" + " obj.code:" + obj.code + " objGetAppTrackers:" + obj.GetAppTrackers()[0]);

                AppContentSet contentSet = GetContentSet(obj, app_state);

                if (contentSet != null)
                {
                    foreach (string s in contentSet.GetAppTrackers())
                    {
                        if (!filteredList.Contains(s))
                        {
                            filteredList.Add(s);
                        }
                    }
                }
            }

            return filteredList;
        }

        /*
        public List<string> GetListTrackerCodesByPackAndAppContentState(string packCode, string app_content_state) {

            //LogUtil.Log("GetListTrackerCodesByPackAndState:" + " packCode:" + packCode + " app_state:" + app_state);

            List<string> filteredList = new List<string>();
            foreach(AppContentAction obj in GetListByPackAndContentState(packCode, app_content_state)) {
                //LogUtil.Log("GetListTrackerCodesByPackAndState:" + " obj.code:" + obj.code + " objGetAppTrackers:" + obj.GetAppTrackers()[0]);
                foreach(string s in obj.GetCurrentContentSet().GetAppTrackers()) {
                    filteredList.Add(s);
                }
            }

            return filteredList;
        }
    */

        public List<AppContentAction> GetListByCodeAndPackCode(string actionCode, string packCode)
        {
            List<AppContentAction> filteredList = new List<AppContentAction>();
            foreach (AppContentAction obj in GetListByPack(packCode))
            {
                if (actionCode.ToLower() == obj.code.ToLower())
                {
                    filteredList.Add(obj);
                }
            }

            return filteredList;
        }

        public List<AppContentAction> GetByTrackerCode(string trackerCode, bool onlyThisPack)
        {
            List<AppContentAction> filtered = new List<AppContentAction>();

            List<AppContentAction> currentSet;

            //if(onlyThisPack) {
            //  currentSet = AppContentActions.Instance.GetByPackCode(GamePacks.Current.code);
            //}
            //else {
            currentSet = AppContentActions.Instance.GetAll();
            //}

            foreach (AppContentAction asset in currentSet)
            {

                AppContentSet contentSet = asset.GetCurrentContentSet();
                if (contentSet != null)
                {
                    if (contentSet.GetAppTrackers().Contains(trackerCode))
                    {
                        if (!onlyThisPack || (onlyThisPack && asset.pack_code == GamePacks.Current.code))
                        {
                            filtered.Add(asset);
                        }
                    }
                }
            }

            return filtered;
        }

        public bool CheckActionByAppState(AppContentAction action, string app_state)
        {

            AppContentSet contentSet = GetContentSet(action, app_state);

            if (contentSet != null)
            {

                if (contentSet.GetAppStates().Contains(app_state)
                    || contentSet.GetAppStates().Contains("*")
                    || contentSet.GetAppStates().Contains("all")
                    || contentSet.GetInitialAppState().IndexOf("all") > -1)
                {
                    return true;
                }
            }
            return false;
        }

        /*
        public bool CheckActionByAppContentState(AppContentAction action, string app_content_state) {
            if(action.GetCurrentContentSet().GetAppContentStates().Contains(app_content_state)
                || action.GetCurrentContentSet().GetAppContentStates().Contains("*")
                || action.GetCurrentContentSet().GetAppContentStates().Contains("all")
                || action.GetCurrentContentSet().GetInitialAppContentState().IndexOf("all") > -1) {
                    return true;
                }
            return false;
        }
        */

        public bool CheckActionByTrackerCode(AppContentAction action, string trackerCode)
        {

            AppContentSet contentSet = GetContentSet(action, AppStates.Current.code);

            if (contentSet != null)
            {
                if (contentSet.GetAppTrackers().Contains(trackerCode)
                    || contentSet.GetAppTrackers().Contains("*")
                    || contentSet.GetAppTrackers().Contains("all")
                    || contentSet.GetInitialTracker().IndexOf("all") > -1)
                {
                    return true;
                }
            }
            return false;
        }

        public List<AppContentAction> GetByTrackerCodeAndAppState(string trackerCode, string app_state, bool onlyThisPack)
        {
            List<AppContentAction> filtered = new List<AppContentAction>();

            List<AppContentAction> currentSet;

            if (onlyThisPack)
            {
                currentSet = AppContentActions.Instance.GetListByPack(GamePacks.Current.code);
            }
            else
            {
                currentSet = AppContentActions.Instance.GetAll();
            }

            foreach (AppContentAction action in currentSet)
            {
                if (CheckActionByAppState(action, app_state)
                    && CheckActionByTrackerCode(action, trackerCode))
                {

                    if (!filtered.Contains(action))
                    {
                        filtered.Add(action);
                    }

                }
            }

            return filtered;
        }

        public List<AppContentAction> GetByTrackerCodeAndAppStateAndAppContentState(
            string trackerCode, string app_state, string app_content_state)
        {
            return GetByTrackerCodeAndAppStateAndAppContentState(trackerCode, app_state, app_content_state, null);
        }

        public List<AppContentAction> GetByTrackerCodeAndAppStateAndAppContentState(
            string trackerCode, string app_state, string app_content_state, string packCode)
        {

            List<AppContentAction> filtered = new List<AppContentAction>();

            List<AppContentAction> currentSet;

            if (!string.IsNullOrEmpty(packCode))
            {
                currentSet = AppContentActions.Instance.GetListByPack(packCode);
            }
            else
            {
                currentSet = AppContentActions.Instance.GetAll();
            }

            foreach (AppContentAction action in currentSet)
            {
                if (CheckActionByAppState(action, app_state)
                    && CheckActionByTrackerCode(action, trackerCode))
                {

                    AppContentSet contentSet = action.GetContentSet(app_state);
                    List<string> app_content_states = contentSet.app_content_states;

                    if (app_content_states.Contains(app_content_state))
                    {
                        if (!filtered.Contains(action))
                        {
                            filtered.Add(action);
                        }
                    }

                }
            }

            return filtered;
        }
        /*
        public void LoadAppContentActionEvents(AppContentAction action) {
            List<AppContentActionEvent> contentEvents = 
                action.GetAppContentActionEvents();

            LogUtil.Log("ShowAllObjectComponents:contentEvents:", contentEvents);

            if(contentEvents != null) {

                LogUtil.Log("ShowAllObjectComponents:contentEvents found:", contentEvents.Count);

                List<AppContentActionEventPoint> points = action.GetAppContentActionEventPoints();          

                    LogUtil.Log("ShowAllObjectComponents:points found:", points.Count);

                    foreach(AppContentActionEventPoint point in points) {
                    if(points != null) {

                        // Check to make sure point isn't already redeemed in profile
                        // Add in points prefab at stat value

                        string uuid = point.uuid;
                        string title = point.title;
                        string description = point.description;
                        double pointsValue = point.points;

                        LogUtil.Log("ShowAllObjectComponents:uuid:", uuid);
                        LogUtil.Log("ShowAllObjectComponents:title:", title);
                        LogUtil.Log("ShowAllObjectComponents:description:", description);
                        LogUtil.Log("ShowAllObjectComponents:pointsValue:", pointsValue);

                    }
                }
            }   
        }
        */

        /*
        public List<AppContentAction> GetByTrackerCodeAndContentAppState(string trackerCode, string app_content_state, bool onlyThisPack) {
            List<AppContentAction> filtered = new List<AppContentAction>();

            List<AppContentAction> currentSet;

            if(onlyThisPack) {
                currentSet = AppContentActions.Instance.GetListByPack(GamePacks.Current.code);
            }
            else {
                currentSet = AppContentActions.Instance.GetAll();
            }


            foreach(AppContentAction action in currentSet) {
                if(CheckActionByAppContentState(action, app_content_state) 
                    && CheckActionByTrackerCode(action, trackerCode)) {

                    if(!filtered.Contains(action) && action != null) {
                        filtered.Add(action);
                    }

                }
            }

            return filtered;
        }
        */

        /*
        public void ChangeState(AppState app_stateTo) {
            if(lastAppState != app_stateTo) {
                app_state = app_stateTo;
                HandleStateChange();
            }
        }

        public void HandleStateChange() {
            if(app_state == AppState.StateNotSet) {
                OnAppStateNotStarted();
            }
            else if(app_state == AppState.StateBook) {
                OnAppStateBooks();
            }
            else if(app_state == AppState.StateCards) {
                OnAppStateCards();
            }
            else if(app_state == AppState.StateGames) {
                OnAppStateGames();
            }
        }

        public void OnAppStateNotStarted() {
            app_state = AppState.StateNotSet;
            appSubState = AppSubState.StateSubEnvironment;

            Messenger<AppState, AppSubState>.Broadcast(AppViewerAppControllerMessages.StateNotSet, app_state, appSubState);
        }

        public void OnAppStateBooks() {
            app_state = AppState.StateBook;
            appSubState = AppSubState.StateSubEnvironment;

            currentApp = new App();
            currentApp.appCode = "default";

            Messenger<AppState, AppSubState>.Broadcast(AppViewerAppControllerMessages.StateBooks, app_state, appSubState);
        }

        public void OnAppStateCards() {
            app_state = AppState.StateCards;
            appSubState = AppSubState.StateSubEnvironment;

            currentApp = new App();
            currentApp.appCode = "default";

            Messenger<AppState, AppSubState>.Broadcast(AppViewerAppControllerMessages.StateCards, app_state, appSubState);
        }

        public void OnAppStateGames() {
            app_state = AppState.StateCards;
            appSubState = AppSubState.StateSubEnvironment;

            currentApp = new App();
            currentApp.appCode = "default";

            Messenger<AppState, AppSubState>.Broadcast(AppViewerAppControllerMessages.StateGames, app_state, appSubState);
        }
        */
    }

    public class AppContentActionAttributes
    {
        public static string app_states = "app_states";
        public static string AppContentStates = "app_content_states";
        public static string actionStates = "actionStates";
        public static string appTrackers = "appTrackers";
        public static string required_assets = "required_assets";
        public static string actionTypes = "actionTypes";
    }

    public class AppToolTip
    {

        public double localPositionX = 0;
        public double localPositionY = 5;
        public double localPositionZ = 0;
        public double scaleX = 1;
        public double scaleY = 1;
        public double scaleZ = 1;
        public string title = "Loading...";
        public string type = "default";
        public string style = "default";
        public string parentObject = "default";
        public bool billboard = true;

        public AppToolTip()
        {

        }
    }

    public class AppDataDisplay
    {

        public double localPositionX = -1.5f;
        public double localPositionY = 0;
        public double localPositionZ = 0;
        public double scaleX = 1;
        public double scaleY = 1;
        public double scaleZ = 1;
        public string title = "Loading...";
        public string type = "default";
        public string style = "default";
        public string parentObject = "default";
        public bool billboard = true;
        public string displayType = "content"; // content or ui

        public List<AppDataDisplayItem> dataItems = new List<AppDataDisplayItem>();

        public AppDataDisplay()
        {

        }
    }

    public class AppDataDisplayItem
    {

        public string type = ""; // title or value
        public string key = ""; // for headers
        public string itemType = "1col";
        public string data1 = "";
        public string data2 = "";
        public string data3 = "";
        public string data4 = "";
        public string data5 = "";

        public AppDataDisplayItem()
        {

        }
    }

    public class Vector3Data
    {
        public double x = 0.0;
        public double y = 0.0;
        public double z = 0.0;

        public Vector2 GetVector2()
        {
            return new Vector2((float)x, (float)y);
        }

        public Vector3 GetVector3()
        {
            return new Vector3((float)x, (float)y, (float)z);
        }

        public Vector3Data()
        {

        }

        public Vector3Data(float xTo, float yTo, float zTo)
        {
            x = xTo;
            y = yTo;
            z = zTo;
        }

        public Vector3Data(Vector3 vec3)
        {
            FromVector3(vec3);
        }

        public Vector3 GetVector2(float x, float y)
        {
            return new Vector2(x, y);
        }

        public void Load(float x, float y, float z)
        {
            FromVector3(new Vector3(x, y, z));
        }

        public void FromVector3(Vector3 vec3)
        {
            x = vec3.x;
            y = vec3.y;
            z = vec3.z;
        }


    }

    public class Vector2Data
    {
        public double x = 0.0;
        public double y = 0.0;
        public double z = 0.0;

        public Vector2 GetVector2()
        {
            return new Vector2((float)x, (float)y);
        }
    }

    public class AppContentSetActionType
    {
        public static string actionTap = "tap";
        public static string actionSwipe = "swipe";
        public static string actionDrag = "drag";
        public static string actionPinchRotate = "pinchrotate";
        public static string actionPinch = "pinch";
        public static string actionDoubleTap = "doubletap";
        public static string actionNone = "none";
    }

    public class AppContentSet : DataObjectItem
    {

        public string type = "";
        public string actionType = "";
        public List<string> app_states = new List<string>();
        public List<string> app_content_states = new List<string>();
        public List<string> appTrackers = new List<string>();
        public List<string> required_assets = new List<string>();
        public List<string> actionStates = new List<string>();
        public Vector3Data rotate = new Vector3Data();
        public string actionAdvance = AppContentSetActionType.actionTap;
        public string actionRotate = AppContentSetActionType.actionDrag;
        public string actionZoom = AppContentSetActionType.actionPinch;
        public int sort_order = 0;
        public int sort_order_type = 0;
        public string loadType = "default";

        public List<string> GetAppStates()
        {
            return app_states;
        }

        public string GetInitialAppState()
        {
            List<string> states = new List<string>();
            string initial = "";
            states = app_states;
            if (states.Count > 0)
            {
                initial = states[0];
            }

            return initial;
        }

        public List<string> GetAppContentStates()
        {
            return app_content_states;
        }

        public string GetInitialAppContentState()
        {
            List<string> contentStates = new List<string>();
            string initial = "";
            contentStates = app_content_states;
            if (contentStates.Count > 0)
            {
                initial = contentStates[0];
            }

            return initial;
        }

        public List<string> GetAppTrackers()
        {
            return appTrackers;
        }

        public string GetInitialTracker()
        {
            List<string> states = new List<string>();
            string initial = "";
            states = appTrackers;
            if (states.Count > 0)
            {
                initial = states[0];
            }

            return initial;
        }

        public bool IsInternalActionType()
        {
            return actionType == "internal";
            // has internal type objects nexted
        }

        public bool IsGenericInternalActionType()
        {
            return actionType == "generic-internal";
            // has internal type objects that are not just app states
        }

        public List<string> GetActionTypes()
        {
            List<string> actionTypes = new List<string>();
            actionTypes.Add(actionType);
            return actionTypes;
        }

        public string GetInitialActionType()
        {
            List<string> states = new List<string>();
            string initial = "";
            states = GetActionTypes();
            if (states.Count > 0)
            {
                initial = states[0];
            }

            return initial;
        }

        public List<string> GetActionStates()
        {
            return app_states;
        }

        public string GetInitialActionState()
        {
            List<string> states = new List<string>();
            string initial = "";
            states = app_states;
            if (states.Count > 0)
            {
                initial = states[0];
            }

            return initial;
        }

        public List<string> GetRequiredAssets()
        {
            return required_assets;
        }

        public string GetInitialRequiredAsset()
        {
            List<string> states = new List<string>();
            string initial = "";
            states = required_assets;
            if (states.Count > 0)
            {
                initial = states[0];
            }

            return initial;
        }

    }

    // events display

    public class AppContentActionEventInfos
    {
        public Dictionary<string, AppContentActionEventInfo> infos
            = new Dictionary<string, AppContentActionEventInfo>();
    }

    public class AppContentActionEventInfo
    {
        public string eventType = "";
        public double totalEvents = 0;
        public double totalEventsCompleted = 0;
    }

    public class AppContentActionEventPointInfo : AppContentActionEventInfo
    {

        public AppContentActionEventPointInfo()
        {
            eventType = AppContentActionEventType.eventPoint;
        }
    }


    // --------------------------------------------------------------------------------

    public class AppActionObject
    {
        public string actionCode = "";
        public string packCode = "";
        public string bundleName = "";
        public string assetName = "";
        public string trackerCode = "";
        public GameObject actionObject;
#if ENABLE_FEATURE_AR
    public ARDataSetTracker tracker;
#endif
        public AppContentAction action;
        public AppContentAsset asset;
        public int actionIndex = 0;

#if ENABLE_AR_FULL
    public AppPlaceholderObject placeholderObject;
#endif
        public string eventsUuid = "";
        public List<AppContentActionEventAudio> eventsAudio;
        public List<AppContentActionEventVideo> eventsVideo;
        public List<AppContentActionEventLink> eventsLink;
        public List<AppContentActionEventPoint> eventsPoint;
        public List<AppContentActionEventTip> eventsTip;
        public List<AppContentActionEventAsset> eventsAsset;

        //public Dictionary<string,object> urls;// = new Dictionary<string, object>();

        public AppActionObject()
        {
            Reset();
        }

        public void Reset()
        {
            actionCode = "";
            packCode = "";
            bundleName = "";
            assetName = "";
            trackerCode = "";
#if ENABLE_FEATURE_AR
        tracker = null;
#endif
            actionObject = null;
            action = null;
            asset = null;
            actionIndex = 0;

#if ENABLE_AR_FULL
        placeholderObject = null;
#endif
            eventsUuid = UniqueUtil.CreateUUID4();
            eventsAudio = new List<AppContentActionEventAudio>();
            eventsVideo = new List<AppContentActionEventVideo>();
            eventsLink = new List<AppContentActionEventLink>();
            eventsPoint = new List<AppContentActionEventPoint>();
            eventsTip = new List<AppContentActionEventTip>();
            eventsAsset = new List<AppContentActionEventAsset>();
        }

        public bool hasAudioEvents
        {
            get
            {
                return eventsAudio.Count > 0 ? true : false;
            }
        }

        public bool hasVideoEvents
        {
            get
            {
                return eventsVideo.Count > 0 ? true : false;
            }
        }

        public bool hasLinkEvents
        {
            get
            {
                return eventsLink.Count > 0 ? true : false;
            }
        }

        public bool hasPointEvents
        {
            get
            {
                return eventsPoint.Count > 0 ? true : false;
            }
        }

        public bool hasTipEvents
        {
            get
            {
                return eventsTip.Count > 0 ? true : false;
            }
        }

        public bool hasAssetEvents
        {
            get
            {
                return eventsAsset.Count > 0 ? true : false;
            }
        }

        public bool HasEventActionTrigger(string eventName)
        {
            if (HasEventActionTriggerAsset(eventName))
                return true;
            if (HasEventActionTriggerAudio(eventName))
                return true;
            if (HasEventActionTriggerLink(eventName))
                return true;
            if (HasEventActionTriggerVideo(eventName))
                return true;
            if (HasEventActionTriggerPoint(eventName))
                return true;
            if (HasEventActionTriggerTip(eventName))
                return true;
            return false;
        }

        public bool HasEventActionTriggerTip(string eventName)
        {
            if (hasTipEvents)
            {
                foreach (AppContentActionEventTip eventAction in eventsTip)
                {
                    if (eventAction.triggerType.ToLower() == AppContentActionEventTriggerType.eventType.ToLower())
                    {
                        if (eventAction.triggerCode.ToLower() == eventName.ToLower())
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public bool HasEventActionTriggerAsset(string eventName)
        {
            if (hasAssetEvents)
            {
                foreach (AppContentActionEventAsset eventAction in eventsAsset)
                {
                    if (eventAction.triggerType.ToLower() == AppContentActionEventTriggerType.eventType.ToLower())
                    {
                        if (eventAction.triggerCode.ToLower() == eventName.ToLower())
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public bool HasEventActionTriggerAudio(string eventName)
        {
            if (hasAudioEvents)
            {
                foreach (AppContentActionEventAudio eventAction in eventsAudio)
                {
                    if (eventAction.triggerType.ToLower() == AppContentActionEventTriggerType.eventType.ToLower())
                    {
                        if (eventAction.triggerCode.ToLower() == eventName.ToLower())
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public bool HasEventActionTriggerLink(string eventName)
        {
            if (hasLinkEvents)
            {
                foreach (AppContentActionEventLink eventAction in eventsLink)
                {
                    if (eventAction.triggerType.ToLower() == AppContentActionEventTriggerType.eventType.ToLower())
                    {
                        if (eventAction.triggerCode.ToLower() == eventName.ToLower())
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public bool HasEventActionTriggerVideo(string eventName)
        {
            if (hasVideoEvents)
            {
                foreach (AppContentActionEventVideo eventAction in eventsVideo)
                {
                    if (eventAction.triggerType.ToLower() == AppContentActionEventTriggerType.eventType.ToLower())
                    {
                        if (eventAction.triggerCode.ToLower() == eventName.ToLower())
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }

        public bool HasEventActionTriggerPoint(string eventName)
        {
            if (hasPointEvents)
            {
                foreach (AppContentActionEventPoint eventAction in eventsPoint)
                {
                    if (eventAction.triggerType.ToLower() == AppContentActionEventTriggerType.eventType.ToLower())
                    {
                        if (eventAction.triggerCode.ToLower() == eventName.ToLower())
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }
    }

    // events base

    //public string triggerType = "";
    //public string code = "";
    //public string codeType = AppContentActionEventCodeType.equal;
    //public string compareType = StatEqualityTypeString.STAT_GREATER_THAN;
    //public double compareValue = 1.0;
    //public AppContentActionEventData includeKeys = new AppContentActionEventData();


    // --------------------------------------------------------------------------------

    public class AppContentActionEventData
    {

        public AppActionObject appActionObject = null;

        public string uuid = "";
        public string appContentActionEventType = ""; // audio, video, link, point, asset
        public string appContentActionEventCode = ""; // appAction code
        public string appContentActionEventTriggerType = ""; // trigger type i.e. event
        public string appContentActionEventTriggerCode = ""; // trigger code i.e. ActionStartup
        public string appContentActionEventValue = ""; // trigger code i.e. ActionStartup
        public string appContentActionEventObject = ""; // trigger code i.e. ActionStartup

        public object appActionEvent = null;

        public AppContentActionEventData()
        {
            Reset();
        }

        public void Reset()
        {
            appActionObject = null;
            uuid = "";
            appContentActionEventType = ""; // audio, video, link, point, asset
            appContentActionEventCode = ""; // appAction code
            appContentActionEventTriggerType = ""; // trigger type i.e. event
            appContentActionEventTriggerCode = ""; // trigger code i.e. ActionStartup
            appContentActionEventValue = ""; // trigger code i.e. ActionStartup

            appActionEvent = null;
        }

        public bool IsAudioType()
        {
            return appContentActionEventType == AppContentActionEventType.eventAudio ? true : false;
        }
        public bool IsAssetType()
        {
            return appContentActionEventType == AppContentActionEventType.eventAsset ? true : false;
        }
        public bool IsLinkType()
        {
            return appContentActionEventType == AppContentActionEventType.eventLink ? true : false;
        }
        public bool IsPointType()
        {
            return appContentActionEventType == AppContentActionEventType.eventPoint ? true : false;
        }
        public bool IsVideoType()
        {
            return appContentActionEventType == AppContentActionEventType.eventVideo ? true : false;
        }

        // app action events

        public bool IsAppActionEventPlay()
        {
            if (appContentActionEventTriggerCode.Contains(
                AppContentActionEventMessages.AppActionPlayContentObjectEvent))
            {
                return true;
            }
            return false;
        }

        public bool IsAppActionEventStop()
        {
            if (appContentActionEventTriggerCode.Contains(
                AppContentActionEventMessages.AppActionStopContentObjectEvent))
            {
                return true;
            }
            return false;
        }

        public bool IsAppActionEventPause()
        {
            if (appContentActionEventTriggerCode.Contains(
                AppContentActionEventMessages.AppActionPauseContentObjectEvent))
            {
                return true;
            }
            return false;
        }

        public bool IsAppActionEventRestart()
        {
            if (appContentActionEventTriggerCode.Contains(
                AppContentActionEventMessages.AppActionRestartContentObjectEvent))
            {
                return true;
            }
            return false;
        }

        public bool IsAppActionEventReset()
        {
            if (appContentActionEventTriggerCode.Contains(
                AppContentActionEventMessages.AppActionResetContentObjectEvent))
            {
                return true;
            }
            return false;
        }

        public bool IsAppActionEventNext()
        {
            if (appContentActionEventTriggerCode.Contains(
                AppContentActionEventMessages.AppActionNextContentObjectEvent))
            {
                return true;
            }
            return false;
        }

        public bool IsAppActionEventPrevious()
        {
            if (appContentActionEventTriggerCode.Contains(
                AppContentActionEventMessages.AppActionPreviousContentObjectEvent))
            {
                return true;
            }
            return false;
        }

        public bool IsAppActionEventActionPrevious()
        {
            if (appContentActionEventTriggerCode.Contains(
                AppContentActionEventMessages.AppActionPreviousContentAppEvent))
            {
                return true;
            }
            return false;
        }

        public bool IsAppActionEventActionNext()
        {
            if (appContentActionEventTriggerCode.Contains(
                AppContentActionEventMessages.AppActionNextContentAppEvent))
            {
                return true;
            }
            return false;
        }
    }


    // --------------------------------------------------------------------------------

    /*
public class AppContentActionEventMessages {
    public static string AppActionTimelineEvent = "AppActionTimelineEvent";
    //
    public static string AppActionEvent = "AppActionEvent";
    public static string AppActionExternalLink = "AppActionExternalLink";
    public static string AppActionAudioEvent = "AppActionAudioEvent";
    public static string AppActionVideoEvent = "AppActionVideoEvent";
    public static string AppActionInternalLinkEvent = "AppActionInternalLinkEvent"; // TODO
    public static string AppActionPointEvent = "AppActionPointEvent";
    public static string AppActionAssetEvent = "AppActionAssetEvent";
    public static string AppActionTipEvent = "AppActionTipEvent";

    // Play custom content
    public static string AppActionPlayContentObjectEvent = "AppActionPlayContentObjectEvent";
    public static string AppActionStopContentObjectEvent = "AppActionStopContentObjectEvent";
    public static string AppActionResetContentObjectEvent = "AppActionResetContentObjectEvent";
    public static string AppActionPauseContentObjectEvent = "AppActionPauseContentObjectEvent";
    public static string AppActionNextContentObjectEvent = "AppActionNextContentObjectEvent";
    public static string AppActionPreviousContentObjectEvent = "AppActionPreviousContentObjectEvent";
    public static string AppActionRestartContentObjectEvent = "AppActionRestartContentObjectEvent";
    public static string AppActionNextContentAppEvent = "AppActionNextContentAppEvent";
    public static string AppActionPreviousContentAppEvent = "AppActionPreviousContentAppEvent";

    // Collectables

    public static string AppActionPointCollectEvent = "AppActionPointCollectEvent";

}
*/

    public class AppContentActionEventType
    {
        public static string eventPoint = "point";
        public static string eventLink = "link";
        public static string eventAudio = "audio";
        public static string eventVideo = "video";
        public static string eventAsset = "asset";
        public static string eventTip = "tip";
    }

    public class AppContentActionEventCodeType
    {
        public static string like = "like";
        public static string equal = "equal";
        public static string startsWith = "startsWith";
        public static string endsWith = "endsWith";
        public static string all = "all";
    }

    public class AppContentActionTipType
    {
        public static string controlsType = "control";
        public static string contentType = "content";
    }

    public class AppContentActionEventTriggerType
    {
        public static string statisticType = "statistic";
        public static string achievementType = "achievement";
        public static string pointsType = "points";
        public static string randomType = "random";
        public static string explicitType = "explicit";
        public static string eventType = "event";
    }

    public class AppContentActionEventTriggerActionType
    {
        public static string randomType = "random";
        public static string explicitType = "explicit";
    }

    public class AppContentActionEventActionType
    {
        public static string localType = "local";
        public static string remoteType = "remote";
    }

    public class AppContentActionEventDisplayType
    {
        public static string webviewType = "webview";
        public static string silentType = "silent";
    }

    public class AppContentActionEventMethod
    {
        public static string getType = "get";
        public static string postType = "post";
    }

    public class AppContentActionEventPlayType
    {
        public static string loop = "loop";
        public static string once = "once";
        public static string randomOnce = "randomOnce";
        public static string randomLoop = "randomLoop";
    }

    public class AppContentActionEventSoundType
    {
        public static string sound2d = "sound2d";
        public static string sound3d = "sound3d";
    }

    //AppContentActionEventSoundType

    public class AppContentActionEventPlayActionType
    {
        public static string autoplayType = "autoplay";
        public static string stoppedType = "stopped";
        public static string eventType = "event";
        public static string autoplayFullType = "autoplayFull";
        public static string stoppedFullType = "stoppedFull";
        public static string eventFullType = "eventFull";
    }

    public class AppContentActionEvent
    {
        public string type = "";
        public string obj = "";
    }

    public class AppContentActionEventBase
    {
        public Dictionary<string, object> data = new Dictionary<string, object>();

        public bool GetKeyValueBool(string key)
        {
            if (data.ContainsKey(key))
            {
                object o = data[key];
                return Convert.ToBoolean(o);
            }
            return false;
        }

        public string GetKeyValueString(string key)
        {
            if (data.ContainsKey(key))
            {
                object o = data[key];
                return Convert.ToString(o);
            }
            return "";
        }

        public double GetKeyValueDouble(string key)
        {
            if (data.ContainsKey(key))
            {
                object o = data[key];
                return Convert.ToDouble(o);
            }
            return 0.0;
        }

        public int GetKeyValueInt(string key)
        {
            if (data.ContainsKey(key))
            {
                object o = data[key];
                return Convert.ToInt32(o);
            }
            return 0;
        }
    }

    public class AppContentActionEventPoint
    {
        public string uuid = "";
        public string triggerType = AppContentActionEventTriggerType.statisticType;
        public string triggerCode = "";
        public string triggerCodeType = AppContentActionEventCodeType.like;
        public string triggerActionType = AppContentActionEventTriggerActionType.randomType;
        public double triggerCodeValue = 1.0;
        public double triggerDelay = 1.0;
        public string title = "";
        public string description = "";
        public double points = 1.0;
        public string asset = "";

        public void Fill(AppContentActionEventBase eventData)
        {
            uuid = eventData.GetKeyValueString("uuid");
            triggerType = eventData.GetKeyValueString("triggerType");
            triggerCode = eventData.GetKeyValueString("triggerCode");
            triggerCodeType = eventData.GetKeyValueString("triggerCodeType");
            triggerActionType = eventData.GetKeyValueString("triggerActionType");
            triggerCodeValue = eventData.GetKeyValueDouble("triggerCodeValue");
            triggerDelay = eventData.GetKeyValueDouble("triggerDelay");
            title = eventData.GetKeyValueString("title");
            description = eventData.GetKeyValueString("description");
            points = eventData.GetKeyValueDouble("points");
            asset = eventData.GetKeyValueString("asset");
        }
    }

    public class AppContentActionEventTip
    {
        public string uuid = "";
        public string triggerType = AppContentActionEventTriggerType.explicitType;
        public string triggerCode = "";
        public string triggerCodeType = AppContentActionEventCodeType.like;
        public string triggerActionType = AppContentActionEventTriggerActionType.explicitType;
        public double triggerCodeValue = 1.0;
        public double triggerDelay = 1.0;
        public string title = "";
        public string description = "";
        public string asset = "";
        public string type = "";

        public void Fill(AppContentActionEventBase eventData)
        {
            uuid = eventData.GetKeyValueString("uuid");
            triggerType = eventData.GetKeyValueString("triggerType");
            triggerCode = eventData.GetKeyValueString("triggerCode");
            triggerCodeType = eventData.GetKeyValueString("triggerCodeType");
            triggerActionType = eventData.GetKeyValueString("triggerActionType");
            triggerCodeValue = eventData.GetKeyValueDouble("triggerCodeValue");
            triggerDelay = eventData.GetKeyValueDouble("triggerDelay");
            title = eventData.GetKeyValueString("title");
            description = eventData.GetKeyValueString("description");
            asset = eventData.GetKeyValueString("asset");
            type = eventData.GetKeyValueString("type");
        }
    }

    public class AppContentActionEventAudio
    {
        public string uuid = "";
        public string triggerType = AppContentActionEventTriggerType.explicitType;
        public string triggerCode = "";
        public string triggerCodeType = AppContentActionEventCodeType.like;
        public string triggerActionType = AppContentActionEventTriggerActionType.explicitType;
        public double triggerCodeValue = 1.0;
        public double triggerDelay = 0.0;
        public string objectId = "";
        public string asset = "";
        public string playType = AppContentActionEventPlayType.loop;
        public double volume = 1.0;
        public double delay = 0.0;
        public string soundType = AppContentActionEventSoundType.sound2d;

        public void Fill(AppContentActionEventBase eventData)
        {
            uuid = eventData.GetKeyValueString("uuid");
            triggerType = eventData.GetKeyValueString("triggerType");
            triggerCode = eventData.GetKeyValueString("triggerCode");
            triggerCodeType = eventData.GetKeyValueString("triggerCodeType");
            triggerActionType = eventData.GetKeyValueString("triggerActionType");
            triggerCodeValue = eventData.GetKeyValueDouble("triggerCodeValue");
            triggerDelay = eventData.GetKeyValueDouble("triggerDelay");
            objectId = eventData.GetKeyValueString("objectId");
            asset = eventData.GetKeyValueString("asset");
            playType = eventData.GetKeyValueString("playType");
            volume = eventData.GetKeyValueDouble("volume");
            delay = eventData.GetKeyValueDouble("delay");
            soundType = eventData.GetKeyValueString("soundType");
        }
    }

    public class AppContentActionEventVideo
    {
        public string uuid = "";
        public string triggerType = AppContentActionEventTriggerType.eventType;
        public string triggerCode = "";
        public string triggerCodeType = AppContentActionEventCodeType.like;
        public string triggerActionType = AppContentActionEventTriggerActionType.explicitType;
        public double triggerCodeValue = 1.0;
        public double triggerDelay = 0.0;
        public string objectId = "";
        public string asset = "";
        public string url = "";  // remote or hd version
        public string actionType = AppContentActionEventActionType.localType;
        public string playType = AppContentActionEventPlayType.loop;
        public string playActionType = AppContentActionEventPlayActionType.autoplayType;
        public double volume = 1.0;
        public double delay = 0.0;
        public double seekPosition = 0.0;

        public void Fill(AppContentActionEventBase eventData)
        {
            uuid = eventData.GetKeyValueString("uuid");
            triggerType = eventData.GetKeyValueString("triggerType");
            triggerCode = eventData.GetKeyValueString("triggerCode");
            triggerCodeType = eventData.GetKeyValueString("triggerCodeType");
            triggerActionType = eventData.GetKeyValueString("triggerActionType");
            triggerCodeValue = eventData.GetKeyValueDouble("triggerCodeValue");
            triggerDelay = eventData.GetKeyValueDouble("triggerDelay");
            objectId = eventData.GetKeyValueString("objectId");
            asset = eventData.GetKeyValueString("asset");
            url = eventData.GetKeyValueString("url");
            actionType = eventData.GetKeyValueString("actionType");
            playType = eventData.GetKeyValueString("playType");
            playActionType = eventData.GetKeyValueString("playActionType");
            volume = eventData.GetKeyValueDouble("volume");
            delay = eventData.GetKeyValueDouble("delay");
            seekPosition = eventData.GetKeyValueDouble("seekPosition");
        }
    }

    public class AppContentActionEventLink
    {
        public string uuid = "";
        public string triggerType = AppContentActionEventTriggerType.eventType;
        public string triggerCode = "";
        public string triggerCodeType = AppContentActionEventCodeType.like;
        public string triggerActionType = AppContentActionEventTriggerActionType.explicitType;
        public double triggerCodeValue = 1.0;
        public double triggerDelay = 0.0;
        public string objectId = "";
        public string title = "";
        public string url = "";
        public string method = AppContentActionEventMethod.getType;
        public string actionType = AppContentActionEventActionType.remoteType;
        public string displayType = AppContentActionEventDisplayType.webviewType;
        public string values = "";
        public bool track = true;

        public void Fill(AppContentActionEventBase eventData)
        {
            uuid = eventData.GetKeyValueString("uuid");
            triggerType = eventData.GetKeyValueString("triggerType");
            triggerCode = eventData.GetKeyValueString("triggerCode");
            triggerCodeType = eventData.GetKeyValueString("triggerCodeType");
            triggerActionType = eventData.GetKeyValueString("triggerActionType");
            triggerCodeValue = eventData.GetKeyValueDouble("triggerCodeValue");
            triggerDelay = eventData.GetKeyValueDouble("triggerDelay");
            objectId = eventData.GetKeyValueString("objectId");
            url = eventData.GetKeyValueString("url");
            title = eventData.GetKeyValueString("title");
            method = eventData.GetKeyValueString("method");
            actionType = eventData.GetKeyValueString("actionType");
            displayType = eventData.GetKeyValueString("displayType");
            values = eventData.GetKeyValueString("values");
            track = eventData.GetKeyValueBool("track");
        }
    }

    public class AppContentActionEventAsset
    {
        public string uuid = "";
        public string triggerType = AppContentActionEventTriggerType.statisticType;
        public string triggerCode = "";
        public string triggerCodeType = AppContentActionEventCodeType.like;
        public string triggerActionType = AppContentActionEventTriggerActionType.explicitType;
        public double triggerCodeValue = 1.0;
        public double triggerDelay = 1.0;
        public string blurb = "";
        public double points = 1.0;
        public string asset = "";

        public void Fill(AppContentActionEventBase eventData)
        {
            uuid = eventData.GetKeyValueString("uuid");
            triggerType = eventData.GetKeyValueString("triggerType");
            triggerCode = eventData.GetKeyValueString("triggerCode");
            triggerCodeType = eventData.GetKeyValueString("triggerCodeType");
            triggerActionType = eventData.GetKeyValueString("triggerActionType");
            triggerCodeValue = eventData.GetKeyValueDouble("triggerCodeValue");
            triggerDelay = eventData.GetKeyValueDouble("triggerDelay");
            blurb = eventData.GetKeyValueString("blurb");
            points = eventData.GetKeyValueDouble("points");
            asset = eventData.GetKeyValueString("asset");
        }
    }

    public partial class AppContentAction : BaseAppContentAction
    {
        public override void Reset()
        {
            base.Reset();
        }
    }
}