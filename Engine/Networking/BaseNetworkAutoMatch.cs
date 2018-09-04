using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Engine.Networking {

    public class BaseNetworkAutoMatch : BaseNetwork {
#if !UNITY_FLASH && NETWORK_USE_UNITY

        public BaseNetworkAutoMatch() {
        }

        public bool FindMatch(string username, int playerGroup, string type) {
            if (hostData.Length > 0) {

                // Connect to an existing match or attempt to
                for (int i = 0; i < hostData.Length; i++) {
                    HostData hostDataItem = hostData[i];
                    LogUtil.Log("BaseNetworkAutoMatch::FindMatch::");
                    LogUtil.Log("Game name: " + hostDataItem.gameName);
                    LogUtil.Log("Game comment: " + hostDataItem.comment);
                    LogUtil.Log("Game connectedPlayers: " + hostDataItem.connectedPlayers);
                    LogUtil.Log("Game gameType: " + hostDataItem.gameType);
                    LogUtil.Log("Game guid: " + hostDataItem.guid);
                    LogUtil.Log("Game ip: " + hostDataItem.ip);
                    LogUtil.Log("Game passwordProtected: " + hostDataItem.passwordProtected);
                    LogUtil.Log("Game playerLimit: " + hostDataItem.playerLimit);
                    LogUtil.Log("Game port: " + hostDataItem.port);
                    LogUtil.Log("Game useNat: " + hostDataItem.useNat);

                    if (hostDataItem.connectedPlayers < hostDataItem.playerLimit) {

                        // Try to connect to this server
                        Connect(hostDataItem.ip[0] + "."
                            + hostDataItem.ip[1] + "."
                            + hostDataItem.ip[2] + "."
                            + hostDataItem.ip[3], hostDataItem.port);
                    }
                }
            }
            else {

                // Create a match
                StartMatchServer(username, type, "auto-matchup", 25001, 2);
            }

            return true;
        }

        public bool StartMatchServer(string gameName, string gameType, string gameDescription, int port, int playerCount) {
            this.StartServer(playerCount, port);
            MasterServer.ipAddress = facilitatorEndpoint;
            MasterServer.port = facilitatorPort;
            MasterServer.RegisterHost(gameType, gameName, gameDescription);
            return true;
        }

        public override void Tick(float deltaTime) {
        }

#endif
    }
}