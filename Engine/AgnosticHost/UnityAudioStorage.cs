using System;
using System.Collections.Generic;
using System.IO;

using Agnostic.Core;
using Agnostic.Host;

using Engine.Audio;

using UnityEngine;

namespace Engine.AgnosticHost {

    // U-A, over AudioSystem. Clip keys are the house's audio names ("ui-click"), resolved under
    // AudioSystem.audioRootPath exactly as AudioSystem.PlayEffect does.
    //
    // AudioSystem plays through POOLED audio-item objects, so a voice's GameObject is recycled for
    // another sound once it finishes. A voice is therefore identified by its AudioSource AND the
    // clip it started with; when either no longer matches, or a one-shot stopped playing, the voice
    // is dead and answers deadHandle — same as the null host after Stop.
    public class UnityAudio : IHostAudio {

        private class Voice {
            public AudioSource source;
            public AudioClip clip;
            public string bus;
            public float volume;
            public bool loop;
        }

        private readonly Dictionary<int, Voice> voices = new Dictionary<int, Voice>();
        private readonly Dictionary<string, float> busVolume = new Dictionary<string, float>(StringComparer.Ordinal);
        private readonly List<int> deadBuffer = new List<int>();
        private readonly UnityHandleTable handles;
        private readonly IHostLog log;
        private int nextId = 1;

        // 3D voices use the house default; 2D voices are fully 2D.
        public float spatialBlend3D = 0.9f;

        public UnityAudio(UnityHandleTable handles, IHostLog log) {
            this.handles = handles;
            this.log = log;
        }

        public Handle Play(string clip, string bus, float volume, float pitch) {
            return Start(clip, bus, volume, pitch, false, null, 0f);
        }

        public Handle Play3D(string clip, string bus, Handle emitter, float volume, float pitch) {

            GameObject go;

            if (!handles.TryGetGameObject(emitter, out go)) {
                return Handle.none;
            }

            return Start(clip, bus, volume, pitch, false, go.transform, spatialBlend3D);
        }

        public Handle PlayLoop(string clip, string bus, float volume) {
            return Start(clip, bus, volume, 1f, true, null, 0f);
        }

        public HostStatus Stop(Handle voice) {

            Voice v;

            if (!TryGetLive(voice, out v)) {
                return HostStatus.deadHandle;
            }

            v.source.Stop();
            voices.Remove(voice.id);
            return HostStatus.ok;
        }

        public HostStatus SetVoiceVolume(Handle voice, float volume) {

            Voice v;

            if (!TryGetLive(voice, out v)) {
                return HostStatus.deadHandle;
            }

            v.volume = Clamp01(volume);
            v.source.volume = v.volume * BusVolume(v.bus);
            return HostStatus.ok;
        }

        public HostStatus SetBusVolume(string bus, float volume) {

            string key = bus ?? "";
            float bv = Clamp01(volume);
            busVolume[key] = bv;

            foreach (KeyValuePair<int, Voice> kv in voices) {
                if (kv.Value.bus == key && IsLive(kv.Value)) {
                    kv.Value.source.volume = kv.Value.volume * bv;
                }
            }

            return HostStatus.ok;
        }

        public float BusVolume(string bus) {
            float v;
            return busVolume.TryGetValue(bus ?? "", out v) ? v : 1f;
        }

        // Drops finished one-shots. The driver calls it once a frame so the table never grows
        // with every gunshot of a round.
        public int Prune() {

            deadBuffer.Clear();

            foreach (KeyValuePair<int, Voice> kv in voices) {
                if (!IsLive(kv.Value)) {
                    deadBuffer.Add(kv.Key);
                }
            }

            for (int i = 0; i < deadBuffer.Count; i++) {
                voices.Remove(deadBuffer[i]);
            }

            return deadBuffer.Count;
        }

        public int liveCount {
            get {
                return voices.Count;
            }
        }

