using TMPro;
using UnityEngine;

namespace _Game.Code.Scripts
{
    public class UIResourceBar : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI coins;
        [SerializeField] private TextMeshProUGUI power;
        [SerializeField] private TextMeshProUGUI restarts;


        private void Start()
        {
            HandleCons();
            HandlePower();
            HandleRestarts();
            
            EventBus.CoinsChanged.AddListener(HandleCons);
            EventBus.PowerChanged.AddListener(HandlePower);
            EventBus.RestartsChanged.AddListener(HandleRestarts);
        }

        private void HandleCons()
        {
            coins.text = NumberFormatter.Format(GameState.Coins);
        }

        private void HandlePower()
        {
            power.text = NumberFormatter.Format(GameState.Power);
        }

        private void HandleRestarts()
        {
            restarts.text = GameState.Restarts.ToString();
        }

        private void OnDestroy()
        {
            EventBus.CoinsChanged.RemoveListener(HandleCons);
            EventBus.PowerChanged.RemoveListener(HandlePower);
            EventBus.RestartsChanged.RemoveListener(HandleRestarts);
        }
    }
}