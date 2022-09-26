using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float _speed;
    [SerializeField] private Hook _hook;

    private Rigidbody2D _rigidbody;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();       
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _hook.CreateHook();
        }

        if (Input.GetMouseButtonDown(1))
        {
            _hook.DisableHook();
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            _rigidbody.AddForce(Vector2.up * 500);
        }
    }

    private void FixedUpdate()
    {
        if (!_hook.enabled)
        {
            float horizontalDirection = Input.GetAxisRaw("Horizontal");
            _rigidbody.AddForce(new Vector2(horizontalDirection, 0) * _speed * Time.fixedDeltaTime, 
            ForceMode2D.Impulse);
        }
    }
}
