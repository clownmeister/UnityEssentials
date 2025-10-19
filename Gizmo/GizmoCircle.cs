using UnityEditor;
using UnityEngine;

namespace ClownMeister.UnityEssentials.Gizmo
{
    public static class GizmoCircle
    {
        public static void Draw(Vector3 position, float radius, Color color, int segments = 32)
        {
            if (radius <= 0 || segments <= 0) return;

            Handles.color = color;
            Handles.DrawWireDisc(position, Vector3.up, radius);
        }

        public static void DrawGizmo(Vector3 position, float radius, Color color, int segments = 32)
        {
            if (radius <= 0 || segments <= 0) return;

            Gizmos.color = color;
            float angleStep = 360f / segments;
            Vector3 prevPoint = position + new Vector3(radius, 0, 0);

            for (int i = 1; i <= segments; i++)
            {
                float angle = i * angleStep;
                Vector3 nextPoint = position + Quaternion.Euler(0, angle, 0) * new Vector3(radius, 0, 0);
                Gizmos.DrawLine(prevPoint, nextPoint);
                prevPoint = nextPoint;
            }
        }
    }
}