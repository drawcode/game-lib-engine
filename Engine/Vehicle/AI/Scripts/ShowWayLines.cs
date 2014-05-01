using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class ShowWayLines : GameObjectBehavior {

    public bool show = false;
    public Color color = Color.magenta;
    private GameVehicleAIDriverController aiDriverController;

    public void OnDrawGizmos() {
        if (!Application.isPlaying || show) {
            aiDriverController = gameObject.GetComponent("GameVehicleAIDriverController") as GameVehicleAIDriverController;
                      
            List<Transform> waypoints = aiDriverController.waypoints;
            
            Vector3 wpPosLast = Vector3.zero;           
            
            foreach (Transform wp in waypoints) {               
                
                if (wpPosLast != Vector3.zero) {
                    Debug.DrawRay(wpPosLast, wp.position, color);
                }
                
                wpPosLast = wp.position;
                
            }
            
                        
        }
    }
    
}
