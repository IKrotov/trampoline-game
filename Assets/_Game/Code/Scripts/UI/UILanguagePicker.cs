using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using _Game.Code.Scripts.Localization;

namespace _Game.Code.Scripts.UI
{
    public class UILanguagePicker : MonoBehaviour
    {
        [Serializable]
        public struct LanguageEntry
        {
            public string Code;
            public string DisplayName;
        }

        [SerializeField] private LanguageEntry[] _languages = new[]
        {
            new LanguageEntry { Code = LanguageCodes.Russian,  DisplayName = "Русский" },
            new LanguageEntry { Code = LanguageCodes.English,  DisplayName = "English"  },
            new LanguageEntry { Code = LanguageCodes.Turkish,  DisplayName = "Türkçe"   },
        };

        [SerializeField] private Button _buttonPrefab;
        [SerializeField] private Transform _container;

        private Button[] _buttons;

        private void Start()
        {
            _buttons = new Button[_languages.Length];

            for (int i = 0; i < _languages.Length; i++)
            {
                var entry = _languages[i];
                var btn = Instantiate(_buttonPrefab, _container);
                btn.GetComponentInChildren<TextMeshProUGUI>().text = entry.DisplayName;
                btn.onClick.AddListener(() => LocalizationManager.SwitchLanguage(entry.Code));
                _buttons[i] = btn;
            }

            RefreshSelection();
            EventBus.LangChanged.AddListener(OnLangChanged);
        }

        private void OnLangChanged(string _) => RefreshSelection();

        private void RefreshSelection()
        {
            string current = LocalizationManager.CurrentLanguage;
            for (int i = 0; i < _languages.Length; i++)
                _buttons[i].interactable = _languages[i].Code != current;
        }

        private void OnDestroy()
        {
            EventBus.LangChanged.RemoveListener(OnLangChanged);
        }
    }
}
