using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GS_SimpleAimedAttack : GromarState
{

    public override void OnEnter()
    {
        gromar.StartCoroutine(ShootAtPlayerContinuously(10f, 1f));
    }
    public override void OnExit() { }

    public IEnumerator ShootAtPlayerContinuously(float speed, float delay)
    {
        while (true) // infinite loop
        {
            // calculate direction from shootPoint to player
            Vector2 playerTarget = (Vector2)gromar.player.transform.position + new Vector2(0, 0.5f); // adjust 0.5f to player height
            Vector2 dir = (playerTarget - (Vector2)gromar.ShootingPoint.position).normalized;


            // spawn bullet at shootPoint
            GameObject bullet = GameObject.Instantiate(gromar.smallBullet, gromar.ShootingPoint.position, Quaternion.identity);

            // give bullet velocity
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            if (rb != null)
            {
                rb.velocity = dir * speed;
            }

            yield return new WaitForSeconds(delay);
        }
    }


}
