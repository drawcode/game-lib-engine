using UnityEngine;
using System.Collections;

public class GameObjectVisibilityChildren : MonoBehaviour {
	
	// Apply this class to objects needed to be hidden but later found
	// by using GetComponentsInChildren with the inactive flag set without
	// searching recursively through the whole heirarchy of that object.

    // BELOW

    public bool hideBelowXWorld = false;
    public bool hideBelowYWorld = false;
    public bool hideBelowZWorld = false;

    public bool hideBelowXLocal = false;
    public bool hideBelowYLocal = false;
    public bool hideBelowZLocal = false;
	
    public float hideBelowXWorldVal = 0;
    public float hideBelowYWorldVal = 0;
    public float hideBelowZWorldVal = 0;

    public float hideBelowXLocalVal = 0;
    public float hideBelowYLocalVal = 0;
    public float hideBelowZLocalVal = 0;

    // ABOVE

    public bool hideAboveXWorld = false;
    public bool hideAboveYWorld = false;
    public bool hideAboveZWorld = false;

    public bool hideAboveXLocal = false;
    public bool hideAboveYLocal = false;
    public bool hideAboveZLocal = false;

    public float hideAboveXWorldVal = 0;
    public float hideAboveYWorldVal = 0;
    public float hideAboveZWorldVal = 0;

    public float hideAboveXLocalVal = 0;
    public float hideAboveYLocalVal = 0;
    public float hideAboveZLocalVal = 0;

    bool hidden = false;

    Vector3 posWorld = Vector3.zero;
    Vector3 posLocal = Vector3.zero;

	void Start () {
        
	}

    void Update() {


        bool hide = false;

        posWorld = transform.position;
        posLocal = transform.localPosition;

        // HIDING BELOW

        // WORLD

        if(hideBelowXWorld) {
            if(posWorld.x < hideBelowXWorldVal) {
                hide = true;
            }
        }

        if(hideBelowYWorld) {
            if(posWorld.y < hideBelowYWorldVal) {
                hide = true;
            }
        }

        if(hideBelowZWorld) {
            if(posWorld.z < hideBelowZWorldVal) {
                hide = true;
            }
        }

        // LOCAL

        if(hideBelowXLocal) {
            if(posLocal.x < hideBelowXLocalVal) {
                hide = true;
            }
        }

        if(hideBelowYLocal) {
            if(posLocal.y < hideBelowYLocalVal) {
                hide = true;
            }
        }

        if(hideBelowZLocal) {
            if(posLocal.z < hideBelowZLocalVal) {
                hide = true;
            }
        }

        // HIDING ABOVE

        // WORLD

        if(hideAboveXWorld) {
            if(posWorld.x > hideAboveXWorldVal) {
                hide = true;
            }
        }

        if(hideAboveYWorld) {
            if(posWorld.y > hideAboveYWorldVal) {
                hide = true;
            }
        }

        if(hideAboveZWorld) {
            if(posWorld.z > hideAboveZWorldVal) {
                hide = true;
            }
        }

        // LOCAL

        if(hideAboveXLocal) {
            if(posLocal.x > hideAboveXLocalVal) {
                hide = true;
            }
        }

        if(hideAboveYLocal) {
            if(posLocal.y > hideAboveYLocalVal) {
                hide = true;
            }
        }

        if(hideAboveZLocal) {
            if(posLocal.z > hideAboveZLocalVal) {
                hide = true;
            }
        }

        // state

        if(hide) {
            if(!hidden) {
                gameObject.HideChildren();
                hidden = true;
            }

        }
        else {

            if(hidden) {
                gameObject.ShowChildren();
                hidden = false;
            }
        }


    }
}
