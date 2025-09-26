using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GS_RandomBarrage : GromarState
{
    public override void OnEnter()
    {
     
    }
    public override void OnExit() { }


    public void ShootBulletBarrage()
    {
        int bulletNumber = 20;
        if (gromar == null || gromar.bigBullet == null)
        {
            Debug.Log("Gromar or bulletPrefab is not assigned");
            return;

        }

        for (int i = 0; i < bulletNumber; i++)
        {
            Vector3 spawnPosition = gromar.GetRandomShootPosition();
            GameObject bullet = GameObject.Instantiate(gromar.bigBullet, spawnPosition, Quaternion.identity);
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                // Always left, with random up/down angle
                Vector2 dir = new Vector2(-1f, Random.Range(-1f, 1f)).normalized;

                rb.velocity = dir * 5f; // adjust speed

                // Rotate bullet to face movement direction
                bullet.transform.right = dir;
            }
            Debug.Log("omarestgro");
        }

    }
}
