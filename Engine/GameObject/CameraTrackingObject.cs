using UnityEngine;
using System.Collections;

using Vuforia;

public class CameraTrackingObject : MonoBehaviour {
    
    public Camera cameraMain;

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
		if(cameraMain == null) {
            cameraMain = Camera.main;
		}
	}

    void Update() {

        //currentPositionTo = transform.position;
        currentRotationTo = transform.rotation.eulerAngles;

        transform.LookAt(transform.position + cameraMain.transform.rotation * Vector3.up,
            cameraMain.transform.rotation * Vector3.up);

        if(trackRotationX) {
            currentRotationTo.x = transform.eulerAngles.x;
        }

        if(trackRotationY) {
            currentRotationTo.y = transform.eulerAngles.y;
        }

        if(trackRotationZ) {
            currentRotationTo.z = transform.eulerAngles.z;
        }

        //if(currentPositionTo != lastPositionTo) {
        //    lastPositionTo = currentPositionTo;
        //    transform.position = currentPositionTo;
        //}

        if(currentRotationTo != lastRotationTo) {
            lastRotationTo = currentRotationTo;
            transform.rotation = Quaternion.Euler(
                Vector3.zero
                .WithX(currentRotationTo.x)
                .WithY(currentRotationTo.y)
                .WithZ(currentRotationTo.z));
        }

    }
}