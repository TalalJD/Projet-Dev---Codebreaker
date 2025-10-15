using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GS_Cone : GromarState
{

    public override void OnEnter()
    {
        if (gromar.transform.position == gromar.SPAWNPOINT.position) { ShootCone(10, 60f, 15f); }// 10 bullets, 60° spread, speed 15
        else if(gromar.transform.position == gromar.MAPMIDPOINT.position) { ShootCone(10, 35f, 15f); }
        else { ShootConeAtPlayer(10, 60f, 15f); }
        
    }

    public override void OnExit()
    {
        
    }
    private void ShootConeAtPlayer(int bulletCount, float spreadAngle, float speed)
    {
        if (gromar == null || gromar.player == null) return;

        Vector2 origin = gromar.ShootingPoint.position;
        Vector2 playerPos = gromar.player.transform.position;
        Vector2 toPlayer = (playerPos - origin).normalized;

        float baseAngle = Mathf.Atan2(toPlayer.y, toPlayer.x) * Mathf.Rad2Deg;
        float startAngle = baseAngle - spreadAngle / 2f;

        for (int i = 0; i < bulletCount; i++)
        {
            float t = (bulletCount == 1) ? 0.5f : (float)i / (bulletCount - 1);
            float angle = startAngle + t * spreadAngle;
            Vector2 dir = new Vector2(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad));

            gromar.ShootSmallBullet(origin, dir, speed);
        }
    }
    private void ShootCone(int bulletCount, float spreadAngle, float speed)
    {
        Vector2 origin = gromar.ShootingPoint.position;

        // Use the shoot point's facing direction (right), negate if it points the wrong way
        Vector2 baseDir = gromar.ShootingPoint.right * -1f; // negate if inverted

        // Base angle for the cone
        float baseAngle = Mathf.Atan2(baseDir.y, baseDir.x) * Mathf.Rad2Deg;
        float halfSpread = spreadAngle / 2f;

        for (int i = 0; i < bulletCount; i++)
        {
            float t = (bulletCount == 1) ? 0.5f : (float)i / (bulletCount - 1);
            float angleOffset = Mathf.Lerp(-halfSpread, halfSpread, t);
            float bulletAngle = baseAngle + angleOffset;

            Vector2 dir = new Vector2(Mathf.Cos(bulletAngle * Mathf.Deg2Rad), Mathf.Sin(bulletAngle * Mathf.Deg2Rad));

            gromar.ShootSmallBullet(origin, dir, speed);
        }
    }



}
