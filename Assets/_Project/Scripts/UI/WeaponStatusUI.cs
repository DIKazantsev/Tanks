using Tanks.Player;
using UnityEngine;
using UnityEngine.UI;

namespace Tanks.UI
{
    public sealed class WeaponStatusUI : MonoBehaviour
    {
        [SerializeField] private TankCannon cannon = null;
        [SerializeField] private Text statusText = null;

        private void Update()
        {
            statusText.text = cannon.IsReady
                ? "READY"
                : $"RELOADING {cannon.ReloadRemaining:0.0}";
        }
    }
}
