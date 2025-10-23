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
        Add(new GS_Idle());
        Add(new GS_Warp());
        Add(new GS_Explosion());
        Add(new GS_Cone());
        Add(new GS_MissilAttack());
        Add(new GS_LaserAttack());

        // === Modern builder-based system ===
        //attackPatterns.Add(
        //    AttackPatternBuilder.New("Pattern A", 3f)
        //        .Warp(new WarpArgs { CornerOnly = true })
        //        .Cone(new ConeArgs { Count = 3, Delay = 0.3f })
        //        .Warp(new WarpArgs { CornerOnly = true })
        //        .ParabolicMissile(new ParabolicMissileArgs { Count = 6, Delay = 0.1f })
        //        .Build()
        //);

        //attackPatterns.Add(
        //    AttackPatternBuilder.New("Pattern B", 3f)
        //        .Repeat(10, b => b
        //            .Warp(new WarpArgs { CornerOnly = true })
        //            .Cone(new ConeArgs { Count = 1, Delay = 0.3f })
        //        )
        //        .Build()
        //);

        attackPatterns.Add(
            AttackPatternBuilder.New("Laser Pattern", 3f)
                .Warp()
                .Laser()
                .Warp()
                .Build()
        );
        // ===================================

        Initialize<GS_Idle>();

        shortcutMap = new Dictionary<int, Type>();
        for (int i = 0; i < AvailableStates.Count; i++)
            shortcutMap[i + 1] = AvailableStates[i].GetType();
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
            // In the new system, statecall.arg is a single object (WarpArgs, ConeArgs, etc.)
            state.SetParam(statecall.arg);
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


            WarpArgs warpArgs = warpToSpawn
                ? new WarpArgs { Times = 1, Spawn = true, Middle = false, SkipNextState = true }
                : new WarpArgs { Times = 1, Spawn = false, Middle = true, SkipNextState = true };

            warpState.SetParam(warpArgs);



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
            idleState.SetParam(new IdleArgs { Duration = delay });
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
