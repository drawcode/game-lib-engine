using System;
using System.Collections;
using System.Collections.Generic;
using Engine.Utility;
using UnityEngine;

namespace Engine.Networking {
    
    #if NETWORK_USE_UNITY
    public enum MatchupState {
        MATCHUP_NOT_STARTED = 0,
        MATCHUP_FINDING,
        MATCHUP_SERVER_STARTING,
        MATCHUP_SERVER_STARTED,
        MATCHUP_SERVER_CONNECTING,
        MATCHUP_SERVER_HOST_STARTING,
        MATCHUP_SERVER_HOST_STARTED,
        MATCHUP_SERVER_HOST_FOUND,
        MATCHUP_SERVER_HOST_NOT_FOUND,
        MATCHUP_WAITING_FOR_CLIENT,
        MATCHUP_CLIENT_CONNECTED,
        MATCHUP_MADE,
        MATCHUP_READY
    }

    public class NetworkPlayerInfo {
        public NetworkPlayer networkPlayer;
        public string name;
        public Transform transform;
        public string deviceId;
        public bool isLocal;
        public Dictionary<string, string> attributes;
        private string localDeviceId;

        public NetworkPlayerInfo() {
            Reset();
        }

        public void Reset() {
            networkPlayer = Network.player;
            name = "Player 1";
            transform = null;
            isLocal = true;
            localDeviceId = SystemInfo.deviceUniqueIdentifier;
            if (Application.platform == RuntimePlatform.Android
                || Application.platform == RuntimePlatform.IPhonePlayer) {
                localDeviceId = SystemInfo.deviceUniqueIdentifier;
            }
            deviceId = localDeviceId;
            attributes = new Dictionary<string, string>();
        }

        public NetworkPlayerInfo Clone() {
            NetworkPlayerInfo np = new NetworkPlayerInfo();
            np.networkPlayer = networkPlayer;
            np.name = name;
            np.transform = transform;
            np.isLocal = isLocal;
            np.deviceId = deviceId;
            np.attributes = attributes;
            return np;
        }
    }

    public class MatchupGame : GameObjectBehavior {
        private Transform localTransform;
        private List<NetworkPlayerInfo> playerList;
        private bool autoJoinRunning = false;
        private HostData[] hostData = null;
        public string currentPlayerName;
        public int MAX_CONNECTIONS = 4;
        public Transform playerPrefab;
        public Transform spawnObject;
        public static MatchupGame Instance;
        public bool matchupStarted = false;
        public GUIStyle customGUIStyle;
        public Camera mainCamera;
        public bool useExistingTransforms = true;
        public string matchmakingStatus;
        public string debugMatchmakingStatus;
        public string localDeviceId;
        public MatchupState currentMatchupState = MatchupState.MATCHUP_NOT_STARTED;
        public bool pseudoMultiplayer = false; // multiplayer faked with AI

        public bool IsMatchupFilled {
            get {
                return playerList.Count == MAX_CONNECTIONS;
            }
        }

        public void Awake() {
            Instance = this;

            useExistingTransforms = true;

            //localDeviceId = Puid.New();

            if (Application.platform == RuntimePlatform.Android
                || Application.platform == RuntimePlatform.IPhonePlayer) {
                localDeviceId = SystemInfo.deviceUniqueIdentifier;
            }

            playerList = new List<NetworkPlayerInfo>();

            mainCamera = Camera.main;

            currentPlayerName = "player " + UnityEngine.Random.Range(1, 50000);
            PlayerPrefs.SetString("playerName", currentPlayerName);

            matchmakingStatus = "Preparing your player data...";
            debugMatchmakingStatus = "Testing connection for NAT...";
        }

        public void ChangeState(MatchupState matchState) {
            currentMatchupState = matchState;
        }

        public void CreateMatchupSession() {
            StopAllCoroutines();

            UnityNetworking.Instance.SetupMasterServer();

            StartCoroutine(WaitForLevelInit());

            InvokeRepeating("ServerRegisterHost", 0, 300);

            matchmakingStatus = "Preparing connection...";

            Network.minimumAllocatableViewIDs = 1;

            Network.isMessageQueueRunning = true;

            if (Network.isServer) {
                ServerStartedExisting();
            }
            else {
                if (Network.isClient) {

                    //Client: Already connected
                }
                else {

                    //No connection: Try connecting, otherwise host
                    StartCoroutine(AutoJoinFeature());
                }
            }

            UnityNetworking.Instance.SetHostListDelegate(FullHostListReceived);

            matchupStarted = true;
        }

