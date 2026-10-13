using System;
using System.Text;

using Agnostic.Core;
using Agnostic.Host;

using UnityEngine;

namespace Engine.AgnosticHost {

    // The storage seam the house facades delegate inward to (SystemPrefUtil kv, the BaseGameState
    // profile blobs). One UnityStorage for the process, created on first use: it needs no scene
    // object and no UnityHost. The kill switches live on the facades (SystemPrefUtil.inward,
    // BaseGameState.profileInward); this class only hands out the seam.
    //
    // The profile store runs FrameMode.none (phase A of contexts/gamedev/agnostic/
    // context-agnostic-storage.md S4): same file name, same bytes as FileSystemUtil.WriteString, so
    // turning a switch off reads everything the inward path wrote. The only change is the atomic write.
    public static class AgnosticStorage {

        public const string profileSchema = "https://schemas.drawk.com/games/profile.v1.json";

        // FileSystemUtil.WriteString uses StreamWriter's default: UTF-8, no BOM.
        public static readonly UTF8Encoding utf8NoBom = new UTF8Encoding(false);

        private static UnityStorage storageInstance;
        private static ProfileStore profileStore;
        private static string profileDirectory;

        public static IHostStorage storage {
            get {
                if (storageInstance == null) {
                    storageInstance = new UnityStorage(new UnityLog());
                }

                return storageInstance;
            }
        }

        // The profile store over an OS directory, or null when that directory is not under
        // persistentDataPath (the caller then stays on its legacy path).
        public static ProfileStore Profiles(string osDirectory) {

            if (string.IsNullOrEmpty(osDirectory)) {
                return null;
            }

            if (profileStore != null && profileDirectory == osDirectory) {
                return profileStore;
            }

            string root = Application.persistentDataPath.Replace('\\', '/').TrimEnd('/');
            string dir = osDirectory.Replace('\\', '/').TrimEnd('/');

            if (dir != root && !dir.StartsWith(root + "/", StringComparison.Ordinal)) {
                return null;
            }

            string logical = UnityStorage.saveRoot + dir.Substring(root.Length).TrimStart('/');
            profileStore = new ProfileStore(storage, logical, profileSchema, 1, FrameMode.none);
            profileDirectory = osDirectory;
            return profileStore;
        }

        // StreamReader(path, true) drops a UTF-8 BOM; so does this.
        public static string DecodeUtf8(byte[] bytes) {

            if (bytes == null) {
                return "";
            }

            int skip = bytes.Length >= 3 && bytes[0] == 0xEF && bytes[1] == 0xBB && bytes[2] == 0xBF ? 3 : 0;
            return utf8NoBom.GetString(bytes, skip, bytes.Length - skip);
        }

        // Domain reload is off, so statics outlive play mode. The store's dedupe bytes must not:
        // a profile restored on disk between plays would otherwise be skipped on the next save.
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics() {
            profileStore = null;
            profileDirectory = null;
        }
    }
}
