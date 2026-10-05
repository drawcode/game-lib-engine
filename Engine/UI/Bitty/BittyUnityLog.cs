using UnityEngine;

namespace Engine.UI.Bitty {

    // Installs LogUtil as the sink for the engine-free bitty core (BittyParser.logError), so a
    // bad view still logs as it did before the core moved out of Assembly-CSharp. Runtime and
    // edit time both, since views are parsed by Editor tooling too.
    public static class BittyUnityLog {

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void Install() {
            BittyParser.logError = message => LogUtil.LogError(message);
        }

#if UNITY_EDITOR
        [UnityEditor.InitializeOnLoadMethod]
        private static void InstallEditor() {
            Install();
        }
#endif
    }
}
