using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Engine.Cameras {

    /// <summary>
    /// Builds the URP camera overlay stack at runtime after all scenes are loaded.
    /// Attach to the same GameObject as CameraClear (the Base camera) in GameUISceneRoot.
    /// </summary>
    [RequireComponent(typeof(Camera))]
    public class URPCameraStackSetup : MonoBehaviour {

        // Overlay cameras are discovered by name and added in this depth order.
        // Populate from the Inspector or leave empty to use auto-discovery by name.
        [Tooltip("Optional: manually assign overlay cameras in the order they should composite (lowest depth first).")]
        public List<Camera> overlayCamerasOrdered = new List<Camera>();

        private void Start() {
            BuildStack();
        }

        /// <summary>
        /// Discovers and registers all overlay cameras onto this Base camera's stack.
        /// Safe to call again if scenes change at runtime.
        /// </summary>
        public void BuildStack() {

            var baseCameraData = GetComponent<UniversalAdditionalCameraData>();
            if (baseCameraData == null) {
                Debug.LogError("[URPCameraStackSetup] No UniversalAdditionalCameraData on Base camera.", this);
                return;
            }

            // cameraStack is only available on Base cameras — silently skip Overlay cameras.
            if (baseCameraData.renderType != CameraRenderType.Base) {
                return;
            }

            baseCameraData.cameraStack.Clear();

            if (overlayCamerasOrdered != null && overlayCamerasOrdered.Count > 0) {
                BuildFromManualList(baseCameraData);
            } else {
                BuildFromDepthSort(baseCameraData);
            }

            Debug.Log($"[URPCameraStackSetup] Built camera stack with {baseCameraData.cameraStack.Count} overlay cameras.", this);
        }

        private void BuildFromManualList(UniversalAdditionalCameraData baseCameraData) {

            foreach (Camera cam in overlayCamerasOrdered) {
                if (cam == null) continue;
                EnsureOverlay(cam);
                baseCameraData.cameraStack.Add(cam);
            }
        }

        private void BuildFromDepthSort(UniversalAdditionalCameraData baseCameraData) {

            // Collect every Camera across all loaded scenes including inactive, excluding Base.
            Camera[] all = FindObjectsByType<Camera>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            Camera baseCamera = GetComponent<Camera>();
            var overlays = new List<Camera>(all.Length);

            foreach (Camera cam in all) {
                if (cam == baseCamera) continue;

                var data = cam.GetComponent<UniversalAdditionalCameraData>();
                if (data == null) continue;

                // Force Overlay — handles any camera the converter or previous pass missed.
                if (data.renderType != CameraRenderType.Overlay) {
                    data.renderType = CameraRenderType.Overlay;
                    Debug.Log($"[URPCameraStackSetup] Forced '{cam.name}' (depth {cam.depth}) to Overlay.", cam);
                }

                // Cameras that used clearFlags Nothing depend on the depth buffer written
                // by earlier cameras — URP overlays default clearDepth to true, which
                // would draw them over everything instead.
                SetClearDepth(data, cam.clearFlags != CameraClearFlags.Nothing);

                overlays.Add(cam);
            }

            // Sort ascending by depth — URP composites in list order, lowest depth first.
            overlays.Sort((a, b) => a.depth.CompareTo(b.depth));

            foreach (Camera cam in overlays) {
                baseCameraData.cameraStack.Add(cam);
            }
        }

        /// <summary>Ensures the camera is marked as Overlay — safe to call repeatedly.</summary>
        private static void EnsureOverlay(Camera cam) {
            var data = cam.GetComponent<UniversalAdditionalCameraData>();
            if (data != null) {
                if (data.renderType != CameraRenderType.Overlay) {
                    data.renderType = CameraRenderType.Overlay;
                }
                SetClearDepth(data, cam.clearFlags != CameraClearFlags.Nothing);
            }
        }

        // UniversalAdditionalCameraData.clearDepth has no setter — write the serialized
        // m_ClearDepth field via reflection.
        private static System.Reflection.FieldInfo clearDepthField;

        private static void SetClearDepth(UniversalAdditionalCameraData data, bool value) {
            if (clearDepthField == null) {
                clearDepthField = typeof(UniversalAdditionalCameraData).GetField(
                    "m_ClearDepth",
                    System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic);
            }
            if (clearDepthField != null) {
                clearDepthField.SetValue(data, value);
            }
        }
    }
}
