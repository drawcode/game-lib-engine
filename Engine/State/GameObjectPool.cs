using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Engine.State {
    public class GameObjectPoolItemMeta {
        public GameObject go;
        public int amountToBuffer = 3;
        public string name = "";
    }

    public class GameObjectPool : MonoBehaviour {
        public static GameObjectPool instance;

        /// <summary>
        /// The object prefabs which the pool can handle.
        /// </summary>
        public Dictionary<string, GameObjectPoolItemMeta> objectPrefabs;

        /// <summary>
        /// The pooled objects currently available.
        /// </summary>
        public Dictionary<string, List<GameObject>> pooledObjects;

        /// <summary>
        /// The amount of objects of each type to buffer.
        /// </summary>
        public int[] amountToBuffer;
        public int defaultBufferAmount = 3;

        /// <summary>
        /// The container object that we will keep unused pooled objects so we dont clog up the editor with objects.
        /// </summary>
        protected GameObject containerObject;

        private void Awake() {
            instance = this;
        }

        // Use this for initialization
        private void Start() {
            containerObject = new GameObject("GameObjectPool");
        }

        public void CachePrefab(string prefabName, int bufferAmount) {

            //Loop through the object prefabs and make a new list for each one.
            //We do this because the pool can only support prefabs set to it in the editor,
            //so we can assume the lists of pooled objects are in the same order as object prefabs in the array
            pooledObjects = new Dictionary<string, List<GameObject>>();

            int i = 0;
            foreach (GameObjectPoolItemMeta item in objectPrefabs.Values) {

                // pooledObjects[item.name][i] = new Dictionary<string, List<GameObject>>();

                for (int n = 0; n < bufferAmount; n++) {
                    GameObject newObj = Instantiate(Resources.Load(prefabName)) as GameObject;
                    newObj.name = prefabName;
                    PoolObject(newObj);
                    Debug.Log("item:" + item.name);
                }

                i++;
            }
        }

        /*
    public void CachePrefab(UnityEngine.Ob objectPrefab, int bufferAmount) {

        //Loop through the object prefabs and make a new list for each one.
        //We do this because the pool can only support prefabs set to it in the editor,
        //so we can assume the lists of pooled objects are in the same order as object prefabs in the array
        pooledObjects = new List<GameObject>[objectPrefabs.Length];

        int i = 0;
        foreach ( GameObject objectPrefab in objectPrefabs ) {
            pooledObjects[i] = new List<GameObject>();

            int bufferAmountItem;

            if(i < amountToBuffer.Length) bufferAmountItem = amountToBuffer[i];
            else
                bufferAmountItem = defaultBufferAmount;

            for ( int n=0; n<bufferAmountItem; n++) {
                GameObject newObj = Instantiate(objectPrefab) as GameObject;
                newObj.name = objectPrefab.name;
                PoolObject(newObj);
            }

            i++;
        }
    }
    */

        /// <summary>
        /// Gets a new object for the name type provided.  If no object type exists or if onlypooled is true and there is no objects of that type in the pool
        /// then null will be returned.
        /// </summary>
        /// <returns>
        /// The object for type.
        /// </returns>
        /// <param name='objectType'>
        /// Object type.
        /// </param>
        /// <param name='onlyPooled'>
        /// If true, it will only return an object if there is one currently pooled.
        /// </param>
        public GameObject GetObjectForType(string objectType, bool onlyPooled) {
            if (pooledObjects.ContainsKey(objectType)) { 
                foreach (GameObject go in pooledObjects[objectType]) {
                    GameObject pooledObject = go;

                    //pooledObjects[i].RemoveAt(0);
                    pooledObject.transform.parent = null;
                    pooledObject.SetActive(true);
                    return pooledObject;
                }
            }
            else if (!onlyPooled) {
                return Instantiate(Resources.Load(objectPrefabs[objectType].name)) as GameObject;
            }

            return null;
        }

        /// <summary>
        /// Pools the object specified.  Will not be pooled if there is no prefab of that type.
        /// </summary>
        /// <param name='obj'>
        /// Object to be pooled.
        /// </param>
        public void PoolObject(GameObject obj) {
            /*
            for ( int i=0; i<objectPrefabs.Length; i++)
            {
                if(objectPrefabs[i].name == obj.name)
                {
                    obj.SetActive(false);
                    obj.transform.parent = containerObject.transform;
                    pooledObjects[i].Add(obj);
                    return;
                }
            }
            */
        }
    }
}