        public void RunNetworkAndSpawn() {
            StartCoroutine(WaitForLevelInit());

            InvokeRepeating("ServerRegisterHost", 0, 300);

            Network.isMessageQueueRunning = true;

            if (Network.isServer) {

                //Server
                SpawnLocalPlayer();
                ServerStarted();
            }
            else {
                SpawnLocalPlayer();
                if (Network.isClient) {

                    //Client: Already connected
                }
                else {

                    //No connection: Try connecting, otherwise host
                    StartCoroutine(AutoJoinFeature());
                }
            }

            UnityNetworking.Instance.SetHostListDelegate(FullHostListReceived);

            matchupStarted = true;
        }

        private IEnumerator WaitForLevelInit() {
            yield return new WaitForSeconds(5);
        }

        public int PlayerCount {
            get {
                if (playerList != null)
                    return playerList.Count;
                else
                    return 0;
            }
        }

        public List<NetworkPlayerInfo> GetPlayerList() {
            return playerList;
        }

        public List<NetworkPlayerInfo> GetPlayerListCopy() {
            List<NetworkPlayerInfo> newPlayerList = new List<NetworkPlayerInfo>();
            foreach (NetworkPlayerInfo playerInfo in playerList) {
                newPlayerList.Add(playerInfo);
            }
            return newPlayerList;
        }

        [RPC]
        private void AddPlayer(NetworkPlayer networkPlayer, string pname) {
            NetworkPlayerInfo currentPlayer = GetPlayer(networkPlayer);
            if (currentPlayer != null) {
                LogUtil.Log("NetworkMatchupGame: AddPlayer: Player already exists! Updating...");
                currentPlayer.networkPlayer = networkPlayer;
                currentPlayer.name = pname;

                if (Network.player == networkPlayer || Network.player + "" == "-1") {
                    currentPlayer.isLocal = true;
                }

                playerList.Remove(currentPlayer);
                playerList.Add(currentPlayer);
            }
            else {
                NetworkPlayerInfo np = new NetworkPlayerInfo();
                np.networkPlayer = networkPlayer;
                np.name = pname;

                if (Network.player == networkPlayer || Network.player + "" == "-1") {
                    np.isLocal = true;
                }

                playerList.Add(np);
            }
        }

        public void UpdatePlayerAttributeValue(NetworkPlayer networkPlayer, string key, string keyValue) {
            UpdatePlayerAttributeSync(networkPlayer, key, keyValue);
            networkView.RPC("UpdatePlayerAttributeSync", RPCMode.Others, networkPlayer, key, keyValue);
        }

        [RPC]
        private void UpdatePlayerAttributeSync(NetworkPlayer networkPlayer, string key, string keyValue) {
            NetworkPlayerInfo currentPlayer = GetPlayer(networkPlayer);
            LogUtil.Log("NetworkMatchupGame: UpdatePlayerAttribute: key:" + key + " keyValue: " + keyValue);

            if (currentPlayer != null) {
                LogUtil.Log("NetworkMatchupGame: UpdatePlayerAttribute: Player already exists! Updating...");

                if (currentPlayer.attributes.ContainsKey(key)) {
                    currentPlayer.attributes[key] = keyValue;
                    LogUtil.Log("NetworkMatchupGame: UpdatePlayerAttribute: contained key:" + key + " keyValue: " + keyValue);
                }
                else {
                    currentPlayer.attributes.Add(key, keyValue);
                    LogUtil.Log("NetworkMatchupGame: UpdatePlayerAttribute: add key:" + key + " keyValue: " + keyValue);
                }

                SetPlayerAttributes(networkPlayer, currentPlayer);
            }
            else {
                LogUtil.Log("NetworkMatchupGame: UpdatePlayerAttribute: Player doesn't exist! Adding...");
                NetworkPlayerInfo np = new NetworkPlayerInfo();
                np.networkPlayer = networkPlayer;

                if (np.attributes.ContainsKey(key)) {
                    np.attributes[key] = keyValue;
                }
                else {
                    np.attributes.Add(key, keyValue);
                }

                if (Network.player == networkPlayer || Network.player + "" == "-1") {
                    np.isLocal = true;
                }

                playerList.Add(np);
            }
        }

