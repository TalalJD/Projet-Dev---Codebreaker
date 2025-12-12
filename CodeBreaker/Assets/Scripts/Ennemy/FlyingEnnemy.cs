using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEnnemy : Ennemy
{
    [SerializeField] private Transform firePoint;
    [SerializeField] private GameObject projectilePrefab; // fallback optionnel
    [SerializeField] private float projectileSpeed = 12f;

    [SerializeField] private float attackRange = 10f;
    [SerializeField] private float idleRange = 15f;

    [SerializeField] private float patrolRange = 3f;
    [SerializeField] private float patrolMoveDuration = 2f;
    [SerializeField] private float patrolStopDuration = 2f;
    [SerializeField] private bool enableIdlePatrol = true;

    [SerializeField] private Vector2 followOffset = new Vector2(2f, 4f);
    [SerializeField] private float moveSpeed = 4f;
    
    // exécution (runtime)
    protected float distanceTarget;
    private Rigidbody2D _rb;

    // état d'exécution de la patrouille
    private float patrolCenterX;
    private float patrolMinX;
    private float patrolMaxX;
    private int patrolDirection = 1;
    private float patrolTimer;
    private bool patrolMoving = true;

    protected override void Start()
    {
        base.Start();
        if (firePoint == null)
        {
            firePoint = transform;
        }
        _rb = GetComponent<Rigidbody2D>();
        ResetCooldown();

        // initialiser les limites de patrouille autour de la position d'apparition
        patrolCenterX = transform.position.x;
        patrolMinX = patrolCenterX - patrolRange;
        patrolMaxX = patrolCenterX + patrolRange;

        // initialiser le minutage de patrouille
        patrolMoving = true;
        patrolTimer = patrolMoveDuration;
    }

    private void FixedUpdate()
    {
        if (_target != null)
        {
            distanceTarget = Vector2.Distance(transform.position, _target.position);
        }
        else
        {
            distanceTarget = Mathf.Infinity;
        }

        bool inAttackZone = distanceTarget <= attackRange;

        // Si le joueur est à portée d'attaque et que le cooldown le permet -> arrêter la patrouille brièvement et tirer.
        // Sinon ne pas interrompre la patrouille ; continuer le mouvement de patrouille en idle.
        if (inAttackZone && CanAttack())
        {
            // faire face au joueur et tirer
            RegarderJoueur(_targetDirection.x);

            // interrompre la patrouille brièvement lors du tir : entrer en phase d'arrêt
            patrolMoving = false;
            patrolTimer = patrolStopDuration;

            Attack();
            // Attack() appelle ResetCooldown(); après cette frame le cycle de patrouille normal continuera.
        }

        // Idle / Patrouille s'exécutent toujours (sauf si désactivé). Quand un tir se produit ci-dessus, nous avons déjà défini patrolMoving=false et patrolTimer.
        if (enableIdlePatrol)
        {
            // diminuer le minuteur
            patrolTimer -= Time.fixedDeltaTime;

            if (patrolMoving)
            {
                DoPatrol();
            }
            else
            {
                // phase arrêtée - garder la position (pas de mouvement horizontal)
                // rien à faire ici
            }

            if (patrolTimer <= 0f)
            {
                patrolMoving = !patrolMoving;
                patrolTimer = patrolMoving ? patrolMoveDuration : patrolStopDuration;
            }
        }
    }

    private void DoPatrol()
    {
        if (_rb == null) return;

        // choisir la position X cible en fonction de la direction de patrouille
        float targetX = patrolDirection > 0 ? patrolMaxX : patrolMinX;
        float diff = targetX - _rb.position.x;

        const float epsilon = 0.05f;
        if (Mathf.Abs(diff) <= epsilon)
        {
            // atteint la borne -> inverser la direction
            patrolDirection *= -1;
            targetX = patrolDirection > 0 ? patrolMaxX : patrolMinX;
            diff = targetX - _rb.position.x;
        }

        float dir = Mathf.Sign(diff);
        if (dir == 0) dir = patrolDirection;

        // Se déplacer horizontalement vers la borne prévue tout en conservant Y
        Vector2 newPos = Vector2.MoveTowards(
            _rb.position,
            new Vector2(targetX, _rb.position.y),
            moveSpeed * Time.fixedDeltaTime
        );

        _rb.MovePosition(newPos);

        // faire face à la direction de patrouille (pour éviter que l'ennemi "verrouille" visuellement le joueur pendant la patrouille)
        RegarderJoueur(dir);
    }

    public override void Attack()
    {
        base.Attack();

        // déterminer une position cible fiable (rechercher le joueur si _target a été perdu)
        Vector2 targetPos;
        if (_target != null)
            targetPos = _target.position;
        else
        {
            var go = GameObject.FindGameObjectWithTag("Player");
            targetPos = go != null ? (Vector2)go.transform.position : (Vector2)firePoint.position;
        }

        // Utiliser toujours ProjectileManager pour spawner (retourne une instance Projectile pour régler la vitesse)
        if (ProjectileManager.Instance != null)
        {
            var proj = ProjectileManager.Instance.Spawn(ProjectileType.EnemyBullet, firePoint.position, targetPos);
            if (proj != null)
            {
                proj.speed = projectileSpeed;
            }
        }
        else
        {
            // secours : instancier le prefab et définir la vélocité vers targetPos
            if (projectilePrefab != null && firePoint != null)
            {
                GameObject projectile = Instantiate(projectilePrefab, firePoint.position, Quaternion.identity);
                Rigidbody2D rb = projectile.GetComponent<Rigidbody2D>();
                if (rb != null)
                {
                    var dir = (targetPos - (Vector2)firePoint.position).normalized;
                    rb.linearVelocity = dir * projectileSpeed;
                }
            }
        }

        ResetCooldown();
    }

    // Dessiner des gizmos pour visualiser les portées dans l'éditeur lorsque l'objet est sélectionné
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, idleRange);

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        // dessiner les limites de patrouille
        Gizmos.color = Color.magenta;
        Vector3 min = new Vector3(patrolMinX, transform.position.y - 0.2f, 0f);
        Vector3 max = new Vector3(patrolMaxX, transform.position.y - 0.2f, 0f);
        Gizmos.DrawLine(min, max);

#if UNITY_EDITOR
        if (_target != null)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawLine(transform.position, _target.position);
        }
#endif
    }
}
