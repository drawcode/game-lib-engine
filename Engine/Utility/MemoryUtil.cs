using System;
using System.Collections;

using UnityEngine;
using UnityEngine.Scripting;

/// <summary>
/// The one place the game asks for memory to be reclaimed.
///
/// Before this, `GC.Collect()` was called directly from a HUD coroutine, from two audio
/// recorders and from the audio downloader. Every one of those is a full, blocking,
/// stop-the-world mark and sweep, and at least one of them (BaseGameHUD's vehicle-sound
/// coroutine) can fire while the game is being played -- which is exactly what a frame
/// spike on GC.Collect looks like.
///
/// The rule this util enforces:
///
///   * While gameplay is live (`busy`), NOTHING blocks. Reclaim happens as incremental
///     slices bounded by `sliceMilliseconds`, spread across frames.
///   * A full collect and an asset unload only happen at a SAFE POINT -- a loading
///     screen, a results screen, a pause -- where a long frame costs nothing.
///   * Requests are coalesced. Ten callers asking for a collect in the same second get
///     one collection, not ten.
///
/// Incremental slicing requires "Use Incremental GC" in Player Settings
/// (`PlayerSettings.gcIncremental`). If it is off, `GarbageCollector.isIncremental` is
/// false and this util degrades to deferring everything to safe points, which is still a
/// large improvement on collecting mid-frame.
/// </summary>
public class MemoryUtil : GameObjectBehavior {

    // ------------------------------------------------------------------
    // TUNING

    /// <summary>
    /// Per-frame budget for incremental marking, in milliseconds. Unity's own default
    /// slice is 3ms; 2ms leaves more headroom on a 60fps mobile frame and simply means a
    /// cycle takes a few more frames to finish.
    /// </summary>
    public static float sliceMilliseconds = 2f;

    /// <summary>
    /// A full collect at a safe point will not run again within this many seconds. Stops
    /// a screen that transitions several times in a row from collecting on each one.
    /// </summary>
    public static float fullCollectCooldownSeconds = 20f;

    /// <summary>
    /// Managed heap growth, in bytes, that starts an incremental cycle on its own without
    /// anybody having to ask. 8MB is roughly a minute of combat allocation in this game.
    /// </summary>
    public static long heapGrowthTriggerBytes = 8 * 1024 * 1024;

    /// <summary>
    /// How often to sample the managed heap size. `GC.GetTotalMemory(false)` is cheap but
    /// not free, and nothing here needs frame accuracy.
    /// </summary>
    public static int heapSampleFrameInterval = 30;

    /// <summary>
    /// Release pooled objects beyond this many per bucket when a safe point runs.
    /// `Resources.UnloadUnusedAssets` cannot free a mesh, material or texture that a
    /// parked pool object still references, so without this the level you just left stays
    /// resident behind a few hundred recycled bullets and effects.
    /// </summary>
    public static int poolKeepPerBucket = 16;

    public static bool trimPoolsAtSafePoint = true;

    /// <summary>
    /// Re-derive the numbers above from the device on first run. A 2GB phone should
    /// collect sooner, in smaller bites, and be allowed to unload more often than a
    /// desktop with 32GB and eight cores. Set false BEFORE the driver is created if you
    /// want to pin your own values.
    /// </summary>
    public static bool autoTuneForDevice = true;

    public static bool logEnabled = false;

    /// <summary>
    /// Coarse device tier, derived once from system memory and core count. Exposed so
    /// gameplay code can make its own quality calls off the same classification instead
    /// of inventing a second one.
    /// </summary>
    public enum DeviceTier {
        Low,
        Medium,
        High
    }

    public static DeviceTier deviceTier { get; private set; }

    // ------------------------------------------------------------------
    // STATE

    /// <summary>
    /// True while a frame spike would be felt -- set this from the gameplay layer when a
    /// round starts and clear it when it ends. game-lib-engine cannot see game state, so
    /// the game layer has to tell it.
    /// </summary>
    public static bool busy = false;

    private static bool pendingIncremental = false;
    private static bool pendingSafePointCollect = false;
    private static bool pendingSafePointUnload = false;
    private static bool safePointRunning = false;

    private static float lastFullCollectTime = -99999f;
    private static long heapBaselineBytes = 0;

    public static int incrementalCycles { get; private set; }
    public static int fullCollects { get; private set; }
    public static int unloads { get; private set; }
    public static int poolObjectsTrimmed { get; private set; }