        private void SetPlayerTransform(NetworkPlayer networkPlayer, Transform pTransform) {
            if (!pTransform) {
                LogUtil.LogError("NetworkMatchupGame: SetPlayersTransform has a NULL playerTransform!");
            }
            NetworkPlayerInfo thePlayer = GetPlayer(networkPlayer);
            if (thePlayer == null) {
                LogUtil.LogError("NetworkMatchupGame: SetPlayersPlayerTransform: No player found!");
            }
            thePlayer.transform = pTransform;
        }

        // -------------------------------------------------------------------------------------
        // PLAYER

        [RPC]
        private void RemovePlayer(NetworkPlayer networkPlayer) {
            NetworkPlayerInfo thePlayer = GetPlayer(networkPlayer);

            Network.RemoveRPCs(networkPlayer);
            if (Network.isServer) {
                Network.DestroyPlayerObjects(networkPlayer);
            }
            if (thePlayer.transform) {
                Destroy(thePlayer.transform.gameObject);
            }
            playerList.Remove(thePlayer);
        }

        public NetworkPlayerInfo GetPlayer(NetworkPlayer networkPlayer) {
            foreach (NetworkPlayerInfo np in playerList) {
                if (np.networkPlayer == networkPlayer) {
                    return np;
                }
            }
            return null;
        }

        public void SetPlayer(NetworkPlayer networkPlayer, NetworkPlayerInfo playerInfo) {
            for (int i = 0; i < playerList.Count; i++) {
                if (playerList[i].networkPlayer == networkPlayer) {
                    playerList[i] = playerInfo;
                }
            }
        }

        // -------------------------------------------------------------------------------------
        // PLAYER ATTRIBUTES

        public void SetPlayerAttributes(NetworkPlayer networkPlayer, NetworkPlayerInfo playerInfo) {
            object syncRoot = new System.Object();
            lock (syncRoot) {
                for (int i = 0; i < playerList.Count; i++) {
                    if (playerList[i].networkPlayer == networkPlayer) {
                        playerList[i].attributes = playerInfo.attributes;
                    }
                }
            }
        }

        public void SetPlayerAttributeValue(NetworkPlayer networkPlayer, string key, string keyValue) {
            UpdatePlayerAttributeValue(networkPlayer, key, keyValue);
        }

        public void SetPlayerAttributeValue(NetworkPlayer networkPlayer, string key, bool keyValue) {
            SetPlayerAttributeValue(networkPlayer, key, System.Convert.ToString(keyValue));
        }

        public void SetPlayerAttributeValue(NetworkPlayer networkPlayer, string key, int keyValue) {
            SetPlayerAttributeValue(networkPlayer, key, System.Convert.ToString(keyValue));
        }

        public void SetPlayerAttributeValue(NetworkPlayer networkPlayer, string key, double keyValue) {
            SetPlayerAttributeValue(networkPlayer, key, System.Convert.ToString(keyValue));
        }

        public int GetPlayerAttributeValueInt(NetworkPlayer networkPlayer, string key) {
            int attValue = 0;
            NetworkPlayerInfo playerInfo = GetPlayer(networkPlayer);
            if (playerInfo.attributes.ContainsKey(key)) {
                string _value = playerInfo.attributes[key];
                if (!string.IsNullOrEmpty(_value)) {
                    int.TryParse(_value, out attValue);
                }
            }
            return attValue;
        }

        public bool GetPlayerAttributeValueBool(NetworkPlayer networkPlayer, string key) {
            bool attValue = false;
            NetworkPlayerInfo playerInfo = GetPlayer(networkPlayer);
            if (playerInfo.attributes.ContainsKey(key)) {
                string _value = playerInfo.attributes[key];
                if (!string.IsNullOrEmpty(_value)) {
                    if (!bool.TryParse(_value, out attValue))
                        attValue = false;
                }
            }
            return attValue;
        }

        // -------------------------------------------------------------------------------------
        // LOCAL PLAYER

        public string GetLocalPlayerAttributeStringValue(string key) {
            string attValue = "";
            try {
                NetworkPlayerInfo playerInfo = GetPlayerLocal();
                if (playerInfo != null) {
                    if (playerInfo.attributes != null) {
                        if (playerInfo.attributes.ContainsKey(key)) {
                            attValue = playerInfo.attributes[key];
                        }
                    }
                }
            }
            catch (System.Exception e) {
                LogUtil.Log("Error:" + e.Message + e.StackTrace);
            }
            return attValue;
        }

