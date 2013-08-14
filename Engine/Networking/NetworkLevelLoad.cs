using System.Collections;
using System.Collections.Generic;
using Engine;
using Engine.Data;
using Engine.Networking;
using Engine.Utility;
using UnityEngine;

namespace Engine.Networking {
#if !UNITY_FLASH

    [RequireComponent(typeof(NetworkView))]
    public class NetworkLevelLoad : BaseEngineBehavior {

        //string[] supportedNetworkLevels = new string[0];// = ["mylevel"];
        public List<string> supportedNetworkLevels = new List<string>();

        public string disconnectedLevel = "loader";

        // Keep track of the last level prefix (increment each time a new level loads)
        private int lastLevelPrefix = 0;

        private void Awake() {

            // Network level loading is done in a seperate channel.
            DontDestroyOnLoad(this);
            supportedNetworkLevels.Add("mylevel");
            networkView.group = 1;
            Application.LoadLevel(disconnectedLevel);
        }

        private void OnGUI() {

            // When network is running (server or client) then display the levels
            // configured in the supportedNetworkLevels array and allow them to be loaded
            // at the push of a button
            if (Network.peerType != NetworkPeerType.Disconnected) {
                GUILayout.BeginArea(new Rect(0, Screen.height - 30, Screen.width, 30));
                GUILayout.BeginHorizontal();

                foreach (string level in supportedNetworkLevels) {
                    if (GUILayout.Button(level)) {

                        // Make sure no old RPC calls are buffered and then send load level command
                        Network.RemoveRPCsInGroup(0);
                        Network.RemoveRPCsInGroup(1);

                        // Load level with incremented level prefix (for view IDs)
                        networkView.RPC("LoadLevel", RPCMode.AllBuffered, level, lastLevelPrefix + 1);
                    }
                }
                GUILayout.FlexibleSpace();
                GUILayout.EndHorizontal();
                GUILayout.EndArea();
            }
        }

        [RPC]
        public IEnumerator LoadLevel(string level, int levelPrefix) {
            Debug.Log("Loading level " + level + " with prefix " + levelPrefix);
            lastLevelPrefix = levelPrefix;

            // There is no reason to send any more data over the network on the default channel,
            // because we are about to load the level, thus all those objects will get deleted anyway
            Network.SetSendingEnabled(0, false);

            // We need to stop receiving because first the level must be loaded.
            // Once the level is loaded, RPC's and other state update attached to objects in the level are allowed to fire
            Network.isMessageQueueRunning = false;

            // All network views loaded from a level will get a prefix into their NetworkViewID.
            // This will prevent old updates from clients leaking into a newly created scene.
            Network.SetLevelPrefix(levelPrefix);
            Application.LoadLevel(level);
            yield return 0;
            yield return 0;

            // Allow receiving data again
            Network.isMessageQueueRunning = true;

            // Now the level has been loaded and we can start sending out data
            Network.SetSendingEnabled(0, true);

            // Notify our objects that the level and the network is ready
            foreach (GameObject go in FindObjectsOfType(typeof(GameObject)))
                go.SendMessage("OnNetworkLoadedLevel", SendMessageOptions.DontRequireReceiver);
        }

        private void OnDisconnectedFromServer() {
            Application.LoadLevel(disconnectedLevel);
        }
    }

#endif
}