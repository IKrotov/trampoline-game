using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace _Game.Code.Scripts
{
    public class UIPlayerPets : MonoBehaviour
    {
        [SerializeField] private UIPetCell cellPrefab;
        [SerializeField] private Transform cellContainer;
        [SerializeField] private TextMeshProUGUI activeCountText;

        private readonly List<UIPetCell> _cells = new();

        private void OnEnable()
        {
            Rebuild();
            EventBus.PetHatched.AddListener(OnPetHatched);
            EventBus.PetActivated.AddListener(OnPetChanged);
            EventBus.PetDeactivated.AddListener(OnPetChanged);
        }

        private void OnDisable()
        {
            EventBus.PetHatched.RemoveListener(OnPetHatched);
            EventBus.PetActivated.RemoveListener(OnPetChanged);
            EventBus.PetDeactivated.RemoveListener(OnPetChanged);
            ClearCells();
        }

        private void OnPetHatched(OwnedPet _) => Rebuild();
        private void OnPetChanged(OwnedPet _) => RefreshAll();

        private void Rebuild()
        {
            ClearCells();
            foreach (var pet in GameState.OwnedPets)
            {
                var cell = Instantiate(cellPrefab, cellContainer);
                cell.Initialize(pet);
                _cells.Add(cell);
            }
            UpdateCounter();
        }

        private void RefreshAll()
        {
            foreach (var cell in _cells)
                cell.Refresh();
            UpdateCounter();
        }

        private void UpdateCounter()
        {
            activeCountText.text = $"Активных: {GameState.GetActivePetCount()}/3";
        }

        private void ClearCells()
        {
            foreach (var cell in _cells)
                if (cell != null) Destroy(cell.gameObject);
            _cells.Clear();
        }
    }
}
