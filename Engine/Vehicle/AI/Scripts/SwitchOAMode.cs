using UnityEngine;
using System.Collections;

public class SwitchOAMode : MonoBehaviour {
    public string tagName = "Untagged";
    public bool switchUseOaTo = false;
    
    //2012-06-23
    void Start() {
        
        gameObject.layer = 2;
        gameObject.collider.isTrigger = true;
        gameObject.transform.renderer.enabled = false;
        
    }
    //2012-06-23
    
    void OnTriggerEnter(Collider other) {
        
        //if (other.gameObject.transform.root.gameObject.tag == tagName)    //2013-08-02
        if (other.gameObject.transform.root.gameObject.CompareTag(tagName)) { //2013-08-02
            AIDriverController aIDriverController = 
                other.gameObject.transform.root.gameObject.GetComponentInChildren<AIDriverController>();
            if (aIDriverController != null) {
                //aIDriverController.useObstacleAvoidance = switchUseOaTo;
                aIDriverController.SwitchOaMode(switchUseOaTo);
            }
        }
    }
}
