using System.Collections;
using UnityEngine;

namespace UniUtils.Extensions
{
    /// <summary>
    /// Provides extension methods for the Transform class.
    /// </summary>
    public static class TransformExtension
    {
        #region Movement & Rotation

        /// <summary>
        /// Smoothly moves a Transform to match the position of a target Transform over a specified duration.
        /// Can optionally move in local space.
        /// </summary>
        /// <param name="target">The Transform to move.</param>
        /// <param name="destination">The target Transform to move towards.</param>
        /// <param name="duration">The duration of the movement in seconds. Defaults to 0.5 seconds.</param>
        /// <param name="useLocal">If true, operates on localPosition instead of world position.</param>
        /// <returns>An IEnumerator to be used in a coroutine.</returns>
        /// <example>
        /// <code>
        /// StartCoroutine(enemyTransform.MoveToTransform(playerTransform, 1f, false));
        /// StartCoroutine(handTransform.MoveToTransform(itemSlotTransform, 0.5f));
        /// </code>
        /// </example>
        public static IEnumerator MoveToTransform(
            this Transform target,
            Transform destination,
            float duration = 0.5f,
            bool useLocal = true
        )
        {
            Vector3 startPos = useLocal ? target.localPosition : target.position;
            Vector3 endPos = useLocal ? destination.localPosition : destination.position;
            yield return LerpPosition(target, startPos, endPos, duration, useLocal);
        }

        /// <summary>
        /// Smoothly moves a Transform to a specified position over a given duration.
        /// Can optionally move in local space.
        /// </summary>
        /// <param name="target">The Transform to move.</param>
        /// <param name="endPosition">The target position to move towards.</param>
        /// <param name="duration">The duration of the movement in seconds. Defaults to 0.5 seconds.</param>
        /// <param name="useLocal">If true, operates on localPosition instead of world position.</param>
        /// <returns>An IEnumerator to be used in a coroutine.</returns>
        /// <example>
        /// <code>
        /// StartCoroutine(platformTransform.MoveToPosition(new Vector3(0f, 3f, 0f), 0.8f));
        /// </code>
        /// </example>
        public static IEnumerator MoveToPosition(
            this Transform target,
            Vector3 endPosition,
            float duration = 0.5f,
            bool useLocal = true
        )
        {
            Vector3 startPos = useLocal ? target.localPosition : target.position;
            yield return LerpPosition(target, startPos, endPosition, duration, useLocal);
        }

        /// <summary>
        /// Smoothly rotates a Transform to match another Transform's rotation over a specified duration.
        /// Can optionally rotate in local space.
        /// </summary>
        /// <param name="target">The Transform to rotate.</param>
        /// <param name="destination">The Transform whose rotation to match.</param>
        /// <param name="duration">The duration of the rotation in seconds. Defaults to 0.5 seconds.</param>
        /// <param name="useLocal">If true, operates on localRotation instead of world rotation.</param>
        /// <returns>An IEnumerator to be used in a coroutine.</returns>
        /// <example>
        /// <code>
        /// StartCoroutine(turretTransform.RotateToTransform(enemyTransform, 0.7f, false));
        /// </code>
        /// </example>
        public static IEnumerator RotateToTransform(
            this Transform target,
            Transform destination,
            float duration = 0.5f,
            bool useLocal = true
        )
        {
            Quaternion startRot = useLocal ? target.localRotation : target.rotation;
            Quaternion endRot = useLocal ? destination.localRotation : destination.rotation;
            yield return LerpRotation(target, startRot, endRot, duration, useLocal);
        }

        /// <summary>
        /// Smoothly rotates a Transform to a specified Quaternion rotation over a given duration.
        /// Can optionally rotate in local space.
        /// </summary>
        /// <param name="target">The Transform to rotate.</param>
        /// <param name="endRot">The target rotation as a Quaternion.</param>
        /// <param name="duration">The duration of the rotation in seconds. Defaults to 0.5 seconds.</param>
        /// <param name="useLocal">If true, operates on localRotation instead of world rotation.</param>
        /// <returns>An IEnumerator to be used in a coroutine.</returns>
        /// <example>
        /// <code>
        /// StartCoroutine(propellerTransform.RotateToRotation(Quaternion.Euler(0f, 0f, 360f), 2f, false));
        /// StartCoroutine(cameraTransform.RotateToRotation(Quaternion.Euler(15f, 0f, 0f), 1f, true));
        /// </code>
        /// </example>
        public static IEnumerator RotateToRotation(
            this Transform target,
            Quaternion endRot,
            float duration = 0.5f,
            bool useLocal = true
        )
        {
            Quaternion startRot = useLocal ? target.localRotation : target.rotation;
            yield return LerpRotation(target, startRot, endRot, duration, useLocal);
        }

