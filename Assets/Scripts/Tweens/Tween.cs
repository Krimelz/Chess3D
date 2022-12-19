using System;
using System.Collections;
using UnityEngine;

namespace Animations
{
	public class Tween : MonoBehaviour
    {
        public void MoveToPosition(Transform transform, Vector3 endPosition, float duration,
            AnimationCurve curve, Action onStarted = null, Action onCompleted = null)
        {
            StartCoroutine(MoveToPositionCoroutine(transform, endPosition, duration, curve, onStarted, onCompleted));
        }

        public void RotateAroundPoint(Transform transform, Vector3 point, Vector3 target, float duration,
            AnimationCurve curve, Action onStarted = null, Action onCompleted = null)
        {
            StartCoroutine(RotateAroundPointCoroutine(transform, point, target, duration, curve, onStarted, onCompleted));
        }

        private IEnumerator MoveToPositionCoroutine(Transform transform, Vector3 endPosition, float duration,
            AnimationCurve curve, Action onStarted, Action onCompleted)
        {
            var elapsedTime = 0f;
            var startPosition = transform.position;

            onStarted?.Invoke();
            
            while (elapsedTime <= duration)
            {
                elapsedTime += Time.deltaTime;
                
                var time = elapsedTime / duration;
                var value = curve.Evaluate(time);

                var position = Vector3.Lerp(startPosition, endPosition, value);
                transform.position = position;

                yield return null;
            }
            
            onCompleted?.Invoke();
        }

        private IEnumerator RotateAroundPointCoroutine(Transform transform, Vector3 endPosition, Vector3 point, float duration,
            AnimationCurve curve, Action onStarted, Action onCompleted)
        {
            var elapsedTime = 0f;

            var start = transform.position;
            var end = endPosition;

            onStarted?.Invoke();

            while (elapsedTime <= duration)
            {
                elapsedTime += Time.deltaTime;

                var time = elapsedTime / duration;
                var value = curve.Evaluate(time);

                transform.position = Vector3.Slerp(start, end, value);
                transform.LookAt(point);

                yield return null;
            }

            onCompleted?.Invoke();
        }
    }
}