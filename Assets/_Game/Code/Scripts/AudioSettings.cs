using UnityEngine;

namespace _Game.Code.Scripts
{
    // Local device preference — not part of SaveData, not synced to cloud saves.
    public static class AudioSettings
    {
        private const string MasterVolumeKey = "Audio_MasterVolume";
        private const string MusicMutedKey = "Audio_MusicMuted";
        private const string SfxMutedKey = "Audio_SfxMuted";

        public static float MasterVolume { get; private set; } = PlayerPrefs.GetFloat(MasterVolumeKey, 1f);
        public static bool MusicMuted { get; private set; } = PlayerPrefs.GetInt(MusicMutedKey, 0) == 1;
        public static bool SfxMuted { get; private set; } = PlayerPrefs.GetInt(SfxMutedKey, 0) == 1;

        public static float MusicVolume => MusicMuted ? 0f : MasterVolume;
        public static float SfxVolume => SfxMuted ? 0f : MasterVolume;

        public static void SetMasterVolume(float volume)
        {
            MasterVolume = volume;
            PlayerPrefs.SetFloat(MasterVolumeKey, volume);
            EventBus.RaiseAudioSettingsChanged();
        }

        public static void SetMusicMuted(bool muted)
        {
            MusicMuted = muted;
            PlayerPrefs.SetInt(MusicMutedKey, muted ? 1 : 0);
            EventBus.RaiseAudioSettingsChanged();
        }

        public static void SetSfxMuted(bool muted)
        {
            SfxMuted = muted;
            PlayerPrefs.SetInt(SfxMutedKey, muted ? 1 : 0);
            EventBus.RaiseAudioSettingsChanged();
        }
    }
}
