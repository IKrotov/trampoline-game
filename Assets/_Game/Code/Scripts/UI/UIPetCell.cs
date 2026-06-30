using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.Code.Scripts
{
    public class UIPetCell : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI bonusText;
        [SerializeField] private Button activateButton;
        [SerializeField] private Button deactivateButton;
        [SerializeField] private Image petIcon;
        [SerializeField] private GameAssetsRegistry registry;

        private OwnedPet _pet;
        private Texture2D _previewTexture;

        public void Initialize(OwnedPet pet)
        {
            _pet = pet;
            nameText.text = pet.Definition.Name;
            bonusText.text = $"+{pet.Definition.PowerMultiplier * 100f:F0}% к силе";

            activateButton.onClick.AddListener(() => EventBus.RaiseActivatePetRequest(_pet));
            deactivateButton.onClick.AddListener(() => EventBus.RaiseDeactivatePetRequest(_pet));

            Refresh();
        }

        private IEnumerator Start()
        {
            yield return null;
            if (petIcon == null || registry == null || _pet == null) yield break;
            var prefab = registry.FindPetPrefab(_pet.PetDefinitionId);
            if (prefab == null) yield break;
            _previewTexture = ModelPreviewer.Generate(prefab.transform, 128, 128);
            if (_previewTexture == null) yield break;
            petIcon.sprite = Sprite.Create(
                _previewTexture,
                new Rect(0, 0, _previewTexture.width, _previewTexture.height),
                Vector2.one * 0.5f
            );
        }

        public void Refresh()
        {
            bool isActive = _pet.IsActive;
            activateButton.gameObject.SetActive(!isActive);
            deactivateButton.gameObject.SetActive(isActive);
            // disable Activate if all 3 slots are taken and this pet is not active
            activateButton.interactable = GameState.CanActivatePet();
        }

        private void OnDestroy()
        {
            activateButton.onClick.RemoveAllListeners();
            deactivateButton.onClick.RemoveAllListeners();
            if (_previewTexture != null)
                Destroy(_previewTexture);
        }
    }
}
