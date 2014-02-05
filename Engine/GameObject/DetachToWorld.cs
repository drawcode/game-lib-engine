using UnityEngine;
using System.Collections;

public class DetatchToWorld : MonoBehaviour {

    public void Start() {
        transform.parent = null;
    }
    
    public void  Update() {
    
    }
}