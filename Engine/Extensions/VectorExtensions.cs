using System;
using UnityEngine;

public static class Vector3Extensions {

    public static Vector3 WithX(this Vector3 inst, float x) {
        inst.x = x;
        return inst;
    }

    public static Vector3 WithY(this Vector3 inst, float y) {
        inst.y = y;
        return inst;
    }

    public static Vector3 WithZ(this Vector3 inst, float z) {
        inst.z = z;
        return inst;
    }
}