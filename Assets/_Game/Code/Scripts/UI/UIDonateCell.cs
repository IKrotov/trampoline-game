using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using YG;

namespace _Game.Code.Scripts
{
    public class UIDonateCell : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI bonusText;
        [SerializeField] private TextMeshProUGUI priceText;
        [SerializeField] private Button buyButton;
        [SerializeField] private GameObject ownedBadge;
        [SerializeField] private Image petIcon;
        [SerializeField] private GameAssetsRegistry registry;

        private DonatePetData _data;
        private Texture2D _previewTexture;

        public void Initialize(DonatePetData data)
        {
            _data = data;
            nameText.text = data.Name;
            bonusText.text = $"x{data.PowerMultiplier} к силе";

            var catalogEntry = YG2.PurchaseByID(data.ProductId);
            priceText.text = catalogEntry != null ? catalogEntry.priceValue : "—";

            buyButton.onClick.AddListener(OnBuyClicked);
            EventBus.PetHatched.AddListener(OnPetHatched);

            Refresh();
        }

        private IEnumerator Start()
        {
            yield return null;
            if (petIcon == null || registry == null || _data == null) yield break;
            var prefab = registry.FindPetPrefab(_data.PetDefinitionId);
            if (prefab == null) yield break;
            _previewTexture = ModelPreviewer.Generate(prefab.transform, 128, 128);
            if (_previewTexture == null) yield break;
            petIcon.sprite = Sprite.Create(
                _previewTexture,
                new Rect(0, 0, _previewTexture.width, _previewTexture.height),
                Vector2.one * 0.5f
            );
        }

        private void OnPetHatched(OwnedPet _) => Refresh();

        private void Refresh()
        {
            bool owned = DonateShopController.IsOwned(_data.PetDefinitionId);
            buyButton.gameObject.SetActive(!owned);
            if (ownedBadge != null) ownedBadge.SetActive(owned);
        }

        private void OnBuyClicked() => DonateShopController.BuyPet(_data.ProductId);

        private void OnDestroy()
        {
            buyButton.onClick.RemoveAllListeners();
            EventBus.PetHatched.RemoveListener(OnPetHatched);
            if (_previewTexture != null) Destroy(_previewTexture);
        }
    }
}
