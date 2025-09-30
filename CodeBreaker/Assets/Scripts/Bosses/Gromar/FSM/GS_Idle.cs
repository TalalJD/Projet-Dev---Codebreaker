using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GS_Idle : GromarState
{
    public float timer = .5f;
    public float currentTimer;

    public override void OnEnter()
    {
        Debug.Log("IdleGroamr");
        currentTimer = timer;
    }
    public override void OnFixedUpdate()
    {
       if (currentTimer <= 0) { Machine.Set<GS_SimpleAimedAttack>(); return; }
       else { currentTimer -= Time.deltaTime; }

    }
}
