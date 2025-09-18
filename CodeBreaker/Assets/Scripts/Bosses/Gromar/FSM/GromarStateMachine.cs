using CodeBreaker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GromarStateMachine : StateMachine<GromarState>
{



    public override void Add(GromarState state)
    {
        base.Add(state);
        state.Machine = this;
        state.gromar = GetComponent<Gromar>();
    }
    /// <summary>
    /// permet d'excuter / entrer dans un etat
    /// </summary>
    /// <typeparam name="G">etat quelconque choisi</typeparam>
    public override void Set<G>(bool TriggerEnter = true)
    {
        base.Set<G>(TriggerEnter);
    }

    public void Init()
    {
        Add(new GS_AttackState());
        Add(new GS_Idle());
        Initialize<GS_Idle>();

    }

    public void Update()
    {
        CurrentState?.OnUpdate();
    }

    public void FixedUpdate()
    {
        CurrentState?.OnFixedUpdate();
    }
}
