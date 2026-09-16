using UnityEngine;

namespace Engine.UI {

    // A textured, vertex-coloured quad on a plain MeshRenderer. AGNOSTIC — no UI backend types.
    //
    // Why this exists: some legacy sprites must stay INSIDE the camera stack, behind 3D content.
    // The menu backdrop (red plain, vignette, per-screen backer card) sits under the worlds rig,
    // the small character rig and the menu particles. A UI Toolkit view cannot do that: the shared
    // PanelSettings renders in OVERLAY mode, so every toolkit view composites after every camera
    // and a full-screen backdrop view would bury all of it (see UILayers.notification).
    //
    // The quad reproduces the legacy draw 1:1 rather than approximating it: the four corners, UVs
    // and colour are BAKED from the running legacy widget's own geometry (local space, pivot and
    // trim padding already applied), and it lives on the same GameObject, so it inherits the same
    // transform, layer (camera) and every position tween. Colour is written straight into the
    // vertex colours exactly like the legacy mesh, so linear/sRGB handling is identical.
    //
    // Who switches it on is the owner's business (kill switch): SetVisible only toggles the
    // renderer, it never touches the legacy widget.
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class UIQuadSprite : MonoBehaviour {

        public Material material;

        // Local-space corners, in the order bottom-left, top-left, top-right, bottom-right.
        public Vector3[] corners = new Vector3[] {
            new Vector3(-.5f, -.5f, 0f), new Vector3(-.5f, .5f, 0f),
            new Vector3(.5f, .5f, 0f), new Vector3(.5f, -.5f, 0f)
        };

        // Normalized texture coordinates matching `corners`.
        public Vector2[] uvs = new Vector2[] {
            new Vector2(0f, 0f), new Vector2(0f, 1f), new Vector2(1f, 1f), new Vector2(1f, 0f)
        };

        public Color color = Color.white;

        // Draw order under the same camera. Legacy widget depth - 100: NGUI drew its panels before
        // the particles sharing the camera (sorting order 0), so the quads must sort below them.
        public int sortingOrder;

        static readonly int[] triangles = new int[] { 0, 1, 2, 2, 3, 0 };

        Mesh mesh;
        MeshRenderer meshRenderer;
        readonly Color32[] colors = new Color32[4];

        public bool isVisible {
            get {
                return meshRenderer != null && meshRenderer.enabled;
            }
        }

        void Awake() {
            Build();
        }

        void OnDestroy() {
            if (mesh != null) {
                Destroy(mesh);
                mesh = null;
            }
        }

        public void Build() {

            meshRenderer = GetComponent<MeshRenderer>();
            meshRenderer.sharedMaterial = material;
            meshRenderer.sortingOrder = sortingOrder;
            meshRenderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
            meshRenderer.receiveShadows = false;

            if (mesh == null) {
                mesh = new Mesh();
                mesh.name = "UIQuadSprite";
                mesh.MarkDynamic();
                GetComponent<MeshFilter>().sharedMesh = mesh;
            }

            mesh.Clear();
            mesh.vertices = corners;
            mesh.uv = uvs;
            mesh.triangles = triangles;
            ApplyColor();
            mesh.RecalculateBounds();
        }

        public void SetColor(Color value) {
            color = value;
            ApplyColor();
        }

        public void SetVisible(bool visible) {
            if (meshRenderer == null) {
                Build();
            }
            meshRenderer.enabled = visible;
        }

        void ApplyColor() {
            if (mesh == null) {
                return;
            }
            Color32 c = color;
            for (int i = 0; i < colors.Length; i++) {
                colors[i] = c;
            }
            mesh.colors32 = colors;
        }
    }
}
