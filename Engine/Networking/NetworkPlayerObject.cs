using System.Collections;
using UnityEngine;

namespace Engine.Networking {

    public class NetworkPlayerObject : MonoBehaviour {
#if !UNITY_FLASH
        public bool isMe = false;

        public void SetOwner(bool owner) {
            isMe = owner;
        }

        /*
        void Update()
        {
            if (isMe)
            {
                Vector3 moveDirection = new Vector3(-1 * Input.GetAxis("Vertical"), 0, Input.GetAxis("Horizontal"));
                float speed = 5;
                transform.Translate(speed * moveDirection * Time.deltaTime);//now really move!
            }
        }
        */
#endif
    }
}