using System.Collections.Generic;
using UnityEngine;

namespace _Game.Code.Scripts
{
    public class UIEggShop : MonoBehaviour
    {
        [SerializeField] private UIEggCell cellPrefab;
        [SerializeField] private Transform cellContainer;

        private readonly List<GameObject> _cells = new();
        private EggType _currentEgg;

        public void Initialize(EggType egg)
        {
            _currentEgg = egg;
        }

        private void OnEnable()
        {
            if (_currentEgg == null) return;
            ClearCells();
            var cell = Instantiate(cellPrefab, cellContainer);
            cell.Initialize(_currentEgg);
            _cells.Add(cell.gameObject);
        }

        private void OnDisable()
        {
            ClearCells();
        }

        private void ClearCells()
        {
            _cells.ForEach(Destroy);
            _cells.Clear();
        }
    }
}
