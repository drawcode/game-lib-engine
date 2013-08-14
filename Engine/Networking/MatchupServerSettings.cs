using UnityEngine;

namespace Engine.Networking {

    public class MatchupServerSettings : MonoBehaviour {
#if !UNITY_FLASH

        public static void Clear() {
            serverTitle = description = motd = IP = password = "";
            isDedicatedServer = false;
        }

        public static string serverTitle;
        public static string description;
        public static string motd;
        public static string password;
        public static string IP;
        public static int port;
        public static int players;
        public static bool isDedicatedServer;
#endif
    }
}