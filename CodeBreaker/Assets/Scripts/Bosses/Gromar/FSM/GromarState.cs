using CodeBreaker;
using UnityEngine;

public class GromarState : State
{
    public int StateNumber = -1;
    public Gromar gromar;
    public GromarStateMachine Machine;

    // Flags pour la synchro anim / logique
    protected bool logicFinished;
    protected bool animFinished;

    // Empêche d'appeler Machine.ExecuteNextState() plusieurs fois pour un même état
    protected bool finishedNotified;

    // Indique si la logique (coroutine) du state a été démarrée (utile pour éviter les démarrages doubles)
    public bool LogicStarted { get; set; }

    public GromarState(int number = -1) { StateNumber = number; }

    public override void OnEnter()
    {
        // IMPORTANT : reset à chaque entrée d’état
        logicFinished = false;
        animFinished = false;
        finishedNotified = false;
        LogicStarted = false;
    }

    public override void OnExit()
    {
    }

    /// <summary>
    /// À appeler quand la logique de l’attaque est terminée
    /// (coroutine finie, projectiles spawnés, etc.).
    /// </summary>
    public virtual void NotifyLogicFinished()
    {
        logicFinished = true;
        LogicStarted = false;
        TryFinish();
    }

    /// <summary>
    /// Appelée par l’event d’animation de fin d’attaque.
    /// </summary>
    public virtual void NotifyAnimationFinished()
    {
        animFinished = true;
        TryFinish();
    }

    /// <summary>
    /// Avance le pattern seulement si anim + logique sont finies.
    /// </summary>
    protected virtual void TryFinish()
    {
        // Evite d'exécuter plusieurs fois (ex: animation/events qui se déclenchent en double)
        if (finishedNotified) return;

        if (!logicFinished || !animFinished)
            return;

        finishedNotified = true;

        if (Machine != null)
        {
            Machine.ExecuteNextState();
        }
    }
}
