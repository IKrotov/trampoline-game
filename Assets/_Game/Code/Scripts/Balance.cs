using System;

namespace _Game.Code.Scripts
{
    public static class Balance
    {
        public const double StartingStrength = 100.0;
        public const long   StartingCoins    = 0;
        public const float  MoneyMultiplier  = 0.32f;
        public const float  GymTickInterval  = 0.3f;
        public const int    MaxActivePets    = 3;

        // Gym: levels up automatically when accumulated Strength crosses StrengthToNext
        public static readonly GymLevelData[] GymLevels =
        {
            new() { Level = 0,  PowerPerTick = 1,     StrengthToNext = 200 },
            new() { Level = 1,  PowerPerTick = 3,     StrengthToNext = 700 },
            new() { Level = 2,  PowerPerTick = 7,     StrengthToNext = 2_000 },
            new() { Level = 3,  PowerPerTick = 15,    StrengthToNext = 6_000 },
            new() { Level = 4,  PowerPerTick = 35,    StrengthToNext = 20_000 },
            new() { Level = 5,  PowerPerTick = 80,    StrengthToNext = 70_000 },
            new() { Level = 6,  PowerPerTick = 180,   StrengthToNext = 250_000 },
            new() { Level = 7,  PowerPerTick = 400,   StrengthToNext = 900_000 },
            new() { Level = 8,  PowerPerTick = 900,   StrengthToNext = 3_000_000 },
            new() { Level = 9,  PowerPerTick = 2_000, StrengthToNext = 10_000_000 },
            new() { Level = 10, PowerPerTick = 5_000, StrengthToNext = long.MaxValue },
        };

        // Trampolines: bought with coins, index 0 is free starting trampoline
        // Height = sqrt(Strength) * HeightMultiplier
        public static readonly TrampolineData[] Trampolines =
        {
            new() { Name = "Старый батут",           Price = 0,             HeightMultiplier = 1f },
            new() { Name = "Дворовый батут",         Price = 100,           HeightMultiplier = 1.3f },
            new() { Name = "Спортивный батут",       Price = 450,           HeightMultiplier = 1.8f },
            new() { Name = "Пружинный батут",        Price = 1_500,         HeightMultiplier = 2.6f },
            new() { Name = "Профессиональный батут", Price = 5_000,         HeightMultiplier = 3.8f },
            new() { Name = "Турбо-батут",            Price = 18_000,        HeightMultiplier = 5.5f },
            new() { Name = "Космический батут",      Price = 75_000,        HeightMultiplier = 8f },
            new() { Name = "Плазменный батут",       Price = 300_000,       HeightMultiplier = 12f },
            new() { Name = "Антигравити-батут",      Price = 1_200_000,     HeightMultiplier = 18f },
            new() { Name = "Лунный батут",           Price = 5_000_000,     HeightMultiplier = 27f },
        };

        // Wings: bought with coins, index 0 is free (no wings)
        // JumpTime = BaseJumpTime / SpeedMultiplier
        public static readonly WingsData[] WingsItems =
        {
            new() { Name = "Нет крыльев",          Price = 0,          SpeedMultiplier = 1f },
            new() { Name = "Маленькие крылья",     Price = 60,         SpeedMultiplier = 1.5f },
            new() { Name = "Перья",                Price = 150,        SpeedMultiplier = 2f },
            new() { Name = "Воздушный ранец",      Price = 500,        SpeedMultiplier = 2.8f },
            new() { Name = "Реактивные ботинки",   Price = 1_600,      SpeedMultiplier = 4f },
            new() { Name = "Крылья ветра",         Price = 5_000,      SpeedMultiplier = 5.5f },
            new() { Name = "Турбо-крылья",         Price = 15_000,     SpeedMultiplier = 7f },
            new() { Name = "Плазменные крылья",    Price = 45_000,     SpeedMultiplier = 10f },
            new() { Name = "Космокрылья",          Price = 100_000,    SpeedMultiplier = 15f },
            new() { Name = "Антигравити-крылья",   Price = 300_000,    SpeedMultiplier = 25f },
            new() { Name = "Световые крылья",      Price = 1_000_000,  SpeedMultiplier = 40f },
            new() { Name = "Галактические крылья", Price = 4_000_000,  SpeedMultiplier = 70f },
        };

        // Pets: gacha. PetMultiplier = sum of active pet multipliers (min 1 if none equipped)
        public static readonly EggTierData[] EggTiers =
        {
            new()
            {
                Price = 500,
                Rolls = new[]
                {
                    new PetRoll { Id = "pet_t0_x1.5",   Chance = 0.60f, PowerMultiplier = 1.5f },
                    new PetRoll { Id = "pet_t0_x2.5",   Chance = 0.30f, PowerMultiplier = 2.5f },
                    new PetRoll { Id = "pet_t0_x4",     Chance = 0.10f, PowerMultiplier = 4f },
                }
            },
            new()
            {
                Price = 25_000,
                Rolls = new[]
                {
                    new PetRoll { Id = "pet_t1_x6",     Chance = 0.60f, PowerMultiplier = 6f },
                    new PetRoll { Id = "pet_t1_x10",    Chance = 0.30f, PowerMultiplier = 10f },
                    new PetRoll { Id = "pet_t1_x18",    Chance = 0.10f, PowerMultiplier = 18f },
                }
            },
            new()
            {
                Price = 1_000_000,
                Rolls = new[]
                {
                    new PetRoll { Id = "pet_t2_x30",    Chance = 0.60f, PowerMultiplier = 30f },
                    new PetRoll { Id = "pet_t2_x55",    Chance = 0.30f, PowerMultiplier = 55f },
                    new PetRoll { Id = "pet_t2_x100",   Chance = 0.10f, PowerMultiplier = 100f },
                }
            },
            new()
            {
                Price = 50_000_000,
                Rolls = new[]
                {
                    new PetRoll { Id = "pet_t3_x250",   Chance = 0.60f, PowerMultiplier = 250f },
                    new PetRoll { Id = "pet_t3_x500",   Chance = 0.30f, PowerMultiplier = 500f },
                    new PetRoll { Id = "pet_t3_x1000",  Chance = 0.10f, PowerMultiplier = 1000f },
                }
            },
        };

