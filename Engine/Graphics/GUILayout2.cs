using System;
using UnityEngine;

namespace Engine.Graphics {

    public class GUILayout2 : GameObjectBehavior {
        public Vector2 pixelScale;
#if UNITY_EDITOR
		public bool debug;
#endif
    }
}