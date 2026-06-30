using System.Collections.Generic;
using UnityEngine;

namespace _Game.Code.Scripts
{
    public class PetWorldManager : MonoBehaviour
    {
        [SerializeField] private Transform[] _spawnPoints;
        [SerializeField] private PetGo[] _petPrefabs;

        private bool[] _occupiedSlots;
        private readonly Dictionary<string, (GameObject obj, int slot)> _activeInstances = new();

        private void Awake()
        {
            _occupiedSlots = new bool[_spawnPoints.Length];
        }

        private void OnEnable()
        {
            EventBus.SaveLoaded.AddListener(OnSaveLoaded);
            EventBus.PetActivated.AddListener(OnPetActivated);
            EventBus.PetDeactivated.AddListener(OnPetDeactivated);
        }

        private void OnDisable()
        {
            EventBus.SaveLoaded.RemoveListener(OnSaveLoaded);
            EventBus.PetActivated.RemoveListener(OnPetActivated);
            EventBus.PetDeactivated.RemoveListener(OnPetDeactivated);
        }

        private void OnSaveLoaded()
        {
            foreach (var pet in GameState.OwnedPets)
                if (pet.IsActive) SpawnPet(pet);
        }

        private void OnPetActivated(OwnedPet pet) => SpawnPet(pet);

        private void OnPetDeactivated(OwnedPet pet)
        {
            if (!_activeInstances.TryGetValue(pet.Guid, out var entry)) return;
            Destroy(entry.obj);
            _occupiedSlots[entry.slot] = false;
            _activeInstances.Remove(pet.Guid);
        }

        private void SpawnPet(OwnedPet pet)
        {
            int slot = FindFreeSlot();
            if (slot < 0) return;

            var prefab = FindPrefab(pet.PetDefinitionId);
            if (prefab == null)
            {
                Debug.LogWarning($"[PetWorldManager] No prefab for pet id '{pet.PetDefinitionId}'");
                return;
            }

            var instance = Instantiate(prefab, _spawnPoints[slot]);
            _occupiedSlots[slot] = true;
            _activeInstances[pet.Guid] = (instance.gameObject, slot);
        }

        private int FindFreeSlot()
        {
            for (int i = 0; i < _spawnPoints.Length; i++)
                if (!_occupiedSlots[i]) return i;
            return -1;
        }

        private PetGo FindPrefab(string petId)
        {
            foreach (var prefab in _petPrefabs)
                if (prefab != null && prefab.Id == petId) return prefab;
            return null;
        }
    }
}
