namespace _Game.Code.Scripts.Localization
{
    public static class LocalizationManager
    {
        private static ILocalizationService _service;

        public static string CurrentLanguage => _service?.CurrentLanguage ?? LanguageCodes.Russian;

        public static void Initialize(ILocalizationService service)
        {
            _service = service;
        }

        public static void SwitchLanguage(string langCode)
        {
            _service?.SwitchLanguage(langCode);
        }
    }
}