    /// <summary>
    /// Raised at the START of a safe point, before anything is collected, so a product can
    /// drop its own caches while the frame is already being spent. The argument is the
    /// reason string the safe point was requested with. Keep handlers cheap and
    /// exception-free -- one that throws takes the rest of the safe point with it.
    /// </summary>
    public static event Action<string> onSafePointReclaim;

    // ------------------------------------------------------------------
    // SINGLETON -- same shape as CoroutineUtil

    private static MemoryUtil instance;

    private static MemoryUtil Instance {
        get {

            if (instance != null) {
                return instance;
            }

            instance = UnityObjectUtil.FindObject<MemoryUtil>();

            if (instance != null) {
                return instance;
            }

            // Never spawn the driver outside Play mode. A static call from an editor
            // script or an inspector would otherwise add "_MemoryUtil" to whatever scene
            // happened to be open and dirty it. There is nothing to drive in edit mode
            // anyway -- no frames, no gameplay, no spike to avoid.

            if (!Application.isPlaying) {
                return null;
            }

            instance =
                new GameObject("_MemoryUtil", typeof(MemoryUtil))
                    .GetComponent<MemoryUtil>();

            return instance;
        }
    }

    /// <summary>
    /// Optional -- any static request creates the driver on demand. Call it from app boot
    /// if you want the object to exist before the first one.
    /// </summary>
    public static void Init() {
        if (Instance != null) {
            // Reading Instance is the initialisation.
        }
    }

    private void Awake() {

        if (instance != null && instance != this) {
            Destroy(gameObject);
            return;
        }

        instance = this;

        DontDestroyOnLoad(gameObject);

        heapBaselineBytes = GC.GetTotalMemory(false);

        TuneForDevice();

        // Unity's default slice is set per-project; make ours the one the tuning field
        // above describes so a caller reading this file gets what it says.

        if (isIncrementalAvailable) {
            GarbageCollector.incrementalTimeSliceNanoseconds = SliceNanoseconds();
        }

        // The OS telling us it is about to start killing apps. This is the one case where
        // a spike is the lesser evil, so it runs even mid-round.

        Application.lowMemory += OnLowMemory;
    }

    private void OnDestroy() {

        if (instance == this) {
            Application.lowMemory -= OnLowMemory;
            instance = null;
        }
    }

    private static void TuneForDevice() {

        // systemMemorySize is in MB and is 0 on platforms that will not say; treat
        // unknown as Medium rather than punishing it.

        int memoryMB = SystemInfo.systemMemorySize;
        int cores = SystemInfo.processorCount;

        if (memoryMB > 0 && (memoryMB < 3072 || cores <= 4)) {
            deviceTier = DeviceTier.Low;
        }
        else if (memoryMB > 0 && memoryMB < 6144) {
            deviceTier = DeviceTier.Medium;
        }
        else {
            deviceTier = DeviceTier.High;
        }

        if (!autoTuneForDevice) {
            return;
        }

        // Low-end: collect sooner and more often, in smaller bites. A smaller trigger
        // means each incremental cycle has less to mark, which is what keeps the slice
        // honest -- not a bigger budget.

        if (deviceTier == DeviceTier.Low) {
            heapGrowthTriggerBytes = 4 * 1024 * 1024;
            sliceMilliseconds = 1.5f;
            fullCollectCooldownSeconds = 8f;
            poolKeepPerBucket = 4;
        }
        else if (deviceTier == DeviceTier.Medium) {
            heapGrowthTriggerBytes = 8 * 1024 * 1024;
            sliceMilliseconds = 2f;
            fullCollectCooldownSeconds = 15f;
            poolKeepPerBucket = 8;
        }
        else {
            heapGrowthTriggerBytes = 16 * 1024 * 1024;
            sliceMilliseconds = 3f;
            fullCollectCooldownSeconds = 30f;
            poolKeepPerBucket = 16;
        }
    }

    private void OnLowMemory() {

        LogUtil.LogWarning(
            "MemoryUtil:Application.lowMemory -- collecting immediately, busy:" + busy
            + " tier:" + deviceTier
            + " heapMB:" + (GC.GetTotalMemory(false) / (1024 * 1024)));

        pendingSafePointCollect = true;
        pendingSafePointUnload = true;

        // Deliberately ignores `busy` and the cooldown. Being killed by the OS costs more
        // than a dropped frame.

        RunSafePoint("low-memory");
    }

