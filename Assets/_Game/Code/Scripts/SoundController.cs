using UnityEngine;

namespace _Game.Code.Scripts
{
    public class SoundController : MonoBehaviour
    {
        [SerializeField] private PlayerMovement player;
        [SerializeField] private AudioSource sfxSource;

        [SerializeField] private AudioClip jumpClip;
        [SerializeField] private AudioClip landClip;
        [SerializeField] private AudioClip coinClip;
        [SerializeField] private AudioClip upgradeClip;
        [SerializeField] private AudioClip rebirthClip;

        private void OnEnable()
        {
            player.OnJump.AddListener(PlayJump);
            player.OnLand.AddListener(PlayLand);
            EventBus.CoinsEarned.AddListener(PlayCoin);
            EventBus.UpgradeApplied.AddListener(PlayUpgrade);
            EventBus.RestartsChanged.AddListener(PlayRebirth);
            EventBus.AudioSettingsChanged.AddListener(ApplyVolume);
            ApplyVolume();
        }

        private void OnDisable()
        {
            player.OnJump.RemoveListener(PlayJump);
            player.OnLand.RemoveListener(PlayLand);
            EventBus.CoinsEarned.RemoveListener(PlayCoin);
            EventBus.UpgradeApplied.RemoveListener(PlayUpgrade);
            EventBus.RestartsChanged.RemoveListener(PlayRebirth);
            EventBus.AudioSettingsChanged.RemoveListener(ApplyVolume);
        }

        private void ApplyVolume() => sfxSource.volume = AudioSettings.SfxVolume;

        private void PlayJump() => sfxSource.PlayOneShot(jumpClip);
        private void PlayLand(float height) => sfxSource.PlayOneShot(landClip);
        private void PlayCoin() => sfxSource.PlayOneShot(coinClip);
        private void PlayUpgrade() => sfxSource.PlayOneShot(upgradeClip);
        private void PlayRebirth() => sfxSource.PlayOneShot(rebirthClip);
    }
}
