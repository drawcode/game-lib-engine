using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.TextCore.Text;

using Engine.Utility;
using Engine.Game.App.BaseApp;

namespace Engine.UI {

    // Runtime OS-font fallbacks for the UI Toolkit text path. Dimbo-SDF (the display font, set
    // at :root in common.uss) is Latin-1 only -- Polish diacritics, Cyrillic and CJK all need a
    // fallback or they render as tofu (missing-glyph boxes). This is additive: it only ever
    // touches PanelSettings.textSettings.fallbackFontAssets, never Dimbo-SDF.asset itself, and
    // bundles nothing -- every fallback is an OS-installed font, picked at runtime with
    // Font.GetOSInstalledFontNames() so a device that lacks a candidate just skips it.
    //
    // Call Init() once at boot (BaseGameGlobal.InitLocalization, right after
    // GameLocalizationService.Init()). It also registers a LanguageChanged listener: ja/ko/
    // zh-Hans/zh-Hant share Han code points but want different regional glyph shapes, so the
    // active locale's own CJK font is moved to the FRONT of the fallback list on every change
    // (and once at Init, for whatever locale GameLocalizationService already resolved).
    public static class UIToolkitFontFallbacks {

        private static bool _initialized;

        // Candidate family names per script group, most-preferred first -- iOS/macOS system
        // fonts ahead of the Android/Noto names, per the product decision (contexts/games/
        // action-bots/context-game-localization.md: OS fonts preferred, bundle only on a tofu
        // failure). Latin-ext (Polish) and Cyrillic (ru/uk) both read fine off the platform's
        // general-purpose UI font, so they share one group with the Latin-ext font first.
        private static readonly string[] latinExtAndCyrillic = { "Helvetica Neue", "Roboto" };
        private static readonly string[] cjkJP =
            { "Hiragino Sans", "Hiragino Kaku Gothic ProN", "Noto Sans CJK JP" };
        private static readonly string[] cjkKR = { "Apple SD Gothic Neo", "Noto Sans CJK KR" };
        // Hiragino Sans GB ("GuoBiao") is macOS's other built-in Simplified Chinese UI font --
        // listed after PingFang SC because PingFang SC is the platform's modern default, but kept
        // as a second OS candidate: verified in this session that a Unity Editor's font
        // enumeration does not always surface PingFang SC/TC (see the font-fallbacks context),
        // and Hiragino Sans GB reliably does.
        private static readonly string[] cjkSC = { "PingFang SC", "Hiragino Sans GB", "Noto Sans CJK SC" };
        private static readonly string[] cjkTC = { "PingFang TC", "Noto Sans CJK TC" };

        // Per-family style-name candidates tried in order until one produces a real FontAsset.
        // "Regular" covers most OS fonts; several CJK families expose weights only under their
        // OS-internal subfamily name instead (Hiragino Sans's actual style names are W0..W9, not
        // "Regular" -- CreateFontAsset("Hiragino Sans", "Regular", ...) returns null although the
        // family itself is installed, confirmed in-Editor on 6000.5.2f1 / macOS).
        private static readonly string[] styleCandidates =
            { "Regular", "Normal", "W3", "W4", "Medium", "" };

        private static readonly Dictionary<string, FontAsset> _createdByFamily =
            new Dictionary<string, FontAsset>();

        public static bool isInitialized {
            get {
                return _initialized;
            }
        }

        public static void Init() {

            if (_initialized) {
                return;
            }

            _initialized = true;

            BuildFallbacks();

            // Apply the CJK ordering for whatever locale Init() (called just before this, in
            // InitLocalization) already resolved -- the picker must show every native name
            // correctly the FIRST time it opens, not only after a language change.
            OnLanguageChanged(L10n.CurrentCode);

            GameLocalizationService.LanguageChanged += OnLanguageChanged;
        }

        // Test/teardown seam -- EditMode tests and a hot-reload path can force a clean rebuild.
        public static void ResetForTests() {
            _initialized = false;
            _createdByFamily.Clear();
        }

        private static PanelTextSettings ActiveTextSettings() {

            PanelSettings ps = UIToolkitBackend.panelSettings;

            if (ps == null) {
                return null;
            }

            return ps.textSettings as PanelTextSettings;
        }

