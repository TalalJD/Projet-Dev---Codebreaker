using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GS_Warp : GromarState
{
    private SpriteRenderer[] sprites;
    private readonly Queue<Transform> warpHistory = new();
    private const int maxHistory = 3;

    private int warpTimes = 1;
    private bool tpOnPlayer = false;
    private bool tpMiddle = false;
    private bool tpCornerOnly = false;
    private bool tpSpawn = false;
    private bool skipNextState = false;

    public GS_Warp() : base(1) { }

    public override void SetParam(object args)
    {
        // Default every time to avoid sticky params
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

    public override void OnEnter()
    {
        sprites = gromar.GetComponentsInChildren<SpriteRenderer>();
        gromar.StartCoroutine(DoWarp());
    }

    private IEnumerator DoWarp()
    {
        for (int i = 0; i < warpTimes; i++)
        {
            WarpPosition();
            yield return new WaitForSeconds(0.3f);
        }

        if (!skipNextState)
        {
            yield return new WaitForSeconds(0.3f);
            Machine.ExecuteNextState();
        }
    }

    public void WarpPosition()
    {
        DisableSprites(false);

        if (tpOnPlayer)
        {
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
            Transform target = GetRandomPointAvoidPattern();
            gromar.transform.position = target.position;

            warpHistory.Enqueue(target);
            if (warpHistory.Count > maxHistory) warpHistory.Dequeue();
        }

        DisableSprites(true);
    }

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

                // avoid ABAB pattern: thirdLast == last and candidate == secondLast
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

    private void DisableSprites(bool enable)
    {
        if (sprites == null) return;
        foreach (var s in sprites) s.enabled = enable;
    }
}
