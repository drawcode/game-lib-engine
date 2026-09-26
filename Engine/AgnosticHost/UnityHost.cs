using System;
using System.Collections.Generic;
using System.Globalization;

using Agnostic.Core;
using Agnostic.Host;

using UnityEngine;

using DisplayInfo = Agnostic.Host.DisplayInfo;

namespace Engine.AgnosticHost {

    // The Unity reference host: all 12 IHost* over the house facades, plus the core services a
    // host owns (clock, bus, scheduler), mirroring Agnostic.Null.NullHost member for member.
    //
    // Engine types stop here. Everything above this assembly boundary sees Handles and values.
    // The frame itself is driven by UnityHostDriver, which reproduces NullHost.Frame's order.
    public class UnityHost : IHost {

        public readonly UnityHandleTable handles = new UnityHandleTable();

        public readonly UnityClock unityClock = new UnityClock();
        public readonly UnityLog unityLog = new UnityLog();
        public readonly UnityDisplay unityDisplay = new UnityDisplay();
        public readonly UnityPlatform unityPlatform = new UnityPlatform();
        public readonly UnityWorld unityWorld;
        public readonly UnityInput unityInput;
        public readonly UnityProps unityProps;
        public readonly UnityAnim unityAnim;
        public readonly UnityAudio unityAudio;
        public readonly UnityStorage unityStorage;
        public readonly UnityAssets unityAssets;
        public readonly UnityPhysics unityPhysics;

        public readonly GameClock gameClock = new GameClock();
        public readonly EventBus bus = new EventBus();
        public readonly Scheduler scheduler = new Scheduler();

        public UnityHost() {
            unityWorld = new UnityWorld(handles, unityLog);
            unityInput = new UnityInput(unityDisplay, unityLog);
            unityProps = new UnityProps(handles);
            unityAnim = new UnityAnim(handles);
            unityAudio = new UnityAudio(handles, unityLog);
            unityStorage = new UnityStorage(unityLog);
            unityAssets = new UnityAssets(handles);
            unityPhysics = new UnityPhysics(handles);
            bus.log = unityLog;
        }

        public IHostClock clock {
            get {
                return unityClock;
            }
        }

        public IHostWorld world {
            get {
                return unityWorld;
            }
        }

        public IHostInput input {
            get {
                return unityInput;
            }
        }

        public IHostProps props {
            get {
                return unityProps;
            }
        }

        public IHostAnim anim {
            get {
                return unityAnim;
            }
        }

        public IHostAudio audio {
            get {
                return unityAudio;
            }
        }

        public IHostStorage storage {
            get {
                return unityStorage;
            }
        }

        public IHostAssets assets {
            get {
                return unityAssets;
            }
        }

        public IHostPhysics physics {
            get {
                return unityPhysics;
            }
        }

        public IHostDisplay display {
            get {
                return unityDisplay;
            }
        }

        public IHostPlatform platform {
            get {
                return unityPlatform;
            }
        }

        public IHostLog log {
            get {
                return unityLog;
            }
        }

        // One engine frame in NullHost.Frame's order, with the two steps only a real engine has
        // placed where the contract puts them: raw input before everything, contacts before any
        // OnFixed. Returns the fixed steps run.
        public int Frame(float unscaledDt, ICoreLoop loop) {

            unityInput.Poll();

            int steps = gameClock.Tick(unscaledDt);

            unityAssets.PumpAsync();
            unityPhysics.DeliverContacts();

            if (loop != null) {
                loop.OnFrame(unscaledDt);
            }

            for (int i = 0; i < steps; i++) {

                if (loop != null) {
                    loop.OnFixed(gameClock.fixedDt);
                }
            }

            scheduler.Advance(gameClock.now.dt, gameClock.now.unscaledDt);
            bus.Drain(8);
            unityAudio.Prune();
            return steps;
        }

        public void Lifecycle(AppLifecycle e, ICoreLoop loop) {

            if (e == AppLifecycle.paused || e == AppLifecycle.focusLost) {
                unityInput.ReleaseAll();
            }

            if (loop != null) {
                loop.OnLifecycle(e);
            }
        }
    }

    // Drives a UnityHost from Unity's player loop. Add one to a persistent object and hand it the
    // core's ICoreLoop; nothing else in the game needs to know the core has a frame.
    public class UnityHostDriver : MonoBehaviour {

        public UnityHost host;
        public ICoreLoop loop;

        public int lastSteps;

        public static UnityHostDriver Create(UnityHost host, ICoreLoop loop, string name = "_AgnosticHost") {

            GameObject go = new GameObject(name);
            DontDestroyOnLoad(go);

            UnityHostDriver driver = go.AddComponent<UnityHostDriver>();
            driver.host = host;
            driver.loop = loop;
            host.input.SetSink(loop as IRawInputSink);
            host.physics.SetContactSink(loop as IContactSink);
            return driver;
        }

        private void OnEnable() {
            Application.lowMemory += OnLowMemory;
        }

        private void OnDisable() {
            Application.lowMemory -= OnLowMemory;
        }

        private void Start() {
            Lifecycle(AppLifecycle.started);
        }

        private void Update() {

            if (host != null) {
                lastSteps = host.Frame(Time.unscaledDeltaTime, loop);
            }
        }

        private void OnApplicationPause(bool paused) {
            Lifecycle(paused ? AppLifecycle.paused : AppLifecycle.resumed);
        }

        private void OnApplicationFocus(bool focused) {
            Lifecycle(focused ? AppLifecycle.focusGained : AppLifecycle.focusLost);
        }

