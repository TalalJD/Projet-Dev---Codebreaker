using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEnnemy : Ennemy
{
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject projectilePrefab; // optional fallback
    [SerializeField] private float projectileSpeed = 12f;

    [Header("Ranges")]
    [SerializeField, Tooltip("Distance at which the enemy will move/attack the player")]
    private float attackRange = 10f;

    [SerializeField, Tooltip("Distance used for editor visualization (not used to start/stop patrol)")]
    private float idleRange = 15f;

    [Header("Idle / Patrol")]
    [SerializeField, Tooltip("Horizontal half-range around spawn where the flying enemy will patrol while idling")]
    private float patrolRange = 3f;
    [SerializeField, Tooltip("Seconds to move during idle patrol phase")]
    private float patrolMoveDuration = 2f;
    [SerializeField, Tooltip("Seconds to stop during idle patrol phase")]
    private float patrolStopDuration = 2f;
    [SerializeField, Tooltip("Enable idle patrol behaviour")]
    private bool enableIdlePatrol = true;

    [Header("Movement")]
    [SerializeField] private Vector2 followOffset = new Vector2(2f, 4f);
    [SerializeField] private float moveSpeed = 4f;
    

    // runtime
    protected float distanceTarget;
    private Rigidbody2D _rb;

    // patrol runtime state
    private float patrolCenterX;
    private float patrolMinX;
    private float patrolMaxX;
    private int patrolDirection = 1;
    private float patrolTimer;
    private bool patrolMoving = true;

    protected override void Start()
    {
        base.Start();
        if (firePoint == null)
        {
            firePoint = transform;
        }
        _rb = GetComponent<Rigidbody2D>();
        ResetCooldown();

        // init patrol bounds around spawn position
        patrolCenterX = transform.position.x;
        patrolMinX = patrolCenterX - patrolRange;
        patrolMaxX = patrolCenterX + patrolRange;

        // init patrol timing
        patrolMoving = true;
        patrolTimer = patrolMoveDuration;
    }

    private void FixedUpdate()
    {
        if (_target != null)
        {
            distanceTarget = Vector2.Distance(transform.position, _target.position);
        }
        else
        {
            distanceTarget = Mathf.Infinity;
        }

        bool inAttackZone = distanceTarget <= attackRange;

        // If player in attack range and cooldown allows -> stop patrol briefly and shoot.
        // Otherwise do not interrupt patrol; keep performing idle patrol motion.
        if (inAttackZone && CanAttack())
        {
            // face the player and fire
            RegarderJoueur(_targetDirection.x);

            // interrupt patrol briefly when shooting: enter stop phase
            patrolMoving = false;
            patrolTimer = patrolStopDuration;

            Attack();
            // Attack() calls ResetCooldown(); after this frame Normal patrol cycle will continue.
        }

        // Idle / Patrol always runs (unless disabled). When shooting happens above, we already set patrolMoving=false and patrolTimer.
        if (enableIdlePatrol)
        {
            // decrement timer
            patrolTimer -= Time.fixedDeltaTime;

            if (patrolMoving)
            {
                DoPatrol();
            }
            else
            {
                // stopped phase - hold position (no horizontal movement)
                // nothing to do here
            }

            if (patrolTimer <= 0f)
            {
                patrolMoving = !patrolMoving;
                patrolTimer = patrolMoving ? patrolMoveDuration : patrolStopDuration;
            }
        }
    }

    private void DoPatrol()
    {
        if (_rb == null) return;

        // pick target X depending on patrol direction
        float targetX = patrolDirection > 0 ? patrolMaxX : patrolMinX;
        float diff = targetX - _rb.position.x;

        const float epsilon = 0.05f;
        if (Mathf.Abs(diff) <= epsilon)
        {
            // reached bound -> flip direction
            patrolDirection *= -1;
            targetX = patrolDirection > 0 ? patrolMaxX : patrolMinX;
            diff = targetX - _rb.position.x;
        }

        float dir = Mathf.Sign(diff);
        if (dir == 0) dir = patrolDirection;

        // Move horizontally toward the intended bound while maintaining Y
        Vector2 newPos = Vector2.MoveTowards(
            _rb.position,
            new Vector2(targetX, _rb.position.y),
            moveSpeed * Time.fixedDeltaTime
        );

        _rb.MovePosition(newPos);

        // face patrol direction (so the enemy doesn't "lock" visually on the player while patrolling)
        RegarderJoueur(dir);
    }

    public override void Attack()
    {
        base.Attack();

        // determine a reliable target position (re-query the player if _target was lost)
        Vector2 targetPos;
        if (_target != null)
            targetPos = _target.position;
        else
        {
            var go = GameObject.FindGameObjectWithTag("Player");
            targetPos = go != null ? (Vector2)go.transform.position : (Vector2)firePoint.position;
        }

        // Always use ProjectileManager spawn (returns Projectile instance so you can set speed)
        if (ProjectileManager.Instance != null)
        {
            var proj = ProjectileManager.Instance.Spawn(ProjectileType.EnemyBullet, firePoint.position, targetPos);
            if (proj != null)
            {
                proj.speed = projectileSpeed;
            }
        }
        else
        {
            // fallback: instantiate prefab and set velocity toward targetPos
            if (projectilePrefab != null && firePoint != null)
            {
                GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
                Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    var dir = (targetPos - (Vector2)firePoint.position).normalized;
                    rb.linearVelocity = dir * projectileSpeed;
                }
            }
        }

        ResetCooldown();
    }

    // Draw gizmos to visualize ranges in the editor when the object is selected
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, idleRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // draw patrol bounds
        Gizmos.color = Color.magenta;
        Vector3 min = new Vector3(patrolMinX, transform.position.y - 0.2f, 0f);
        Vector3 max = new Vector3(patrolMaxX, transform.position.y - 0.2f, 0f);
        Gizmos.DrawLine(min, max);

#if UNITY_EDITOR
        if (_target != null)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawLine(transform.position, _target.position);
        }
#endif
    }
}
