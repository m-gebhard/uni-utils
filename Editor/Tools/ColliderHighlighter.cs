using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
namespace UniUtils.Editor
{
    /// <summary>
    /// Provides a custom editor window for highlighting colliders in the scene view.
    /// </summary>
    public class ColliderHighlighter : EditorWindow
    {
        /// <summary>
        /// Indicates whether highlighting is enabled.
        /// </summary>
        private static bool enableHighlighting = true;

        /// <summary>
        /// Indicates whether only trigger colliders should be highlighted.
        /// </summary>
        private static bool onlyTriggers = true;

        /// <summary>
        /// The color used to highlight trigger colliders.
        /// </summary>
        private static Color triggerColor = Color.yellow;

        /// <summary>
        /// The color used to highlight normal colliders.
        /// </summary>
        private static Color normalColor = Color.grey;

        /// <summary>
        /// The count of trigger colliders in the scene.
        /// </summary>
        private static int triggerCount;

        /// <summary>
        /// The count of normal colliders in the scene.
        /// </summary>
        private static int normalCount;

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
        /// Opens the Collider Highlighter window from the Unity Editor menu.
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
            onlyTriggers = EditorGUILayout.Toggle("Only Show Triggers", onlyTriggers);
            triggerColor = EditorGUILayout.ColorField("Trigger Collider Color", triggerColor);
            normalColor = EditorGUILayout.ColorField("Normal Collider Color", normalColor);

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Collider Counts", EditorStyles.boldLabel);
            EditorGUILayout.LabelField($"Trigger Colliders: {triggerCount}");
            EditorGUILayout.LabelField($"Normal Colliders: {normalCount}");
            EditorGUILayout.LabelField($"Total: {triggerCount + normalCount}");
        }

        /// <summary>
        /// Handles the scene GUI rendering to highlight colliders in the scene view.
        /// </summary>
        /// <param name="sceneView">The current SceneView instance.</param>
        private static void SceneView_duringSceneGui(SceneView sceneView)
        {
            if (!enableHighlighting) return;

            triggerCount = 0;
            normalCount = 0;

            foreach (Collider collider in FindObjectsByType<Collider>(FindObjectsSortMode.None))
            {
                if (onlyTriggers && !collider.isTrigger) continue;

                if (collider.isTrigger) triggerCount++;
                else normalCount++;

                Handles.color = collider.isTrigger ? triggerColor : normalColor;

                if (collider is SphereCollider sphere)
                {
                    float maxScale = Mathf.Max(
                        sphere.transform.lossyScale.x,
                        sphere.transform.lossyScale.y,
                        sphere.transform.lossyScale.z);

                    float worldRadius = sphere.radius * maxScale;
                    Vector3 worldCenter = sphere.transform.TransformPoint(sphere.center);

                    Handles.DrawWireDisc(worldCenter, Vector3.up, worldRadius);
                    Handles.DrawWireDisc(worldCenter, Vector3.right, worldRadius);
                    Handles.DrawWireDisc(worldCenter, Vector3.forward, worldRadius);
                }
                else
                {
                    Bounds bounds = collider.bounds;
                    Handles.DrawWireCube(bounds.center, bounds.size);
                }

                Vector3 screenPoint = Camera.current.WorldToScreenPoint(collider.bounds.center);
                if (!(screenPoint.z > 0)) continue;

                Handles.BeginGUI();

                GUIStyle style = new()
                {
                    normal = { textColor = Handles.color },
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

            Handles.BeginGUI();
            GUILayout.BeginArea(new Rect(10, sceneView.position.height - 60, 250, 60));

            GUIStyle countStyle = new(EditorStyles.boldLabel)
            {
                normal = { textColor = Color.white },
                fontSize = 12,
            };

            GUILayout.Label($"Trigger Colliders: {triggerCount}", countStyle);
            if (!onlyTriggers)
            {
                GUILayout.Label($"Normal Colliders: {normalCount}", countStyle);
                GUILayout.Label($"Total: {triggerCount + normalCount}", countStyle);
            }

            GUILayout.EndArea();
            Handles.EndGUI();
        }
    }
}
#endif