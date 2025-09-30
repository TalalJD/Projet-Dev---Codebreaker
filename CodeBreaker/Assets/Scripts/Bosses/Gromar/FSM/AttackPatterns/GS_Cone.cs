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
        Vector3 midPoint = (gromar.MINSHOOT.position + gromar.MAXSHOOT.position) / 2f;
        Vector2 baseDir = Vector2.left;
        float startAngle = -spreadAngle / 2f;

        for (int i = 0; i < bulletCount; i++)
        {
            float t = (bulletCount == 1) ? 0.5f : (float)i / (bulletCount - 1);
            float angle = startAngle + t * spreadAngle;
            Vector2 dir = Quaternion.Euler(0, 0, angle) * baseDir;

            gromar.ShootSmallBullet(midPoint, dir, speed);
        }
    }
}
