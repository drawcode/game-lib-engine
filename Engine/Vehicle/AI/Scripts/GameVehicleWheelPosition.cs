using UnityEngine;
using System.Collections;

public class GameVehicleWheelPosition : MonoBehaviour {
    public WheelCollider WheelCol;
    public bool isHittingGround = false; //2013-06-18
    private Vector3 newPos;

    void Update() {
        RaycastHit hit;

        if (Physics.Raycast(WheelCol.transform.position, -WheelCol.transform.up, out hit, WheelCol.suspensionDistance + WheelCol.radius)) {   
            isHittingGround = true; //2013-06-18
            if (hit.collider.isTrigger) {               
                newPos = transform.position;                
            }
            else {
                newPos = hit.point + WheelCol.transform.up * WheelCol.radius;
                
            }           
        }
        else {
            isHittingGround = false; //2013-06-18
            newPos = WheelCol.transform.position - (WheelCol.transform.up * WheelCol.suspensionDistance);   
            //Debug.Log(gameObject.name + " isHitting FALSE");
        }
        
        transform.position = newPos;
    }
}
