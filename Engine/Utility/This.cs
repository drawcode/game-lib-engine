using System;
using System.Collections.Generic;
using UnityEngine;

public class This : GameObjectBehavior {

    // Only one ObjectPoolManager can exist. We use a singleton pattern to enforce this.

    private static This _instance = null;
    
    public static This instance {
        get {
            if (!_instance) {
                
                // check if an This is already available in the scene graph
                _instance = FindAnyObjectByType(typeof(This)) as This;
                
                // nope, create a new one
                if (!_instance) {
                    var obj = new GameObject("_This");
                    _instance = obj.AddComponent<This>();
                }
            }
            
            return _instance;
        }
    }
    
    private void OnApplicationQuit() {
        
        // release reference on exit
        _instance = null;
    }

    // time 

    private static volatile ThisTime instTime;
    private static System.Object syncRoot = new System.Object();
        
    public static ThisTime time {
        get {
            if (instTime == null) {
                lock (syncRoot) {
                    if (instTime == null)
                        instTime = new ThisTime();
                }
            }
            
            return instTime;
        }
    }

    public float currentTime = 0;

    public void Update() {
        currentTime += Time.deltaTime;
    }

}

public class ThisTime {

    public Dictionary<string, float> times = new Dictionary<string, float>();

    public bool timeHasPassed(string key, float amount) {
    
        float timeCurrent = 0f;
        if(times.ContainsKey(key)) {
            timeCurrent = times[key];
            LogUtil.Log("timeCurrent" + timeCurrent);
        }

        return false;
    }
}