using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class DeviceOrientationMessages {
	public static string deviceOrientationChange = "device-orientation-change";
	public static string deviceScreenRatioChange = "device-screen-ratio-change";
}

public class DeviceOrientationUtil {

    private static volatile DeviceOrientationUtil instance;
    private static System.Object syncRoot = new System.Object();

    public DeviceOrientation currentDeviceOrientation;
    public ScreenOrientation currentScreenOrientation;

    public static DeviceOrientationUtil Instance {
        get {
            if (instance == null) {
                lock (syncRoot) {
                    if (instance == null)
                        instance = new DeviceOrientationUtil();
                }
            }

            return instance;
        }
    }

    public DeviceOrientationUtil() {
        Init();
    }

    public void Init() {
        currentScreenOrientation = Screen.orientation;
        currentDeviceOrientation = Input.deviceOrientation;
    }

}