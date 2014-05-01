using System;
using System.Collections.Generic;
using Engine.Networking;
using UnityEngine;

public class MatchupContainer : GameObjectBehavior {
#if !UNITY_FLASH
    public string trackName;
    public UnityNetworking unityNetworkingComponent;
    public MatchupGame matchupGameComponent;
    public Transform playerPrefab;

    public void Awake() {
        AttachComponents();
        DontDestroyOnLoad(gameObject);
    }

    public void Prepare(Transform transform) {
        matchupGameComponent.playerPrefab = transform;
    }

    public void AttachComponents() {
        gameObject.AddComponent<NetworkView>();
        unityNetworkingComponent = gameObject.AddComponent<UnityNetworking>();
        matchupGameComponent = gameObject.AddComponent<MatchupGame>();
        matchupGameComponent.playerPrefab = playerPrefab;
    }

    public void DestroyMatchup() {
        DestroyObject(gameObject.transform);
    }

#endif
}