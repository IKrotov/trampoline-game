using System;
using UnityEngine;
using YG;

namespace _Game.Code.Scripts
{
    public class DonateShopController : MonoBehaviour
    {
        private void OnEnable()
        {
            YG2.onPurchaseSuccess += OnPurchaseSuccess;
            EventBus.SaveLoaded.AddListener(OnSaveLoaded);
        }

        private void OnDisable()
        {
            YG2.onPurchaseSuccess -= OnPurchaseSuccess;
            EventBus.SaveLoaded.RemoveListener(OnSaveLoaded);
        }

        // Called after save data is loaded — processes purchases made in previous sessions
        // that were paid for but not yet granted (e.g. game crashed before grant)
        private void OnSaveLoaded()
        {
            foreach (var purchase in YG2.purchases)
            {
                if (purchase.consumed) continue;
                var donateDef = Balance.FindDonatePet(purchase.id);
                if (donateDef == null) continue;
                GrantDonatePet(donateDef, purchase.id);
            }
        }

        private void OnPurchaseSuccess(string productId)
        {
            var donateDef = Balance.FindDonatePet(productId);
            if (donateDef == null) return;
            GrantDonatePet(donateDef, productId);
        }

        private void GrantDonatePet(DonatePetData donateDef, string productId)
        {
            if (!IsOwned(donateDef.PetDefinitionId))
            {
                var def = new PetDefinition
                {
                    Id = donateDef.PetDefinitionId,
                    Name = donateDef.Name,
                    PowerMultiplier = donateDef.PowerMultiplier
                };
                var owned = new OwnedPet
                {
                    Guid = Guid.NewGuid().ToString(),
                    PetDefinitionId = donateDef.PetDefinitionId,
                    IsActive = false,
                    Definition = def
                };
                GameState.OwnedPets.Add(owned);
                EventBus.RaisePetHatched(owned);
            }

            YG2.ConsumePurchaseByID(productId, false);
        }

        public static void BuyPet(string productId) => YG2.BuyPayments(productId);

        public static bool IsOwned(string petDefinitionId)
        {
            foreach (var p in GameState.OwnedPets)
                if (p.PetDefinitionId == petDefinitionId) return true;
            return false;
        }
    }
}
