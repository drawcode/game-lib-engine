using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

using UnityEngine;

public class NetworkUtil {	
	public static bool HasNetworkAccess {
		get {
			if(Application.internetReachability != NetworkReachability.NotReachable) {
				return true;
			}	
			return false;
		}		
	}
	
	public static bool HasConnectionViaLocalNetwork {
		get {
			if(Application.internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork) {
				return true;
			}	
			return false;
		}		
	}

	public static bool HasConnectionViaCarrierNetwork {
		get {
			if(Application.internetReachability == NetworkReachability.ReachableViaCarrierDataNetwork) {
				return true;
			}	
			return false;
		}		
	}
	
}
	
