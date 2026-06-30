using UnityEngine;
using _Game.Code.Scripts;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CharacterController))]
public class PlayerAnimator : MonoBehaviour
{
    private static readonly int SpeedHash = Animator.StringToHash("Speed");
    private static readonly int IsTrainingHash = Animator.StringToHash("IsTraining");

    private Animator _animator;
    private Vector3 _lastPosition;
    
    private bool _isTraining;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _lastPosition = transform.position;
    }

    private void OnEnable()
    {
        EventBus.EnterToGym.AddListener(OnEnterGym);
        EventBus.ExitToGym.AddListener(OnExitGym);
    }

    private void OnDisable()
    {
        EventBus.EnterToGym.RemoveListener(OnEnterGym);
        EventBus.ExitToGym.RemoveListener(OnExitGym);
    }

    private void OnEnterGym() => SetTraining(true);
    private void OnExitGym() => SetTraining(false);

    private void Update()
    {
        if (_isTraining) return;
        
        Vector3 delta = transform.position - _lastPosition;
        delta.y = 0f;
        float speed = delta.magnitude / Time.deltaTime;
        if (speed < 0.1f) speed = 0f;
        _lastPosition = transform.position;
        _animator.SetFloat(SpeedHash, speed);
    }

    public void SetTraining(bool isTraining)
    {
        //_animator.applyRootMotion = isTraining;
        _isTraining = isTraining;
        _animator.SetFloat(SpeedHash, 0);
        _animator.SetBool(IsTrainingHash, isTraining);
    }
}