    /// <summary>
    /// Backgrounding is a free safe point -- nothing is being drawn, so a long frame costs
    /// nothing, and on mobile this is exactly when the OS is deciding whether to keep the
    /// process alive.
    /// </summary>
    private void OnApplicationPause(bool paused) {

        if (!paused) {
            return;
        }

        pendingSafePointCollect = true;
        pendingSafePointUnload = true;

        RunSafePoint("application-paused");
    }

    // ------------------------------------------------------------------
    // CAPABILITY

    /// <summary>
    /// Incremental collection is only possible when the player setting is on AND the
    /// collector has not been switched to Manual/Disabled by somebody else.
    /// </summary>
    public static bool isIncrementalAvailable {
        get {
            return GarbageCollector.isIncremental
                && GarbageCollector.GCMode == GarbageCollector.Mode.Enabled;
        }
    }

    private static ulong SliceNanoseconds() {
        float ms = sliceMilliseconds < 0.25f ? 0.25f : sliceMilliseconds;
        return (ulong)(ms * 1000000f);
    }

    // ------------------------------------------------------------------
    // PUBLIC API

    /// <summary>
    /// The gameplay layer flips this. While true, no blocking collect and no asset unload
    /// will run -- requests are held until it goes false or until someone explicitly asks
    /// for a safe point.
    /// </summary>
    public static void SetBusy(bool value) {

        if (busy == value) {
            return;
        }

        busy = value;

        Init();

        // Leaving gameplay IS a safe point. Anything that was requested during the round
        // gets serviced now, on the transition, where the frame cost is invisible.

        MemoryUtil driver = Instance;

        if (!busy && driver != null
            && (pendingSafePointCollect || pendingSafePointUnload)) {

            driver.RunSafePoint("busy-cleared");
        }
    }

    /// <summary>
    /// "Some memory just became garbage." The cheap, always-safe call -- use this at the
    /// end of a wave, when a pool is trimmed, when a panel is torn down. It never blocks:
    /// while busy it just nudges the incremental collector along.
    /// </summary>
    public static void RequestCollect(string reason) {

        Init();

        pendingIncremental = true;

        if (busy || !CanFullCollectNow()) {
            Log("RequestCollect deferred to incremental", reason);
            return;
        }

        pendingSafePointCollect = true;

        RunSafePointIfPossible(reason);
    }

    /// <summary>
    /// Ask for unreferenced assets to be released. This is the expensive one -- it walks
    /// every loaded object -- so it is ALWAYS deferred to a safe point, never run inline.
    /// </summary>
    public static void RequestUnloadUnusedAssets(string reason) {

        Init();

        pendingSafePointUnload = true;

        if (busy || !CanFullCollectNow()) {
            Log("RequestUnloadUnusedAssets deferred", reason);
            return;
        }

        RunSafePointIfPossible(reason);
    }

    /// <summary>
    /// Declare that right now is a safe point -- a loading screen, results, a pause -- and
    /// service everything that has been asked for. This is where a full collect and an
    /// asset unload actually happen. Returns immediately; the work runs as a coroutine.
    ///
    /// Honours `fullCollectCooldownSeconds`, so it is safe to call from a transition that
    /// a player can bounce in and out of -- menu, results, pause -- without collecting on
    /// every bounce.
    /// </summary>
    public static void CollectAtSafePoint(string reason) {
        CollectAtSafePoint(reason, false);
    }

    /// <summary>
    /// As above, but `force` ignores the cooldown. Use it for the transitions that are
    /// genuinely worth a collection every single time and cannot repeat quickly -- a level
    /// load, a level teardown -- and NOT for anything a player can spam.
    /// </summary>
    public static void CollectAtSafePoint(string reason, bool force) {

        Init();

        if (!force && !CanFullCollectNow()) {
            Log("CollectAtSafePoint suppressed by cooldown", reason);
            return;
        }

        pendingSafePointCollect = true;
        pendingSafePointUnload = true;

        RunSafePointIfPossible(reason);
    }

    /// <summary>
    /// The escape hatch: a genuine, immediate, blocking collect. Only correct when the
    /// frame is already lost -- straight after a scene load, or after freeing a large
    /// native buffer the collector cannot see. Warns if called during gameplay.
    /// </summary>
    public static void CollectBlocking(string reason) {

        if (busy) {
            LogUtil.LogWarning(
                "MemoryUtil:CollectBlocking called while busy -- this is a frame spike. Reason: "
                + reason);
        }

        GC.Collect();

        fullCollects++;
        lastFullCollectTime = Time.realtimeSinceStartup;
        heapBaselineBytes = GC.GetTotalMemory(false);
        pendingIncremental = false;

        Log("CollectBlocking", reason);
    }

