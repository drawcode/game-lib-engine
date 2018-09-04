using System;
using System.Collections.Generic;
using System.IO;
using Engine;
using Engine.Data;
using Engine.Networking;
using Engine.Utility;
using UnityEngine;

namespace Engine.Networking {

    public class BaseNetwork : BaseEngineObject, INetwork {
#if !UNITY_FLASH && NETWORK_USE_UNITY
        public static int DEFAULT_PORT = 25010;
        public static int DEFAULT_FACILITATOR_PORT = 25011;

        public EngineNetworkType networkType { get; set; }

        public EngineNetworkConnectionType networkConnectionType { get; set; }

        private int playerCount = 0;

        public HostData[] hostData;

        public bool hostDataRequested = false;
        public bool hostDataReceived = false;

        // --------------------------------------------------------------------------
        // current Network

        public UnityEngine.Network currentNetwork { get; set; }

        public void SetNetwork(UnityEngine.Network network) {
            currentNetwork = network;
        }

        public UnityEngine.Network GetNetwork() {
            return currentNetwork;
        }

        // --------------------------------------------------------------------------
        // facilitator

        public string facilitatorEndpoint { get; set; }

        public void SetFacilitatorEndpoint(string endpoint) {
            facilitatorEndpoint = endpoint;
        }

        public string GetFacilitatorEndPoint() {
            return facilitatorEndpoint;
        }

        public int facilitatorPort { get; set; }

        public void SetFacilitatorPort(int port) {
            facilitatorPort = port;
        }

        public int GetFacilitatorPort() {
            return facilitatorPort;
        }

        // --------------------------------------------------------------------------
        // server

        public string serverEndpoint { get; set; }

        public void SetServerEndpoint(string endpoint) {
            serverEndpoint = endpoint;
        }

        public string GetServerEndPoint() {
            return serverEndpoint;
        }

        public int serverPort { get; set; }

        public void SetServerPort(int port) {
            serverPort = port;
        }

        public int GetServerPort() {
            return serverPort;
        }

        // --------------------------------------------------------------------------
        // connections

        public int currentConnections { get; set; }

        public int GetCurrentConnections() {
            return UnityEngine.Network.connections.Length;
        }

        public int maxConnections { get; set; }

        public int GetMaxConnections() {
            return UnityEngine.Network.maxConnections;
        }

        public NetworkPlayer[] connectedPlayers { get; set; }

        public NetworkPlayer[] GetConnectedPlayers() {
            return UnityEngine.Network.connections;
        }

        // --------------------------------------------------------------------------
        // states

        public bool IsClient() {
            return UnityEngine.Network.isClient;
        }

        public bool IsServer() {
            return UnityEngine.Network.isServer;
        }

        public bool ShouldUseNat() {
            bool useNat = !HasPublicAddress();
            return useNat;
        }

        public bool HasPublicAddress() {
            return UnityEngine.Network.HavePublicAddress();
        }

        private void SetRecievingEnabled(int group, bool enabled) {
            foreach (NetworkPlayer player in UnityEngine.Network.connections) {
                UnityEngine.Network.SetReceivingEnabled(player, group, enabled);
            }
        }

        private void SetLevelPrefix(int prefix) {
            UnityEngine.Network.SetLevelPrefix(prefix);
        }

        // --------------------------------------------------------------------------
        // info

        public int GetPlayerLastPing(NetworkPlayer player) {
            return UnityEngine.Network.GetLastPing(player);
        }

        public int GetPlayerAveragePing(NetworkPlayer player) {
            return UnityEngine.Network.GetAveragePing(player);
        }

        // --------------------------------------------------------------------------
        // server

        public bool StartServer(int connections, int port) {
            UnityEngine.Network.InitializeServer(connections, port, true);
            return true;
        }

        public bool StartServer(int connections, int port, bool natEnabled) {
            LogUtil.Log("BaseNetwork::StartServer connections:" + connections.ToString() + "port:" + port.ToString() + "natEnabled:" + natEnabled.ToString());
            UnityEngine.Network.InitializeServer(connections, port, natEnabled);
            return true;
        }

