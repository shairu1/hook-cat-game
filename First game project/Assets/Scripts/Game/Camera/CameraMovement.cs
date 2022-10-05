using UnityEngine;


public class CameraMovement : MonoBehaviour
{
    private static CameraMovement instance;

    public Transform Target;
    public float Speed;
    public float Size; // use SetSize()
    public float SizeSpeed;
    public float MinDeltaSize;

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
        if (Size != _camera.orthographicSize)
        {
            if (Size > _camera.orthographicSize)
                _camera.orthographicSize += SizeSpeed * Time.fixedDeltaTime;
            else
                _camera.orthographicSize -= SizeSpeed * Time.fixedDeltaTime;


            if (Mathf.Abs(Size - _camera.orthographicSize) <= MinDeltaSize)
            {
                _camera.orthographicSize = Size;
            }
        }

        Vector3 targetPosition = Target.position;
        targetPosition.z = _transform.position.z;

        _transform.Translate(Vector3.Lerp(
            _transform.position, targetPosition, Speed * Time.fixedDeltaTime) - _transform.position, 
            Space.World);
    }

    public static void SetSize(float size, bool smoothly = true)
    {
        instance.Size = size;

        if (!smoothly)
        {
            instance._camera.orthographicSize = size;
        }
    }
}
