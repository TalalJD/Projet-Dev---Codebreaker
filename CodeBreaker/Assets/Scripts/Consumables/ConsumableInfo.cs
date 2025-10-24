using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Consumable Object", menuName = "ConsumableSystem/ConsumableObject")]
public class ConsumableInfo : ScriptableObject
{
    public Sprite consumableSprite;
    public GameObject consumablePrefab;
}
