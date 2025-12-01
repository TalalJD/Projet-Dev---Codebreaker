using CodeBreaker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Player death state:
/// - StateNumber = 5
/// - 2s timer, then fires Player.OnDeath event
/// </summary>
public class DeathState : PlayerState
{
    public DeathState() : base(5) { }

    private float timer = 2f;

    public override void OnEnter()
    {
        timer = 2f;

        if(Player != null)
        {
            if (Player.Rb != null)
            {
                Player.Rb.linearVelocity = Vector2.zero;
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
        if (timer <= 0f)
        {
            Player.TriggerDeathEvent();
        }
    }

    public override void OnExit() {}

    public override void OnUpdate() { }
}