using System.Collections;
using Engine;
using Engine.Data;
using Engine.Game.Controllers;
using Engine.Networking;
using Engine.Utility;
using UnityEngine;

namespace Engine.Networking {

    public class CarNetworkInit : BaseEngineBehavior {
#if !UNITY_FLASH

        private void OnNetworkInstantiate(NetworkMessageInfo msg) {

            // This is our own player
            if (networkView.isMine) {
                Camera.main.SendMessage("SetTarget", transform);
                GetComponent<NetworkRigidbody>().enabled = false;
            }

            // This is just some remote controlled player, don't execute direct
            // user input on this
            else {
                name += "Remote";
                GetComponent<CarController>().SetEnableUserInput(false);
                GetComponent<NetworkRigidbody>().enabled = true;
            }
        }

#endif
    }
}