        public bool GetLocalPlayerAttributeBoolValue(string key) {
            bool returnValue = false;
            try {
                string attValue = GetLocalPlayerAttributeStringValue(key);
                if (!string.IsNullOrEmpty(attValue)) {
                    bool.TryParse(attValue, out returnValue);
                }
            }
            catch (System.Exception e) {
                LogUtil.Log("Error:" + e.Message + e.StackTrace);
            }
            return returnValue;
        }

        public int GetLocalPlayerAttributeIntValue(string key) {
            int returnValue = 0;
            try {
                string attValue = GetLocalPlayerAttributeStringValue(key);
                if (!string.IsNullOrEmpty(attValue)) {
                    int.TryParse(attValue, out returnValue);
                }
            }
            catch (System.Exception e) {
                LogUtil.Log("Error:" + e.Message + e.StackTrace);
            }
            return returnValue;
        }

        // -------------------------------------------------------------------------------------
        // OPPONENT PLAYER IF MATCHUP (2 PLAYERS)

        public string GetOpponentPlayerAttributeStringValue(string key) {
            string attValue = "";
            try {
                NetworkPlayerInfo playerInfo = GetPlayerOpponent();
                if (playerInfo != null) {
                    if (playerInfo.attributes.ContainsKey(key)) {
                        attValue = playerInfo.attributes[key];
                    }
                }
            }
            catch (System.Exception e) {
                LogUtil.Log("Error:" + e.Message + e.StackTrace);
            }
            return attValue;
        }

        public bool GetOpponentPlayerAttributeBoolValue(string key) {
            bool returnValue = false;
            try {
                string attValue = GetOpponentPlayerAttributeStringValue(key);
                if (!string.IsNullOrEmpty(attValue)) {
                    bool.TryParse(attValue, out returnValue);
                }
            }
            catch (System.Exception e) {
                LogUtil.Log("Error:" + e.Message + e.StackTrace);
            }
            return returnValue;
        }

        public int GetOpponentPlayerAttributeIntValue(string key) {
            int returnValue = 0;
            try {
                string attValue = GetOpponentPlayerAttributeStringValue(key);
                if (!string.IsNullOrEmpty(attValue)) {
                    int.TryParse(attValue, out returnValue);
                }
            }
            catch (System.Exception e) {
                LogUtil.Log("Error:" + e.Message + e.StackTrace);
            }
            return returnValue;
        }

        // -------------------------------------------------------------------------------------
        // PLAYER LOOKUP

        public NetworkPlayerInfo GetPlayerByName(string playerName) {
            foreach (NetworkPlayerInfo np in playerList) {
                if (np.name == playerName) {
                    return np;
                }
            }
            return null;
        }

        public NetworkPlayerInfo GetPlayerByUdid(string udid) {
            foreach (NetworkPlayerInfo np in playerList) {
                if (np.deviceId == udid) {
                    return np;
                }
            }
            return null;
        }

        public NetworkPlayerInfo GetPlayerLocal() {
            try {
                foreach (NetworkPlayerInfo np in playerList) {
                    if (np.networkPlayer == Network.player) {
                        return np;
                    }
                }
            }
            catch {
                LogUtil.Log("MatchupGame: Error getting local player...");
            }
            return null;
        }

        public NetworkPlayerInfo GetPlayerOpponent() {
            try {
                foreach (NetworkPlayerInfo np in playerList) {
                    if (np.networkPlayer != Network.player) {
                        return np;
                    }
                }
            }
            catch {
                LogUtil.Log("MatchupGame: Error getting opponent player...");
            }
            return null;
        }

        private void ServerStarted() {
            matchmakingStatus = "Finding match opponent...";
            debugMatchmakingStatus = "ServerStarted";

            playerList.Clear();

            UnityNetworking.Instance.RegisterHost(currentPlayerName + "s game", "NoComment");

            networkView.RPC("AddPlayer", RPCMode.AllBuffered, Network.player, currentPlayerName);

            NetworkViewID id1 = Network.AllocateViewID();
            if (localTransform != null) {
                SetNetworkViewIDs(localTransform.gameObject, id1);
                SetPlayerTransform(Network.player, localTransform);

                networkView.RPC("SpawnOnNetwork", RPCMode.OthersBuffered, transform.position, transform.rotation, id1, currentPlayerName, false, Network.player);
            }
        }

