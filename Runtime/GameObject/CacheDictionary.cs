using System;
using System.Collections.Generic;
using UniUtils.GameObjects;

namespace UniUtils.Data
{
    /// <summary>
    /// Represents a dictionary that caches items and provides methods to manage the cache.
    /// </summary>
    /// <typeparam name="TKey">The type of the key used in the dictionary.</typeparam>
    /// <typeparam name="T">The type of the value stored in the dictionary.</typeparam>
    /// <example>
    /// Example of using a cache to store surface types based on a ground transform:
    /// <code>
    /// public class SurfaceType
    /// {
    ///     public string type;
    /// }
    ///
    /// public class GroundSurfaceCache : CacheDictionary&lt;Transform, string&gt; { }
    ///
    /// public class Player
    /// {
    ///     public void OnFootstep(Transform groundTransform)
    ///     {
    ///         string surfaceType = GroundSurfaceCache.Instance.GetOrAdd(
    ///             groundTransform,
    ///             transform =&gt; transform.GetComponent&lt;SurfaceType&gt;()?.type,
    ///             discardNullValue: true
    ///         );
    ///         Debug.Log("Footstep on surface: " + surfaceType);
    ///     }
    /// }
    /// </code>
    /// </example>
    public abstract class CacheDictionary<TKey, T> : EphemeralSingleton<CacheDictionary<TKey, T>>
    {
        /// <summary>
        /// The internal dictionary used for caching items.
        /// </summary>
        private readonly Dictionary<TKey, T> cached = new();

        /// <summary>
        /// Retrieves a value from the cache or adds it using the specified factory function if it does not exist.
        /// </summary>
        /// <param name="key">The key to retrieve or add.</param>
        /// <param name="valueFactory">The function used to create the value if it does not exist.</param>
        /// <param name="discardNullValue">Indicates whether to discard null values. Defaults to <c>true</c>.</param>
        /// <returns>The value associated with the key.</returns>
        public T GetOrAdd(
            TKey key,
            Func<TKey, T> valueFactory,
            bool discardNullValue = true
        )
        {
            if (!TryGetValue(key, out T value))
            {
                value = valueFactory(key);

                if (value != null || !discardNullValue)
                {
                    cached[key] = value;
                }
            }

            return value;
        }

        /// <summary>
        /// Adds or updates an item in the cache.
        /// </summary>
        /// <param name="key">The key of the item to add or update.</param>
        /// <param name="value">The value to associate with the key.</param>
        public void AddCachedItem(TKey key, T value)
        {
            cached[key] = value;
        }

        /// <summary>
        /// Removes an item from the cache.
        /// </summary>
        /// <param name="key">The key of the item to remove.</param>
        public void RemoveCachedItem(TKey key)
        {
            cached.Remove(key);
        }

        /// <summary>
        /// Attempts to retrieve a value from the cache.
        /// </summary>
        /// <param name="key">The key to retrieve.</param>
        /// <param name="value">When this method returns, contains the value associated with the key if found; otherwise, the default value for the type of the value parameter.</param>
        /// <returns><c>true</c> if the key exists in the cache; otherwise, <c>false</c>.</returns>
        public bool TryGetValue(TKey key, out T value)
        {
            return cached.TryGetValue(key, out value);
        }

        /// <summary>
        /// Determines whether the cache contains the specified key.
        /// </summary>
        /// <param name="key">The key to check for existence.</param>
        /// <returns><c>true</c> if the key exists in the cache; otherwise, <c>false</c>.</returns>
        public bool ContainsKey(TKey key)
        {
            return cached.ContainsKey(key);
        }

        /// <summary>
        /// Clears all items from the cache.
        /// </summary>
        public void ClearCache()
        {
            cached.Clear();
        }
    }
}