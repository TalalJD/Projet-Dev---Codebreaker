using System.Collections;
using System.Collections.Generic;
using Unity.Loading;
using UnityEngine;

public class EnnemyBullet : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Default")
        {
            Destroy(gameObject);
        }
        
    }
}
