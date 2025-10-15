using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GS_MissilAttack : GromarState
{
    public override void OnEnter()
    {
        gromar.StartCoroutine(ShootAtPlayerContinuously(10f, 1f));
    }
    public override void OnExit() { }

    public IEnumerator ShootAtPlayerContinuously(float speed, float delay)
    {
        for (int i = 0; i < 10; i++)
        {
            Vector2 playerTarget = (Vector2)gromar.player.transform.position + new Vector2(0, 0.5f);
            Vector2 dir = (playerTarget - (Vector2)gromar.ShootingPoint.position).normalized;

            // centralized bullet spawning
            gromar.ShootMissileBullet(gromar.ShootingPoint.position, gromar.player.transform.position);

            yield return new WaitForSeconds(delay);
        }
    }
}
