using UnityEngine;

namespace Tanks.Player
{
    public sealed class TankTurret : MonoBehaviour
    {
        [SerializeField, Min(0f)] private float rotationSpeed = 90f;

        private Quaternion targetRotation;

        private void Awake() => targetRotation = transform.rotation;

        private void Update()
        {
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        public void SetAimDirection(Vector3 worldDirection)
        {
            worldDirection.y = 0f;
            if (worldDirection.sqrMagnitude < 0.001f) return;
            targetRotation = Quaternion.LookRotation(worldDirection.normalized, Vector3.up);
        }
    }
}
