using UnityEngine;
using YG;

namespace _Game.Code.Scripts.Leaderboard
{
    public class YaLeaderBoard : MonoBehaviour
    {
        private void Start()
        {
            EventBus.CoinsChanged.AddListener(HandleLeaderBoard);
        }

        private void HandleLeaderBoard()
        {
            YG2.SetLeaderboard("trampolines", (int)GameState.Coins);
        }

        private void OnDestroy()
        {
            EventBus.CoinsChanged.RemoveListener(HandleLeaderBoard);
        }
    }
}