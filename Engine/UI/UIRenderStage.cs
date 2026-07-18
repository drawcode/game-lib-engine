using UnityEngine;

namespace Engine.UI {

    // Renders a small 3D subtree (spinning coin, character preview) into a private RenderTexture
    // so a UI backend can composite REAL 3D content inside its views. UI Toolkit panels draw above
    // the entire camera stack, so world-rendered content can never appear on top of a toolkit
    // view — this stage is the platform's answer: 3D -> RT -> element background (3B, coin first;
    // the same machinery serves character previews later).
    //
    // Design decisions (2026-07-17, discussed):
    //  * Plain RenderTexture, NOT RTHandle — RTHandle exists for screen-relative scaling and SRP
    //    frame-graph pooling; a widget snapshot is fixed-size and persistent, so that machinery
    //    is dead weight and an SRP API coupling. The allocation lives behind this class, so the
    //    strategy can change later without touching callers.
    //  * PER-WIDGET RT, not a shared atlas — passes dominate cost, not texture objects (a 128px
    //    RGBA RT is ~64KB); sharing a texture doesn't merge camera passes but does add packing/
    //    clear/lifetime complexity. Revisit inside this seam if a screen ever needs dozens of
    //    3D widgets at once (store lists).
    //  * Renders ONLY while visible — SetVisible(false) disables the camera entirely, so a
    //    hidden widget costs nothing (same philosophy as refresh-coins-on-show).
    //
    // The content stays WHERE IT IS in the scene: the stage flips it (recursively) onto an
    // isolated layer so game/NGUI cameras stop drawing it, and only the stage camera (culling
    // that layer alone) sees it. Detach() restores the original layers — kill-switch safe.
    public class UIRenderStage : MonoBehaviour {

        public Camera stageCamera;
        public RenderTexture texture;

        private GameObject content;
        private int[] originalLayers;
        private Transform[] contentTransforms;

        // Frame the content's MESH bounds (particles excluded — their bounds balloon and would
        // zoom the framing out) and render it on `layer` into a size×size RT. framePadding is the
        // margin around the meshes: ~1.15 crops tight; widgets whose particle effects should
        // spill past the model (the coin's glow) want ~1.6-1.8 so the effect has RT room.
        public static UIRenderStage Attach(
            GameObject content, int layer, int size = 256, float framePadding = 1.15f) {

            if (content == null || layer < 0) {
                return null;
            }

            GameObject go = new GameObject("ui-render-stage-" + content.name);
            UIRenderStage stage = go.AddComponent<UIRenderStage>();
            stage.content = content;

            // Remember + flip layers so ONLY the stage camera renders this subtree.
            stage.contentTransforms = content.GetComponentsInChildren<Transform>(true);
            stage.originalLayers = new int[stage.contentTransforms.Length];

            for (int i = 0; i < stage.contentTransforms.Length; i++) {
                stage.originalLayers[i] = stage.contentTransforms[i].gameObject.layer;
                stage.contentTransforms[i].gameObject.layer = layer;
            }

            // Fit to the renderable meshes (skip particle renderers for framing).
            Bounds bounds = new Bounds(content.transform.position, Vector3.one * .01f);
            bool found = false;

            foreach (Renderer r in content.GetComponentsInChildren<Renderer>(true)) {

                if (r is ParticleSystemRenderer) {
                    continue;
                }

                if (!found) {
                    bounds = r.bounds;
                    found = true;
                }
                else {
                    bounds.Encapsulate(r.bounds);
                }
            }

            float extent = Mathf.Max(bounds.extents.x, bounds.extents.y, .005f);
            float dist = Mathf.Max(bounds.extents.z * 4f, extent * 4f);

            // The NGUI/UI plane faces the -Z side (its cameras look down +Z), so the stage camera
            // sits on -Z of the content looking forward.
            GameObject camGo = new GameObject("stage-camera");
            camGo.transform.SetParent(go.transform, false);
            camGo.transform.position = bounds.center + Vector3.back * dist;
            camGo.transform.rotation = Quaternion.identity;

            Camera cam = camGo.AddComponent<Camera>();
            cam.orthographic = true;
            cam.orthographicSize = extent * framePadding;
            cam.nearClipPlane = dist * .05f;
            cam.farClipPlane = dist * 4f;
            cam.cullingMask = 1 << layer;
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = Color.clear;   // transparent: the element composites over its skin
            cam.allowHDR = false;
            cam.allowMSAA = false;

            stage.texture = new RenderTexture(size, size, 16, RenderTextureFormat.ARGB32);
            stage.texture.name = go.name;
            cam.targetTexture = stage.texture;

            // A stage-only light so lit materials read; scene lights sit on other layers.
            GameObject lightGo = new GameObject("stage-light");
            lightGo.transform.SetParent(camGo.transform, false);
            lightGo.transform.rotation = Quaternion.Euler(35f, -30f, 0f);
            Light light = lightGo.AddComponent<Light>();
            light.type = LightType.Directional;
            light.cullingMask = 1 << layer;
            light.intensity = 1.1f;

            stage.stageCamera = cam;

            return stage;
        }

        // Camera fully off while hidden — a hidden widget costs nothing.
        public void SetVisible(bool visible) {

            if (stageCamera != null) {
                stageCamera.enabled = visible;
            }
        }

        // Restore the content's original layers (game/NGUI cameras own it again) and tear the
        // stage down. Safe to call when already detached.
        public void Detach() {

            if (contentTransforms != null) {

                for (int i = 0; i < contentTransforms.Length; i++) {

                    if (contentTransforms[i] != null) {
                        contentTransforms[i].gameObject.layer = originalLayers[i];
                    }
                }

                contentTransforms = null;
            }

            Destroy(gameObject);
        }

        void OnDestroy() {

            if (stageCamera != null) {
                stageCamera.targetTexture = null;
            }

            if (texture != null) {
                texture.Release();
                Destroy(texture);
                texture = null;
            }
        }
    }
}
