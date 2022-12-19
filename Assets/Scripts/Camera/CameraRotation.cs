using UnityEngine;

namespace Animations
{
	public class CameraRotation : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private float sensitivity;

        private void Update()
        {
            if (Input.GetMouseButton(1))
            {
                RotateFree();
                transform.LookAt(target);
            }
        }

        private void RotateFree()
        {
            var deltaX = Input.GetAxis("Mouse X") * sensitivity;
            var deltaY = Input.GetAxis("Mouse Y") * sensitivity;

            var position = target.position;

            transform.RotateAround(position, transform.up, deltaX);
            transform.RotateAround(position, transform.right, -deltaY);
        }
    }
}
