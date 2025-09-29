using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GS_Explostion : GromarState
{
    public override void OnEnter()
    {
        ShootExplosion(20, 10);
    }

    public override void OnExit() { }
    public void ShootExplosion(int bulletCount, float speed)
    {
        if (gromar == null || gromar.smallBullet == null)
            return;

        Vector2 center = gromar.transform.position;

        for (int i = 0; i < bulletCount; i++)
        {
            // Calculate evenly spaced directions around a circle
            float angle = i * Mathf.PI * 2f / bulletCount;
            Vector2 dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)).normalized;

            // Spawn bullet at Gromar's position
            GameObject bullet = GameObject.Instantiate(gromar.smallBullet, center, Quaternion.identity);

            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = dir * speed;
                bullet.transform.right = dir; // rotate sprite to face movement
            }
        }
    }

}
