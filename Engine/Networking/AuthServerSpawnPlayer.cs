using System.Collections;
using System.Collections.Generic;
using Engine;
using Engine.Data;
using Engine.Game.Controllers;
using Engine.Networking;
using Engine.Utility;
using UnityEngine;

public class AuthServerSpawnPlayer : BaseEngineBehavior {
#if !UNITY_FLASH
    public Transform playerPrefab;

    // Local player information when one is instantiated
    private NetworkPlayer localPlayer;

    private NetworkViewID localTransformViewID;
    private NetworkViewID localAnimationViewID;
    private bool isInstantiated = false;

    // The server uses this to track all intanticated player
    private List<PlayerInfo> playerInfo = new List<PlayerInfo>();

    public class PlayerInfo {
        public NetworkViewID transformViewID;
        public NetworkViewID animationViewID;
        public NetworkPlayer player;
    }

    private void OnGUI() {
        if (Network.isClient && !isInstantiated)
            if (GUI.Button(new Rect(20, Screen.height - 60, 90, 20), "SpawnPlayer")) {

                // Spawn the player on all machines
                networkView.RPC("SpawnPlayer", RPCMode.AllBuffered, localPlayer, localTransformViewID, localAnimationViewID);
                isInstantiated = true;
            }
    }

    // Receive server initialization, record own identifier as seen by the server.
    // This is later used to recognize if a network spawned player is the local player.
    // Also record assigned view IDs so the server can synch the player correctly.
    [RPC]
    private void InitPlayer(NetworkPlayer player, NetworkViewID tViewID, NetworkViewID aViewID) {
        Debug.Log("Received player init " + player + ". ViewIDs " + tViewID + " and " + aViewID);
        localPlayer = player;
        localTransformViewID = tViewID;
        localAnimationViewID = aViewID;
    }

    // Create a networked player in the game. Instantiate a local copy of the player, set the view IDs
    // accordingly.
    [RPC]
    private void SpawnPlayer(NetworkPlayer playerIdentifier, NetworkViewID transformViewID, NetworkViewID animationViewID) {
        Debug.Log("Instantiating player " + playerIdentifier);

        // TODO check this
        Transform instantiatedPlayer = (Transform)Instantiate(playerPrefab, transform.position, transform.rotation);
        NetworkView[] networkViews = instantiatedPlayer.GetComponents<NetworkView>();

        // Assign view IDs to player object
        if (networkViews.Length != 2) {
            Debug.Log("Error while spawning player, prefab should have 2 network views, has " + networkViews.Length);
            return;
        }
        else {
            networkViews[0].viewID = transformViewID;
            networkViews[1].viewID = animationViewID;
        }

        // Initialize local player
        if (playerIdentifier == localPlayer) {
            Debug.Log("Enabling user input as this is the local player");

            // W are doing client prediction and thus enable the controller script + user input processing
            instantiatedPlayer.GetComponent<ThirdPersonController>().enabled = true;
            instantiatedPlayer.GetComponent<ThirdPersonController>().getUserInput = true;

            // Enable input network synchronization (server gets input)
            instantiatedPlayer.GetComponent<NetworkController>().enabled = true;
            instantiatedPlayer.SendMessage("SetOwnership", playerIdentifier);
            return;

            // Initialize player on server
        }
        else if (Network.isServer) {
            instantiatedPlayer.GetComponent<ThirdPersonController>().enabled = true;
            instantiatedPlayer.GetComponent<AuthServerPersonAnimation>().enabled = true;

            // Record player info so he can be destroyed properly
            PlayerInfo playerInstance = new PlayerInfo();
            playerInstance.transformViewID = transformViewID;
            playerInstance.animationViewID = animationViewID;
            playerInstance.player = playerIdentifier;
            playerInfo.Add(playerInstance);
            Debug.Log("There are now " + playerInfo.Count + " players active");
        }
    }

    // This runs if the scene is executed from the loader scene.
    // Here we must check if we already have clients connect which must be reinitialized.
    // This is the same procedure as in OnPlayerConnected except we process already
    // connected players instead of new ones. The already connected players have also
    // reloaded the level and thus have a clean slate.
    private void OnNetworkLoadedLevel() {
        if (Network.isServer && Network.connections.Length > 0) {
            foreach (NetworkPlayer p in Network.connections) {
                Debug.Log("Resending player init to " + p);
                NetworkViewID transformViewID = Network.AllocateViewID();
                NetworkViewID animationViewID = Network.AllocateViewID();
                Debug.Log("Player given view IDs " + transformViewID + " and " + animationViewID);
                networkView.RPC("InitPlayer", p, p, transformViewID, animationViewID);
            }
        }
    }

    // Send initalization info to the new player, before that he cannot spawn himself
    private void OnPlayerConnected(NetworkPlayer player) {
        Debug.Log("Sending player init to " + player);
        NetworkViewID transformViewID = Network.AllocateViewID();
        NetworkViewID animationViewID = Network.AllocateViewID();
        Debug.Log("Player given view IDs " + transformViewID + " and " + animationViewID);
        networkView.RPC("InitPlayer", player, player, transformViewID, animationViewID);
    }

    private void OnPlayerDisconnected(NetworkPlayer player) {
        Debug.Log("Cleaning up player " + player);

        // Destroy the player object this network player spawned
        PlayerInfo deletePlayer = new PlayerInfo();
        foreach (PlayerInfo playerInstance in playerInfo) {
            if (player == playerInstance.player) {
                Debug.Log("Destroying objects belonging to view ID " + playerInstance.transformViewID);
                Network.Destroy(playerInstance.transformViewID);
                deletePlayer = playerInstance;
            }
        }
        playerInfo.Remove(deletePlayer);
        Network.RemoveRPCs(player, 0);
        Network.DestroyPlayerObjects(player);
    }

#endif
}