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

        /// <summary>
        /// Draws an arrow in the Unity Editor using Gizmos.
        /// </summary>
        /// <param name="startPosition">The starting position of the arrow.</param>
        /// <param name="direction">The direction vector of the arrow.</param>
        /// <param name="color">The color of the arrow. Defaults to blue if not specified.</param>
        /// <param name="length">The length of the arrow. Defaults to 0.5.</param>
        /// <param name="drawEndDot">Whether to draw a small dot at the end of the arrow. Defaults to false.</param>
        public static void DrawArrow(
            Vector3 startPosition,
            Vector3 direction,
            Color? color = null,
            float length = 0.5f,
            bool drawEndDot = false
        )
        {
            Gizmos.color = color ?? Color.blue;
            Vector3 dir = direction.normalized;
            Vector3 end = startPosition + dir * length;

            Gizmos.DrawLine(startPosition, end);

            const float arrowHeadLength = 0.12f;
            Vector3 right = Vector3.Cross(dir, Vector3.up).normalized;
            if (right.sqrMagnitude < 0.001f) right = Vector3.Cross(dir, Vector3.forward).normalized;

            Vector3 v1 = (-dir + right * 0.5f).normalized * arrowHeadLength;
            Vector3 v2 = (-dir - right * 0.5f).normalized * arrowHeadLength;

            Gizmos.DrawLine(end, end + v1);
            Gizmos.DrawLine(end, end + v2);

            if (drawEndDot) Gizmos.DrawSphere(startPosition, 0.02f);
        }

        /// <summary>
        /// Draws an image in the Unity Editor with customizable width, height, and scale mode.
        /// </summary>
        /// <param name="image">The texture to be drawn.</param>
        /// <param name="width">The width of the image in pixels. Defaults to 300.</param>
        /// <param name="height">The height of the image in pixels. Defaults to 60.</param>
        /// <param name="imageScaleMode">
        /// The scale mode to use when drawing the image. Defaults to <see cref="ScaleMode.StretchToFill"/>.
        /// </param>
        /// <remarks>
        /// This method reserves a layout rect that expands horizontally, then forces the rect width to the supplied width.
        /// It also accounts for <see cref="EditorGUIUtility.pixelsPerPoint"/> so the texture height is crisp on high-DPI displays.
        /// </remarks>
        /// <example>
        /// <code>
        /// // Inside an EditorWindow:
        /// private Texture2D logoImage;
        ///
        /// public void OnEnable()
        /// {
        ///     logoImage = Resources.Load&lt;Texture2D&gt;("Logo");
        /// }
        ///
        /// public override void OnGUI()
        /// {
        ///     // draw stretched across the entire editor client width
        ///     EditorGUILayoutExtension.DrawImage(logoImage, width: position.width, height: 80f);
        /// }
        /// </code>
        /// </example>
        public static void DrawImage(
            Texture image,
            float width = 300f,
            float height = 60f,
            ScaleMode imageScaleMode = ScaleMode.StretchToFill
        )
        {
            Rect r = GUILayoutUtility.GetRect(1, height, GUILayout.ExpandWidth(true));
            r.x = 0;
            r.width = width;

            float pixelsPerPoint = EditorGUIUtility.pixelsPerPoint;
            r.height = Mathf.RoundToInt(height * pixelsPerPoint);

            GUI.DrawTexture(r, image, imageScaleMode, true);
        }
    }
}
#endif