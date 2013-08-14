// ©2011 Starscene Software. All rights reserved. Redistribution of source code without permission not allowed.

using UnityEngine;

namespace Engine.Graphics.Vector {

    [AddComponentMenu("Vectrosity/VisibilityControlAlways")]
    public class VisibilityControlAlways : MonoBehaviour {
        private RefInt m_objectNumber;
        private VectorLine vectorLine;
        private bool destroyed = false;

        public RefInt objectNumber {
            get { return m_objectNumber; }
        }

        public void Setup(VectorLine line) {
            VectorManager.VisibilitySetup(transform, line, out m_objectNumber);
            VectorManager.DrawArrayLine2(m_objectNumber.i);
            vectorLine = line;
        }

        private void OnDestroy() {
            if (destroyed) return;	// Paranoia check
            destroyed = true;
            VectorManager.VisibilityRemove(m_objectNumber.i);
            Vector.DestroyLine(ref vectorLine);
        }
    }
}