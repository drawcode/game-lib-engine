// ©2011 Starscene Software. All rights reserved. Redistribution of source code without permission not allowed.

using System.Collections;
using UnityEngine;

namespace Engine.Graphics.Vector {

    [AddComponentMenu("Vectrosity/VisibilityControlStatic")]
    public class VisibilityControlStatic : MonoBehaviour {
        private RefInt m_objectNumber;
        private VectorLine vectorLine;
        private bool destroyed = false;

        public RefInt objectNumber {
            get { return m_objectNumber; }
        }

        public void Setup(VectorLine line, bool makeBounds) {
            if (makeBounds) {
                VectorManager.SetupBoundsMesh(gameObject, line);
            }

            // Adjust points to this position, so the line doesn't have to be updated with the transform of this object
            // We make a new array since each line must therefore be a unique instance, not a reference to the original set of Vector3s
            var thisPoints = new Vector3[line.points3.Length];
            var thisMatrix = transform.localToWorldMatrix;
            for (int i = 0; i < thisPoints.Length; i++) {
                thisPoints[i] = thisMatrix.MultiplyPoint3x4(line.points3[i]);
            }
            line.points3 = thisPoints;
            vectorLine = line;

            VectorManager.VisibilityStaticSetup(line, out m_objectNumber);
            StartCoroutine(WaitCheck());
        }

        private IEnumerator WaitCheck() {

            // Ensure that the line is drawn once even if the camera isn't moving
            // Otherwise this object would be invisible until the camera moves
            // However, the camera might not have been set up yet, so wait a frame and turn off if necessary
            VectorManager.DrawArrayLine(m_objectNumber.i);

            yield return null;
            if (!renderer.isVisible) {
                Vector.Active(vectorLine, false);
            }
        }

        private void OnBecameVisible() {
            Vector.Active(vectorLine, true);

            // Draw line now, otherwise's there's a 1-frame delay before the line is actually drawn in the next LateUpdate
            VectorManager.DrawArrayLine(m_objectNumber.i);
        }

        private void OnBecameInvisible() {
            Vector.Active(vectorLine, false);
        }

        private void OnDestroy() {
            if (destroyed) return;	// Paranoia check
            destroyed = true;
            VectorManager.VisibilityStaticRemove(m_objectNumber.i);
            Vector.DestroyLine(ref vectorLine);
        }
    }
}