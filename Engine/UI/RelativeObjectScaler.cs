using System;
using UnityEngine;

namespace Engine.UI {

    //[ExecuteInEditMode]
    public class RelativeObjectScaler : MonoBehaviour {
        public Vector2 originalSize = new Vector2(960f, 640f);
        public bool realtime = false;
        public bool broadcast = true;
        public bool run = true;

        //Vector3 positionTo;
        private Vector3 originalPosition;

        private Vector3 originalScale;

        //float lastScreenRatio;
        private float currentTimeBlock = 2.0f;

        private float sampleInterval = 1f;

        public Vector2 Scale {
            get { return new Vector2(camera.pixelRect.width / originalSize.x, camera.pixelRect.height / originalSize.y); }
        }

        private void Start() {
            ResetPositions();
        }

        private void ResetPositions() {
            originalScale = gameObject.transform.localScale;

            originalScale.x = 1.0f;
            originalScale.y = 1.0f;
            originalScale.z = 1.0f;
            gameObject.transform.localScale = originalScale;

            originalPosition = gameObject.transform.localPosition;

            originalPosition.x = 0.0f;
            originalPosition.y = 0.0f;
            originalPosition.z = 0.0f;

            gameObject.transform.localPosition = originalPosition;
        }

        private void Update() {
            currentTimeBlock += Time.deltaTime;
            if (currentTimeBlock > sampleInterval) {
                currentTimeBlock = 0.0f;
                UpdateViewport();

                //Disable in non-realtime situations
                if (!Application.isEditor && !realtime) {
                    run = false;
                }
            }
        }

        private void UpdateViewport() {

            //if(run) {
            //float origScreenRatio = originalSize.x / originalSize.y;
            float screenRatio = (float)Screen.width / (float)Screen.height;

            //if(screenRatio != lastScreenRatio) {
            //lastScreenRatio = screenRatio;

            //LogUtil.Log("screenRatio:" + screenRatio);

            if (screenRatio < 1.4) {

                // adjust 4:3 and above to fit better.
                Vector3 scaleTo = gameObject.transform.localScale;
                float ratioFiltered = 1.0f + ((1.0f - screenRatio) / 2.0f);
                scaleTo.x = ratioFiltered;
                scaleTo.y = ratioFiltered;
                scaleTo.z = ratioFiltered;
                gameObject.transform.localScale = scaleTo;

                Vector3 positionTo = originalPosition;

                float relativeScaleAdjust = (((1.0f + ((1.0f - screenRatio) / 2.0f)) / 4) / 4);
                float offset = (relativeScaleAdjust * 10) - (.625f / 2);
                positionTo.x = originalPosition.x - offset;

                //GameDatas.Current.currentRelativeScaleAdjust = ratioFiltered;
                //GameDatas.Current.currentRelativeScaleOffset = offset;

                gameObject.transform.localPosition = positionTo;
            }
            else {
                ResetPositions();
            }

            //}

            //run = false;
            //}
        }
    }
}