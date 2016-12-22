using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

/*
public class ObjectPoolItem {
    public GameObject parentObject;
    public GameObject itemObject;

}
*/

// The ObjectPool is the storage class for pooled objects of the same kind (e.g. "Pistol Bullet", or "Enemy A")
// This is used by the ObjectPoolManager and is not meant to be used separately

public class ObjectPool : System.Object {

    public int maxPoolItems = 5;

    // The type of object this pool is handling
    public GameObject prefab;

    // This stores the cached objects waiting to be reactivated
    //public Dictionary<int, GameObject> pool;

    public Queue<GameObject> pool;

    public string key = "default";

    // How many objects are currently sitting in the cache
    public int Count {
        get { return pool.Count; }
    }

    public ObjectPool() {
        pool = new Queue<GameObject>();
    }

    public GameObject instantiate(Vector3 position, Quaternion rotation) {
        GameObject obj;

        if (pool.Count > maxPoolItems) {
            //return null;
        }

        // if we don't have any object already in the cache, create a new one
        if (pool.Count == 0 || pool.Count > maxPoolItems) {
            obj = UnityEngine.Object.Instantiate(prefab, position, rotation) as GameObject;
        }
        else { // else pull one from the cache
            obj = pool.Dequeue();

            // reactivate the object
            obj.transform.parent = null;
            obj.transform.position = position;
            obj.transform.rotation = rotation;
            obj.SetActive(true);

            // Call Start again
            obj.SendMessage("Start", SendMessageOptions.DontRequireReceiver);
        }

        return obj;
    }

    // put the object in the cache and deactivate it
    public void recycle(GameObject obj, string key = null) {

        if (obj == null) {
            return;
        }
        
        if (pool.Count > maxPoolItems) {
            obj.DestroyGameObject(0, false);
            return;            
        }

        // deactivate the object
        obj.SetActive(false);

        // put the recycled object in this ObjectPool's bucket

        if (!string.IsNullOrEmpty(key)) {
            obj.transform.parent = ObjectPoolKeyedManager.instance.gameObject.transform;
        }
        else {
            obj.transform.parent = ObjectPoolManager.instance.gameObject.transform;
        }


        if (!pool.Contains(obj)) {
            // put object back in cache for reuse later
            pool.Enqueue(obj);
        }
    }
}