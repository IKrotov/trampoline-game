using UnityEngine;
using UnityEngine.InputSystem;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] private Vector3 _offset = new Vector3(0f, 5f, -7f);
    [SerializeField] private float _smoothSpeed = 5f;
    [SerializeField] private float _rotationSpeed = 3f;
    [SerializeField] private float _minPitch = -80f;

    private float _yaw;
    private float _pitch;
    private float _currentYaw;
    private float _currentPitch;

    private void Start()
    {
        Vector3 angles = transform.eulerAngles;
        _yaw = _currentYaw = angles.y;
        _pitch = _currentPitch = angles.x;
    }

    private void LateUpdate()
    {
        if (_target == null) return;

        if (Mouse.current.rightButton.isPressed)
        {
            Vector2 delta = Mouse.current.delta.ReadValue();
            _yaw   += delta.x * _rotationSpeed * Time.deltaTime * 100f;
            _pitch -= delta.y * _rotationSpeed * Time.deltaTime * 100f;
            _pitch  = Mathf.Clamp(_pitch, _minPitch, 80f);
        }

        _currentYaw   = Mathf.Lerp(_currentYaw,   _yaw,   _smoothSpeed * Time.deltaTime);
        _currentPitch = Mathf.Lerp(_currentPitch, _pitch, _smoothSpeed * Time.deltaTime);

        Quaternion rotation = Quaternion.Euler(_currentPitch, _currentYaw, 0f);
        transform.position = _target.position + rotation * _offset;
        transform.rotation = Quaternion.LookRotation(_target.position - transform.position, rotation * Vector3.up);
    }
}
