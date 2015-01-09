using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// To use the ObjectPoolManager, you simply need to replace your regular object 
// creation & destruction calls by ObjectPoolManager.createPooled()
// and ObjectPoolManager.destroyPooled(). Here's an exemple:
//
// 1) Without using the ObjectPoolManager:
// Projectile bullet = Instantiate( bulletPrefab, position, rotation ) as Projectile;
// Destroy( bullet.gameObject );
//
// 2) Using the ObjetPoolManager:
// Projectile bullet = ObjectPoolManager.createPooled( bulletPrefab.gameObject, position, rotation ).GetComponent<Bullet>();
// ObjectPoolManager.destroyPooled( bullet.gameObject );
//
// When a recycled object is revived from the cache, the ObjectPoolManager calls 
// its Start() method again, so this object can reset itself as
// if it just got newly created.
//
// When using the ObjectPoolManager with your objects, you need to keep several 
// things in mind:
// 1. You need to be in full control of the creation and destruction of the object 
//    (so they go through ObjectPoolManager). This means you shouldn't
//    use it on objects that use exotic destruction methods (e.g. auto-destroy option on 
//    particle effects) because the ObjectPoolManager will
//    not be able to recycle the object
// 2. When they get revived from the ObjectPoolManager cache, the pooled objects are 
//    responsible for re-initializing themselves as if they had
//    just been newly created via a regular call Instantiate(). So look out for any 
//    dynamic component additions and modifications of the initial
//    object public fields during gameplay

public class ObjectPoolKeyed : MonoBehaviour {

    public ObjectPoolKeyedItem data;

    public void Start() {
        data = new ObjectPoolKeyedItem();
    }
}

public class ObjectPoolKeyedItem : GameDataObject {
    public ObjectPoolKeyedItem() {
        uid = System.Guid.NewGuid().ToString();
    }
}

public class ObjectPoolKeyedManager : GameObjectBehavior {
#if UNITY_EDITOR

    // turn this on to activate debugging information
    public bool debug = false;

    // the GUI block where the debugging info will be displayed
    public Rect debugGuiRect = new Rect( 5, 200, 160, 400 );

#endif

    public int maxPerPool = 100;

    // This maps a prefab to its ObjectPool
    private Dictionary<string, ObjectPool> prefab2pool;

    // This maps a game object instance to the ObjectPool that created/recycled it
    private Dictionary<string, ObjectPool> instance2pool;

    // Only one ObjectPoolManager can exist. We use a singleton pattern to enforce this.
    private static ObjectPoolKeyedManager _instance = null;

    public static ObjectPoolKeyedManager instance {
        get {
            if (!_instance) {

                // check if an ObjectPoolManager is already available in the scene graph
                _instance = FindObjectOfType(typeof(ObjectPoolKeyedManager)) as ObjectPoolKeyedManager;

                // nope, create a new one
                if (!_instance) {
                    var obj = new GameObject("_ObjectPoolKeyedManager");
                    _instance = obj.AddComponent<ObjectPoolKeyedManager>();
                }
            }

            return _instance;
        }
    }

    private void OnApplicationQuit() {

        // release reference on exit
        _instance = null;
    }

    #region Public Interface (static for convenience)

    // Create a pooled object. This will either reuse an object from the cache or allocate a new one
    public static GameObject createPooled(string key, GameObject prefab, Vector3 position, Quaternion rotation) {

        return instance.internalCreate(key, prefab, position, rotation);
    }

    public static GameObject setPooled(string key, GameObject inst) {
        
        return instance.internalSet(key, inst);
    }

    public static bool hasPooled(string key) {
        
        return instance.internalHasPooled(key);
    }

    // Destroy the object now
    public static void destroyPooled(GameObject obj) {
        instance.internalDestroy(obj);
    }

