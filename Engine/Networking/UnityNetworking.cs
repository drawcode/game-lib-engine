using System.Collections;
using Engine;
using Engine.Events;
using Engine.Utility;
using UnityEngine;

namespace Engine.Networking {
    public class NetworkMessages {
        public static string NetworkConnected = "networkConnected";
        public static string NetworkDisconnected = "networkDisconnected";
        public static string NetworkNatTestComplete = "networkNatTestComplete";
    }

    public class UnityNetworking : GameObjectBehavior {
#if !UNITY_FLASH

        // TODO move to downloadable config.
        public string masterserverGameName = "defaultgame";
        public int defaultServerPort = 50666;
        public int connectTimeoutValue = 30;
        public int connectTestTimeValue = 9;
        public int masterServerPort = 25010;
        public int connectionTesterPort = 25011;
        public int natFacilitatorPort = 25011;
        public string masterserveriPAddressOrDns = "matchup.drawlabs.com";
        private bool awaitingHostList = false;

        // CONNECTING

        private bool connecting = false;
        private HostData lastConnectHostData;
        private string[] lastConnectIP = null;
        private int lastConnectPort;
        private string lastConnectPW = "";
        private float lastConnectStarted;
        private bool lastConnectUsedNAT = true;
        private VoidDelegate lastConnectionFailDelegate;
        private bool lastConnectMayRetry = true;
        private NetworkConnectionError lastConnectionError;
        private static bool hasTestedNAT = false;
        private static bool testedUseNat = false;

        // MASTERSERVER

        private float lastMSRetry = 0;
        private int msRetries = 0;

        /*
         * METHODS AVAILABLE
         *
        public bool ReadyLoading()
             Is NAT tested & Masterserver loaded?
        public void HostDataConnect(HostData hData, string password, bool doRetryConnection, VoidDelegate failDelegate)
        public void DirectConnect(string IP, int port, string password, bool doRetryConnection,  VoidDelegate failDelegate)

        public void CancelConnection()
        public bool IsConnecting()

        public FetchHostList()
            Request new hostlist, you need to manually call this!
        public void SetHostListDelegate(VoidDelegate newD)
            Required! Set this to a void function of your own to be notified when the new hostlist has arrived.
        public HostData[] GetHostData()
            Get this list in your HostList delegate function!

        public bool HasReceivedHostList()
                Set TRUE after a succesfull Hostlist response, call FetchHostlist first

        public void StartServer(string password, int port, int maxConnections, bool enableSecurity)
        public void RegisterServer(string serverTitle, string comment)
        public void UnRegisterServer()

        public bool GetNATStatus()

        public string ConnectingToAddress()
            IP:port..i.e.:  127.0.0.1:25005
        public string[] LastIP()
        public int LastPort()
        public NetworkConnectionError LastConnectionError()
        public float TimeSinceLastConnect()
        public static byte[] StringToBytes(string str)
             Convert string to byte[] to workaround the 4096 character limit
        public static string BytesToString(byte[] by)
        */

        public static UnityNetworking Instance;

        public delegate void VoidDelegate();

        private static bool hasLoadedMasterserverSettings = false;
        private static HostData[] hostData = null; //Latest cached hostData result

        public void Awake() {
            if (Instance != null && this != Instance) {

                //There is already a copy of this script running
                Destroy(this);
                return;
            }

            Instance = this;
            StartCoroutine(TestConnection());

            //DontDestroyOnLoad(this); //enable this to make this an persistent GO
        }

        public void SetupMasterServer() {
            StartCoroutine(SetupMasterServerRoutine());
        }

        public bool ReadyLoading() {
            return (hasLoadedMasterserverSettings && hasTestedNAT);
        }

