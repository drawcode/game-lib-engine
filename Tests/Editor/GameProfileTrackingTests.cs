using System.Collections.Generic;
using NUnit.Framework;

using Engine.Game.App.BaseApp;

namespace Engine.Game.App.Tests {

    // GameProfileTracking: a flat, portable JSON blob of "<group>:<code>" markers. What matters is
    // that a marker survives the JSON round trip the save takes (primitives come back as bool,
    // long or double, never the type written), that groups don't bleed into each other, and that
    // the file is the plain object a person can read.
    public class GameProfileTrackingTests {

        static GameProfileTracking RoundTrip(GameProfileTracking tracking) {
            return tracking.ToJson().FromJson<GameProfileTracking>();
        }

        [Test]
        public void FlagsStartUnsetAndAreScopedByGroup() {
            GameProfileTracking tracking = new GameProfileTracking();

            Assert.IsFalse(tracking.IsSet("tutorial", "tutorial-attack"));

            tracking.Mark("tutorial", "tutorial-attack");

            Assert.IsTrue(tracking.IsSet("tutorial", "tutorial-attack"));
            Assert.IsFalse(tracking.IsSet("tutorial", "tutorial-build"));
            Assert.IsFalse(tracking.IsSet("hint", "tutorial-attack"));
        }

        [Test]
        public void MarkersSurviveTheJsonRoundTrip() {
            GameProfileTracking tracking = new GameProfileTracking();
            tracking.Mark("tutorial", "tutorial-repair");
            tracking.SetNumber("hint", "joystick", 3);
            tracking.SetString("intro", "last-world", "world-planet-z");

            GameProfileTracking loaded = RoundTrip(tracking);

            Assert.IsTrue(loaded.IsSet("tutorial", "tutorial-repair"));
            Assert.AreEqual(3, loaded.GetNumber("hint", "joystick"));
            Assert.AreEqual("world-planet-z", loaded.GetString("intro", "last-world"));
        }

        [Test]
        public void TheFileIsAFlatObjectOfMarkers() {
            GameProfileTracking tracking = new GameProfileTracking();
            tracking.Mark("tutorial", "tutorial-attack");

            string json = tracking.ToJson().Replace(" ", "").Replace("\n", "").Replace("\r", "");

            StringAssert.Contains("\"tutorial:tutorial-attack\":true", json);
        }

        [Test]
        public void FlagsReadTolerantlyFromHandEditedOrOlderFiles() {
            GameProfileTracking tracking =
                "{\"a:text\":\"true\",\"a:one\":1,\"a:zero\":0,\"a:false\":false,\"a:word\":\"no\"}"
                .FromJson<GameProfileTracking>();

            Assert.IsTrue(tracking.IsSet("a", "text"));
            Assert.IsTrue(tracking.IsSet("a", "one"));
            Assert.IsFalse(tracking.IsSet("a", "zero"));
            Assert.IsFalse(tracking.IsSet("a", "false"));
            Assert.IsFalse(tracking.IsSet("a", "word"));
        }

        [Test]
        public void AddCountsFromZeroAndClearRemoves() {
            GameProfileTracking tracking = new GameProfileTracking();

            Assert.AreEqual(1, tracking.Add("hint", "joystick"));
            Assert.AreEqual(3, tracking.Add("hint", "joystick", 2));

            tracking.Clear("hint", "joystick");

            Assert.AreEqual(0, tracking.GetNumber("hint", "joystick"));
            Assert.AreEqual(7, tracking.GetNumber("hint", "joystick", 7));
        }

        [Test]
        public void CodesListsOnlyTheGroup() {
            GameProfileTracking tracking = new GameProfileTracking();
            tracking.Mark("tutorial", "tutorial-attack");
            tracking.Mark("tutorial", "tutorial-build");
            tracking.Mark("tutorials", "not-this-one");
            tracking.SetNumber("hint", "joystick", 1);

            List<string> codes = RoundTrip(tracking).Codes("tutorial");
            codes.Sort();

            CollectionAssert.AreEqual(new List<string> { "tutorial-attack", "tutorial-build" }, codes);
        }

