using UnityEngine;

namespace Tanks.CameraSystem
{
    public sealed class ThirdPersonCamera : MonoBehaviour
    {
        [SerializeField] private Transform target = null;
        [SerializeField, Min(0.1f)] private float distance = 7.5f;
        [SerializeField, Min(0f)] private float height = 4.25f;
        [SerializeField, Min(0.01f)] private float followSmoothness = 8f;
        [SerializeField, Min(0f)] private float lookHeight = 1.6f;

        private Vector3 velocity;

        private void LateUpdate()
        {
            if (target == null) return;
            Vector3 desiredPosition = target.position - target.forward * distance + Vector3.up * height;
            float smoothTime = 1f / followSmoothness;
            transform.position = Vector3.SmoothDamp(transform.position, desiredPosition, ref velocity, smoothTime);

            Vector3 lookTarget = target.position + target.forward * (distance * 0.25f) + Vector3.up * lookHeight;
            transform.rotation = Quaternion.LookRotation(lookTarget - transform.position, Vector3.up);
        }
    }
}
