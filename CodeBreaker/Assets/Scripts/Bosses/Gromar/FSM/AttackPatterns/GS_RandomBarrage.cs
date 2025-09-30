using System.Collections;
using UnityEngine;

public class GS_RandomBarrage : GromarState
{
    public override void OnEnter()
    {
        // You can call ShootBulletBarrage here or from a coroutine if needed
        ShootBulletBarrage();
    }

    public override void OnExit() { }

    public void ShootBulletBarrage()
    {
        if (gromar == null)
        {
            Debug.Log("Gromar is not assigned");
            return;
        }

        int bulletNumber = 20;

        for (int i = 0; i < bulletNumber; i++)
        {
            // Random spawn position between MINSHOOT and MAXSHOOT
            Vector3 spawnPosition = gromar.GetRandomShootPosition();

            // Random direction to left with some vertical variance
            Vector2 dir = new Vector2(-1f, Random.Range(-1f, 1f)).normalized;

            // Use Gromar's centralized method
            gromar.ShootBigBullet(spawnPosition, dir, 5f); // speed = 5
        }
    }
}