        // ITEMS + SIDECARS

        class MemorySidecars : IProfileSidecarStore {
            public readonly Dictionary<string, string> files = new Dictionary<string, string>();
            public string Load(string name) {
                string json;
                return files.TryGetValue(name, out json) ? json : null;
            }
            public void Save(string name, object document) {
                files[name] = document.ToJson();
            }
            public void Delete(string name) {
                files.Remove(name);
            }
        }

        static Dictionary<string, object> Stages(int count) {
            Dictionary<string, object> stages = new Dictionary<string, object>();
            for (int i = 0; i < count; i++) {
                stages["arcade-battle-world-" + i] = i % 10;
            }
            return stages;
        }

        int threshold;

        [SetUp]
        public void KeepThreshold() {
            threshold = GameProfileTracking.sidecarThreshold;
        }

        [TearDown]
        public void RestoreThreshold() {
            GameProfileTracking.sidecarThreshold = threshold;
        }

        [Test]
        public void ASmallItemIsInlineWithItsTypeTag() {
            GameProfileTracking tracking = new GameProfileTracking();
            tracking.UseSidecarStore(new MemorySidecars());
            tracking.SetItem("arcade", "stages", "arcade-stages", Stages(3));

            GameProfileTracking loaded = RoundTrip(tracking);

            Assert.IsFalse(loaded.IsSidecar("arcade", "stages"));
            Assert.AreEqual("arcade-stages", loaded.ItemType("arcade", "stages"));
            IDictionary<string, object> stages =
                loaded.GetItem("arcade", "stages", "arcade-stages") as IDictionary<string, object>;
            Assert.AreEqual(3, stages.Count);
            Assert.AreEqual(2, System.Convert.ToInt32(stages["arcade-battle-world-2"]));
        }

        [Test]
        public void ATypedReadOfAnotherTypeGetsNothing() {
            GameProfileTracking tracking = new GameProfileTracking();
            tracking.SetItem("arcade", "stages", "arcade-stages", Stages(1));

            Assert.IsNull(tracking.GetItem("arcade", "stages", "something-else"));
            Assert.IsNotNull(tracking.GetItem("arcade", "stages"));
        }

        [Test]
        public void ABigItemMovesToATaggedSidecarAndReadsBackAfterALoad() {
            GameProfileTracking.sidecarThreshold = 64;
            MemorySidecars files = new MemorySidecars();
            GameProfileTracking tracking = new GameProfileTracking();
            tracking.UseSidecarStore(files);

            tracking.SetItem("arcade", "stages", "arcade-stages", Stages(20));
            tracking.SaveSidecars();

            string sidecar = GameProfileTracking.SidecarName("arcade", "stages");
            Assert.AreEqual("arcade--stages", sidecar);
            Assert.IsTrue(files.files.ContainsKey(sidecar));

            // the sidecar names its type and key; the blob keeps only the reference
            Dictionary<string, object> doc = files.files[sidecar].FromJson<Dictionary<string, object>>();
            Assert.AreEqual("arcade-stages", doc[GameProfileTracking.typeField]);
            Assert.AreEqual("arcade:stages", doc[GameProfileTracking.keyField]);
            string blob = tracking.ToJson();
            StringAssert.DoesNotContain("arcade-battle-world-7", blob);
            StringAssert.Contains(GameProfileTracking.sidecarField, blob);

            GameProfileTracking loaded = RoundTrip(tracking);
            loaded.UseSidecarStore(files);

            Assert.IsTrue(loaded.IsSidecar("arcade", "stages"));
            IDictionary<string, object> stages =
                loaded.GetItem<Dictionary<string, object>>("arcade", "stages", "arcade-stages");
            Assert.AreEqual(20, stages.Count);
            Assert.AreEqual(7, System.Convert.ToInt32(stages["arcade-battle-world-7"]));
        }

