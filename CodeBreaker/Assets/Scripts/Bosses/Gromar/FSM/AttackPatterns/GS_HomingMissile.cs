using System.Collections;
using UnityEngine;

/// <summary>
/// Etat du boss Gromar qui tire des missiles a tete chercheuse vers le joueur.
/// </summary>
public class GS_HomingMissile : GromarState
{
    private int nbMissile = 1;   // nombre de missiles a tirer
    private float delay = 1f;    // delai entre chaque tir
    private float nextStateDelay = 0.3f;

    public GS_HomingMissile() : base(6) { }

    /// <summary>
    /// Recoit les parametres (HomingMissileArgs) pour configurer le nombre et le delai.
    /// </summary>
    public override void SetParam(object args)
    {
        nbMissile = 1;
        delay = 1f;
        nextStateDelay = 0.3f;

        if (args is HomingMissileArgs a)
        {
            nbMissile = Mathf.Max(1, a.Count);
            delay = Mathf.Max(0f, a.Delay);
            nextStateDelay = Mathf.Max(0f, a.nextStateDelay);
        }
    }

    public override void OnEnter()
    {
        gromar.StartCoroutine(ShootHomingMissiles());
    }

    /// <summary>
    /// Tire plusieurs missiles a tete chercheuse vers le joueur avec un delai entre chaque tir.
    /// </summary>
    private IEnumerator ShootHomingMissiles()
    {
        for (int i = 0; i < nbMissile; i++)
        {
            Vector2 origin = gromar.ShootingPoint.position;
            Vector2 target = gromar.player.transform.position + Vector3.up * 0.5f;

            if (ProjectileManager.Instance != null)
            {
                var missile = ProjectileManager.Instance.Spawn(ProjectileType.HomingMissile, origin, target);
                if (missile != null)
                {
                    // Optionnel: ajuster vitesse / lifetime ici si tu veux
                    // missile.speed = 10f;
                    // missile.lifetime = 8f;
                }
            }
            else
            {
                Debug.LogWarning("[GS_HomingMissile] ProjectileManager.Instance is null.");
            }

            if (delay > 0f)
                yield return new WaitForSeconds(delay);
        }

        // courte pause avant de passer a l'etat suivant
        yield return new WaitForSeconds(nextStateDelay);
        Machine.ExecuteNextState();
    }
}
