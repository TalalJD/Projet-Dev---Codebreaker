using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon Object", menuName = "WeaponSystem/WeaponObject")]
public class WeaponInfo : ScriptableObject
{
    public Sprite weaponSprite;
    public GameObject bulletPrefab;
    public GameObject weaponPrefab;
    public float damage;
    public float attackSpeed;
    public float bulletSpeed;
    public float bulletLifeTime;
    
}