        [Test]
        public void AShrunkItemComesBackInlineAndItsSidecarIsDeleted() {
            GameProfileTracking.sidecarThreshold = 64;
            MemorySidecars files = new MemorySidecars();
            GameProfileTracking tracking = new GameProfileTracking();
            tracking.UseSidecarStore(files);

            tracking.SetItem("arcade", "stages", "arcade-stages", Stages(20));
            tracking.SaveSidecars();
            tracking.SetItem("arcade", "stages", "arcade-stages", Stages(1));
            tracking.SaveSidecars();

            Assert.IsFalse(tracking.IsSidecar("arcade", "stages"));
            Assert.AreEqual(0, files.files.Count);
            Assert.IsNotNull(tracking.GetItem("arcade", "stages", "arcade-stages"));
        }

        [Test]
        public void ASidecarTaggedForAnotherTypeOrKeyReadsNothing() {
            GameProfileTracking.sidecarThreshold = 64;
            MemorySidecars files = new MemorySidecars();
            GameProfileTracking tracking = new GameProfileTracking();
            tracking.UseSidecarStore(files);
            tracking.SetItem("arcade", "stages", "arcade-stages", Stages(20));
            tracking.SaveSidecars();

            string name = GameProfileTracking.SidecarName("arcade", "stages");
            files.files[name] = files.files[name].Replace("\"arcade-stages\"", "\"other-type\"");

            GameProfileTracking loaded = RoundTrip(tracking);
            loaded.UseSidecarStore(files);

            Assert.IsNull(loaded.GetItem("arcade", "stages", "arcade-stages"));
        }

        [Test]
        public void WithoutAStoreABigItemStaysInline() {
            GameProfileTracking.sidecarThreshold = 64;
            GameProfileTracking tracking = new GameProfileTracking();

            tracking.SetItem("arcade", "stages", "arcade-stages", Stages(20));

            Assert.IsFalse(tracking.IsSidecar("arcade", "stages"));
            Assert.AreEqual(20, ((IDictionary<string, object>)RoundTrip(tracking)
                .GetItem("arcade", "stages", "arcade-stages")).Count);
        }

        [Test]
        public void ClearingASidecarItemDeletesItsFileAtTheNextSave() {
            GameProfileTracking.sidecarThreshold = 64;
            MemorySidecars files = new MemorySidecars();
            GameProfileTracking tracking = new GameProfileTracking();
            tracking.UseSidecarStore(files);
            tracking.SetItem("arcade", "stages", "arcade-stages", Stages(20));
            tracking.SaveSidecars();

            tracking.Clear("arcade", "stages");
            tracking.SaveSidecars();

            Assert.IsFalse(tracking.HasItem("arcade", "stages"));
            Assert.AreEqual(0, files.files.Count);
        }

        // HELP moved from profile attributes; reads fall back to them

        [Test]
        public void HelpReadsTheOldAttributeUntilTrackingHoldsAValue() {
            GameProfileTracking saved = GameProfileTracking.Current;
            try {
                GameProfileTracking.Current = new GameProfileTracking();
                BaseGameProfile profile = new BaseGameProfile();

                profile.SetAttributeBoolValue(BaseGameProfileAttributes.ATT_UI_HAS_SEEN_HELP, true);
                profile.SetAttributeIntValue(BaseGameProfileAttributes.ATT_HELP_TIPS_SHOWN_DAY, 4);
                Assert.IsTrue(profile.GetHasSeenHelp());
                Assert.AreEqual(4, profile.GetHelpTipsShownDay());

                profile.SetHasSeenHelp(false);
                profile.SetHelpTipsShownDay(9);

                Assert.IsFalse(profile.GetHasSeenHelp());
                Assert.AreEqual(9, profile.GetHelpTipsShownDay());
                Assert.IsTrue(GameProfileTracking.Current.Has("help", "seen"));
            }
            finally {
                GameProfileTracking.Current = saved;
            }
        }
    }
}