        private IEnumerator SetupMasterServerRoutine() {

            // Set ports & IP
            // Try to resolve the masterserver IP instead of hardcoding an IP.
            // Do note that a DNS resolve adds some plugins to the webplayer.
            // Call a webpage containing the right IP(s)ipt to reduce filesize

            MasterServer.port = masterServerPort;

            Network.connectionTesterPort = connectionTesterPort;
            Network.natFacilitatorPort = natFacilitatorPort;
            Network.natFacilitatorIP = Network.connectionTesterIP = MasterServer.ipAddress = masterserveriPAddressOrDns;

            //Network.natFacilitatorIP = MasterServer.ipAddress = masterserveriPAddressOrDns;
            //Network.natFacilitatorIP = MasterServer.ipAddress = masterserveriPAddressOrDns;

            // If we don't set any IP/ports we use the public&free Unity masterserver.

            hasLoadedMasterserverSettings = true;
            FetchHostList();
            yield break;
        }

        // CONNECTING

        public void HostDataConnect(HostData hData, string password, bool doRetryConnection, VoidDelegate failDelegate) {
            StartedConnecting();

            lastConnectHostData = hData;
            lastConnectUsedNAT = lastConnectHostData.useNat;
            lastConnectMayRetry = doRetryConnection;

            if (password == "")
                Network.Connect(hData);
            else
                Network.Connect(hData, password);

            Invoke("ConnectTimeout", connectTimeoutValue);

            lastConnectIP = hData.ip;
            lastConnectPort = hData.port;
            lastConnectPW = password;
            lastConnectStarted = Time.time;

            if (failDelegate != null)
                lastConnectionFailDelegate = failDelegate;

            connecting = true;
        }

        public void DirectConnect(string IP, int port, string password, bool doRetryConnection, VoidDelegate failDelegate) {
            string[] ips = new string[1];
            ips[0] = IP;
            DirectConnect(ips, port, password, doRetryConnection, failDelegate);
        }

        public void DirectConnect(string[] IP, int port, string password, bool doRetryConnection, VoidDelegate failDelegate) {
            StartedConnecting();
            lastConnectMayRetry = doRetryConnection;
            lastConnectUsedNAT = false;

            if (password == "")
                Network.Connect(IP, port);
            else
                Network.Connect(IP, port, password);

            Invoke("ConnectTimeout", connectTimeoutValue);

            connecting = true;
            lastConnectHostData = null;
            lastConnectIP = IP;
            lastConnectPort = port;
            lastConnectPW = password;
            lastConnectStarted = Time.time;

            if (failDelegate != null)
                lastConnectionFailDelegate = failDelegate;
        }

        private void StartedConnecting() {
            CancelInvoke("ConnectTimeout");
            connecting = true;
            lastConnectStarted = Time.realtimeSinceStartup;
        }

        public void CancelConnection() {
            CancelInvoke("ConnectTimeout");
            lastConnectMayRetry = false;
            connecting = false;
        }

        public void ConnectTimeout() {
            LogUtil.Log("Connect timeout");
            OnFailedToConnect(NetworkConnectionError.NoError);
        }

        private void OnFailedToConnect(NetworkConnectionError info) {
            CancelInvoke("ConnectTimeout");

            if (lastConnectIP != null) {
                LogUtil.Log("Failed to connect ["
                    + lastConnectIP[0]
                    + ":"
                    + lastConnectPort
                    + " ] info:"
                    + info);
                StartCoroutine(FailedConnectRetry(info));
            }
            else {
                LogUtil.Log("Failed to connect, no data: " + info);
                connecting = false;
            }
        }

        public void OnConnectedToServer() {
            CancelInvoke("ConnectTimeout");
        }

        //Try again with some different settings..mainly try connecting without NAT
        private IEnumerator FailedConnectRetry(NetworkConnectionError info) {
            lastConnectionError = info;

            if (!lastConnectMayRetry
                || info == NetworkConnectionError.TooManyConnectedPlayers
                || info == NetworkConnectionError.InvalidPassword) {

                //Stop retrying
            }
            else if (lastConnectUsedNAT
                || lastConnectPort != defaultServerPort) {

                //Retry (without NAT) on default port!
                yield return 0; //Workaround against "too many open connections"
                DirectConnect(lastConnectIP,
                              UnityNetworking.Instance.defaultServerPort,
                              lastConnectPW,
                              true,
                              lastConnectionFailDelegate);
                yield break;
            }

            //Finished: failed
            connecting = false;

            if (lastConnectionFailDelegate != null) {
                lastConnectionFailDelegate();
            }
        }

        public bool IsConnecting() {
            return connecting;
        }

