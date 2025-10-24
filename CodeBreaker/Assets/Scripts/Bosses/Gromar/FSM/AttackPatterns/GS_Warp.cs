using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Etat du boss qui gere les teleports (warps) de Gromar.
/// </summary>
public class GS_Warp : GromarState
{
    private SpriteRenderer[] sprites;             // sprites du boss, caches pendant le teleport
    private readonly Queue<Transform> warpHistory = new(); // historique des derniers points de teleport
    private const int maxHistory = 3;             // taille max de l'historique

    // parametres du warp
    private int warpTimes = 1;
    private bool tpOnPlayer = false;
    private bool tpMiddle = false;
    private bool tpCornerOnly = false;
    private bool tpSpawn = false;
    private bool skipNextState = false;

    public GS_Warp() : base(1) { }

    /// <summary>
    /// Recoit et applique les arguments du warp (WarpArgs).
    /// </summary>
    public override void SetParam(object args)
    {
        // reinitialisation
        warpTimes = 1;
        tpOnPlayer = tpMiddle = tpCornerOnly = tpSpawn = skipNextState = false;

        if (args is WarpArgs w)
        {
            warpTimes = Mathf.Max(1, w.Times);
            tpOnPlayer = w.OnPlayer;
            tpMiddle = w.Middle;
            tpCornerOnly = w.CornerOnly;
            tpSpawn = w.Spawn;
            skipNextState = w.SkipNextState;
        }
    }

    /// <summary>
    /// Lance la sequence de teleportations.
    /// </summary>
    public override void OnEnter()
    {
        sprites = gromar.GetComponentsInChildren<SpriteRenderer>();
        gromar.StartCoroutine(DoWarp());
    }

    /// <summary>
    /// Coroutine qui effectue plusieurs teleports consecutifs.
    /// </summary>
    private IEnumerator DoWarp()
    {
        for (int i = 0; i < warpTimes; i++)
        {
            WarpPosition();
            yield return new WaitForSeconds(0.3f);
        }

        // passe a l'etat suivant sauf si skipNextState est actif
        if (!skipNextState)
        {
            yield return new WaitForSeconds(0.3f);
            Machine.ExecuteNextState();
        }
    }

    /// <summary>
    /// Choisit une nouvelle position selon les options de teleport.
    /// </summary>
    public void WarpPosition()
    {
        DisableSprites(false); // fait disparaitre le sprite pendant le teleport

        if (tpOnPlayer)
        {
            // se teleporte sur le joueur
            var p = gromar.player.transform.position;
            gromar.transform.position = new Vector3(p.x, 1f, gromar.transform.position.z);
        }
        else if (tpMiddle)
        {
            gromar.transform.position = gromar.MAPMIDPOINT.position;
        }
        else if (tpSpawn)
        {
            gromar.transform.position = gromar.SPAWNPOINT.position;
        }
        else
        {
            // se teleporte vers un point aleatoire en evitant les repetitions
            Transform target = GetRandomPointAvoidPattern();
            gromar.transform.position = target.position;

            warpHistory.Enqueue(target);
            if (warpHistory.Count > maxHistory) warpHistory.Dequeue();
        }

        DisableSprites(true); // reapparait
    }

    /// <summary>
    /// Choisit un point aleatoire qui evite les motifs repetitifs (ex: ABAB).
    /// </summary>
    public Transform GetRandomPointAvoidPattern()
    {
        var points = gromar.mapPoints;

        if (warpHistory.Count < 3)
            return GetRandomPointSimple();

        Transform last = warpHistory.Last();
        Transform secondLast = warpHistory.ElementAt(warpHistory.Count - 2);
        Transform thirdLast = warpHistory.First();

        var possible = points
            .Where(p =>
            {
                if (p.position == gromar.transform.position) return false;
                if (tpCornerOnly && p.position == gromar.MAPMIDPOINT.position) return false;

                // evite le pattern ABAB : si on a deja fait A -> B -> A, ne pas refaire B
                if (warpHistory.Count >= 3)
                {
                    if (thirdLast == last && p == secondLast) return false;
                }
                return true;
            })
            .ToList();

        int idx = Random.Range(0, possible.Count);
        return possible[idx];
    }

    /// <summary>
    /// Choisit un point aleatoire sans verification de pattern.
    /// </summary>
    private Transform GetRandomPointSimple()
    {
        var possible = gromar.mapPoints
            .Where(p =>
            {
                if (p.position == gromar.transform.position) return false;
                if (tpCornerOnly && p.position == gromar.MAPMIDPOINT.position) return false;
                return true;
            })
            .ToList();

        int idx = Random.Range(0, possible.Count);
        return possible[idx];
    }

    /// <summary>
    /// Active ou desactive les sprites (utile pendant les teleports).
    /// </summary>
    private void DisableSprites(bool enable)
    {
        if (sprites == null) return;
        foreach (var s in sprites) s.enabled = enable;
    }
}
