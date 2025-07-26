using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
namespace UniUtils.Extensions
{
    /// <summary>
    /// Provides static helper methods for editor GUI layout.
    /// </summary>
    public static class EditorGUILayoutExtension
    {
        /// <summary>
        /// Draws a horizontal line in the Unity Editor with customizable color, thickness, and padding.
        /// </summary>
        /// <param name="color">The color of the line. Defaults to gray if not specified.</param>
        /// <param name="thickness">The thickness of the line in pixels. Defaults to 1.</param>
        /// <param name="padding">The vertical padding around the line in pixels. Defaults to 10.</param>
        /// <example>
        /// <code>
        /// // Draw a red line with thickness 2 and padding 5
        /// EditorGUILayoutExtension.DrawLine(Color.red, 2, 5);
        /// </code>
        /// </example>
        public static void DrawLine(Color? color = null, int thickness = 1, int padding = 10)
        {
            color ??= Color.gray;

            Rect r = EditorGUILayout.GetControlRect(false, thickness + padding);
            r.height = thickness;
            r.y += padding * 0.5f;

            EditorGUI.DrawRect(r, color.Value);
        }
    }
}
#endif