        // Rebirths: resets Strength and GymLevel, gives +25% to Strength gain per rebirth
        // RebirthMultiplier = 1 + RebirthCount * 0.25
        public static readonly long[] RebirthCosts =
        {
            250_000, 400_000, 700_000, 1_200_000, 1_900_000,
            3_000_000, 5_000_000, 8_500_000, 14_000_000, 23_000_000,
            37_000_000, 62_000_000, 100_000_000, 165_000_000, 270_000_000,
            450_000_000, 750_000_000, 1_200_000_000, 2_000_000_000, 3_500_000_000L,
        };

        // Donate pets — ProductId must match the product ID registered in the Yandex catalog
        public static readonly DonatePetData[] DonatePets =
        {
            new() { ProductId = "donate_pet_1", PetDefinitionId = "donate_pet_1", Name = "Огненный дракон", PowerMultiplier = 7f  },
            new() { ProductId = "donate_pet_2", PetDefinitionId = "donate_pet_2", Name = "Феникс",          PowerMultiplier = 15f },
            new() { ProductId = "donate_pet_3", PetDefinitionId = "donate_pet_3", Name = "Единорог",        PowerMultiplier = 25f },
        };

        public static DonatePetData FindDonatePet(string productId)
        {
            foreach (var d in DonatePets)
                if (d.ProductId == productId) return d;
            return null;
        }

        public static DonatePetData FindDonatePetByDefinitionId(string petDefinitionId)
        {
            foreach (var d in DonatePets)
                if (d.PetDefinitionId == petDefinitionId) return d;
            return null;
        }

        public static readonly ZoneData[] Zones =
        {
            new() { Name = "Двор",        MinHeight = 0 },
            new() { Name = "Крыши",       MinHeight = 100 },
            new() { Name = "Облака",      MinHeight = 500 },
            new() { Name = "Самолеты",    MinHeight = 2_000 },
            new() { Name = "Стратосфера", MinHeight = 10_000 },
            new() { Name = "Космос",      MinHeight = 50_000 },
            new() { Name = "Луна",        MinHeight = 250_000 },
            new() { Name = "Галактика",   MinHeight = 1_000_000 },
        };

        // Returns current gym level based on accumulated strength
        public static int GetGymLevel(double strength)
        {
            int level = 0;
            for (int i = 0; i < GymLevels.Length - 1; i++)
            {
                if (strength >= GymLevels[i].StrengthToNext) level = i + 1;
                else break;
            }
            return level;
        }

        // Strength per tick = GymPower * PetMultiplier * RebirthMultiplier
        public static double CalcRebirthMultiplier(int rebirthCount)
            => 1.0 + rebirthCount * 0.25;

        // Height = sqrt(strength) * trampolineMultiplier — единая формула для наград, UI и зон
        public static double CalcHeight(double strength, float trampolineMultiplier)
            => Math.Sqrt(strength) * trampolineMultiplier;

        // Сжимает игровую высоту в Unity-метры: ~3m при старте, ~40m на максимуме
        public static float CalcPhysicalJumpHeight(double strength, float trampolineMultiplier)
            => 2f * (float)Math.Pow(CalcHeight(strength, trampolineMultiplier), 0.5);

        // Reward = Height^1.5 * MoneyMultiplier — суперлинейный рост, монеты разгоняются с прогрессом
        public static long CalcReward(double height)
            => Math.Max(1, (long)(Math.Pow(height, 1.5) * MoneyMultiplier));

        // BaseJumpTime = 1.2 + 0.9 * Height^0.35  (up phase only)
        public static float CalcBaseJumpTime(float height)
            => 1.2f + 0.9f * (float)Math.Pow(height, 0.35);

        public static string GetZoneName(double height)
        {
            string zone = Zones[0].Name;
            for (int i = 1; i < Zones.Length; i++)
            {
                if (height >= Zones[i].MinHeight) zone = Zones[i].Name;
                else break;
            }
            return zone;
        }
    }

    public abstract class ShopItem { public string Name; public long Price; }
    public class GymLevelData   { public int Level; public double PowerPerTick; public long StrengthToNext; }
    public class TrampolineData : ShopItem { public float HeightMultiplier; }
    public class WingsData      : ShopItem { public float SpeedMultiplier; }
    public class EggTierData    { public long Price; public PetRoll[] Rolls; }
    public class PetRoll        { public string Id; public float Chance; public float PowerMultiplier; }
    public class ZoneData       { public string Name; public double MinHeight; }
    public class DonatePetData  { public string ProductId; public string PetDefinitionId; public string Name; public float PowerMultiplier; }
}
