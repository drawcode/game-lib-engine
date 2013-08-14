using UnityEngine;
using System.Collections;

public class GameBaseCameraSmoothFollow : MonoBehaviour {
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
			Vector3 temp = target.position;
			
			if(followX) {
				temp.x = Mathf.SmoothDamp(thisTransform.position.x, 
					target.position.x + offset.x, ref velocity.x, smoothTime);
			}
			
			if(followY) {
				temp.y = Mathf.SmoothDamp( thisTransform.position.y, 
					target.position.y + offset.y, ref velocity.y, smoothTime);
			}
			
			if(followZ) {
				temp.z = Mathf.SmoothDamp( thisTransform.position.z, 
					target.position.z + offset.z, ref velocity.z, smoothTime);
			}
				
			thisTransform.position = temp;
		}
	}
}