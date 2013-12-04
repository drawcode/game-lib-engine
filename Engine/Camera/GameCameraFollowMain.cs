using UnityEngine;
using System.Collections;

public class GameCameraFollowMain : MonoBehaviour {
	public float smoothTime= 0.0f;
	public Vector3 offset = Vector3.zero;
	
	public bool followX = true;
	public bool followY = false;
	public bool followZ = true;

    Camera camMain;
    Transform thisTransform;
    Vector3 velocity;
	
	public void  Start (){
		thisTransform = transform;
	}

    public void FindMainCamera() {
        if(camMain == null) {
            camMain = Camera.main;
        }
    }
	
	void LateUpdate() {

        FindMainCamera();

        if(thisTransform != null 
           && camMain != null 
           && camMain.transform != null) {

            Vector3 temp = camMain.transform.position;
			
			if(followX) {
				temp.x = Mathf.SmoothDamp(thisTransform.position.x, 
					camMain.transform.position.x + offset.x, ref velocity.x, smoothTime);
			}
			
			if(followY) {
				temp.y = Mathf.SmoothDamp( thisTransform.position.y, 
					camMain.transform.position.y + offset.y, ref velocity.y, smoothTime);
			}
			
			if(followZ) {
				temp.z = Mathf.SmoothDamp( thisTransform.position.z, 
					camMain.transform.position.z + offset.z, ref velocity.z, smoothTime);
			}
				
			thisTransform.position = temp;
		}
	}
}