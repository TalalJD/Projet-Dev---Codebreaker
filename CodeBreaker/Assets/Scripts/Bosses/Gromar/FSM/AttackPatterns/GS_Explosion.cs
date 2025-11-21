using System.Collections;
using UnityEngine;

/// <summary>
/// Etat du boss Gromar qui inflige des degats en zone (explosion).
/// </summary>
public class GS_Explosion : GromarState
{
    public GS_Explosion() : base(4) { }
    private float nextStateDelay = 0.3f;

    public override void SetParam(object args)
    {
        nextStateDelay = 0.3f;
        if (args is ExplosionArgs a)
            nextStateDelay = Mathf.Max(0f, a.nextStateDelay);
    }
    public override void OnEnter()
    {
        base.OnEnter();
        if (gromar != null)
            gromar.showExplosionGizmo = true;

        float radius = gromar != null ? gromar.explosionRadius : 3f;
        gromar.animator.SetTrigger("ExplosionAttack");
        // Lance la coroutine qui gere les degats de l'explosion
        //gromar.StartCoroutine(DoExplosionDamage(3f, 1));
    }


    /// <summary>
    /// Applique des degats a tous les joueurs dans un rayon specifique.
    /// </summary>
    public IEnumerator DoExplosionDamage(float radius, int damage)
    {
        Vector2 center = gromar.transform.position;
        Collider2D[] hits = Physics2D.OverlapCircleAll(center, radius);

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                var p = hit.GetComponent<Player>();
                if (p != null) p.ModifyHealth(-damage);
            }
        }

        // petite pause avant de passer a l'etat suivant
        yield return new WaitForSeconds(nextStateDelay);

        if (gromar != null)
            gromar.showExplosionGizmo = false;

        if (gromar.forcedExplosion)
        {
            Machine.StartRandomPattern();
            gromar.forcedExplosion = false;
        }
        else
        {
            NotifyLogicFinished();
        }
          
    }
}
