using UnityEngine;
using System.Collections;

public class DestroyObjectTimed : GameObjectBehavior {
    
    public float delay = 5.0f;
    
    public void Start() {
        GameObjectHelper.DestroyDelayed(gameObject, delay);
    }

    public void Update() {

    }
}