using UnityEngine;

namespace _Game.Code.Scripts.Save
{
    public class SaveManager : MonoBehaviour
    {
        [SerializeField] private float saveInterval = 30f;

        private ISaveService _saveService;
        private bool _isDirty;
        private float _timer;

        private void Start()
        {
            Debug.Log("[SaveManager] Start");
            _saveService = new LocalSaveService();
            LoadGame();

            EventBus.CoinsChanged.AddListener(MarkDirty);
            EventBus.PowerChanged.AddListener(MarkDirty);
            EventBus.UpgradeApplied.AddListener(MarkDirty);
            EventBus.PetHatched.AddListener(_ => MarkDirty());
            EventBus.PetActivated.AddListener(_ => MarkDirty());
            EventBus.PetDeactivated.AddListener(_ => MarkDirty());
        }

        private void LoadGame()
        {
            _saveService.Load(
                onLoaded: data =>
                {
                    if (data != null)
                    {
                        GameState.ApplySaveData(data);
                        Debug.Log($"[SaveManager] Loaded: coins={data.Coins} power={data.Power} gymLevel={data.GymLevel} pets={data.Pets?.Count ?? 0}");
                    }
                    else
                    {
                        Debug.Log("[SaveManager] No save found, starting fresh");
                    }
                    EventBus.RaiseSaveLoaded();
                },
                onError: err => Debug.LogError($"[SaveManager] Load failed: {err}")
            );
        }

        private void Update()
        {
            if (!_isDirty || _saveService == null) return;

            _timer += Time.deltaTime;
            if (_timer >= saveInterval)
                Flush();
        }

        private void MarkDirty()
        {
            _isDirty = true;
        }

        private void Flush()
        {
            _isDirty = false;
            _timer = 0f;
            var data = GameState.ToSaveData();
            _saveService.Save(
                data,
                onSuccess: () => Debug.Log($"[SaveManager] Saved: coins={data.Coins} power={data.Power} pets={data.Pets?.Count ?? 0}"),
                onError: err => Debug.LogError($"[SaveManager] Save failed: {err}")
            );
        }

        private void OnApplicationFocus(bool hasFocus)
        {
            if (!hasFocus && _isDirty) Flush();
        }

        private void OnApplicationPause(bool isPaused)
        {
            if (isPaused && _isDirty) Flush();
        }

        private void OnDestroy()
        {
            if (_isDirty) Flush();

            EventBus.CoinsChanged.RemoveListener(MarkDirty);
            EventBus.PowerChanged.RemoveListener(MarkDirty);
            EventBus.UpgradeApplied.RemoveListener(MarkDirty);
            EventBus.PetHatched.RemoveListener(_ => MarkDirty());
            EventBus.PetActivated.RemoveListener(_ => MarkDirty());
            EventBus.PetDeactivated.RemoveListener(_ => MarkDirty());
        }
    }
}
