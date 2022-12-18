using UnityEditor;
using UnityEngine;

namespace Animations.Editor
{
    [CustomEditor(typeof(MoveTo))]
    public class MoveToEditor : UnityEditor.Editor
    {
        [DrawGizmo(GizmoType.Active | GizmoType.Pickable | GizmoType.Selected)]
        public static void DrawCustomGizmo(MoveTo moveTo, GizmoType gizmoType)
        {
            for (int i = 0; i < moveTo.points.Length - 1; i++)
            {
                Gizmos.DrawLine(moveTo.points[i].position, moveTo.points[i + 1].position);
            }
        }
    }
}