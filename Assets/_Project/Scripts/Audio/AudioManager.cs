using UnityEngine;

namespace Tanks.Audio
{
    public sealed class AudioManager : MonoBehaviour
    {
        public static AudioManager Instance { get; private set; }

        [SerializeField, Range(0f, 1f)] private float masterVolume = 0.8f;
        [SerializeField] private AudioSource output = null;
        [SerializeField] private AudioClip genericImpactClip = null;
        [SerializeField] private AudioClip metalImpactClip = null;

        public float MasterVolume => masterVolume;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            output ??= GetComponent<AudioSource>();
        }

        public void PlayOneShot(AudioClip clip, float volume = 1f)
        {
            if (clip == null || output == null) return;
            output.PlayOneShot(clip, Mathf.Clamp01(volume) * masterVolume);
        }

        public void PlayImpact(Vector3 position, bool metal)
        {
            AudioClip clip = metal ? metalImpactClip : genericImpactClip;
            if (clip == null) return;
            AudioSource.PlayClipAtPoint(clip, position, masterVolume * 0.7f);
        }
    }
}
