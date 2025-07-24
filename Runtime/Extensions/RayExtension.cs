using UnityEngine;

namespace UniUtils.Extensions
{
    /// <summary>
    /// Provides extensions for Rays.
    /// </summary>
    public static class RayExtension
    {
        /// <summary>
        /// Draws a ray in the Unity editor for debugging purposes.
        /// </summary>
        /// <param name="ray">The ray to draw.</param>
        /// <param name="color">The color of the ray.</param>
        /// <param name="length">The length of the ray (default is 100f).</param>
        /// <param name="duration">The duration the ray will be visible in seconds (default is 0.25f).</param>
        /// <example>
        /// <code>
        /// Ray ray = new Ray(transform.position, transform.forward);
        /// ray.Draw(Color.red, 50f, 1f);
        /// </code>
        /// </example>
        public static void Draw(this Ray ray, Color color, float length = 100f, float duration = 0.25f)
        {
#if UNITY_EDITOR
            Debug.DrawRay(ray.origin, ray.direction * length, color, duration);
#endif
        }
    }
}