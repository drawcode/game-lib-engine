using UnityEngine;
using System.Collections;

public class BarrierBehaviour : GameObjectBehavior {
    
    public bool show = false;
    public bool triggerCollider = true;

    void Awake() {
        gameObject.GetComponent<Collider>().isTrigger = triggerCollider;
        gameObject.transform.GetComponent<Renderer>().enabled = show;
    }
        
    
}



