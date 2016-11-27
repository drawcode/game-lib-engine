using UnityEngine;
using System.Collections;

public class PrefabLoaderReference : MonoBehaviour {

    public GameObject prefabObject;
    public GameObject container;

    void Start () {
        LoadPrefab();
	}

    void LoadPrefab() {
        
        if(container == null) {
            container = gameObject;
        }
        
        if(prefabObject != null) {
            GameObject go = GameObjectHelper.CreateGameObject(
                prefabObject, Vector3.zero, Quaternion.identity, true);
            go.transform.parent = gameObject.transform;
            go.TrackObject(gameObject);
        }
    }
	
	void Update () {
	
	}
}
