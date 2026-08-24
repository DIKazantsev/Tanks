using UnityEngine;
using UnityEngine.Events;

namespace Tanks.Core
{
    public sealed class Health : MonoBehaviour
    {
        [SerializeField, Min(1f)] private float maxHealth = 100f;
        [SerializeField] private bool destroyOnDeath = true;
        [SerializeField] private UnityEvent<float, float> onHealthChanged = new();
        [SerializeField] private UnityEvent onDeath = new();

        public float MaxHealth => maxHealth;
        public float CurrentHealth { get; private set; }
        public bool IsDead { get; private set; }
        public UnityEvent<float, float> OnHealthChanged => onHealthChanged;
        public UnityEvent OnDeath => onDeath;

        private void Awake()
        {
            CurrentHealth = maxHealth;
            onHealthChanged.Invoke(CurrentHealth, maxHealth);
        }

        public void TakeDamage(float amount)
        {
            if (IsDead || amount <= 0f) return;
            CurrentHealth = Mathf.Max(0f, CurrentHealth - amount);
            onHealthChanged.Invoke(CurrentHealth, maxHealth);
            if (CurrentHealth > 0f) return;

            IsDead = true;
            onDeath.Invoke();
            if (destroyOnDeath) Destroy(gameObject);
        }
    }
}
