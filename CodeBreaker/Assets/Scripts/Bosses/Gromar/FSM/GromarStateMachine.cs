using CodeBreaker;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GromarStateMachine : StateMachine<GromarState>
{

    private Dictionary<int, System.Type> shortcutMap;
    private List<AttackPattern> attackPatterns = new List<AttackPattern>();
    private Queue<StateCall> currentPatternQueue;
    private AttackPattern currentPattern;


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

       /* attackPatterns.Add(new AttackPattern(
            "Pattern Test", 3f,
            new StateCall(typeof(GS_Cone)),                // cone
            new StateCall(typeof(GS_Warp)),                // warp (default params)
            new StateCall(typeof(GS_Cone)),                // cone again
            new StateCall(typeof(GS_MissilAttack), 5, 0.5f) // missile (5 missiles, 0.5s delay)
        ));*/

        attackPatterns.Add(new AttackPattern(
            "Pattern A", 3f,
            new StateCall(typeof(GS_Warp),1, false, false, true, false),
            new StateCall(typeof(GS_Cone),3,.3f),
            new StateCall(typeof(GS_MissilAttack),6,.1f),
            new StateCall(typeof(GS_Cone),3,.3f)
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
        currentPattern = attackPatterns[index];
        currentPatternQueue = new Queue<StateCall>(currentPattern.sequence);


        Debug.Log($"Starting pattern: {currentPattern.name}");

        ExecuteNextState();
    }

    public void ExecuteNextState()
    {
        if (currentPatternQueue == null || currentPatternQueue.Count == 0)
        {
            Debug.Log("Pattern finished, returning to Idle.");

            float delay = currentPattern != null ? currentPattern.delay : 1f;

            SetIdleWithDelay(delay);
            return;
        }

        StateCall nextCall = currentPatternQueue.Dequeue();
        SetStateCall(nextCall);
    }


    private void SetStateCall(StateCall call)
    {
        // dynamically create state using its constructor arguments
        GromarState newState = Activator.CreateInstance(call.stateType, call.args) as GromarState;

        if (newState != null)
        {
            newState.Machine = this;
            newState.gromar = GetComponent<Gromar>();

            CurrentState?.OnExit();
            CurrentState = newState;
            CurrentState?.OnEnter();
        }
        else
        {
            Debug.LogWarning($"Failed to create state of type {call.stateType}");
        }
    }


    private void SetIdleWithDelay(float delay)
    {
        var idleState = Get<GS_Idle>();
        if (idleState != null)
        {
            idleState.Idletimer = delay;
            Set<GS_Idle>();
        }
        else
        {
            Debug.LogWarning("Idle state not found!");
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
