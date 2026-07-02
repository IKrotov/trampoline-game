using UnityEngine.Events;

namespace _Game.Code.Scripts
{
    public static class EventBus
    {
        public static readonly UnityEvent<string> LangChanged = new();
        public static void RaiseLangChanged(string langCode) => LangChanged.Invoke(langCode);
        public static readonly UnityEvent EnterToTrampoline = new();
        public static readonly UnityEvent ExitToTrampoline = new();
        public static readonly UnityEvent AutoJumpStarted = new();
        public static readonly UnityEvent AutoJumpStopped = new();

        public static readonly UnityEvent EnterToGym = new();
        public static readonly UnityEvent ExitToGym = new();

        public static readonly UnityEvent CoinsChanged = new();
        public static readonly UnityEvent CoinsEarned = new();
        public static readonly UnityEvent PowerChanged = new();
        public static readonly UnityEvent RestartsChanged = new();
        public static readonly UnityEvent SaveLoaded = new();
        public static readonly UnityEvent AudioSettingsChanged = new();

        public static readonly UnityEvent<Upgrade> Upgrade = new();
        public static readonly UnityEvent UpgradeApplied = new();

        // Pet commands (UI → GameController)
        public static readonly UnityEvent<EggType> BuyEgg = new();
        public static readonly UnityEvent<OwnedPet> ActivatePetRequest = new();
        public static readonly UnityEvent<OwnedPet> DeactivatePetRequest = new();
        public static readonly UnityEvent RebirthRequest = new();

        // Pet results (GameController → world/UI)
        public static readonly UnityEvent<OwnedPet> PetHatched = new();
        public static readonly UnityEvent<OwnedPet> PetActivated = new();
        public static readonly UnityEvent<OwnedPet> PetDeactivated = new();

        public static void RaiseEnterToTrampoline() => EnterToTrampoline.Invoke();
        public static void RaiseExitToTrampoline() => ExitToTrampoline.Invoke();
        public static void RaiseAutoJumpStarted() => AutoJumpStarted.Invoke();
        public static void RaiseAutoJumpStopped() => AutoJumpStopped.Invoke();
        public static void RaiseEnterToGym() => EnterToGym.Invoke();
        public static void RaiseExitToGym() => ExitToGym.Invoke();
        public static void RaiseCoinsChanged() => CoinsChanged.Invoke();
        public static void RaiseCoinsEarned() => CoinsEarned.Invoke();
        public static void RaisePowerChanged() => PowerChanged.Invoke();
        public static void RaiseRestartsChanged() => RestartsChanged.Invoke();
        public static void RaiseSaveLoaded() => SaveLoaded.Invoke();
        public static void RaiseAudioSettingsChanged() => AudioSettingsChanged.Invoke();
        public static void RaiseUpgrade(Upgrade upgrade) => Upgrade.Invoke(upgrade);
        public static void RaiseUpgradeApplied() => UpgradeApplied.Invoke();
        public static void RaiseBuyEgg(EggType egg) => BuyEgg.Invoke(egg);
        public static void RaiseActivatePetRequest(OwnedPet pet) => ActivatePetRequest.Invoke(pet);
        public static void RaiseDeactivatePetRequest(OwnedPet pet) => DeactivatePetRequest.Invoke(pet);
        public static void RaiseRebirthRequest() => RebirthRequest.Invoke();
        public static void RaisePetHatched(OwnedPet pet) => PetHatched.Invoke(pet);
        public static void RaisePetActivated(OwnedPet pet) => PetActivated.Invoke(pet);
        public static void RaisePetDeactivated(OwnedPet pet) => PetDeactivated.Invoke(pet);
    }
}