using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SetStartPosition : GameObjectBehavior {

    public int startWaypoint = 0;
    public int heightOffset = 2;
    public bool usingSplinePathWaypoints = true;
    public float delay = 0;
    // Use this for initialization
    void Start() {
        
        if (delay == 0)
            SetNewPosition();
        else {
            StartCoroutine(DelayedStart());
        }
        
                                        
    }
    
    void SetNewPosition() {
        if (startWaypoint > 0) {
            Vector3 startPos;           
            GameVehicleAIDriverController aiDriverController;
            SplinePathWaypoints splinePathWaypoints; //2012-07-29
            
            aiDriverController = gameObject.GetComponent("GameVehicleAIDriverController") as GameVehicleAIDriverController;           
            
            //2012-07-29 -B
            splinePathWaypoints = gameObject.GetComponent("SplinePathWaypoints") as SplinePathWaypoints; //2012-07-29
            if (splinePathWaypoints != null) {
                //if (splinePathWaypoints.active)
                if (usingSplinePathWaypoints) { //because other cars, how are using the same waypoints could create the spline path                 
                    startWaypoint = (startWaypoint) * (splinePathWaypoints.steps + 1);
                }
            }
            //2012-07-29 -E
            
            if (aiDriverController.waypoints.Count > startWaypoint) {
                startPos = aiDriverController.waypoints[startWaypoint].position;
                startPos.y += heightOffset;
                gameObject.transform.position = startPos;
                gameObject.transform.rotation = aiDriverController.waypoints[startWaypoint].rotation;
                
                if (aiDriverController.waypoints.Count > startWaypoint + 1) {
                    aiDriverController.currentWaypoint = startWaypoint + 1; 
                }
                else {
                    aiDriverController.currentWaypoint = 0;
                }
            }
            else {
                LogUtil.LogError("StartWaypoint number is to high. Maximum is" + (aiDriverController.waypoints.Count - 1) + "). Please check the Execution Order of the scripts (see the documentation)."); //2012-07-29
            }
        }
    }
    
    IEnumerator DelayedStart() {
        
        yield return new WaitForSeconds(delay);
        SetNewPosition();
        
    }
}
