using UnityEngine;

namespace Tanks.Combat
{
    [CreateAssetMenu(fileName = "WeaponConfig", menuName = "Tanks/Weapon Config")]
    public sealed class WeaponConfig : ScriptableObject
    {
        [Min(1f)] public float damage = 25f;
        [Min(0.01f)] public float reloadTime = 0.75f;
        [Min(0.1f)] public float projectileSpeed = 35f;
        [Min(0.1f)] public float projectileLifetime = 4f;
    }
}
