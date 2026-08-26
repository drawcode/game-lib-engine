using System;
using System.IO;
using UnityEngine;

namespace Engine.Editor {

    /// <summary>
    /// Result of comparing two PNG captures. Public fields only — JsonUtility
    /// serializes this straight back to the CLI caller.
    /// </summary>
    [Serializable]
    public class GameEditorImageCompareResult {
        public bool ok;
        public string error;

        public string pathA;
        public string pathB;
        public string diffPath;

        public int widthA;
        public int heightA;
        public int widthB;
        public int heightB;
        public bool sizeMismatch;

        public int comparedPixels;
        public int differingPixels;
        public float differingRatio;
        public float meanAbsError;
        public int maxAbsError;

        /// <summary>Bounding box of the differing pixels, in top-left-origin pixels.</summary>
        public int diffMinX;
        public int diffMinY;
        public int diffMaxX;
        public int diffMaxY;
    }

    /// <summary>
    /// Game-agnostic PNG comparison for baseline-vs-current screenshot gates.
    /// No com.unity.pipeline dependency — see GameEditorCaptureUtil.
    /// </summary>
    public static class GameEditorImageCompareUtil {

        /// <summary>
        /// Compare two PNGs channel-by-channel.
        ///
        /// <paramref name="tolerance"/> is the per-channel 0-255 delta below which a
        /// pixel counts as unchanged; use ~8 to absorb font antialiasing and
        /// compression noise, 0 for an exact match.
        ///
        /// When the two images differ in size, only the overlapping bottom-left
        /// region is compared and sizeMismatch is set — a resolution change is a
        /// finding in itself, never something to silently rescale away.
        /// </summary>
        public static GameEditorImageCompareResult Compare(string pathA, string pathB, string diffPath,
                                                           int tolerance = 8) {
            var result = new GameEditorImageCompareResult { pathA = pathA, pathB = pathB };

            Texture2D a = null;
            Texture2D b = null;
            Texture2D diff = null;

            try {
                a = Load(pathA, ref result.error);
                b = Load(pathB, ref result.error);
                if (a == null || b == null) return result;

                result.widthA = a.width;
                result.heightA = a.height;
                result.widthB = b.width;
                result.heightB = b.height;
                result.sizeMismatch = a.width != b.width || a.height != b.height;

                int width = Mathf.Min(a.width, b.width);
                int height = Mathf.Min(a.height, b.height);
                if (width <= 0 || height <= 0) {
                    result.error = "Images share no overlapping region.";
                    return result;
                }

                if (tolerance < 0) tolerance = 0;

                Color32[] pixelsA = a.GetPixels32();
                Color32[] pixelsB = b.GetPixels32();
                bool wantDiff = !string.IsNullOrEmpty(diffPath);
                Color32[] pixelsDiff = wantDiff ? new Color32[width * height] : null;

                int differing = 0;
                long errorSum = 0L;
                int maxError = 0;
                int minX = int.MaxValue, minY = int.MaxValue, maxX = -1, maxY = -1;

                for (int y = 0; y < height; y++) {
                    int rowA = y * a.width;
                    int rowB = y * b.width;
                    int rowDiff = y * width;

                    for (int x = 0; x < width; x++) {
                        Color32 ca = pixelsA[rowA + x];
                        Color32 cb = pixelsB[rowB + x];

                        int dr = Abs(ca.r - cb.r);
                        int dg = Abs(ca.g - cb.g);
                        int db = Abs(ca.b - cb.b);
                        int da = Abs(ca.a - cb.a);

                        int worst = dr;
                        if (dg > worst) worst = dg;
                        if (db > worst) worst = db;
                        if (da > worst) worst = da;

                        errorSum += worst;
                        if (worst > maxError) maxError = worst;

                        bool changed = worst > tolerance;
                        if (changed) {
                            differing++;

                            // Report in top-left-origin coordinates; Color32[] is bottom-up.
                            int topDownY = height - 1 - y;
                            if (x < minX) minX = x;
                            if (x > maxX) maxX = x;
                            if (topDownY < minY) minY = topDownY;
                            if (topDownY > maxY) maxY = topDownY;
                        }

                        if (wantDiff) {
                            pixelsDiff[rowDiff + x] = changed
                                ? new Color32(255, 0, 0, 255)
                                : Dim(ca);
                        }
                    }
                }

                int compared = width * height;
                result.comparedPixels = compared;
                result.differingPixels = differing;
                result.differingRatio = (float)differing / compared;
                result.meanAbsError = (float)((double)errorSum / compared);
                result.maxAbsError = maxError;

                if (differing > 0) {
                    result.diffMinX = minX;
                    result.diffMinY = minY;
                    result.diffMaxX = maxX;
                    result.diffMaxY = maxY;
                } else {
                    result.diffMinX = result.diffMinY = result.diffMaxX = result.diffMaxY = -1;
                }

                if (wantDiff) {
                    diff = new Texture2D(width, height, TextureFormat.RGBA32, false);
                    diff.SetPixels32(pixelsDiff);
                    diff.Apply(false, false);

                    string fullDiff = Path.GetFullPath(diffPath);
                    string dir = Path.GetDirectoryName(fullDiff);
                    if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);
                    File.WriteAllBytes(fullDiff, diff.EncodeToPNG());
                    result.diffPath = fullDiff;
                }

                result.ok = true;
                return result;
            }
            catch (Exception exception) {
                result.ok = false;
                result.error = exception.Message;
                return result;
            }
            finally {
                if (a != null) UnityEngine.Object.DestroyImmediate(a);
                if (b != null) UnityEngine.Object.DestroyImmediate(b);
                if (diff != null) UnityEngine.Object.DestroyImmediate(diff);
            }
        }

        static Texture2D Load(string path, ref string error) {
            if (string.IsNullOrEmpty(path)) {
                error = "Both image paths are required.";
                return null;
            }

            string full = Path.GetFullPath(path);
            if (!File.Exists(full)) {
                error = "Image not found: " + full;
                return null;
            }

            var texture = new Texture2D(2, 2, TextureFormat.RGBA32, false);
            if (!texture.LoadImage(File.ReadAllBytes(full))) {
                UnityEngine.Object.DestroyImmediate(texture);
                error = "Not a readable PNG/JPG: " + full;
                return null;
            }
            return texture;
        }

        static int Abs(int value) {
            return value < 0 ? -value : value;
        }

        /// <summary>Wash the base image out so the red diff pixels read at a glance.</summary>
        static Color32 Dim(Color32 color) {
            byte luminance = (byte)((color.r * 77 + color.g * 150 + color.b * 29) >> 8);
            byte washed = (byte)(160 + (luminance >> 2));
            return new Color32(washed, washed, washed, 255);
        }
    }
}