        private Handle Start(string clip, string bus, float volume, float pitch, bool loop, Transform parent, float spatialBlend) {

            if (string.IsNullOrEmpty(clip)) {
                return Handle.none;
            }

            AudioSystem system = AudioSystem.Instance;

            if (system == null) {
                Log(LogLevel.warning, "no AudioSystem; '" + clip + "' not played");
                return Handle.none;
            }

            string b = bus ?? "";
            float v = Clamp01(volume);

            // A 2D voice belongs to no world object, so it lives under AudioSystem's own object
            // (DontDestroyOnLoad) and survives a scene load until the core stops it. Parented to
            // the scene-bound disposable container, a music loop died with the boot scene.
            if (parent == null) {
                parent = system.transform;
            }

            // increment 0 marks a persistent (DontDestroyOnLoad) item in AudioSystem; one-shots
            // pass 1 so they get an AudioDestroy and go back to the pool when done.
            GameObject go = system.PlayFileFromResourcesObject(
                parent, system.audioRootPath + clip, loop, loop ? 0 : 1, v * BusVolume(b), spatialBlend);

            AudioSource source = go != null ? go.GetComponent<AudioSource>() : null;

            if (source == null || source.clip == null) {
                Log(LogLevel.warning, "clip not found: '" + clip + "'");
                return Handle.none;
            }

            source.pitch = pitch;

            Voice voice = new Voice();
            voice.source = source;
            voice.clip = source.clip;
            voice.bus = b;
            voice.volume = v;
            voice.loop = loop;

            int id = nextId++;
            voices[id] = voice;
            return new Handle(id);
        }

        private bool TryGetLive(Handle h, out Voice v) {

            if (!voices.TryGetValue(h.id, out v)) {
                return false;
            }

            if (!IsLive(v)) {
                voices.Remove(h.id);
                v = null;
                return false;
            }

            return true;
        }

        private static bool IsLive(Voice v) {
            return v.source != null
                && v.source.clip == v.clip
                && (v.source.isPlaying || (v.loop && v.source.gameObject.activeInHierarchy));
        }

        private void Log(LogLevel level, string message) {

            if (log != null && log.IsEnabled(level)) {
                log.Log(level, "host.audio", message);
            }
        }

        private static float Clamp01(float v) {
            return v < 0f ? 0f : (v > 1f ? 1f : v);
        }
    }

    // U-P1/P2/P3, over SystemPrefUtil (key/value) and FileSystemUtil (files).
    //
    // Logical roots: save:/ -> persistentDataPath, cache:/ -> temporaryCachePath,
    // content:/ -> streamingAssetsPath (read-only; on Android it lives inside the APK and a
    // synchronous read reports ioError). A path with a ".." segment is refused, so the core
    // cannot reach outside its roots.
    public class UnityStorage : IHostStorage {

        public const string saveRoot = "save:/";
        public const string cacheRoot = "cache:/";
        public const string contentRoot = "content:/";

        private readonly IHostLog log;

        public UnityStorage(IHostLog log) {
            this.log = log;
        }

        // ---- key / value --------------------------------------------------------------------

        public bool KvGet(string key, out string value) {

            value = null;

            if (string.IsNullOrEmpty(key) || !SystemPrefUtil.HasLocalSetting(key)) {
                return false;
            }

            value = SystemPrefUtil.GetLocalSettingString(key);
            return true;
        }

        public HostStatus KvSet(string key, string value) {

            if (string.IsNullOrEmpty(key)) {
                return HostStatus.invalidArgument;
            }

            SystemPrefUtil.SetLocalSettingString(key, value ?? "");
            return HostStatus.ok;
        }

        public HostStatus KvDelete(string key) {

            if (string.IsNullOrEmpty(key) || !SystemPrefUtil.HasLocalSetting(key)) {
                return HostStatus.notFound;
            }

            SystemPrefUtil.DeleteKey(key);
            return HostStatus.ok;
        }

        public HostStatus KvFlush() {
            SystemPrefUtil.Save();
            return HostStatus.ok;
        }

        // ---- files --------------------------------------------------------------------------

        public HostStatus Read(string path, out byte[] bytes) {

            bytes = null;
            bool writable;
            string os = ToOsPath(path, out writable);

            if (os == null) {
                return HostStatus.invalidArgument;
            }

            if (!File.Exists(os)) {
                return HostStatus.notFound;
            }

            try {
                bytes = FileSystemUtil.ReadAllBytes(os);
                return HostStatus.ok;
            }
            catch (Exception e) {
                Log(LogLevel.error, "Read " + path + ": " + e.Message);
                return HostStatus.ioError;
            }
        }

