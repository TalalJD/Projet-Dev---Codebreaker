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
        Add(new GS_Idle());
        Add(new GS_Warp());
        Add(new GS_Explosion());
        Add(new GS_Cone());
        Add(new GS_MissilAttack());
        Add(new GS_LaserAttack());


        //attackPatterns.Add(new AttackPattern(
        //    "Pattern A", 3f,
        //    new StateCall(typeof(GS_Warp), 1, false, false, true, false),
        //    new StateCall(typeof(GS_Cone), 3, .3f),
        //    new StateCall(typeof(GS_Warp), 1, false, false, true, false),
        //    new StateCall(typeof(GS_MissilAttack), 6, .1f)
        //    ));

        //attackPatterns.Add(
        //    AttackPattern.BuildAlternatingPattern(
        //        "Pattern B", 3f, 10,
        //        new StateCall(typeof(GS_Warp), 1, false, false, true, false),
        //        new StateCall(typeof(GS_Cone), 1, .3f)
        //    )
        //);
        attackPatterns.Add(new AttackPattern(
            "Laser Pattern", 3f,
            new StateCall(typeof(GS_Warp), 1, false, false, true, false),
            new StateCall(typeof(GS_LaserAttack)),
            new StateCall(typeof(GS_Warp), 1, false, false, true, false)
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


    private void SetStateCall(StateCall statecall)
    {

        var state = AvailableStates.Find(s => s.GetType() == statecall.stateType);

        if (state != null)
        {

            state.SetParam(statecall.args);
            ForceSet(state);
        }

    }

    /// <summary>
    /// Set le idle avec un delai mais choisi une position a warp avant de idle entre le millieu et le spawnpoint du boss
    /// </summary>
    private void SetIdleWithDelay(float delay)
    {

        var warpState = Get<GS_Warp>();

        if (warpState != null)
        {

            bool warpToSpawn = UnityEngine.Random.value < 0.5f;//entre 0 et 1 aka false or true


            object[] args = warpToSpawn
                ? new object[] { 1, false, false, false, true, true }   //si true on set les args a tp au spawn
                : new object[] { 1, false, true, false, false, true };  //si false on set les args a tp au middle

            warpState.SetParam(args); //on set les args du warp


            ForceSet(warpState);


            GoIdle(delay);
        }
        else
        {
            Debug.LogWarning("Warp state not found — going directly to Idle.");
            GoIdle(delay);
        }
    }

    /// <summary>
    /// get le idle state et rentre dedans en lui pasasnt le temps a etre en delay
    /// </summary>
    private void GoIdle(float delay)
    {
        var idleState = Get<GS_Idle>();
        if (idleState != null)
        {
            idleState.Idletimer = delay;
            Set<GS_Idle>();
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
