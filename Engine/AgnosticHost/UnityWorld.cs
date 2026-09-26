using System;
using System.Collections.Generic;

using Agnostic.Core;
using Agnostic.Host;

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Engine.AgnosticHost {

    // U-O: objects and world, over GameObjectHelper.
    //
    // Spawn instantiates for real and Destroy destroys for real. Reuse is the core's job
    // (Agnostic.Core.HandlePool over Spawn/SetVisible), so the house object pool is deliberately
    // NOT used here: a pooled Destroy leaves the GameObject alive, and a host that answers
    // Alive=true for something the core destroyed breaks the contract the null host pins.
    //
    // SetTransform/GetTransform are the LOCAL pose (relative to the parent, or the world at the
    // root), in the canonical frame. That is what Godot's `transform` and the null host store.
    public class UnityWorld : IHostWorld {

        private readonly UnityHandleTable handles;
        private readonly IHostLog log;
        private readonly Dictionary<string, GameObject> prefabs = new Dictionary<string, GameObject>(StringComparer.Ordinal);

        // key -> prefab. Defaults to the house's AssetUtil (Resources today). A title with a
        // manifest swaps this for its own lookup.
        public Func<string, GameObject> resolvePrefab;

        public UnityWorld(UnityHandleTable handles, IHostLog log) {
            this.handles = handles;
            this.log = log;
            resolvePrefab = DefaultResolve;
        }

        private static GameObject DefaultResolve(string key) {
            return AssetUtil.LoadAsset<GameObject>(key);
        }

        public Handle Spawn(string key, Handle parent) {

            if (string.IsNullOrEmpty(key)) {
                return Handle.none;
            }

            GameObject prefab;

            if (!prefabs.TryGetValue(key, out prefab) || prefab == null) {
                prefab = resolvePrefab != null ? resolvePrefab(key) : null;

                if (prefab == null) {
                    Log(LogLevel.warning, "Spawn: no template for key '" + key + "'");
                    return Handle.none;
                }

                prefabs[key] = prefab;
            }

            Transform parentTransform = null;

            if (parent.isSome) {
                GameObject p;

                if (!handles.TryGetGameObject(parent, out p)) {
                    Log(LogLevel.warning, "Spawn: parent " + parent + " is dead");
                    return Handle.none;
                }

                parentTransform = p.transform;
            }

            GameObject go = GameObjectHelper.CreateGameObject(
                key, prefab, Vector3.zero, Quaternion.identity, false);

            if (go == null) {
                return Handle.none;
            }

            if (parentTransform != null) {
                go.transform.SetParent(parentTransform, false);
            }

            return handles.Track(go);
        }

        public HostStatus Destroy(Handle h) {

            GameObject go;

            if (!handles.TryGetGameObject(h, out go)) {
                return HostStatus.deadHandle;
            }

            handles.ReleaseHierarchy(go);
            GameObjectHelper.DestroyGameObject(go, false);
            return HostStatus.ok;
        }

        public bool Alive(Handle h) {
            return handles.Alive(h);
        }

        public Handle Find(Handle root, string path) {

            if (string.IsNullOrEmpty(path)) {
                return Handle.none;
            }

            Transform found = null;

            if (root.isSome) {
                GameObject r;

                if (!handles.TryGetGameObject(root, out r)) {
                    return Handle.none;
                }

                found = r.transform.Find(path.TrimStart('/'));
            }
            else {
                // Scene root: the first segment is a root object, the rest is Transform.Find.
                string trimmed = path.TrimStart('/');
                int slash = trimmed.IndexOf('/');
                string first = slash < 0 ? trimmed : trimmed.Substring(0, slash);
                GameObject top = FindRoot(first);

                if (top != null) {
                    found = slash < 0 ? top.transform : top.transform.Find(trimmed.Substring(slash + 1));
                }
            }

            return found != null ? handles.Track(found.gameObject) : Handle.none;
        }

        private static readonly List<GameObject> rootsBuffer = new List<GameObject>();

        private static GameObject FindRoot(string name) {

            for (int s = 0; s < SceneManager.sceneCount; s++) {
                Scene scene = SceneManager.GetSceneAt(s);

                if (!scene.isLoaded) {
                    continue;
                }

                scene.GetRootGameObjects(rootsBuffer);

                for (int i = 0; i < rootsBuffer.Count; i++) {
                    if (rootsBuffer[i].name == name) {
                        GameObject hit = rootsBuffer[i];
                        rootsBuffer.Clear();
                        return hit;
                    }
                }
            }

            rootsBuffer.Clear();
            return null;
        }

        public HostStatus SetVisible(Handle h, bool visible) {

            GameObject go;

            if (!handles.TryGetGameObject(h, out go)) {
                return HostStatus.deadHandle;
            }

            if (visible) {
                GameObjectHelper.Show(go);
            }
            else {
                GameObjectHelper.Hide(go);
            }

            return HostStatus.ok;
        }

        public HostStatus SetTransform(Handle h, Vec3 position, Quat rotation, Vec3 scale) {

            GameObject go;

            if (!handles.TryGetGameObject(h, out go)) {
                return HostStatus.deadHandle;
            }

            Transform t = go.transform;
            t.SetLocalPositionAndRotation(UnityFrame.ToUnity(position), UnityFrame.ToUnity(rotation));
            t.localScale = UnityFrame.ScaleToUnity(scale);
            return HostStatus.ok;
        }

        public bool GetTransform(Handle h, out Vec3 position, out Quat rotation, out Vec3 scale) {

            GameObject go;

            if (!handles.TryGetGameObject(h, out go)) {
                position = Vec3.zero;
                rotation = Quat.identity;
                scale = Vec3.one;
                return false;
            }

            Transform t = go.transform;
            position = UnityFrame.ToCore(t.localPosition);
            rotation = UnityFrame.ToCore(t.localRotation);
            scale = UnityFrame.ScaleToCore(t.localScale);
            return true;
        }

        public HostStatus Attach(Handle child, Handle parent, bool keepWorld) {

            GameObject c;

            if (!handles.TryGetGameObject(child, out c)) {
                return HostStatus.deadHandle;
            }

            Transform parentTransform = null;

            if (parent.isSome) {
                GameObject p;

                if (!handles.TryGetGameObject(parent, out p)) {
                    return HostStatus.deadHandle;
                }

                if (p.transform.IsChildOf(c.transform)) {
                    return HostStatus.invalidArgument;
                }

                parentTransform = p.transform;
            }

            c.transform.SetParent(parentTransform, keepWorld);
            return HostStatus.ok;
        }

        private void Log(LogLevel level, string message) {

            if (log != null && log.IsEnabled(level)) {
                log.Log(level, "host.world", message);
            }
        }
    }

    // U-M1 apply side. The tween evaluator is core; this only writes a value.
    //
    // Canonical names: position/rotation/scale write the local transform (same as SetTransform),
    // alpha writes a CanvasGroup, then a UI Graphic, then a SpriteRenderer, then the renderer's
    // colour alpha; color writes a UI Graphic, a SpriteRenderer, then the renderer. Renderer
    // writes go through one shared MaterialPropertyBlock, so a colour tween never instantiates a
    // material. Any other name is a material float/colour property on the renderer.
    public class UnityProps : IHostProps {

        private static readonly int colorId = Shader.PropertyToID("_Color");
        private static readonly int baseColorId = Shader.PropertyToID("_BaseColor");

        private readonly UnityHandleTable handles;
        private readonly MaterialPropertyBlock block = new MaterialPropertyBlock();

        public UnityProps(UnityHandleTable handles) {
            this.handles = handles;
        }

        public HostStatus SetFloat(Handle h, string prop, float value) {

            GameObject go;

            if (!handles.TryGetGameObject(h, out go)) {
                return HostStatus.deadHandle;
            }

            if (prop == "alpha") {
                return SetAlpha(go, value);
            }

            Renderer r = go.GetComponent<Renderer>();

            if (r == null || r.sharedMaterial == null || string.IsNullOrEmpty(prop)) {
                return HostStatus.notFound;
            }

            int id = Shader.PropertyToID(prop);

            if (!r.sharedMaterial.HasProperty(id)) {
                return HostStatus.notFound;
            }

            r.GetPropertyBlock(block);
            block.SetFloat(id, value);
            r.SetPropertyBlock(block);
            return HostStatus.ok;
        }

        public HostStatus SetVec3(Handle h, string prop, Vec3 value) {

            GameObject go;

            if (!handles.TryGetGameObject(h, out go)) {
                return HostStatus.deadHandle;
            }

            if (prop == "position") {
                go.transform.localPosition = UnityFrame.ToUnity(value);
                return HostStatus.ok;
            }

            if (prop == "scale") {
                go.transform.localScale = UnityFrame.ScaleToUnity(value);
                return HostStatus.ok;
            }

            return HostStatus.notFound;
        }

        public HostStatus SetQuat(Handle h, string prop, Quat value) {

            GameObject go;

            if (!handles.TryGetGameObject(h, out go)) {
                return HostStatus.deadHandle;
            }

            if (prop == "rotation") {
                go.transform.localRotation = UnityFrame.ToUnity(value);
                return HostStatus.ok;
            }

            return HostStatus.notFound;
        }

        public HostStatus SetColor(Handle h, string prop, ColorRgba value) {

            GameObject go;

            if (!handles.TryGetGameObject(h, out go)) {
                return HostStatus.deadHandle;
            }

            Color c = UnityFrame.ToUnity(value);

            if (prop == "color") {

                Graphic g = go.GetComponent<Graphic>();

                if (g != null) {
                    g.color = c;
                    return HostStatus.ok;
                }

                SpriteRenderer sr = go.GetComponent<SpriteRenderer>();

                if (sr != null) {
                    sr.color = c;
                    return HostStatus.ok;
                }

                return SetRendererColor(go, c, -1);
            }

            return string.IsNullOrEmpty(prop) ? HostStatus.notFound : SetRendererColor(go, c, Shader.PropertyToID(prop));
        }

        private HostStatus SetAlpha(GameObject go, float alpha) {

            CanvasGroup cg = go.GetComponent<CanvasGroup>();

            if (cg != null) {
                cg.alpha = alpha;
                return HostStatus.ok;
            }

            Graphic g = go.GetComponent<Graphic>();

            if (g != null) {
                Color c = g.color;
                c.a = alpha;
                g.color = c;
                return HostStatus.ok;
            }

            SpriteRenderer sr = go.GetComponent<SpriteRenderer>();

            if (sr != null) {
                Color c = sr.color;
                c.a = alpha;
                sr.color = c;
                return HostStatus.ok;
            }

            Renderer r = go.GetComponent<Renderer>();

            if (r == null || r.sharedMaterial == null) {
                return HostStatus.notFound;
            }

            int id = ColorPropertyOf(r.sharedMaterial);

            if (id == 0) {
                return HostStatus.notFound;
            }

            r.GetPropertyBlock(block);
            Color current = block.isEmpty ? r.sharedMaterial.GetColor(id) : block.GetColor(id);
            current.a = alpha;
            block.SetColor(id, current);
            r.SetPropertyBlock(block);
            return HostStatus.ok;
        }

        // id < 0 means "the material's main colour".
        private HostStatus SetRendererColor(GameObject go, Color c, int id) {

            Renderer r = go.GetComponent<Renderer>();

            if (r == null || r.sharedMaterial == null) {
                return HostStatus.notFound;
            }

            if (id < 0) {
                id = ColorPropertyOf(r.sharedMaterial);
            }

            if (id == 0 || !r.sharedMaterial.HasProperty(id)) {
                return HostStatus.notFound;
            }

            r.GetPropertyBlock(block);
            block.SetColor(id, c);
            r.SetPropertyBlock(block);
            return HostStatus.ok;
        }

        private static int ColorPropertyOf(Material m) {

            if (m.HasProperty(baseColorId)) {
                return baseColorId;
            }

            return m.HasProperty(colorId) ? colorId : 0;
        }
    }

    // U-M2. An Animator first, then the legacy Animation component the older rigs still use.
    public class UnityAnim : IHostAnim {

        private readonly UnityHandleTable handles;

        // Parameter names per controller, so a missing parameter answers notFound instead of the
        // Animator's own console warning. Animator.parameters allocates, so it is read once.
        private readonly Dictionary<UnityEngine.Object, HashSet<string>> paramsByController =
            new Dictionary<UnityEngine.Object, HashSet<string>>(ReferenceComparer.instance);

        public UnityAnim(UnityHandleTable handles) {
            this.handles = handles;
        }

        public HostStatus PlayClip(Handle h, string clip, float blendSeconds) {

            GameObject go;

            if (!handles.TryGetGameObject(h, out go)) {
                return HostStatus.deadHandle;
            }

            if (string.IsNullOrEmpty(clip)) {
                return HostStatus.invalidArgument;
            }

            Animator animator = go.GetComponentInChildren<Animator>();

            if (animator != null && animator.runtimeAnimatorController != null) {

                int hash = Animator.StringToHash(clip);

                if (!animator.HasState(0, hash)) {
                    return HostStatus.notFound;
                }

                if (blendSeconds > 0f) {
                    animator.CrossFadeInFixedTime(hash, blendSeconds, 0);
                }
                else {
                    animator.Play(hash, 0, 0f);
                }

                return HostStatus.ok;
            }

            UnityEngine.Animation legacy = go.GetComponentInChildren<UnityEngine.Animation>();

            if (legacy != null) {

                if (legacy.GetClip(clip) == null) {
                    return HostStatus.notFound;
                }

                if (blendSeconds > 0f) {
                    legacy.CrossFade(clip, blendSeconds);
                }
                else {
                    legacy.Play(clip);
                }

                return HostStatus.ok;
            }

            return HostStatus.unsupported;
        }

        public HostStatus SetParamFloat(Handle h, string param, float value) {

            Animator animator;
            HostStatus status = ResolveParam(h, param, out animator);

            if (status == HostStatus.ok) {
                animator.SetFloat(param, value);
            }

            return status;
        }

        public HostStatus SetParamBool(Handle h, string param, bool value) {

            Animator animator;
            HostStatus status = ResolveParam(h, param, out animator);

            if (status == HostStatus.ok) {
                animator.SetBool(param, value);
            }

            return status;
        }

        public HostStatus SetTrigger(Handle h, string param) {

            Animator animator;
            HostStatus status = ResolveParam(h, param, out animator);

            if (status == HostStatus.ok) {
                animator.SetTrigger(param);
            }

            return status;
        }

        private HostStatus ResolveParam(Handle h, string param, out Animator animator) {

            animator = null;
            GameObject go;

            if (!handles.TryGetGameObject(h, out go)) {
                return HostStatus.deadHandle;
            }

            animator = go.GetComponentInChildren<Animator>();

            if (animator == null || animator.runtimeAnimatorController == null) {
                return HostStatus.unsupported;
            }

            RuntimeAnimatorController controller = animator.runtimeAnimatorController;
            HashSet<string> names;

            if (!paramsByController.TryGetValue(controller, out names)) {

                names = new HashSet<string>(StringComparer.Ordinal);
                AnimatorControllerParameter[] ps = animator.parameters;

                for (int i = 0; i < ps.Length; i++) {
                    names.Add(ps[i].name);
                }

                paramsByController[controller] = names;
            }

            return param != null && names.Contains(param) ? HostStatus.ok : HostStatus.notFound;
        }
    }

    // U-C1/C2. Completions are queued and delivered by the driver on a later frame, never from
    // inside the Load call, even when Unity finishes synchronously.
    public class UnityAssets : IHostAssets {

        private readonly UnityHandleTable handles;
        private readonly Queue<Action> ready = new Queue<Action>();

        public UnityAssets(UnityHandleTable handles) {
            this.handles = handles;
        }

        public int pendingCount {
            get {
                return ready.Count;
            }
        }

        public void LoadWorld(string key, Action<HostStatus> onDone) {

            if (string.IsNullOrEmpty(key) || !Application.CanStreamedLevelBeLoaded(key)) {
                Enqueue(onDone, HostStatus.notFound);
                return;
            }

            AsyncOperation op = SceneManager.LoadSceneAsync(key);

            if (op == null) {
                Enqueue(onDone, HostStatus.ioError);
                return;
            }

            op.completed += delegate {
                Enqueue(onDone, HostStatus.ok);
            };
        }

        public void LoadAsset(string key, Action<HostStatus, Handle> onDone) {

            if (string.IsNullOrEmpty(key)) {
                Enqueue(onDone, HostStatus.invalidArgument, Handle.none);
                return;
            }

            ResourceRequest req = Resources.LoadAsync(key);

            req.completed += delegate {

                if (req.asset == null) {
                    Enqueue(onDone, HostStatus.notFound, Handle.none);
                }
                else {
                    Enqueue(onDone, HostStatus.ok, handles.Track(req.asset));
                }
            };
        }

        // A GameObject prefab cannot be unloaded on its own in Unity; the handle is dropped and
        // Resources.UnloadUnusedAssets reclaims it with the rest.
        public HostStatus Release(Handle asset) {

            UnityEngine.Object o;

            if (!handles.TryGet(asset, out o)) {
                return HostStatus.deadHandle;
            }

            handles.Release(asset);

            if (!(o is GameObject) && !(o is Component)) {
                Resources.UnloadAsset(o);
            }

            return HostStatus.ok;
        }

        public int PumpAsync() {

            int n = ready.Count;

            for (int i = 0; i < n; i++) {
                ready.Dequeue()();
            }

            return n;
        }

        private void Enqueue(Action<HostStatus> onDone, HostStatus status) {
            if (onDone != null) {
                ready.Enqueue(delegate {
                    onDone(status);
                });
            }
        }

        private void Enqueue(Action<HostStatus, Handle> onDone, HostStatus status, Handle h) {
            if (onDone != null) {
                ready.Enqueue(delegate {
                    onDone(status, h);
                });
            }
        }
    }

    // U-X. Queries only; the simulation stays PhysX's. A hit reports the rigidbody's object when
    // there is one (what a spawned prefab's handle points at), else the collider's.
    public class UnityPhysics : IHostPhysics, IContactSink {

        private const int bufferSize = 64;

        private readonly UnityHandleTable handles;
        private readonly Collider[] overlapBuffer = new Collider[bufferSize];

        private struct PendingContact {
            public Handle a;
            public Handle b;
            public ContactKind kind;
            public Vec3 point;
        }

        private readonly List<PendingContact> pending = new List<PendingContact>();
        private IContactSink sink;

        public QueryTriggerInteraction triggers = QueryTriggerInteraction.Ignore;

        public UnityPhysics(UnityHandleTable handles) {
            this.handles = handles;
        }

        public bool Raycast(Vec3 origin, Vec3 direction, float maxDistance, int mask, out QueryHit hit) {

            RaycastHit rh;

            if (UnityEngine.Physics.Raycast(UnityFrame.ToUnity(origin), UnityFrame.ToUnity(direction), out rh, maxDistance, mask, triggers)) {
                hit = ToHit(rh);
                return true;
            }

            hit = default(QueryHit);
            return false;
        }

        public int Overlap(Shape shape, Vec3 position, Quat rotation, int mask, List<Handle> results) {

            Vector3 p = UnityFrame.ToUnity(position);
            Quaternion q = UnityFrame.ToUnity(rotation);
            int n;

            switch (shape.kind) {
                case ShapeKind.sphere:
                    n = UnityEngine.Physics.OverlapSphereNonAlloc(p, shape.size.x, overlapBuffer, mask, triggers);
                    break;
                case ShapeKind.box:
                    n = UnityEngine.Physics.OverlapBoxNonAlloc(p, UnityFrame.ScaleToUnity(shape.size), overlapBuffer, q, mask, triggers);
                    break;
                case ShapeKind.capsule: {
                        Vector3 axis = q * Vector3.up * shape.size.y;
                        n = UnityEngine.Physics.OverlapCapsuleNonAlloc(p - axis, p + axis, shape.size.x, overlapBuffer, mask, triggers);
                        break;
                    }
                default:
                    n = 0;
                    break;
            }

            if (results == null) {
                return n;
            }

            results.Clear();

            for (int i = 0; i < n; i++) {
                Handle h = HandleOf(overlapBuffer[i]);

                if (!results.Contains(h)) {
                    results.Add(h);
                }

                overlapBuffer[i] = null;
            }

            return results.Count;
        }

        public bool Sweep(Shape shape, Vec3 from, Vec3 to, Quat rotation, int mask, out QueryHit hit) {

            Vector3 a = UnityFrame.ToUnity(from);
            Vector3 delta = UnityFrame.ToUnity(to) - a;
            float distance = delta.magnitude;
            hit = default(QueryHit);

            if (distance <= 0f) {
                return false;
            }

            Vector3 dir = delta / distance;
            Quaternion q = UnityFrame.ToUnity(rotation);
            RaycastHit rh;
            bool got;

            switch (shape.kind) {
                case ShapeKind.sphere:
                    got = UnityEngine.Physics.SphereCast(a, shape.size.x, dir, out rh, distance, mask, triggers);
                    break;
                case ShapeKind.box:
                    got = UnityEngine.Physics.BoxCast(a, UnityFrame.ScaleToUnity(shape.size), dir, out rh, q, distance, mask, triggers);
                    break;
                case ShapeKind.capsule: {
                        Vector3 axis = q * Vector3.up * shape.size.y;
                        got = UnityEngine.Physics.CapsuleCast(a - axis, a + axis, shape.size.x, dir, out rh, distance, mask, triggers);
                        break;
                    }
                default:
                    rh = default(RaycastHit);
                    got = false;
                    break;
            }

            if (got) {
                hit = ToHit(rh);
            }

            return got;
        }

        public void SetContactSink(IContactSink sink) {
            this.sink = sink;
        }

        // Adds the relay that turns OnCollision*/OnTrigger* on this object into queued contacts.
        // Not part of the contract: which objects report contacts is a host setup concern.
        public HostStatus WatchContacts(Handle h) {

            GameObject go;

            if (!handles.TryGetGameObject(h, out go)) {
                return HostStatus.deadHandle;
            }

            UnityContactRelay relay = go.GetComponent<UnityContactRelay>();

            if (relay == null) {
                relay = go.AddComponent<UnityContactRelay>();
            }

            relay.Bind(this, handles);
            return HostStatus.ok;
        }

        // IContactSink, fed by the relays during Unity's physics step. Queued, not forwarded,
        // because the contract delivers contacts on the core's schedule, before OnFixed.
        public void OnContact(Handle a, Handle b, ContactKind kind, Vec3 point) {
            PendingContact c;
            c.a = a;
            c.b = b;
            c.kind = kind;
            c.point = point;
            pending.Add(c);
        }

        public int DeliverContacts() {

            int n = pending.Count;

            if (sink != null) {
                for (int i = 0; i < n; i++) {
                    sink.OnContact(pending[i].a, pending[i].b, pending[i].kind, pending[i].point);
                }
            }

            pending.Clear();
            return n;
        }

        private QueryHit ToHit(RaycastHit rh) {
            QueryHit hit;
            hit.handle = HandleOf(rh.collider);
            hit.point = UnityFrame.ToCore(rh.point);
            hit.normal = UnityFrame.ToCore(rh.normal);
            hit.distance = rh.distance;
            return hit;
        }

        public Handle HandleOf(Collider c) {

            if (c == null) {
                return Handle.none;
            }

            Rigidbody body = c.attachedRigidbody;
            return handles.Track(body != null ? body.gameObject : c.gameObject);
        }
    }

    public class UnityContactRelay : MonoBehaviour {

        private IContactSink sink;
        private UnityHandleTable handles;
        private Handle self;

        public void Bind(IContactSink sink, UnityHandleTable handles) {
            this.sink = sink;
            this.handles = handles;
            self = handles.Track(gameObject);
        }

        private void Report(Collider other, ContactKind kind, Vector3 point) {

            if (sink == null || other == null) {
                return;
            }

            Rigidbody body = other.attachedRigidbody;
            Handle o = handles.Track(body != null ? body.gameObject : other.gameObject);
            sink.OnContact(self, o, kind, UnityFrame.ToCore(point));
        }

        private void OnCollisionEnter(Collision c) {
            Report(c.collider, ContactKind.collisionEnter, c.contactCount > 0 ? c.GetContact(0).point : transform.position);
        }

        private void OnCollisionExit(Collision c) {
            Report(c.collider, ContactKind.collisionExit, transform.position);
        }

        private void OnTriggerEnter(Collider other) {
            Report(other, ContactKind.triggerEnter, other.transform.position);
        }

        private void OnTriggerExit(Collider other) {
            Report(other, ContactKind.triggerExit, other.transform.position);
        }
    }
}