        private void OnApplicationQuit() {
            Lifecycle(AppLifecycle.quitting);
        }

        private void OnLowMemory() {
            Lifecycle(AppLifecycle.lowMemory);
        }

        private void Lifecycle(AppLifecycle e) {

            if (host != null) {
                host.Lifecycle(e, loop);
            }
        }
    }

    // ---- the small adapters ------------------------------------------------------------------

    public class UnityClock : IHostClock {

        public double RealtimeSeconds() {
            return Time.realtimeSinceStartupAsDouble;
        }

        public HostStatus SetEngineTimeScale(float scale) {

            if (float.IsNaN(scale)) {
                return HostStatus.invalidArgument;
            }

            Time.timeScale = scale < 0f ? 0f : scale;
            return HostStatus.ok;
        }
    }

    public class UnityLog : IHostLog {

        // Stripped levels cost one compare: the core checks IsEnabled before it formats.
        public LogLevel minimum = Application.isEditor ? LogLevel.info : LogLevel.warning;

        public bool IsEnabled(LogLevel level) {
            return level >= minimum;
        }

        public void Log(LogLevel level, string category, string message) {

            if (!IsEnabled(level)) {
                return;
            }

            string line = "[" + (category ?? "") + "] " + (message ?? "");

            if (level >= LogLevel.error) {
                Debug.LogError(line);
            }
            else if (level == LogLevel.warning) {
                Debug.LogWarning(line);
            }
            else {
                Debug.Log(line);
            }
        }
    }

    // The house authors UI at 960x640 and scales to fit height (NGUI's UIRoot and the UI Toolkit
    // panels both use a 640-unit-high design space), so designScale = pixelHeight / 640.
    public class UnityDisplay : IHostDisplay {

        public Vec2 designSize = new Vec2(960f, 640f);

        // Screen.dpi is 0 on some desktops; the contract wants a usable number.
        public float fallbackDpi = 96f;

        public DisplayInfo GetDisplay() {

            int w = Screen.width;
            int h = Screen.height;
            Rect safe = Screen.safeArea;

            DisplayInfo d;
            d.pixelWidth = w;
            d.pixelHeight = h;
            d.dpi = Screen.dpi > 0f ? Screen.dpi : fallbackDpi;

            // Unity's safe area is bottom-left origin; the contract's is top-left.
            d.safeArea = new RectF(safe.x, h - safe.y - safe.height, safe.width, safe.height);
            d.designSize = designSize;
            d.designScale = designSize.y > 0f ? h / designSize.y : 1f;
            return d;
        }
    }

    public class UnityPlatform : IHostPlatform {

        // SystemLanguage is the only locale source that works on every Unity platform, but it has
        // no region. The .NET culture carries one where the runtime reports it (pt-BR), and is
        // used only when it agrees with SystemLanguage on the language.
        public string SystemLocale() {

            string tag = TagOf(Application.systemLanguage);
            string culture = null;

            try {
                culture = CultureInfo.CurrentUICulture.Name;
            }
            catch (Exception) {
            }

            if (!string.IsNullOrEmpty(culture)
                && tag != "zh-Hans" && tag != "zh-Hant"
                && culture.StartsWith(tag + "-", StringComparison.OrdinalIgnoreCase)) {
                return culture;
            }

            return tag;
        }

        public HostStatus Vibrate(string pattern) {

#if UNITY_IOS || UNITY_ANDROID
            if (Application.isEditor) {
                return HostStatus.unsupported;
            }

            // The house facade honours the player's vibrate setting; the pattern is advisory
            // until a haptics backend with more than one pattern exists.
            DeviceUtil.Vibrate();
            return HostStatus.ok;
#else
            return HostStatus.unsupported;
#endif
        }

        public HostStatus OpenUrl(string url) {

            if (string.IsNullOrEmpty(url)) {
                return HostStatus.invalidArgument;
            }

            Application.OpenURL(url);
            return HostStatus.ok;
        }

        // No native share sheet in the house libs yet.
        public HostStatus Share(string text, string url) {
            return HostStatus.unsupported;
        }

        private static readonly Dictionary<SystemLanguage, string> tags = new Dictionary<SystemLanguage, string> {
            { SystemLanguage.English, "en" },
            { SystemLanguage.Spanish, "es" },
            { SystemLanguage.German, "de" },
            { SystemLanguage.French, "fr" },
            { SystemLanguage.Italian, "it" },
            { SystemLanguage.Dutch, "nl" },
            { SystemLanguage.Polish, "pl" },
            { SystemLanguage.Portuguese, "pt" },
            { SystemLanguage.Russian, "ru" },
            { SystemLanguage.Ukrainian, "uk" },
            { SystemLanguage.Japanese, "ja" },
            { SystemLanguage.Korean, "ko" },
            { SystemLanguage.Chinese, "zh-Hans" },
            { SystemLanguage.ChineseSimplified, "zh-Hans" },
            { SystemLanguage.ChineseTraditional, "zh-Hant" }
        };

        // Languages outside the shared 15 are passed through by English name's ISO code where
        // .NET knows it, so the core's registry does the fallback, not the host.
        public static string TagOf(SystemLanguage lang) {

            string tag;

            if (tags.TryGetValue(lang, out tag)) {
                return tag;
            }

            foreach (CultureInfo c in CultureInfo.GetCultures(CultureTypes.NeutralCultures)) {
                if (c.EnglishName == lang.ToString()) {
                    return c.TwoLetterISOLanguageName;
                }
            }

            return "en";
        }
    }
}
