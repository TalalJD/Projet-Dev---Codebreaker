using System.Collections;
using UnityEngine;

public class GS_LaserAttack : GromarState
{
    public GS_LaserAttack() : base(8) { }

    private float warningTime = 1f;   // track window
    private float lockDelay = 0.2f;   // lock-in delay
    private float fireDuration = 1.5f;

    private LineRenderer laser;
    private Transform firingPoint;
    private Transform player;

    private bool firingActive;
    private Vector2 lockedDirection;
    private float damageTimer;

    private float beamLength = 200f;
    private float warningBeamWidth = 0.05f;
    private float shootingBeamWidth = 1.25f;

    public override void SetParam(object args)
    {
        // reset each time
        warningTime = 1f;
        lockDelay = 0.2f;
        fireDuration = 1.5f;

        if (args is LaserArgs a)
        {
            if (a.WarningTime > 0f) warningTime = a.WarningTime;
            if (a.LockDelay > 0f) lockDelay = a.LockDelay;
            if (a.FireDuration > 0f) fireDuration = a.FireDuration;
        }
    }

    public override void OnEnter()
    {
        firingPoint = gromar.ShootingPoint;
        player = gromar.player.transform;

        if (laser == null)
        {
            GameObject laserObj = Object.Instantiate(gromar.laserPrefab);
            laser = laserObj.GetComponent<LineRenderer>();
        }

        gromar.StartCoroutine(LaserRoutine());
    }

    public override void OnExit()
    {
        if (laser != null) laser.enabled = false;
    }

    private IEnumerator LaserRoutine()
    {
        // PHASE 1: warning track
        laser.enabled = true;
        firingActive = false;
        laser.startWidth = warningBeamWidth;
        laser.endWidth = warningBeamWidth;

        float t = 0f;
        while (t < warningTime)
        {
            UpdateLaserPositions(trackPlayer: true);
            t += Time.deltaTime;
            yield return null;
        }

        // PHASE 2: lock-in
        lockedDirection = ((Vector2)player.position - (Vector2)firingPoint.position).normalized;
        laser.startWidth = warningBeamWidth + 0.02f;
        laser.endWidth = warningBeamWidth + 0.02f;

        float lockTimer = 0f;
        while (lockTimer < lockDelay)
        {
            UpdateLaserPositions(trackPlayer: false);
            lockTimer += Time.deltaTime;
            yield return null;
        }

        // PHASE 3: fire
        firingActive = true;
        damageTimer = 0f;
        laser.startWidth = shootingBeamWidth;
        laser.endWidth = shootingBeamWidth;

        float fireTimer = 0f;
        while (fireTimer < fireDuration)
        {
            UpdateLaserPositions(trackPlayer: false);
            DealDamageAlongLaser();
            fireTimer += Time.deltaTime;
            yield return null;
        }

        // end
        firingActive = false;
        laser.enabled = false;
        yield return new WaitForSeconds(0.3f);
        Machine.ExecuteNextState();
    }

    private void UpdateLaserPositions(bool trackPlayer)
    {
        if (!laser || !firingPoint) return;

        Vector3 start = firingPoint.position;
        Vector3 end;

        if (trackPlayer && player != null)
        {
            Vector2 dir = ((Vector2)player.position - (Vector2)firingPoint.position).normalized;
            end = (Vector2)firingPoint.position + dir * beamLength;
        }
        else
        {
            end = (Vector2)firingPoint.position + lockedDirection * beamLength;
        }

        laser.SetPosition(0, start);
        laser.SetPosition(1, end);
    }

    private void DealDamageAlongLaser()
    {
        if (!firingActive) return;

        damageTimer -= Time.deltaTime;
        if (damageTimer > 0f) return;
        damageTimer = 1f;

        Vector2 start = gromar.ShootingPoint.position;
        RaycastHit2D[] hits = Physics2D.RaycastAll(start, lockedDirection, beamLength);

        foreach (var hit in hits)
        {
            if (hit.collider == null) continue;

            if (hit.collider.CompareTag("Player"))
            {
                var p = hit.collider.GetComponent<Player>();
                if (p != null) p.ModifyHealth(-1);
            }
        }
    }
}
