using UnityEngine;
using System.Collections;

public class DetchToWorld : GameObjectBehavior {

    public void Start() {
        transform.parent = null;
    }
    
    public void  Update() {
    
    }
}