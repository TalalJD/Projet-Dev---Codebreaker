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
        Add(new GS_Lobe());
        Add(new GS_LaserAttack());
        Add(new GS_HomingMissile());

        // --- Definition des schemas d'attaque ---
        attackPatterns.Add(
            AttackPatternBuilder.New("3 cones x 6 lobes", 3f)
                .Warp(new WarpArgs { CornerOnly = true })
                .Repeat(3, b => b.Cone(new ConeArgs { Speed = 20f }))
                .Warp(new WarpArgs { CornerOnly = true })
                .Repeat(6, b => b.ParabolicMissile(new ParabolicMissileArgs { nextStateDelay = 0.1f }))
                .Build()
        );

        attackPatterns.Add(
            AttackPatternBuilder.New("10 cones x warp", 3f)
                .Repeat(10, b => b
                    .Warp(new WarpArgs { CornerOnly = true })
                    .Cone(new ConeArgs { Speed = 20f })
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
             AttackPatternBuilder.New("5 homings", 3f)
                .Warp(new WarpArgs { CornerOnly = true })
                // repeat the homing-shot 5 times; each homing state will wait 5s before next
                .Repeat(5, b => b.HomingMissile(new HomingMissileArgs { nextStateDelay = 2f }))
                .Build()
        );

        attackPatterns.Add(
             AttackPatternBuilder.New("Explosion middle", 3f)
                .Warp(new WarpArgs { Middle = true })
                .Add(typeof(GS_Explosion))
                .Build()
        );


        Initialize<GS_Idle>();

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
    }

    private void FixedUpdate()
    {
        CurrentState?.OnFixedUpdate();
    }

    public void ForceExplosion()
    {
        // Si on est déjà en explosion, ne rien faire
        if (IsCurrentState<GS_Explosion>())
            return;

        // Arrêter complètement le pattern en cours
        DebugStop(); // remet currentPattern = null, queue = null, inPatternMode = false

        // Récupérer l'état d'explosion
        var explosionState = Get<GS_Explosion>();
        if (explosionState == null)
        {
            Debug.LogWarning("GS_Explosion state not found in GromarStateMachine.");
            return;
        }

        // Forcer le changement d'état vers l'explosion
        ForceSet(explosionState);
    }


}
