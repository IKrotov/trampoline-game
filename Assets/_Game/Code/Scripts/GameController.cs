using System;
using System.Collections;
using UnityEngine;

namespace _Game.Code.Scripts
{
    public class GameController : MonoBehaviour
    {
        [SerializeField] private GameObject player;
        [SerializeField] private TrampolineGo trampoline;
        [SerializeField] private GymGo gym;
        [SerializeField] private GameAssetsRegistry assetsRegistry;

        private CharacterController _playerController;
        private PlayerMovement _playerMovement;

        private bool _autoJump = false;
        private Coroutine _gymCoroutine;

        private void Start()
        {
            _playerController = player.GetComponent<CharacterController>();
            _playerMovement = player.GetComponent<PlayerMovement>();
            _playerMovement.OnLand.AddListener(HandleLand);

            EventBus.Upgrade.AddListener(Upgrade);
            EventBus.BuyEgg.AddListener(BuyEgg);
            EventBus.ActivatePetRequest.AddListener(ActivatePet);
            EventBus.DeactivatePetRequest.AddListener(DeactivatePet);
            EventBus.SaveLoaded.AddListener(ApplyTrampolineModel);
        }

        public void Upgrade(Upgrade upgrade)
        {
            if (!upgrade.Enable || GameState.Coins < upgrade.BaseCost || upgrade.ApplyCount >= upgrade.ApplyLimit)
                return;

            upgrade.ApplyCount += 1;
            GameState.SpendCoins(upgrade.BaseCost);

            switch (upgrade.Type)
            {
                case UpgradeType.TRAMPOLINE:
                    GameState.Trampoline.Level += upgrade.Value;
                    ApplyTrampolineModel();
                    break;
                case UpgradeType.WINGS:
                    GameState.Wings.Level += upgrade.Value;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            if (upgrade.ApplyCount >= upgrade.ApplyLimit)
            {
                upgrade.Enable = false;
                var list = upgrade.Type == UpgradeType.TRAMPOLINE ? GameState.TrampolineUpgrades : GameState.WingsUpgrades;
                var next = list.Find(u => u.Level == upgrade.Level + 1);
                if (next != null) next.Enable = true;
            }

            EventBus.RaiseUpgradeApplied();
        }

        private void BuyEgg(EggType egg)
        {
            if (GameState.Coins < egg.Cost) return;

            GameState.SpendCoins(egg.Cost);

            float roll = UnityEngine.Random.value;
            float cumulative = 0f;
            var def = egg.Pool[egg.Pool.Count - 1];
            for (int i = 0; i < egg.Pool.Count; i++)
            {
                cumulative += egg.Pool[i].Chance;
                if (roll < cumulative) { def = egg.Pool[i]; break; }
            }
            var owned = new OwnedPet
            {
                Guid = Guid.NewGuid().ToString(),
                PetDefinitionId = def.Id,
                IsActive = false,
                Definition = def
            };
            GameState.OwnedPets.Add(owned);
            EventBus.RaisePetHatched(owned);
        }

        private void ActivatePet(OwnedPet pet)
        {
            if (pet.IsActive || !GameState.CanActivatePet()) return;
            pet.IsActive = true;
            EventBus.RaisePetActivated(pet);
        }

        private void DeactivatePet(OwnedPet pet)
        {
            if (!pet.IsActive) return;
            pet.IsActive = false;
            EventBus.RaisePetDeactivated(pet);
        }

        public void EnterToGym()
        {
            GameState.PlayerState = PlayerState.IN_GYM;
            _playerMovement.EnableWasd = false;
            _playerMovement.EnableJump = false;
            EventBus.RaiseEnterToGym();
            Teleport(gym.StandPoint.position, gym.StandPoint.rotation);
            StartGym();
        }

        public void ExitToGym()
        {
            StopGym();
            GameState.PlayerState = PlayerState.IN_WORLD;
            _playerMovement.EnableWasd = true;
            _playerMovement.EnableJump = true;
            EventBus.RaiseExitToGym();
            Teleport(gym.ExitPoint.transform.position);
        }

        private void ApplyTrampolineModel()
        {
            var prefab = assetsRegistry.FindTrampolinePrefab(GameState.Trampoline.Level);
            trampoline.SwapModel(prefab);
        }

        public void EnterToTrampoline()
        {
            GameState.PlayerState = PlayerState.ON_TRAMPOLINE;
            _playerMovement.EnableWasd = false;
            EventBus.RaiseEnterToTrampoline();
            Teleport(trampoline.StandPoint.position);
        }

        public void ExitToTrampoline()
        {
            StopAutoJump();
            GameState.PlayerState = PlayerState.IN_WORLD;
            _playerMovement.EnableWasd = true;
            EventBus.RaiseExitToTrampoline();
            Teleport(trampoline.Trigger.transform.position);
        }

        private void HandleLand(float height)
        {
            if (GameState.PlayerState == PlayerState.ON_TRAMPOLINE)
            {
                GameState.AddCoinsFromLanding();
                if (_autoJump)
                    _playerMovement.Jump(GetTrampolineJumpHeight());
            }
        }

        private float GetTrampolineJumpHeight()
        {
            float multiplier = Balance.Trampolines[GameState.Trampoline.Level].HeightMultiplier;
            return Balance.CalcPhysicalJumpHeight(GameState.Power, multiplier);
        }

        public void HandleAutoJump()
        {
            if (_autoJump) StopAutoJump();
            else StartAutoJump();
        }

        private void StartGym()
        {
            _gymCoroutine = StartCoroutine(GymRoutine());
        }

        private void StopGym()
        {
            StopCoroutine(_gymCoroutine);
        }

        private IEnumerator GymRoutine()
        {
            while (true)
            {
                GameState.IncrementPower();
                yield return new WaitForSeconds(Balance.GymTickInterval);
                yield return null;
            }
        }

        private void StartAutoJump()
        {
            _playerMovement.EnableJump = false;
            _autoJump = true;
            _playerMovement.Jump(GetTrampolineJumpHeight());
            EventBus.RaiseAutoJumpStarted();
        }

        private void StopAutoJump()
        {
            _playerMovement.EnableJump = true;
            _autoJump = false;
            EventBus.RaiseAutoJumpStopped();
        }

        private void Teleport(Vector3 position, Quaternion? rotation = null)
        {
            _playerController.enabled = false;
            player.transform.position = position;
            if (rotation.HasValue)
                player.transform.rotation = rotation.Value;
            _playerController.enabled = true;
        }

        private void OnDestroy()
        {
            _playerMovement.OnLand.RemoveListener(HandleLand);
            EventBus.Upgrade.RemoveListener(Upgrade);
            EventBus.BuyEgg.RemoveListener(BuyEgg);
            EventBus.ActivatePetRequest.RemoveListener(ActivatePet);
            EventBus.DeactivatePetRequest.RemoveListener(DeactivatePet);
            EventBus.SaveLoaded.RemoveListener(ApplyTrampolineModel);
        }
    }
}
