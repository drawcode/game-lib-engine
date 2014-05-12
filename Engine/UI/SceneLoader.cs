using System.Collections;
using System.Collections.Generic;
using Engine.Utility;
using UnityEngine;

namespace Engine.UI {

    public class SceneLoader : GameObjectBehavior {
        public bool sceneTransitionStarted = false;
        public string currentSceneName = "";

        public static SceneLoader Instance;

        public static string SCENE_SPLASH = "UISceneSplash";
        public static string SCENE_MAIN = "UISceneMain";
        public static string SCENE_HELP = "UISceneHelp";
        public static string SCENE_RECORDS = "UISceneRecords";
        public static string SCENE_INFO = "UISceneInfo";
        public static string SCENE_RACE_MODE_SELECT = "UISceneRaceModeSelect";
        public static string SCENE_RACE_MODE_ARCADE = "UISceneRaceModeArcade";
        public static string SCENE_RACE_MODE_SERIES_PACK = "UISceneRaceModeSeriesPack";
        public static string SCENE_RACE_MODE_SERIES_PACKS = "UISceneRaceModeSeriesPacks";
        public static string SCENE_RACE_MODE_ENDLESS = "UISceneRaceModeEndless";
        public static string SCENE_RACE_START = "UISceneRaceStart";
        public static string SCENE_RACE_SETUP = "UISceneRaceSetup";
        public static string SCENE_RACE_RESULTS = "UISceneRaceResults";

        private void Awake() {
            if (Instance != null && this != Instance) {

                //There is already a copy of this script running
                Destroy(this);
                return;
            }

            Instance = this;

            currentSceneName = Application.srcValue.Replace(".unity3d", "");
        }

        public void LoadSceneTransition(string sceneName) {
            AnimateOut();
            LoadScene(sceneName, .3f);
        }

        public void LoadScene(string sceneName, float delay) {
            StartCoroutine(LoadSceneRoutine(sceneName, delay));
        }

        private IEnumerator LoadSceneRoutine(string sceneName, float delay) {
            yield return new WaitForSeconds(delay);
            LoadScene(sceneName);
        }

        public void LoadScene(string sceneName) {
            LogUtil.Log("Trying load scene: " + sceneName);
            LogUtil.Log("sceneTransitionStarted: " + sceneTransitionStarted);
            if (!sceneTransitionStarted) {
                sceneTransitionStarted = true;

                //TestFlight.LogSceneExit();
                //TestFlight.LogSceneLoading(sceneName);
                LogUtil.Log("Loading scene: " + sceneName);
                LoadScene(sceneName, true);
            }
        }

        public bool IsComingFromResults(string currentSceneName) {
            if (currentSceneName == SCENE_RACE_MODE_ENDLESS
                || currentSceneName == SCENE_RACE_MODE_SERIES_PACK
                || currentSceneName == SCENE_RACE_MODE_ARCADE
                || currentSceneName == SCENE_RACE_MODE_SELECT
                || currentSceneName == SCENE_RACE_RESULTS) {
                return true;
            }
            return false;
        }

        private void LoadScene(string sceneName, bool async) {
            currentSceneName = sceneName;

            // If scene is right save the profile

            if (IsComingFromResults(currentSceneName)) {

                //if(GameState.Instance != null) {
                //	GameState.SaveProfile();
                //}
            }

            if (async) {
                Application.LoadLevelAsync(sceneName);
            }
            else {
                Application.LoadLevel(sceneName);
            }
            LogUtil.Log("Loaded scene: " + sceneName);

            //foreach (UnityEngine.Object obj in GameObject.FindObjectsOfType(typeof(ScreenScaler))) {
            //ScreenScaler scaler = obj as ScreenScaler;
            //scaler.CheckScreenScale();
            //}

            //Invoke("ResetSceneContext", .4f);
        }

        public void ResetSceneContext() {
            sceneTransitionStarted = false;
        }

        public void LoadSceneMain() {
            LoadSceneTransition(SCENE_MAIN);
        }

        public void LoadSceneSplash() {
            LoadSceneTransition(SCENE_SPLASH);
        }

        public void LoadSceneHelp() {
            LoadSceneTransition(SCENE_HELP);
        }

        public void LoadSceneRecords() {
            LoadSceneTransition(SCENE_RECORDS);
        }

        public void LoadSceneInfo() {
            LoadSceneTransition(SCENE_INFO);
        }

        public void LoadSceneRaceSelect() {

            //if(GameDatas.Current.currentRaceMode == GameRaceMode.GAME_RACE_MODE_SERIES) {
            //	LoadSceneRaceModeSeriesPack();
            //}
            //else if(GameDatas.Current.currentRaceMode == GameRaceMode.GAME_RACE_MODE_ARCADE) {
            //	LoadSceneRaceModeArcade();
            //}
            //else if(GameDatas.Current.currentRaceMode == GameRaceMode.GAME_RACE_MODE_ENDLESS) {
            //	LoadSceneRaceModeEndless();
            //}
        }

        public void LoadSceneRaceModeSelect() {
            LoadSceneTransition(SCENE_RACE_MODE_SELECT);
        }

        public void LoadSceneRaceModeArcade() {
            LoadSceneTransition(SCENE_RACE_MODE_ARCADE);
        }

        public void LoadSceneRaceModeSeriesPack() {
            LoadSceneTransition(SCENE_RACE_MODE_SERIES_PACK);
        }

        public void LoadSceneRaceModeSeriesPacks() {
            LoadSceneTransition(SCENE_RACE_MODE_SERIES_PACKS);
        }

        public void LoadSceneRaceModeEndless() {
            LoadSceneTransition(SCENE_RACE_MODE_ENDLESS);
        }

        public void LoadSceneRaceStart() {
            LoadSceneTransition(SCENE_RACE_START);
        }

        public void LoadSceneRaceSetup() {

            //if(GameLoadingObject.Instance != null) {
            //	GameLoadingObject.Instance.readyToPlay = false;
            //	GameLoadingObject.Instance.ShowBackground();
            //	GameLoadingObject.Instance.ShowHelpTips();
            //	GameLoadingObject.Instance.ShowLoadingHelp();
            //	LoadSceneTransition(SCENE_RACE_SETUP);
            //}
        }

        public void LoadSceneRaceResults() {
            LoadSceneTransition(SCENE_RACE_RESULTS);
        }

        public void AnimateIn() {

            //GameLoadingObject.FadeIn();
        }

        public void AnimateOut() {

            //GameLoadingObject.FadeOut();
        }

        public void SlideObject(GameObject obj, float distanceX, float distanceY, float distanceZ, float time, float delay) {
            if (obj != null) {
                //obj.MoveFrom(new Vector3(distanceX,
                //            distanceY,
                //            distanceZ),
                //            time, delay);
            }
        }
    }
}