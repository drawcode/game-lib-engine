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

        // Local-space corners, FOUR PER QUAD, each quad in a consistent winding (a baked NGUI fill
        // keeps NGUI's own order: TR, BR, BL, TL). One quad for a simple sprite, nine for a sliced
        // one. The shader culls nothing, so either winding renders.
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

        // Linear fill (a legacy filled sprite: a slider foreground, a progress bar). ONE quad only,
        // in the baked NGUI order TR, BR, BL, TL; `corners`/`uvs` hold the FULL (amount 1) quad and
        // SetFillAmount cuts it exactly like NGUI 2.7's UISprite.FilledFill: Horizontal keeps the
        // left part (invert: the right), Vertical keeps the bottom part (invert: the top), and the
        // UVs are cut by the same fraction so the texture is cropped, never squashed.
        public enum FillAxis {
            None,
            Horizontal,
            Vertical
        }

        public FillAxis fillAxis = FillAxis.None;
        public bool fillInvert;

        [Range(0f, 1f)]
        public float fillAmount = 1f;

        Mesh mesh;
        MeshRenderer meshRenderer;
        Color32[] colors;

        // Fill scratch, allocated once in Build: SetFillAmount runs on value changes in gameplay
        // (health bars), so it rewrites these in place and never allocates.
        Vector3[] fillCorners;
        Vector2[] fillUvs;
        float appliedFill = -1f;

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

            int quads = corners.Length / 4;
            int[] triangles = new int[quads * 6];

            for (int q = 0; q < quads; q++) {
                int v = q * 4, t = q * 6;
                triangles[t] = v;
                triangles[t + 1] = v + 1;
                triangles[t + 2] = v + 2;
                triangles[t + 3] = v + 2;
                triangles[t + 4] = v + 3;
                triangles[t + 5] = v;
            }

            colors = new Color32[quads * 4];

            mesh.Clear();
            mesh.vertices = corners;
            mesh.uv = uvs;
            mesh.triangles = triangles;
            ApplyColor();
            mesh.RecalculateBounds();

            // Bounds stay those of the full quad (a superset of any fill), so a fill change never
            // has to recalculate them.
            appliedFill = -1f;

            if (isFillable) {

                if (fillCorners == null) {
                    fillCorners = new Vector3[4];
                    fillUvs = new Vector2[4];
                }

                ApplyFill();
            }
        }

        bool isFillable {
            get {
                return fillAxis != FillAxis.None
                    && corners != null && corners.Length == 4
                    && uvs != null && uvs.Length == 4;
            }
        }

        // Mirror a legacy fill amount. Cheap to call every frame: an unchanged value returns after
        // one compare, a changed one rewrites 4 vertices + 4 UVs into preallocated arrays.
        public void SetFillAmount(float value) {

            value = Mathf.Clamp01(value);

            if (value == appliedFill) {
                return;
            }

            fillAmount = value;
            ApplyFill();
        }

        void ApplyFill() {

            if (mesh == null || !isFillable || fillCorners == null) {
                return;
            }

            float f = Mathf.Clamp01(fillAmount);

            // Full quad, NGUI order: [0] TR, [1] BR, [2] BL, [3] TL.
            float x0 = corners[2].x, x1 = corners[0].x;
            float yTop = corners[0].y, yBottom = corners[1].y;
            float z = corners[0].z;
            float u0 = uvs[2].x, u1 = uvs[0].x;
            float v0 = uvs[2].y, v1 = uvs[0].y;

            if (fillAxis == FillAxis.Horizontal) {

                float w = (x1 - x0) * f;
                float du = (u1 - u0) * f;

                if (fillInvert) {
                    x0 = x1 - w;
                    u0 = u1 - du;
                }
                else {
                    x1 = x0 + w;
                    u1 = u0 + du;
                }
            }
            else {

                float h = (yTop - yBottom) * f;
                float dv = (v1 - v0) * f;

                if (fillInvert) {
                    yBottom = yTop - h;
                    v0 = v1 - dv;
                }
                else {
                    yTop = yBottom + h;
                    v1 = v0 + dv;
                }
            }

            fillCorners[0] = new Vector3(x1, yTop, z);
            fillCorners[1] = new Vector3(x1, yBottom, z);
            fillCorners[2] = new Vector3(x0, yBottom, z);
            fillCorners[3] = new Vector3(x0, yTop, z);

            fillUvs[0] = new Vector2(u1, v1);
            fillUvs[1] = new Vector2(u1, v0);
            fillUvs[2] = new Vector2(u0, v0);
            fillUvs[3] = new Vector2(u0, v1);

            mesh.vertices = fillCorners;
            mesh.uv = fillUvs;

            appliedFill = f;
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
            if (mesh == null || colors == null) {
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
