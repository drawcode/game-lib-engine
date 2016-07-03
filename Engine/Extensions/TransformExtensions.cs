using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public static class TransformExtensions {

    public static bool IsPrefabGhost(this Transform inst) {
        var tmp = new GameObject();
        try {
            tmp.transform.parent = inst.parent;
            
            var index = inst.GetSiblingIndex();
            
            inst.SetSiblingIndex(int.MaxValue);
            if (inst.GetSiblingIndex() == 0)
                return true;
            
            inst.SetSiblingIndex(index);
            return false;
        }
        finally {
            //UnityEngine.Object.DestroyImmediate(tmp);
            UnityEngine.Object.Destroy(tmp);
        }
    }
}