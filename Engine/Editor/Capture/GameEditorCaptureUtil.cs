using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Engine.Editor {

    /// <summary>
    /// Game-agnostic screenshot primitives for agent-driven Editor sessions.
    ///
    /// Deliberately has NO dependency on com.unity.pipeline — the CLI-facing
    /// [CliCommand] wrappers live in the consuming project and call into this.
    /// See contexts/games/shared/skills/unity-cli-capture.md.
    /// </summary>
    public static class GameEditorCaptureUtil {

        /// <summary>
        /// Composited-frame capture: everything the Game view shows, including
        /// Screen Space Overlay canvases, NGUI and UI Toolkit panels.
        ///
        /// ScreenCapture writes at the END of a frame, so the file does NOT exist
        /// when this returns. Poll for it from the caller (`until [ -f ... ]` in bash, or the
        /// capture-status command). Never block the main thread waiting — the frame
        /// cannot finish while you hold it, so the file would never appear.
        /// Reliable in Play mode; in Edit mode the Game view may not repaint, so
        /// prefer CaptureCamera there.
        /// </summary>
        public static string CaptureScreen(string path, int superSize = 1) {
            string full = PrepareOutputPath(path);
            if (superSize < 1) superSize = 1;

            // Delete first so a stale file is never mistaken for the new capture.
            if (File.Exists(full)) File.Delete(full);

            ScreenCapture.CaptureScreenshot(full, superSize);
            return full;
        }

        /// <summary>
        /// Render one camera into an offscreen target at an exact pixel size.
        /// Works in Edit mode and at any resolution, but does NOT include Screen
        /// Space Overlay UI — that is only composited into the real backbuffer.
        /// Unlike CaptureScreen, the file exists when this returns.
        /// </summary>
        public static string CaptureCamera(Camera camera, string path, int width, int height) {
            if (camera == null) camera = ResolveCamera(null);
            if (camera == null) throw new InvalidOperationException("No camera available to capture.");
            if (width <= 0 || height <= 0) throw new ArgumentException("width and height must be positive.");

            string full = PrepareOutputPath(path);

            RenderTexture target = RenderTexture.GetTemporary(width, height, 24, RenderTextureFormat.ARGB32);
            RenderTexture previousActive = RenderTexture.active;
            RenderTexture previousTarget = camera.targetTexture;
            Texture2D readback = null;

            try {
                camera.targetTexture = target;
                camera.Render();

                RenderTexture.active = target;
                readback = new Texture2D(width, height, TextureFormat.RGBA32, false);
                readback.ReadPixels(new Rect(0, 0, width, height), 0, 0, false);
                readback.Apply(false, false);

                File.WriteAllBytes(full, readback.EncodeToPNG());
            }
            finally {
                camera.targetTexture = previousTarget;
                RenderTexture.active = previousActive;
                RenderTexture.ReleaseTemporary(target);
                if (readback != null) UnityEngine.Object.DestroyImmediate(readback);
            }

            return full;
        }

        /// <summary>
        /// Find a camera by name, else the highest-depth enabled camera, else Camera.main.
        /// </summary>
        public static Camera ResolveCamera(string cameraName) {
            if (!string.IsNullOrEmpty(cameraName)) {
                Camera[] all = Camera.allCameras;
                for (int i = 0; i < all.Length; i++) {
                    if (all[i] != null && all[i].name == cameraName) return all[i];
                }
                return null;
            }

            Camera best = null;
            Camera[] cameras = Camera.allCameras;
            for (int i = 0; i < cameras.Length; i++) {
                Camera candidate = cameras[i];
                if (candidate == null || !candidate.isActiveAndEnabled) continue;
                if (best == null || candidate.depth > best.depth) best = candidate;
            }
            return best != null ? best : Camera.main;
        }

        /// <summary>
        /// Capture a PNG frame sequence off EditorApplication.update — the raw material
        /// for reviewing a transition or animation, and the input to an optional
        /// `ffmpeg -framerate ... -i frame_%04d.png out.mp4` pass.
        ///
        /// Returns the paths it WILL write; poll for the last one to know it finished.
        /// </summary>
        public static string[] CaptureSequence(string directory, string prefix, int frameCount,
                                               float intervalSeconds, int superSize = 1) {
            if (frameCount <= 0) throw new ArgumentException("frameCount must be positive.");
            if (frameCount > 600) throw new ArgumentException("frameCount capped at 600 — capture a shorter window.");
            if (intervalSeconds < 0f) intervalSeconds = 0f;
            if (string.IsNullOrEmpty(prefix)) prefix = "frame";

            string dir = Path.GetFullPath(directory);
            Directory.CreateDirectory(dir);

            var planned = new string[frameCount];
            for (int i = 0; i < frameCount; i++) {
                planned[i] = Path.Combine(dir, string.Format("{0}_{1:D4}.png", prefix, i));
                if (File.Exists(planned[i])) File.Delete(planned[i]);
            }

            RunSequence(planned, intervalSeconds, superSize);
            return planned;
        }

        static void RunSequence(string[] paths, float intervalSeconds, int superSize) {
            int next = 0;
            double due = EditorApplication.timeSinceStartup;
            EditorApplication.CallbackFunction tick = null;

            tick = () => {
                if (EditorApplication.timeSinceStartup < due) return;

                ScreenCapture.CaptureScreenshot(paths[next], superSize < 1 ? 1 : superSize);
                next++;
                due = EditorApplication.timeSinceStartup + intervalSeconds;

                if (next >= paths.Length) EditorApplication.update -= tick;
            };

            EditorApplication.update += tick;
        }

        /// <summary>Describe how many of a planned sequence have landed.</summary>
        public static int CountExisting(IList<string> paths) {
            int count = 0;
            for (int i = 0; i < paths.Count; i++) {
                if (File.Exists(paths[i]) && new FileInfo(paths[i]).Length > 0L) count++;
            }
            return count;
        }

        static string PrepareOutputPath(string path) {
            if (string.IsNullOrEmpty(path)) throw new ArgumentException("path is required.");

            string full = Path.GetFullPath(path);
            string dir = Path.GetDirectoryName(full);
            if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);
            return full;
        }
    }
}
