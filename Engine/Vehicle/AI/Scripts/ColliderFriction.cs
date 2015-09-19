using UnityEngine;
using System.Collections;

public class ColliderFriction : GameObjectBehavior {

    public float frictionValue=0;
    
	// Use this for initialization
	void Start () {
        collider.material.staticFriction = frictionValue;
        //collider.material.staticFriction2 = frictionValue;
        collider.material.dynamicFriction = frictionValue;
        //collider.material.dynamicFriction2 = frictionValue;
	}
	
	
}
