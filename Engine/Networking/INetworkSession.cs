using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Engine.Networking {

    internal interface INetworkSession {
#if !UNITY_FLASH

        bool StartSession();

        bool EndSession();

        bool UpdateSession();

        bool SetPlayer(EngineNetworkPlayer player);

        bool RemovePlayer(EngineNetworkPlayer player);

#endif
    }
}