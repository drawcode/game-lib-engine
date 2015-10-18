using UnityEngine;
using System.Collections;

public class EditorApplicationUtil : MonoBehaviour {

    // Use this for initialization
    void Start() {
    
    }
    
    // Update is called once per frame
    void Update() {
    
        if (Application.isEditor) {
        
            if (Input.GetKey(KeyCode.LeftControl)) {
                if (Input.GetKeyDown(KeyCode.Space)) {

                    // Toggle paused
                    UnityEditor.EditorApplication.isPaused = !UnityEditor.EditorApplication.isPaused ? true : false;

                }
            }
        }
    }
}