        private void ServerStartedExisting() {
            matchmakingStatus = "Finding match opponent...";
            debugMatchmakingStatus = "ServerStarted";

            playerList.Clear();

            UnityNetworking.Instance.RegisterHost(currentPlayerName + "s game", "NoComment");

            AddPlayer(Network.player, currentPlayerName);
            networkView.RPC("AddPlayer", RPCMode.AllBuffered, Network.player, currentPlayerName);

            LogUtil.Log("MatchupGame::ServerStartedExisting:  isServer:" + Network.isServer);

            /*
            NetworkViewID id1 = Network.AllocateViewID();
            if(localTransform != null)
            {
                SetNetworkViewIDs(localTransform.gameObject, id1);
                SetPlayerTransform(Network.player, localTransform);

                networkView.RPC("SpawnOnNetwork", RPCMode.OthersBuffered, transform.position, transform.rotation, id1, currentPlayerName, false, Network.player);
            }
            */
        }

        public void ServerRegisterHost() {
            if (!Network.isServer) {
                return;
            }
            UnityNetworking.Instance.RegisterHost(currentPlayerName + "", "NoComment");
        }

        public void SpawnLocalPlayerExisting(string playerName, Transform transform) {
            currentPlayerName = playerName;

            //localTransform = transform;

            NetworkViewID id1 = new NetworkViewID();

            if (Network.isClient) {
                id1 = Network.AllocateViewID();
            }

            AddPlayer(Network.player, currentPlayerName);
            SpawnOnNetworkExistingTransform(id1, playerName, true, Network.player);

            if (Network.isClient) {
                networkView.RPC("AddPlayer", RPCMode.OthersBuffered, Network.player, currentPlayerName);
                networkView.RPC("SpawnOnNetworkExistingTransform", RPCMode.OthersBuffered, id1, playerName, false, Network.player);
            }
        }

        private void SpawnLocalPlayer() {

            //Spawn local player
            Vector3 pos = transform.position;
            Quaternion rot = Quaternion.identity;

            //GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("Spawnpoint");
            pos = spawnObject.position;
            rot = spawnObject.rotation;

            NetworkViewID id1 = Network.AllocateViewID(); // new NetworkViewID()
            if (Network.isClient) {
                id1 = Network.AllocateViewID();
            }

            if (string.IsNullOrEmpty(currentPlayerName)) {
                currentPlayerName = "TESTER";
            }

            AddPlayer(Network.player, currentPlayerName);
            SpawnOnNetwork(pos, rot, id1, currentPlayerName, true, Network.player);
            if (Network.isClient) {
                networkView.RPC("AddPlayer", RPCMode.OthersBuffered, Network.player, currentPlayerName);
                networkView.RPC("SpawnOnNetwork", RPCMode.OthersBuffered, pos, rot, id1, currentPlayerName, false, Network.player);
            }
        }

        [RPC]
        public void SpawnOnNetworkExistingTransform(NetworkViewID id1, string playerName, bool amOwner, NetworkPlayer np) {
            LogUtil.Log("MatchupGame::SpawnOnNetworkExistingTransform:  isServer:" + Network.isServer);

            if (playerPrefab.gameObject != null) {
                if (playerPrefab.gameObject.GetComponent<NetworkView>() == null) {
                    playerPrefab.gameObject.AddComponent<NetworkView>();
                }
            }
            else {
                LogUtil.Log("MatchupGame::SpawnOnNetworkExistingTransform: playerPrefab.gameObject is null");
            }

            SetPlayerTransform(np, playerPrefab);

            SetNetworkViewIDs(playerPrefab.gameObject, id1);

            if (amOwner) {
                localTransform = playerPrefab;
            }
        }

        [RPC]
        public void SpawnOnNetwork(Vector3 pos, Quaternion rot, NetworkViewID id1, string playerName, bool amOwner, NetworkPlayer np) {
            Transform newPlayer = Instantiate(playerPrefab, pos, rot) as Transform;
            SetPlayerTransform(np, newPlayer);

            SetNetworkViewIDs(newPlayer.gameObject, id1);

            if (amOwner) {
                localTransform = newPlayer;
            }
        }

        private void SetNetworkViewIDs(GameObject go, NetworkViewID id1) {
            Component[] nViews = go.GetComponentsInChildren<NetworkView>();
            (nViews[0] as NetworkView).viewID = id1;
        }