        public HostStatus WriteAtomic(string path, byte[] bytes) {

            bool writable;
            string os = ToOsPath(path, out writable);

            if (os == null || bytes == null || !writable) {
                return HostStatus.invalidArgument;
            }

            string temp = os + ".tmp";

            try {
                FileSystemUtil.WriteAllBytes(temp, bytes);

                if (File.Exists(os)) {
                    File.Replace(temp, os, null);
                }
                else {
                    File.Move(temp, os);
                }

                return HostStatus.ok;
            }
            catch (Exception e) {
                Log(LogLevel.error, "WriteAtomic " + path + ": " + e.Message);

                // The old file is intact; only the temp sibling may be left behind.
                try {
                    if (File.Exists(temp)) {
                        File.Delete(temp);
                    }
                }
                catch (Exception) {
                }

                return HostStatus.ioError;
            }
        }

        public bool Exists(string path) {
            bool writable;
            string os = ToOsPath(path, out writable);
            return os != null && File.Exists(os);
        }

        public HostStatus Delete(string path) {

            bool writable;
            string os = ToOsPath(path, out writable);

            if (os == null || !writable) {
                return HostStatus.invalidArgument;
            }

            if (!File.Exists(os)) {
                return HostStatus.notFound;
            }

            try {
                File.Delete(os);
                return HostStatus.ok;
            }
            catch (Exception e) {
                Log(LogLevel.error, "Delete " + path + ": " + e.Message);
                return HostStatus.ioError;
            }
        }

        // Full logical paths below `directory`, recursive, ordinal-sorted — the null host's order.
        public int List(string directory, List<string> results) {

            if (results == null) {
                return 0;
            }

            results.Clear();
            bool writable;
            string os = ToOsPath(directory, out writable);

            if (os == null || !Directory.Exists(os)) {
                return 0;
            }

            string root = RootOf(directory);
            string rootOs = ToOsPath(root, out writable);

            string[] files = Directory.GetFiles(os, "*", SearchOption.AllDirectories);

            for (int i = 0; i < files.Length; i++) {

                if (files[i].EndsWith(".tmp", StringComparison.Ordinal)) {
                    continue;
                }

                string rel = files[i].Substring(rootOs.Length).Replace('\\', '/').TrimStart('/');
                results.Add(root + rel);
            }

            results.Sort(StringComparer.Ordinal);
            return results.Count;
        }

        // ---- mapping ------------------------------------------------------------------------

        private static string RootOf(string path) {

            if (path.StartsWith(saveRoot, StringComparison.Ordinal)) {
                return saveRoot;
            }

            if (path.StartsWith(cacheRoot, StringComparison.Ordinal)) {
                return cacheRoot;
            }

            return contentRoot;
        }

        // null when the path is not under a known root or tries to climb out of it.
        public static string ToOsPath(string path, out bool writable) {

            writable = false;

            if (string.IsNullOrEmpty(path)) {
                return null;
            }

            string baseDir;
            string rest;

            if (path.StartsWith(saveRoot, StringComparison.Ordinal)) {
                baseDir = Application.persistentDataPath;
                rest = path.Substring(saveRoot.Length);
                writable = true;
            }
            else if (path.StartsWith(cacheRoot, StringComparison.Ordinal)) {
                baseDir = Application.temporaryCachePath;
                rest = path.Substring(cacheRoot.Length);
                writable = true;
            }
            else if (path.StartsWith(contentRoot, StringComparison.Ordinal)) {
                baseDir = Application.streamingAssetsPath;
                rest = path.Substring(contentRoot.Length);
            }
            else {
                return null;
            }

            string[] parts = rest.Split('/');

            for (int i = 0; i < parts.Length; i++) {
                if (parts[i] == "..") {
                    writable = false;
                    return null;
                }
            }

            return rest.Length == 0 ? baseDir : baseDir + "/" + rest;
        }

        private void Log(LogLevel level, string message) {

            if (log != null && log.IsEnabled(level)) {
                log.Log(level, "host.storage", message);
            }
        }
    }
}
