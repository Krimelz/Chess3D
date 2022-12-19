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
            
            /*
            Gizmos.color = Color.red;
            var position = target.position;
            Gizmos.DrawRay(position, axis * 2f);
            
            Gizmos.color = Color.green;
            var position1 = transform.position;
            Vector3 startPosition = position1 - position;
            Vector3 oldPosition = position1;
            
            for (float i = 0f; i <= angle; i += 1f)
            {
                var rotation = Quaternion.AngleAxis(i, axis);
                var newPosition = rotation * startPosition + target.position;
                
                Gizmos.DrawLine(oldPosition, newPosition);

                oldPosition = newPosition;
            }
            */
        }
    }
}