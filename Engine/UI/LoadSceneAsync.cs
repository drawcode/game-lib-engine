using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.SceneManagement;

#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3
#else
using UnityEngine.UI;
#endif

using Engine.Events;

public class LoadSceneMessages {
    public static string LevelLoaded = "level-loaded";
    public static string LevelLoadProgress = "level-load-progress";
    public static string LevelLoadStarted = "level-load-started";
}

public enum LoadSceneState {
    LevelNotStarted,
    LevelLoaded,
    LevelLoadProgress,
    LevelLoadStarted
}

public class LoadSceneAsync : GameObjectBehavior {

    public AsyncOperation asyncLevelLoad;
    public LoadSceneState levelLoadState = LoadSceneState.LevelNotStarted;
    public float currentLoadedBytes = 0f;
    public string levelLoadingName = "";

    public bool running = false;

#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3
    public UISlider progressBarUI;
    public UILabel progressBarTextUI;
#else
    public Slider progressBarUI;
    public Text progressBarTextUI;
#endif


    public void Start() {
        if(running) {
            if(!string.IsNullOrEmpty(levelLoadingName)) {
                LoadLevel(levelLoadingName);
            }
        }
    }

    public void ChangeState(LoadSceneState loadSceneState) {
        // change fsm
        if(loadSceneState != levelLoadState) {
            levelLoadState = loadSceneState;
            // do stuff based on state change or fire and event.
        }
    }

    public void LoadLevel(string levelName) {
        running = true;
        levelLoadingName = levelName;
        ChangeState(LoadSceneState.LevelLoadStarted);
        StartCoroutine(LoadLevelHandlerCo());
    }

    public void OnEnable() {
        Messenger.AddListener(LoadSceneMessages.LevelLoaded, OnLevelLoaded);
        Messenger.AddListener(LoadSceneMessages.LevelLoadStarted, OnLevelLoadStarted);
        Messenger<float>.AddListener(LoadSceneMessages.LevelLoadProgress, OnLevelLoadProgress);
    }

    public void OnDisable() {
        Messenger.RemoveListener(LoadSceneMessages.LevelLoaded, OnLevelLoaded);
        Messenger.RemoveListener(LoadSceneMessages.LevelLoadStarted, OnLevelLoadStarted);
        Messenger<float>.RemoveListener(LoadSceneMessages.LevelLoadProgress, OnLevelLoadProgress);
    }

    public void OnLevelLoadProgress(float progress) {
        LogUtil.Log("Loading Progress:" + progress);
        ChangeState(LoadSceneState.LevelLoadProgress);
    }

    public void OnLevelLoadStarted() {
        LogUtil.Log("Loading Level Started:" + levelLoadingName);
        ChangeState(LoadSceneState.LevelLoadStarted);
    }

    public void OnLevelLoaded() {
        LogUtil.Log("Loading Level:" + levelLoadingName);
        ChangeState(LoadSceneState.LevelLoaded);
    }

    IEnumerator LoadLevelHandlerCo() {

        currentLoadedBytes = 0f;

        ChangeState(LoadSceneState.LevelLoadProgress);

        yield return new WaitForSeconds(2f);

        asyncLevelLoad = SceneManager.LoadSceneAsync(levelLoadingName);

        UIUtil.SetSliderValue(progressBarUI, 1f);
        UIUtil.SetLabelValue(progressBarTextUI, "100%");

        yield return new WaitForSeconds(2f);

        yield return asyncLevelLoad;

        running = false;

        yield return new WaitForSeconds(2f);

        //currentLoadedBytes = 1.0f;

        ChangeState(LoadSceneState.LevelLoaded);
        Messenger.Broadcast(LoadSceneMessages.LevelLoaded);
    }

    void Update() {

        if(running) {

            if(asyncLevelLoad == null) {
                return;
            }

            if(asyncLevelLoad.isDone) {
                UIUtil.SetSliderValue(progressBarUI, 1f);
                UIUtil.SetLabelValue(progressBarTextUI, "100%");
            }
            else {

                UIUtil.SetSliderValue(progressBarUI, currentLoadedBytes);
                UIUtil.SetLabelValue(progressBarTextUI, currentLoadedBytes.ToString("P0"));

                if(levelLoadState != LoadSceneState.LevelNotStarted) {
                    if(asyncLevelLoad != null) {
                        currentLoadedBytes = asyncLevelLoad.progress; // 0 to 1 float when 1.0 it is complete use for progress
                        Messenger<float>.Broadcast(LoadSceneMessages.LevelLoadProgress, currentLoadedBytes);
                    }
                }
            }
        }
    }
}