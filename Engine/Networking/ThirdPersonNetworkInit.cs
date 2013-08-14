using System;
using Engine;
using Engine.Game.Controllers;
using UnityEngine;

namespace Engine.Networking {

    public class ThirdPersonNetworkInit : BaseEngineBehavior {
#if !UNITY_FLASH

        private void OnNetworkInstantiate(NetworkMessageInfo msg) {

            // This is our own player
            if (networkView.isMine) {
                Camera.main.SendMessage("SetTarget", transform);
                GetComponent<NetworkInterpolatedTransform>().enabled = false;
            }

            // This is just some remote controlled player
            else {
                name += "Remote";
                GetComponent<ThirdPersonController>().enabled = false;
                GetComponent<ThirdPersonSimpleAnimation>().enabled = false;
                GetComponent<NetworkInterpolatedTransform>().enabled = true;
            }
        }

#endif
    }
}