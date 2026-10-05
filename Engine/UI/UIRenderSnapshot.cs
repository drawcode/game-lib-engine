using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

namespace Engine.UI {

    // Renders a 3D asset ONCE into a small RenderTexture and keeps the texture, so a list of many
    // rows can show a thumbnail per row without a live camera per row (B11.4, 2026-10-04; first
    // caller: the level editor's item picker, 163 rows). UIRenderStage is the LIVE answer — one
    // camera + RT per widget, rendering every frame while visible — and is right for a coin or a
    // character card; it is the wrong tool for a list, where every row would hold a camera.
    //
    // What a snapshot does, per asset code, at most once per cache lifetime:
    //   spawn (caller's Spawner, under a holder parked far outside every camera)
    //   -> UIRenderStage.Attach (the SAME framing as the live widgets, and the layer's SHARED
    //      light through its LIGHTS API: the light is raised to max(visible stages, ours) for the
    //      duration and re-solved by Detach — nothing is left on)
    //   -> one synchronous Camera.Render into a width x height RT owned by this cache
    //   -> Detach + destroy the spawned content (deactivated first, so no other camera ever sees it)
    //
    // The stage's own square RT is never rendered into (the camera is pointed at the cache's RT
    // first), so Unity never allocates it; Detach destroys it unused.
    //
    // COST CONTROL. Requests queue; a single runner renders them in request order inside a
    // per-frame time budget (frameBudgetMs, at least one per frame), so opening a long list costs
    // a few frames of small work, not one long hitch. Callers request visible rows first. The
    // runner disables itself when every queue is empty — zero per-frame cost while idle.
    //
    // LIFETIME. The caller owns the cache: Clear() releases every RT (call it when the view is
    // freed or hidden). Application.lowMemory clears every live cache. liveTextureCount is the
    // engine-wide count of snapshot RTs alive, for probes and tests.
    //
    // UIRef-free on purpose: this produces a Texture; the caller hands it to UIUtil.SetImageTexture.
    public class UIRenderSnapshot {

        // Spawns the content under `parent` (already parked out of view). null = nothing to show.
        public delegate GameObject Spawner(GameObject parent, string code);

        // texture == null: the code has no renderable model (text-only row).
        public delegate void Ready(string code, Texture texture);

        // RT size. Match the element's aspect (UIUtil.SetImageTexture stretches to the element),
        // so a non-square element never squashes the model. Aspect >= 1 keeps the stage framing
        // intact (it fits max(extent x, y) to the vertical half-height).
        public int width = 112;
        public int height = 80;

        public float framePadding = 1.15f;

        // The stage light this snapshot asks for while it renders (<= 0 borrows the layer's).
        public float lightIntensity = 1.1f;

        // < 0: UIRenderStageBinding.DefaultLayer().
        public int layer = -1;

        // Drop renderers that are FLAT along the camera's up axis before framing: a level asset's
        // blob shadow is a horizontal quad, invisible edge-on from the stage camera, yet its bounds
        // (2x the rock it sits under) set the framing and shrank every thumbnail to half size.
        // The spawned copy is thrown away after the render, so removing them costs nothing.
        public bool skipFlatRenderers = true;
        public float flatRatio = .1f;

        // Per-frame render budget across every cache, milliseconds. At least one render per frame.
        public static float frameBudgetMs = 6f;

        // Where spawned content is parked: far outside every game/UI camera and stage frustum.
        public static Vector3 spawnOrigin = new Vector3(0f, -20000f, 0f);

        // PROBES: engine-wide live RT count, and timings of the last / worst render and frame.
        public static int liveTextureCount { get; private set; }
        public static double lastRenderMs { get; private set; }
        public static double maxRenderMs { get; private set; }
        public static double maxFrameMs { get; private set; }
        public static int renderCount { get; private set; }

        internal static void RecordFrame(double ms) {
            if (ms > maxFrameMs) {
                maxFrameMs = ms;
            }
        }

        public static void ResetStats() {
            lastRenderMs = 0;
            maxRenderMs = 0;
            maxFrameMs = 0;
            renderCount = 0;
        }

        private struct Pending {
            public string code;
            public Spawner spawner;
            public Ready ready;
        }

        private readonly Dictionary<string, RenderTexture> cache = new Dictionary<string, RenderTexture>();
        private readonly HashSet<string> empty = new HashSet<string>();
        private readonly List<Pending> pending = new List<Pending>();
        private int pendingHead;

        private static readonly List<UIRenderSnapshot> live = new List<UIRenderSnapshot>();
        private static bool lowMemoryHooked;

        public int cachedCount {
            get {
                return cache.Count;
            }
        }

