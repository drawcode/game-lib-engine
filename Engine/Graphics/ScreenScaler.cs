using System;
using UnityEngine;

namespace Engine.Graphics {

    [AddComponentMenu("Engine/Graphics/Screen Scaler")]
    [ExecuteInEditMode]
    public class ScreenScaler : GameObjectBehavior {
        public Vector2 originalSize = new Vector2(960f, 640f);
        public bool realtime = false;
        public bool broadcast = true;

        public Vector2 Scale {
            get { return new Vector2(camera.pixelRect.width / originalSize.x, camera.pixelRect.height / originalSize.y); }
        }

        private void Update() {
            UpdateViewport();

            //Disable in non-realtime situations
            if (!Application.isEditor && !realtime) {
                enabled = false;
            }
        }

        private void UpdateViewport() {
            float origScreenRatio = originalSize.x / originalSize.y;
            float screenRatio = (float)Screen.width / (float)Screen.height;

            Rect newViewportRect = new Rect();
            float w = 0.0f;
            float h = 0.0f;

            if (screenRatio > origScreenRatio) {
                w = (float)Screen.height * origScreenRatio;
                newViewportRect.height = 1.0f;
                newViewportRect.width = w / (float)Screen.width;
                newViewportRect.y = 0.0f;
                newViewportRect.x = (1.0f - newViewportRect.width) / 2.0f;
            }
            else {
                h = (float)Screen.width / origScreenRatio;
                newViewportRect.width = 1.0f;
                newViewportRect.height = h / (float)Screen.height;
                newViewportRect.x = 0.0f;
                newViewportRect.y = (1.0f - newViewportRect.height) / 2.0f;
            }

            camera.rect = newViewportRect;
        }
    }
}