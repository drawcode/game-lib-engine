// ©2011 Starscene Software. All rights reserved. Redistribution of source code without permission not allowed.

using UnityEngine;

namespace Engine.Graphics.Vector {

    [AddComponentMenu("Vectrosity/VisibilityControl")]
    public class VisibilityControl : GameObjectBehavior {
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

            VectorManager.VisibilitySetup(transform, line, out m_objectNumber);
            vectorLine = line;
        }

        private void OnBecameVisible() {
            Vector.Active(vectorLine, true);

            // Draw line now, otherwise's there's a 1-frame delay before the line is actually drawn in the next LateUpdate
            VectorManager.DrawArrayLine2(m_objectNumber.i);
        }

        private void OnBecameInvisible() {
            Vector.Active(vectorLine, false);
        }

        private void OnDestroy() {
            if (destroyed) return;	// Paranoia check
            destroyed = true;
            VectorManager.VisibilityRemove(m_objectNumber.i);
            Vector.DestroyLine(ref vectorLine);
        }
    }
}