using UnityEngine;
using UnityEngine.UI;

namespace _Game.Code.Scripts
{
    public class UISettingsPopup : MonoBehaviour
    {
        [SerializeField] private Toggle musicToggle;
        [SerializeField] private Toggle sfxToggle;
        [SerializeField] private Slider volumeSlider;

        private void OnEnable()
        {
            musicToggle.SetIsOnWithoutNotify(!AudioSettings.MusicMuted);
            sfxToggle.SetIsOnWithoutNotify(!AudioSettings.SfxMuted);
            volumeSlider.SetValueWithoutNotify(AudioSettings.MasterVolume);

            musicToggle.onValueChanged.AddListener(OnMusicToggleChanged);
            sfxToggle.onValueChanged.AddListener(OnSfxToggleChanged);
            volumeSlider.onValueChanged.AddListener(AudioSettings.SetMasterVolume);
        }

        private void OnDisable()
        {
            musicToggle.onValueChanged.RemoveListener(OnMusicToggleChanged);
            sfxToggle.onValueChanged.RemoveListener(OnSfxToggleChanged);
            volumeSlider.onValueChanged.RemoveListener(AudioSettings.SetMasterVolume);
        }

        private void OnMusicToggleChanged(bool enabled) => AudioSettings.SetMusicMuted(!enabled);
        private void OnSfxToggleChanged(bool enabled) => AudioSettings.SetSfxMuted(!enabled);
    }
}
