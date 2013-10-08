using System;
using UnityEngine;

public static class RigidbodyExtensions {

    public static void Freeze(this Rigidbody inst) {

        if(inst == null) {
            return;
        }

        if(inst != null) {
            //inst.freezeRotation = true;
            //inst.angularDrag = 0;
            //inst.angularVelocity = Vector3.zero;
            //inst.velocity = Vector3.zero;
            inst.constraints = RigidbodyConstraints.FreezeAll;
            inst.Sleep();
        }
    }

    public static void UnFreeze(this Rigidbody inst) {

        if(inst == null) {
            return;
        }

        if(inst != null) {
            //inst.freezeRotation = false;
            //inst.angularDrag = 0;
            //inst.angularVelocity = Vector3.zero;
            //inst.velocity = Vector3.zero;
            inst.constraints = RigidbodyConstraints.None;
            inst.WakeUp();
        }
    }
}