        public bool SetServerPassword(string password) {
            UnityEngine.Network.incomingPassword = password;
            return true;
        }

        // --------------------------------------------------------------------------
        // client

        public bool Connect(string endpoint) {
            LogUtil.Log("BaseNetwork::Connect endpoint:" + endpoint);

            UnityEngine.Network.Connect(endpoint, DEFAULT_PORT);
            return true;
        }

        public bool Connect(string endpoint, int port) {
            LogUtil.Log("BaseNetwork::Connect endpoint:" + endpoint + "port:" + port.ToString());
            UnityEngine.Network.Connect(endpoint, port);
            return true;
        }

        public bool Disconnect() {
            LogUtil.Log("BaseNetwork::Disconnect Unity");
            UnityEngine.Network.Disconnect();
            if (IsServer()) {
                MasterServer.UnregisterHost();
                LogUtil.Log("BaseNetwork::Disconnect UnregisterHost Unity MasterServer");
            }
            return true;
        }

        // --------------------------------------------------------------------------
        // EVENTS

        private void OnConnectedToServer() {
            LogUtil.Log("BaseNetwork::OnConnectedToServer Connected to server");
        }

        private void OnDisconnectedFromServer(NetworkDisconnection info) {
            if (UnityEngine.Network.isServer)
                LogUtil.Log("BaseNetwork::OnDisconnectedFromServer Local server connection disconnected");
            else
                if (info == NetworkDisconnection.LostConnection)
                    LogUtil.Log("BaseNetwork::OnDisconnectedFromServer Lost connection to the server");
                else
                    LogUtil.Log("BaseNetwork::OnDisconnectedFromServer Successfully diconnected from the server");
        }

        private void OnFailedToConnect(NetworkConnectionError error) {
            LogUtil.Log("BaseNetwork::OnFailedToConnect Could not connect to server: " + error);
        }

        private void OnFailedToConnectToMasterServer(NetworkConnectionError info) {
            LogUtil.Log("BaseNetwork::OnFailedToConnectToMasterServer Could not connect to master server: " + info);
        }

        private void OnNetworkInstantiate(NetworkMessageInfo info) {
            LogUtil.Log("BaseNetwork::OnNetworkInstantiate New object instantiated by " + info.sender);
        }

        private void OnPlayerConnected(NetworkPlayer player) {
            LogUtil.Log("BaseNetwork::OnPlayerConnected Player " + playerCount++ + " connected from " + player.ipAddress + ":" + player.port);
        }

        private void OnPlayerDisconnected(NetworkPlayer player) {
            LogUtil.Log("BaseNetwork::OnPlayerDisconnected Clean up after player " + player);
            UnityEngine.Network.RemoveRPCs(player);
            LogUtil.Log("BaseNetwork::OnPlayerDisconnected RemoveRPCs player::" + player);
            UnityEngine.Network.DestroyPlayerObjects(player);
            LogUtil.Log("BaseNetwork::OnPlayerDisconnected DestroyPlayerObjects player::" + player);
        }

        private void OnServerInitialized() {
            LogUtil.Log("BaseNetwork::OnServerInitialized Server initialized and ready");
        }

        // --------------------------------------------------------------------------
        // Network

        // Master server

        public void RequestHostList(string listName) {

            // Make sure list is empty and request a new list
            MasterServer.ClearHostList();
            MasterServer.RequestHostList(listName);
            hostDataRequested = true;
        }

        public void PollHostList() {
            MasterServer.PollHostList();
        }

        public void ClearHostList() {
            MasterServer.ClearHostList();
            hostDataRequested = false;
            hostDataReceived = false;
        }

        public virtual void Tick(float deltaTime) {

            // Tick MasterServer
            if (MasterServer.PollHostList().Length != 0) {
                hostData = MasterServer.PollHostList();
                hostDataReceived = false;

                for (int i = 0; i < hostData.Length; i++) {
                    LogUtil.Log("Game name: " + hostData[i].gameName);
                }
                MasterServer.ClearHostList();
            }
        }

#endif
    }
}