using System;
using UnityEngine;

namespace Engine.Graphics {

    /// <summary>
    ///	A Unity GUITexture replacement that allows rotation
    /// </summary>
    ///

    public class GUITexture2 : GUIItem {
        public Texture2D texture = null;

        protected override void Draw() {
            GUI.DrawTexture(rect, texture, ScaleMode.StretchToFill, true);
        }
    }
}