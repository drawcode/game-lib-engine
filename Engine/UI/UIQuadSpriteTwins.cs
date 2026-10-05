using System.Collections.Generic;

using UnityEngine;

namespace Engine.UI {

    // RUNTIME UIQuadSprite twins for legacy sprites that live IN WORLD SPACE (bot health bars,
    // action-zone icons, choice markers), plus the kill-switch swap for them.
    //
    // Why not a toolkit view: the shared PanelSettings is overlay mode, so a toolkit view composites
    // after every camera — it can neither sit at a 3D position nor be occluded by the level. A quad
    // in the camera stack can (same reasoning as the backdrop quads, iter 29).
    //
    // Why runtime and not UIQuadSpriteBaker: these sprites sit in big gameplay prefabs
    // (GamePlayerObject is 74k YAML lines) and some change at runtime (zone icons are renamed by
    // action, the health foreground is a FILLED sprite the baker skips). Building the twin from the
    // live widget — its atlas, sprite UVs, pivot, colour, depth and fill — keeps the prefab YAML
    // untouched and always matches whatever the widget currently shows. The cost: the builder reads
    // NGUI, so it compiles out with NGUI. At NGUI removal (step 3) these roots must be BAKED (the
    // serialized twin survives; the fill fields are serialized too).
    //
    // Geometry is NGUI 2.7's own maths (SimpleFill / SlicedFill / FilledFill linear), in the
    // widget's local space with the pivot offset applied, on the widget's own GameObject — so the
    // twin inherits the transform, every tween and every billboard rotation. Sort order =
    // sortingBase + NGUI depth. Idempotent: syncing again refreshes UVs/colour (call it after a
    // sprite rename).
    //
    // AGNOSTIC surface: callers pass a GameObject; no backend type leaks out of this file.
    public static class UIQuadSpriteTwins {

        public const string quadShaderName = "Drawlabs/UI/Quad Transparent Colored";

        // One quad material per atlas material, shared by every twin of every instance (all bots
        // batch together). Same shader the baked quads use; built at runtime because the baked
        // <Atlas>Quad.mat assets are not under Resources.
        static readonly Dictionary<Material, Material> quadMaterials =
            new Dictionary<Material, Material>();

        // Build or refresh the twins under `root`, then apply the kill switch. Returns the number of
        // twins (0 without NGUI: there is nothing to build from, existing quads are still swapped).
        public static int Sync(GameObject root, int sortingBase = 0) {

            if (root == null) {
                return 0;
            }

            int count = 0;

#if USE_UI_NGUI_2_7
            foreach (UISprite sprite in root.GetComponentsInChildren<UISprite>(true)) {
                if (Twin(sprite, sortingBase) != null) {
                    count++;
                }
            }
#endif

            Apply(root);

            return count;
        }

        // Kill switch for every UIQuadSprite under root: toolkit on -> quads draw, legacy widgets
        // disabled; off -> the reverse. Same rule as UIQuadSpriteSwap (games-ui), which this engine
        // file cannot reference.
        public static void Apply(GameObject root) {

            if (root == null) {
                return;
            }

            bool useQuads = UIPlatform.toolkitViewsEnabled;

            foreach (UIQuadSprite quad in root.GetComponentsInChildren<UIQuadSprite>(true)) {

                quad.SetVisible(useQuads);

#if USE_UI_NGUI_2_7 || USE_UI_NGUI_3
                UIWidget widget = quad.GetComponent<UIWidget>();

                if (widget != null) {
                    widget.enabled = !useQuads;
                }
#endif
            }
        }