        //On client: When just connected to a server
        public IEnumerator OnConnectedToServer() {
            matchmakingStatus = "Match found...";
            debugMatchmakingStatus = "connected to server (client)";
            playerList = new List<NetworkPlayerInfo>();
            networkView.RPC("AddPlayer", RPCMode.AllBuffered, Network.player, currentPlayerName);

            if (localTransform != null && !useExistingTransforms) {
                NetworkViewID id1 = Network.AllocateViewID();
                networkView.RPC("SpawnOnNetwork", RPCMode.OthersBuffered, localTransform.position, localTransform.rotation, id1, currentPlayerName, false, Network.player);
                yield return 0;
                SetPlayerTransform(Network.player, localTransform);
                SetNetworkViewIDs(localTransform.gameObject, id1);
            }
        }

        //On server: When client disconnects
        public void OnPlayerDisconnected(NetworkPlayer player) {
            NetworkPlayerInfo pNode = GetPlayer(player);
            if (pNode != null) {

                //string playerNameLeft = pNode.name;
                //Chat.Instance.addGameChatMessage(playerNameLeft + " left the game");
                if (string.IsNullOrEmpty(pNode.name)) {
                    pNode.name = "Opponent";
                }
                matchmakingStatus = pNode.name + " left the game";
            }
            networkView.RPC("RemovePlayer", RPCMode.All, player);

            Network.RemoveRPCs(player);
            Network.DestroyPlayerObjects(player);
        }

        public void OnPlayerConnected(NetworkPlayer player) {
            NetworkPlayerInfo pNode = GetPlayer(player);
            if (pNode != null) {

                //string playerNameLeft = pNode.name;
                //Chat.Instance.addGameChatMessage(playerNameLeft + " left the game");
                if (string.IsNullOrEmpty(pNode.name)) {
                    pNode.name = "Player 1";
                }
                matchmakingStatus = pNode.name + " joined the game";
            }
        }

        //On server: When this game just switched from non-networking to networked
        public void OnServerInitialized() {
            if (!useExistingTransforms)
                ServerStarted();
            else
                ServerStartedExisting();
        }

        public IEnumerator OnDisconnectedFromServer(NetworkDisconnection info) {
            if (Network.isServer) {

                //We shut down our own server, remove all players except local

                yield return new WaitForSeconds(1);

                List<NetworkPlayerInfo> listCopy = GetPlayerListCopy();

                foreach (NetworkPlayerInfo np in listCopy) {
                    if (np.networkPlayer != Network.player && (np.networkPlayer + "") != "0") {
                        RemovePlayer(np.networkPlayer);
                    }
                }

                //The auto join feature should make sure you get a new connection

                if (!autoJoinRunning) {
                    StartCoroutine(AutoJoinFeature());
                }

                debugMatchmakingStatus = "Successfully disconnected from the server !";
                matchmakingStatus = "Match ended, you left the game...";
                LogUtil.Log("NetworkMatchupGame: " + matchmakingStatus);
            }
            else {
                if (info == NetworkDisconnection.LostConnection) {
                    debugMatchmakingStatus = "Lost connection to the server";
                    matchmakingStatus = "Match ended, opponent left the game...";
                    LogUtil.Log("NetworkMatchupGame: " + debugMatchmakingStatus);
                }
                else {
                    debugMatchmakingStatus = "Successfully disconnected from the server !";
                    matchmakingStatus = "Match ended, you left the game...";
                    LogUtil.Log("NetworkMatchupGame: " + matchmakingStatus);
                }

                //Remove all players except yourself

                yield return new WaitForSeconds(1);

                List<NetworkPlayerInfo> listCopyClone = GetPlayerListCopy();

                foreach (NetworkPlayerInfo np in listCopyClone) {
                    if (np.networkPlayer != Network.player) {
                        RemovePlayer(np.networkPlayer);
                        Network.CloseConnection(np.networkPlayer, false);
                    }
                }

                if (!autoJoinRunning) {
                    StartCoroutine(AutoJoinFeature());
                }
            }

            SendMessageUpwards("OnDisconnectedMatch", SendMessageOptions.DontRequireReceiver);
        }

