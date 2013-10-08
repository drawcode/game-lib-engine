using System;
using UnityEngine;

/*
public static class Vector4Extensions {

    public static Vector4 WithW(this Vector4 inst, float w) {
        inst.w = w;
        return inst;
    }

    public static Vector4 WithX(this Vector4 inst, float x) {
        inst.x = x;
        return inst;
    }

    public static Vector4 WithY(this Vector4 inst, float y) {
        inst.y = y;
        return inst;
    }

    public static Vector4 WithZ(this Vector4 inst, float z) {
        inst.z = z;
        return inst;
    }
}
*/

public static class VectorExtensions {

    // 3

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

    // 2

    public static Vector2 WithX(this Vector2 inst, float x) {
        inst.x = x;
        return inst;
    }

    public static Vector2 WithY(this Vector2 inst, float y) {
        inst.y = y;
        return inst;
    }
}