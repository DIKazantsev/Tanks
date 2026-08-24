using Tanks.Core;
using UnityEngine;

namespace Tanks.Combat
{
    public sealed class HealthDeathVfx : MonoBehaviour
    {
        [SerializeField] private Health health = null;
        [SerializeField] private GameObject effectPrefab = null;

        private void Awake() => health ??= GetComponent<Health>();

        private void OnEnable()
        {
            if (health != null) health.OnDeath.AddListener(SpawnEffect);
        }

        private void OnDisable()
        {
            if (health != null) health.OnDeath.RemoveListener(SpawnEffect);
        }

        private void SpawnEffect()
        {
            if (effectPrefab != null) Instantiate(effectPrefab, transform.position + Vector3.up, Quaternion.identity);
        }
    }
}