        // MASTERSERVER

        public void OnFailedToConnectToMasterServer(NetworkConnectionError info) {

            //Two possible causes: FetchHostList OR RegisterHost failed to connect
            LogUtil.Log("OnFailedToConnectToMasterServer: " + info);

            // 5,10,20,50,.. to not overload the masterserver when its down
            int retryTime = 5 + 5 * msRetries * msRetries;

            LogUtil.Log("UnityNetworking: OnFailedToConnectToMasterServer - msRetries: " + msRetries);
            LogUtil.Log("UnityNetworking: OnFailedToConnectToMasterServer - retryTime: " + retryTime);

            if (lastMSRetry < Time.time - retryTime) {
                LogUtil.Log("UnityNetworking: OnFailedToConnectToMasterServer - lastMSRetry: " + lastMSRetry);
                lastMSRetry = Time.time;
                FetchHostList();
            }
        }

        private float lastHostListRequest = -999;

        public void FetchHostList() {
            LogUtil.Log("UnityNetworking: FetchHostList: " + masterserverGameName);

            if (!hasLoadedMasterserverSettings) {
                LogUtil.LogError("Calling FetchHostList but we havent loaded MS settings yet");
                return;
            }

            hostData = MasterServer.PollHostList();

            LogUtil.Log("UnityNetworking: FetchHostList - PollHostList: ");
            LogUtil.Log("UnityNetworking: FetchHostList - hostData.Length: " + hostData.Length);

            if (lastHostListRequest < Time.realtimeSinceStartup - 1) {
                LogUtil.Log("UnityNetworking: FetchHostList - lastHostListRequest: " + lastHostListRequest);
                LogUtil.Log("UnityNetworking: FetchHostList - Time.realtimeSinceStartup: " + Time.realtimeSinceStartup);
                lastHostListRequest = Time.realtimeSinceStartup;
                MasterServer.RequestHostList(masterserverGameName);
                LogUtil.Log("UnityNetworking: FetchHostList - RequestHostList: " + masterserverGameName);
            }
        }

        private VoidDelegate currentHostListDelegate;

        public void SetHostListDelegate(VoidDelegate newD) {
            currentHostListDelegate = newD;
        }

        public HostData[] GetHostData() {
            return hostData;
        }

        private bool hasReceivedHostListResponse = false;

        public bool HasReceivedHostList() {
            return hasReceivedHostListResponse;
        }

        public void OnMasterServerEvent(MasterServerEvent msEvent) {
            LogUtil.Log(Time.realtimeSinceStartup + " OnMasterEvent: " + msEvent);

            switch (msEvent) {
            case MasterServerEvent.HostListReceived:

                    //WARNING: It does 1 call per item in the full host list!
                    //A list of 100 items generates 100 calls. The 34th call contains 34 items total etc.
                LogUtil.Log("UnityNetworking: OnMasterServerEvent - HostListReceived: ");
                StartCoroutine(WaitForAllHostData());
                break;

            case MasterServerEvent.RegistrationFailedGameName:
                LogUtil.Log("UnityNetworking: OnMasterServerEvent - RegistrationFailedGameName: ");
                break;

            case MasterServerEvent.RegistrationFailedGameType:
                LogUtil.Log("UnityNetworking: OnMasterServerEvent - RegistrationFailedGameType: ");
                break;

            case MasterServerEvent.RegistrationFailedNoServer:
                LogUtil.Log("Masterserver error: " + msEvent);
                LogUtil.Log("UnityNetworking: OnMasterServerEvent - RegistrationFailedNoServer: ");
                break;

            case MasterServerEvent.RegistrationSucceeded:
                LogUtil.Log("UnityNetworking: OnMasterServerEvent - RegistrationSucceeded: ");
                break;

            default:
                break;
            }
        }

