using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnnemy : Ennemy
{
    private float moveSpeed = 5f;
    private float gravity = -30f;
    private float maxFallSpeed = -20f;

    [SerializeField] private LayerMask LayerMask;
    private float groundCheck = 0.55f;

    private Vector2 _velocity;
    private bool _isGrounded;
    [SerializeField] private GameObject capsuleHitbox;
    private SpriteRenderer _renderer;
    private float attackDuration = 0.5f;

    protected override void Start()
    {
        base.Start();
        _renderer = capsuleHitbox.GetComponent<SpriteRenderer>();
        capsuleHitbox.SetActive(false);
        _renderer.enabled = false;
    }
    protected void FixedUpdate()
    {
        if (!inAttack)
        {
            float directionX = Mathf.Sign(_targetDirection.x) * moveSpeed;
            _velocity.x = directionX;
        }
        else
        {
            _velocity.x = 0;
        }

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
        if (CanAttack() && !inAttack)
        {
            Attack();
        }
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
        StartCoroutine(DoAttack());
    }

    private IEnumerator DoAttack()
    {
        _renderer.enabled = true;
        inAttack = true;
        capsuleHitbox.SetActive(true);
        yield return new WaitForSeconds(attackDuration);
        inAttack = false;
        _renderer.enabled = false;
        capsuleHitbox.SetActive(false);
        ResetCooldown();
    }
}
