using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

using Engine.Events;
using Engine.Utility;

namespace Engine.Game.App.BaseApp {

    // Runtime localization service for the UI Toolkit path. Builds a per-locale string cache
    // once per language change -- {{^key}} self-references resolved a single time at load, never
    // per Tr() call -- and notifies listeners on change.
    //
    // #if USE_GAME_LIB_GAMES gates every profile/legacy-data touch, the same convention
    // BaseGameLocalizations already uses for Locos/LocoKeys. Outside that define Tr() still
    // works (falls through every lookup to the key itself), matching GetString's existing
    // "never throw" contract.
    public static class GameLocalizationService {

        public const string PrefKeyAppLanguage = "app_language";

        // Fires after SetLanguage/ResyncFromProfile actually applies a new locale. NOT fired
        // by Init() -- nothing has rendered yet, so there is nothing to re-localize.
        public static event Action<string> LanguageChanged;

        private static bool _initialized;
        private static string _currentCode = "en";
        private static List<string> _chain = new List<string> { "en" };

        private static Dictionary<string, Dictionary<string, string>> _cache =
            new Dictionary<string, Dictionary<string, string>>();

        private static readonly HashSet<string> _loggedMissing = new HashSet<string>();

        private static readonly Regex markerRegex =
            new Regex(@"\{\{[ ]*\^[ ]*(.*?)[ ]*\}\}", RegexOptions.Compiled);

        // The resolved locale currently active (never "", always a registry code).
        public static string CurrentCode {
            get { return _currentCode; }
        }

        // The raw saved preference -- "" means "follow the device". Not the same as
        // CurrentCode, which is always resolved to a concrete locale.
        public static string SavedLanguage {
            get { return ReadSaved(); }
        }

        // Idempotent. Safe to call multiple times (each boot path that might run first calls
        // it) -- only the first call does anything.
        public static void Init() {

            if (_initialized) {
                return;
            }

            _initialized = true;

            Apply(ReadSaved(), false);
        }

        public static void SetLanguage(string codeOrEmpty) {

            string val = codeOrEmpty ?? "";

#if USE_GAME_LIB_GAMES
            if (GameProfiles.Current != null) {
                GameProfiles.Current.SetAppLanguage(val);
                GameState.SaveProfile();
            }
#endif
            SystemPrefUtil.SetLocalSettingString(PrefKeyAppLanguage, val);
            SystemPrefUtil.Save();

            Apply(val, true);
        }

        // The profile lazily auto-vivifies blank on first touch, so a Init() that ran before the
        // real profile finished loading cannot tell "no saved value" from "profile not loaded
        // yet" -- it just sees no attribute either way and falls back to the pref mirror. Call
        // this once the profile has actually loaded (after GameState.Instance / InitState) to
        // reconcile: if the profile's value disagrees with the mirror Init() used, the profile
        // wins and gets re-applied.
        public static void ResyncFromProfile() {
#if USE_GAME_LIB_GAMES
            if (GameProfiles.Current == null
                    || !GameProfiles.Current.CheckIfAttributeExists(
                        BaseGameProfileAttributes.ATT_APP_LANGUAGE)) {
                return;
            }

            string profileVal = GameProfiles.Current.GetAppLanguage();
            string mirror = SystemPrefUtil.GetLocalSettingString(PrefKeyAppLanguage);

            if (profileVal == mirror) {
                return;
            }

            SystemPrefUtil.SetLocalSettingString(PrefKeyAppLanguage, profileVal);
            SystemPrefUtil.Save();

            Apply(profileVal, true);
#endif
        }

        private static string ReadSaved() {
#if USE_GAME_LIB_GAMES
            if (GameProfiles.Current != null
                    && GameProfiles.Current.CheckIfAttributeExists(
                        BaseGameProfileAttributes.ATT_APP_LANGUAGE)) {
                return GameProfiles.Current.GetAppLanguage();
            }
#endif
            return SystemPrefUtil.GetLocalSettingString(PrefKeyAppLanguage);
        }

