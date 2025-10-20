using log4net.Util;
using System;
using System.Collections;
using UnityEngine;

public class GS_Explosion : GromarState
{
    public override void OnEnter() => DoExplosionDamage(3,1);

    public IEnumerator DoExplosionDamage(float radius, int damage)
    {
        // Centre de l'explosion (le boss)
        Vector2 center = gromar.transform.position;

        // Tous les objets dans le rayon
        Collider2D[] hits = Physics2D.OverlapCircleAll(center, radius);

        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Player")) 
            {
                Debug.Log("Player hit by Gromar explosion!");

                
                Player player = hit.GetComponent<Player>();
                if (player != null)
                    player.ModifyHealth(-damage);
            }
        }

        // Debug visuel dans la scène
        DrawDebugCircle(center, radius, Color.red);

        yield return new WaitForSeconds(1.5f);
        Machine.ExecuteNextState();

    }
    void DrawDebugCircle(Vector2 center, float radius, Color color, int segments = 32)
    {
        float step = 360f / segments;
        for (int i = 0; i < segments; i++)
        {
            float radA = Mathf.Deg2Rad * (i * step);
            float radB = Mathf.Deg2Rad * ((i + 1) * step);

            Vector3 pointA = center + new Vector2(Mathf.Cos(radA), Mathf.Sin(radA)) * radius;
            Vector3 pointB = center + new Vector2(Mathf.Cos(radB), Mathf.Sin(radB)) * radius;

            Debug.DrawLine(pointA, pointB, color, 0.5f);
        }
    }




    public override void OnExit() { }
}
