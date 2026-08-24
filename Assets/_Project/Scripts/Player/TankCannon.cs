using Tanks.Combat;
using Tanks.Audio;
using Tanks.Input;
using UnityEngine;

namespace Tanks.Player
{
    public sealed class TankCannon : MonoBehaviour
    {
        [SerializeField] private TankInput input = null;
        [SerializeField] private Transform muzzle = null;
        [SerializeField] private Projectile projectilePrefab = null;
        [SerializeField] private WeaponConfig weaponConfig = null;
        [SerializeField] private ParticleSystem muzzleFlash = null;
        [SerializeField] private ParticleSystem muzzleSmoke = null;
        [SerializeField] private TankAudio audioEvents = null;

        private float nextShotTime;
        private Collider[] ownerColliders;

        public bool IsReloading => Time.time < nextShotTime;
        public bool IsReady => !IsReloading;
        public float ReloadRemaining => Mathf.Max(0f, nextShotTime - Time.time);
        public Transform Muzzle => muzzle;

        private void Awake() => ownerColliders = GetComponentsInParent<Collider>();

        private void Update()
        {
            if (input != null && input.FirePressedThisFrame && IsReady) Fire();
        }

        public bool TryFire()
        {
            if (!IsReady) return false;
            Fire();
            return true;
        }

        private void Fire()
        {
            Projectile projectile = Instantiate(projectilePrefab, muzzle.position, muzzle.rotation);
            projectile.Launch(weaponConfig, ownerColliders);
            muzzleFlash?.Play(true);
            muzzleSmoke?.Play(true);
            audioEvents?.PlayCannonFire();
            nextShotTime = Time.time + weaponConfig.reloadTime;
        }
    }
}
