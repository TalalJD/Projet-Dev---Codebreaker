using System.Collections;
using UnityEngine;

/// <summary>
/// Etat d'attente de Gromar entre deux patterns.
/// Le boss reste immobile pendant un certain temps avant de relancer une attaque.
/// </summary>
public class GS_Idle : GromarState
{
    private float idleTimer = 1f; // duree d'attente avant le prochain pattern

    public GS_Idle() : base(0) { }

    /// <summary>
    /// Recoit les parametres d'attente (IdleArgs).
    /// </summary>
    public override void SetParam(object args)
    {
        idleTimer = 1f;
        if (args is IdleArgs a) idleTimer = Mathf.Max(0f, a.Duration);
    }

    /// <summary>
    /// Lance la coroutine d'attente.
    /// </summary>
    public override void OnEnter()
    {
       
        gromar.StartCoroutine(WaitAndAttack());
    }

    public override void OnExit()
    {
        base.OnExit();
    }

    /// <summary>
    /// Attend pendant idleTimer secondes puis lance un nouveau pattern.
    /// </summary>
    private IEnumerator WaitAndAttack()
    {
        if (idleTimer > 0f) yield return new WaitForSeconds(idleTimer);
        Machine.StartRandomPattern();
    }
}
