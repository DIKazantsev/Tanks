using Tanks.Core;
using UnityEngine;

namespace Tanks.Player
{
    [RequireComponent(typeof(TankMovement), typeof(Health))]
    public sealed class Tank : MonoBehaviour
    {
        [SerializeField] private Transform hull = null;
        [SerializeField] private Transform leftTrack = null;
        [SerializeField] private Transform rightTrack = null;
        [SerializeField] private TankTurret turret = null;
        [SerializeField] private TankCannon cannon = null;

        public Transform Hull => hull;
        public Transform LeftTrack => leftTrack;
        public Transform RightTrack => rightTrack;
        public TankTurret Turret => turret;
        public TankCannon Cannon => cannon;
    }
}
