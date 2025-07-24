using UnityEngine;

namespace UniUtils.Extensions
{
    /// <summary>
    /// Provides extension methods for the Vector3 class.
    /// </summary>
    public static class Vector3Extension
    {
        /// <summary>
        /// Creates a new vector with the specified components replaced by the provided values.
        /// </summary>
        /// <param name="vector">The original vector.</param>
        /// <param name="x">The new x-component value, or null to keep the original x-component.</param>
        /// <param name="y">The new y-component value, or null to keep the original y-component.</param>
        /// <param name="z">The new z-component value, or null to keep the original z-component.</param>
        /// <returns>A new vector with the specified components replaced by the provided values.</returns>
        /// <example>
        /// <code>
        /// Vector3 v = new Vector3(1, 2, 3);
        /// Vector3 modified = v.With(y: 10);  // Result: (1, 10, 3)
        /// </code>
        /// </example>
        public static Vector3 With(this Vector3 vector, float? x = null, float? y = null, float? z = null)
        {
            return new Vector3(
                x ?? vector.x,
                y ?? vector.y,
                z ?? vector.z
            );
        }

        /// <summary>
        /// Multiplies each component of the vector by the corresponding component of another vector.
        /// </summary>
        /// <param name="vector">The original vector.</param>
        /// <param name="other">The vector to multiply with.</param>
        /// <returns>A new vector with each component multiplied by the corresponding component of the other vector.</returns>
        /// <example>
        /// <code>
        /// Vector3 a = new Vector3(2, 3, 4);
        /// Vector3 b = new Vector3(5, 6, 7);
        /// Vector3 result = a.Multiply(b);  // Result: (10, 18, 28)
        /// </code>
        /// </example>
        public static Vector3 Multiply(this Vector3 vector, Vector3 other)
        {
            return new Vector3(vector.x * other.x, vector.y * other.y, vector.z * other.z);
        }

        /// <summary>
        /// Multiplies each component of the vector by a scalar value.
        /// </summary>
        /// <param name="vector">The original vector.</param>
        /// <param name="value">The scalar value to multiply with.</param>
        /// <returns>A new vector with each component multiplied by the scalar value.</returns>
        /// <example>
        /// <code>
        /// Vector3 v = new Vector3(1, 2, 3);
        /// Vector3 result = v.Multiply(2);  // Result: (2, 4, 6)
        /// </code>
        /// </example>
        public static Vector3 Multiply(this Vector3 vector, float value)
        {
            return new Vector3(vector.x * value, vector.y * value, vector.z * value);
        }

        /// <summary>
        /// Divides each component of the vector by the corresponding component of another vector.
        /// </summary>
        /// <param name="vector">The original vector.</param>
        /// <param name="other">The vector to divide by.</param>
        /// <returns>A new vector with each component divided by the corresponding component of the other vector, or the original component if the divisor is zero.</returns>
        /// <example>
        /// <code>
        /// Vector3 a = new Vector3(10, 20, 30);
        /// Vector3 b = new Vector3(2, 0, 5);
        /// Vector3 result = a.Divide(b);  // Result: (5, 20, 6) — y unchanged because divisor is 0
        /// </code>
        /// </example>
        public static Vector3 Divide(this Vector3 vector, Vector3 other)
        {
            return new Vector3(
                other.x != 0 ? vector.x / other.x : vector.x,
                other.y != 0 ? vector.y / other.y : vector.y,
                other.z != 0 ? vector.z / other.z : vector.z
            );
        }

        /// <summary>
        /// Divides each component of the vector by a scalar value.
        /// </summary>
        /// <param name="vector">The original vector.</param>
        /// <param name="value">The scalar value to divide by.</param>
        /// <returns>A new vector with each component divided by the scalar value, or the original component if the divisor is zero.</returns>
        /// <example>
        /// <code>
        /// Vector3 v = new Vector3(10, 20, 30);
        /// Vector3 result = v.Divide(2);  // Result: (5, 10, 15)
        /// </code>
        /// </example>
        public static Vector3 Divide(this Vector3 vector, float value)
        {
            bool isZero = value == 0;
            return new Vector3(
                !isZero ? vector.x / value : vector.x,
                !isZero ? vector.y / value : vector.y,
                !isZero ? vector.z / value : vector.z
            );
        }

        /// <summary>
        /// Calculates the distance between two vectors.
        /// </summary>
        /// <param name="vector">The original vector.</param>
        /// <param name="other">The vector to calculate the distance to.</param>
        /// <returns>The distance between the two vectors.</returns>
        /// <example>
        /// <code>
        /// Vector3 a = new Vector3(0, 0, 0);
        /// Vector3 b = new Vector3(3, 4, 0);
        /// float dist = a.DistanceTo(b);  // Result: 5
        /// </code>
        /// </example>
        public static float DistanceTo(this Vector3 vector, Vector3 other)
        {
            return Vector3.Distance(vector, other);
        }
    }
}