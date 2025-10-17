using CodeBreaker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GromarStateMachine : StateMachine<GromarState>
{

    private Dictionary<int, System.Type> shortcutMap;

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
       // Add(new GS_AttackState());
        Add(new GS_Idle());//1
        Add(new GS_Warp());//2
        Add(new GS_Explosion());//4
        Add(new GS_Cone());//5
        Add(new GS_MissilAttack());//7
        

        Initialize<GS_Idle>();

        shortcutMap = new Dictionary<int, System.Type>();
        for (int i = 0; i < AvailableStates.Count; i++)
        {
            shortcutMap[i + 1] = AvailableStates[i].GetType();
        }
    }

    private void HandleShortcutKeys()
    {
       
            for (int i = 1; i <= shortcutMap.Count; i++)
            {
                if (Input.GetKeyDown(KeyCode.Alpha0 + i)) // Alpha1..Alpha9
                {
                    Set(shortcutMap[i]);
                    Debug.Log($"Switched to state: {shortcutMap[i].Name}");
                }
            }
        
    }

    private void Set(System.Type stateType)
    {
        var state = AvailableStates.Find(s => s.GetType() == stateType);
        if (state != null)
        {
            CurrentState?.OnExit();
            CurrentState = state;
            CurrentState?.OnEnter();
        }
    }

    public void Update()
    {
        CurrentState?.OnUpdate();
        HandleShortcutKeys();
    }

    public void FixedUpdate()
    {
        CurrentState?.OnFixedUpdate();
    }
}
