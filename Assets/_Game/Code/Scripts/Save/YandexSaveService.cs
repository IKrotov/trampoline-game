using System;
using System.Collections.Generic;
using UnityEngine;
using YG;

namespace _Game.Code.Scripts.Save
{
    public class YandexSaveService : ISaveService
    {
        public void Save(SaveData data, Action onSuccess = null, Action<string> onError = null)
        {
            YG2.saves.Coins = data.Coins;
            YG2.saves.Power = data.Power;
            YG2.saves.GymLevel = data.GymLevel;
            YG2.saves.PetsJson = JsonUtility.ToJson(new PetSaveList { Items = data.Pets });
            YG2.SaveProgress();
        }

        public void Load(Action<SaveData> onLoaded, Action<string> onError = null)
        {
            var data = new SaveData
            {
                Coins = YG2.saves.Coins,
                Power = YG2.saves.Power,
                GymLevel = YG2.saves.GymLevel
            };

            if (!string.IsNullOrEmpty(YG2.saves.PetsJson))
            {
                var wrapper = JsonUtility.FromJson<PetSaveList>(YG2.saves.PetsJson);
                if (wrapper?.Items != null)
                    data.Pets = wrapper.Items;
            }

            onLoaded?.Invoke(data);
        }

        [Serializable]
        private class PetSaveList
        {
            public List<OwnedPetSaveData> Items = new();
        }
    }
}
