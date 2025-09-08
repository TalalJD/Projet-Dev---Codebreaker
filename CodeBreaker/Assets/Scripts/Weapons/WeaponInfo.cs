using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon Object", menuName = "WeaponSystem/WeaponObject")]
public class WeaponInfo : ScriptableObject
{
    public GameObject weaponPrefab;
    public GameObject bulletPrefab;
    public int damage;
    public int attackSpeed;
    public Sprite bulletSprite;
    public Sprite gunSprite;

}
