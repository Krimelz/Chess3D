using System;
using System.Collections;
using UnityEngine;

namespace Tweens
{
	public class Tween : MonoBehaviour
    {
        private Coroutine _moveToPositionCoroutine;
        private Coroutine _rotateAroundPointCoroutine;

        public void MoveToPosition(Transform objectTransform, Vector3 endPosition, float duration,
            AnimationCurve curve, Action onStarted = null, Action onCompleted = null)
        {
            if (_moveToPositionCoroutine != null)
			{
                StopCoroutine(_moveToPositionCoroutine);
            }

            _moveToPositionCoroutine = 
                StartCoroutine(MoveToPositionCoroutine(objectTransform, endPosition, duration, curve, onStarted, onCompleted));
        }

        public void RotateAroundPoint(Transform objectTransform, Vector3 point, Vector3 target, float duration,
            AnimationCurve curve, Action onStarted = null, Action onCompleted = null)
        {
            if (_rotateAroundPointCoroutine != null)
			{
                StopCoroutine(_rotateAroundPointCoroutine);
            }

            _rotateAroundPointCoroutine = 
                StartCoroutine(RotateAroundPointCoroutine(objectTransform, point, target, duration, curve, onStarted, onCompleted));
        }

        private IEnumerator MoveToPositionCoroutine(Transform objectTransform, Vector3 endPosition, float duration,
            AnimationCurve curve, Action onStarted, Action onCompleted)
        {
            var elapsedTime = 0f;
            var startPosition = objectTransform.position;

            onStarted?.Invoke();
            
            while (elapsedTime <= duration)
            {
                elapsedTime += Time.deltaTime;
                
                var time = elapsedTime / duration;
                var value = curve.Evaluate(time);

                var position = Vector3.Lerp(startPosition, endPosition, value);
                objectTransform.position = position;

                yield return null;
            }
            
            onCompleted?.Invoke();
        }

        private IEnumerator RotateAroundPointCoroutine(Transform objectTransform, Vector3 endPosition, Vector3 point, float duration,
            AnimationCurve curve, Action onStarted, Action onCompleted)
        {
            var elapsedTime = 0f;
            var startPosition = objectTransform.position;

            onStarted?.Invoke();

            while (elapsedTime <= duration)
            {
                elapsedTime += Time.deltaTime;

                var time = elapsedTime / duration;
                var value = curve.Evaluate(time);

                objectTransform.position = Vector3.Slerp(startPosition, endPosition, value);
                objectTransform.LookAt(point);

                yield return null;
            }

            onCompleted?.Invoke();
        }
    }
}