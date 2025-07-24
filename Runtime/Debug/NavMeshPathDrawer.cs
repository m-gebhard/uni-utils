using UnityEngine;
using UnityEngine.AI;

namespace UniUtils.Debugging
{
    /// <summary>
    /// Draws the path of a NavMeshAgent using a LineRenderer.
    /// </summary>
    /// <example>
    /// <code>
    /// Attach this component to a GameObject with a NavMeshAgent to visualize the agent's path.
    /// Example usage:
    ///
    /// 1. Add NavMeshAgent component to your GameObject.
    /// 2. Add this NavMeshPathDrawer component.
    /// 3. Configure 'showPath', 'pathColor', and 'lineWidth' in the Inspector.
    ///
    /// The path will be drawn automatically every frame.
    /// </code>
    /// </example>
    [RequireComponent(typeof(NavMeshAgent))]
    public class NavMeshPathDrawer : MonoBehaviour
    {
        /// <summary>
        /// Whether to show the path.
        /// </summary>
        [SerializeField] private bool showPath = true;

        /// <summary>
        /// The color of the path.
        /// </summary>
        [SerializeField] private Color pathColor = Color.yellow;

        /// <summary>
        /// The width of the line representing the path.
        /// </summary>
        [SerializeField] private float lineWidth = 0.1f;

        private NavMeshAgent agent;
        private LineRenderer lineRenderer;

#if UNITY_EDITOR
        /// <summary>
        /// Initializes the NavMeshAgent component.
        /// </summary>
        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
        }

        /// <summary>
        /// Updates the path drawing each frame.
        /// </summary>
        private void Update()
        {
            if (showPath && agent.hasPath)
            {
                DrawPath(agent.path);
            }
            else
            {
                lineRenderer.positionCount = 0;
            }
        }

        /// <summary>
        /// Creates the LineRenderer component if it does not exist.
        /// </summary>
        private void CreateLineRenderer()
        {
            if (lineRenderer) return;

            lineRenderer = gameObject.AddComponent<LineRenderer>();
            lineRenderer.startWidth = lineWidth;
            lineRenderer.endWidth = lineWidth;
            lineRenderer.material = new Material(Shader.Find("Sprites/Default")) { color = pathColor };
            lineRenderer.enabled = true;
        }

        /// <summary>
        /// Enables the LineRenderer component.
        /// </summary>
        private void OnEnable()
        {
            if (lineRenderer)
                lineRenderer.enabled = true;
            else
                CreateLineRenderer();
        }

        /// <summary>
        /// Disables the LineRenderer component.
        /// </summary>
        private void OnDisable()
        {
            if (lineRenderer)
                lineRenderer.enabled = false;
        }

        /// <summary>
        /// Draws the path using the LineRenderer.
        /// </summary>
        /// <param name="path">The NavMeshPath to draw.</param>
        private void DrawPath(NavMeshPath path)
        {
            if (!lineRenderer) return;

            if (path.corners.Length < 2)
            {
                lineRenderer.positionCount = 0;
                return;
            }

            lineRenderer.positionCount = path.corners.Length;
            lineRenderer.SetPositions(path.corners);
        }

        /// <summary>
        /// Draws the path using Gizmos in the editor.
        /// </summary>
        private void OnDrawGizmos()
        {
            if (!showPath || agent == null || agent.path == null)
                return;

            Gizmos.color = pathColor;
            NavMeshPath path = agent.path;

            for (int i = 1; i < path.corners.Length; i++)
            {
                Gizmos.DrawLine(path.corners[i - 1], path.corners[i]);
            }
        }

#endif
    }
}