using System.Collections;
using UnityEngine;

public class GS_SpiralExplostion : GromarState
{
    public override void OnEnter()
    {
        gromar.StartCoroutine(ShootSpiralExplosion(50, 10, 20, 0.05f));
    }

    public override void OnExit() { }

    public IEnumerator ShootSpiralExplosion(int bulletCount, float speed, float angleStep, float delay)
    {
        if (gromar == null)
            yield break;

        Vector2 center = gromar.transform.position;
        float currentAngle = 0f;

        for (int i = 0; i < bulletCount; i++)
        {
            // Calculate direction based on current angle
            float rad = currentAngle * Mathf.Deg2Rad;
            Vector2 dir = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad)).normalized;

            // Spawn bullet using Gromar's centralized method
            gromar.ShootSmallBullet(center, dir, speed);

            // Increment angle for spiral effect
            currentAngle += angleStep;

            yield return new WaitForSeconds(delay); // optional delay between bullets
        }
    }
}
