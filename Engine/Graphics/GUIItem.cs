using System;
using UnityEngine;

namespace Engine.Graphics {

    /// <summary>
    ///	A Unity GUITexture replacement that allows rotation
    /// </summary>
    ///
    [ExecuteInEditMode()]
    public class GUIItem : GameObjectBehavior {
        public Color color = Color.gray;
        public Rect pixelOffset;
        public Vector2 pixelScale = new Vector2(1, 1);
        public GUILayout2 layout;
        protected Vector2 pos = new Vector2(0, 0);
        protected Rect rect;
        protected Vector2 pivot;
        protected Vector2 scale;
        protected float rotation;

        public Rect PixelRect {
            get { return rect; }
        }

        public void Start() {

            //useGUILayout = true;

            UpdateSettings();
        }

        protected virtual void UpdateSettings() {
            if (layout != null)
                pixelScale = layout.pixelScale;

            Rect cameraRect;

            if (Camera.current != null) {
                cameraRect = Camera.current.pixelRect;
                cameraRect.x = Mathf.RoundToInt(cameraRect.x);
                cameraRect.y = Mathf.RoundToInt(cameraRect.y);
                cameraRect.width = Mathf.RoundToInt(cameraRect.width);
                cameraRect.height = Mathf.RoundToInt(cameraRect.height);
            }
            else {
                cameraRect = new Rect(0f, 0f, Screen.width, Screen.height);
            }

            pos = new Vector2(transform.position.x * cameraRect.width, transform.position.y * cameraRect.height);
            pos.x += cameraRect.x;
            pos.y += cameraRect.y;

            rect = new Rect((pos.x + pixelOffset.x * pixelScale.x) - (pixelOffset.width * pixelScale.x * 0.5f),
                            (pos.y + pixelOffset.y * pixelScale.y) - (pixelOffset.height * pixelScale.y * 0.5f),
                            pixelOffset.width * pixelScale.x,
                            pixelOffset.height * pixelScale.y);

            pivot = new Vector2(rect.xMin + rect.width * 0.5f, rect.yMin + rect.height * 0.5f);

            scale = transform.lossyScale;
            rotation = transform.eulerAngles.z;
        }

        public void OnGUI() {
#if UNITY_EDITOR
			UpdateSettings();
#endif

            var matrix = GUI.matrix;

            GUIUtility.ScaleAroundPivot(scale, pivot);
            GUIUtility.RotateAroundPivot(rotation, pivot);
            GUI.color = color;

            GUI.depth = Mathf.RoundToInt(transform.position.z);

            Draw();

            GUI.matrix = matrix;
        }

        protected virtual void Draw() {
        }

        public bool HitTest(Vector2 point) {
            point.y = Screen.height - point.y;
            return PixelRect.Contains(point);
        }
    }
}