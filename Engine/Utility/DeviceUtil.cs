using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeviceUtil : MonoBehaviour {
    private static DeviceUtil instance;

    private static DeviceUtil Instance {
        get {
            if (instance == null) {
                instance = ObjectUtil.FindObject<DeviceUtil>();
                if (instance == null) {
                    instance =
                        new GameObject("DeviceUtil", typeof(DeviceUtil))
                            .GetComponent<DeviceUtil>();
                }
            }
            return instance;
        }
    }
	
	void Start() {
	
	}
	
	public static void Vibrate() {

        if(!GameProfiles.Current.GetControlVibrate()) {
            return;
        }

#if UNITY_IPHONE || UNITY_ANDROID
        if(!Application.isEditor) {
	        Handheld.Vibrate();
        }
#endif
	}


}