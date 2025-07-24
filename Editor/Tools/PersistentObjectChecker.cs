using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UniUtils.GameObjects;

namespace UniUtils.Editor
{
    public class PersistentObjectChecker : EditorWindow
    {
        /// <summary>
        /// Editor tool to check for duplicate GUIDs in the scene
        /// </summary>
        [MenuItem("Tools/Utils/Check Duplicate GUIDs")]
        public static void CheckForDuplicateGUIDs()
        {
            PersistentObject[] allObjects = FindObjectsByType<PersistentObject>(FindObjectsSortMode.None);
            Dictionary<string, List<PersistentObject>> guidMap = new Dictionary<string, List<PersistentObject>>();

            foreach (PersistentObject obj in allObjects)
            {
                if (string.IsNullOrEmpty(obj.guid)) continue;

                if (!guidMap.ContainsKey(obj.guid))
                    guidMap[obj.guid] = new List<PersistentObject>();

                guidMap[obj.guid].Add(obj);
            }

            bool hasDuplicates = false;
            foreach (KeyValuePair<string, List<PersistentObject>> kvp in guidMap)
            {
                if (kvp.Value.Count > 1)
                {
                    hasDuplicates = true;
                    Debug.LogError($"Duplicate GUID found: {kvp.Key}");

                    foreach (PersistentObject obj in kvp.Value)
                    {
                        Debug.LogError($"  - {obj.name} (Scene: {obj.gameObject.scene.name})", obj);
                    }
                }
            }

            if (!hasDuplicates)
            {
                Debug.Log("✅ No duplicate GUIDs found!");
            }
        }
    }
}