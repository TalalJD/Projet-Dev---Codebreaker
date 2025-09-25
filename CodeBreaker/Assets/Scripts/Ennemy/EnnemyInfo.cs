using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Ennemy Object", menuName = "EnnemySystem/EnnemyObject")]
public class EnnemyInfo : ScriptableObject
{
    public Sprite ennemySprite;
    public GameObject ennemyPrefab;
    public float movementSpeed;
    public float healthPoints;
    public float attackSpeed;
    public int attackDamage;
}
