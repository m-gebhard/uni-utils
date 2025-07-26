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
    public class TextureSizeEditorWindow : EditorWindow
    {
        /// <summary>
        /// List of target folders to apply the texture settings.
        /// </summary>
        private List<string> targetFolders = new();

        /// <summary>
        /// Maximum texture size to be set.
        /// </summary>
        private int maxTextureSize = 2048;

        /// <summary>
        /// Flag to enable or disable texture compression.
        /// </summary>
        private bool isCompressionEnabled = true;

        /// <summary>
        /// Path of the new folder to be added to the target folders list.
        /// </summary>
        private string newFolderPath = "";

        /// <summary>
        /// Shows the Texture Size Editor window.
        /// </summary>
        [MenuItem("Tools/Utils/Set Max Texture Size")]
        public static void ShowWindow()
        {
            GetWindow<TextureSizeEditorWindow>("Texture Size Editor");
        }

        /// <summary>
        /// Draws the GUI for the editor window.
        /// </summary>
        private void OnGUI()
        {
            GUILayout.Label("Texture Settings", EditorStyles.boldLabel);

            maxTextureSize = EditorGUILayout.IntField("Max Texture Size", maxTextureSize);
            isCompressionEnabled = EditorGUILayout.Toggle("Enable Compression", isCompressionEnabled);

            GUILayout.Space(10);
            GUILayout.Label("Target Folders:", EditorStyles.boldLabel);

            for (int i = 0; i < targetFolders.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                targetFolders[i] = EditorGUILayout.TextField(targetFolders[i]);

                if (GUILayout.Button("Remove", GUILayout.Width(70)))
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