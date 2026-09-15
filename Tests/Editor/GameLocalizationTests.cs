using System.Collections.Generic;

using NUnit.Framework;

using Engine.Game.App.BaseApp;

namespace Engine.Game.App.BaseApp.Tests {

    // Pure-logic tests for the runtime localization core: the locale registry (GameLocales) and
    // the Tr() fallback/format behavior (GameLocalizationService). No Resources, no profile, no
    // games lib -- both classes have a test seam (GameLocales.SetAllForTests /
    // GameLocalizationService.SetStateForTests) for exactly this.
    public class GameLocalizationTests {

        // Verbatim contexts/base/localization/locales.json -- the same content copied into
        // Resources as game-localization-locales-data.json.txt. Embedded rather than read from
        // disk so this test has no dependency on repo layout.
        private const string RegistryJson = @"{
  ""version"": 1,
  ""source"": ""en"",
  ""system_default_key"": ""lang_system_default"",
  ""notes"": ""Single source of truth for supported locales across every drawk/drawlabs/drawcode game and app. Order here is picker order. `code` is the in-product code (file suffix, saved setting). `fallback` is the next locale tried for a missing key before `source`. `script` picks the font group. Consumers copy or generate from this file; never hand-edit a product's own list."",
  ""locales"": [
    { ""code"": ""en"",      ""native"": ""English"",              ""english"": ""English"",               ""script"": ""latin"",     ""fallback"": null, ""bcp47"": ""en"",      ""apple"": ""en"",      ""google_play"": ""en-US"", ""store"": ""en-US"",   ""unity_system_language"": [""English""] },
    { ""code"": ""es"",      ""native"": ""Español"",              ""english"": ""Spanish"",               ""script"": ""latin"",     ""fallback"": null, ""bcp47"": ""es"",      ""apple"": ""es"",      ""google_play"": ""es-ES"", ""store"": ""es-ES"",   ""unity_system_language"": [""Spanish""], ""aliases"": [""sp""] },
    { ""code"": ""de"",      ""native"": ""Deutsch"",              ""english"": ""German"",                ""script"": ""latin"",     ""fallback"": null, ""bcp47"": ""de"",      ""apple"": ""de"",      ""google_play"": ""de-DE"", ""store"": ""de-DE"",   ""unity_system_language"": [""German""] },
    { ""code"": ""fr"",      ""native"": ""Français"",             ""english"": ""French"",                ""script"": ""latin"",     ""fallback"": null, ""bcp47"": ""fr"",      ""apple"": ""fr"",      ""google_play"": ""fr-FR"", ""store"": ""fr-FR"",   ""unity_system_language"": [""French""] },
    { ""code"": ""it"",      ""native"": ""Italiano"",             ""english"": ""Italian"",               ""script"": ""latin"",     ""fallback"": null, ""bcp47"": ""it"",      ""apple"": ""it"",      ""google_play"": ""it-IT"", ""store"": ""it-IT"",   ""unity_system_language"": [""Italian""] },
    { ""code"": ""nl"",      ""native"": ""Nederlands"",           ""english"": ""Dutch"",                 ""script"": ""latin"",     ""fallback"": null, ""bcp47"": ""nl"",      ""apple"": ""nl"",      ""google_play"": ""nl-NL"", ""store"": ""nl-NL"",   ""unity_system_language"": [""Dutch""] },
    { ""code"": ""pl"",      ""native"": ""Polski"",               ""english"": ""Polish"",                ""script"": ""latin-ext"", ""fallback"": null, ""bcp47"": ""pl"",      ""apple"": ""pl"",      ""google_play"": ""pl-PL"", ""store"": ""pl-PL"",   ""unity_system_language"": [""Polish""] },
    { ""code"": ""pt"",      ""native"": ""Português (Portugal)"", ""english"": ""Portuguese (Portugal)"", ""script"": ""latin"",     ""fallback"": ""pt-BR"", ""bcp47"": ""pt-PT"", ""apple"": ""pt-PT"", ""google_play"": ""pt-PT"", ""store"": ""pt-PT"", ""unity_system_language"": [], ""culture_match"": [""pt-PT"", ""pt-AO"", ""pt-MZ""] },
    { ""code"": ""pt-BR"",   ""native"": ""Português (Brasil)"",   ""english"": ""Portuguese (Brazil)"",   ""script"": ""latin"",     ""fallback"": null, ""bcp47"": ""pt-BR"",   ""apple"": ""pt-BR"",   ""google_play"": ""pt-BR"", ""store"": ""pt-BR"",   ""unity_system_language"": [""Portuguese""] },
    { ""code"": ""ru"",      ""native"": ""Русский"",              ""english"": ""Russian"",               ""script"": ""cyrillic"",  ""fallback"": null, ""bcp47"": ""ru"",      ""apple"": ""ru"",      ""google_play"": ""ru-RU"", ""store"": ""ru-RU"",   ""unity_system_language"": [""Russian"", ""Belarusian""] },
    { ""code"": ""uk"",      ""native"": ""Українська"",           ""english"": ""Ukrainian"",             ""script"": ""cyrillic"",  ""fallback"": null, ""bcp47"": ""uk"",      ""apple"": ""uk"",      ""google_play"": ""uk"",    ""store"": ""uk"",      ""unity_system_language"": [""Ukrainian""] },
    { ""code"": ""ja"",      ""native"": ""日本語"",                ""english"": ""Japanese"",              ""script"": ""cjk-jp"",    ""fallback"": null, ""bcp47"": ""ja"",      ""apple"": ""ja"",      ""google_play"": ""ja-JP"", ""store"": ""ja-JP"",   ""unity_system_language"": [""Japanese""] },
    { ""code"": ""ko"",      ""native"": ""한국어"",                ""english"": ""Korean"",                ""script"": ""cjk-kr"",    ""fallback"": null, ""bcp47"": ""ko"",      ""apple"": ""ko"",      ""google_play"": ""ko-KR"", ""store"": ""ko-KR"",   ""unity_system_language"": [""Korean""] },
    { ""code"": ""zh-Hans"", ""native"": ""简体中文"",              ""english"": ""Chinese (Simplified)"",  ""script"": ""cjk-sc"",    ""fallback"": null, ""bcp47"": ""zh-Hans"", ""apple"": ""zh-Hans"", ""google_play"": ""zh-CN"", ""store"": ""zh-Hans"", ""unity_system_language"": [""ChineseSimplified"", ""Chinese""] },
    { ""code"": ""zh-Hant"", ""native"": ""繁體中文"",              ""english"": ""Chinese (Traditional)"", ""script"": ""cjk-tc"",    ""fallback"": null, ""bcp47"": ""zh-Hant"", ""apple"": ""zh-Hant"", ""google_play"": ""zh-TW"", ""store"": ""zh-Hant"", ""unity_system_language"": [""ChineseTraditional""] }
  ],
  ""pseudo"": { ""code"": ""qps"", ""native"": ""[Pseudo]"", ""script"": ""latin"", ""dev_only"": true }
}
";

