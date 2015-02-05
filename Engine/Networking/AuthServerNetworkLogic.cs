using System.Collections;
using Engine;
using Engine.Data;
using Engine.Networking;
using Engine.Utility;
using UnityEngine;

public class AuthServerNetworkLogin : BaseEngineBehavior {
    #if NETWORK_UNITY

    private void Start() {
        MasterServer.dedicatedServer = true;
    }

    private void OnGUI() {
        if (Network.isServer)
            GUI.Label(new Rect(20, Screen.height - 50, 200, 20), "Running as a dedicated server");
    }

#endif
}