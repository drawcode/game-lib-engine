using System.Collections;
using Engine;
using Engine.Data;
using Engine.Networking;
using Engine.Utility;
using UnityEngine;

namespace Engine.Networking {
    public class PingTester : BaseEngineBehavior {
#if !UNITY_FLASH
#if !UNITY_IPHONE
        private string host = "83.221.146.11";
        private PingWin pinger;
        private int pingTime = 0;

        private void Awake() {

            // Choose correct ping class according to platform
            if (System.Environment.OSVersion.Platform == System.PlatformID.Unix) {

                //pinger = GetComponent<PingCustom>();
            }
            else {
                pinger = GetComponent<PingWin>();
            }
        }

        private void OnGUI() {
            host = GUILayout.TextField(host, GUILayout.Width(100));
            if (GUILayout.Button("Ping")) {
                pingTime = pinger.Ping(host, 1000);

                // Retry ping if a bogus value is returned
                if (pingTime < 0 || pingTime > 1500)
                    pingTime = pinger.Ping(host, 1000);
            }
            if (pingTime == -1)
                GUILayout.Label("Ping NOT supported in current player");
            else
                GUILayout.Label(pingTime + " ms");
        }

#endif
#endif
    }
}