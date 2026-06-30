using System;

namespace _Game.Code.Scripts.Save
{
    public interface ISaveService
    {
        void Save(SaveData data, Action onSuccess = null, Action<string> onError = null);
        void Load(Action<SaveData> onLoaded, Action<string> onError = null);
    }
}
