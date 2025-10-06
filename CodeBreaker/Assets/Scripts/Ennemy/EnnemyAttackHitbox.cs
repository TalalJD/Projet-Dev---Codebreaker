using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnnemyAttackHitbox : MonoBehaviour
{
    [SerializeField] public EnnemyInfo _ennemyInfo;
    
    //private void OnTriggerEnter2D(Collider2D other)
    //{
    //    if (other.CompareTag("Player"))
    //    {
    //        Player player = other.GetComponent<Player>();
    //        player.ModifyHealth(-_ennemyInfo.attackDamage);
    //        Debug.Log("Touching player !");
    //    }
    //}
}
