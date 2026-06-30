using System.Collections.Generic;
using UnityEngine;

namespace _Game.Code.Scripts
{
    public class UIShop : MonoBehaviour
    {
        [SerializeField] private UIUpgradeCell cellPrefab;
        [SerializeField] private Transform cellContainer;
        
        private List<GameObject> cells = new ();

        public void Initialize(List<Upgrade> upgrades)
        {
            ClearCells();
            upgrades.ForEach(u =>
            {
                var cell = Instantiate(cellPrefab, cellContainer);
                cell.Initialize(u);
                cells.Add(cell.gameObject);
            });
        }

        private void ClearCells()
        {
            cells.ForEach(Destroy);
            cells.Clear();
        }
    }
}