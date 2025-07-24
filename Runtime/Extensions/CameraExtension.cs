using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;

namespace UniUtils.Extensions
{
    /// <summary>
    /// Provides extension methods for the Camera.
    /// </summary>
    public static class CameraExtension
    {
        /// <summary>
        /// Captures the current camera view and invokes a callback with the image data as a byte array.
        /// </summary>
        /// <param name="camera">The Camera instance to capture the view from.</param>
        /// <param name="callback">The callback to invoke with the captured image data.</param>
        /// <param name="excludedLayers">The layers to exclude from the camera capture. Defaults to none.</param>
        /// <example>
        /// <code>
        /// // Usage example:
        /// void Start()
        /// {
        ///     Camera mainCam = Camera.main;
        ///     mainCam.Capture(OnCaptured);
        /// }
        ///
        /// void OnCaptured(byte[] imageData)
        /// {
        ///     Debug.Log("Captured image byte size: " + imageData.Length);
        ///     // You can now save the image or use it however you want
        /// }
        /// </code>
        /// </example>
        public static void Capture(this Camera camera, Action<byte[]> callback, LayerMask excludedLayers = default)
        {
            MonoBehaviour mono = camera.gameObject.GetComponent<MonoBehaviour>();
            if (!mono)
            {
                nameof(CameraExtension).LogError(
                    "Camera must be attached to a GameObject with a MonoBehaviour component to use Capture.");
                return;
            }

            mono.StartCoroutine(RecordFrame(camera, callback, excludedLayers));
        }

        /// <summary>
        /// Performs a raycast from the camera's screen point and returns whether a hit occurred.
        /// </summary>
        /// <param name="camera">The Camera instance to perform the raycast from.</param>
        /// <param name="includedLayers">The layers to include in the raycast.</param>
        /// <param name="hit">The RaycastHit object containing information about the hit.</param>
        /// <param name="position">The screen position to cast the ray from. Defaults to the center of the screen.</param>
        /// <param name="maxDistance">The maximum distance for the raycast. Defaults to infinity.</param>
        /// <param name="queryTriggerInteraction">Specifies whether to include trigger colliders in the raycast.</param>
        /// <param name="drawRay">Indicates whether to draw the ray in the scene view for debugging purposes.</param>
        /// <returns><c>true</c> if the raycast hit an object; otherwise, <c>false</c>.</returns>
        /// <example>
        /// Example usage:
        /// <code>
        /// RaycastHit hit;
        /// bool didHit = Camera.main.Raycast(
        ///     includedLayers: LayerMask.GetMask("Default"),
        ///     hit: out hit,
        ///     drawRay: true
        /// );
        ///
        /// if (didHit)
        /// {
        ///     Debug.Log($"Hit: {hit.collider.name}");
        /// }
        /// </code>
        /// </example>
        public static bool Raycast(
            this Camera camera,
            out RaycastHit hit,
            LayerMask includedLayers,
            Vector3? position = null,
            float maxDistance = float.MaxValue,
            QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.Ignore,
            bool drawRay = false
        )
        {
            position ??= new Vector3(Screen.width / 2f, Screen.height / 2f);

            Ray ray = camera.ScreenPointToRay(position!.Value);
            bool isHit = Physics.Raycast(ray, out hit, maxDistance, includedLayers, queryTriggerInteraction);
            if (drawRay) ray.Draw(isHit ? Color.green : Color.red, maxDistance, 0.1f);

            return isHit;
        }

        /// <summary>
        /// Coroutine that captures the current frame of the camera and invokes the callback with the image data.
        /// </summary>
        /// <param name="camera">The Camera instance to capture the view from.</param>
        /// <param name="callback">The callback to invoke with the captured image data.</param>
        /// <param name="excludedLayers">The layers to exclude from the camera capture.</param>
        /// <returns>An IEnumerator for the coroutine.</returns>
        private static IEnumerator RecordFrame(Camera camera, Action<byte[]> callback, LayerMask excludedLayers)
        {
            // Stores the original culling mask of the camera
            int originalMask = camera.cullingMask;
            camera.cullingMask = originalMask & ~excludedLayers;

            bool captured = false;
            byte[] pngData = null;

            // Subscribes to the endCameraRendering event to capture the frame
            RenderPipelineManager.endCameraRendering += OnEndCameraRendering;

            // Waits for the end of the frame before capturing
            yield return new WaitForEndOfFrame();

            // Restores the original culling mask of the camera
            camera.cullingMask = originalMask;

            // Invokes the callback with the captured image data
            callback(pngData ?? Array.Empty<byte>());
            yield break;

            void OnEndCameraRendering(ScriptableRenderContext ctx, Camera cam)
            {
                // Ensures the event is triggered only for the specified camera and only once
                if (cam != camera || captured) return;
                captured = true;

                // Captures the rendered frame as a texture and encodes it to PNG format
                RenderTexture activeRT = RenderTexture.active;
                Texture2D tex = new(Screen.width, Screen.height, TextureFormat.RGB24, false);
                tex.ReadPixels(new Rect(0, 0, Screen.width, Screen.height), 0, 0);
                tex.Apply();

                pngData = tex.EncodeToPNG();

                // Cleans up resources and unsubscribes from the event
                UnityEngine.Object.Destroy(tex);
                RenderPipelineManager.endCameraRendering -= OnEndCameraRendering;
                RenderTexture.active = activeRT;
            }
        }
    }
}