using System.Collections;
using UnityEngine;

/// <summary>
/// Etat d'attaque laser du boss Gromar.
/// Le laser suit le joueur pendant un court instant, se verrouille,
/// puis tire un rayon infligeant des degats.
/// </summary>
public class GS_LaserAttack : GromarState
{
    public GS_LaserAttack() : base(5) { }

    // durees des differentes phases
    private float warningTime = 1f;   // temps de suivi avant verrouillage
    private float lockDelay = 0.2f;   // petit delai apres le verrouillage
    private float fireDuration = 1.5f; // duree du tir

    // references
    private LineRenderer laser;
    private Transform firingPoint;
    private Transform player;

    // variables internes
    private bool firingActive;
    private Vector2 lockedDirection;
    private float damageTimer;

    // parametres visuels
    private float beamLength = 200f;
    private float warningBeamWidth = 0.05f;   // epaisseur "fine" (utilisee au debut)
    private float shootingBeamWidth = 15f;  // epaisseur max (au bout du laser en phase 3)

    private float nextStateDelay = 0.3f;

    public override void SetParam(object args)
    {
        // reinitialisation
        warningTime = 1f;
        lockDelay = 0.2f;
        fireDuration = 1.5f;
        nextStateDelay = 0.3f;

        if (args is LaserArgs a)
        {
            if (a.WarningTime > 0f) warningTime = a.WarningTime;
            if (a.LockDelay > 0f) lockDelay = a.LockDelay;
            if (a.FireDuration > 0f) fireDuration = a.FireDuration;
            nextStateDelay = Mathf.Max(0f, a.nextStateDelay);
        }
    }

    /// <summary>
    /// Debute l'attaque laser.
    /// </summary>
    public override void OnEnter()
    {
        firingPoint = gromar.LaserSP;
        player = gromar.player.transform;

        gromar.animator.SetTrigger("LaserAttack");

        // cree le laser s'il n'existe pas encore
        if (laser == null)
        {
            GameObject laserObj = Object.Instantiate(gromar.laserPrefab);
            laser = laserObj.GetComponent<LineRenderer>();

            // assure que le laser utilise bien le meme materiel que le prefab
            laser.sharedMaterial = gromar.laserPrefab.GetComponent<LineRenderer>().sharedMaterial;
        }

        // pense a lancer la coroutine depuis la state machine ou ici :
        // gromar.StartCoroutine(LaserRoutine());
    }

    public override void OnExit()
    {
        if (laser != null) laser.enabled = false;
    }

    /// <summary>
    /// Gere le cycle complet du laser :
    /// 1. avertissement (suit le joueur)
    /// 2. verrouillage
    /// 3. tir
    /// </summary>
    public IEnumerator LaserRoutine()
    {
        // PHASE 1 : avertissement
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

        // PHASE 2 : verrouillage
        lockedDirection = ((Vector2)player.position - (Vector2)firingPoint.position).normalized;
        laser.startWidth = warningBeamWidth;
        laser.endWidth = warningBeamWidth;

        float lockTimer = 0f;
        while (lockTimer < lockDelay)
        {
            UpdateLaserPositions(trackPlayer: false);
            lockTimer += Time.deltaTime;
            yield return null;
        }

        // PHASE 3 : tir
        firingActive = true;
        damageTimer = 0f;

        // ICI : forme "cône" -> fin au debut, plus large au bout
        laser.startWidth = warningBeamWidth;    // mince au niveau du shooting point
        laser.endWidth = shootingBeamWidth;   // épais au bout du rayon

        float fireTimer = 0f;
        while (fireTimer < fireDuration)
        {
            UpdateLaserPositions(trackPlayer: false);
            DealDamageAlongLaser();
            fireTimer += Time.deltaTime;
            yield return null;
        }

        // fin de l'attaque
        firingActive = false;
        laser.enabled = false;
        yield return new WaitForSeconds(nextStateDelay);
        Machine.ExecuteNextState();
    }

    /// <summary>
    /// Met a jour les positions du LineRenderer (debut et fin du laser).
    /// </summary>
    private void UpdateLaserPositions(bool trackPlayer)
    {
        if (!laser || !firingPoint) return;

        Vector3 start = firingPoint.position;
        Vector3 end;

        if (trackPlayer && player != null)
        {
            // suit la position actuelle du joueur
            Vector2 dir = ((Vector2)player.position - (Vector2)firingPoint.position).normalized;
            end = (Vector2)firingPoint.position + dir * beamLength;
        }
        else
        {
            // tire dans la direction verrouillee
            end = (Vector2)firingPoint.position + lockedDirection * beamLength;
        }

        laser.SetPosition(0, start);
        laser.SetPosition(1, end);
    }

    /// <summary>
    /// Inflige des degats au joueur touche par le rayon.
    /// </summary>
    private void DealDamageAlongLaser()
    {
        if (!firingActive) return;

        damageTimer -= Time.deltaTime;
        if (damageTimer > 0f) return;
        damageTimer = 1f; // applique les degats une fois par seconde

        Vector2 start = gromar.LaserSP.position;
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
