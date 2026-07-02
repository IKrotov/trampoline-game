using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.Code.Scripts
{
    public class UIRebirthPopup : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI cost;
        [SerializeField] private TextMeshProUGUI multiplier;
        [SerializeField] private Button rebirthButton;

        private void Start()
        {
            rebirthButton.onClick.AddListener(() => EventBus.RaiseRebirthRequest());
        }

        private void OnEnable()
        {
            Refresh();
            EventBus.CoinsChanged.AddListener(Refresh);
            EventBus.RestartsChanged.AddListener(Refresh);
        }

        private void OnDisable()
        {
            EventBus.CoinsChanged.RemoveListener(Refresh);
            EventBus.RestartsChanged.RemoveListener(Refresh);
        }

        private void Refresh()
        {
            long price = GameState.GetRebirthCost();
            bool canAfford = GameState.Coins >= price;

            cost.text = NumberFormatter.Format(price);
            cost.color = canAfford ? Color.green : Color.red;
            rebirthButton.interactable = canAfford;

            double current = Balance.CalcRebirthMultiplier(GameState.Restarts);
            double next = Balance.CalcRebirthMultiplier(GameState.Restarts + 1);
            multiplier.text = $"x{current:0.00} -> x{next:0.00}";
        }

        private void OnDestroy()
        {
            rebirthButton.onClick.RemoveAllListeners();
        }
    }
}
