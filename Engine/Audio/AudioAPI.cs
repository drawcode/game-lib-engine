using System.Collections;
using Engine;
using Engine.Networking;
using Engine.Utility;
using UnityEngine;

namespace Engine.Audio
{
    public class AudioAPI : BaseEngineObject
    {

        // -------------------------------------------------------------------
        // Singleton access

        private static volatile AudioAPI instance;
        private static object syncRoot = new Object();

        private AudioAPI()
        {
        }

        public static AudioAPI Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new AudioAPI();
                    }
                }

                return instance;
            }
        }

        // -------------------------------------------------------------------
        // Unity Methods

        private void Start()
        {
        }

        private void Update()
        {
        }

        // -------------------------------------------------------------------
        // Custom
    }
}