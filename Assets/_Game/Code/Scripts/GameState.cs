using System;
using System.Collections.Generic;
using _Game.Code.Scripts.Save;

namespace _Game.Code.Scripts
{
    public static class GameState
    {
        public static long Coins { get; private set; } = Balance.StartingCoins;
        public static double Power { get; private set; } = Balance.StartingStrength;
        public static int Restarts { get; private set; } = 0;
        public static PlayerState PlayerState { get; set; } = PlayerState.IN_WORLD;

        // Gym has no shop — levels up automatically when Strength crosses thresholds (see Balance.GetGymLevel)
        public static List<Upgrade> GymUpgrades { get; } = new();

        public static List<Upgrade> TrampolineUpgrades { get; } = BuildShopUpgrades(Balance.Trampolines, UpgradeType.TRAMPOLINE);
        public static List<Upgrade> WingsUpgrades      { get; } = BuildShopUpgrades(Balance.WingsItems,  UpgradeType.WINGS);

        private static List<Upgrade> BuildShopUpgrades(ShopItem[] items, UpgradeType type)
        {
            var list = new List<Upgrade>();
            for (int i = 1; i < items.Length; i++)
                list.Add(new Upgrade { Enable = i == 1, Level = i, Name = items[i].Name, BaseCost = items[i].Price, Value = 1, ApplyLimit = 1, Type = type });
            return list;
        }

        public static List<EggType> Eggs { get; } = BuildEggs();

        private static List<EggType> BuildEggs()
        {
            var names = new[] { "Обычное яйцо", "Редкое яйцо", "Эпическое яйцо", "Легендарное яйцо" };
            var result = new List<EggType>();
            for (int i = 0; i < Balance.EggTiers.Length; i++)
            {
                var tier = Balance.EggTiers[i];
                var pool = new List<PetDefinition>();
                foreach (var roll in tier.Rolls)
                    pool.Add(new PetDefinition { Id = roll.Id, Name = roll.Id, Chance = roll.Chance, PowerMultiplier = roll.PowerMultiplier });
                result.Add(new EggType { Id = $"egg_{i}", Name = names[i], Cost = tier.Price, Pool = pool });
            }
            return result;
        }

        public static List<OwnedPet> OwnedPets { get; } = new();

        public static Trampoline Trampoline { get; } = new();
        public static Wings      Wings      { get; } = new();

        // --- Coins ---

        public static void AddCoins(long coins)
        {
            Coins += coins;
            EventBus.RaiseCoinsChanged();
        }

        public static void AddCoinsFromLanding()
        {
            float trampolineMultiplier = Balance.Trampolines[Trampoline.Level].HeightMultiplier;
            double height = Balance.CalcHeight(Power, trampolineMultiplier);
            AddCoins(Balance.CalcReward(height));
        }

        public static void SpendCoins(long coins)
        {
            if (coins > Coins) coins = Coins;
            Coins -= coins;
            EventBus.RaiseCoinsChanged();
        }

        // --- Power (Strength) ---

        public static void AddPower(double power)
        {
            Power += power;
            EventBus.RaisePowerChanged();
        }

        // StrengthPerTick = GymPower * PetMultiplier * RebirthMultiplier
        public static void IncrementPower()
        {
            int gymLevel = Balance.GetGymLevel(Power);
            double gymPower = Balance.GymLevels[gymLevel].PowerPerTick;
            double petMultiplier = GetPetMultiplier();
            double rebirthMultiplier = Balance.CalcRebirthMultiplier(Restarts);
            AddPower(gymPower * petMultiplier * rebirthMultiplier);
        }

        // --- Pets ---

        public static int GetActivePetCount()
        {
            int count = 0;
            foreach (var pet in OwnedPets)
                if (pet.IsActive) count++;
            return count;
        }

        public static bool CanActivatePet() => GetActivePetCount() < Balance.MaxActivePets;

        // PetMultiplier = sum of active pet multipliers; 1 if none equipped
        public static double GetPetMultiplier()
        {
            double total = 0;
            foreach (var pet in OwnedPets)
                if (pet.IsActive && pet.Definition != null)
                    total += pet.Definition.PowerMultiplier;
            return total > 0 ? total : 1.0;
        }

