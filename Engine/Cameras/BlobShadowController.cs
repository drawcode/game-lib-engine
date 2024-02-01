using System.Collections;
using UnityEngine;

namespace Engine.Cameras
{
    public class BlobShadowController : GameObjectBehavior
    {
        private void Update()
        {

            transform.position = transform.parent.position + Vector3.up * 8.246965f;
            transform.rotation = Quaternion.LookRotation(-Vector3.up, transform.parent.forward);
        }
    }
}