        [TearDown]
        public void TearDown() {

            // Force the next All/Get call in any OTHER test to reload from Resources rather
            // than keep whatever this fixture injected.
            GameLocales.SetAllForTests(null);
            GameLocalizationService.SetStateForTests(null, null, null);
        }

        private static List<GameLocaleInfo> LoadRegistry() {

            List<GameLocaleInfo> locales = GameLocales.Parse(RegistryJson);
            GameLocales.SetAllForTests(locales);

            return locales;
        }

        // --------------------------------------------------------------------
        // REGISTRY PARSE

        [Test]
        public void Parse_Returns15Locales_InRegistryOrder() {

            List<GameLocaleInfo> locales = GameLocales.Parse(RegistryJson);

            string[] expectedOrder = {
                "en", "es", "de", "fr", "it", "nl", "pl",
                "pt", "pt-BR", "ru", "uk", "ja", "ko", "zh-Hans", "zh-Hant"
            };

            Assert.AreEqual(15, locales.Count);

            for (int i = 0; i < expectedOrder.Length; i++) {
                Assert.AreEqual(expectedOrder[i], locales[i].code, "index " + i);
            }
        }

        [Test]
        public void Parse_Pt_CarriesFallbackAndCultureMatch() {

            List<GameLocaleInfo> locales = GameLocales.Parse(RegistryJson);
            GameLocaleInfo pt = locales.Find(l => l.code == "pt");

            Assert.IsNotNull(pt);
            Assert.AreEqual("pt-BR", pt.fallback);
            Assert.Contains("pt-PT", pt.cultureMatch);
            Assert.Contains("pt-AO", pt.cultureMatch);
        }

        // --------------------------------------------------------------------
        // ALIAS

        [Test]
        public void Get_SpAlias_ResolvesToEs() {

            LoadRegistry();

            GameLocaleInfo info = GameLocales.Get("sp");

            Assert.IsNotNull(info);
            Assert.AreEqual("es", info.code);
        }

        [Test]
        public void Get_UnknownCode_ReturnsNull() {

            LoadRegistry();

            Assert.IsNull(GameLocales.Get("xx"));
            Assert.IsFalse(GameLocales.IsSupported("xx"));
            Assert.IsTrue(GameLocales.IsSupported("sp"));
        }