        public static PetDefinition FindPetDefinition(string petId)
        {
            foreach (var egg in Eggs)
                foreach (var def in egg.Pool)
                    if (def.Id == petId) return def;

            var donate = Balance.FindDonatePetByDefinitionId(petId);
            if (donate != null)
                return new PetDefinition { Id = donate.PetDefinitionId, Name = donate.Name, PowerMultiplier = donate.PowerMultiplier };

            return null;
        }

        // --- Save ---

        public static SaveData ToSaveData()
        {
            var trampolineUpgrades = new List<UpgradeSaveData>();
            foreach (var u in TrampolineUpgrades)
                trampolineUpgrades.Add(new UpgradeSaveData { Name = u.Name, ApplyCount = u.ApplyCount });

            var wingsUpgrades = new List<UpgradeSaveData>();
            foreach (var u in WingsUpgrades)
                wingsUpgrades.Add(new UpgradeSaveData { Name = u.Name, ApplyCount = u.ApplyCount });

            var pets = new List<OwnedPetSaveData>();
            foreach (var p in OwnedPets)
                pets.Add(new OwnedPetSaveData { Guid = p.Guid, PetDefinitionId = p.PetDefinitionId, IsActive = p.IsActive });

            return new SaveData
            {
                Coins              = Coins,
                Power              = Power,
                Restarts           = Restarts,
                TrampolineUpgrades = trampolineUpgrades,
                WingsUpgrades      = wingsUpgrades,
                Pets               = pets
            };
        }

        public static void ApplySaveData(SaveData data)
        {
            Coins    = data.Coins;
            Power    = data.Power;
            Restarts = data.Restarts;

            RestoreShopUpgrades(TrampolineUpgrades, data.TrampolineUpgrades, out Trampoline.Level);
            RestoreShopUpgrades(WingsUpgrades,      data.WingsUpgrades,      out Wings.Level);

            OwnedPets.Clear();
            if (data.Pets != null)
                foreach (var ps in data.Pets)
                {
                    var def = FindPetDefinition(ps.PetDefinitionId);
                    if (def == null) continue;
                    OwnedPets.Add(new OwnedPet { Guid = ps.Guid, PetDefinitionId = ps.PetDefinitionId, IsActive = ps.IsActive, Definition = def });
                }

            EventBus.RaisePowerChanged();
            EventBus.RaiseCoinsChanged();
            EventBus.RaiseRestartsChanged();
        }

        private static void RestoreShopUpgrades(List<Upgrade> upgrades, List<UpgradeSaveData> saved, out int level)
        {
            level = 0;
            if (saved != null)
                foreach (var s in saved)
                {
                    var u = upgrades.Find(x => x.Name == s.Name);
                    if (u != null) u.ApplyCount = s.ApplyCount;
                }

            for (int i = 0; i < upgrades.Count; i++)
            {
                var u = upgrades[i];
                if (u.ApplyCount > 0) level = u.Level;
                u.Enable = u.ApplyCount == 0 && (i == 0 || upgrades[i - 1].ApplyCount > 0);
            }
        }
    }

    public enum PlayerState { IN_WORLD, ON_TRAMPOLINE, IN_GYM }

    public class Trampoline { public int Level = 0; }
    public class Wings      { public int Level = 0; }

    public enum UpgradeType { TRAMPOLINE, WINGS, GYM }

    public class Upgrade
    {
        public bool   Enable;
        public int    Level;
        public string Name;
        public long   BaseCost;
        public int    Value;
        public int    ApplyLimit;
        public int    ApplyCount;
        public UpgradeType Type;
    }

    public class PetDefinition
    {
        public string Id;
        public string Name;
        public float  Chance;
        public float  PowerMultiplier;
    }

    public class EggType
    {
        public string          Id;
        public string          Name;
        public long            Cost;
        public List<PetDefinition> Pool = new();
    }

    public class OwnedPet
    {
        public string        Guid;
        public string        PetDefinitionId;
        public bool          IsActive;
        public PetDefinition Definition;
    }
}
