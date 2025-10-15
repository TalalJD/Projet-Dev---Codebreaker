using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticEnnemy : Ennemy
{
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject projectilePrefab;
    [SerializeField] private float projectileSpeed = 12f;
    [SerializeField] private float attackRange = 10f;

    protected override void Start()
    {
        base.Start();
        if (firePoint == null)
        {
            firePoint = transform;
        }
    }
    protected override void Update()
    {
        base.Update();
        if (CanAttack() && _targetDirection.magnitude <= attackRange)
        {
            Attack();
        }
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