        private static void Apply(string codeOrEmpty, bool notify) {

            GameLocaleInfo resolved = string.IsNullOrEmpty(codeOrEmpty)
                ? GameLocales.ResolveSystemLocale()
                : (GameLocales.Get(codeOrEmpty) ?? GameLocales.Get("en"));

            string code = resolved != null ? resolved.code : "en";

            BuildChain(code);

            _currentCode = code;

            if (notify) {
                Messenger<string>.Broadcast(GameLocalizationMessages.gameLocalizationChanged, code);
                LanguageChanged?.Invoke(code);
            }
        }

        // Loads target + its fallback chain + en (deduped), snapshots each locale's RAW strings
        // once, resolves every {{^key}} marker once, and caches the resolved dictionaries. Tr()
        // then only ever does dictionary lookups over the precomputed _chain -- no regex, no
        // fallback-walk allocation, at call time.
        private static void BuildChain(string targetCode) {

            List<string> chain = new List<string>();
            HashSet<string> visited = new HashSet<string>();
            string code = targetCode;

            while (!string.IsNullOrEmpty(code) && visited.Add(code)) {

                chain.Add(code);

                GameLocaleInfo info = GameLocales.Get(code);
                code = info != null ? info.fallback : null;
            }

            if (!chain.Contains("en")) {
                chain.Add("en");
            }

            Dictionary<string, Dictionary<string, string>> cache =
                new Dictionary<string, Dictionary<string, string>>();

            string targetFileCode = targetCode;

            for (int i = 0; i < chain.Count; i++) {

                string c = chain[i];

                if (cache.ContainsKey(c)) {
                    continue;
                }

                string fileCodeUsed;
                Dictionary<string, string> raw = SnapshotLocale(c, out fileCodeUsed);

                cache[c] = ResolveAll(raw);

                if (c == targetCode) {
                    targetFileCode = fileCodeUsed;
                }
            }

            _chain = chain;
            _cache = cache;

#if USE_GAME_LIB_GAMES
            // Leave the legacy singleton pointing at the SAME locale L10n.Tr just resolved, so
            // existing Locos.GetString callers keep seeing the target language, not whichever
            // fallback/en happened to load last while building the chain above.
            Locos.Instance.ChangeCurrent(targetFileCode);
#endif
        }

        // A registry code (e.g. "es") does not always name the actual data file on disk today
        // -- Spanish still ships as game-localization-data-sp.json.txt, the alias. Try the
        // canonical code first (so this keeps working once a real -es file exists), then each
        // alias.
        private static Dictionary<string, string> SnapshotLocale(string code, out string fileCodeUsed) {

            fileCodeUsed = code;

#if USE_GAME_LIB_GAMES
            Dictionary<string, string> raw = SnapshotLocaleFile(code);

            if (raw.Count == 0) {

                GameLocaleInfo info = GameLocales.Get(code);
                List<string> aliases = info != null ? info.aliases : null;

                if (aliases != null) {
                    for (int i = 0; i < aliases.Count; i++) {

                        Dictionary<string, string> aliasRaw = SnapshotLocaleFile(aliases[i]);

                        if (aliasRaw.Count > 0) {
                            fileCodeUsed = aliases[i];
                            return aliasRaw;
                        }
                    }
                }
            }

            return raw;
#else
            return new Dictionary<string, string>();
#endif
        }

#if USE_GAME_LIB_GAMES
        // A missing locale file must not throw: LoadLocale/Reset already degrades to an empty
        // items list, and GetById then simply fails to find fileCode -- raw comes back empty
        // rather than silently reusing whatever locale happened to load previously.
        private static Dictionary<string, string> SnapshotLocaleFile(string fileCode) {

            Dictionary<string, string> raw = new Dictionary<string, string>();

            Locos.Instance.ChangeCurrent(fileCode);

            GameLocalization loc = GameLocalizations.Instance.GetById(fileCode);

            if (loc != null && loc.data != null && loc.data.strings != null) {

                foreach (KeyValuePair<string, GameLocalizationDataItem> kv in loc.data.strings) {
                    raw[kv.Key] = kv.Value != null ? kv.Value.valString : null;
                }
            }

            return raw;
        }
#endif

