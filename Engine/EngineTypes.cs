using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.Networking {

    // Enums
    public enum EngineNetworkType {
        NETWORK_ONLINE,
        NETWORK_LOCAL
    }

    public enum EngineNetworkConnectionType {
        NETWORK_CONNECTION_BROADBAND = 0,
        NETWORK_CONNECTION_MOBILE = 1
    }

    // Data Classes

    public class BaseData {

        public string uuid { get; set; }

        public BaseData() {
            Reset();
        }

        public virtual void Reset() {
            uuid = "";
        }
    }

    public class BaseMeta : BaseData {

        public string name { get; set; }

        public string code { get; set; }

        public string value { get; set; }

        public BaseMeta() {
            Reset();
        }

        public override void Reset() {
            name = "";
            code = "";
            value = "";
            base.Reset();
        }
    }

    public class BaseMetaDetail : BaseMeta {

        public string status { get; set; }

        public DateTime dateModified { get; set; }

        public DateTime dateCreated { get; set; }

        public BaseMetaDetail() {
            Reset();
        }

        public override void Reset() {
            status = "";
            dateModified = DateTime.Now;
            dateCreated = DateTime.Now;
            base.Reset();
        }
    }

    /*
     * useNat    Does this server require NAT punchthrough?
gameType     The type of the game (like "MyUniqueGameType")
gameName     The name of the game (like John Doe's Game)
connectedPlayers     Currently connected players
playerLimit  Maximum players limit
ip   Server IP address
port     Server port
passwordProtected    Does the server require a password?
comment  A miscellaneous comment (can hold data)
guid     The GUID of the host, needed when connecting with NAT punchthrough
     * */

    public class EngineNetworkSession {

        public string useNat { get; set; }

        public string gameType { get; set; }

        public string gameName { get; set; }

        public int connectedPlayers { get; set; }

        public int playerLimit { get; set; }

        public string serverEndpoint { get; set; }

        public int serverPort { get; set; }

        public string passwordProtected { get; set; }

        public string comment { get; set; }

        public string guid { get; set; }

        public EngineNetworkSession() {
            Reset();
        }

        public void Reset() {
            useNat = "";
            gameType = "";
            gameName = "";
            connectedPlayers = 0;
            playerLimit = 0;
            serverEndpoint = "";
            serverPort = 0;
            passwordProtected = "";
            comment = "";
            guid = "";
        }
    }

    public class EngineNetworkPlayerAttribute : BaseMetaDetail {

        public EngineNetworkPlayerAttribute() {
            Reset();
        }

        public override void Reset() {
            base.Reset();
        }
    }

    public class EngineNetworkInfo {
        public string endpointCode;
        public string endpointPort;

        public EngineNetworkInfo() {
            Reset();
        }

        public void Reset() {
            endpointCode = "";
            endpointPort = "";
        }
    }

    public class EngineNetworkId {
        public string id;
        public string username;
        public string networkId;

        public EngineNetworkId() {
            Reset();
        }

        public void Reset() {
            id = "";
            username = "";
            networkId = ""; // gamecenter, openfeint, facebook, blackbox, steam, etc...
        }
    }

    public class EngineNetworkPlayer {
        public EngineNetworkId networkId = new EngineNetworkId();
        public EngineNetworkInfo networkInfo = new EngineNetworkInfo();
        public Dictionary<string, EngineNetworkPlayerAttribute> playerAttributes = new Dictionary<string, EngineNetworkPlayerAttribute>();

        public EngineNetworkPlayer() {
            Reset();
        }

        public void Reset() {
            networkId = new EngineNetworkId();
            networkInfo = new EngineNetworkInfo();
            playerAttributes.Clear();
        }
    }
}