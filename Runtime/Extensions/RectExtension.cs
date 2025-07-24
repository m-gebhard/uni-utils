using UnityEngine;

namespace UniUtils.Extensions
{
    public static class RectExtension
    {
        /// <summary>
        /// Converts a Rect from local space to world space.
        /// </summary>
        /// <param name="r">The Rect to convert.</param>
        /// <param name="transform">The Transform to use for conversion.</param>
        /// <returns>A Rect in world space.</returns>
        /// <example>
        /// <code>
        /// Rect localRect = new Rect(0, 0, 1, 1);
        /// Rect worldRect = localRect.ToWorldSpace(transform);
        /// </code>
        /// </example>
        public static Rect ToWorldSpace(this Rect r, Transform transform)
        {
            return new Rect
            {
                min = transform.TransformPoint(r.min),
                max = transform.TransformPoint(r.max),
            };
        }

        /// <summary>
        /// Converts a Rect from world space to local space.
        /// </summary>
        /// <param name="r">The Rect to convert.</param>
        /// <param name="transform">The Transform to use for conversion.</param>
        /// <returns>A Rect in local space.</returns>
        /// <example>
        /// <code>
        /// Rect worldRect = new Rect(0, 0, 1, 1);
        /// Rect localRect = worldRect.ToLocalSpace(transform);
        /// </code>
        /// </example>
        public static Rect ToLocalSpace(this Rect r, Transform transform)
        {
            return new Rect
            {
                min = transform.InverseTransformPoint(r.min),
                max = transform.InverseTransformPoint(r.max),
            };
        }
    }
}