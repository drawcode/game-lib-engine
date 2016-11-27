using UnityEngine;
using System.Collections;

public class GameObjectVisibility : MonoBehaviour {
	
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



	void Start () {
		
	}

    void Update() {

        if(hideBelowXWorld) {
        }

        if(hideBelowXWorld) {
        }
    }
}
