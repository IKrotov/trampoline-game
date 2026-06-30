using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.Code.Scripts
{
    public class UIEggCell : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI costText;
        [SerializeField] private Image eggIcon;
        [SerializeField] private Transform poolContainer;
        [SerializeField] private Button buyButton;
        [SerializeField] private UIPetInEgg petInEggPrefab;
        [SerializeField] private GameAssetsRegistry registry;

        private EggType _egg;
        private Texture2D _eggPreviewTexture;

        public void Initialize(EggType egg)
        {
            _egg = egg;
            nameText.text = egg.Name;
            BuildPoolIcons(egg);
            Refresh();
            buyButton.onClick.AddListener(() => EventBus.RaiseBuyEgg(_egg));
            EventBus.CoinsChanged.AddListener(Refresh);
        }

        private IEnumerator Start()
        {
            yield return null;
            BuildEggIcon(_egg);
        }

        private void BuildEggIcon(EggType egg)
        {
            var prefab = registry.FindEggPrefab(egg.Id);
            if (prefab == null) return;

            _eggPreviewTexture = ModelPreviewer.Generate(prefab.transform, 256, 256);
            if (_eggPreviewTexture == null) return;
            eggIcon.sprite = Sprite.Create(
                _eggPreviewTexture,
                new Rect(0, 0, _eggPreviewTexture.width, _eggPreviewTexture.height),
                Vector2.one * 0.5f
            );
        }

        private void BuildPoolIcons(EggType egg)
        {
            foreach (Transform child in poolContainer)
                Destroy(child.gameObject);

            foreach (var def in egg.Pool)
            {
                var entry = Instantiate(petInEggPrefab, poolContainer);
                entry.Initialize(def, registry.FindPetPrefab(def.Id));
            }
        }

        private void Refresh()
        {
            bool canAfford = GameState.Coins >= _egg.Cost;
            costText.text = NumberFormatter.Format(_egg.Cost);
            costText.color = canAfford ? Color.green : Color.red;
            buyButton.interactable = canAfford;
        }

        private void OnDestroy()
        {
            buyButton.onClick.RemoveAllListeners();
            EventBus.CoinsChanged.RemoveListener(Refresh);
            if (_eggPreviewTexture != null)
                Destroy(_eggPreviewTexture);
        }
    }
}
