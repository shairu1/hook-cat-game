using System;
using System.Numerics;
using Unity.VisualScripting;

using UnityEngine;
using Vector2 = UnityEngine.Vector2;


public class Player : MonoBehaviour
{
    [SerializeField] private Hook _hook;

    public float speed;
    public float maxSpeed;
    public float brakingSpeed;
    public float JumpForce;
    public float StompForce;
    // public float GroundCheckDistance;
    // public float objectMass;

    private Rigidbody2D _rigidbody;
    private bool _isGround;


    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        UseHook();
        UpdateVerticalPosition();
        UpdateHorizontalPosition();

        UIManager.SetVelocityText(((int)(_rigidbody.velocity.magnitude * 10) / 10.0f).ToString());
    }

    /// <summary>
    /// Обновляет позицию игрока при нажатии W и S.
    /// W - прыжок
    /// S - ускорение вертикально вниз, для большей маневренности
    /// </summary>
    private void UpdateVerticalPosition()
    {
        if (Input.GetKeyDown(KeyCode.W) && _isGround)
        {
            _rigidbody.AddForce(Vector2.up * JumpForce);
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            _rigidbody.AddForce(Vector2.down * StompForce);
        }

    }
    /// <summary>
    /// Обновляет позицию игрока при нажатии A и D.
    /// Общий принцип остался тот же, но сделан небольшой рефактор.
    /// Также добавлена возможность передвижения в воздухе и внутри проекта
    /// исправлена возможность прикрепляться к стенам.
    /// </summary>
    private void UpdateHorizontalPosition()
    {
        Vector2 velocity = _rigidbody.velocity;

        if (_hook.enabled)
            return;

        float horizontalDirection = (Input.GetKey(KeyCode.A) || Input.GetKeyDown(KeyCode.A)) ? -1 :
            ((Input.GetKeyDown(KeyCode.D) || Input.GetKey(KeyCode.D)) ? 1 : 0);
        if (horizontalDirection != 0)
        {
            _rigidbody.AddForce(new Vector2(horizontalDirection, 0) * speed, ForceMode2D.Impulse);

            if (!(Mathf.Abs(_rigidbody.velocity.x) > maxSpeed))
                return;

            velocity.x = _rigidbody.velocity.x > 0 ? maxSpeed : -maxSpeed;
            _rigidbody.velocity = velocity;
        }
        else if (_rigidbody.velocity.x != 0)
        {
            velocity.x = Mathf.Lerp(velocity.x, 0, brakingSpeed * Time.fixedDeltaTime * 5);
            _rigidbody.velocity = velocity;
        }
    }
    /// <summary>
    /// Применение крюка-кошки.
    /// </summary>
    private void UseHook()
    {
        if (Input.GetKeyUp(KeyCode.LeftShift) && _hook.enabled)
        {
            _hook.DestroyHook();
        }
        else if (Input.GetKeyDown(KeyCode.LeftShift))
        {
            _hook.CreateHook();
        }
    }

    /// <summary>
    /// Проверка наличия коллизий
    /// </summary>
    /// <param name="collision2D">Детали коллизии, можно использовать позже, на данный момент исользуется для повторения старого функционала</param>
    private void OnCollisionEnter2D(Collision2D collision2D)
    {
        _isGround = collision2D.gameObject.CompareTag("Ground");
    }
    private void OnCollisionExit2D()
    {
        _isGround = false;
    }

    private void FixedUpdate()
    {

    }
    
}
