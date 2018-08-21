
using UnityEngine;

public static class QuaternionExtensions {
    // Reset

    public static Quaternion Reset(this Quaternion inst) {

        inst = Quaternion.identity;

        return inst;
    }
}