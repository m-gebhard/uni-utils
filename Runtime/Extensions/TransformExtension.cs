using System.Collections;
using UnityEngine;

namespace UniUtils.Extensions
{
    /// <summary>
    /// Provides extension methods for the Transform class.
    /// </summary>
    public static class TransformExtension
    {
        /// <summary>
        /// Smoothly moves a Transform to match the position and rotation of a target Transform over a specified duration.
        /// </summary>
        /// <param name="target">The Transform to move.</param>
        /// <param name="destination">The target Transform to move towards.</param>
        /// <param name="duration">The duration of the movement in seconds. Defaults to 0.5 seconds.</param>
        /// <returns>An IEnumerator that can be used in a coroutine to perform the movement.</returns>
        /// <example>
        /// <code>
        /// StartCoroutine(transform.MoveToTargetTransform(targetTransform, 1.0f));
        /// </code>
        /// </example>
        public static IEnumerator MoveToTargetTransform(
            this Transform target,
            Transform destination,
            float duration = 0.5f
        )
        {
            float elapsed = 0f;

            Vector3 startPos = target.position;
            Quaternion startRot = target.rotation;

            Vector3 endPos = destination.position;
            Quaternion endRot = destination.rotation;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.SmoothStep(0, 1, elapsed / duration);

                target.position = Vector3.Lerp(startPos, endPos, t);
                target.rotation = Quaternion.Slerp(startRot, endRot, t);

                yield return null;
            }

            target.position = endPos;
            target.rotation = endRot;
        }

        /// <summary>
        /// Performs a raycast from the Transform's position towards a target position, with optional offsets and settings.
        /// </summary>
        /// <param name="transform">The Transform from which the raycast originates.</param>
        /// <param name="targetPosition">The position to raycast towards.</param>
        /// <param name="hit">The RaycastHit object that will store information about the hit.</param>
        /// <param name="includedLayers">The layers to include in the raycast.</param>
        /// <param name="localRayOffset">An optional offset applied to the ray's origin.</param>
        /// <param name="targetRayOffset">An optional offset applied to the ray's target position.</param>
        /// <param name="maxDistance">The maximum distance for the raycast. Defaults to float.MaxValue.</param>
        /// <param name="queryTriggerInteraction">Specifies whether the raycast should interact with trigger colliders.</param>
        /// <param name="drawRay">Whether to draw the ray in the scene for debugging purposes.</param>
        /// <returns>True if the raycast hits an object; otherwise, false.</returns>
        public static bool Raycast(
            this Transform transform,
            Vector3 targetPosition,
            out RaycastHit hit,
            LayerMask includedLayers,
            Vector3 localRayOffset = default,
            Vector3 targetRayOffset = default,
            float maxDistance = float.MaxValue,
            QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.Ignore,
            bool drawRay = false
        )
        {
            Vector3 origin = transform.position + localRayOffset;
            Vector3 direction = (targetPosition + targetRayOffset - origin).normalized;

            return DoRaycast(
                origin,
                direction,
                out hit,
                includedLayers,
                maxDistance,
                queryTriggerInteraction,
                drawRay
            );
        }

        /// <summary>
        /// Performs a raycast from the Transform's position in a specified direction, with optional offsets and settings.
        /// </summary>
        /// <param name="transform">The Transform from which the raycast originates.</param>
        /// <param name="direction">The direction in which to raycast.</param>
        /// <param name="hit">The RaycastHit object that will store information about the hit.</param>
        /// <param name="includedLayers">The layers to include in the raycast.</param>
        /// <param name="localRayOffset">An optional offset applied to the ray's origin.</param>
        /// <param name="maxDistance">The maximum distance for the raycast. Defaults to float.MaxValue.</param>
        /// <param name="queryTriggerInteraction">Specifies whether the raycast should interact with trigger colliders.</param>
        /// <param name="drawRay">Whether to draw the ray in the scene for debugging purposes.</param>
        /// <returns>True if the raycast hits an object; otherwise, false.</returns>
        public static bool RaycastDirection(
            this Transform transform,
            Vector3 direction,
            out RaycastHit hit,
            LayerMask includedLayers,
            Vector3 localRayOffset = default,
            float maxDistance = float.MaxValue,
            QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.Ignore,
            bool drawRay = false
        )
        {
            Vector3 origin = transform.position + localRayOffset;

            return DoRaycast(
                origin,
                direction.normalized,
                out hit,
                includedLayers,
                maxDistance,
                queryTriggerInteraction,
                drawRay
            );
        }

        /// <summary>
        /// Performs the actual raycast operation and optionally draws the ray for debugging purposes.
        /// </summary>
        /// <param name="origin">The origin point of the ray.</param>
        /// <param name="direction">The direction of the ray.</param>
        /// <param name="hit">The RaycastHit object that will store information about the hit.</param>
        /// <param name="includedLayers">The layers to include in the raycast.</param>
        /// <param name="maxDistance">The maximum distance for the raycast.</param>
        /// <param name="queryTriggerInteraction">Specifies whether the raycast should interact with trigger colliders.</param>
        /// <param name="drawRay">Whether to draw the ray in the scene for debugging purposes.</param>
        /// <returns>True if the raycast hits an object; otherwise, false.</returns>
        private static bool DoRaycast(
            Vector3 origin,
            Vector3 direction,
            out RaycastHit hit,
            LayerMask includedLayers,
            float maxDistance,
            QueryTriggerInteraction queryTriggerInteraction,
            bool drawRay
        )
        {
            bool isHit = Physics.Raycast(origin, direction, out hit, maxDistance, includedLayers,
                queryTriggerInteraction);

            if (drawRay)
            {
                Debug.DrawRay(origin, direction * maxDistance, isHit ? Color.green : Color.red);
            }

            return isHit;
        }
    }
}