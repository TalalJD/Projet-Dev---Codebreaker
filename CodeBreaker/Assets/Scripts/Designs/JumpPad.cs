using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPad : MonoBehaviour
{
    [SerializeField] private float boostForce = 25f;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();
        if (player != null)
        {

            // reset le speed vertical 
            player.YSpeed = boostForce;

            // Maintenant dans l'air
            var airState = player.StateMachine.Get<AirState>();
            airState.isJump = true;
            player.StateMachine.Set<AirState>();

            Debug.Log($"Force appliquer: {boostForce} ");
        }
    }
}
