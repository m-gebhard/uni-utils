using UnityEditor;
using UnityEngine;

namespace UniUtils.Editor
{
    /// <summary>
    /// A MonoBehaviour that visualizes the bounds of a Collider in the Unity Editor.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class ColliderBoundsDrawer : MonoBehaviour
    {
        [SerializeField] private string label;
        [SerializeField] private Color drawColor = Color.cyan;

        private Collider colliderToVisualize;

        private void OnDrawGizmos()
        {
#if UNITY_EDITOR
            colliderToVisualize ??= GetComponent<Collider>();
            if (!colliderToVisualize) return;

            Gizmos.color = drawColor;
            Vector3 center = colliderToVisualize.bounds.center;

            switch (colliderToVisualize)
            {
                case BoxCollider box:
                {
                    Matrix4x4 matrix = Matrix4x4.TRS(
                        transform.position,
                        transform.rotation,
                        transform.lossyScale
                    );
                    Gizmos.matrix = matrix;
                    Gizmos.DrawWireCube(box.center, box.size);
                    center = transform.TransformPoint(box.center);
                    break;
                }
                case SphereCollider sphere:
                    Gizmos.DrawWireSphere(
                        transform.TransformPoint(sphere.center),
                        sphere.radius * Mathf.Max(
                            transform.lossyScale.x,
                            transform.lossyScale.y,
                            transform.lossyScale.z)
                    );
                    center = transform.TransformPoint(sphere.center);
                    break;
                default:
                    Gizmos.DrawWireCube(colliderToVisualize.bounds.center, colliderToVisualize.bounds.size);
                    break;
            }

            GUIStyle style = new()
            {
                normal = new GUIStyleState { textColor = drawColor },
                fontStyle = FontStyle.Bold,
            };
            string displayName = string.IsNullOrEmpty(label) ? name : label;
            Handles.Label(center + Vector3.up * 0.25f, displayName, style);
#endif
        }
    }
}