using System;
using UnityEngine;

namespace _Game.Code.Scripts
{
    [CreateAssetMenu(fileName = "GameAssetsRegistry", menuName = "Game/Assets Registry")]
    public class GameAssetsRegistry : ScriptableObject
    {
        public PetPrefabEntry[] PetPrefabs;
        public EggPrefabEntry[] EggPrefabs;
        public TrampolinePrefabEntry[] TrampolinePrefabs;
        public WingsPrefabEntry[]     WingsPrefabs;

        public GameObject FindPetPrefab(string petId)
        {
            foreach (var entry in PetPrefabs)
                if (entry.PetId == petId) return entry.Prefab;
            return null;
        }

        public GameObject FindEggPrefab(string eggId)
        {
            foreach (var entry in EggPrefabs)
                if (entry.EggId == eggId) return entry.Prefab;
            return null;
        }

        public GameObject FindTrampolinePrefab(int level)
        {
            foreach (var entry in TrampolinePrefabs)
                if (entry.Level == level) return entry.Prefab;
            return null;
        }

        public GameObject FindWingsPrefab(int level)
        {
            foreach (var entry in WingsPrefabs)
                if (entry.Level == level) return entry.Prefab;
            return null;
        }
    }

    [Serializable]
    public struct PetPrefabEntry
    {
        public string PetId;
        public GameObject Prefab;
    }

    [Serializable]
    public struct EggPrefabEntry
    {
        public string EggId;
        public GameObject Prefab;
    }

    [Serializable]
    public struct TrampolinePrefabEntry
    {
        public int Level;
        public GameObject Prefab;
    }

    [Serializable]
    public struct WingsPrefabEntry
    {
        public int Level;
        public GameObject Prefab;
    }
}
