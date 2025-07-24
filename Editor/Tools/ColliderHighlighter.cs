using UnityEditor;
using UnityEngine;

namespace UniUtils.Editor
{
    /// <summary>
    /// Provides a custom editor window for highlighting colliders in the scene view.
    /// </summary>
    public class ColliderHighlighter : EditorWindow
    {
        private static bool enableHighlighting = true;
        private static bool onlyTriggers = true;
        private static Color highlightColor = Color.green;

        /// <summary>
        /// Subscribes to the SceneView.duringSceneGui event when the window is enabled.
        /// </summary>
        private void OnEnable()
        {
            SceneView.duringSceneGui += SceneView_duringSceneGui;
        }

        /// <summary>
        /// Unsubscribes from the SceneView.duringSceneGui event when the window is disabled.
        /// </summary>
        private void OnDisable()
        {
            SceneView.duringSceneGui -= SceneView_duringSceneGui;
        }

        /// <summary>
        /// Shows the Collider Highlighter window.
        /// </summary>
        [MenuItem("Tools/Utils/Collider Highlighter")]
        public static void ShowWindow()
        {
            GetWindow<ColliderHighlighter>("Collider Highlighter").Show();
        }

        /// <summary>
        /// Draws the GUI for the Collider Highlighter window.
        /// </summary>
        private void OnGUI()
        {
            enableHighlighting = EditorGUILayout.Toggle("Enable Highlighting", enableHighlighting);
            onlyTriggers = EditorGUILayout.Toggle("Only Triggers", onlyTriggers);
            highlightColor = EditorGUILayout.ColorField("Highlight Color", highlightColor);
        }

        /// <summary>
        /// Draws the colliders in the scene view with the specified highlight color.
        /// </summary>
        /// <param name="sceneView">The SceneView to draw the colliders in.</param>
        private static void SceneView_duringSceneGui(SceneView sceneView)
        {
            Handles.color = highlightColor;

            foreach (Collider collider in FindObjectsByType<Collider>(FindObjectsSortMode.None))
            {
                if (onlyTriggers && !collider.isTrigger) continue;

                Bounds bounds = collider.bounds;
                Handles.DrawWireCube(bounds.center, bounds.size);

                Vector3 screenPoint = Camera.current.WorldToScreenPoint(bounds.center);

                if (!(screenPoint.z > 0)) continue;

                Handles.BeginGUI();

                GUIStyle style = new()
                {
                    normal =
                    {
                        textColor = highlightColor,
                    },
                    fontSize = 12,
                    alignment = TextAnchor.MiddleCenter,
                    fontStyle = FontStyle.Bold,
                };

                Vector2 labelSize = style.CalcSize(new GUIContent(collider.name));

                GUI.Label(
                    new Rect(
                        screenPoint.x - labelSize.x / 2,
                        Screen.height - screenPoint.y - 10,
                        labelSize.x,
                        labelSize.y),
                    collider.name, style);

                Handles.EndGUI();
            }
        }
    }
}