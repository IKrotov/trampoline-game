#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.InputSystem;

namespace _Game.Code.Scripts
{
    public class Cheats : MonoBehaviour
    {
        private const long   COINS_AMOUNT  = 1_000_000;
        private const double POWER_AMOUNT  = 1_000;
        private const int    TRAMPOLINE_MAX = 10;

        private bool _visible = true;

        private void Update()
        {
            var kb = Keyboard.current;
            if (kb == null) return;

            if (kb.backquoteKey.wasPressedThisFrame) _visible = !_visible;

            if (kb.f1Key.wasPressedThisFrame) AddCoins();
            if (kb.f2Key.wasPressedThisFrame) AddPower();
            if (kb.f3Key.wasPressedThisFrame) MaxTrampoline();
            if (kb.f4Key.wasPressedThisFrame) ResetSave();
        }

        private void OnGUI()
        {
            if (!_visible) return;

            GUILayout.BeginArea(new Rect(10, 10, 220, 200));
            GUILayout.BeginVertical(GUI.skin.box);

            GUILayout.Label("CHEATS  [ ` — скрыть ]");

            if (GUILayout.Button($"F1 +{COINS_AMOUNT:N0} монет"))  AddCoins();
            if (GUILayout.Button($"F2 +{POWER_AMOUNT:N0} силы"))   AddPower();
            if (GUILayout.Button("F3  Батут → макс"))              MaxTrampoline();
            if (GUILayout.Button("F4  Сброс сохранения"))          ResetSave();

            GUILayout.EndVertical();
            GUILayout.EndArea();
        }

        private void AddCoins() => GameState.AddCoins(COINS_AMOUNT);

        private void AddPower() => GameState.AddPower(POWER_AMOUNT);

        private void MaxTrampoline()
        {
            GameState.Trampoline.Level = TRAMPOLINE_MAX;
            EventBus.RaiseSaveLoaded(); // GameController слушает это и вызывает SwapModel
        }

        private void ResetSave()
        {
            PlayerPrefs.DeleteAll();
            UnityEngine.SceneManagement.SceneManager.LoadScene(0);
        }
    }
}
#endif
