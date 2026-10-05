using NUnit.Framework;

using UnityEngine;
using UnityEngine.TestTools;

using Engine.Animation;
using Engine.UI.Bitty;
using Engine.Utility;

namespace Engine.UI.Bitty.Tests {

    // The point of 2.3 is that the token source becomes the AUTHORITY for named transitions.
    // These tests pin that wiring: a token file edit must move TweenPresets, because that is
    // what makes panel timing data-driven without touching a single call site.
    public class UITokensTests {

        [TearDown]
        public void TearDown() {

            // Restore the legacy-faithful table so a token test can't leak timings into the
            // other suites (TweenPresets is a process-wide static).
            UITokens.Defaults().Seed();
        }

        [Test]
        public void Defaults_MatchTheLegacyTweenUtilTimings() {

            UITokens tokens = UITokens.Defaults();
            tokens.Seed();

            // Exactly TweenUtil's durationShow / durationDelayShow. If these drift, every panel
            // transition in the game silently changes speed.
            TweenPreset show = TweenPresets.Get("panel-show");

            Assert.AreEqual(.45f, show.time, .0001f);
            Assert.AreEqual(.5f, show.delay, .0001f);

            TweenPreset hide = TweenPresets.Get("panel-hide");

            Assert.AreEqual(.45f, hide.time, .0001f);
            Assert.AreEqual(0f, hide.delay, .0001f);
        }

        [Test]
        public void TokenFile_OverridesTweenPresets() {

            string json = "{\"motion\":{\"panel-show\":{\"time\":0.9,\"delay\":0.1,"
                + "\"ease\":\"linear\",\"fade\":false}}}";

            Assert.IsTrue(UITokens.LoadFromJson(json));

            TweenPreset show = TweenPresets.Get("panel-show");

            Assert.AreEqual(.9f, show.time, .0001f);
            Assert.AreEqual(.1f, show.delay, .0001f);
            Assert.AreEqual(TweenEaseType.linear, show.easeType);
            Assert.IsFalse(show.fade);
        }

        [Test]
        public void TokenFile_MergesOntoDefaults_RatherThanReplacingThem() {

            // A file that only overrides one color must not blank the motion table.
            string json = "{\"color\":{\"panel-bg\":\"#112233\"}}";

            Assert.IsTrue(UITokens.LoadFromJson(json));

            Assert.AreEqual(new Color32(0x11, 0x22, 0x33, 0xFF),
                (Color32)UITokens.current.GetColor("panel-bg", Color.white));

            // Untouched motion still present and still legacy-faithful.
            TweenPreset show = TweenPresets.Get("panel-show");
            Assert.AreEqual(.45f, show.time, .0001f);
        }

        [Test]
        public void BadJson_KeepsDefaults_AndDoesNotThrow() {

            // A malformed token file logs an error and keeps the defaults — it must never take
            // the UI down. The logged error is the expected outcome here, not a test failure.
            LogAssert.ignoreFailingMessages = true;

            Assert.IsFalse(UITokens.LoadFromJson("{ not json ["));
            Assert.IsFalse(UITokens.LoadFromJson(""));

            LogAssert.ignoreFailingMessages = false;

            // Defaults survived the bad parse.
            Assert.AreEqual(.45f, TweenPresets.Get("panel-show").time, .0001f);
        }

        [Test]
        public void MissingToken_ReturnsFallback_NeverThrows() {

            UITokens tokens = UITokens.Defaults();

            Assert.AreEqual(Color.magenta, tokens.GetColor("no-such-color", Color.magenta));
            Assert.AreEqual(42f, tokens.GetSize("no-such-size", 42f), .0001f);
            Assert.AreEqual("Fallback", tokens.GetFont("no-such-font", "Fallback"));

            // GetMotion never returns null — same "Get never returns null" policy as TweenPresets.
            Assert.IsNotNull(tokens.GetMotion("no-such-motion"));
        }

        [Test]
        public void UnknownEase_FallsBackToDefault() {
            Assert.AreEqual(TweenEaseType.quadEaseInOut, UITokens.ParseEase("cubicElasticWhatever"));
            Assert.AreEqual(TweenEaseType.linear, UITokens.ParseEase("linear"));
        }

        [Test]
        public void Schema_LocalizationPrefix_RoundTrips() {

            Assert.IsTrue(BittySchema.IsLocalized("@loc:settings.title"));
            Assert.AreEqual("settings.title", BittySchema.LocKey("@loc:settings.title"));

            Assert.IsFalse(BittySchema.IsLocalized("Settings"));
            Assert.AreEqual("Settings", BittySchema.LocKey("Settings"));
        }
    }
}
