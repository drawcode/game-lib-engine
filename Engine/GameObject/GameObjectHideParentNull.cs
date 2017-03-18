using UnityEngine;
using System.Collections;

public class GameObjectHideParentNull : MonoBehaviour {
		
	void Start () {
    //    gameObject.Hide();	
	}

    void Update() {
        if(transform.parent == null) {
            gameObject.DestroyGameObject();
        }
    }
}
