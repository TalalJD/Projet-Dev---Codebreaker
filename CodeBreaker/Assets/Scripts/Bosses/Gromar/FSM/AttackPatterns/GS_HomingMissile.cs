using System.Collections;
using UnityEngine;

/// <summary>
/// Single-shot homing missile state (one projectile per state entry).
/// </summary>
public class GS_HomingMissile : GromarState
{
    private float nextStateDelay = 0.3f;
    private const string animName = "HomingMissilAttack";

    public GS_HomingMissile() : base(6) { }

    public override void SetParam(object args)
    {
        nextStateDelay = 0.3f;

        if (args is HomingMissileArgs a)
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
            //gromar.animator.Play(animName, 0, 0f);
        }
        else
        {
            Debug.LogWarning("[GS_HomingMissile] animator is null.");
        }

        // Wait for animation event to call Gromar.CallMissilAttack()
    }

    public IEnumerator ShootHomingMissiles()
    {
        Vector2 origin = gromar.HomingSP.position;
        Vector2 target = gromar.player.transform.position + Vector3.up * 0.5f;

        if (ProjectileManager.Instance != null)
        {
            var missile = ProjectileManager.Instance.Spawn(ProjectileType.HomingMissile, origin, target);
            if (missile != null)
            {
                // optional adjustments
            }
        }
        else
        {
            Debug.LogWarning("[GS_HomingMissile] ProjectileManager.Instance is null.");
        }

        if (nextStateDelay > 0f) yield return new WaitForSeconds(nextStateDelay);
        NotifyLogicFinished();
    }
}
