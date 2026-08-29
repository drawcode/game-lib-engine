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

    public int maxPoolItems = 5000;

    // The type of object this pool is handling
    public GameObject prefab;

    // This stores the cached objects waiting to be reactivated
    //public Dictionary<int, GameObject> pool;

    public Queue<GameObject> pool;

    // Mirrors the queue's contents. recycle() has to reject an object that is already
    // parked, and Queue<T>.Contains is a linear scan -- with maxPoolItems at 5000 and
    // bullets, muzzles, shells, hit effects and audio objects all recycling every frame
    // that scan was the most expensive part of returning an object to the pool.
    private HashSet<GameObject> pooledSet = new HashSet<GameObject>();

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
            pooledSet.Remove(obj);

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


        // Add() returns false when it is already parked, which replaces the old
        // linear pool.Contains(obj) scan.
        if (pooledSet.Add(obj)) {
            // put object back in cache for reuse later
            pool.Enqueue(obj);
        }
    }

    public void clear() {

        foreach (GameObject go in pool) {
            go.DestroyNow();
        }

        pool.Clear();
        pooledSet.Clear();
    }

    /// <summary>
    /// Release cached objects beyond <paramref name="keep"/>, oldest first, and return how
    /// many were destroyed.
    ///
    /// This only ever touches objects sitting in the queue -- something that is live in the
    /// world was never enqueued, so it cannot be taken away from its owner. Keeping a few
    /// back matters: `clear()` empties the bucket entirely and the next spawn pays a full
    /// Instantiate, which is exactly the spike a pool exists to avoid.
    /// </summary>
    public int trim(int keep) {

        if (keep < 0) {
            keep = 0;
        }

        int destroyed = 0;

        while (pool.Count > keep) {

            GameObject go = pool.Dequeue();

            pooledSet.Remove(go);

            if (go != null) {
                go.DestroyNow();
                destroyed++;
            }
        }

        return destroyed;
    }
}