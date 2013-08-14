using System;
using UnityEngine;

namespace Engine.Graphics {

    [RequireComponent(typeof(Renderer))]
    public class UVTileAnimator : MonoBehaviour {
        public int tileCount;
        public int tileColumns;
        public float framesPerSecond = 10;
        public bool repeat = true;

        private int tileRows;
        private Vector2 tileSize;
        private Material material;

        private void OnEnable() {
            tileRows = (tileCount + (tileColumns - 1)) / tileColumns;
            tileSize = new Vector2(1.0f / tileColumns, 1.0f / tileRows);

            material = renderer.material;
            material.SetTextureScale("_MainTex", tileSize);
        }

        private void Update() {

            // Calculate index
            int index = (int)(Time.time * framesPerSecond);

            if (repeat)
                index = index % tileCount;

            // split into horizontal and vertical index
            var uIndex = index % tileColumns;
            var vIndex = index / tileColumns;

            // build offset
            // v coordinate is the bottom of the image in opengl so we need to invert.
            var offset = new Vector2(uIndex * tileSize.x, 1.0f - tileSize.y - vIndex * tileSize.y);

            material.SetTextureOffset("_MainTex", offset);
        }
    }
}