        /// <summary>
        /// Smoothly moves a Transform to match the position and rotation of a target Transform over a specified duration.
        /// Can optionally move in local space.
        /// </summary>
        /// <param name="target">The Transform to move.</param>
        /// <param name="destination">The target Transform to move towards.</param>
        /// <param name="duration">The duration of the movement in seconds. Defaults to 0.5 seconds.</param>
        /// <param name="useLocal">If true, operates on localPosition/localRotation instead of world position/rotation.</param>
        /// <returns>An IEnumerator to be used in a coroutine.</returns>
        /// <example>
        /// <code>
        /// StartCoroutine(characterTransform.MoveAndRotateToTransform(spawnTransform, 1.2f));
        /// </code>
        /// </example>
        public static IEnumerator MoveAndRotateToTransform(
            this Transform target,
            Transform destination,
            float duration = 0.5f,
            bool useLocal = true
        )
        {
            Vector3 startPos = useLocal ? target.localPosition : target.position;
            Vector3 endPos = useLocal ? destination.localPosition : destination.position;
            Quaternion startRot = useLocal ? target.localRotation : target.rotation;
            Quaternion endRot = useLocal ? destination.localRotation : destination.rotation;
            yield return LerpPositionAndRotation(target, startPos, endPos, startRot, endRot, duration, useLocal);
        }

        /// <summary>
        /// Smoothly interpolates the position of a Transform from start to end over a duration.
        /// Can operate in local or world space.
        /// </summary>
        /// <param name="target">The Transform to move.</param>
        /// <param name="start">Starting position.</param>
        /// <param name="end">Target position.</param>
        /// <param name="duration">Duration of the movement.</param>
        /// <param name="useLocal">If true, uses localPosition, otherwise position.</param>
        /// <returns>An IEnumerator to be used in a coroutine.</returns>
        private static IEnumerator LerpPosition(Transform target,
            Vector3 start,
            Vector3 end,
            float duration,
            bool useLocal)
        {
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);
                if (useLocal) target.localPosition = Vector3.Lerp(start, end, t);
                else target.position = Vector3.Lerp(start, end, t);
                yield return null;
            }

            if (useLocal) target.localPosition = end;
            else target.position = end;
        }

        /// <summary>
        /// Smoothly interpolates the rotation of a Transform from start to end over a duration.
        /// Can operate in local or world space.
        /// </summary>
        /// <param name="target">The Transform to rotate.</param>
        /// <param name="start">Starting rotation.</param>
        /// <param name="end">Target rotation.</param>
        /// <param name="duration">Duration of the rotation.</param>
        /// <param name="useLocal">If true, uses localRotation, otherwise rotation.</param>
        /// <returns>An IEnumerator to be used in a coroutine.</returns>
        private static IEnumerator LerpRotation(Transform target,
            Quaternion start,
            Quaternion end,
            float duration,
            bool useLocal)
        {
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);
                if (useLocal) target.localRotation = Quaternion.Slerp(start, end, t);
                else target.rotation = Quaternion.Slerp(start, end, t);
                yield return null;
            }

            if (useLocal) target.localRotation = end;
            else target.rotation = end;
        }

        /// <summary>
        /// Smoothly interpolates both position and rotation of a Transform simultaneously over a duration.
        /// Can operate in local or world space.
        /// </summary>
        /// <param name="target">The Transform to move and rotate.</param>
        /// <param name="startPos">Starting position.</param>
        /// <param name="endPos">Target position.</param>
        /// <param name="startRot">Starting rotation.</param>
        /// <param name="endRot">Target rotation.</param>
        /// <param name="duration">Duration of the movement and rotation.</param>
        /// <param name="useLocal">If true, uses local space, otherwise world space.</param>
        /// <returns>An IEnumerator to be used in a coroutine.</returns>
        private static IEnumerator LerpPositionAndRotation(
            Transform target,
            Vector3 startPos,
            Vector3 endPos,
            Quaternion startRot,
            Quaternion endRot,
            float duration,
            bool useLocal
        )
        {
            float elapsed = 0f;
            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.SmoothStep(0f, 1f, elapsed / duration);
                if (useLocal)
                {
                    target.localPosition = Vector3.Lerp(startPos, endPos, t);
                    target.localRotation = Quaternion.Slerp(startRot, endRot, t);
                }
                else
                {
                    target.position = Vector3.Lerp(startPos, endPos, t);
                    target.rotation = Quaternion.Slerp(startRot, endRot, t);
                }

                yield return null;
            }

            if (useLocal)
            {
                target.localPosition = endPos;
                target.localRotation = endRot;
            }
            else
            {
                target.position = endPos;
                target.rotation = endRot;
            }
        }

        #endregion

        #region Raycast

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
        /// <example>
        /// <code>
        /// RaycastHit hit;
        /// if(transform.Raycast(target.position, out hit, LayerMask.GetMask("Default")))
        /// {
        ///     Debug.Log("Hit object: " + hit.collider.name);
        /// }
        /// </code>
        /// </example>
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
        /// <example>
        /// <code>
        /// RaycastHit hit;
        /// Vector3 direction = transform.up;
        /// if(transform.RaycastDirection(direction, out hit, LayerMask.GetMask("Default"), maxDistance: 10f))
        /// {
        ///     Debug.Log("Hit object in up direction: " + hit.collider.name);
        /// }
        /// </code>
        /// </example>
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

        #endregion
    }
}