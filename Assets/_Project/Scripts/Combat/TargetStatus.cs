using UnityEngine;

namespace Tanks.Combat
{
    public sealed class TargetStatus : MonoBehaviour
    {
        [SerializeField] private string displayName = "ENEMY TARGET";

        public string DisplayName => displayName;
    }
}
