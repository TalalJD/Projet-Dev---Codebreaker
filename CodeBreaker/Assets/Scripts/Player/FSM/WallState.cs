using CodeBreaker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallState : PlayerState
{
    

    public bool isTouchingWall()
    {
        float inputX = Input.GetAxisRaw("Horizontal"); // x

        float wallDistance = 0.35f;
        Vector2 velocity = Player.Rb.velocity;

        Vector2 origin = (Vector2)Player.transform.position;
        Vector2 dir = new Vector2(Mathf.Sign(inputX), 0f);
        RaycastHit2D hit = Physics2D.Raycast(origin, dir, wallDistance, Player.LayerMask);
        Debug.DrawRay(origin, dir * wallDistance, hit ? Color.red : Color.green);

        return hit;
    }



  

}
