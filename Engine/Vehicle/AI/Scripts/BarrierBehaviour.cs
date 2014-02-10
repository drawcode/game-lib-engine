using UnityEngine;
using System.Collections;

public class BarrierBehaviour : MonoBehaviour {
    
    public bool show = false;
    public bool triggerCollider = true;

    void Awake() {
        gameObject.collider.isTrigger = triggerCollider;
        gameObject.transform.renderer.enabled = show;
    }
        
    
}



