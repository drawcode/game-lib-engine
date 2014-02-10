using UnityEngine;
using System.Collections;

public class ChangeWaypointSet : MonoBehaviour {

    public string tagName = "Untagged";
    public string folderName = "Waypoints";
    public string preName = "WP";
    public float maxSpeed = 100;
    public int nextWaypointNo = 1;
    
    void Start() {
        gameObject.layer = 2;
        gameObject.collider.isTrigger = true;
        gameObject.transform.renderer.enabled = false;
    }
    
    void OnTriggerEnter(Collider other) {
        
        //if (other.gameObject.transform.root.gameObject.tag == tagName) //2013-08-02
        if (other.gameObject.transform.root.gameObject.CompareTag(tagName)) { //2013-08-02
            AIDriverController aIDriverController = other.gameObject.transform.root.gameObject.GetComponentInChildren<AIDriverController>();
            if (aIDriverController != null) {
                            
                aIDriverController.SetNewWaypointSet(folderName, preName, maxSpeed, nextWaypointNo);
                
            }
        }
    }
}
