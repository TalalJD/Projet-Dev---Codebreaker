using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkingEnnemy : Ennemy
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
    [SerializeField] private float idleRange = 20f;
    [SerializeField] private float inAttackRange = 1f;
    protected float distanceTarget;

    // Patrol fields
    [Header("Patrol (idle)")]
    [SerializeField] private bool enablePatrol = true;
    [SerializeField] private float patrolRange = 3f; // half-range from start position
    private float patrolCenterX;
    private float patrolMinX;
    private float patrolMaxX;
    private int patrolDirection = 1; // 1 = right, -1 = left

    // Idle patrol timing: walk for X seconds, then stop for Y seconds
    [SerializeField] private float patrolMoveDuration = 2f;
    [SerializeField] private float patrolStopDuration = 2f;
    private float patrolTimer;
    private bool patrolMoving = true;
    private bool wasInIdleRange = false;

    protected override void Start()
    {
        base.Start();
        _renderer = capsuleHitbox.GetComponent<SpriteRenderer>();
        capsuleHitbox.SetActive(false);
        _renderer.enabled = false;

        // initialize patrol bounds around start position
        patrolCenterX = transform.position.x;
        patrolMinX = patrolCenterX - patrolRange;
        patrolMaxX = patrolCenterX + patrolRange;

        // initialize patrol timer/state
        patrolMoving = true;
        patrolTimer = patrolMoveDuration;
        wasInIdleRange = false;
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

        // aggro = inside yellow circle
        bool aggro = distanceTarget <= aggroRange;
        // withinIdleRange = inside cyan circle (player close enough that enemy should idle/patrol if not aggro)
        bool withinIdleRange = distanceTarget <= idleRange;
        bool tryAttack = distanceTarget <= inAttackRange;

        if (inAttack)
        {
            // currently performing attack animation/hit - don't move
            _velocity.x = 0;
        }
        else if (tryAttack)
        {
            // Player is inside attack range: stop moving toward the player so the enemy doesn't push into them.
            _velocity.x = 0;
            // still face the player
            RegarderJoueur(_targetDirection.x);
        }
        else if (aggro)
        {
            // chase the player only when outside attack range
            float directionX = Mathf.Sign(_targetDirection.x) * moveSpeed;
            _velocity.x = directionX;
            RegarderJoueur(_targetDirection.x);

            // leaving idle region - reset patrol cycle so it restarts next time
            wasInIdleRange = false;
        }
        else if (withinIdleRange && enablePatrol)
        {
            // Player is within idle range but not aggro -> patrol with timed walk/stop cycle

            // if we just entered idle range, reset the patrol cycle
            if (!wasInIdleRange)
            {
                wasInIdleRange = true;
                patrolMoving = true;
                patrolTimer = patrolMoveDuration;
            }

            // update patrol timer
            patrolTimer -= Time.fixedDeltaTime;

            if (patrolMoving)
            {
                DoPatrol();
            }
            else
            {
                // stopped phase
                _velocity.x = 0;
            }

            if (patrolTimer <= 0f)
            {
                // toggle moving/stopped and reset timer accordingly
                patrolMoving = !patrolMoving;
                patrolTimer = patrolMoving ? patrolMoveDuration : patrolStopDuration;
            }
        }
        else
        {
            // Player is further than idle range (or patrol disabled) -> stop moving
            _velocity.x = 0;

            // reset patrol cycle so it starts fresh next time we enter idle range
            wasInIdleRange = false;
            patrolMoving = true;
            patrolTimer = patrolMoveDuration;
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

        if (_velocity.x != 0)
        {
            animator.SetBool("IsWalking", true);
        }
        else
        {
            animator.SetBool("IsWalking", false);
        }
    }

    private void DoPatrol()
    {
        // Determine direction toward current patrol target and move
        float targetX = patrolDirection > 0 ? patrolMaxX : patrolMinX;
        float diff = targetX - transform.position.x;

        // If very close to target, flip direction
        const float epsilon = 0.05f;
        if (Mathf.Abs(diff) <= epsilon)
        {
            patrolDirection *= -1;
            targetX = patrolDirection > 0 ? patrolMaxX : patrolMinX;
            diff = targetX - transform.position.x;
        }

        float dir = Mathf.Sign(diff);
        if (dir == 0) dir = patrolDirection; // fallback

        _velocity.x = dir * moveSpeed;

        // make enemy face patrol direction
        RegarderJoueur(dir);
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
        base.Attack();
        StartCoroutine(DoAttack());
    }

    private IEnumerator DoAttack()
    {
        _renderer.enabled = false;
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
        Gizmos.color = Color.cyan; Gizmos.DrawWireSphere(transform.position, idleRange);
        Gizmos.color = Color.red; Gizmos.DrawWireSphere(transform.position, inAttackRange);

        // draw patrol bounds in editor
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(new Vector3(patrolMinX, transform.position.y - 0.2f, 0f), new Vector3(patrolMaxX, transform.position.y - 0.2f, 0f));
    }
}
