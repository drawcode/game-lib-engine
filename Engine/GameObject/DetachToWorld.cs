using UnityEngine;
using System.Collections;

public class DetachToWorld : GameObjectBehavior {

    public void Start() {
        transform.parent = null;
    }
    
    public void  Update() {
    
    }
}