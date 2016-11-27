using UnityEngine;
using System.Collections;

using Vuforia;

public class GameObjectTrackingLocal : MonoBehaviour {

    public GameObject trackObject;

    public bool trackRotationX = false;
    public bool trackRotationY = true;
    public bool trackRotationZ = false;

    public bool trackPositionX = false;
    public bool trackPositionY = false;
    public bool trackPositionZ = false;

    public Vector3 currentPositionTo = Vector3.zero;
    public Vector3 currentRotationTo = Vector3.zero;

    public Vector3 lastPositionTo = Vector3.zero;
    public Vector3 lastRotationTo = Vector3.zero;

    void Start() {

    }

    void Update() {

        // POSITION

        currentPositionTo = transform.localPosition;

        if(trackPositionX) {
            currentPositionTo.x = trackObject.transform.localPosition.x;
        }

        if(trackPositionY) {
            currentPositionTo.y = trackObject.transform.localPosition.y;
        }

        if(trackPositionZ) {
            currentPositionTo.z = trackObject.transform.localPosition.z;
        }

        if(currentPositionTo != lastPositionTo) {
            lastPositionTo = currentPositionTo;
            transform.localPosition = 
                Vector3.zero
                    .WithX(currentPositionTo.x)
                    .WithY(currentPositionTo.y)
                    .WithZ(currentPositionTo.z);
        }

        // ROTATION

        currentRotationTo = transform.localRotation.eulerAngles;

        if(trackRotationX) {
            currentRotationTo.x = trackObject.transform.localRotation.eulerAngles.x;
        }

        if(trackRotationY) {
            currentRotationTo.y = trackObject.transform.localRotation.eulerAngles.y;
        }

        if(trackRotationZ) {
            currentRotationTo.z = trackObject.transform.localRotation.eulerAngles.z;
        }

        //if(currentPositionTo != lastPositionTo) {
        //    lastPositionTo = currentPositionTo;
        //    transform.position = currentPositionTo;
        //}

        if(currentRotationTo != lastRotationTo) {
            lastRotationTo = currentRotationTo;
            transform.localRotation = 
                Quaternion.Euler(
                    Vector3.zero
                    .WithX(currentRotationTo.x)
                    .WithY(currentRotationTo.y)
                    .WithZ(currentRotationTo.z));
        }

    }
}