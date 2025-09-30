using System.Collections;
using System.Collections.Generic;
using Unity.Loading;
using UnityEngine;

public class EnnemyBullet : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag != "Player" && collision.tag != "Enemy" && collision.tag != "Boss" && collision.tag !="EnemyBullet")
        {
            Destroy(gameObject);
        }
        
    }
}
