using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [field:SerializeField] public bool EnableWasd { get; set; } = true;
    [field:SerializeField] public bool EnableJump { get; set; } = true;
    
    public UnityEvent OnJump = new();
    public UnityEvent<float> OnLand = new();

    [SerializeField] private float _speed = 5f;
    [SerializeField] private float _rotationSpeed = 10f;
    [SerializeField] private float _jumpHeight = 1.5f;
    [SerializeField] private float _gravity = -20f;
    [SerializeField] private Transform _camera;

    private CharacterController _controller;
    private Vector3 _velocity;

    private bool _wasGrounded;
    private float _takeoffY;
    private float _peakY;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
    }

    public bool IsGrounded() => _controller.isGrounded;

    private void Update()
    {
        var keyboard = Keyboard.current;
        if (keyboard == null) return;

        bool isGrounded = IsGrounded();

        if (!_wasGrounded && isGrounded)
        {
            float height = _peakY - _takeoffY;
            if (height > 0.05f) OnLand.Invoke(height);
        }

        if (_wasGrounded && !isGrounded)
        {
            _takeoffY = transform.position.y;
            _peakY = _takeoffY;
        }

        if (!isGrounded && transform.position.y > _peakY)
            _peakY = transform.position.y;

        _wasGrounded = isGrounded;

        if (isGrounded && _velocity.y < 0f)
            _velocity.y = -2f;

        if (EnableWasd)
        {
            float horizontal = (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed ? 1f : 0f)
                               - (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed  ? 1f : 0f);
            float vertical   = (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed    ? 1f : 0f)
                               - (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed  ? 1f : 0f);

            Vector3 input = new Vector3(horizontal, 0f, vertical);

            if (input.sqrMagnitude > 0.01f)
            {
                Vector3 camForward = _camera.forward;
                Vector3 camRight   = _camera.right;
                camForward.y = 0f;
                camRight.y   = 0f;
                camForward.Normalize();
                camRight.Normalize();

                Vector3 direction = (camForward * vertical + camRight * horizontal).normalized;

                _controller.Move(direction * (_speed * Time.deltaTime));

                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, _rotationSpeed * Time.deltaTime);
            }
        }

        if (EnableJump && keyboard.spaceKey.wasPressedThisFrame && isGrounded && _velocity.y <= 0f)
            Jump();


        _velocity.y += _gravity * Time.deltaTime;
        _controller.Move(_velocity * Time.deltaTime);
    }

    public void Jump()
    {
        _velocity.y = Mathf.Sqrt(_jumpHeight * -2f * _gravity);
        OnJump.Invoke();
    }

    public void Jump(float jumpHeight)
    {
        _velocity.y = Mathf.Sqrt(jumpHeight * -2f * _gravity);
        OnJump.Invoke();
    }
}
