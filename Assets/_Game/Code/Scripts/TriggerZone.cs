using UnityEngine;
using UnityEngine.Events;

namespace _Game.Code.Scripts
{
    public class TriggerZone : MonoBehaviour
    {
        [SerializeField] private string _targetTag = "Player";
        
        public UnityEvent OnEnter;
        public UnityEvent OnExit;

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(_targetTag))
                OnEnter?.Invoke();
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag(_targetTag))
                OnExit?.Invoke();
        }
    }
}
