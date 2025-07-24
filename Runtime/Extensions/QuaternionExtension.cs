using UnityEngine;

namespace UniUtils.Extensions
{
    /// <summary>
    /// Provides extension methods for the Quaternion class.
    /// </summary>
    public static class QuaternionExtension
    {
        /// <summary>
        /// Returns a new Quaternion with the specified Euler angles applied.
        /// </summary>
        /// <param name="quaternion">The original Quaternion.</param>
        /// <param name="x">The x component of the Euler angles to apply. Defaults to 0 if null.</param>
        /// <param name="y">The y component of the Euler angles to apply. Defaults to 0 if null.</param>
        /// <param name="z">The z component of the Euler angles to apply. Defaults to 0 if null.</param>
        /// <returns>A new Quaternion with the specified Euler angles applied.</returns>
        /// <example>
        /// <code>
        /// Quaternion original = Quaternion.Euler(10, 20, 30);
        /// Quaternion modified = original.With(y: 45);
        /// Debug.Log(modified.eulerAngles); // Output: (10, 45, 30)
        /// </code>
        /// </example>
        public static Quaternion With(this Quaternion quaternion, float? x = null, float? y = null, float? z = null)
        {
            Vector3 euler = quaternion.eulerAngles;

            if (x.HasValue) euler.x = x.Value;
            if (y.HasValue) euler.y = y.Value;
            if (z.HasValue) euler.z = z.Value;

            return Quaternion.Euler(euler);
        }
    }
}