        public int pendingCount {
            get {
                return pending.Count - pendingHead;
            }
        }

        public bool TryGet(string code, out Texture texture) {

            RenderTexture rt;

            if (code != null && cache.TryGetValue(code, out rt) && rt != null) {
                texture = rt;
                return true;
            }

            texture = null;
            return false;
        }

        // Answers at once from the cache (or the known-empty set); otherwise queues the render
        // and answers from the runner. `ready` is always called exactly once unless the request
        // is cancelled (CancelPending / Clear) first.
        public void Request(string code, Spawner spawner, Ready ready) {

            if (string.IsNullOrEmpty(code) || spawner == null) {
                return;
            }

            Texture texture;

            if (TryGet(code, out texture)) {
                if (ready != null) {
                    ready(code, texture);
                }
                return;
            }

            if (empty.Contains(code)) {
                if (ready != null) {
                    ready(code, null);
                }
                return;
            }

            Pending p;
            p.code = code;
            p.spawner = spawner;
            p.ready = ready;
            pending.Add(p);

            if (!live.Contains(this)) {
                live.Add(this);
            }

            HookLowMemory();
            UIRenderSnapshotRunner.Wake();
        }

        // Drop queued renders (their callbacks never run). Cached textures stay.
        public void CancelPending() {
            pending.Clear();
            pendingHead = 0;
        }

        // Drop queued renders AND release every texture this cache holds.
        public void Clear() {

            CancelPending();

            foreach (KeyValuePair<string, RenderTexture> kv in cache) {
                ReleaseTexture(kv.Value);
            }

            cache.Clear();
            empty.Clear();
            live.Remove(this);
        }

        // Every live cache (Application.lowMemory). Textures already bound to elements go blank.
        public static void ClearAll() {

            for (int i = live.Count - 1; i >= 0; i--) {

                if (i < live.Count) {
                    live[i].Clear();
                }
            }
        }

        // The runner's step: render ONE queued request. false = this cache's queue is empty.
        internal bool ProcessOne() {

            if (pendingHead >= pending.Count) {
                CancelPending();
                return false;
            }

            Pending p = pending[pendingHead];
            pending[pendingHead] = default(Pending);
            pendingHead++;

            if (pendingHead >= pending.Count) {
                CancelPending();
            }

            Texture texture;

            // A duplicate code queued twice: the first render answers the second.
            if (!TryGet(p.code, out texture) && !empty.Contains(p.code)) {

                RenderTexture rt = RenderCode(p.code, p.spawner);

                if (rt != null) {
                    cache[p.code] = rt;
                    texture = rt;
                }
                else {
                    empty.Add(p.code);
                }
            }

            if (p.ready != null) {
                p.ready(p.code, texture);
            }

            return true;
        }

        private RenderTexture RenderCode(string code, Spawner spawner) {

            Stopwatch sw = Stopwatch.StartNew();

            GameObject holder = new GameObject("ui-render-snapshot-holder");
            holder.transform.position = spawnOrigin;

            RenderTexture rt = null;

            try {

                GameObject content = spawner(holder, code);

                if (content != null) {

                    if (skipFlatRenderers) {
                        RemoveFlatRenderers(holder, flatRatio);
                    }

                    int useLayer = layer >= 0 ? layer : UIRenderStageBinding.DefaultLayer();

                    rt = Render(holder, useLayer, width, height, framePadding, lightIntensity);

                    if (rt != null) {
                        rt.name = "ui-render-snapshot-" + code;
                    }
                }
            }
            finally {

                // Out of every camera's sight at once, gone at the end of the frame.
                holder.SetActive(false);
                DestroySafe(holder);
            }

            sw.Stop();
            lastRenderMs = sw.Elapsed.TotalMilliseconds;
            renderCount++;

            if (lastRenderMs > maxRenderMs) {
                maxRenderMs = lastRenderMs;
            }

            return rt;
        }

        // The single-shot core, usable on its own: frame `content` exactly as a UIRenderStage
        // would, render it once on `layer` into a new width x height RT, detach. The caller owns
        // (and must ReleaseTexture) the result. null when the content has no mesh to draw —
        // particle-only content would frame to a point.
        public static RenderTexture Render(
            GameObject content, int layer, int width, int height,
            float framePadding = 1.15f, float lightIntensity = 1.1f) {

            if (content == null || layer < 0 || width <= 0 || height <= 0 || !HasMesh(content)) {
                return null;
            }

            UIRenderStage stage = UIRenderStage.Attach(
                content, layer, Mathf.Max(width, height), framePadding,
                false, false, lightIntensity);

            if (stage == null) {
                return null;
            }

            RenderTexture rt = null;

            try {

                Camera cam = stage.stageCamera;
                cam.enabled = false;   // rendered by hand, once — never by the pipeline

                rt = new RenderTexture(width, height, 16, RenderTextureFormat.ARGB32);
                rt.Create();
                liveTextureCount++;

                cam.targetTexture = rt;
                cam.aspect = (float)width / height;
                cam.Render();
                cam.targetTexture = null;
            }
            finally {
                // Restores layers, re-solves the shared layer light without us, destroys the stage.
                stage.Detach();
            }

            return rt;
        }

