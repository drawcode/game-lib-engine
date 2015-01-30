using System;
using Engine;
using UnityEngine;

namespace Engine.Networking {
    public class SpawnPrefab : BaseEngineBehavior {
        
        #if NETWORK_UNITY

        public Transform playerPrefab;

        private void OnNetworkLoadedLevel() {
            Network.Instantiate(playerPrefab, transform.position, transform.rotation, 0);
        }

        private void OnPlayerDisconnected(NetworkPlayer player) {
            LogUtil.Log("Server destroying player");
            Network.RemoveRPCs(player, 0);
            Network.DestroyPlayerObjects(player);
        }

#endif
    }
}