        public IEnumerator ForceDisconnect() {
            LogUtil.Log("MatchupGame::FORCEDISCONNECT");

            if (Network.isServer) {

                //We shut down our own server, remove all players except local

                yield return new WaitForSeconds(1);

                List<NetworkPlayerInfo> listCopy = GetPlayerListCopy();

                foreach (NetworkPlayerInfo np in listCopy) {
                    if (np.networkPlayer != Network.player && (np.networkPlayer + "") != "0") {
                        RemovePlayer(np.networkPlayer);
                    }
                }

                debugMatchmakingStatus = "Successfully disconnected from the server !";
                matchmakingStatus = "Match connection ended...";
                LogUtil.Log("NetworkMatchupGame: " + matchmakingStatus);

                // TODO check if this is correct...
                Network.Disconnect();
                MasterServer.UnregisterHost();
            }
            else {
                debugMatchmakingStatus = "Successfully disconnected from the server !";
                matchmakingStatus = "Match connection ended...";
                LogUtil.Log("NetworkMatchupGame: " + matchmakingStatus);

                //Remove all players except yourself

                yield return new WaitForSeconds(1);

                List<NetworkPlayerInfo> listCopyClone = GetPlayerListCopy();

                foreach (NetworkPlayerInfo np in listCopyClone) {
                    if (np.networkPlayer != Network.player) {
                        RemovePlayer(np.networkPlayer);
                        Network.CloseConnection(np.networkPlayer, false);
                    }
                }

                Network.Disconnect();
                MasterServer.UnregisterHost();
            }

            SendMessageUpwards("OnDisconnectedMatch", SendMessageOptions.DontRequireReceiver);

            matchupStarted = false;
        }

        //This runs on disconnected clients..trying to connect to every possible host..
        private IEnumerator AutoJoinFeature() {
            autoJoinRunning = true;

            while (!UnityNetworking.Instance.ReadyLoading()) {
                yield return 0;//Wait for masterserver connection and connection tester
            }

            //Try to join games...otherwise..HOST
            while (Network.connections.Length == 0) {
                if (!Network.isServer) {
                    yield return StartCoroutine(AutoJoinTryConnecting());
                }

                //yield return 0;
                if (Network.connections.Length <= 0) {
                    yield return StartCoroutine(AutoJoinTryHosting());
                }
                yield return 0;
            }

            autoJoinRunning = false;
        }

        private void FullHostListReceived() {
            hostData = UnityNetworking.Instance.GetHostData();
        }

        private IEnumerator AutoJoinTryConnecting() {

            //Get host list
            for (int retries = 0; (retries < 3 && (hostData == null || hostData.Length >= 0)); retries++) {
                debugMatchmakingStatus = "fetching host list...";
                matchmakingStatus = "Preparing connection...";
                LogUtil.Log("NetworkMatchupGame: " + matchmakingStatus);
                UnityNetworking.Instance.FetchHostList();

                yield return new WaitForSeconds(2);
            }

            if (hostData == null || hostData.Length == 0) {
                debugMatchmakingStatus = "hostdata is empty...";
                matchmakingStatus = "Creating a match...";
                LogUtil.Log("NetworkMatchupGame: " + debugMatchmakingStatus);

                yield break;
            }

            //Connect to games in the list
            foreach (HostData element in hostData) {
                if (Network.connections.Length > 0) {
                    yield break;
                }

                yield return 0;

                //Cant be full, must be same level
                if (element.connectedPlayers < element.playerLimit) { 
                    //&& int.Parse(element.comment) == Application.loadedLevel) {
                    if (Network.connections.Length > 0) {
                        yield break;
                    }
                    UnityNetworking.Instance.HostDataConnect(element, "", true, null);
                    yield return new WaitForSeconds(1.5f);
                    if (Network.connections.Length > 0) {
                        yield break;
                    }
                }
            }
        }

        private IEnumerator AutoJoinTryHosting() {
            UnityNetworking.Instance.StartServer("", UnityNetworking.Instance.defaultServerPort, MAX_CONNECTIONS, true);

            for (int i = 0; i < 3; i++) {
                debugMatchmakingStatus = "trying to host...";
                matchmakingStatus = "Finding match...";
                LogUtil.Log("NetworkMatchupGame: " + debugMatchmakingStatus);

                yield return new WaitForSeconds(UnityEngine.Random.Range(20, 40));

                if (Network.connections.Length > 0) {
                    yield break;
                }
            }

            Network.Disconnect();
        }
    }
#endif
}