        public static void ReleaseTexture(RenderTexture rt) {

            if (rt == null) {
                return;
            }

            rt.Release();
            DestroySafe(rt);
            liveTextureCount--;
        }

        // Non-particle renderers whose world bounds are flat in Y (height < ratio x widest side).
        // The stage camera looks along +Z with Y up, so such a renderer is seen edge-on.
        private static void RemoveFlatRenderers(GameObject content, float ratio) {

            Renderer[] renderers = content.GetComponentsInChildren<Renderer>(true);

            for (int i = 0; i < renderers.Length; i++) {

                Renderer r = renderers[i];

                if (r is ParticleSystemRenderer) {
                    continue;
                }

                Vector3 size = r.bounds.size;
                float wide = Mathf.Max(size.x, size.z);

                if (wide > 0f && size.y < wide * ratio) {
                    DestroySafeImmediate(r);
                }
            }
        }

        // Immediate even at runtime: the framing walks the renderers in this same call.
        private static void DestroySafeImmediate(Object o) {

            if (o != null) {
                Object.DestroyImmediate(o);
            }
        }

        private static bool HasMesh(GameObject content) {

            Renderer[] renderers = content.GetComponentsInChildren<Renderer>(true);

            for (int i = 0; i < renderers.Length; i++) {

                if (!(renderers[i] is ParticleSystemRenderer)) {
                    return true;
                }
            }

            return false;
        }

        private static void HookLowMemory() {

            if (lowMemoryHooked) {
                return;
            }

            lowMemoryHooked = true;
            Application.lowMemory += ClearAll;
        }

        private static void DestroySafe(Object o) {

            if (o == null) {
                return;
            }

            if (Application.isPlaying) {
                Object.Destroy(o);
            }
            else {
                Object.DestroyImmediate(o);
            }
        }

        // The runner drains every live cache within the frame budget.
        internal static bool ProcessBudget(float budgetMs, Stopwatch frame) {

            bool first = true;

            for (int i = 0; i < live.Count; i++) {

                UIRenderSnapshot s = live[i];

                while (s.pendingCount > 0) {

                    if (!first && frame.Elapsed.TotalMilliseconds >= budgetMs) {
                        return true;
                    }

                    first = false;
                    s.ProcessOne();
                }
            }

            // More may have been queued by a ready callback.
            for (int i = 0; i < live.Count; i++) {

                if (live[i].pendingCount > 0) {
                    return true;
                }
            }

            return false;
        }
    }

    // One hidden object that drains the snapshot queues; disabled whenever they are empty.
    public class UIRenderSnapshotRunner : MonoBehaviour {

        private static UIRenderSnapshotRunner instance;
        private readonly Stopwatch frame = new Stopwatch();

        // The frame the queue was (re)filled on renders nothing: that is the frame the caller
        // just built its list in, and its layout + repaint is already the expensive part
        // (measured: 157 rows ~25 ms of UI Toolkit layout/repaint on the open frame).
        private int wakeFrame = -1;

        public static bool isRunning {
            get {
                return instance != null && instance.enabled;
            }
        }

        public static void Wake() {

            // Edit mode (tests): no player loop to spread over — drain now.
            if (!Application.isPlaying) {
                Stopwatch sw = new Stopwatch();
                sw.Start();
                UIRenderSnapshot.ProcessBudget(float.MaxValue, sw);
                return;
            }

            if (instance == null) {
                GameObject go = new GameObject("ui-render-snapshot-runner");
                DontDestroyOnLoad(go);
                instance = go.AddComponent<UIRenderSnapshotRunner>();
                instance.wakeFrame = Time.frameCount;
            }

            if (!instance.enabled) {
                instance.wakeFrame = Time.frameCount;
            }

            instance.enabled = true;
        }

        void LateUpdate() {

            if (Time.frameCount == wakeFrame) {
                return;
            }

            frame.Reset();
            frame.Start();

            bool more = UIRenderSnapshot.ProcessBudget(UIRenderSnapshot.frameBudgetMs, frame);

            frame.Stop();

            double ms = frame.Elapsed.TotalMilliseconds;

            UIRenderSnapshot.RecordFrame(ms);

            if (!more) {
                enabled = false;
            }
        }
    }
}
