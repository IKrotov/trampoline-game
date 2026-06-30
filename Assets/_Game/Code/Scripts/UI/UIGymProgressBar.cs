using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.Code.Scripts
{
    public class UIGymProgressBar : MonoBehaviour
    {
        [SerializeField] private Image _fillImage;
        [SerializeField] private TextMeshProUGUI _levelText;

        private void OnEnable()
        {
            EventBus.PowerChanged.AddListener(Refresh);
            Refresh();
        }

        private void OnDisable()
        {
            EventBus.PowerChanged.RemoveListener(Refresh);
        }

        private void Refresh()
        {
            int level = Balance.GetGymLevel(GameState.Power);

            if (_levelText != null)
                _levelText.text = $"Тренажёр {level} / {Balance.GymLevels.Length - 1}";

            if (_fillImage == null) return;

            if (level >= Balance.GymLevels.Length - 1)
            {
                _fillImage.fillAmount = 1f;
                return;
            }

            double prev = level > 0 ? Balance.GymLevels[level - 1].StrengthToNext : 0;
            double next = Balance.GymLevels[level].StrengthToNext;
            _fillImage.fillAmount = (float)((GameState.Power - prev) / (next - prev));
        }
    }
}
