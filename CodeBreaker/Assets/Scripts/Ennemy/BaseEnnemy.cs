using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnnemy : Ennemy
{
    private float moveSpeed = 10f;
    private float gravity = -30f;
    private float maxFallSpeed = -20f;

    [SerializeField] private LayerMask LayerMask;
    private float groundCheck = 0.55f;

    private Collider2D _col;
    private Vector2 _velocity;
    private bool _isGrounded;


    protected override void Start()
    {
        base.Start();
        _col = GetComponent<Collider2D>();
    }
    protected void FixedUpdate()
    {
        float directionX = Mathf.Sign(_targetDirection.x) * moveSpeed;
        _velocity.x = directionX;


        _isGrounded = CheckOnGround();

        if (!_isGrounded)
        {
            _velocity.y += gravity * Time.fixedDeltaTime;
            if (_velocity.y < maxFallSpeed)
            {
                _velocity.y = maxFallSpeed;
            }
        }
        else
        {
            _velocity.y = Mathf.Min(_velocity.y, -0.5f);
        }

        Vector2 mouvement = rigidBody.position + _velocity * Time.fixedDeltaTime;
        rigidBody.MovePosition(mouvement);
        Debug.Log(_isGrounded);
    }


    private bool CheckOnGround()
    {
        var hit = Physics2D.Raycast(transform.position, Vector2.down, groundCheck, LayerMask);

        Debug.DrawRay(transform.position, Vector2.down * .25f);
        if (hit)
        {
            float offset = 0.1f;
            rigidBody.position = new Vector2(rigidBody.position.x, rigidBody.position.y + offset);
            return true;
        }

        return false;
    }
    public override void Attack()
    {
    }

}
