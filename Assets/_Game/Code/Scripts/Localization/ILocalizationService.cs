namespace _Game.Code.Scripts.Localization
{
    public interface ILocalizationService
    {
        string CurrentLanguage { get; }
        void SwitchLanguage(string langCode);
    }
}
