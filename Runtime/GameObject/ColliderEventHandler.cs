using System;
using System.Collections.Generic;
using UnityEngine;

namespace UniUtils.GameObjects
{
    /// <summary>
    /// Handles runtime registration of collision and trigger events based on object tags.
    /// </summary>
    /// <example>
    /// <code>
    /// public class MyColliderHandler : ColliderEventHandler
    /// {
    ///     private void Start()
    ///     {
    ///         // Register a collision event for objects tagged "Enemy"
    ///         RegisterColliderEvent(new ColliderEvent
    ///         {
    ///             Tag = "Enemy",
    ///             OnEnter = (go, contacts) => Debug.Log($"Enemy collided with {go.name}"),
    ///             OnExit = (go, contacts) => Debug.Log($"Enemy stopped colliding with {go.name}")
    ///         });
    ///
    ///         // Register a trigger event for objects tagged "Pickup"
    ///         RegisterColliderEvent(new ColliderEvent
    ///         {
    ///             Tag = "Pickup",
    ///             OnEnter = (go, _) => Debug.Log($"Entered pickup trigger: {go.name}"),
    ///             OnExit = (go, _) => Debug.Log($"Exited pickup trigger: {go.name}")
    ///         }, isTriggerEvent: true);
    ///     }
    /// }
    /// </code>
    /// </example>
    [RequireComponent(typeof(Collider))]
    public abstract class ColliderEventHandler : MonoBehaviour
    {
        /// <summary>
        /// Stores collision and trigger events by tag.
        /// </summary>
        protected readonly Dictionary<string, ColliderEventEntry> registeredEvents = new();

        /// <summary>
        /// Registers a collider event by tag.
        /// </summary>
        protected virtual void RegisterColliderEvent(ColliderEvent colliderEvent, bool isTriggerEvent = false)
        {
            if (!registeredEvents.TryGetValue(colliderEvent.Tag, out ColliderEventEntry entry))
            {
                entry = new ColliderEventEntry();
            }

            if (isTriggerEvent)
            {
                entry.TriggerEvent = colliderEvent;
            }
            else
            {
                entry.CollisionEvent = colliderEvent;
            }

            registeredEvents[colliderEvent.Tag] = entry;
        }

        /// <summary>
        /// Unregisters a collider event by a given tag.
        /// </summary>
        protected virtual void UnregisterColliderEvent(string eventTag)
        {
            registeredEvents.Remove(eventTag);
        }

        /// <summary>
        /// Called when a trigger collider enters the trigger.
        /// </summary>
        /// <param name="other">The other collider involved in the trigger event.</param>
        protected virtual void OnTriggerEnter(Collider other)
        {
            if (registeredEvents.TryGetValue(other.tag, out ColliderEventEntry entry))
            {
                entry.TriggerEvent.OnEnter?.Invoke(other.gameObject, null);
            }
        }

        /// <summary>
        /// Called when a trigger collider exits the trigger.
        /// </summary>
        /// <param name="other">The other collider involved in the trigger event.</param>
        protected virtual void OnTriggerExit(Collider other)
        {
            if (registeredEvents.TryGetValue(other.tag, out ColliderEventEntry entry))
            {
                entry.TriggerEvent.OnExit?.Invoke(other.gameObject, null);
            }
        }

        /// <summary>
        /// Called when a collision occurs.
        /// </summary>
        /// <param name="other">The collision data associated with the collision event.</param>
        protected virtual void OnCollisionEnter(Collision other)
        {
            if (registeredEvents.TryGetValue(other.gameObject.tag, out ColliderEventEntry entry))
            {
                entry.CollisionEvent.OnEnter?.Invoke(other.gameObject, other.contacts);
            }
        }

        /// <summary>
        /// Called when a collision ends.
        /// </summary>
        /// <param name="other">The collision data associated with the collision event.</param>
        protected virtual void OnCollisionExit(Collision other)
        {
            if (registeredEvents.TryGetValue(other.gameObject.tag, out ColliderEventEntry entry))
            {
                entry.CollisionEvent.OnExit?.Invoke(other.gameObject, other.contacts);
            }
        }
    }

    /// <summary>
    /// Struct representing a collider event with callbacks for enter and exit interactions.
    /// </summary>
    public struct ColliderEvent
    {
        /// <summary>
        /// The tag of the GameObject to match during collision events.
        /// Only GameObjects with this tag will trigger the callbacks.
        /// </summary>
        public string Tag;

        /// <summary>
        /// The action to invoke when a collider with the matching tag enters this collider.
        /// </summary>
        /// <remarks>
        /// The first parameter is the other UnityEngine.GameObject involved in the collision.
        /// The second parameter is an array of UnityEngine.ContactPoint providing collision details.
        /// </remarks>
        public Action<GameObject, ContactPoint[]> OnEnter;

        /// <summary>
        /// The action to invoke when a collider with the matching tag exits this collider.
        /// </summary>
        /// <remarks>
        /// The first parameter is the other UnityEngine.GameObject involved in the collision.
        /// The second parameter is an array of UnityEngine.ContactPoint from the exit interaction.
        /// </remarks>
        public Action<GameObject, ContactPoint[]> OnExit;
    }

    /// <summary>
    /// Stores collision and trigger events.
    /// </summary>
    public struct ColliderEventEntry
    {
        public ColliderEvent CollisionEvent;
        public ColliderEvent TriggerEvent;
    }
}