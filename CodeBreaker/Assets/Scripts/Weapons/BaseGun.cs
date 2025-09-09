using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseGun : Weapon
{
    public override void Attack()
    {
        if (CanAttack())
        {
            GameObject bullet = Instantiate(_weaponInfo.bulletPrefab, _weaponInfo.firingPoint);
            ResetCooldown();
        }
    }

}