        // --------------------------------------------------------------------
        // SYSTEM MAPPING

        [Test]
        public void ResolveSystemLocale_PortugueseWithoutMatchingCulture_FallsBackToPtBR() {

            LoadRegistry();

            GameLocaleInfo resolved = GameLocales.ResolveSystemLocale("Portuguese", "en-US");

            Assert.AreEqual("pt-BR", resolved.code);
        }

        [Test]
        public void ResolveSystemLocale_PortugueseWithPtPTCulture_ResolvesToPt() {

            LoadRegistry();

            GameLocaleInfo resolved = GameLocales.ResolveSystemLocale("Portuguese", "pt-PT");

            Assert.AreEqual("pt", resolved.code);
        }

        [Test]
        public void ResolveSystemLocale_ChineseMapsToZhHans() {

            LoadRegistry();

            GameLocaleInfo resolved = GameLocales.ResolveSystemLocale("Chinese", "zh-CN");

            Assert.AreEqual("zh-Hans", resolved.code);
        }

        [Test]
        public void ResolveSystemLocale_UkrainianMapsToUk() {

            LoadRegistry();

            GameLocaleInfo resolved = GameLocales.ResolveSystemLocale("Ukrainian", "uk-UA");

            Assert.AreEqual("uk", resolved.code);
        }

        [Test]
        public void ResolveSystemLocale_UnknownLanguage_DefaultsToEn() {

            LoadRegistry();

            GameLocaleInfo resolved = GameLocales.ResolveSystemLocale("Klingon", "en-US");

            Assert.AreEqual("en", resolved.code);
        }

        // --------------------------------------------------------------------
        // FALLBACK CHAIN (GameLocalizationService.Tr)

        [Test]
        public void Tr_MissingInPtAndPtBR_FallsThroughToEn() {

            Dictionary<string, Dictionary<string, string>> cache =
                new Dictionary<string, Dictionary<string, string>> {
                    { "pt", new Dictionary<string, string>() },
                    { "pt-BR", new Dictionary<string, string>() },
                    { "en", new Dictionary<string, string> { { "game_ui_test_key", "Hello" } } },
                };

            GameLocalizationService.SetStateForTests(
                "pt", new List<string> { "pt", "pt-BR", "en" }, cache);

            Assert.AreEqual("Hello", GameLocalizationService.Tr("game_ui_test_key"));
        }

        [Test]
        public void Tr_FoundInNearerLocale_DoesNotFallThrough() {

            Dictionary<string, Dictionary<string, string>> cache =
                new Dictionary<string, Dictionary<string, string>> {
                    { "pt", new Dictionary<string, string> { { "k", "PT VALUE" } } },
                    { "pt-BR", new Dictionary<string, string> { { "k", "PTBR VALUE" } } },
                    { "en", new Dictionary<string, string> { { "k", "EN VALUE" } } },
                };

            GameLocalizationService.SetStateForTests(
                "pt", new List<string> { "pt", "pt-BR", "en" }, cache);

            Assert.AreEqual("PT VALUE", GameLocalizationService.Tr("k"));
        }

        [Test]
        public void Tr_MissingEverywhereInChain_ReturnsKeyItself() {

            Dictionary<string, Dictionary<string, string>> cache =
                new Dictionary<string, Dictionary<string, string>> {
                    { "es", new Dictionary<string, string>() },
                    { "en", new Dictionary<string, string>() },
                };

            GameLocalizationService.SetStateForTests(
                "es", new List<string> { "es", "en" }, cache);

            Assert.AreEqual("unknown_key", GameLocalizationService.Tr("unknown_key"));
        }

        // --------------------------------------------------------------------
        // Tr WITH ARGS

        [Test]
        public void Tr_WithArgs_FormatsPlaceholders() {

            Dictionary<string, Dictionary<string, string>> cache =
                new Dictionary<string, Dictionary<string, string>> {
                    { "en", new Dictionary<string, string> {
                        { "game_ui_test_level", "Level {0}" } } },
                };

            GameLocalizationService.SetStateForTests("en", new List<string> { "en" }, cache);

            Assert.AreEqual("Level 3", GameLocalizationService.Tr("game_ui_test_level", 3));
        }

        [Test]
        public void L10n_Tr_DelegatesToService() {

            Dictionary<string, Dictionary<string, string>> cache =
                new Dictionary<string, Dictionary<string, string>> {
                    { "en", new Dictionary<string, string> { { "k", "V" } } },
                };

            GameLocalizationService.SetStateForTests("en", new List<string> { "en" }, cache);

            Assert.AreEqual("V", L10n.Tr("k"));
            Assert.AreEqual("en", L10n.CurrentCode);
        }
    }
}
