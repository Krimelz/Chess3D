using System;
using System.Collections;
using UnityEngine;

namespace Animations
{
    public class Tween : MonoBehaviour
    {
        public void MoveToPosition(Transform t, Vector3 endPosition, float duration,
            AnimationCurve curve, Action onCompleted = null)
        {
            StartCoroutine(Move(t, endPosition, duration, curve, onCompleted));
        }

        public void RotateAroundPoint(Transform t, Vector3 point, Vector3 axis, float angle, float duration,
            AnimationCurve curve, Action onCompleted = null)
        {
            StartCoroutine(Rotate(t, point, axis, angle, duration, curve, onCompleted));
        }

        private IEnumerator Move(Transform t, Vector3 endPosition, float duration,
            AnimationCurve curve, Action onCompleted)
        {
            var elapsedTime = 0f;
            var startPosition = t.position;

            while (elapsedTime <= duration)
            {
                elapsedTime += Time.deltaTime;
                
                var time = elapsedTime / duration;
                var value = curve.Evaluate(time);

                var position = Vector3.Lerp(startPosition, endPosition, value);
                t.position = position;

                yield return null;
            }
            
            onCompleted?.Invoke();
        }
        
        private IEnumerator Rotate(Transform t, Vector3 point, Vector3 axis, float angle, float duration,
            AnimationCurve curve, Action onCompleted)
        {
            var elapsedTime = 0f;
            var oldAngle = 0f;

            while (elapsedTime <= duration)
            {
                elapsedTime += Time.deltaTime;
                
                var time = elapsedTime / duration;
                var value = curve.Evaluate(time);

                var newAngle = Mathf.Lerp(0f, angle, value);
                t.RotateAround(point, axis, newAngle - oldAngle);
                t.LookAt(point);

                oldAngle = newAngle;

                yield return null;
            }
            
            onCompleted?.Invoke();
        }
    }
}