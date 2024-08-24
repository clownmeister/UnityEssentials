using UnityEngine;

namespace ClownMeister.UnityEssentials.Gizmo
{
    public static class GizmoArrow
    {
        public static void DrawArrow(Vector3 pos, Vector3 direction, float arrowHeadLength = 0.5f, float arrowHeadAngle = 20.0f)
        {
            Gizmos.DrawRay(pos, direction);

            Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 + arrowHeadAngle, 0) * new Vector3(0, 0, 1);
            Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, 180 - arrowHeadAngle, 0) * new Vector3(0, 0, 1);
            Gizmos.DrawRay(pos + direction, right * arrowHeadLength);
            Gizmos.DrawRay(pos + direction, left * arrowHeadLength);
        }

        public static void DrawArrow(Vector3 pos, Vector3 direction, Color color, float arrowHeadLength = 0.5f, float arrowHeadAngle = 20.0f)
        {
            Gizmos.color = color;
            DrawArrow(pos, direction, arrowHeadLength, arrowHeadAngle);
        }

        public static void DrawArrow(Vector3 pos, Transform target, float arrowHeadLength = 0.5f, float arrowHeadAngle = 20.0f)
        {
            Vector3 direction = (pos - target.position).normalized;

            Gizmos.DrawLine(pos, target.position);

            Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0, arrowHeadAngle, 0) * new Vector3(0, 0, 1);
            Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0, -arrowHeadAngle, 0) * new Vector3(0, 0, 1);
            Gizmos.DrawRay(target.position, right * arrowHeadLength);
            Gizmos.DrawRay(target.position, left * arrowHeadLength);
        }

        public static void DrawArrow(Vector3 pos, Transform target, Color color, float arrowHeadLength = 0.5f, float arrowHeadAngle = 20.0f)
        {
            Gizmos.color = color;
            DrawArrow(pos, target, arrowHeadLength, arrowHeadLength);
        }
    }
}