using UnityEngine;
using YG;

namespace _Game.Code.Scripts.Localization
{
    public class YandexLocalizationService : MonoBehaviour, ILocalizationService
    {
        public string CurrentLanguage => YG2.lang;

        private void Awake()
        {
            LocalizationManager.Initialize(this);
        }

        private void OnEnable()
        {
            YG2.onSwitchLang += OnLangSwitched;
        }

        private void OnDisable()
        {
            YG2.onSwitchLang -= OnLangSwitched;
        }

        public void SwitchLanguage(string langCode)
        {
            YG2.SwitchLanguage(langCode);
        }

        private void OnLangSwitched(string langCode)
        {
            EventBus.RaiseLangChanged(langCode);
        }
    }
}
