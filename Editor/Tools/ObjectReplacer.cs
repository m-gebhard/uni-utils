using UnityEditor;
using UnityEngine;
using System.Collections.Generic;

namespace UniUtils.Editor
{
    /// <summary>
    /// Editor window for replacing scene objects with a specified prefab.
    /// Allows users to select objects, specify a replacement prefab, and optionally delete the original objects.
    /// </summary>
    public class ObjectReplacer : EditorWindow
    {
        /// <summary>
        /// List of GameObjects to be replaced in the scene.
        /// </summary>
        private List<GameObject> objectsToReplace = new();

        /// <summary>
        /// The prefab that will replace the selected objects.
        /// </summary>
        private GameObject replacementPrefab;

        /// <summary>
        /// Whether to delete the original objects after replacement.
        /// </summary>
        private bool shouldDeleteOriginals;

        /// <summary>
        /// Scroll position for the list of objects to replace.
        /// </summary>
        private Vector2 scrollPos;

        /// <summary>
        /// Opens the Object Replacer editor window.
        /// </summary>
        [MenuItem("Tools/Utils/Replace Objects")]
        public static void ShowWindow()
        {
            GetWindow<ObjectReplacer>("Object Replacer");
        }

        /// <summary>
        /// Draws the GUI for the Object Replacer window.
        /// </summary>
        private void OnGUI()
        {
            EditorGUILayout.LabelField("Replace Scene Objects with Prefab", EditorStyles.boldLabel);

            EditorGUILayout.Space();
            replacementPrefab =
                (GameObject)EditorGUILayout.ObjectField("Replacement Prefab", replacementPrefab, typeof(GameObject),
                    false);

            EditorGUILayout.Space();
            shouldDeleteOriginals = EditorGUILayout.Toggle("Delete Originals", shouldDeleteOriginals);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Objects to Replace:", EditorStyles.boldLabel);

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, GUILayout.Height(150));
            int removeIndex = -1;
            for (int i = 0; i < objectsToReplace.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                objectsToReplace[i] =
                    (GameObject)EditorGUILayout.ObjectField(objectsToReplace[i], typeof(GameObject), true);
                if (GUILayout.Button("X", GUILayout.Width(20)))
                {
                    removeIndex = i;
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndScrollView();

            if (removeIndex >= 0 && removeIndex < objectsToReplace.Count)
            {
                objectsToReplace.RemoveAt(removeIndex);
            }

            if (GUILayout.Button("Add Selected"))
            {
                foreach (GameObject obj in Selection.gameObjects)
                {
                    if (!objectsToReplace.Contains(obj))
                        objectsToReplace.Add(obj);
                }
            }

            EditorGUILayout.Space();

            GUI.enabled = replacementPrefab != null && objectsToReplace.Count > 0;
            if (GUILayout.Button("Replace Objects"))
            {
                ReplaceObjects();
            }

            GUI.enabled = true;
        }

        /// <summary>
        /// Replaces the selected objects in the scene with the specified prefab.
        /// </summary>
        private void ReplaceObjects()
        {
            if (replacementPrefab == null)
            {
                Debug.LogError("No replacement prefab selected.");
                return;
            }

            Undo.RegisterCompleteObjectUndo(objectsToReplace.ToArray(), "Replace Objects");

            foreach (GameObject original in objectsToReplace)
            {
                if (original == null) continue;

                // Instantiate the replacement prefab in the same scene as the original object
                GameObject newObj = (GameObject)PrefabUtility.InstantiatePrefab(replacementPrefab, original.scene);
                Undo.RegisterCreatedObjectUndo(newObj, "Instantiate Replacement");

                // Match the transform of the new object to the original object
                newObj.transform.SetParent(original.transform.parent, true);
                newObj.transform.localPosition = original.transform.localPosition;
                newObj.transform.localRotation = original.transform.localRotation;
                newObj.transform.localScale = original.transform.localScale;

                if (shouldDeleteOriginals)
                {
                    Undo.DestroyObjectImmediate(original);
                }
                else
                {
                    original.SetActive(false);
                }
            }

            objectsToReplace.Clear();
        }
    }
}