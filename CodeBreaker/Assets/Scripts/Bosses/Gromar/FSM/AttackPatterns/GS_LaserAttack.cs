using System.Collections;
using UnityEngine;

public class GS_LaserAttack : GromarState
{
    public GS_LaserAttack() : base(8) { }

    private float warningTime = 1f;     // Temps ou le joueur ce fait track
    private float lockDelay = 0.2f;     // temps avant le tire ou le laser ce lock
    private float fireDuration = 1.5f;  // temps du tire du laser
    private float damageTimer = 0f;     //temps entre chaque tic de degat

   
   

    private LineRenderer laser;
    private Transform firingPoint;
    private Transform player;

    private bool firingActive;
    private Vector2 lockedDirection;

    private float beamLength = 200f; // distance a la quelle le laser s'etend visuellement

    private float warningBeamWidth = 0.05f; //largeur du tire d'avertissement
    private float shootingBeamWidth = 1.25f; //largeur du vrai tire




    public override void OnEnter()
    {
        firingPoint = gromar.ShootingPoint;
        player = gromar.player.transform;

        // Spawn ou reutilise le laser
        if (laser == null)
        {
            GameObject laserObj = Object.Instantiate(gromar.laserPrefab);
            laser = laserObj.GetComponent<LineRenderer>();
        }

        gromar.StartCoroutine(LaserRoutine());
    }

    private IEnumerator LaserRoutine()
    {
        // --- PHASE 1:tire de precaution (traque le joueur) ---
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

        // --- PHASE 2: LOCK-IN (pas de degat, 0.2s) ---
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

        // --- PHASE 3: Vrai tire (direction locked-in, fait des degats par seconde) ---
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

        // --- FIN ---
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



        Vector2 start = firingPoint.position;
        RaycastHit2D[] hits = Physics2D.RaycastAll(start, lockedDirection, beamLength);

        foreach (var hit in hits)
        {
            if (hit.collider == null)
                continue;
             
             Debug.Log($"Laser hit: {hit.collider.name} (tag: {hit.collider.tag})");

            if (hit.collider.CompareTag("Player"))
            {
                Player p = hit.collider.GetComponent<Player>();
                if (p != null)
                {
                    Debug.Log("Laser essaye de faire des degats!");
                    p.ModifyHealth(-1);
                }
                else
                {
                    Debug.LogWarning("Laser a trouver un collider avec le tag player mais pas de script");
                }
            }

        }
    }


    public override void OnExit()
    {
        if (laser != null)
            laser.enabled = false;
    }
}
