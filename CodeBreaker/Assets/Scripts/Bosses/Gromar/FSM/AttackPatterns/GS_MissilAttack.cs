using System.Collections;
using UnityEngine;

public class GS_MissilAttack : GromarState
{
    public override void OnEnter()
    {
        // Commence le pattern de tir
        gromar.StartCoroutine(ShootAtPlayerContinuously(1f, 10));
    }

    public override void OnExit() { }

    private IEnumerator ShootAtPlayerContinuously(float delay, int number)
    {
        for (int i = 0; i < number; i++)
        {
            // Position du point de tir du boss
            Vector2 origin = gromar.ShootingPoint.position;

            // Position de la cible (joueur + petit décalage vers le haut)
            Vector2 target = gromar.player.transform.position + Vector3.up * 0.5f;

            // Vérifie que le manager est prêt
            if (ProjectileManager.Instance != null)
            {
                // Demande au manager de créer un missile parabolique
                var missile = ProjectileManager.Instance.Spawn(
                    ProjectileType.ParabolicMissile,
                    origin,
                    target
                );

                // Si le projectile a bien été créé, ajuste ses paramètres
                if (missile != null)
                {
                    missile.speed = Random.Range(8f, 12f);  // légère variation de vitesse
                    missile.lifetime = 8f;                  // durée de vie avant destruction
                }
            }
            else
            {
                Debug.LogWarning("ProjectileManager.Instance est null — le missile n’a pas pu être créé !");
            }

            // Délai entre les tirs
            yield return new WaitForSeconds(delay);
        }
    }
}
