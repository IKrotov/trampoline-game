using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.Code.Scripts
{
    public class UIUpgradeCell : MonoBehaviour
    {
        [SerializeField] private Image icon;
        [SerializeField] private TextMeshProUGUI description;
        [SerializeField] private TextMeshProUGUI cost;
        [SerializeField] private TextMeshProUGUI count;
        [SerializeField] private Button buyButton;
        [SerializeField] private TextMeshProUGUI completeText;
        [SerializeField] private GameAssetsRegistry registry;

        private Upgrade _upgrade;
        private Texture2D _previewTexture;

        public void Initialize(Upgrade upgrade)
        {
            _upgrade = upgrade;

            SetParams();
            buyButton.onClick.AddListener(() => EventBus.RaiseUpgrade(upgrade));
            EventBus.UpgradeApplied.AddListener(SetParams);
            EventBus.CoinsChanged.AddListener(SetParams);
        }

        private IEnumerator Start()
        {
            yield return null;
            BuildIcon();
        }

        private void BuildIcon()
        {
            if (icon == null || registry == null) return;

            GameObject prefab = _upgrade.Type switch
            {
                UpgradeType.TRAMPOLINE => registry.FindTrampolinePrefab(_upgrade.Level),
                UpgradeType.WINGS      => registry.FindWingsPrefab(_upgrade.Level),
                _                      => null
            };
            if (prefab == null) return;

            _previewTexture = ModelPreviewer.Generate(prefab.transform, 256, 256);
            if (_previewTexture == null) return;

            icon.sprite = Sprite.Create(
                _previewTexture,
                new Rect(0, 0, _previewTexture.width, _previewTexture.height),
                Vector2.one * 0.5f
            );
        }

        private void SetParams()
        {
            description.text = _upgrade.Name;
            cost.text = NumberFormatter.Format(_upgrade.BaseCost);
            count.text = _upgrade.ApplyCount + "/" + _upgrade.ApplyLimit;

            bool canAfford = GameState.Coins >= _upgrade.BaseCost;
            bool isMaxed = _upgrade.ApplyCount >= _upgrade.ApplyLimit;
            buyButton.interactable = _upgrade.Enable && canAfford && !isMaxed;
            buyButton.gameObject.SetActive(!isMaxed && canAfford);
            cost.color = canAfford ? Color.green : Color.red;
            cost.gameObject.SetActive(!isMaxed);
            completeText.gameObject.SetActive(isMaxed);
        }

        private void OnDestroy()
        {
            buyButton.onClick.RemoveAllListeners();
            EventBus.UpgradeApplied.RemoveListener(SetParams);
            EventBus.CoinsChanged.RemoveListener(SetParams);
            if (_previewTexture != null)
                Destroy(_previewTexture);
        }
        
    }
}