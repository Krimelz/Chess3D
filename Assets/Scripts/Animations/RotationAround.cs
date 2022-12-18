using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;

namespace Animations
{
    public class RotationAround : MonoBehaviour
    {
        [SerializeField] private AnimationCurve curve;
        [SerializeField] private float duration;
        [SerializeField] private float angle;
        [SerializeField] private Vector3 axis = Vector3.up;
        [SerializeField] private Transform target;

        // TODO: Move to Editor script
        private void OnDrawGizmos()
        {
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
        }
    }
}
