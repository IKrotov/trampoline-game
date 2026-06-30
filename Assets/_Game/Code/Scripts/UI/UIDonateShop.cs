using System.Collections.Generic;
using UnityEngine;

namespace _Game.Code.Scripts
{
    public class UIDonateShop : MonoBehaviour
    {
        [SerializeField] private UIDonateCell cellPrefab;
        [SerializeField] private Transform cellContainer;

        private readonly List<GameObject> _cells = new();

        private void OnEnable()
        {
            ClearCells();
            foreach (var donatePet in Balance.DonatePets)
            {
                var cell = Instantiate(cellPrefab, cellContainer);
                cell.Initialize(donatePet);
                _cells.Add(cell.gameObject);
            }
        }

        private void OnDisable() => ClearCells();

        private void ClearCells()
        {
            _cells.ForEach(Destroy);
            _cells.Clear();
        }
    }
}
