using UnityEngine;
using System.Collections;

public class GameObjectInactive : GameObjectBehavior {
    
    // Apply this class to objects needed to be hidden but later found
    // by using GetComponentsInChildren with the inactive flag set without
    // searching recursively through the whole heirarchy of that object.

    public string code = "";
    public string type = "";
    
    void Start() {
        
    }
}
