using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Engine.Networking {

    public interface INetwork {
#if !UNITY_FLASH && NETWORK_UNITY

        UnityEngine.Network currentNetwork { get; set; }

        void SetNetwork(UnityEngine.Network network);

        UnityEngine.Network GetNetwork();

        string facilitatorEndpoint { get; set; }

        int facilitatorPort { get; set; }

        void SetFacilitatorEndpoint(string endpoint);

        string GetFacilitatorEndPoint();

        void SetFacilitatorPort(int endpoint);

        int GetFacilitatorPort();

        string serverEndpoint { get; set; }

        int serverPort { get; set; }

        void SetServerEndpoint(string endpoint);

        string GetServerEndPoint();

        void SetServerPort(int endpoint);

        int GetServerPort();

        int currentConnections { get; set; }

        int maxConnections { get; set; }

        int GetCurrentConnections();

        int GetMaxConnections();

        NetworkPlayer[] connectedPlayers { get; set; }

        NetworkPlayer[] GetConnectedPlayers();

        bool IsClient();

        bool IsServer();

        bool StartServer(int connections, int port);

        bool StartServer(int connections, int port, bool natEnabled);

        bool Connect(string endpoint);

        bool Connect(string endpoint, int port);

        bool Disconnect();

#endif
    }
}