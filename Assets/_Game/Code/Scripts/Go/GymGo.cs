using UnityEngine;

namespace _Game.Code.Scripts
{
    public class GymGo : MonoBehaviour
    {
        [field:SerializeField] public GameObject Gym { get; private set; }
        [field:SerializeField] public GameObject Trigger { get; private set; }
        [field:SerializeField] public GameObject ExitPoint { get; private set; }
        [field:SerializeField] public GameObject Barbell { get; private set; }
        [field:SerializeField] public Transform StandPoint { get; private set; }

        private void OnEnable()
        {
            EventBus.EnterToGym.AddListener(HideBarbell);
            EventBus.ExitToGym.AddListener(ShowBarbell);
        }

        private void OnDisable()
        {
            EventBus.EnterToGym.RemoveListener(HideBarbell);
            EventBus.ExitToGym.RemoveListener(ShowBarbell);
        }

        private void HideBarbell() => Barbell.SetActive(false);
        private void ShowBarbell() => Barbell.SetActive(true);
    }
}