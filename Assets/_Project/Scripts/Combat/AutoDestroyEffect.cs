using UnityEngine;

namespace Tanks.Combat
{
    public sealed class AutoDestroyEffect : MonoBehaviour
    {
        [SerializeField, Min(0.05f)] private float lifetime = 2f;

        private void OnEnable() => Invoke(nameof(DestroyEffect), lifetime);

        private void OnDisable() => CancelInvoke();

        private void DestroyEffect() => Destroy(gameObject);
    }
}
