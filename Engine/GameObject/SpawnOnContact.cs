using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpawnOnContact : GameObjectBehavior {
    
    public GameObject objectToCreate;
    
    public void Start() {

    }
    
    public void OnCollisionEnter(Collision collision) {

        GameObject explosion = GameObjectHelper.CreateGameObject(objectToCreate, transform.position, transform.rotation, true);

        if(explosion != null) {
            explosion.transform.parent = null;

            GameObjectHelper.DestroyGameObject(explosion, 1f, true);  
        }
        
    }
}

