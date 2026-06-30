using UnityEngine;

namespace _Game.Code.Scripts
{
    public class TrampolineGo : MonoBehaviour
    {
        [field:SerializeField] public GameObject Trampoline { get; private set; }
        [field:SerializeField] public GameObject Trigger { get; private set; }
        [field:SerializeField] public Transform StandPoint { get; private set; }

        public void SwapModel(GameObject prefab)
        {
            if (Trampoline != null)
                Destroy(Trampoline);

            if (prefab != null)
                Trampoline = Instantiate(prefab, transform);
        }
    }
}