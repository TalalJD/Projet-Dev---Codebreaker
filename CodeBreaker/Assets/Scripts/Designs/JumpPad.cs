using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPad : MonoBehaviour
{
    [SerializeField] public float boostForce = 25f;

    public void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();
        
        if (player != null)
        {

            // reset le speed vertical 
            player.YSpeed = boostForce;

            // Maintenant dans l'air
            var airState = player.StateMachine.Get<AirState>();
            if (airState != null)
            {
                airState.isJump = true;
                player.StateMachine.Set<AirState>();
            }


        }
    }
}
    
