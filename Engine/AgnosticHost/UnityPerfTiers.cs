using System;
using System.Collections.Generic;
using System.Reflection;

using Agnostic.Core;

using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Engine.AgnosticHost {

    // The Unity adapter for the agnostic perf tiers (Agnostic.Core.PerfGovernor, perf-tiers.v1).
    // Design: contexts/gamedev/agnostic/context-agnostic-perf-tiers.md.
    //
    // The core decides the tier; this maps the tier's named numbers onto URP and QualitySettings:
    //
    //   ao              renderer index on every registered world camera (0 default forward,
    //                   1 forward + SSAO lite, 2 forward + SSAO full, as listed on each URP asset).
    //                   Per camera, so no asset is written at runtime.
    //   lodBias         QualitySettings.lodBias
    //   maxLodLevel     QualitySettings.maximumLODLevel
    //   shadowDistance  the active URP asset's shadowDistance
    //   renderScale     the active URP asset's renderScale
    //
    // A feature the tier does not name is left as authored. The last four ARE settings/asset
    // writes (in the Editor they would persist past Play), so the authored values are snapshotted
    // on first write and restored on teardown.
    //
    // SSAO needs a positive near plane: URP reconstructs eye depth from near/far, so an orthographic
    // world camera authored with near < 0 (action-bots CameraMain: -300..100, so the ground at the
    // camera's own height is drawn) renders AO that ramps to black with depth. While an AO renderer
    // is on, such a camera is rendered from its eye pulled back by -near along its view axis with
    // near/far shifted by the same amount: the identical orthographic slab, pixel for pixel. It is
    // applied inside beginCameraRendering and undone in endCameraRendering, so gameplay code
    // (Camera.main.ScreenToWorldPoint, rays) never sees a changed camera.
    //
    // Frame time comes from FrameTimingManager when Frame Timing Stats is on (real work time, so the
    // governor sees load and headroom under vsync, judged by workDownRatio/workUpRatio); otherwise from
    // the frame delta, judged by downRatio/upRatio. Thermal comes from
    // Adaptive Performance through Engine.AgnosticHost.AdaptivePerformance, bound by name, and
    // reads 0 where that assembly or a provider is absent.
    [DefaultExecutionOrder(-990)]
    public class UnityPerfTiers : MonoBehaviour {

        public const string featureAo = "ao";
        public const string featureLodBias = "lodBias";
        public const string featureMaxLodLevel = "maxLodLevel";
        public const string featureShadowDistance = "shadowDistance";
        public const string featureRenderScale = "renderScale";

        public PerfDef def;
        public PerfDevice device;
        public PerfGovernor governor;

        // The game's "not in a round" test: while it returns true the governor is held, so menus
        // and loads never count against a tier.
        public Func<bool> holdWhen;

        // Debug/A-B: apply this tier's features regardless of the governor. -1 = off.
        public int overrideTier = -1;

        public event Action<string, string, string> tierChanged;

        public int appliedTier = -1;
        public double lastFrameMs;
        public double lastThermal;
        public bool frameTimingLive;

        private readonly List<UniversalAdditionalCameraData> worldCameras = new List<UniversalAdditionalCameraData>();
        private readonly Dictionary<UniversalAdditionalCameraData, int> cameraRendererAuthored =
            new Dictionary<UniversalAdditionalCameraData, int>();

        private readonly Dictionary<UniversalRenderPipelineAsset, float> shadowAuthored =
            new Dictionary<UniversalRenderPipelineAsset, float>();
        private readonly Dictionary<UniversalRenderPipelineAsset, float> scaleAuthored =
            new Dictionary<UniversalRenderPipelineAsset, float>();

        private bool lodBiasSaved;
        private float lodBiasAuthored;
        private bool maxLodSaved;
        private int maxLodAuthored;

        private RenderPipelineAsset appliedPipeline;

        private bool aoActive;
        private bool depthHooked;
        private Camera shiftedCamera;
        private float shiftedNear;
        private float shiftedFar;

        // Kept above zero so the shifted near is never on the eye itself.
        public const float orthoNearMargin = 0.01f;
        private readonly FrameTiming[] timings = new FrameTiming[1];

        private static readonly Func<float> thermalRead = BindThermal();
        private static FieldInfo rendererIndexField;

        public static UnityPerfTiers Create(PerfDef def, PerfDevice device, string name = "_PerfTiers") {

            GameObject go = new GameObject(name);
            DontDestroyOnLoad(go);

            UnityPerfTiers t = go.AddComponent<UnityPerfTiers>();
            t.def = def;
            t.device = device ?? DeviceFromSystem();
            t.governor = new PerfGovernor(def, null);
            t.governor.Classify(t.device);
            t.Apply();
            return t;
        }

        public static PerfDevice DeviceFromSystem() {

            PerfDevice d = new PerfDevice();
            d.platform = Application.platform.ToString();
            d.model = SystemInfo.deviceModel ?? "";
            d.gpu = SystemInfo.graphicsDeviceName ?? "";
            d.memoryMB = SystemInfo.systemMemorySize;
            d.gpuMemoryMB = SystemInfo.graphicsMemorySize;
            d.cores = SystemInfo.processorCount;
            d.shaderLevel = SystemInfo.graphicsShaderLevel;
            return d;
        }

        public int activeTier {
            get {
                return overrideTier >= 0 && def != null && overrideTier < def.tiers.Count ? overrideTier
                    : governor != null ? governor.tier : -1;
            }
        }

        public string activeTierId {
            get {
                int t = activeTier;
                return t >= 0 ? def.tiers[t].id : "";
            }
        }

        // The active tier's value for a feature, or the fallback when it does not name it.
        public double Feature(string name, double fallback) {

            int t = activeTier;

            if (t < 0) {
                return fallback;
            }

            double v;
            return def.tiers[t].features.TryGetValue(name, out v) ? v : fallback;
        }

        // A camera that draws the level. Its renderer follows the tier's `ao` from now on.
        public void RegisterWorldCamera(Camera cam) {

            if (cam == null) {
                return;
            }

            UniversalAdditionalCameraData data = cam.GetComponent<UniversalAdditionalCameraData>();

            if (data == null || worldCameras.Contains(data)) {
                return;
            }

            worldCameras.Add(data);
            cameraRendererAuthored[data] = RendererIndexOf(data);
            ApplyAo();
        }

        public void UnregisterWorldCamera(Camera cam) {

            if (cam == null) {
                return;
            }

            UniversalAdditionalCameraData data = cam.GetComponent<UniversalAdditionalCameraData>();

            if (data == null || !worldCameras.Remove(data)) {
                return;
            }

            RestoreCamera(data);
        }

        // Call on a level load so its hitch is not counted against the tier.
        public void ResetWindow() {

            if (governor != null) {
                governor.ResetWindow();
            }
        }

        public void SetOverride(int tierIndex) {
            overrideTier = tierIndex;
            Apply();
        }

        private void Update() {

            if (governor == null) {
                return;
            }

            double dt = Time.unscaledDeltaTime;
            lastFrameMs = FrameMs(dt);
            lastThermal = thermalRead != null ? thermalRead() : 0.0;
            governor.held = holdWhen != null && holdWhen();
            // Work time (FrameTimingManager) shows GPU/CPU load under vsync, so it gets the tighter
            // work ratios: a busy or heating GPU steps down before frames actually drop.
            governor.workTiming = frameTimingLive;

            string before = governor.tierId;

            if (governor.Tick(dt, lastFrameMs, lastThermal)) {
                Apply();

                if (tierChanged != null) {
                    tierChanged(before, governor.tierId, governor.reason);
                }
#if UNITY_EDITOR || DEVELOPMENT_BUILD
                Debug.Log("[perf] tier " + before + " -> " + governor.tierId + " (" + governor.reason
                    + ", " + governor.smoothedFrameMs.ToString("F1") + " ms, thermal " + lastThermal.ToString("F2") + ")");
#endif
            }
            else if (GraphicsSettings.currentRenderPipeline != appliedPipeline) {
                // A quality-level switch swaps the URP asset; its own values need this tier's too.
                Apply();
            }
        }

        private void OnDestroy() {
            HookDepthRange(false);
            Restore();
        }

        public void Apply() {

            appliedTier = activeTier;
            appliedPipeline = GraphicsSettings.currentRenderPipeline;

            if (appliedTier < 0) {
                return;
            }

            ApplyAo();

            double lodBias = Feature(featureLodBias, -1.0);

            if (lodBias > 0.0) {

                if (!lodBiasSaved) {
                    lodBiasAuthored = QualitySettings.lodBias;
                    lodBiasSaved = true;
                }

                QualitySettings.lodBias = (float)lodBias;
            }

            double maxLod = Feature(featureMaxLodLevel, -1.0);

            if (maxLod >= 0.0) {

                if (!maxLodSaved) {
                    maxLodAuthored = QualitySettings.maximumLODLevel;
                    maxLodSaved = true;
                }

                QualitySettings.maximumLODLevel = (int)maxLod;
            }

            UniversalRenderPipelineAsset urp = appliedPipeline as UniversalRenderPipelineAsset;

            if (urp == null) {
                return;
            }

            double shadow = Feature(featureShadowDistance, -1.0);

            if (shadow >= 0.0) {

                if (!shadowAuthored.ContainsKey(urp)) {
                    shadowAuthored[urp] = urp.shadowDistance;
                }

                urp.shadowDistance = (float)shadow;
            }

            double scale = Feature(featureRenderScale, -1.0);

            if (scale > 0.0) {

                if (!scaleAuthored.ContainsKey(urp)) {
                    scaleAuthored[urp] = urp.renderScale;
                }

                urp.renderScale = (float)scale;
            }
        }

        private void ApplyAo() {

            if (activeTier < 0) {
                return;
            }

            UniversalRenderPipelineAsset urp = GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;
            int count = urp != null ? urp.rendererDataList.Length : 0;
            int index = (int)Math.Round(Feature(featureAo, 0.0));
            aoActive = index > 0 && index < count;
            HookDepthRange(aoActive && worldCameras.Count > 0);

            for (int i = worldCameras.Count - 1; i >= 0; i--) {
                UniversalAdditionalCameraData data = worldCameras[i];

                if (data == null) {
                    worldCameras.RemoveAt(i);
                    continue;
                }

                // An asset that does not list the AO renderers keeps the camera's own renderer.
                int authored;
                cameraRendererAuthored.TryGetValue(data, out authored);
                data.SetRenderer(index > 0 && index < count ? index : authored);
            }
        }

        private void Restore() {

            for (int i = 0; i < worldCameras.Count; i++) {
                RestoreCamera(worldCameras[i]);
            }

            worldCameras.Clear();

            if (lodBiasSaved) {
                QualitySettings.lodBias = lodBiasAuthored;
                lodBiasSaved = false;
            }

            if (maxLodSaved) {
                QualitySettings.maximumLODLevel = maxLodAuthored;
                maxLodSaved = false;
            }

            foreach (KeyValuePair<UniversalRenderPipelineAsset, float> kv in shadowAuthored) {
                if (kv.Key != null) {
                    kv.Key.shadowDistance = kv.Value;
                }
            }

            foreach (KeyValuePair<UniversalRenderPipelineAsset, float> kv in scaleAuthored) {
                if (kv.Key != null) {
                    kv.Key.renderScale = kv.Value;
                }
            }

            shadowAuthored.Clear();
            scaleAuthored.Clear();
        }

        private void RestoreCamera(UniversalAdditionalCameraData data) {

            int authored;

            if (data != null && cameraRendererAuthored.TryGetValue(data, out authored)) {
                data.SetRenderer(authored);
            }

            cameraRendererAuthored.Remove(data);
        }

        private void HookDepthRange(bool on) {

            if (on == depthHooked) {
                return;
            }

            depthHooked = on;

            if (on) {
                RenderPipelineManager.beginCameraRendering += OnBeginCamera;
                RenderPipelineManager.endCameraRendering += OnEndCamera;
            }
            else {
                RenderPipelineManager.beginCameraRendering -= OnBeginCamera;
                RenderPipelineManager.endCameraRendering -= OnEndCamera;
                UnshiftCamera();
            }
        }

        private void OnBeginCamera(ScriptableRenderContext context, Camera cam) {

            if (cam == null || !cam.orthographic || cam.nearClipPlane >= 0f) {
                return;
            }

            UniversalAdditionalCameraData data = cam.GetComponent<UniversalAdditionalCameraData>();

            if (data == null || !worldCameras.Contains(data)) {
                return;
            }

            float pullback = orthoNearMargin - cam.nearClipPlane;
            shiftedCamera = cam;
            shiftedNear = cam.nearClipPlane;
            shiftedFar = cam.farClipPlane;

            // Unity's view matrix looks down -Z: pulling the eye back along forward moves every point
            // pullback further down -Z.
            Matrix4x4 view = Matrix4x4.Scale(new Vector3(1f, 1f, -1f)) * cam.transform.worldToLocalMatrix;
            cam.worldToCameraMatrix = Matrix4x4.Translate(new Vector3(0f, 0f, -pullback)) * view;
            cam.nearClipPlane = shiftedNear + pullback;
            cam.farClipPlane = shiftedFar + pullback;
        }

        private void OnEndCamera(ScriptableRenderContext context, Camera cam) {

            if (cam == shiftedCamera) {
                UnshiftCamera();
            }
        }

        private void UnshiftCamera() {

            if (shiftedCamera == null) {
                shiftedCamera = null;
                return;
            }

            shiftedCamera.ResetWorldToCameraMatrix();
            shiftedCamera.nearClipPlane = shiftedNear;
            shiftedCamera.farClipPlane = shiftedFar;
            shiftedCamera = null;
        }

        private double FrameMs(double dt) {

            FrameTimingManager.CaptureFrameTimings();

            if (FrameTimingManager.GetLatestTimings(1, timings) > 0) {
                FrameTiming t = timings[0];
                // The main thread's frame time includes its wait for vsync Present; only the rest is work.
                double main = Math.Max(0.0, t.cpuMainThreadFrameTime - t.cpuMainThreadPresentWaitTime);
                double work = Math.Max(main, Math.Max(t.cpuRenderThreadFrameTime, t.gpuFrameTime));

                if (work > 0.0) {
                    frameTimingLive = true;
                    return work;
                }
            }

            frameTimingLive = false;
            return dt * 1000.0;
        }

        // m_RendererIndex has no public getter; -1 means "the asset's default renderer".
        private static int RendererIndexOf(UniversalAdditionalCameraData data) {

            if (rendererIndexField == null) {
                rendererIndexField = typeof(UniversalAdditionalCameraData).GetField("m_RendererIndex",
                    BindingFlags.Instance | BindingFlags.NonPublic);
            }

            return rendererIndexField != null ? (int)rendererIndexField.GetValue(data) : -1;
        }

        private static Func<float> BindThermal() {

            Type t = Type.GetType("Engine.AgnosticHost.Perf.AdaptivePerformanceThermal, Engine.AgnosticHost.AdaptivePerformance");
            MethodInfo m = t != null ? t.GetMethod("TemperatureLevel", BindingFlags.Public | BindingFlags.Static) : null;
            return m != null ? (Func<float>)Delegate.CreateDelegate(typeof(Func<float>), m) : null;
        }
    }
}
