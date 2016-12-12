using UnityEngine;
using System.Collections;

public class GameObjectTrackingWorld : MonoBehaviour {

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

        currentPositionTo = transform.position;

        if(trackPositionX) {
            currentPositionTo.x = trackObject.transform.position.x;
        }

        if(trackPositionY) {
            currentPositionTo.y = trackObject.transform.position.y;
        }

        if(trackPositionZ) {
            currentPositionTo.z = trackObject.transform.position.z;
        }

        if(currentPositionTo != lastPositionTo) {
            lastPositionTo = currentPositionTo;
            transform.position = 
                Vector3.zero
                    .WithX(currentPositionTo.x)
                    .WithY(currentPositionTo.y)
                    .WithZ(currentPositionTo.z);
        }

        // ROTATION

        currentRotationTo = transform.rotation.eulerAngles;

        if(trackRotationX) {
            currentRotationTo.x = trackObject.transform.rotation.eulerAngles.x;
        }

        if(trackRotationY) {
            currentRotationTo.y = trackObject.transform.rotation.eulerAngles.y;
        }

        if(trackRotationZ) {
            currentRotationTo.z = trackObject.transform.rotation.eulerAngles.z;
        }

        //if(currentPositionTo != lastPositionTo) {
        //    lastPositionTo = currentPositionTo;
        //    transform.position = currentPositionTo;
        //}

        if(currentRotationTo != lastRotationTo) {
            lastRotationTo = currentRotationTo;
            transform.rotation = 
                Quaternion.Euler(
                    Vector3.zero
                    .WithX(currentRotationTo.x)
                    .WithY(currentRotationTo.y)
                    .WithZ(currentRotationTo.z));
        }

    }
}