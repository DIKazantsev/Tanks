using Tanks.Core;
using Tanks.Audio;
using UnityEngine;

namespace Tanks.Combat
{
    [RequireComponent(typeof(Rigidbody), typeof(Collider))]
    public sealed class Projectile : MonoBehaviour
    {
        private Rigidbody body;
        private float damage;
        private float lifetime;
        private bool hasHit;

        [SerializeField] private GameObject impactEffectPrefab = null;
        [SerializeField] private GameObject metalImpactEffectPrefab = null;

        private void Awake() => body = GetComponent<Rigidbody>();

        public void Launch(WeaponConfig config, Collider[] ownerColliders)
        {
            damage = config.damage;
            lifetime = config.projectileLifetime;
            foreach (Collider ownerCollider in ownerColliders)
                Physics.IgnoreCollision(GetComponent<Collider>(), ownerCollider);
            body.linearVelocity = transform.forward * config.projectileSpeed;
            Destroy(gameObject, lifetime);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (hasHit) return;
            hasHit = true;
            ContactPoint contact = collision.contactCount > 0 ? collision.GetContact(0) : default;
            bool isMetal = collision.collider.GetComponentInParent<ImpactSurface>() != null;
            GameObject effectPrefab = isMetal ? metalImpactEffectPrefab : impactEffectPrefab;
            AudioManager.Instance?.PlayImpact(contact.point, isMetal);
            if (effectPrefab != null)
            {
                Quaternion rotation = contact.normal.sqrMagnitude > 0.001f
                    ? Quaternion.LookRotation(contact.normal)
                    : Quaternion.identity;
                Instantiate(effectPrefab, contact.point, rotation);
            }
            collision.collider.GetComponentInParent<Health>()?.TakeDamage(damage);
            Destroy(gameObject);
        }
    }
}
