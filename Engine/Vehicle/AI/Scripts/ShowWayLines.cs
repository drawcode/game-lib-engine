using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class ShowWayLines : MonoBehaviour {

    public bool show = false;
    public Color color = Color.magenta;
    private AIDriverController aiDriverController;

    public void OnDrawGizmos() {
        if (!Application.isPlaying || show) {
            aiDriverController = gameObject.GetComponent("AIDriverController") as AIDriverController;
                      
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
