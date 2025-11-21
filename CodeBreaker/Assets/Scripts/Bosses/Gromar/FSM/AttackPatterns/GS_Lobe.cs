using System.Collections;
using UnityEngine;

public class GS_Lobe : GromarState
{
    private float nextStateDelay = 0.3f;
    private const string animName = "LobeAttack";

    public GS_Lobe() : base(3) { }

    public override void SetParam(object args)
    {
        nextStateDelay = 0.3f;

        if (args is ParabolicMissileArgs a)
        {
            nextStateDelay = Mathf.Max(0f, a.nextStateDelay);
        }
    }

    public override void OnEnter()
    {
        base.OnEnter();

        if (gromar?.animator != null)
        {
            gromar.animator.ResetTrigger(animName);
            gromar.animator.SetTrigger(animName);
           // gromar.animator.Play(animName, 0, 0f);
        }
        else
        {
            Debug.LogWarning("[GS_Lobe] animator is null.");
        }

        // Wait for animation event to call Gromar.CallLobe()
    }

    public IEnumerator ShootAtPlayerContinuously()
    {
        Vector2 origin = gromar.LobeSP.position;
        Vector2 target = gromar.player.transform.position + Vector3.up * 0.5f;

        if (ProjectileManager.Instance != null)
        {
            var missile = ProjectileManager.Instance.Spawn(ProjectileType.ParabolicMissile, origin, target);
            if (missile != null)
            {
                missile.speed = Random.Range(8f, 12f);
                missile.lifetime = 8f;
            }
        }
        else
        {
            Debug.LogWarning("[GS_Lobe] ProjectileManager.Instance is null.");
        }

        if (nextStateDelay > 0f) yield return new WaitForSeconds(nextStateDelay);
        NotifyLogicFinished();
    }
}
