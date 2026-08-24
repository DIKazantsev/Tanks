using Tanks.Combat;
using UnityEngine;
using UnityEngine.AI;

namespace Tanks.Player
{
    /// <summary>Small NavMesh tank brain adapted from the MIT Tanks-Unity seek/flee approach.</summary>
    public sealed class EnemyTankAI : MonoBehaviour
    {
        [SerializeField] private TankTurret turret = null;
        [SerializeField] private TankCannon cannon = null;
        [SerializeField, Min(1f)] private float preferredDistance = 28f;
        [SerializeField, Min(1f)] private float fireDistance = 42f;
        [SerializeField, Min(0f)] private float moveSpeed = 4f;
        [SerializeField, Min(0f)] private float turnSpeed = 55f;
        [SerializeField, Min(0.1f)] private float pathRefreshTime = 0.5f;

        private Rigidbody body;
        private Transform target;
        private NavMeshPath path;
        private float pathTimer;
        private int currentCorner;

        private void Awake()
        {
            body = GetComponent<Rigidbody>();
            path = new NavMeshPath();
        }

        private void Update()
        {
            if (target == null)
            {
                Tank player = FindAnyObjectByType<Tank>();
                target = player == null ? null : player.transform;
                pathTimer = pathRefreshTime;
            }
            if (target == null || !target.gameObject.activeInHierarchy) return;

            Vector3 directionToTarget = target.position - turret.transform.position;
            turret.SetAimDirection(directionToTarget);

            pathTimer -= Time.deltaTime;
            if (pathTimer <= 0f)
            {
                pathTimer = pathRefreshTime;
                NavMesh.CalculatePath(transform.position, target.position, NavMesh.AllAreas, path);
                currentCorner = 1;
            }

            float distance = Vector3.Distance(transform.position, target.position);
            if (distance <= fireDistance && HasLineOfSight()) cannon.TryFire();
        }

        private void FixedUpdate()
        {
            if (target == null || path == null || path.corners.Length < 2) return;
            Vector3 destination = path.corners[Mathf.Min(currentCorner, path.corners.Length - 1)];
            Vector3 direction = destination - transform.position;
            direction.y = 0f;
            if (direction.sqrMagnitude < 1f)
            {
                currentCorner = Mathf.Min(currentCorner + 1, path.corners.Length - 1);
                return;
            }

            float targetDistance = Vector3.Distance(transform.position, target.position);
            bool shouldAdvance = targetDistance > preferredDistance;
            Vector3 forward = transform.forward;
            float alignment = Mathf.Clamp01(Vector3.Dot(forward, direction.normalized));
            float angle = Vector3.SignedAngle(forward, direction.normalized, Vector3.up);
            float turn = Mathf.Clamp(angle, -turnSpeed * Time.fixedDeltaTime, turnSpeed * Time.fixedDeltaTime);
            body.MoveRotation(body.rotation * Quaternion.Euler(0f, turn, 0f));
            Vector3 velocity = shouldAdvance ? forward * (alignment * moveSpeed) : Vector3.zero;
            body.linearVelocity = new Vector3(velocity.x, body.linearVelocity.y, velocity.z);
        }

        private bool HasLineOfSight()
        {
            if (cannon.Muzzle == null) return false;
            if (!Physics.Linecast(cannon.Muzzle.position, target.position, out RaycastHit hit, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore)) return true;
            return hit.collider.GetComponentInParent<Tank>() != null;
        }
    }
}
