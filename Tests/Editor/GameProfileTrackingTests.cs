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
    }
}
