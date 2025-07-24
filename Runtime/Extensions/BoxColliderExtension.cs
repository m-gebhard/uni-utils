using UnityEngine;

namespace UniUtils.Extensions
{
    /// <summary>
    /// Provides extension methods for the BoxCollider class.
    /// </summary>
    public static class BoxColliderExtension
    {
        /// <summary>
        /// Returns a random point within the bounds of the BoxCollider.
        /// </summary>
        /// <param name="includeY">If true, includes the Y coordinate in the random point;
        /// otherwise, uses the Y position of the collider's transform.</param>
        /// <returns>A random point within the bounds of the BoxCollider.</returns>
        /// <example>
        /// <code>
        /// // Usage example:
        /// void Example(BoxCollider boxCollider)
        /// {
        ///     // Get a random point inside the collider ignoring Y (using collider's Y position)
        ///     Vector3 point = boxCollider.RandomPoint();
        ///
        ///     // Get a random point inside the collider including Y coordinate
        ///     Vector3 pointWithY = boxCollider.RandomPoint(true);
        ///
        ///     Debug.Log("Random point (no Y): " + point);
        ///     Debug.Log("Random point (with Y): " + pointWithY);
        /// }
        /// </code>
        /// </example>
        public static Vector3 RandomPoint(this BoxCollider collider, bool includeY = false)
        {
            Vector3 boundsMin = collider.bounds.min;
            Vector3 boundsMax = collider.bounds.max;

            float x = Random.Range(boundsMin.x, boundsMax.x);
            float y = includeY
                ? Random.Range(boundsMin.y, boundsMax.y)
                : collider.transform.position.y;
            float z = Random.Range(boundsMin.z, boundsMax.z);

            return new Vector3(x, y, z);
        }
    }
}