        public static bool quadsActive {
            get {
                return UIPlatform.toolkitViewsEnabled;
            }
        }

#if USE_UI_NGUI_2_7
        // The twin of one sprite, or null when it cannot be drawn as a quad (no atlas sprite,
        // premultiplied atlas, tiled or radial fill, another renderer already on the GameObject).
        public static UIQuadSprite Twin(UISprite sprite, int sortingBase = 0) {

            if (sprite == null || sprite.atlas == null || !sprite.isValid) {
                return null;
            }

            if (sprite.atlas.premultipliedAlpha) {
                return null;
            }

            UISprite.Type type = sprite.type;
            UIQuadSprite.FillAxis axis = UIQuadSprite.FillAxis.None;

            if (type == UISprite.Type.Filled) {

                if (sprite.fillDirection == UISprite.FillDirection.Horizontal) {
                    axis = UIQuadSprite.FillAxis.Horizontal;
                }
                else if (sprite.fillDirection == UISprite.FillDirection.Vertical) {
                    axis = UIQuadSprite.FillAxis.Vertical;
                }
                else {
                    return null;
                }
            }
            else if (type != UISprite.Type.Simple && type != UISprite.Type.Sliced) {
                return null;
            }

            GameObject go = sprite.gameObject;
            UIQuadSprite quad = go.GetComponent<UIQuadSprite>();

            if (quad == null && go.GetComponent<Renderer>() != null) {
                return null;
            }

            // A twin BAKED into the prefab (UIQuadSpriteBaker: HUD indicators, backdrop) owns its
            // own geometry and sort base; never rebuild it from here.
            if (quad != null && !IsRuntimeMaterial(quad.material)) {
                return quad;
            }

            Material material = QuadMaterial(sprite.atlas.spriteMaterial);

            if (material == null) {
                return null;
            }

            List<Vector3> verts = new List<Vector3>(4);
            List<Vector2> uvs = new List<Vector2>(4);

            Rect outer = sprite.outerUV;
            Rect inner = sprite.innerUV;

            if (type == UISprite.Type.Sliced && outer != inner) {
                SlicedFill(sprite, outer, inner, verts, uvs);
            }
            else {
                // Simple, a sliced sprite without borders, and the FULL quad of a filled sprite.
                SimpleFill(outer, verts, uvs);
            }

            Vector2 pivotOffset = sprite.pivotOffset;
            Vector2 relativeSize = sprite.relativeSize;
            Vector3 offset = new Vector3(pivotOffset.x * relativeSize.x, pivotOffset.y * relativeSize.y, 0f);

            for (int i = 0; i < verts.Count; i++) {
                verts[i] += offset;
            }

            if (quad == null) {
                // RequireComponent brings the MeshFilter + MeshRenderer. Hidden until Apply decides.
                quad = go.AddComponent<UIQuadSprite>();
                quad.SetVisible(false);
            }

            quad.material = material;
            quad.corners = verts.ToArray();
            quad.uvs = uvs.ToArray();
            quad.color = sprite.color;
            quad.sortingOrder = sortingBase + sprite.depth;
            quad.fillAxis = axis;
            quad.fillInvert = sprite.invert;
            quad.fillAmount = axis == UIQuadSprite.FillAxis.None ? 1f : sprite.fillAmount;
            quad.Build();

            return quad;
        }

        // UISprite.SimpleFill (also the full quad FilledFill starts from).
        static void SimpleFill(Rect outer, List<Vector3> verts, List<Vector2> uvs) {

            Vector2 uv0 = new Vector2(outer.xMin, outer.yMin);
            Vector2 uv1 = new Vector2(outer.xMax, outer.yMax);

            verts.Add(new Vector3(1f, 0f, 0f));
            verts.Add(new Vector3(1f, -1f, 0f));
            verts.Add(new Vector3(0f, -1f, 0f));
            verts.Add(new Vector3(0f, 0f, 0f));

            uvs.Add(uv1);
            uvs.Add(new Vector2(uv1.x, uv0.y));
            uvs.Add(uv0);
            uvs.Add(new Vector2(uv0.x, uv1.y));
        }

