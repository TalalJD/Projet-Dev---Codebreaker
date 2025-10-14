using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseGun : Weapon
{
    public override void Attack()
    {
        if (CanAttack())
        {
            GameObject bullet = Instantiate(_weaponInfo.bulletPrefab, firingPoint.position, firingPoint.rotation);
            ResetCooldown();
        }
    }
}
