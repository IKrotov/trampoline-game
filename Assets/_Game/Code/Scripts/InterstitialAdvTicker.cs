using UnityEngine;
using YG;

namespace _Game.Code.Scripts
{
    public class InterstitialAdvTicker : MonoBehaviour
    {
        private void Start() => InvokeRepeating(nameof(TryShow), 1f, 1f);

        private void TryShow() => YG2.InterstitialAdvShow();
    }
}
