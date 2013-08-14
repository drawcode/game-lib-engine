using UnityEngine;
using System.Collections;

public class GameCameraSmoothFollow : MonoBehaviour {
	public Transform target;
	public float smoothTime= 0.3f;
	private Transform thisTransform;
	private Vector3 velocity;
	public Vector3 offset;
	
	public bool followX = true;
	public bool followY = false;
	public bool followZ = true;
	
	void  Start (){
		thisTransform = transform;
	}
	
	void LateUpdate (){

		if(thisTransform != null && target != null) {
			Vector3 temp = Vector3.zero;
			
			if(followX) {
				temp.x = Mathf.SmoothDamp(thisTransform.position.x + offset.x, 
					target.position.x, ref velocity.x, smoothTime);
			}
			
			if(followY) {
				temp.y = Mathf.SmoothDamp( thisTransform.position.y + offset.y, 
					target.position.y, ref velocity.y, smoothTime);
			}
			
			if(followZ) {
				temp.z = Mathf.SmoothDamp( thisTransform.position.z + offset.z, 
					target.position.z, ref velocity.z, smoothTime);
			}
				
			thisTransform.position = temp;
		}
	}
}