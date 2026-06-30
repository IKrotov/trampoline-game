using UnityEngine;

namespace _Game.Code.Scripts
{
    public class BarbellVisual : MonoBehaviour
    {
        [SerializeField] private GameObject[] plates;

        private void OnEnable()
        {
            Refresh();
            EventBus.PowerChanged.AddListener(Refresh);
            EventBus.SaveLoaded.AddListener(Refresh);
        }

        private void OnDisable()
        {
            EventBus.PowerChanged.RemoveListener(Refresh);
            EventBus.SaveLoaded.RemoveListener(Refresh);
        }

        private void Start() => Refresh();

        private void Refresh()
        {
            int gymLevel = Balance.GetGymLevel(GameState.Power);
            for (int i = 0; i < plates.Length; i++)
                plates[i].SetActive(i < gymLevel);
        }
    }
}
