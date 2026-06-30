using System;
using System.Collections.Generic;

namespace _Game.Code.Scripts.Save
{
    [Serializable]
    public class SaveData
    {
        public long   Coins;
        public double Power;
        public int    Restarts;
        public List<UpgradeSaveData> TrampolineUpgrades = new();
        public List<UpgradeSaveData> WingsUpgrades      = new();
        public List<OwnedPetSaveData> Pets              = new();
        public int GymLevel;
    }

    [Serializable]
    public class UpgradeSaveData
    {
        public string Name;
        public int    ApplyCount;
    }

    [Serializable]
    public class OwnedPetSaveData
    {
        public string Guid;
        public string PetDefinitionId;
        public bool   IsActive;
    }
}
