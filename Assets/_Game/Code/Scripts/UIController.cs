using UnityEngine;
using UnityEngine.UI;

namespace _Game.Code.Scripts
{
    public class UIController : MonoBehaviour
    {
        [SerializeField] private Button _enterTrampolineButton;
        [SerializeField] private Button _exitTrampolineButton;
        [SerializeField] private Button _autoJumpButton;
        [SerializeField] private Button _enterGymButton;
        [SerializeField] private Button _exitGymButton;
        [SerializeField] private UIShop _shop;
        [SerializeField] private UIEggShop _eggShop;
        [SerializeField] private UIPlayerPets _playerPets;
        [SerializeField] private UIGymProgressBar _gymProgressBar;
        [SerializeField] private UIDonateShop _donateShop;
        [SerializeField] private UIRebirthPopup _rebirthPopup;
        [SerializeField] private UISettingsPopup _settingsPopup;

        private bool _isPlayerPetsOpen = false;
        private bool _isDonateShopOpen = false;
        private bool _isRebirthPopupOpen = false;
        private bool _isSettingsPopupOpen = false;

        private void Start()
        {
            EventBus.EnterToTrampoline.AddListener(HideTrampolineEnterButton);
            EventBus.EnterToTrampoline.AddListener(ShowAutoJumpButton);
            EventBus.EnterToTrampoline.AddListener(ShowTrampolineExitButton);
            EventBus.AutoJumpStarted.AddListener(HideTrampolineExitButton);
            EventBus.AutoJumpStopped.AddListener(ShowTrampolineExitButton);

            EventBus.ExitToTrampoline.AddListener(HideTrampolineExitButton);
            EventBus.ExitToTrampoline.AddListener(ShowTrampolineEnterButton);
            EventBus.ExitToTrampoline.AddListener(HideAutoJumpButton);

            EventBus.EnterToGym.AddListener(HideGymEnterButton);
            EventBus.EnterToGym.AddListener(ShowGymExitButton);
            EventBus.EnterToGym.AddListener(ShowGymProgressBar);

            EventBus.ExitToGym.AddListener(HideGymExitButton);
            EventBus.ExitToGym.AddListener(ShowGymEnterButton);
            EventBus.ExitToGym.AddListener(HideGymProgressBar);
        }

        public void ShowTrampolineEnterButton() => _enterTrampolineButton.gameObject.SetActive(true);
        public void HideTrampolineEnterButton() => _enterTrampolineButton.gameObject.SetActive(false);

        public void ShowTrampolineExitButton() => _exitTrampolineButton.gameObject.SetActive(true);
        public void HideTrampolineExitButton() => _exitTrampolineButton.gameObject.SetActive(false);

        public void ShowGymEnterButton() => _enterGymButton.gameObject.SetActive(true);
        public void HideGymEnterButton() => _enterGymButton.gameObject.SetActive(false);

        public void ShowGymExitButton()    => _exitGymButton.gameObject.SetActive(true);
        public void HideGymExitButton()    => _exitGymButton.gameObject.SetActive(false);

        public void ShowGymProgressBar()   => _gymProgressBar.gameObject.SetActive(true);
        public void HideGymProgressBar()   => _gymProgressBar.gameObject.SetActive(false);

        public void ShowAutoJumpButton() => _autoJumpButton.gameObject.SetActive(true);
        public void HideAutoJumpButton() => _autoJumpButton.gameObject.SetActive(false);

        public void ShowWingsShop()
        {
            _shop.gameObject.SetActive(true);
            _shop.Initialize(GameState.WingsUpgrades);
        }

        public void ShowTrampolineShop()
        {
            _shop.gameObject.SetActive(true);
            _shop.Initialize(GameState.TrampolineUpgrades);
        }

        public void HideShop() => _shop.gameObject.SetActive(false);

        public void ShowEggShop(int eggTierIndex)
        {
            _eggShop.Initialize(GameState.Eggs[eggTierIndex]);
            _eggShop.gameObject.SetActive(true);
        }

        public void HideEggShop() => _eggShop.gameObject.SetActive(false);

        public void HandleDonateShopButton()
        {
            if (_isDonateShopOpen) HideDonateShop();
            else ShowDonateShop();

            _isDonateShopOpen = !_isDonateShopOpen;
        }

        public void ShowDonateShop() => _donateShop.gameObject.SetActive(true);
        public void HideDonateShop() => _donateShop.gameObject.SetActive(false);

        public void HandleRebirthPopupButton()
        {
            if (_isRebirthPopupOpen) HideRebirthPopup();
            else ShowRebirthPopup();

            _isRebirthPopupOpen = !_isRebirthPopupOpen;
        }

        public void ShowRebirthPopup() => _rebirthPopup.gameObject.SetActive(true);
        public void HideRebirthPopup() => _rebirthPopup.gameObject.SetActive(false);

        public void HandleSettingsPopupButton()
        {
            if (_isSettingsPopupOpen) HideSettingsPopup();
            else ShowSettingsPopup();

            _isSettingsPopupOpen = !_isSettingsPopupOpen;
        }

        public void ShowSettingsPopup() => _settingsPopup.gameObject.SetActive(true);
        public void HideSettingsPopup() => _settingsPopup.gameObject.SetActive(false);

        public void HandlePlayerPetsButton()
        {
            if (_isPlayerPetsOpen) HidePlayerPets();
            else ShowPlayerPets();
            
            _isPlayerPetsOpen = !_isPlayerPetsOpen;
        }
        public void ShowPlayerPets() => _playerPets.gameObject.SetActive(true);
        public void HidePlayerPets() => _playerPets.gameObject.SetActive(false);

        private void OnDestroy()
        {
            EventBus.EnterToTrampoline.RemoveListener(HideTrampolineEnterButton);
            EventBus.EnterToTrampoline.RemoveListener(ShowAutoJumpButton);
            EventBus.EnterToTrampoline.RemoveListener(ShowTrampolineExitButton);

            EventBus.ExitToTrampoline.RemoveListener(HideTrampolineExitButton);
            EventBus.ExitToTrampoline.RemoveListener(ShowTrampolineEnterButton);
            EventBus.ExitToTrampoline.RemoveListener(HideAutoJumpButton);
            EventBus.AutoJumpStarted.RemoveListener(HideTrampolineExitButton);
            EventBus.AutoJumpStopped.RemoveListener(ShowTrampolineExitButton);

            EventBus.EnterToGym.RemoveListener(HideGymEnterButton);
            EventBus.EnterToGym.RemoveListener(ShowGymExitButton);
            EventBus.EnterToGym.RemoveListener(ShowGymProgressBar);

            EventBus.ExitToGym.RemoveListener(HideGymExitButton);
            EventBus.ExitToGym.RemoveListener(ShowGymEnterButton);
            EventBus.ExitToGym.RemoveListener(HideGymProgressBar);
        }
    }
}