        // UISprite.SlicedFill (same port as UIQuadSpriteBaker), at the CURRENT localScale.
        static void SlicedFill(UISprite sprite, Rect outer, Rect inner, List<Vector3> verts, List<Vector2> uvs) {

            Vector2[] v = new Vector2[4];
            Vector2[] uv = new Vector2[4];

            v[0] = Vector2.zero;
            v[1] = Vector2.zero;
            v[2] = new Vector2(1f, -1f);
            v[3] = new Vector2(1f, -1f);

            Texture tex = sprite.mainTexture;

            if (tex != null) {

                float pixelSize = sprite.atlas.pixelSize;
                float borderLeft = (inner.xMin - outer.xMin) * pixelSize;
                float borderRight = (outer.xMax - inner.xMax) * pixelSize;
                float borderTop = (inner.yMax - outer.yMax) * pixelSize;
                float borderBottom = (outer.yMin - inner.yMin) * pixelSize;

                Vector3 scale = sprite.transform.localScale;
                scale.x = Mathf.Max(0f, scale.x);
                scale.y = Mathf.Max(0f, scale.y);

                Vector2 sz = new Vector2(scale.x / tex.width, scale.y / tex.height);
                Vector2 tl = new Vector2(borderLeft / sz.x, borderTop / sz.y);
                Vector2 br = new Vector2(borderRight / sz.x, borderBottom / sz.y);

                UIWidget.Pivot pv = sprite.pivot;

                if (pv == UIWidget.Pivot.Right || pv == UIWidget.Pivot.TopRight || pv == UIWidget.Pivot.BottomRight) {
                    v[0].x = Mathf.Min(0f, 1f - (br.x + tl.x));
                    v[1].x = v[0].x + tl.x;
                    v[2].x = v[0].x + Mathf.Max(tl.x, 1f - br.x);
                    v[3].x = v[0].x + Mathf.Max(tl.x + br.x, 1f);
                }
                else {
                    v[1].x = tl.x;
                    v[2].x = Mathf.Max(tl.x, 1f - br.x);
                    v[3].x = Mathf.Max(tl.x + br.x, 1f);
                }

                if (pv == UIWidget.Pivot.Bottom || pv == UIWidget.Pivot.BottomLeft || pv == UIWidget.Pivot.BottomRight) {
                    v[0].y = Mathf.Max(0f, -1f - (br.y + tl.y));
                    v[1].y = v[0].y + tl.y;
                    v[2].y = v[0].y + Mathf.Min(tl.y, -1f - br.y);
                    v[3].y = v[0].y + Mathf.Min(tl.y + br.y, -1f);
                }
                else {
                    v[1].y = tl.y;
                    v[2].y = Mathf.Min(tl.y, -1f - br.y);
                    v[3].y = Mathf.Min(tl.y + br.y, -1f);
                }

                uv[0] = new Vector2(outer.xMin, outer.yMax);
                uv[1] = new Vector2(inner.xMin, inner.yMax);
                uv[2] = new Vector2(inner.xMax, inner.yMin);
                uv[3] = new Vector2(outer.xMax, outer.yMin);
            }

            for (int x = 0; x < 3; ++x) {

                int x2 = x + 1;

                for (int y = 0; y < 3; ++y) {

                    if (!sprite.fillCenter && x == 1 && y == 1) {
                        continue;
                    }

                    int y2 = y + 1;

                    verts.Add(new Vector3(v[x2].x, v[y].y, 0f));
                    verts.Add(new Vector3(v[x2].x, v[y2].y, 0f));
                    verts.Add(new Vector3(v[x].x, v[y2].y, 0f));
                    verts.Add(new Vector3(v[x].x, v[y].y, 0f));

                    uvs.Add(new Vector2(uv[x2].x, uv[y].y));
                    uvs.Add(new Vector2(uv[x2].x, uv[y2].y));
                    uvs.Add(new Vector2(uv[x].x, uv[y2].y));
                    uvs.Add(new Vector2(uv[x].x, uv[y].y));
                }
            }
        }
#endif

        static bool IsRuntimeMaterial(Material material) {
            return material != null && quadMaterials.ContainsValue(material);
        }

        // The runtime quad material for an atlas material (cached; null when the shader is missing).
        public static Material QuadMaterial(Material atlasMaterial) {

            if (atlasMaterial == null) {
                return null;
            }

            Material cached;

            if (quadMaterials.TryGetValue(atlasMaterial, out cached) && cached != null) {
                return cached;
            }

            Shader shader = Shader.Find(quadShaderName);

            if (shader == null) {
                return null;
            }

            Material created = new Material(shader);
            created.name = atlasMaterial.name + "Quad (runtime)";
            created.mainTexture = atlasMaterial.mainTexture;
            created.hideFlags = HideFlags.DontSave;

            quadMaterials[atlasMaterial] = created;

            return created;
        }
    }
}
