using System;
using UnityEngine;

namespace Engine.Graphics {

    /// <summary>
    ///	A Unity GUITexture replacement that allows rotation
    /// </summary>
    ///

    public class GUIText2 : GUIItem {
        public string text;
        public TextAnchor anchor;
        public Font font;
        public Material material;
        private GUIStyle style;

        protected override void UpdateSettings() {
            base.UpdateSettings();

            scale = Vector2.one;

            style = new GUIStyle(GUIStyle.none);
            style.font = font;
            if (material != null)
                style.font.material = material;

            style.alignment = anchor;
            style.stretchHeight = true;
            style.normal.textColor = color;
        }

        protected override void Draw() {
            GUI.matrix = Matrix4x4.Scale(new Vector3(pixelScale.x, pixelScale.y, 1f)) * GUI.matrix;

            float invPSX = 1f / pixelScale.x;
            float invPSY = 1f / pixelScale.y;
            rect.x *= invPSX;
            rect.y *= invPSY;
            rect.width *= invPSX;
            rect.height *= invPSY;

#if UNITY_EDITOR
			if(layout != null && layout.debug)
				GUI.Box(rect, "");
#endif

            GUI.Label(rect, text, style);
        }
    }
}