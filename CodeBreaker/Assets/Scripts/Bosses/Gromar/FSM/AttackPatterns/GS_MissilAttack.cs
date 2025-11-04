using System.Collections;
using UnityEngine;

/// <summary>
/// Etat du boss Gromar qui tire plusieurs missiles paraboliques vers le joueur.
/// </summary>
public class GS_MissilAttack : GromarState
{
    private int nbMissile = 1; // nombre de missiles a tirer
    private float delay = 1f;  // delai entre chaque tir
    private float nextStateDelay = 0.3f;

    public GS_MissilAttack() : base(3) { }

    /// <summary>
    /// Recoit les parametres (ParabolicMissileArgs) pour configurer le nombre et le delai.
    /// </summary>
    public override void SetParam(object args)
    {
        nbMissile = 1;
        delay = 1f;
        nextStateDelay = 0.3f;

        if (args is ParabolicMissileArgs a)
        {
            nbMissile = Mathf.Max(1, a.Count);
            delay = Mathf.Max(0f, a.Delay);
            nextStateDelay = Mathf.Max(0f, a.nextStateDelay);
        }
    }

    /// <summary>
    /// Lance la sequence de tir des missiles.
    /// </summary>
    public override void OnEnter()
    {
        gromar.StartCoroutine(ShootAtPlayerContinuously());
    }

    /// <summary>
    /// Tire plusieurs missiles vers le joueur avec un delai entre chaque tir.
    /// </summary>
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

        // courte pause avant de passer a l'etat suivant
        yield return new WaitForSeconds(nextStateDelay);
        Machine.ExecuteNextState();
    }
}
