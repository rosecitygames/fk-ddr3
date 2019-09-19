using IndieDevTools.Agents;
using UnityEngine;

namespace IndieDevTools.Utils
{
    /// <summary>
    /// A untility class for drawing various gizmos.
    /// </summary>
    public static class DrawGizmosUtil
    {
        /// <summary>
        /// Draws a gizmo line from an agent's position to its target advertisement position.
        /// </summary>
        public static void DrawTargetAdvertisementLocationLine(IAgent agent, Color color)
        {
            Vector3 position = agent.Position;
            Vector3 targetPosition = agent.Map.CellToLocal(agent.TargetAdvertisement.Location);
            Gizmos.color = color;
            Gizmos.DrawLine(position, targetPosition);
        }

        /// <summary>
        /// Draws a solid gizmo sphere with a radius equal to a broadcast distance.
        /// </summary>
        public static void DrawBroadcastDistanceSphere(Vector3 position, float broadcastDistance, Color baseColor)
        {
            Color gizmoColor = baseColor;

            gizmoColor.a = baseColor.a * 0.05f;
            Gizmos.color = gizmoColor;
            Gizmos.DrawSphere(position, broadcastDistance);
        }

        /// <summary>
        /// Draws a wireframe gizmo sphere with a radius equal to a broadcast distance.
        /// </summary>
        public static void DrawBroadcastDistanceWireSphere(Vector3 position, float broadcastDistance, Color baseColor)
        {
            Color gizmoColor = baseColor;
            Gizmos.color = gizmoColor;
            Gizmos.DrawWireSphere(position, broadcastDistance);
        }
    }
}
