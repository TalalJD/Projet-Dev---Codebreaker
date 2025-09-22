using Codice.CM.Common;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class GS_AttackState : GromarState
{
    private const float mediumSpread = 60f;
    private const float bigSpread = 90f;
    private const float smallSpread = 30f;

    public override void OnEnter()
    {
        Debug.Log("atackstateEnteredGromar");
        gromar.StartCoroutine(ShootXPatternBarrageAtPlayer(10, 0.2f, 5f));
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

    public IEnumerator ShootStraightLine(int bulletCount, float speed, float delay, Vector2 direction)
    {
   

        // Get the midpoint between minShoot and maxShoot
        Vector3 midPoint = (gromar.MINSHOOT.position + gromar.MAXSHOOT.position) / 2f;

        // Normalize direction to avoid scaling issues
        direction.Normalize();

        for (int i = 0; i < bulletCount; i++) 
        {
            SpawnSmallBullet(midPoint, direction, speed);
            yield return new WaitForSeconds(delay);
        }

    }
    {
        if (gromar == null || gromar.smallBullet == null)
        {
            Debug.Log("Gromar or smallBullet not assigned");
            yield break;
        }

        // Take the player's position once
        Vector3 playerPos = new Vector3(gromar.player.transform.position.x, gromar.player.transform.position.y + 2f, gromar.player.transform.position.z);

        // Calculate base direction from MINSHOOT to player (up-left line)
        Vector2 dirFromMin = (playerPos - gromar.MINSHOOT.position).normalized;

        // Calculate base direction from MAXSHOOT to player (down-left line)
        Vector2 dirFromMax = (playerPos - gromar.MAXSHOOT.position).normalized;

        for (int i = 0; i < bulletCount; i++)
        {
            // Fire one bullet from MINSHOOT aimed at player
            SpawnSmallBullet(gromar.MINSHOOT.position, dirFromMin, speed);

            // Fire one bullet from MAXSHOOT aimed at player
            SpawnSmallBullet(gromar.MAXSHOOT.position, dirFromMax, speed);

            yield return new WaitForSeconds(delay);
        }
        ShootBulletBarrage();
        Debug.Log("X pattern barrage aimed at player complete");
        
    }
    private void SpawnSmallBullet(Vector3 position, Vector2 dir, float speed)
    {
        GameObject bullet = GameObject.Instantiate(gromar.smallBullet, position, Quaternion.identity);
        Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();

        if (rb != null)
        {
            rb.velocity = dir * speed;
            bullet.transform.right = dir;
        }
    }

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

