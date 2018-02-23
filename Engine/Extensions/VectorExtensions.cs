using System;
using UnityEngine;

public static class VectorExtensions {

    public static bool IsBiggerThanDeadzone(this Vector3 inst, float deadZone) {
        if (Mathf.Abs(inst.x) > deadZone
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

    public static Vector3 LerpPercent(this Vector3 inst, Vector3 positionTo, float percent) {
        return (inst + percent * (positionTo - inst));
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

    //public static float Distance(this Vector2 inst, Vector2 to) {
    //    return Vector2.Distance(inst, to);
    //}

    //public static float Distance(this Vector3 inst, Vector3 to) {
    //    return Vector3.Distance(inst, to);
    //}

    //public static float Distance(this Vector4 inst, Vector4 to) {
    //    return Vector4.Distance(inst, to);
    //}
}