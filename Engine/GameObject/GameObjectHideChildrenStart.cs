using UnityEngine;
using System.Collections;

public class GameObjectHideChildrenStart : MonoBehaviour {
	
	// Apply this class to objects needed to be hidden but later found
	// by using GetComponentsInChildren with the inactive flag set without
	// searching recursively through the whole heirarchy of that object.
	
	void Start () {
        gameObject.HideChildren();	
	}
}