        // Resolves every {{^key}} self-reference ONCE, at load. Memoized + cycle-guarded so a
        // reference chain (a -> b -> a) degrades to "leave the marker literal" instead of
        // recursing forever -- same intent as the old per-call ReplaceLocalized, without its
        // regex-every-GetString cost.
        private static Dictionary<string, string> ResolveAll(Dictionary<string, string> raw) {

            Dictionary<string, string> resolved = new Dictionary<string, string>(raw.Count);
            Dictionary<string, string> memo = new Dictionary<string, string>();

            foreach (KeyValuePair<string, string> kv in raw) {

                // An empty value is an untranslated slot, not a translation of "" -- leave it
                // out so Tr() falls through to the next locale in the chain.
                if (string.IsNullOrEmpty(kv.Value)) {
                    continue;
                }

                resolved[kv.Key] = ResolveValue(kv.Key, raw, memo, new HashSet<string>());
            }

            return resolved;
        }

        private static string ResolveValue(
                string key, Dictionary<string, string> raw,
                Dictionary<string, string> memo, HashSet<string> visiting) {

            string cached;

            if (memo.TryGetValue(key, out cached)) {
                return cached;
            }

            string content;

            if (!raw.TryGetValue(key, out content) || string.IsNullOrEmpty(content)) {
                return content ?? "";
            }

            if (!content.Contains("{{") || visiting.Contains(key)) {
                memo[key] = content;
                return content;
            }

            visiting.Add(key);

            string result = markerRegex.Replace(content, m => {

                string refKey = m.Groups[1].Value;

                if (refKey == key) {
                    return m.Value;
                }

                return ResolveValue(refKey, raw, memo, visiting);
            });

            visiting.Remove(key);
            memo[key] = result;

            return result;
        }

        // LOOKUP
        //
        // Hot path is a plain dictionary lookup over a precomputed chain -- zero allocation,
        // and the fallback walk itself was already done once, in BuildChain.

        public static string Tr(string key) {

            if (string.IsNullOrEmpty(key)) {
                return key;
            }

            for (int i = 0; i < _chain.Count; i++) {

                Dictionary<string, string> dict;
                string val;

                if (_cache.TryGetValue(_chain[i], out dict) && dict.TryGetValue(key, out val)) {
                    return val;
                }
            }

            LogMissingOnce(key);

            return key;
        }

        // True when any locale in the active chain (target, fallbacks, en) defines the key.
        public static bool Has(string key) {

            if (string.IsNullOrEmpty(key)) {
                return false;
            }

            for (int i = 0; i < _chain.Count; i++) {

                Dictionary<string, string> dict;

                if (_cache.TryGetValue(_chain[i], out dict) && dict.ContainsKey(key)) {
                    return true;
                }
            }

            return false;
        }

        // For shared-lib call sites whose consumers may not ship the key: the localized value when
        // the key exists, the caller's own (usually English) text when it doesn't -- never the raw
        // key, and no missing-key warning.
        public static string TrOrDefault(string key, string defaultValue) {
            return Has(key) ? Tr(key) : defaultValue;
        }

        // Formatted variant of the above, for shared-lib sites that interpolate. Tr(key, args)
        // would put the RAW KEY on screen in a game that doesn't ship the key; this falls back to
        // the caller's own English format string and formats that instead.
        public static string TrOrDefault(string key, string defaultFormat, params object[] args) {

            string format = Has(key) ? Tr(key) : defaultFormat;

            if (args == null || args.Length == 0) {
                return format;
            }

            try {
                return string.Format(FormatCulture(), format, args);
            }
            catch (Exception) {
                return format;
            }
        }

