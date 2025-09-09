using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon Object", menuName = "WeaponSystem/WeaponObject")]
public class WeaponInfo : ScriptableObject
{
    public GameObject weaponPrefab;
    public GameObject bulletPrefab;
    public float damage;
    public float attackSpeed;
    public Sprite bulletSprite;
    public Sprite gunSprite;
    public float bulletSpeed;
    public float bulletLifeTime;
    public Transform firingPoint;
}
