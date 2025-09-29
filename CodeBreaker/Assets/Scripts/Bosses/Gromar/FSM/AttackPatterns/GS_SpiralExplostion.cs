using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GS_SpiralExplostion : GromarState
{
    public override void OnEnter()
    {
        gromar.StartCoroutine(ShootSpiralExplosion(50,10,20,0.05f));
    }
    public override void OnExit()
    {
        
    }

    public IEnumerator ShootSpiralExplosion(int bulletCount, float speed, float angleStep, float delay)
    {
        if (gromar == null || gromar.smallBullet == null)
            yield break;

        Vector2 center = gromar.transform.position;
        float currentAngle = 0f;

        for (int i = 0; i < bulletCount; i++)
        {
            // Calculate direction based on current angle
            float rad = currentAngle * Mathf.Deg2Rad;
            Vector2 dir = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)).normalized;

            // Spawn bullet
            GameObject bullet = GameObject.Instantiate(gromar.smallBullet, center, Quaternion.identity);
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = dir * speed;
                bullet.transform.right = dir; // rotate sprite
            }

            // Increment angle for spiral effect
            currentAngle += angleStep;

            yield return new WaitForSeconds(delay); // optional delay between bullets
        }
    }

}