        private IEnumerator WaitForAllHostData() {
            LogUtil.Log("MasterServer polling...");

            if (awaitingHostList)
                yield break;

            LogUtil.Log("MasterServer awaitingHostList...");

            awaitingHostList = true;
            hostData = MasterServer.PollHostList();

            LogUtil.Log("MasterServer hostData.Length: " + hostData.Length);

            while (true) {
                yield return new WaitForSeconds(0.2f);

                LogUtil.Log("MasterServer .PollHostList().Length: " + MasterServer.PollHostList().Length);
                LogUtil.Log("MasterServer hostData.Length: " + hostData.Length);

                if (MasterServer.PollHostList().Length == hostData.Length) {
                    break;
                }

                hostData = MasterServer.PollHostList();

                for (int i = 0; i < hostData.Length; i++) {
                    LogUtil.Log("Masterserver hostdata #" + i.ToString() + " gameName: " + hostData[i].gameName);
                    LogUtil.Log("Masterserver hostdata #" + i.ToString() + " gameType: " + hostData[i].gameType);
                    LogUtil.Log("Masterserver hostdata #" + i.ToString() + " playerLimit: " + hostData[i].playerLimit);
                    LogUtil.Log("Masterserver hostdata #" + i.ToString() + " ip: " + hostData[i].ip);
                    LogUtil.Log("Masterserver hostdata #" + i.ToString() + " connectedPlayers: " + hostData[i].connectedPlayers);
                    LogUtil.Log("Masterserver hostdata #" + i.ToString() + " useNat: " + hostData[i].useNat);
                }

                LogUtil.Log("MasterServer poll loop...");
            }

            if (currentHostListDelegate != null) {
                currentHostListDelegate();
            }

            hasReceivedHostListResponse = true;
            awaitingHostList = false;
        }

        // SERVER

        public void StartServer(string password, int port, int maxConnections, bool enableSecurity) {
            if (port <= 1024) {
                LogUtil.LogError("StartServer tries to use port <=1024. This will probably not work and is illegal! Use a different port!");
            }

            if (password != "") {
                Network.incomingPassword = password;
            }

            if (enableSecurity) {
                try {
                    if (Network.connections.Length == 0)
                        Network.InitializeSecurity();
                }
                catch (System.Exception e) {
                    LogUtil.Log("Error InitializeSecurity:" + e.Message + e.StackTrace);
                }
            }

            Network.InitializeServer(maxConnections, port, GetNATStatus());
        }

        public void RegisterHost(string serverTitle, string comment) {
            if (!Network.isServer) {
                LogUtil.LogError("RegisterServer: is not a server!");
                return;
            }

            if (!ReadyLoading())
                LogUtil.Log("RegisterHost; wasn't ready loading network settings yet though!");

            MasterServer.RegisterHost(masterserverGameName, serverTitle, comment);
        }

        public void UnregisterHost() {
            if (!Network.isServer) {
                LogUtil.LogError("RegisterServer: is not a server!");
                return;
            }

            MasterServer.UnregisterHost();
        }

        public void OnDisconnectedFromServer(NetworkDisconnection info) {
            if (Network.isServer)
                UnregisterHost();

            SendMessageUpwards("OnDisconnectedSession", SendMessageOptions.DontRequireReceiver);

            Messenger<NetworkDisconnection>.Broadcast(NetworkMessages.NetworkDisconnected, info);
        }

        // CONNECTIONTESTER

        public bool GetNATStatus() {
            if (!hasTestedNAT)
                LogUtil.Log("Calling GetNATStatus, but we havent finished testing yet!");

            //return testedUseNat;
            return true;
        }

