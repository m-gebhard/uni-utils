using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UniUtils.Extensions;

namespace UniUtils.GameObjects
{
    /// <summary>
    /// A generic object pool for managing reusable objects of type T.
    /// </summary>
    /// <typeparam name="T">The type of objects to pool, which must be a Component.</typeparam>
    /// <example>
    /// <code>
    /// public class Bullet : MonoBehaviour
    /// {
    ///     // Bullet logic here
    /// }
    ///
    /// public class BulletPool : GenericObjectPool&lt;Bullet&gt;
    /// {
    ///     // Optionally add pool-specific logic here
    ///     // The Pool has to be placed in the scene. Within the inspector, you have to assign it's prefab.
    /// }
    ///
    /// public class PlayerShooter : MonoBehaviour
    /// {
    ///     private void Shoot()
    ///     {
    ///         Bullet bullet = BulletPool.Instance.Get();
    ///         bullet.transform.position = transform.position;
    ///         bullet.gameObject.SetActive(true);
    ///
    ///         // Setup bullet, e.g., velocity, direction...
    ///     }
    ///
    ///     private void OnBulletFinished(Bullet bullet)
    ///     {
    ///         BulletPool.Instance.ReturnToPool(bullet);
    ///     }
    /// }
    /// </code>
    /// </example>
    public abstract class GenericObjectPool<T> : EphemeralSingleton<GenericObjectPool<T>> where T : Component
    {
        #region Fields

        [Header("Prefab")]
        [Tooltip("The prefab to instantiate for the pool.")]
        [SerializeField] protected T prefab;
        [Header("Pre-warm")]
        [Tooltip("The number of objects to prewarm in the pool.")]
        [SerializeField] protected int prewarmCount;
        [Tooltip("Whether to awake objects immediately upon creation.")]
        [SerializeField] protected bool awakeObjectsOnCreation;
        [Tooltip("The number of objects to activate and deactivate in each batch during prewarming.")]
        [SerializeField] protected int awakeObjectsBatchSize = 50;
        [Header("Pooling")]
        [Tooltip("The maximum number of objects that can be pooled. If exceeded, new objects will not be added.")]
        [SerializeField] protected int maxPoolSize = 100;
        [Tooltip("Whether to activate pooled objects when retrieved from the pool.")]
        [SerializeField] protected bool activatePooledObjects = true;
        [Tooltip(
            "Whether to allow recycling of old objects when the pool is full. Use OnObjectRecycled if you need to know of recycled objects.")]
        [SerializeField] protected bool allowObjectRecycling = true;

        public IReadOnlyCollection<T> ActiveObjects => activeObjects;
        public int AvailableSlots => maxPoolSize - allObjects.Count;

        public event Action<T> OnObjectPooled;
        public event Action<T> OnObjectReturned;
        public event Action<T> OnObjectRecycled;
        public event Action<List<T>> OnPrewarmCompleted;

        protected readonly Queue<T> readyToUseObjects = new();
        protected readonly HashSet<T> allObjects = new();
        protected readonly LinkedList<T> activeObjects = new();

        #endregion

        /// <summary>
        /// Initializes the pool and prewarms objects if specified.
        /// </summary>
        protected override void Awake()
        {
            base.Awake();
            AddObjects(prewarmCount);

            if (awakeObjectsOnCreation)
                StartCoroutine(PrewarmCoroutine(awakeObjectsBatchSize));
        }

        /// <summary>
        /// Prewarms the object pool by activating and deactivating objects in batches.
        /// This ensures that objects are properly initialized before use.
        /// </summary>
        /// <param name="batchSize">
        /// The number of objects to activate and deactivate in each batch. Defaults to 50.
        /// </param>
        /// <returns>
        /// An enumerator that performs the prewarming operation over multiple frames.
        /// </returns>
        protected virtual IEnumerator PrewarmCoroutine(int batchSize = 50)
        {
            List<T> snapshot = readyToUseObjects.ToList();
            int total = snapshot.Count;

            for (int i = 0; i < total; i += batchSize)
            {
                int end = Math.Min(i + batchSize, total);

                // Activate this batch
                for (int j = i; j < end; j++)
                    snapshot[j].gameObject.SetActive(true);

                yield return null;

                // Deactivate this batch
                for (int j = i; j < end; j++)
                {
                    T obj = snapshot[j];
                    if (readyToUseObjects.Contains(obj))
                        obj.gameObject.SetActive(false);
                }
            }

            OnPrewarmCompleted?.Invoke(readyToUseObjects.ToList());
        }

        /// <summary>
        /// Retrieves an object from the pool. If no objects are available, attempts to create or recycle one.
        /// </summary>
        /// <returns>
        /// A pooled object of type <typeparamref name="T"/> or <c>null</c> if the operation fails.
        /// </returns>
        public virtual T Get()
        {
            T obj = GetOrCreateObject(out bool wasNew);

            if (obj == null)
                return null;

            NotifyPooled(obj, wasNew);
            return obj;
        }