        private static void BuildFallbacks() {

            PanelTextSettings ts = ActiveTextSettings();

            if (ts == null) {
                LogUtil.LogWarning(
                    "UIToolkitFontFallbacks: no PanelTextSettings on the shared PanelSettings "
                    + "(UIToolkitBackend.panelSettings) -- fallbacks not installed.");
                return;
            }

            HashSet<string> installed = new HashSet<string>(Font.GetOSInstalledFontNames());

            List<FontAsset> list = new List<FontAsset>();

            AddFirstAvailable(list, installed, latinExtAndCyrillic);
            AddFirstAvailable(list, installed, cjkJP);
            AddFirstAvailable(list, installed, cjkKR);
            AddFirstAvailable(list, installed, cjkSC);
            AddFirstAvailable(list, installed, cjkTC);

            ts.fallbackFontAssets = list;
        }

        // Adds the FIRST installed candidate in the group (not all of them) -- one representative
        // font per script is enough; a second installed candidate in the same group would only
        // ever be a lower-priority duplicate of the same coverage.
        private static void AddFirstAvailable(
                List<FontAsset> list, HashSet<string> installed, string[] candidates) {

            for (int i = 0; i < candidates.Length; i++) {

                if (!installed.Contains(candidates[i])) {
                    continue;
                }

                FontAsset fa = GetOrCreate(candidates[i]);

                // A family reporting "installed" can still fail EVERY style-name guess (seen with
                // Hiragino Sans's W-series weights on 6000.5.2f1) -- try the NEXT candidate in the
                // group rather than giving up on the whole script.
                if (fa == null) {
                    continue;
                }

                if (!list.Contains(fa)) {
                    list.Add(fa);
                }

                return;
            }
        }

        private static FontAsset GetOrCreate(string familyName) {

            FontAsset existing;

            if (_createdByFamily.TryGetValue(familyName, out existing)) {
                return existing;
            }

            FontAsset fa = null;

            // The public overload verified present in this build's TextCoreTextEngineModule
            // (6000.5.2f1): CreateFontAsset(string familyName, string styleName, int pointSize).
            // Point size only sizes the initial atlas -- UI Toolkit re-rasterizes per the label's
            // own font-size, same as Dimbo-SDF's dynamic atlas. The style name is NOT always
            // "Regular": Hiragino Sans (installed and listed by Font.GetOSInstalledFontNames)
            // returns null for "Regular" and only succeeds under its OS subfamily name "W3" --
            // confirmed in-Editor -- so every family tries a short list of style guesses before
            // giving up.
            for (int i = 0; i < styleCandidates.Length && fa == null; i++) {

                try {
                    fa = FontAsset.CreateFontAsset(familyName, styleCandidates[i], 64);
                }
                catch (Exception) {
                    // Try the next style guess.
                }
            }

            if (fa == null) {
                LogUtil.LogWarning(
                    "UIToolkitFontFallbacks: CreateFontAsset found no working style for '"
                    + familyName + "' (tried: " + string.Join(", ", styleCandidates) + ")");
            }

            _createdByFamily[familyName] = fa;

            return fa;
        }

        private static void OnLanguageChanged(string code) {

            string[] preferred = PreferredGroupFor(code);

            if (preferred == null) {
                return;
            }

            PanelTextSettings ts = ActiveTextSettings();

            if (ts == null || ts.fallbackFontAssets == null) {
                return;
            }

            HashSet<string> installed = new HashSet<string>(Font.GetOSInstalledFontNames());
            FontAsset target = null;

            for (int i = 0; i < preferred.Length; i++) {

                if (!installed.Contains(preferred[i])) {
                    continue;
                }

                target = GetOrCreate(preferred[i]);

                if (target != null) {
                    break;
                }
            }

            if (target == null) {
                return;
            }

            List<FontAsset> list = ts.fallbackFontAssets;

            list.Remove(target);
            list.Insert(0, target);

            ts.fallbackFontAssets = list;
        }

        private static string[] PreferredGroupFor(string code) {

            if (string.IsNullOrEmpty(code)) {
                return null;
            }

            if (code == "ja") {
                return cjkJP;
            }

            if (code == "ko") {
                return cjkKR;
            }

            if (code == "zh-Hans") {
                return cjkSC;
            }

            if (code == "zh-Hant") {
                return cjkTC;
            }

            return null;
        }
    }
}
