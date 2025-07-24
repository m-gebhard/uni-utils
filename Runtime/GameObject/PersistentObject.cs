using System;
using UnityEditor;
using UnityEngine;

namespace UniUtils.GameObjects
{
    /// <summary>
    /// A Unity component that assigns a unique GUID to the object for persistent identification.
    /// </summary>
    /// <example>
    /// <code>
    /// // Attach PersistentObject component to any GameObject in the editor.
    /// // When you create or duplicate the GameObject, it automatically gets a unique GUID.
    ///
    /// public class ExampleUsage : MonoBehaviour
    /// {
    ///     private void Start()
    ///     {
    ///         PersistentObject persistent = gameObject.GetComponent&lt;PersistentObject&gt;();
    ///         if (persistent != null)
    ///         {
    ///             Debug.Log("Persistent GUID: " + persistent.guid);
    ///         }
    ///     }
    /// }
    /// </code>
    /// </example>
    [ExecuteInEditMode]
    public class PersistentObject : MonoBehaviour
    {
        public string guid;

#if UNITY_EDITOR
        /// <summary>
        /// Create a new unique ID for this object when it's created
        /// </summary>
        private void Awake()
        {
            if (!Application.isEditor || Application.isPlaying || !string.IsNullOrEmpty(guid)) return;

            guid = Guid.NewGuid().ToString();
            PrefabUtility.RecordPrefabInstancePropertyModifications(this);
        }

        /// <summary>
        /// Set the GUID if the object has none assigned
        /// </summary>
        private void Update()
        {
            if (String.IsNullOrEmpty(guid))
            {
                guid = Guid.NewGuid().ToString();
                PrefabUtility.RecordPrefabInstancePropertyModifications(this);
            }
        }
#endif
    }
}