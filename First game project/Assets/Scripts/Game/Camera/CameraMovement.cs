using UnityEngine;


public class CameraMovement : MonoBehaviour
{
    private static CameraMovement instance;

    [SerializeField] private Transform _target;
    [SerializeField] private float _speed;
    [SerializeField] private float _size;
    [SerializeField] private float _sizeSpeed;
    [SerializeField] private float _minDeltaSize;

    private Transform _transform;
    private Camera _camera;

    void Start()
    {
        instance = this;
        _camera = GetComponent<Camera>();
        _transform = GetComponent<Transform>();
    }

    void FixedUpdate()
    {
        if (_size != _camera.orthographicSize)
        {
            if (_size > _camera.orthographicSize)
                _camera.orthographicSize += _sizeSpeed * Time.fixedDeltaTime;
            else
                _camera.orthographicSize -= _sizeSpeed * Time.fixedDeltaTime;


            if (Mathf.Abs(_size - _camera.orthographicSize) <= _minDeltaSize)
            {
                _camera.orthographicSize = _size;
            }
        }

        Vector3 targetPosition = _target.position;
        targetPosition.z = _transform.position.z;

        _transform.Translate(Vector3.Lerp(
            _transform.position, targetPosition, _speed * Time.fixedDeltaTime) - _transform.position, 
            Space.World);
    }

    public static void SetSize(float size, bool smoothly)
    {
        instance._size = size;

        if (!smoothly)
        {
            instance._camera.orthographicSize = size;
        }
    }
}
