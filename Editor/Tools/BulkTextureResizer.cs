using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
namespace UniUtils.Editor
{
    /// <summary>
    /// Editor window for setting the maximum texture size and enabling compression for textures in specified folders.
    /// </summary>
    public class BulkTextureResizer : EditorWindow
    {
        /// <summary>
        /// List of target folders to apply the texture settings.
        /// </summary>
        private readonly List<string> targetFolders = new();

        /// <summary>
        /// Flag to enable or disable texture compression.
        /// </summary>
        private bool isCompressionEnabled = true;

        /// <summary>
        /// Path of the new folder to be added to the target folders list.
        /// </summary>
        private string newFolderPath = "";

        /// <summary>
        /// Index of the currently selected texture size in the AllowedSizes array.
        /// </summary>
        private int selectedSizeIndex = 5;

        /// <summary>
        /// Array of allowed texture sizes.
        /// </summary>
        private static readonly int[] AllowedSizes = { 32, 64, 128, 256, 512, 1024, 2048, 4096, 8192 };

        /// <summary>
        /// Shows the Texture Size Editor window.
        /// </summary>
        [MenuItem("Tools/Utils/Bulk Texture Resizer")]
        public static void ShowWindow()
        {
            GetWindow<BulkTextureResizer>("Bulk Texture Resizer");
        }

        /// <summary>
        /// Draws the GUI for the editor window.
        /// </summary>
        private void OnGUI()
        {
            GUILayout.Label("Texture Settings", EditorStyles.boldLabel);
            EditorGUILayout.LabelField(
                "Add target folders containing the textures you want to modify. Click 'Apply Changes' to set the maximum texture size and optionally enable compression.",
                new GUIStyle(EditorStyles.label) { wordWrap = true });

            GUILayout.Space(10);

            selectedSizeIndex = EditorGUILayout.Popup("Max Texture Size", selectedSizeIndex,
                Array.ConvertAll(AllowedSizes, s => s.ToString()));

            isCompressionEnabled = EditorGUILayout.Toggle("Enable Compression", isCompressionEnabled);

            GUILayout.Space(10);
            GUILayout.Label("Target Folders:", EditorStyles.boldLabel);

            for (int i = 0; i < targetFolders.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                targetFolders[i] = EditorGUILayout.TextField(targetFolders[i]);

                if (GUILayout.Button("X", GUILayout.Width(20)))
                {
                    targetFolders.RemoveAt(i);
                }

                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.BeginHorizontal();
            newFolderPath = EditorGUILayout.TextField(newFolderPath);

            if (GUILayout.Button("Add Folder") && !string.IsNullOrEmpty(newFolderPath))
            {
                if (!targetFolders.Contains(newFolderPath))
                {
                    targetFolders.Add(newFolderPath);
                    newFolderPath = "";
                }
            }

            EditorGUILayout.EndHorizontal();

            GUILayout.Space(10);

            if (GUILayout.Button("Apply Changes", GUILayout.Height(30)))
            {
                SetMaxSize();
            }
        }

        /// <summary>
        /// Sets the maximum texture size and enables compression for textures in the target folders.
        /// </summary>
        private void SetMaxSize()
        {
            int maxTextureSize = AllowedSizes[selectedSizeIndex];
            int totalTextures = 0;

            foreach (string folder in targetFolders)
            {
                totalTextures += AssetDatabase.FindAssets("t:Texture", new[] { folder }).Length;
            }

            int processedTextures = 0;
            AssetDatabase.StartAssetEditing();

            try
            {
                foreach (string folder in targetFolders)
                {
                    string[] guids = AssetDatabase.FindAssets("t:Texture", new[] { folder });

                    foreach (string guid in guids)
                    {
                        string path = AssetDatabase.GUIDToAssetPath(guid);
                        if (AssetImporter.GetAtPath(path) is not TextureImporter textureImporter) continue;

                        bool needsUpdate = false;

                        if (textureImporter.maxTextureSize != maxTextureSize)
                        {
                            textureImporter.maxTextureSize = maxTextureSize;
                            needsUpdate = true;
                        }

                        if (isCompressionEnabled && (!textureImporter.crunchedCompression ||
                                                     textureImporter.compressionQuality != 50))
                        {
                            textureImporter.crunchedCompression = true;
                            textureImporter.compressionQuality = 50;
                            needsUpdate = true;
                        }

                        if (needsUpdate)
                        {
                            AssetDatabase.ImportAsset(path, ImportAssetOptions.ForceUpdate);
                        }

                        processedTextures++;
                        EditorUtility.DisplayProgressBar("Updating Texture Sizes",
                            $"Processing {processedTextures}/{totalTextures}",
                            (float)processedTextures / totalTextures);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error updating texture sizes: {ex.Message}");
            }
            finally
            {
                AssetDatabase.StopAssetEditing();
                AssetDatabase.Refresh();
                EditorUtility.ClearProgressBar();
                Debug.Log($"Updated {processedTextures} textures.");
            }
        }
    }
}
#endif