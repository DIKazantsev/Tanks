using Tanks.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Tanks.UI
{
    public sealed class PlayerHealthUI : MonoBehaviour
    {
        [SerializeField] private Health playerHealth = null;
        [SerializeField] private Image healthFill = null;
        [SerializeField] private Text healthText = null;

        private void OnEnable()
        {
            playerHealth.OnHealthChanged.AddListener(UpdateDisplay);
            UpdateDisplay(playerHealth.CurrentHealth, playerHealth.MaxHealth);
        }

        private void OnDisable() => playerHealth.OnHealthChanged.RemoveListener(UpdateDisplay);

        private void UpdateDisplay(float current, float max)
        {
            float normalized = max <= 0f ? 0f : Mathf.Clamp01(current / max);
            healthFill.fillAmount = normalized;
            healthFill.color = normalized > 0.5f
                ? Color.Lerp(new Color(0.72f, 0.84f, 0.2f), new Color(0.22f, 0.72f, 0.3f), (normalized - 0.5f) * 2f)
                : Color.Lerp(new Color(0.85f, 0.18f, 0.08f), new Color(0.72f, 0.84f, 0.2f), normalized * 2f);
            healthText.text = $"HP: {Mathf.CeilToInt(current)} / {Mathf.CeilToInt(max)}";
        }
    }
}
