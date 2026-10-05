using System;
using System.Collections.Generic;
using System.Globalization;

using UnityEngine;

using Engine.Utility;

using MiniJSON;

namespace Engine.Game.App.BaseApp {

    // One entry in the 15-locale registry (contexts/base/localization/locales.json). Additive,
    // engine-side mirror of that file's shape -- see GameLocales for the loader and lookups.
    public class GameLocaleInfo {

        public string code;
        public string native;
        public string english;
        public string script;
        public string fallback;
        public string bcp47;
        public List<string> unitySystemLanguages = new List<string>();
        public List<string> cultureMatch = new List<string>();
        public List<string> aliases = new List<string>();
    }

    // The locale registry: contexts/base/localization/locales.json, copied verbatim into
    // Resources as game-localization-locales-data.json.txt and parsed once here. Pure data +
    // lookups -- no UnityEngine.UIElements, no profile/pref access -- so EditMode tests can
    // exercise Get/IsSupported/ResolveSystemLocale against a list built directly with Parse()
    // or SetAllForTests(), with no dependency on Resources.
    public static class GameLocales {

        private const string resourceRelativePath = "data/game-localization-locales-data.json";

        private static List<GameLocaleInfo> _all;

        public static List<GameLocaleInfo> All {
            get {
                EnsureLoaded();
                return _all;
            }
        }

        // Test seam: inject a known registry so tests don't depend on Resources being present
        // or on the games lib being compiled in. Passing null resets to "not loaded" so the
        // next All/Get call reloads from Resources again.
        public static void SetAllForTests(List<GameLocaleInfo> locales) {
            _all = locales;
        }

        private static void EnsureLoaded() {

            if (_all != null) {
                return;
            }

            _all = new List<GameLocaleInfo>();

            TextAsset asset = Resources.Load<TextAsset>(ResourcePath());

            if (asset == null || string.IsNullOrEmpty(asset.text)) {
                LogUtil.LogWarning("GameLocales: no locale registry at " + ResourcePath());
                return;
            }

            ParseInto(_all, asset.text);
        }

        private static string ResourcePath() {
#if USE_GAME_LIB_GAMES
            return ContentsConfig.contentRootFolder + "/" + ContentsConfig.contentAppFolder
                + "/version/" + resourceRelativePath;
#else
            return resourceRelativePath;
#endif
        }

        // Parse without touching Resources -- the EditMode test seam, and also usable by a
        // caller that already has the JSON text in hand.
        public static List<GameLocaleInfo> Parse(string json) {

            List<GameLocaleInfo> result = new List<GameLocaleInfo>();
            ParseInto(result, json);

            return result;
        }

        private static void ParseInto(List<GameLocaleInfo> into, string json) {

            object root;

            try {
                root = Json.Deserialize(json);
            }
            catch (Exception e) {
                LogUtil.LogWarning("GameLocales: failed to parse locale registry: " + e.Message);
                return;
            }

            Dictionary<string, object> rootDict = root as Dictionary<string, object>;

            if (rootDict == null) {
                return;
            }

            object localesObj;

            if (!rootDict.TryGetValue("locales", out localesObj)) {
                return;
            }

            List<object> locales = localesObj as List<object>;

            if (locales == null) {
                return;
            }

            for (int i = 0; i < locales.Count; i++) {

                Dictionary<string, object> item = locales[i] as Dictionary<string, object>;

                if (item == null) {
                    continue;
                }

                GameLocaleInfo info = new GameLocaleInfo();
                info.code = Str(item, "code");
                info.native = Str(item, "native");
                info.english = Str(item, "english");
                info.script = Str(item, "script");
                info.fallback = Str(item, "fallback");
                info.bcp47 = Str(item, "bcp47");
                info.unitySystemLanguages = StrList(item, "unity_system_language");
                info.cultureMatch = StrList(item, "culture_match");
                info.aliases = StrList(item, "aliases");

                if (string.IsNullOrEmpty(info.code)) {
                    continue;
                }

                into.Add(info);
            }
        }

        private static string Str(Dictionary<string, object> dict, string key) {

            object val;

            if (dict.TryGetValue(key, out val) && val != null) {
                return val.ToString();
            }

            return null;
        }

        private static List<string> StrList(Dictionary<string, object> dict, string key) {

            List<string> result = new List<string>();
            object val;

            if (dict.TryGetValue(key, out val)) {

                List<object> list = val as List<object>;

                if (list != null) {
                    for (int i = 0; i < list.Count; i++) {
                        if (list[i] != null) {
                            result.Add(list[i].ToString());
                        }
                    }
                }
            }

            return result;
        }

        // LOOKUPS

        public static GameLocaleInfo Get(string codeOrAlias) {

            if (string.IsNullOrEmpty(codeOrAlias)) {
                return null;
            }

            List<GameLocaleInfo> all = All;

            for (int i = 0; i < all.Count; i++) {
                if (string.Equals(all[i].code, codeOrAlias, StringComparison.OrdinalIgnoreCase)) {
                    return all[i];
                }
            }

            for (int i = 0; i < all.Count; i++) {

                List<string> aliases = all[i].aliases;

                if (aliases == null) {
                    continue;
                }

                for (int j = 0; j < aliases.Count; j++) {
                    if (string.Equals(aliases[j], codeOrAlias, StringComparison.OrdinalIgnoreCase)) {
                        return all[i];
                    }
                }
            }

            return null;
        }

        public static bool IsSupported(string codeOrAlias) {
            return Get(codeOrAlias) != null;
        }

        // Application.systemLanguage -> the best registry locale. Portuguese is the one
        // ambiguous case Unity can't resolve on its own (SystemLanguage.Portuguese covers both
        // pt and pt-BR) -- CultureInfo.CurrentCulture.Name disambiguates it, preferring pt-BR
        // when the culture doesn't say otherwise. Everything else is a direct match against
        // unity_system_language, in registry order (this is also how SystemLanguage.Chinese
        // lands on zh-Hans -- it appears earlier in the registry and lists "Chinese" as an
        // alternate).
        public static GameLocaleInfo ResolveSystemLocale() {
            return ResolveSystemLocale(
                Application.systemLanguage.ToString(), CultureInfo.CurrentCulture.Name);
        }

        // Testable overload -- a pure function of the two inputs, no UnityEngine.Application
        // call, so EditMode tests can drive it directly.
        public static GameLocaleInfo ResolveSystemLocale(string systemLanguage, string cultureName) {

            GameLocaleInfo en = Get("en");

            if (string.Equals(systemLanguage, "Portuguese", StringComparison.OrdinalIgnoreCase)) {

                GameLocaleInfo pt = Get("pt");

                if (pt != null && pt.cultureMatch != null && !string.IsNullOrEmpty(cultureName)
                        && pt.cultureMatch.Contains(cultureName)) {
                    return pt;
                }

                return Get("pt-BR") ?? en;
            }

            List<GameLocaleInfo> all = All;

            for (int i = 0; i < all.Count; i++) {

                List<string> langs = all[i].unitySystemLanguages;

                if (langs == null) {
                    continue;
                }

                for (int j = 0; j < langs.Count; j++) {
                    if (string.Equals(langs[j], systemLanguage, StringComparison.OrdinalIgnoreCase)) {
                        return all[i];
                    }
                }
            }

            return en;
        }
    }
}
