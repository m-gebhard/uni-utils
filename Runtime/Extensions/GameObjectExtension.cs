using System.Collections.Generic;
using UnityEngine;

namespace UniUtils.Extensions
{
    /// <summary>
    /// Provides extension methods for the GameObject class.
    /// </summary>
    ///
    public static class GameObjectExtension
    {
        /// <summary>
        /// Retrieves a component of the specified type from the GameObject.
        /// If the component does not exist, it adds a new one to the GameObject.
        /// </summary>
        /// <typeparam name="T">The type of the component to retrieve or add.</typeparam>
        /// <param name="obj">The GameObject to retrieve or add the component to.</param>
        /// <returns>The existing or newly added component of the specified type.</returns>
        /// <example>
        /// <code>
        /// // Usage example:
        /// GameObject obj = new GameObject("MyObject");
        /// Rigidbody rb = obj.GetOrAddComponent&lt;Rigidbody&gt;();
        /// </code>
        /// </example>
        public static T GetOrAddComponent<T>(this GameObject obj) where T : Component
        {
            T component = obj.GetComponent<T>();
            return !component
                ? obj.AddComponent<T>()
                : component;
        }

        /// <summary>
        /// Recursively sets the <see cref="UnityEngine.GameObject.layer"/> of this GameObject and all its children,
        /// returning a dictionary of each GameObject to its original layer mask.
        /// </summary>
        /// <param name="obj">The root GameObject whose layer (and its descendants) will be set.</param>
        /// <param name="layerMask">
        /// The target layer mask.  Internally, <see cref="UnityEngine.GameObject.layer"/> is an int index,
        /// so this method uses <c>layerMask.value</c> as the new layer index.
        /// </param>
        /// <param name="includeInactive">
        /// If <c>true</c>, will include inactive children; otherwise only active ones.
        /// </param>
        /// <returns>
        /// A <see cref="Dictionary{GameObject, LayerMask}"/> mapping each affected GameObject
        /// to its original <see cref="UnityEngine.GameObject.layer"/> (as a <see cref="UnityEngine.LayerMask"/>).
        /// </returns>
        public static Dictionary<GameObject, LayerMask> SetLayersRecursively(
            this GameObject obj,
            LayerMask layerMask,
            bool includeInactive = true
        )
        {
            Dictionary<GameObject, LayerMask> changedObjects = new Dictionary<GameObject, LayerMask>();
            Transform[] all = obj.GetComponentsInChildren<Transform>(includeInactive);

            foreach (Transform t in all)
            {
                GameObject go = t.gameObject;
                LayerMask original = go.layer;
                changedObjects[go] = original;
                go.layer = layerMask.value;
            }

            return changedObjects;
        }
    }
}