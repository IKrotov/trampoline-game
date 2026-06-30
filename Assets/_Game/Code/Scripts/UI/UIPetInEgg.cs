using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace _Game.Code.Scripts
{
    public class UIPetInEgg : MonoBehaviour
    {
        [SerializeField] private Image petIcon;
        [SerializeField] private TextMeshProUGUI chanceText;

        private GameObject _petPrefab;
        private Texture2D _previewTexture;

        public void Initialize(PetDefinition def, GameObject petPrefab)
        {
            _petPrefab = petPrefab;
            chanceText.text = $"{def.Chance * 100f:0}%";
        }

        private IEnumerator Start()
        {
            yield return null;
            if (_petPrefab == null) yield break;
            _previewTexture = ModelPreviewer.Generate(_petPrefab.transform, 128, 128);
            if (_previewTexture == null) yield break;
            petIcon.sprite = Sprite.Create(
                _previewTexture,
                new Rect(0, 0, _previewTexture.width, _previewTexture.height),
                Vector2.one * 0.5f
            );
        }

        private void OnDestroy()
        {
            if (_previewTexture != null)
                Destroy(_previewTexture);
        }
    }
}
