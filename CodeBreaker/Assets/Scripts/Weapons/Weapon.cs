using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    [SerializeField] protected WeaponInfo _weaponInfo;
    [SerializeField] protected Transform firingPoint;
    private float attackCooldown;


    protected virtual void Start()
    {
        attackCooldown = 0f;
    }
    public abstract void Attack();

    protected virtual void Update()
    {
        if (attackCooldown > 0f)
        {
            attackCooldown -= Time.deltaTime;
        }
    }

    public virtual bool CanAttack()
    {
        return attackCooldown <= 0f;
    }

    protected virtual void ResetCooldown()
    {
        attackCooldown = 1f / Mathf.Max(1, _weaponInfo.attackSpeed);
    }
    public WeaponInfo Info => _weaponInfo;
}
