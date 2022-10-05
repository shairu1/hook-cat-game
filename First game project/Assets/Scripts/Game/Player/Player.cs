using UnityEngine;


public class Player : MonoBehaviour
{
    [SerializeField] private Hook _hook;

    public float Speed;
    public float MaxSpeed;
    public float BrakingSpeed;
    public float JumpForce;
    public float GroundCheckDistance;

    private Rigidbody2D _rigidbody;
    private bool _isGround;


    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();       
    }

    private void Update()
    {
        CheckGround();

        if (Input.GetMouseButtonDown(0))
        {
            if (_hook.enabled)
                _hook.DisableHook();

            _hook.CreateHook();
        }

        if (Input.GetMouseButtonDown(1) && _hook.enabled)
        {
            _hook.DisableHook();
        }

        if (Input.GetKeyDown(KeyCode.W) && _isGround)
        {
            _rigidbody.AddForce(Vector2.up * JumpForce);
        }

        UIManager.SetVelocityText(((int)(_rigidbody.velocity.magnitude * 10) / 10.0f).ToString());
    }

    private void CheckGround()
    {
        RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, Vector2.down, GroundCheckDistance);
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].collider != null)
            {
                _isGround = hits[i].collider.tag == "Ground";
            }
        }
    }

    private void FixedUpdate()
    {
        if (!_hook.enabled && _isGround)
        {
            float horizontalDirection = (Input.GetKey(KeyCode.A) || Input.GetKeyDown(KeyCode.A)) ? -1 :
                ((Input.GetKeyDown(KeyCode.D) || Input.GetKey(KeyCode.D)) ? 1 : 0);

            if (horizontalDirection != 0)
            {
                _rigidbody.AddForce(new Vector2(horizontalDirection, 0) * Speed, ForceMode2D.Impulse);
                
                if (Mathf.Abs(_rigidbody.velocity.x) > MaxSpeed)
                {
                    Vector2 velocity = _rigidbody.velocity;
                    velocity.x = _rigidbody.velocity.x > 0 ? MaxSpeed : -MaxSpeed;
                    _rigidbody.velocity = velocity;
                }
            }
            else if (_rigidbody.velocity.x != 0)
            {
                Vector2 velocity = _rigidbody.velocity;
                velocity.x = Mathf.Lerp(velocity.x, 0, BrakingSpeed * Time.fixedDeltaTime * 5);
                _rigidbody.velocity = velocity;
            }
        }
    }
}
