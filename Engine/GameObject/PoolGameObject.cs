using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolGameObject : GameObjectBehavior {

    public bool pooled = true;

    // Incremented every time this object is issued from a pool. A delayed recycle
    // captures the serial it was scheduled against and gives up if the object has
    // since been recycled and handed out again -- otherwise a leftover timer from a
    // previous life reclaims a live object mid-flight.

    public int useSerial = 0;

    void Start() {

    }

    public static int Bump(GameObject go) {

        if (go == null) {
            return 0;
        }

        PoolGameObject poolGameObject = go.GetComponent<PoolGameObject>();

        if (poolGameObject == null) {
            return 0;
        }

        unchecked {
            poolGameObject.useSerial++;
        }

        return poolGameObject.useSerial;
    }

    public static int GetSerial(GameObject go) {

        if (go == null) {
            return 0;
        }

        PoolGameObject poolGameObject = go.GetComponent<PoolGameObject>();

        if (poolGameObject == null) {
            return 0;
        }

        return poolGameObject.useSerial;
    }

    public static bool IsSameUse(GameObject go, int serial) {

        if (go == null) {
            return false;
        }

        PoolGameObject poolGameObject = go.GetComponent<PoolGameObject>();

        if (poolGameObject == null) {
            return true;
        }

        return poolGameObject.useSerial == serial;
    }
}
