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
    
       // ShootBulletBarrage();
        gromar.StartCoroutine(ShootRegularXPattern(45, 0.1f, 6f, gromar.MAPMIDPOINT.position));
        gromar.StartCoroutine(ShootStraightLine(45, 6f, .1f, Vector2.left));
        //ShootCone(10, bigSpread, 5f);
        //ShootExplosion(10, 6f);
        //gromar.StartCoroutine(ShootSpiralExplosion(40, 5f, 10f, 0.05f));


    }

    public IEnumerator ShootStraightLine(int bulletCount, float speed, float delay, Vector2 direction)
    {


        // Get the midpoint between minShoot and maxShoot
        Vector3 midPoint = new Vector3 (gromar.transform.position.x, (gromar.transform.position.y -1f), gromar.transform.position.z);

        // Normalize direction to avoid scaling issues
        direction.Normalize();

        for (int i = 0; i < bulletCount; i++) 
        {
            SpawnSmallBullet(midPoint, direction, speed);
            yield return new WaitForSeconds(delay);
        }

    }


    public IEnumerator ShootRegularXPattern(int bulletCount, float delay, float speed, Vector2 targetPoint)
    {
        if (gromar == null || gromar.smallBullet == null)
        {
            Debug.LogWarning("Gromar or smallBullet not assigned");
            yield break;
        }

        // Define fixed X directions
        Vector2 upLeft = new Vector2(-1f, 1f).normalized;   
        Vector2 downLeft = new Vector2(-1f, -1f).normalized; 

        Vector3 minPos = gromar.MINSHOOT.position;
        Vector3 maxPos = gromar.MAXSHOOT.position;

        Vector2 dirFromMin = (targetPoint - (Vector2)minPos).normalized;
        Vector2 dirFromMax = (targetPoint - (Vector2)maxPos).normalized;

        for (int i = 0; i < bulletCount; i++)
        {
            SpawnSmallBullet(minPos, dirFromMin, speed);
            SpawnSmallBullet(maxPos, dirFromMax, speed);

            yield return new WaitForSeconds(delay); // stagger shots for a stream effect
        }

        Debug.Log("Regular X pattern finished");
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

    
}

