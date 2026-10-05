using NUnit.Framework;

using UnityEngine;

namespace Engine.GameObjects.Tests {

    // Has/Get/GetOrSet search children as well as the object itself, and hundreds of call
    // sites across the shared libs depend on that. These tests pin the DEFAULT as
    // child-inclusive so nobody can flip it later without a red test: doing so would
    // silently change component resolution everywhere at once.
    //
    // They also pin the opt-in strict mode, which exists because a component that is
    // looked up later with a plain GetComponent must live on the exact object it was
    // added to. See BaseGamePlayerCollision, where a hit area's GameDamageManager is
    // found by GetComponent on whichever collider a projectile struck.
    public class GameObjectComponentLookupTests {

        GameObject parent;
        GameObject child;

        [SetUp]
        public void SetUp() {

            parent = new GameObject("lookup-parent");
            child = new GameObject("lookup-child");
            child.transform.parent = parent.transform;

            // Only the CHILD carries the component. Every assertion below turns on
            // whether a lookup on the parent reaches down to it.
            child.AddComponent<BoxCollider>();
        }

        [TearDown]
        public void TearDown() {

            if (parent != null) {
                Object.DestroyImmediate(parent);
            }
        }

        // ------------------------------------------------------------------
        // DEFAULT: child-inclusive (the long-standing behavior)

        [Test]
        public void Has_ByDefault_FindsComponentOnChild() {

            Assert.IsTrue(parent.Has<BoxCollider>());
        }

        [Test]
        public void Get_ByDefault_ReturnsComponentFromChild() {

            BoxCollider found = parent.Get<BoxCollider>();

            Assert.IsNotNull(found);
            Assert.AreSame(child, found.gameObject);
        }

        [Test]
        public void GetOrSet_ByDefault_ReusesChildComponentAndAddsNothing() {

            BoxCollider found = parent.GetOrSet<BoxCollider>();

            Assert.IsNotNull(found);
            Assert.AreSame(child, found.gameObject, "default GetOrSet must reuse the child's component");
            Assert.IsNull(parent.GetComponent<BoxCollider>(), "default GetOrSet must not add to the parent");
        }

        // ------------------------------------------------------------------
        // STRICT: includeChildren = false

        [Test]
        public void Has_Strict_IgnoresComponentOnChild() {

            Assert.IsFalse(parent.Has<BoxCollider>(false));
        }

        [Test]
        public void Get_Strict_ReturnsNullWhenOnlyChildHasIt() {

            Assert.IsNull(parent.Get<BoxCollider>(false));
        }

        [Test]
        public void GetOrSet_Strict_AddsToThisObjectEvenWhenChildHasOne() {

            BoxCollider found = parent.GetOrSet<BoxCollider>(false);

            Assert.IsNotNull(found);
            Assert.AreSame(parent, found.gameObject, "strict GetOrSet must add to this object");
            Assert.IsNotNull(parent.GetComponent<BoxCollider>(),
                "strict GetOrSet must be reachable via a plain GetComponent on this object");
        }

        [Test]
        public void GetOrSet_Strict_ReusesComponentAlreadyOnThisObject() {

            BoxCollider own = parent.AddComponent<BoxCollider>();

            BoxCollider found = parent.GetOrSet<BoxCollider>(false);

            Assert.AreSame(own, found, "strict GetOrSet must not add a second component");
            Assert.AreEqual(1, parent.GetComponents<BoxCollider>().Length);
        }

        // ------------------------------------------------------------------
        // OWN COMPONENT WINS, in both modes

        [Test]
        public void Get_PrefersOwnComponentOverChild() {

            BoxCollider own = parent.AddComponent<BoxCollider>();

            Assert.AreSame(own, parent.Get<BoxCollider>());
            Assert.AreSame(own, parent.Get<BoxCollider>(false));
        }

        [Test]
        public void Get_Strict_StillFindsOwnComponentWhenNoChildHasOne() {

            GameObject lone = new GameObject("lookup-lone");

            try {
                SphereCollider own = lone.AddComponent<SphereCollider>();

                Assert.IsTrue(lone.Has<SphereCollider>(false));
                Assert.AreSame(own, lone.Get<SphereCollider>(false));
            }
            finally {
                Object.DestroyImmediate(lone);
            }
        }
    }
}
