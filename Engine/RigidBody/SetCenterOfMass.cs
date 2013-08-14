using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class SetCenterOfMass : MonoBehaviour {
    public bool overrideWithBelow = true;
    public Vector3 centerOfMassOverride = Vector3.zero;

    private void Start() {
        if (overrideWithBelow)
            rigidbody.centerOfMass = centerOfMassOverride;
    }
}