        /// <summary>
        /// Retrieves an object from the pool and sets its position, rotation, and parent transform.
        /// If no objects are available, attempts to create or recycle one.
        /// </summary>
        /// <param name="position">The position to set for the object.</param>
        /// <param name="rotation">The rotation to set for the object.</param>
        /// <param name="parent">The parent transform to assign to the object. Defaults to <c>null</c>.</param>
        /// <returns>
        /// A pooled object of type <typeparamref name="T"/> or <c>null</c> if the operation fails.
        /// </returns>
        public virtual T Get(Vector3 position, Quaternion rotation, Transform parent = null)
        {
            T obj = GetOrCreateObject(out bool wasNew);

            if (obj == null)
                return null;

            if (parent != null)
                obj.transform.SetParent(parent, false);

            obj.transform.SetPositionAndRotation(position, rotation);

            NotifyPooled(obj, wasNew);

            return obj;
        }

        /// <summary>
        /// Returns an object to the pool.
        /// </summary>
        /// <param name="objectToReturn">The object to return to the pool.</param>
        public virtual void ReturnToPool(T objectToReturn)
        {
            objectToReturn.transform.SetParent(transform, true);
            objectToReturn.gameObject.SetActive(false);
            readyToUseObjects.Enqueue(objectToReturn);

            activeObjects.Remove(objectToReturn);
            OnObjectReturned?.Invoke(objectToReturn);

            if (objectToReturn is IPoolable poolable)
                poolable.OnReturned();
        }

        #region Helpers

        /// <summary>
        /// Retrieves an object from the pool or creates/recycles one if necessary.
        /// </summary>
        /// <param name="wasNew">
        /// Outputs whether the object was newly created (<c>true</c>) or retrieved from the pool (<c>false</c>).
        /// </param>
        /// <returns>
        /// A pooled object of type <typeparamref name="T"/> or <c>null</c> if the operation fails.
        /// </returns>
        protected virtual T GetOrCreateObject(out bool wasNew)
        {
            wasNew = false;

            if (readyToUseObjects.Count == 0)
            {
                if (AvailableSlots > 0)
                {
                    AddObjects(1);
                    wasNew = true;
                }
                else if (activeObjects.Count > 0 && allowObjectRecycling)
                {
                    T oldestActive = activeObjects.First.Value;
                    activeObjects.RemoveFirst();
                    oldestActive.gameObject.SetActive(false);
                    oldestActive.transform.SetParent(transform, false);
                    readyToUseObjects.Enqueue(oldestActive);

                    if (oldestActive is IPoolable poolableRecycled)
                        poolableRecycled.OnRecycled();

                    OnObjectRecycled?.Invoke(oldestActive);
                }
            }

            if (readyToUseObjects.Count == 0)
            {
                this.LogError("No available objects and failed to recycle any. Returning null.");
                return null;
            }

            T obj = readyToUseObjects.Dequeue();
            obj.gameObject.SetActive(activatePooledObjects);
            activeObjects.AddLast(obj);

            return obj;
        }

        /// <summary>
        /// Notifies that an object has been pooled and invokes relevant events.
        /// </summary>
        /// <param name="obj">The object that has been pooled.</param>
        /// <param name="wasNew">
        /// Indicates whether the object was newly created (<c>true</c>) or retrieved from the pool (<c>false</c>).
        /// </param>
        protected virtual void NotifyPooled(T obj, bool wasNew)
        {
            OnObjectPooled?.Invoke(obj);

            if (obj is IPoolable poolable)
                poolable.OnPooled();
        }

        /// <summary>
        /// Creates a new instance of the prefab and initializes it for use in the pool.
        /// </summary>
        /// <returns>
        /// A new instance of type <typeparamref name="T"/> with its GameObject deactivated.
        /// </returns>
        protected virtual T CreateInstance()
        {
            T inst = Instantiate(prefab, transform);
            inst.gameObject.SetActive(false);
            return inst;
        }

        /// <summary>
        /// Adds a specified number of objects to the pool, up to the available slots.
        /// </summary>
        /// <param name="count">The number of objects to add to the pool.</param>
        protected virtual void AddObjects(int count)
        {
            count = Mathf.Min(count, AvailableSlots);

            for (int i = 0; i < count; i++)
            {
                T inst = CreateInstance();
                readyToUseObjects.Enqueue(inst);
                allObjects.Add(inst);
            }
        }

        #endregion
    }

    /// <summary>
    /// Interface for objects that can be managed by a pool.
    /// Provides methods to handle pooling, recycling, and returning operations.
    /// </summary>
    public interface IPoolable
    {
        public void OnPooled();
        public void OnRecycled();
        public void OnReturned();
    }
}