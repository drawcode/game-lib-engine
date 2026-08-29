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

        // followContent: the camera's offset from the content root, held every LateUpdate.
        private bool followContent;
        private Vector3 followOffset;
        private float framePaddingUsed;

        // Frame the content's MESH bounds (particles excluded — their bounds balloon and would
        // zoom the framing out) and render it on `layer` into a size×size RT. framePadding is the
        // margin around the meshes: ~1.15 crops tight; widgets whose particle effects should
        // spill past the model (the coin's glow) want ~1.6-1.8 so the effect has RT room.
        //
        // followContent (3I): the framing below is computed ONCE, from where the content sits at
        // ATTACH TIME. That is fine for a widget pinned in place (the coin) and quietly fatal for
        // one that is animated into position: the character card is attached while its container
        // is still parked off-screen, so the camera framed empty space ~14 units above the bot and
        // the RT came back fully transparent. With this on, the camera holds its offset to the
        // content root each LateUpdate, so it travels with the show/hide tween. Rotation and scale
        // BELOW the staged root still read as motion in the RT — which is why the camera tracks
        // position only, and why it must not simply be parented to the content.
        //
        // keepColliderLayers (3I): the layer flip moves the WHOLE subtree off the UI event layer,
        // which silently kills any interaction the content still owns — the character preview is
        // drag-to-rotate, and UICamera only raycasts its own event mask. Nodes that carry a
        // Collider but NO Renderer contribute nothing to the stage image, so they can keep their
        // original layer and stay pickable while the meshes render off-screen. Off by default:
        // the coin deliberately goes fully inert while staged.
        // lightIntensity: the stage light is the ONLY light the content gets (its cullingMask is
        // the stage layer alone), so it is also the exposure control. The 1.1 default is what the
        // character card was tuned against and is kept for compatibility, but it OVER-EXPOSES the
        // gold coin: measured against the legacy capture, 73% of the coin's pixels clipped at
        // green=255 where legacy clips none, and the median green read 255 against 207 — which is
        // what turned a shaded gold coin into a flat yellow one. Coin callers pass a lower value.
        public static UIRenderStage Attach(
            GameObject content, int layer, int size = 256, float framePadding = 1.15f,
            bool keepColliderLayers = false, bool followContent = false,
            float lightIntensity = 1.1f) {

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

            // The NGUI/UI plane faces the -Z side (its cameras look down +Z), so the stage camera
            // sits on -Z of the content looking forward.
            GameObject camGo = new GameObject("stage-camera");
            camGo.transform.SetParent(go.transform, false);
            camGo.transform.rotation = Quaternion.identity;

            Camera cam = camGo.AddComponent<Camera>();
            cam.orthographic = true;
            cam.cullingMask = 1 << layer;
            cam.clearFlags = CameraClearFlags.SolidColor;
            cam.backgroundColor = Color.clear;   // transparent: the element composites over its skin
            cam.allowHDR = false;
            cam.allowMSAA = false;

            stage.stageCamera = cam;
            stage.framePaddingUsed = framePadding;
            stage.followContent = followContent;
            stage.Frame();

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
            light.intensity = lightIntensity;

            // Interaction nodes stay on their original layer (see keepColliderLayers above).
            // Renderer-less by test, so the stage camera loses nothing by not culling them.
            if (keepColliderLayers) {

                for (int i = 0; i < stage.contentTransforms.Length; i++) {

                    GameObject go2 = stage.contentTransforms[i].gameObject;

                    if (go2.GetComponent<Collider>() != null && go2.GetComponent<Renderer>() == null) {
                        go2.layer = stage.originalLayers[i];
                    }
                }
            }

            return stage;
        }

        // World bounds of what the renderer ACTUALLY draws right now.
        //
        // Renderer.bounds on a SkinnedMeshRenderer is the animation-safe box — sized to hold every
        // pose in the rig, not the pose on screen. Framing on it zoomed the character preview out
        // to roughly half the size the legacy path drew it (measured, iter 9). Baking the current
        // pose gives the real silhouette, so the stage frames what the player sees. Non-skinned
        // renderers (the coin) are unaffected and take the plain bounds.
        private static Bounds PosedBounds(Renderer r) {

            SkinnedMeshRenderer smr = r as SkinnedMeshRenderer;

            if (smr == null || smr.sharedMesh == null) {
                return r.bounds;
            }

            Mesh baked = new Mesh();
            smr.BakeMesh(baked, true);   // true: apply the renderer's scale

            Bounds local = baked.bounds;
            Destroy(baked);

            // Local (renderer space, scale already baked in) -> world, via the 8 corners so a
            // rotated rig still yields a correct axis-aligned box.
            Transform t = smr.transform;
            Vector3 c = local.center;
            Vector3 e = local.extents;

            Bounds world = new Bounds(
                t.TransformPoint(c + new Vector3(-e.x, -e.y, -e.z)), Vector3.zero);

            for (int i = 1; i < 8; i++) {

                Vector3 corner = c + new Vector3(
                    (i & 1) == 0 ? -e.x : e.x,
                    (i & 2) == 0 ? -e.y : e.y,
                    (i & 4) == 0 ? -e.z : e.z);

                world.Encapsulate(t.TransformPoint(corner));
            }

            return world;
        }

        // Fit the camera to the content's MESH bounds (particles excluded — their bounds balloon
        // and would zoom the framing out) and record the offset the follow mode holds.
        private void Frame() {

            if (content == null || stageCamera == null) {
                return;
            }

            Bounds bounds = new Bounds(content.transform.position, Vector3.one * .01f);
            bool found = false;

            foreach (Renderer r in content.GetComponentsInChildren<Renderer>(true)) {

                if (r is ParticleSystemRenderer) {
                    continue;
                }

                Bounds b = PosedBounds(r);

                if (!found) {
                    bounds = b;
                    found = true;
                }
                else {
                    bounds.Encapsulate(b);
                }
            }

            float extent = Mathf.Max(bounds.extents.x, bounds.extents.y, .005f);
            float dist = Mathf.Max(bounds.extents.z * 4f, extent * 4f);

            stageCamera.transform.position = bounds.center + Vector3.back * dist;
            stageCamera.orthographicSize = extent * framePaddingUsed;
            stageCamera.nearClipPlane = dist * .05f;
            stageCamera.farClipPlane = dist * 4f;

            followOffset = stageCamera.transform.position - content.transform.position;
        }

        // Re-fit to the content WHERE IT NOW IS. Call once the content has reached its settled
        // pose/scale: a widget that is posed or zoomed after Attach was framed at its old size,
        // so it renders correct but too small (or cropped) until this runs.
        public void Reframe() {
            Frame();
        }

        // Position only, and LATE — after whatever tween moved the content this frame.
        void LateUpdate() {

            if (!followContent || content == null || stageCamera == null) {
                return;
            }

            stageCamera.transform.position = content.transform.position + followOffset;
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
