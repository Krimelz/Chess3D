using System;
using System.Collections;
using System.Threading.Tasks;
using ChessBoard;
using UnityEngine;

namespace Animations
{
    public class RotationAround : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private float sensitivity;
        [SerializeField] private AnimationCurve curve;
        [SerializeField] private float duration;

        private Coroutine rot;

        private void Update()
        {
            if (Input.GetMouseButton(1) && rot == null)
            {
                RotateFree();
            }

            transform.LookAt(target);
        }

        private void RotateFree()
        {
            var deltaX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
            var deltaY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

            var position = target.position;
            
            transform.RotateAround(position, Vector3.up, deltaX);
            transform.RotateAround(position, Vector3.left, -deltaY);
        }

        public void RotateTo(Vector3 point, Action onStarted = null, Action onCompleted = null)
        {
            rot = StartCoroutine(RotateToCoroutine(point, onStarted, onCompleted));
        }

        private IEnumerator RotateToCoroutine(Vector3 point, Action onStarted, Action onCompleted)
        {
            var elapsedTime = 0f;
            var position = target.position;
            var oldPosition = transform.position - position;
            var newPosition = point - position;
            
            onStarted?.Invoke();

            while (elapsedTime <= duration)
            {
                elapsedTime += Time.deltaTime;
                
                var time = elapsedTime / duration;
                var value = curve.Evaluate(time);

                transform.position = Vector3.Slerp(oldPosition, newPosition, value) + position;

                yield return null;
            }
            
            onCompleted?.Invoke();
            rot = null;
        }
    }
}
