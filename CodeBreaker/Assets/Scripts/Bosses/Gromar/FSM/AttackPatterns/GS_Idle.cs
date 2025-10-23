using System.Collections;
using UnityEngine;

public class GS_Idle : GromarState
{
    private float idleTimer = 1f;

    public GS_Idle() : base(0) { }

    public override void SetParam(object args)
    {
        idleTimer = 1f;
        if (args is IdleArgs a) idleTimer = Mathf.Max(0f, a.Duration);
    }

    public override void OnEnter()
    {
        gromar.StartCoroutine(WaitAndAttack());
    }

    private IEnumerator WaitAndAttack()
    {
        if (idleTimer > 0f) yield return new WaitForSeconds(idleTimer);
        Machine.StartRandomPattern();
    }
}