    // Destroy the object after <delay> seconds have elapsed
    public static void destroyPooled(GameObject obj, float delay) {
        if (obj == null) { 
            return;
        }

        if (instance == null) {
            return;
        }

        instance.StartCoroutine(instance.internalDestroy(obj, delay));
    }

    #endregion

    #region Private implementation

    // Constructor
    private void Awake() {
        prefab2pool = new Dictionary<string, ObjectPool>();
        instance2pool = new Dictionary<string, ObjectPool>();
    }

    private ObjectPool createPool(string key, GameObject prefab) {
        var pool = new ObjectPool();
        pool.prefab = prefab;
        pool.key = key;

        return pool;
    }

    private bool internalHasPooled(string key) {
        return prefab2pool.ContainsKey(key);
    }

    private GameObject internalCreate(string key, GameObject prefab, Vector3 position, Quaternion rotation) {
        ObjectPool pool;

        if (prefab2pool == null || prefab == null) {
            return null;
        }

        if (!prefab2pool.ContainsKey(key)) {
            pool = createPool(key, prefab);
            prefab2pool[key] = pool;
        }
        else {
            pool = prefab2pool[key];
        }
        
        //if(pool.Count > maxPerPool) {
        //    LogUtil.Log("ObjectPool: Too many items in the pool!: " + prefab.name);
        //   return null;
        //}

        // create a new object or reuse an existing one from the pool
        GameObject obj = pool.instantiate(position, rotation);

        if (!obj.Has<ObjectPoolKeyed>()) {
            var keyedObject = obj.AddComponent<ObjectPoolKeyed>();
            keyedObject.data.key = key;
        }
        
        if (obj == null) {
            return null;
        }

        // remember which pool this object was created from
        instance2pool[key] = pool;

        return obj;
    }
    
    private GameObject internalSet(string key, GameObject obj) {

        //if(pool.Count > maxPerPool) {
        //    LogUtil.Log("ObjectPool: Too many items in the pool!: " + prefab.name);
        //   return null;
        //}
        
        if (!obj.Has<PoolGameObject>()) {
            obj.AddComponent<PoolGameObject>();
        }        

        var keyedObject = obj.GetOrSet<ObjectPoolKeyed>();
        keyedObject.data.key = key;

        
        if (obj == null) {
            return null;
        }
        
        // remember which pool this object was created from
        //instance2pool[key] = pool;
        
        return obj;
    }


    private void internalDestroy(GameObject obj) {

        string key = "";

        ObjectPoolKeyed keyedObject = obj.Get<ObjectPoolKeyed>();

        bool hasKeyed = obj.Has<ObjectPoolKeyed>();

        if(hasKeyed) {
            keyedObject = obj.Get<ObjectPoolKeyed>();  
            key = keyedObject.data.key;
        }

        if (!string.IsNullOrEmpty(key) && instance2pool.ContainsKey(key)) {

            //LogUtil.Log( "Recyling object " + obj.name );
            var pool = instance2pool[key];
            pool.recycle(obj);
        }
        else {

            // This object was not created through the ObjectPoolManager, give a warning and destroy it the "old way"
            // LogUtil.Log("Destroying non-pooled object " + obj.name);
            Object.Destroy(obj);
        }
    }

    // must be run as coroutine
    private IEnumerator internalDestroy(GameObject obj, float delay) {
        yield return new WaitForSeconds(delay);
        internalDestroy(obj);
    }

    #endregion

#if UNITY_EDITOR
    void OnGUI() {
        if( debug ) {
            GUILayout.BeginArea( debugGuiRect );
            GUILayout.BeginVertical();

            GUILayout.Label( "Pools: " + prefab2pool.Count );

            foreach( var pool in prefab2pool.Values )
                GUILayout.Label( pool.prefab.name + ": " + pool.Count );
            
            //foreach( var pool in instance2pool.Values )
            //    GUILayout.Label( "instance: " + pool.prefab.name + ": " + pool.Count );

            GUILayout.EndVertical();
            GUILayout.EndArea();
        }
    }
#endif
}