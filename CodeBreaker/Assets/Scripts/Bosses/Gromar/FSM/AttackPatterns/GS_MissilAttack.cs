using System.Collections;
using UnityEngine;

public class GS_MissilAttack : GromarState
{
    private int nbMissile = 1;
    private float delay = 1f;

    public GS_MissilAttack() : base(3) { }

    public override void SetParam(object args)
    {
        nbMissile = 1;
        delay = 1f;

        if (args is ParabolicMissileArgs a)
        {
            nbMissile = Mathf.Max(1, a.Count);
            delay = Mathf.Max(0f, a.Delay);
        }
    }

    public override void OnEnter()
    {
        gromar.StartCoroutine(ShootAtPlayerContinuously());
    }

    private IEnumerator ShootAtPlayerContinuously()
    {
        for (int i = 0; i < nbMissile; i++)
        {
            Vector2 origin = gromar.ShootingPoint.position;
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
                Debug.LogWarning("[GS_MissilAttack] ProjectileManager.Instance is null.");
            }

            if (delay > 0f) yield return new WaitForSeconds(delay);
        }

        yield return new WaitForSeconds(0.3f);
        Machine.ExecuteNextState();
    }
}
