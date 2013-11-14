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

    public static bool IsBiggerThanDeadzone(this Vector3 inst, float deadZone) {
        if(Mathf.Abs(inst.x) > deadZone
           || Mathf.Abs(inst.y) > deadZone
           || Mathf.Abs(inst.z) > deadZone) {
            return true;
        }
        return false;
    }

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

    // Reset

    public static Vector3 Reset(this Vector3 inst) {
        inst = Vector3.zero;
        return inst;
    }

    //

    public static float CrossAngle(this Vector2 toVector2) {
        Vector2 fromVector2 = Vector2.zero.WithY(1); // default to up being second angle
        //Vector2 toVector2 = gesture.Move;

        float angle = Vector2.Angle(fromVector2, toVector2);
        Vector3 cross = Vector3.Cross(fromVector2, toVector2);

        if (cross.z > 0)
            angle = 360 - angle;

        return angle;
    }

    public static float CrossAngle(this Vector2 fromVector2, Vector2 toVector2) {
        //Vector2 fromVector2 = Vector2.zero.WithY(1);
        //Vector2 toVector2 = gesture.Move;

        float angle = Vector2.Angle(fromVector2, toVector2);
        Vector3 cross = Vector3.Cross(fromVector2, toVector2);

        if (cross.z > 0)
            angle = 360 - angle;

        return angle;
    }
}