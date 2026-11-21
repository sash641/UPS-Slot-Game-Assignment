using UnityEngine;

namespace JackpotSlots
{
    /// <summary>Owns the four sounds used by the slot game.</summary>
    public sealed class SlotAudioController : MonoBehaviour
    {
        [Header("Audio Sources")]
        [SerializeField] private AudioSource oneShotSource;
        [SerializeField] private AudioSource reelLoopSource;
        [SerializeField] private AudioSource musicSource;

        [Header("Audio Clips")]
        [SerializeField] private AudioClip reelSpinningLoop;
        [SerializeField] private AudioClip mechanicalLeverPull;
        [SerializeField] private AudioClip uiButtonClick;
        [SerializeField] private AudioClip coinPayout;

        [Header("Music")]
        [SerializeField] private AudioClip backgroundMusic;
        [SerializeField, Range(0f, 1f)] private float musicVolume = 0.35f;

        private void OnEnable()
        {
            PlayBackgroundMusic();
        }

        public void PlayUiButtonClick() => PlayOneShot(uiButtonClick);

        public void PlayLeverPull() => PlayOneShot(mechanicalLeverPull);

        public void StartReelLoop()
        {
            if (reelLoopSource == null || reelSpinningLoop == null)
                return;

            reelLoopSource.clip = reelSpinningLoop;
            reelLoopSource.loop = true;

            if (!reelLoopSource.isPlaying)
                reelLoopSource.Play();
        }

        public void StopReelLoop()
        {
            if (reelLoopSource == null)
                return;

            reelLoopSource.Stop();
            reelLoopSource.clip = null;
        }

        public void PlayCoinPayout() => PlayOneShot(coinPayout);

        public void PlayBackgroundMusic()
        {
            if (musicSource == null || backgroundMusic == null)
                return;

            musicSource.clip = backgroundMusic;
            musicSource.loop = true;
            musicSource.volume = musicVolume;

            if (!musicSource.isPlaying)
                musicSource.Play();
        }

        public void StopBackgroundMusic()
        {
            if (musicSource != null)
                musicSource.Stop();
        }

        private void PlayOneShot(AudioClip clip)
        {
            if (oneShotSource != null && clip != null)
                oneShotSource.PlayOneShot(clip);
        }

        private void OnDisable()
        {
            StopReelLoop();
            StopBackgroundMusic();
        }
    }
}
