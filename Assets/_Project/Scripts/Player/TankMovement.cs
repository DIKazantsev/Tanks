using Tanks.Input;
using UnityEngine;

namespace Tanks.Player
{
    [RequireComponent(typeof(Rigidbody))]
    public sealed class TankMovement : MonoBehaviour
    {
        [SerializeField] private TankInput input = null;
        [SerializeField, Min(0f)] private float forwardSpeed = 8f;
        [SerializeField, Min(0f)] private float reverseSpeed = 4f;
        [SerializeField, Min(0f)] private float turnSpeed = 85f;

        private Rigidbody body;

        private void Awake() => body = GetComponent<Rigidbody>();

        private void FixedUpdate()
        {
            float move = input.Move;
            float speed = move >= 0f ? forwardSpeed : reverseSpeed;
            Vector3 planarVelocity = transform.forward * (move * speed);
            body.linearVelocity = new Vector3(planarVelocity.x, body.linearVelocity.y, planarVelocity.z);

            float turnAmount = input.Rotate * turnSpeed * Time.fixedDeltaTime;
            body.MoveRotation(body.rotation * Quaternion.Euler(0f, turnAmount, 0f));
        }
    }
}
