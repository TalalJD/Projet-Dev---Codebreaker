using CodeBreaker;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GromarStateMachine : StateMachine<GromarState>
{

    private Dictionary<int, System.Type> shortcutMap;
    private List<AttackPattern> attackPatterns = new List<AttackPattern>();
    private Queue<Type> currentPatternQueue;


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
        
        attackPatterns.Add(new AttackPattern
            ("Pattern A",
            typeof(GS_Cone),
            typeof(GS_Warp),
            typeof(GS_Cone),
            typeof(GS_MissilAttack)
            ));

        Initialize<GS_Idle>();

        shortcutMap = new Dictionary<int, Type>();
        for (int i = 0; i < AvailableStates.Count; i++)
        {
            shortcutMap[i + 1] = AvailableStates[i].GetType();
        }
    }

    public void StartRandomPattern()
    {
        if (attackPatterns.Count == 0)
            return;

        int index = UnityEngine.Random.Range(0, attackPatterns.Count);
        AttackPattern chosen = attackPatterns[index];
        currentPatternQueue = new Queue<Type>(chosen.stateSequence);

        Debug.Log($"Starting pattern: {chosen.name}");

        ExecuteNextState();
    }

    public void ExecuteNextState()
    {
        if (currentPatternQueue == null || currentPatternQueue.Count == 0)
        {
            Debug.Log("Pattern finished, returning to Idle.");
            Set<GS_Idle>();
            return;
        }

        Type nextState = currentPatternQueue.Dequeue();
        Set(nextState);
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

    private void Set(Type stateType)
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
