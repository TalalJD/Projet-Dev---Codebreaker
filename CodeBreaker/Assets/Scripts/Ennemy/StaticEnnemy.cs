using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticEnnemy : Ennemy
{
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float projectileSpeed = 12f;
    [SerializeField] private float attackRange = 10f;

    [SerializeField] private Vector2 followOffset = new Vector2(2f, 4f);
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float maxY = 0f;
    protected float distanceTarget;
    private Rigidbody2D _rb;

    protected override void Start()
    {
        base.Start();
        if (firePoint == null)
        {
            firePoint = transform;
        }
        _rb = GetComponent<Rigidbody2D>();
        ResetCooldown();
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

        if (distanceTarget <= attackRange)
        {
            if (CanAttack())
            {
                RegarderJoueur(_targetDirection.x);
                Attack();
            }
            else
            {
                Move();
            }
        }
    }

    private void Move()
    {
        Vector2 direction = (Vector2)_target.position + followOffset;

        direction.y = Mathf.Min(direction.y, maxY);


        Vector2 newPos = Vector2.MoveTowards(
            _rb.position,
            direction,
            moveSpeed * Time.fixedDeltaTime
        );

        _rb.MovePosition(newPos);
    }
    public override void Attack()
    {
        GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
        Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.linearVelocity = _targetDirection.normalized * projectileSpeed;
        }
        ResetCooldown();
    }
}
