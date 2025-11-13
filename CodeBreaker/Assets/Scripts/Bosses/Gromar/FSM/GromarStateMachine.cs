using CodeBreaker;
using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Machine d'états du boss Gromar,
/// basée sur AttackPatternStateMachine.
/// </summary>
public class GromarStateMachine : AttackPatternStateMachine<GromarState>
{
    private Dictionary<int, Type> shortcutMap; // raccourcis clavier -> états

    public override void Add(GromarState state)
    {
        base.Add(state);
        state.Machine = this;
        state.gromar = GetComponent<Gromar>();
    }

    public override void Set<G>(bool TriggerEnter = true)
    {
        base.Set<G>(TriggerEnter);
    }

    /// <summary>
    /// Initialise les états et les patterns d'attaque du boss.
    /// </summary>
    public void Init()
    {
        Add(new GS_Idle());
        Add(new GS_Warp());
        Add(new GS_Explosion());
        Add(new GS_Cone());
        Add(new GS_MissilAttack());
        Add(new GS_LaserAttack());
        Add(new GS_HomingMissile());

        // --- Definition des schemas d'attaque ---
        attackPatterns.Add(
            AttackPatternBuilder.New("Pattern A", 3f)
                .Warp(new WarpArgs { CornerOnly = true })
                .Cone(new ConeArgs { Count = 3, Delay = 0.3f })
                .Warp(new WarpArgs { CornerOnly = true })
                .ParabolicMissile(new ParabolicMissileArgs { Count = 6, Delay = 0.1f })
                .Build()
        );

        attackPatterns.Add(
            AttackPatternBuilder.New("Pattern B", 3f)
                .Repeat(10, b => b
                    .Warp(new WarpArgs { CornerOnly = true })
                    .Cone(new ConeArgs { Count = 1, Delay = 0f, Speed = 20f })
                )
                .ForAllNextStateDelay(.1f)
                .Build()
        );

        attackPatterns.Add(
            AttackPatternBuilder.New("Laser Pattern", 3f)
                .Warp()
                .Laser()
                .Warp()
                .Build()
        );

        attackPatterns.Add(
             AttackPatternBuilder.New("Homing Pattern", 3f)
                .Warp(new WarpArgs { CornerOnly = true })
                .HomingMissile(new HomingMissileArgs { Count = 5, Delay = 5f })
                .Build()
        );


        Initialize<GS_Idle>();

        // map clavier : 1 -> Idle, 2 -> Warp, etc.
        shortcutMap = new();
        for (int i = 0; i < AvailableStates.Count; i++)
            shortcutMap[i + 1] = AvailableStates[i].GetType();
    }

    /// <summary>
    /// Appelé automatiquement quand un pattern est terminé (queue vide).
    /// On applique la logique spécifique de Gromar :
    /// warp aléatoire + Idle avec délai.
    /// </summary>
    protected override void OnPatternFinished(float delay)
    {
        SetIdleWithDelay(delay);
    }

    private void SetIdleWithDelay(float delay)
    {
        var warpState = Get<GS_Warp>();
        if (warpState == null)
        {
            Debug.LogWarning("Warp state not found - going directly to Idle.");
            GoIdle(delay);
            return;
        }

        bool warpToSpawn = UnityEngine.Random.value < 0.5f;
        var warpArgs = warpToSpawn
            ? new WarpArgs { Spawn = true, Middle = false, SkipNextState = true }
            : new WarpArgs { Spawn = false, Middle = true, SkipNextState = true };

        warpState.SetParam(warpArgs);
        ForceSet(warpState);
        GoIdle(delay);
    }

    private void GoIdle(float delay)
    {
        var idleState = Get<GS_Idle>();
        if (idleState == null) return;

        idleState.SetParam(new IdleArgs { Duration = delay });
        Set<GS_Idle>();
    }

    /// <summary>
    /// Permet de changer d'état avec les touches 1,2,3... (debug / test)
    /// </summary>
    private void HandleShortcutKeys()
    {
        for (int i = 1; i <= shortcutMap.Count; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha0 + i))
            {
                Set(shortcutMap[i]);
                Debug.Log($"Switched to state: {shortcutMap[i].Name}");
            }
        }
    }

    private void Set(Type stateType)
    {
        var state = AvailableStates.Find(s => s.GetType() == stateType);
        if (state == null) return;

        CurrentState?.OnExit();
        CurrentState = state;
        CurrentState?.OnEnter();
    }

    private void Update()
    {
        CurrentState?.OnUpdate();
        HandleShortcutKeys();
    }

    private void FixedUpdate()
    {
        CurrentState?.OnFixedUpdate();
    }
}
