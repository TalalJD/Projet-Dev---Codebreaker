using System.Collections;
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
        if (gromar == null)
            return;

        Vector2 center = gromar.transform.position;

        for (int i = 0; i < bulletCount; i++)
        {
            // Calculate evenly spaced directions around a circle
            float angle = i * Mathf.PI * 2f / bulletCount;
            Vector2 dir = new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)).normalized;

            // Use Gromar's centralized method to spawn the bullet
            gromar.ShootSmallBullet(center, dir, speed);
        }
    }
}
