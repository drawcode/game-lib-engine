using System;
using Engine;
using UnityEngine;

namespace Engine.Networking {

    public class SpawnPrefab : BaseEngineBehavior {
#if !UNITY_FLASH

        public Transform playerPrefab;

        private void OnNetworkLoadedLevel() {
            Network.Instantiate(playerPrefab, transform.position, transform.rotation, 0);
        }

        private void OnPlayerDisconnected(NetworkPlayer player) {
            Debug.Log("Server destroying player");
            Network.RemoveRPCs(player, 0);
            Network.DestroyPlayerObjects(player);
        }

#endif
    }
}