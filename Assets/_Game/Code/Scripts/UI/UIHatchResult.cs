using System.Collections;
using TMPro;
using UnityEngine;

namespace _Game.Code.Scripts
{
    public class UIHatchResult : MonoBehaviour
    {
        [SerializeField] private GameObject panel;
        [SerializeField] private TextMeshProUGUI petNameText;
        [SerializeField] private TextMeshProUGUI bonusText;

        private Coroutine _closeCoroutine;

        private void Start()
        {
            panel.SetActive(false);
            EventBus.PetHatched.AddListener(ShowResult);
        }

        private void ShowResult(OwnedPet pet)
        {
            petNameText.text = pet.Definition.Name;
            bonusText.text = $"+{pet.Definition.PowerMultiplier * 100f:F0}% к силе";
            panel.SetActive(true);

            if (_closeCoroutine != null) StopCoroutine(_closeCoroutine);
            _closeCoroutine = StartCoroutine(AutoClose());
        }

        public void Close()
        {
            if (_closeCoroutine != null) StopCoroutine(_closeCoroutine);
            panel.SetActive(false);
        }

        private IEnumerator AutoClose()
        {
            yield return new WaitForSeconds(3f);
            panel.SetActive(false);
        }

        private void OnDestroy()
        {
            EventBus.PetHatched.RemoveListener(ShowResult);
        }
    }
}
