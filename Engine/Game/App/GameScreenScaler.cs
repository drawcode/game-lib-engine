using System;
using UnityEngine;

public class GameScreenScaler : MonoBehaviour {
    public static GameScreenScaler Instance;
    public Vector2 originalSize = new Vector2(960f, 640f);
    public Vector2 scaledSize = new Vector2(960f, 640f);
    public Vector2 ratioToOriginalSize = new Vector2(1.0f, 1.0f);
    public bool realtime = false;
    public bool broadcast = true;
    public bool run = true;

    public float originalScreenRatio = 0.0f;

    public float screenRatio = 0.0f;
    public float screenScaledRatio = 0.0f;

    public Rect scaledViewportRect = new Rect();

    public float currentHeight = 0.0f;
    public float currentWidth = 0.0f;

    private void Awake() {
        if (Instance != null && this != Instance) {

            //There is already a copy of this script running
            Destroy(this);
            return;
        }

        Instance = this;
    }

    public float GetRatioScale() {

        //Vector3 scaleTo = new Vector3(1.0f, 1.0f, 1.0f);
        float ratioFiltered = 1.0f + ((1.0f - screenRatio) / 2.0f);
        return ratioFiltered;
    }

    public bool Is4x3orLower() {
        if (GetRatioScale() >= .82) {
            return true;
        }
        return false;
    }

    private void Update() {
        originalScreenRatio = originalSize.x / originalSize.y;
        screenRatio = (float)Screen.width / (float)Screen.height;

        scaledSize.x = Screen.width;
        scaledSize.y = Screen.height;

        ratioToOriginalSize.x = originalSize.x / scaledSize.x;
        ratioToOriginalSize.y = originalSize.y / scaledSize.y;

        scaledViewportRect = new Rect();
        currentWidth = 0.0f;
        currentHeight = 0.0f;

        if (screenRatio > originalScreenRatio) {
            currentWidth = (float)Screen.height * originalScreenRatio;
            scaledViewportRect.height = 1.0f;
            scaledViewportRect.width = currentWidth / (float)Screen.width;
            scaledViewportRect.y = 1.0f;
            scaledViewportRect.x = (1.0f - scaledViewportRect.width) / 2.0f;
            /*
            width = (float)Screen.height * origScreenRatio;
            scaledViewportRect.height = 1.0f;
            scaledViewportRect.width = h / (float)Screen.width;
            scaledViewportRect.y = 0.0f;
            scaledViewportRect.x =  (1.0f - scaledViewportRect.width) / 2.0f;
            */
        }
        else {
            currentHeight = (float)Screen.width / originalScreenRatio;
            scaledViewportRect.width = 1.0f;
            scaledViewportRect.height = currentHeight / (float)Screen.height;
            scaledViewportRect.x = 1.0f;
            scaledViewportRect.y = (1.0f - scaledViewportRect.height) / 2.0f;
        }
    }
}