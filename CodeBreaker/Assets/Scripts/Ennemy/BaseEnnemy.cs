using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnnemy : Ennemy
{
    private float moveSpeed = 12f;
    private float gravity = -30f;
    private float maxFallSpeed = -20f;

    [SerializeField] private LayerMask LayerMask;
    private float groundCheck = 0.55f;

    private Vector2 _velocity;
    private bool _isGrounded;
    [SerializeField] private GameObject capsuleHitbox;
    private SpriteRenderer _renderer;
    private float attackDuration = 0.5f;
    private float aggroRange = 8f;
    private float stopAggroRange = 12f;
    private float inAttackRange = 2f;
    protected float distanceTarget;
    protected override void Start()
    {
        base.Start();
        _renderer = capsuleHitbox.GetComponent<SpriteRenderer>();
        capsuleHitbox.SetActive(false);
        _renderer.enabled = false;
    }
    protected void FixedUpdate()
    {
        if (_target != null)
        {
            distanceTarget = Vector2.Distance(transform.position, _target.position);
        }
        else
        {
            distanceTarget = Mathf.Infinity;
        }

        bool aggro = distanceTarget <= aggroRange;
        bool idle = distanceTarget >= stopAggroRange;
        bool tryAttack = distanceTarget <= inAttackRange;

        if (inAttack || idle)
        {
            _velocity.x = 0;
        }
        else if (aggro)
        {
            float directionX = Mathf.Sign(_targetDirection.x) * moveSpeed;
            _velocity.x = directionX;
            RegarderJoueur(_targetDirection.x);
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
        if (CanAttack() && !inAttack && tryAttack)
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
        if (_target == null)
        {
            return;
        }

        float distNow = Vector2.Distance(transform.position, _target.position);
        if (distNow > inAttackRange)
        {
            return;
        }
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
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow; Gizmos.DrawWireSphere(transform.position, aggroRange);
        Gizmos.color = Color.cyan; Gizmos.DrawWireSphere(transform.position, stopAggroRange);
        Gizmos.color = Color.red; Gizmos.DrawWireSphere(transform.position, inAttackRange);
    }
}
