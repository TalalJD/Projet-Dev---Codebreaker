using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GS_Cone : GromarState
{

    public override void OnEnter()
    {
        ShootCone(25, 30, 10);
    }

    public override void OnExit()
    {
        
    }
    public void ShootCone(int bulletCount, float spreadAngle, float speed)
    {
        if (gromar == null || gromar.smallBullet == null || gromar.MINSHOOT == null || gromar.MAXSHOOT == null)
            return;

        // Midpoint between min and max shoot
        Vector3 midPoint = (gromar.MINSHOOT.position + gromar.MAXSHOOT.position) / 2f;

        // Base direction (straight left for example)
        Vector2 baseDir = Vector2.left;

        // Start angle so the cone is centered
        float startAngle = -spreadAngle / 2f;

        for (int i = 0; i < bulletCount; i++)
        {
            // Lerp angle across the spread
            float t = (bulletCount == 1) ? 0.5f : (float)i / (bulletCount - 1);
            float angle = startAngle + t * spreadAngle;

            // Rotate the base direction
            Vector2 dir = Quaternion.Euler(0, 0, angle) * baseDir;

            GameObject bullet = GameObject.Instantiate(gromar.smallBullet, midPoint, Quaternion.identity);

            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = dir.normalized * speed;
                bullet.transform.right = dir; // rotate sprite
            }
        }
    }
}
