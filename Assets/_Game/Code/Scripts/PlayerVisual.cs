using UnityEngine;

namespace _Game.Code.Scripts
{
    public class PlayerVisual : MonoBehaviour
    {
        [SerializeField] private Transform wingsAttachPoint;
        [SerializeField] private GameObject[] wingsPrefabs;
        [SerializeField] private GameObject barbell;

        private GameObject _currentWings;
        private bool _inGym;

        private void Awake() => barbell.SetActive(false);

        private void OnEnable()
        {
            EventBus.UpgradeApplied.AddListener(RefreshWings);
            EventBus.SaveLoaded.AddListener(RefreshWings);
            EventBus.EnterToGym.AddListener(OnEnterGym);
            EventBus.ExitToGym.AddListener(OnExitGym);
        }

        private void OnDisable()
        {
            EventBus.UpgradeApplied.RemoveListener(RefreshWings);
            EventBus.SaveLoaded.RemoveListener(RefreshWings);
            EventBus.EnterToGym.RemoveListener(OnEnterGym);
            EventBus.ExitToGym.RemoveListener(OnExitGym);
        }

        private void Start() => RefreshWings();

        private void OnEnterGym()
        {
            _inGym = true;
            barbell.SetActive(true);
            RefreshWings();
        }

        private void OnExitGym()
        {
            _inGym = false;
            barbell.SetActive(false);
            RefreshWings();
        }

        private void RefreshWings()
        {
            if (_currentWings != null)
                Destroy(_currentWings);

            if (_inGym) return;

            int level = GameState.Wings.Level;
            if (level <= 0 || level >= wingsPrefabs.Length || wingsPrefabs[level] == null)
                return;

            _currentWings = Instantiate(wingsPrefabs[level], wingsAttachPoint);
        }
    }
}
