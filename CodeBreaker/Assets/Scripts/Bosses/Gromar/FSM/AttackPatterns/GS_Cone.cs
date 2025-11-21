using System.Collections;
using UnityEngine;

public class GS_Cone : GromarState
{
    private float nextStateDelay = 0.3f;
    private float speed = 10f;
    private const string animName = "ConeAttack";

    public GS_Cone() : base(2) { }

    public override void SetParam(object args)
    {
        // reset to defaults each time
        nextStateDelay = 0.3f;
        speed = 10f;

        if (args is ConeArgs a)
        {
            nextStateDelay = Mathf.Max(0f, a.nextStateDelay);
            speed = Mathf.Max(0f, a.Speed);
        }
    }

    public override void OnEnter()
    {
        base.OnEnter();

        // Force animation restart so animation events fire even for repeated identical states
        if (gromar?.animator != null)
        {
            gromar.animator.ResetTrigger(animName);
            gromar.animator.SetTrigger(animName);
            // Try to force the clip to restart immediately
            //gromar.animator.Play(animName, 0, 0f);
        }
        else
        {
            Debug.LogWarning("[GS_Cone] animator is null.");
        }

        // Do NOT start the logic coroutine here — wait for the animation event (CallCone) to start it.
    }

    public override void OnExit()
    {
        base.OnExit();
    }

    // Fire exactly one cone when started by the animation event (via Gromar.CallCone).
    public IEnumerator ShootCone()
    {
        Vector2 origin = gromar.ConeSP.position;
        Vector2 target = gromar.player.transform.position + Vector3.up * 1.5f;

        var cone = ProjectileManager.Instance.Spawn(ProjectileType.Cone, origin, target);
        if (cone != null) cone.speed = speed;

        if (nextStateDelay > 0f) yield return new WaitForSeconds(nextStateDelay);

        NotifyLogicFinished();
    }
}
