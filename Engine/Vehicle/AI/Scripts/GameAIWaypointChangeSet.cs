using UnityEngine;
using System.Collections;

public class GameAIWaypointChangeSet : GameObjectBehavior {

    public string tagName = "Untagged";
    public string folderName = "Waypoints";
    public string preName = "WP";
    public float maxSpeed = 100;
    public int nextWaypointNo = 1;
    
    void Start() {
        //layer = 2;
        collider.isTrigger = true;
        renderer.enabled = false;
    }
    
    void OnTriggerEnter(Collider other) {
        
        //if (other.gameObject.transform.root.gameObject.tag == tagName) //2013-08-02
        if (other.gameObject.transform.root.gameObject.CompareTag(tagName)) { //2013-08-02
            GameVehicleAIDriverController aIDriverController = other.gameObject.transform.root.gameObject.GetComponentInChildren<GameVehicleAIDriverController>();
            if (aIDriverController != null) {
                            
                aIDriverController.SetNewWaypointSet(folderName, preName, maxSpeed, nextWaypointNo);
                
            }
        }
    }
}