    // ------------------------------------------------------------------
    // INCREMENTAL DRIVER

    private void Update() {

        if (!isIncrementalAvailable) {
            return;
        }

        // Sample the heap occasionally rather than every frame, and start a cycle on
        // growth alone so a long round does not rely on somebody remembering to ask.

        if (!pendingIncremental
            && (Time.frameCount % heapSampleFrameInterval) == 0) {

            long total = GC.GetTotalMemory(false);

            if (total - heapBaselineBytes > heapGrowthTriggerBytes) {
                pendingIncremental = true;
            }
        }

        if (!pendingIncremental) {
            return;
        }

        // CollectIncremental returns true while there is still work left in the cycle.
        // Each call is bounded by the slice, so the cost per frame is what we asked for
        // and not what the heap happens to need.

        bool moreWork = GarbageCollector.CollectIncremental(SliceNanoseconds());

        if (!moreWork) {

            pendingIncremental = false;
            incrementalCycles++;
            heapBaselineBytes = GC.GetTotalMemory(false);

            Log("incremental cycle finished", null);
        }
    }

    // ------------------------------------------------------------------
    // SAFE POINT

    /// <summary>
    /// Everything that has to let go of a reference before a collect is worth running.
    /// Returns how many pooled objects were destroyed.
    /// </summary>
    private static int ReclaimReferences(string reason) {

        int trimmed = 0;

        if (trimPoolsAtSafePoint) {

            trimmed += ObjectPoolKeyedManager.trimPooled(poolKeepPerBucket);
            trimmed += ObjectPoolManager.trimPooled(poolKeepPerBucket);

            poolObjectsTrimmed += trimmed;
        }

        Action<string> handlers = onSafePointReclaim;

        if (handlers != null) {

            // A product's handler must not be able to abort the collect that follows.

            try {
                handlers(reason);
            }
            catch (Exception e) {
                LogUtil.LogWarning("MemoryUtil:onSafePointReclaim handler threw: " + e);
            }
        }

        return trimmed;
    }

    private static void RunSafePointIfPossible(string reason) {

        MemoryUtil driver = Instance;

        if (driver == null) {
            return;
        }

        driver.RunSafePoint(reason);
    }

    private static bool CanFullCollectNow() {
        return Time.realtimeSinceStartup - lastFullCollectTime >= fullCollectCooldownSeconds;
    }

    private void RunSafePoint(string reason) {

        if (safePointRunning) {
            return;
        }

        if (!pendingSafePointCollect && !pendingSafePointUnload) {
            return;
        }

        safePointRunning = true;

        StartCoroutine(SafePointCo(reason));
    }

    private IEnumerator SafePointCo(string reason) {

        // Let the frame that asked finish first. A teardown that requests a collect is
        // usually mid-teardown, and the objects it is about to drop are still rooted.

        yield return null;

        bool doUnload = pendingSafePointUnload;
        bool doCollect = pendingSafePointCollect;

        pendingSafePointUnload = false;
        pendingSafePointCollect = false;

        // Drop references FIRST. A collect that runs before the pools are trimmed frees
        // nothing they are holding, and the unload that follows still sees every asset as
        // referenced. Order is: release -> collect managed -> unload assets -> sweep.

        int trimmed = ReclaimReferences(reason);

        if (doCollect) {

            // Managed first, so the unload below sees assets whose last managed
            // reference has already gone.

            GC.Collect();
            fullCollects++;
        }

        if (doUnload) {

            AsyncOperation op = Resources.UnloadUnusedAssets();

            while (op != null && !op.isDone) {
                yield return null;
            }

            unloads++;

            // Sweep what the unload just released. Still inside the safe point.

            GC.Collect();
            fullCollects++;
        }

        lastFullCollectTime = Time.realtimeSinceStartup;
        heapBaselineBytes = GC.GetTotalMemory(false);
        pendingIncremental = false;

        safePointRunning = false;

        Log("safe point serviced"
            + (doCollect ? " collect" : "")
            + (doUnload ? " unload" : "")
            + " pooledTrimmed:" + trimmed, reason);
    }

    // ------------------------------------------------------------------

    private static void Log(string message, string reason) {

        if (!logEnabled) {
            return;
        }

        LogUtil.Log("MemoryUtil:" + message
            + (string.IsNullOrEmpty(reason) ? "" : " reason:" + reason)
            + " heapMB:" + (GC.GetTotalMemory(false) / (1024 * 1024)));
    }
}
