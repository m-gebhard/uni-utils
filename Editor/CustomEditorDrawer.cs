using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
namespace UniUtils.Editor
{
    /// <summary>
    /// Abstract base class for creating custom property drawers in the Unity Editor.
    /// This class provides functionality for drawing custom properties and buttons in the inspector.
    /// </summary>
    /// <remarks>Make sure the deriving class has a [CustomEditor(typeof(T))] attribute.</remarks>
    /// <remarks>To prevent Build errors, place this script in an 'Editor' folder within your Unity project.</remarks>
    /// <typeparam name="T">The type of the target component this editor is associated with.</typeparam>
    /// <example>
    /// <code>
    /// using UnityEngine;
    /// using UnityEditor;
    /// using UniUtils.Editor;
    ///
    /// public class ExampleComponent : MonoBehaviour
    /// {
    ///     public int myValue;
    /// }
    ///
    /// [CustomEditor(typeof(ExampleComponent))]
    /// public class ExampleComponentEditor : CustomEditorDrawer&lt;ExampleComponent&gt;
    /// {
    ///     protected override string Label =&gt; "Example Tools";
    ///
    ///     protected override List&lt;(string, Action)&gt; EditorButtons =&gt; new()
    ///     {
    ///         ("Log Value", () =&gt; Debug.Log(Target.myValue)),
    ///     };
    ///
    ///     protected override void DrawProperties()
    ///     {
    ///         // Custom drawing can go here, or leave empty to use default only.
    ///     }
    /// }
    /// </code>
    /// </example>
    public abstract class CustomEditorDrawer<T> : UnityEditor.Editor where T : UnityEngine.Object
    {
        /// <summary>
        /// Gets the target component being edited.
        /// </summary>
        protected T Target => (T)target;

        /// <summary>
        /// Gets the label displayed in the custom editor section.
        /// </summary>
        protected virtual string Label { get; } = "";

        /// <summary>
        /// A list of custom buttons to display in the editor, each with a label and a callback action.
        /// </summary>
        protected virtual List<(string, Action)> EditorButtons { get; } = new();


        /// <summary>
        /// Determines whether to show the custom editor section above the default inspector.
        /// </summary>
        protected virtual bool ShowAboveDefaultInspector { get; } = false;

        /// <summary>
        /// Called by Unity to draw the inspector GUI for the target component.
        /// </summary>
        public override void OnInspectorGUI()
        {
            // Update the serialized object to reflect any changes.
            serializedObject.Update();

            // Draw the default inspector and custom editor.
            if (ShowAboveDefaultInspector)
            {
                DrawCustomSection();
                EditorGUILayout.Space();
                DrawDefaultInspector();
            }
            else
            {
                DrawDefaultInspector();
                EditorGUILayout.Space();
                DrawCustomSection();
            }

            // Apply any modified properties to the serialized object.
            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// Draws the custom section (label, properties, and buttons).
        /// </summary>
        private void DrawCustomSection()
        {
            if (!string.IsNullOrEmpty(Label))
                EditorGUILayout.LabelField(Label, EditorStyles.boldLabel);

            DrawProperties();
            EditorGUILayout.Space();
            DrawButtons();
        }

        /// <summary>
        /// Draws the custom buttons defined in <see cref="EditorButtons"/>.
        /// </summary>
        private void DrawButtons()
        {
            foreach ((string text, Action callback) in EditorButtons)
            {
                // Create a button for each entry and invoke its callback when clicked.
                if (GUILayout.Button(text)) callback?.Invoke();
            }
        }

        /// <summary>
        /// Abstract method for drawing custom properties in the inspector.
        /// Must be implemented by derived classes.
        /// </summary>
        protected abstract void DrawProperties();
    }
}
#endif