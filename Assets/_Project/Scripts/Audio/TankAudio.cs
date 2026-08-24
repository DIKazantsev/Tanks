using UnityEngine;

namespace Tanks.Audio
{
    public sealed class TankAudio : MonoBehaviour
    {
        [SerializeField] private AudioManager audioManager = null;
        [SerializeField] private AudioSource source = null;
        [Header("Optional clips")]
        [SerializeField] private AudioClip cannonFireClip = null;
        [SerializeField] private AudioClip projectileFlightClip = null;
        [SerializeField] private AudioClip impactClip = null;
        [SerializeField] private AudioClip explosionClip = null;
        [SerializeField] private AudioClip engineClip = null;
        [SerializeField] private AudioClip trackMovementClip = null;

        public void PlayCannonFire() => Play(cannonFireClip);
        public void PlayProjectileFlight() => Play(projectileFlightClip);
        public void PlayImpact() => Play(impactClip);
        public void PlayExplosion() => Play(explosionClip);
        public void StartEngine() => PlayLoop(engineClip);
        public void StartTracks() => PlayLoop(trackMovementClip);
        public void StopEngine() => StopLoop(engineClip);
        public void StopTracks() => StopLoop(trackMovementClip);

        private void Awake() => audioManager ??= AudioManager.Instance;

        private void Play(AudioClip clip)
        {
            if (clip == null) return;
            if (audioManager != null) audioManager.PlayOneShot(clip);
            else source?.PlayOneShot(clip);
        }

        private void PlayLoop(AudioClip clip)
        {
            if (clip == null || source == null || source.isPlaying) return;
            source.clip = clip;
            source.loop = true;
            source.Play();
        }

        private void StopLoop(AudioClip clip)
        {
            if (source != null && source.clip == clip) source.Stop();
        }
    }
}
