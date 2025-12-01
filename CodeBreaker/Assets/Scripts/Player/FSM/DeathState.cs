using CodeBreaker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DeathState : PlayerState
{
    public DeathState() : base(5) { }

    private float timer = 2f;

    public override void OnEnter()
    {
        timer = 2f;

        if (Player != null)
        {
           
            Player.GroundSpeed = 0f;

            if (Player.Rb != null)
            {
                Player.Rb.angularVelocity = 0f;
            }

            Player.canTakeDmg = false;
            Player.IsBlocking = true;

            Player.ClearHeldItem();

            if (Player.animator != null)
            {
                Player.animator.SetInteger("StateNumber", StateNumber);
            }
        }
    }

    public override void OnFixedUpdate()
    {
        timer -= Time.fixedDeltaTime;

   
        Player.YSpeed -= PhysicsInfo.Gravity * Time.fixedDeltaTime;

        if (Player.YSpeed < -PhysicsInfo.MaxFallSpeed)
        {
            Player.YSpeed = -PhysicsInfo.MaxFallSpeed;
        }

        if (Player.CheckOnGround() && Player.YSpeed < 0)
        {
            Player.YSpeed = 0f;
        }

        
        if (Player.Rb != null)
        {
            Player.Rb.linearVelocity = new Vector2(0f, Player.YSpeed);
        }

        if (timer <= 0f)
        {
          
            Player.TriggerDeathEvent();
        }
    }

    public override void OnExit() { }

    public override void OnUpdate() { }
}