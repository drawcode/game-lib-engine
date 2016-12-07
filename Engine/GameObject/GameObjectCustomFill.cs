using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Engine.Events;

public class GameObjectCustomFill : MonoBehaviour {

    public GameObject containerFill;

    static float t = 0.0f;
    public float range = -200f;

    float currentProgress = 0f;

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

    void Start() {
        if (containerFill == null) {
            containerFill = gameObject;
        }
    }

    void Update() {

        if (containerFill == null) {
            return;
        }

        if (containerFill.transform.localPosition.y <= range / 4) {

            currentProgress = Mathf.Lerp(0f, 1f, t);

            containerFill.transform.localPosition = containerFill.transform.localPosition.WithY(range * (1 - currentProgress));

            t += 0.5f * Time.deltaTime;
        }
    }

    public void OnLevelLoadProgress(float progress) {
        Debug.Log("Loading Progress:" + progress);

        t = progress / 10f;
    }

    public void OnLevelLoadStarted() {

    }

    public void OnLevelLoaded() {

    }
}