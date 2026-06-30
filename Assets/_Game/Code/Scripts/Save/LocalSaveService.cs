using System;
using UnityEngine;

namespace _Game.Code.Scripts.Save
{
    public class LocalSaveService : ISaveService
    {
        private const string SaveKey = "GameSave";

        public void Save(SaveData data, Action onSuccess = null, Action<string> onError = null)
        {
            PlayerPrefs.SetString(SaveKey, JsonUtility.ToJson(data));
            PlayerPrefs.Save();
            onSuccess?.Invoke();
        }

        public void Load(Action<SaveData> onLoaded, Action<string> onError = null)
        {
            var json = PlayerPrefs.GetString(SaveKey, null);
            onLoaded?.Invoke(string.IsNullOrEmpty(json) ? null : JsonUtility.FromJson<SaveData>(json));
        }
    }
}