        private IEnumerator TestConnection() {
            if (hasTestedNAT)
                yield break;

            while (!hasLoadedMasterserverSettings)
                yield return 0;

            testedUseNat = !Network.HavePublicAddress();

            ConnectionTesterStatus connectionTestResult = ConnectionTesterStatus.Undetermined;
            float timeoutAt = Time.realtimeSinceStartup + connectTestTimeValue;
            float timer = 0;
            bool probingPublicIP = false;
            string testMessage = "";

            while (!hasTestedNAT) {
                yield return 0;
                if (Time.realtimeSinceStartup >= timeoutAt) {
                    LogUtil.Log("TestConnect NAT test aborted; timeout");
                    break;
                }
                connectionTestResult = Network.TestConnection();
                switch (connectionTestResult) {
                case ConnectionTesterStatus.Error:
                    testMessage = "Problem determining NAT capabilities";
                    hasTestedNAT = false;
                    break;

                case ConnectionTesterStatus.Undetermined:
                    testMessage = "Undetermined NAT capabilities";
                    hasTestedNAT = false;
                    break;

                case ConnectionTesterStatus.PublicIPIsConnectable:
                    testMessage = "Directly connectable public IP address.";
                    testedUseNat = false;
                    hasTestedNAT = true;
                    break;

                // This case is a bit special as we now need to check if we can
                // circumvent the blocking by using NAT punchthrough
                case ConnectionTesterStatus.PublicIPPortBlocked:
                    testMessage = "Non-connectble public IP address (port " + defaultServerPort + " blocked), running a server is impossible.";
                    hasTestedNAT = false;

                        // If no NAT punchthrough test has been performed on this public IP, force a test
                    if (!probingPublicIP) {
                        LogUtil.Log("Testing if firewall can be circumvented");
                        connectionTestResult = Network.TestConnectionNAT();
                        probingPublicIP = true;
                        timer = Time.time + 10;
                    }

                        // NAT punchthrough test was performed but we still get blocked
                        else if (Time.time > timer) {
                        probingPublicIP = false;        // reset
                        testedUseNat = true;
                        hasTestedNAT = true;
                    }
                    break;

                case ConnectionTesterStatus.PublicIPNoServerStarted:
                    testMessage = "Public IP address but server not initialized, it must be started to check server accessibility. Restart connection test when ready.";
                    break;

                case ConnectionTesterStatus.LimitedNATPunchthroughPortRestricted:
                    testMessage = "Limited NAT punchthrough capabilities. Cannot connect to all types of NAT servers. Running a server is ill adviced as not everyone can connect.";
                    testedUseNat = true;
                    hasTestedNAT = true;
                    break;

                case ConnectionTesterStatus.LimitedNATPunchthroughSymmetric:
                    testMessage = "Limited NAT punchthrough capabilities. Cannot connect to all types of NAT servers. Running a server is ill adviced as not everyone can connect.";
                    testedUseNat = true;
                    hasTestedNAT = true;
                    break;

                case ConnectionTesterStatus.NATpunchthroughAddressRestrictedCone:
                case ConnectionTesterStatus.NATpunchthroughFullCone:
                    testMessage = "NAT punchthrough capable. Can connect to all servers and receive connections from all clients. Enabling NAT punchthrough functionality.";
                    testedUseNat = true;
                    hasTestedNAT = true;
                    break;

                default:
                    testMessage = "Error in test routine, got " + connectionTestResult;
                    break;
                }
            }
            hasTestedNAT = true;

            //Messenger<bool, UnityEngine.ConnectionTesterStatus, bool, bool, string>.Broadcast(NetworkMessages.NetworkNatTestComplete, testedUseNat, connectionTestResult, probingPublicIP, hasTestedNAT, testMessage);
            LogUtil.Log("TestConnection result: testedUseNat=" + testedUseNat + " connectionTestResult=" + connectionTestResult + " probingPublicIP=" + probingPublicIP + " hasTestedNAT=" + hasTestedNAT + " testMessage=" + testMessage);
        }

        public void SetNetworkEnabled(int groupIndex, bool enabled) {
            Network.isMessageQueueRunning = enabled;
            Network.SetSendingEnabled(groupIndex, enabled);

            //Network.SetReceivingEnabled(Network.player, groupIndex, enabled);
        }

        // PROPERTIES / HELPERS

        public string ConnectingToAddress() {
            return lastConnectIP[0] + ":" + lastConnectPort;
        }

        public string[] LastIP() {
            return lastConnectIP;
        }

        public int LastPort() {
            return lastConnectPort;
        }

        public NetworkConnectionError LastConnectionError() {
            return lastConnectionError;
        }

        public float TimeSinceLastConnect() {
            return Time.realtimeSinceStartup - lastConnectStarted;
        }

        //Send/receive large amounds of data via byte[]
        public static byte[] StringToBytes(string str) {
            return System.Text.Encoding.UTF8.GetBytes(str);
        }

        public static string BytesToString(byte[] by) {
            return System.Text.Encoding.UTF8.GetString(by);
        }

#endif
    }
}