        public static string Tr(string key, params object[] args) {

            string format = Tr(key);

            if (args == null || args.Length == 0) {
                return format;
            }

            try {
                return string.Format(FormatCulture(), format, args);
            }
            catch (Exception) {
                return format;
            }
        }

        // Number formatting for the active locale: `value.ToString("N0", L10n.NumberFormat)` gives
        // de "6.495.621" where a bare ToString("N0") uses the thread culture ("6,495,621").
        //
        // Cached per locale, since counters format every frame while they tween. Group separators
        // that are narrow/thin no-break spaces (U+202F, U+2009; e.g. fr) become U+00A0: those glyphs
        // are missing from the game's Latin fonts, and U+00A0 still never wraps.
        private static string _numberFormatCode;
        private static NumberFormatInfo _numberFormat;

        public static NumberFormatInfo NumberFormat {
            get {
                if (_numberFormat == null || _numberFormatCode != _currentCode) {
                    _numberFormat = BuildNumberFormat(FormatCulture());
                    _numberFormatCode = _currentCode;
                }
                return _numberFormat;
            }
        }

        public static NumberFormatInfo BuildNumberFormat(CultureInfo culture) {

            NumberFormatInfo format =
                (NumberFormatInfo)(culture ?? CultureInfo.InvariantCulture).NumberFormat.Clone();

            format.NumberGroupSeparator = FontSafeSeparator(format.NumberGroupSeparator);
            format.CurrencyGroupSeparator = FontSafeSeparator(format.CurrencyGroupSeparator);

            return NumberFormatInfo.ReadOnly(format);
        }

        private static string FontSafeSeparator(string separator) {

            if (separator == " " || separator == " ") {
                return " ";
            }
            return separator;
        }

        private static CultureInfo FormatCulture() {

            GameLocaleInfo info = GameLocales.Get(_currentCode);

            if (info == null || string.IsNullOrEmpty(info.bcp47)) {
                return CultureInfo.InvariantCulture;
            }

            try {
                return CultureInfo.GetCultureInfo(info.bcp47);
            }
            catch (Exception) {
                return CultureInfo.InvariantCulture;
            }
        }

        private static void LogMissingOnce(string key) {

            string logKey = _currentCode + "::" + key;

            if (_loggedMissing.Add(logKey)) {
                LogUtil.LogWarning(
                    "L10n: missing key '" + key + "' for locale '" + _currentCode + "'");
            }
        }

        // TEST SEAM
        //
        // Pure-logic injection point: EditMode tests build a chain + cache directly (no
        // Resources, no profile, no games lib) and assert Tr()'s fallback walk and missing-key
        // behavior against it.
        public static void SetStateForTests(
                string currentCode, List<string> chain,
                Dictionary<string, Dictionary<string, string>> cache) {

            _currentCode = currentCode ?? "en";
            _chain = chain ?? new List<string> { "en" };
            _cache = cache ?? new Dictionary<string, Dictionary<string, string>>();
            _loggedMissing.Clear();
        }
    }

    // Short alias -- Tr(key) reads like the localization standard's call-site convention.
    // Delegates to GameLocalizationService; carries no state of its own.
    public static class L10n {

        public static string CurrentCode {
            get { return GameLocalizationService.CurrentCode; }
        }

        public static string Tr(string key) {
            return GameLocalizationService.Tr(key);
        }

        public static string Tr(string key, params object[] args) {
            return GameLocalizationService.Tr(key, args);
        }

        public static bool Has(string key) {
            return GameLocalizationService.Has(key);
        }

        public static string TrOrDefault(string key, string defaultValue) {
            return GameLocalizationService.TrOrDefault(key, defaultValue);
        }

        public static string TrOrDefault(string key, string defaultFormat, params object[] args) {
            return GameLocalizationService.TrOrDefault(key, defaultFormat, args);
        }

        public static System.Globalization.NumberFormatInfo NumberFormat {
            get { return GameLocalizationService.NumberFormat; }
        }
    }
}
