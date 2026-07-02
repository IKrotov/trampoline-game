using UnityEngine;

namespace _Game.Code.Scripts
{
    public class MusicController : MonoBehaviour
    {
        [SerializeField] private AudioSource musicSource;
        [SerializeField] private AudioClip musicClip;

        private void Start()
        {
            musicSource.clip = musicClip;
            musicSource.loop = true;
            musicSource.volume = AudioSettings.MusicVolume;
            musicSource.Play();
        }

        private void OnEnable()
        {
            EventBus.AudioSettingsChanged.AddListener(ApplyVolume);
        }

        private void OnDisable()
        {
            EventBus.AudioSettingsChanged.RemoveListener(ApplyVolume);
        }

        private void ApplyVolume() => musicSource.volume = AudioSettings.MusicVolume;
    }
}
