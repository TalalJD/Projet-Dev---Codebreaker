using System.Collections;
using UnityEngine;

public class GS_Explosion : GromarState
{
    public GS_Explosion() : base(4) { }

    public override void OnEnter()
    {
        gromar.StartCoroutine(DoExplosionDamage(3f, 1));
    }

    private IEnumerator DoExplosionDamage(float radius, int damage)
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

        // optional: debug circle
        yield return new WaitForSeconds(0.3f);
        Machine.ExecuteNextState();
    }
}
