using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WalkingEnnemy : Ennemy
{
    private float moveSpeed = 12f;
    private float gravity = -30f;
    private float maxFallSpeed = -20f;

    [SerializeField] private LayerMask LayerMask;
    private float groundCheck = 0.55f;

    private Vector2 _velocity;
    private bool _isGrounded;
    [SerializeField] private GameObject capsuleHitbox;
    private SpriteRenderer _renderer;
    private float attackDuration = 0.5f;
    private float aggroRange = 8f;
    [SerializeField] private float idleRange = 20f;
    [SerializeField] private float inAttackRange = 1f;
    protected float distanceTarget;
        
    // Champs de patrouille
    [Header("Patrol (idle)")]
    [SerializeField] private bool enablePatrol = true;
    [SerializeField] private float patrolRange = 3f; // demi-portée autour de la position de départ
    private float patrolCenterX;
    private float patrolMinX;
    private float patrolMaxX;
    private int patrolDirection = 1; // 1 = droite, -1 = gauche

    // Timing de patrouille en idle : marcher X secondes, puis s'arrêter Y secondes
    [SerializeField] private float patrolMoveDuration = 2f;
    [SerializeField] private float patrolStopDuration = 2f;
    private float patrolTimer;
    private bool patrolMoving = true;
    private bool wasInIdleRange = false;

    protected override void Start()
    {
        base.Start();
        _renderer = capsuleHitbox.GetComponent<SpriteRenderer>();
        capsuleHitbox.SetActive(false);
        _renderer.enabled = false;

        // initialiser les bornes de patrouille autour de la position de départ
        patrolCenterX = transform.position.x;
        patrolMinX = patrolCenterX - patrolRange;
        patrolMaxX = patrolCenterX + patrolRange;

        // initialiser le minuteur/état de patrouille
        patrolMoving = true;
        patrolTimer = patrolMoveDuration;
        wasInIdleRange = false;
    }
    protected void FixedUpdate()
    {
        if (_target != null)
        {
            distanceTarget = Vector2.Distance(transform.position, _target.position);
        }
        else
        {
            distanceTarget = Mathf.Infinity;
        }

        // aggro = dans le cercle jaune
        bool aggro = distanceTarget <= aggroRange;
        // withinIdleRange = dans le cercle cyan (le joueur est assez proche pour que l'ennemi idle/patrouille s'il n'est pas en aggro)
        bool withinIdleRange = distanceTarget <= idleRange;
        bool tryAttack = distanceTarget <= inAttackRange;

        if (inAttack)
        {
            // en train d'effectuer l'animation/hit d'attaque - ne pas se déplacer
            _velocity.x = 0;
        }
        else if (tryAttack)
        {
            // Le joueur est à portée d'attaque : arrêter de se déplacer vers lui pour ne pas le pousser
            _velocity.x = 0;
            // continuer à faire face au joueur
            RegarderJoueur(_targetDirection.x);
        }
        else if (aggro)
        {
            // poursuivre le joueur uniquement lorsqu'il est hors de la portée d'attaque
            float directionX = Mathf.Sign(_targetDirection.x) * moveSpeed;
            _velocity.x = directionX;
            RegarderJoueur(_targetDirection.x);

            // en quittant la zone d'idle - réinitialiser le cycle de patrouille pour qu'il redémarre la prochaine fois
            wasInIdleRange = false;
        }
        else if (withinIdleRange && enablePatrol)
        {
            // Le joueur est dans la zone d'idle mais pas en aggro -> patrouille avec cycle marche/arrêt temporisé

            // si on vient d'entrer dans la zone d'idle, réinitialiser le cycle de patrouille
            if (!wasInIdleRange)
            {
                wasInIdleRange = true;
                patrolMoving = true;
                patrolTimer = patrolMoveDuration;
            }

            // mettre à jour le minuteur de patrouille
            patrolTimer -= Time.fixedDeltaTime;

            if (patrolMoving)
            {
                DoPatrol();
            }
            else
            {
                // phase arrêtée
                _velocity.x = 0;
            }

            if (patrolTimer <= 0f)
            {
                // basculer marche/arrêt et réinitialiser le minuteur en conséquence
                patrolMoving = !patrolMoving;
                patrolTimer = patrolMoving ? patrolMoveDuration : patrolStopDuration;
            }
        }
        else
        {
            // Le joueur est plus loin que la zone d'idle (ou la patrouille est désactivée) -> arrêter le mouvement
            _velocity.x = 0;

            // réinitialiser le cycle de patrouille pour qu'il recommence proprement la prochaine fois qu'on entre en zone d'idle
            wasInIdleRange = false;
            patrolMoving = true;
            patrolTimer = patrolMoveDuration;
        }

        _isGrounded = CheckOnGround();

        if (!_isGrounded)
        {
            _velocity.y += gravity * Time.fixedDeltaTime;
            if (_velocity.y < maxFallSpeed)
            {
                _velocity.y = maxFallSpeed;
            }
        }
        else
        {
            _velocity.y = Mathf.Min(_velocity.y, -0.5f);
        }

        Vector2 mouvement = rigidBody.position + _velocity * Time.fixedDeltaTime;
        rigidBody.MovePosition(mouvement);
        if (CanAttack() && !inAttack && tryAttack)
        {
            Attack();
        }

        if (_velocity.x != 0)
        {
            animator.SetBool("IsWalking", true);
        }
        else
        {
            animator.SetBool("IsWalking", false);
        }
    }

    private void DoPatrol()
    {
        // Déterminer la direction vers la cible de patrouille actuelle et se déplacer
        float targetX = patrolDirection > 0 ? patrolMaxX : patrolMinX;
        float diff = targetX - transform.position.x;

        // Si très proche de la cible, inverser la direction
        const float epsilon = 0.05f;
        if (Mathf.Abs(diff) <= epsilon)
        {
            patrolDirection *= -1;
            targetX = patrolDirection > 0 ? patrolMaxX : patrolMinX;
            diff = targetX - transform.position.x;
        }

        float dir = Mathf.Sign(diff);
        if (dir == 0) dir = patrolDirection; // secours

        _velocity.x = dir * moveSpeed;

        // faire face à la direction de patrouille
        RegarderJoueur(dir);
    }

    private bool CheckOnGround()
    {
        var hit = Physics2D.Raycast(transform.position, Vector2.down, groundCheck, LayerMask);

        Debug.DrawRay(transform.position, Vector2.down * .25f);
        if (hit)
        {
            float offset = 0.1f;
            rigidBody.position = new Vector2(rigidBody.position.x, rigidBody.position.y + offset);
            return true;
        }

        return false;
    }
    public override void Attack()
    {
        if (_target == null)
        {
            return;
        }

        float distNow = Vector2.Distance(transform.position, _target.position);
        if (distNow > inAttackRange)
        {
            return;
        }
        base.Attack();
        StartCoroutine(DoAttack());
    }

    private IEnumerator DoAttack()
    {
        _renderer.enabled = false;
        inAttack = true;
        capsuleHitbox.SetActive(true);
        yield return new WaitForSeconds(attackDuration);
        inAttack = false;
        _renderer.enabled = false;
        capsuleHitbox.SetActive(false);
        ResetCooldown();
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow; Gizmos.DrawWireSphere(transform.position, aggroRange);
        Gizmos.color = Color.cyan; Gizmos.DrawWireSphere(transform.position, idleRange);
        Gizmos.color = Color.red; Gizmos.DrawWireSphere(transform.position, inAttackRange);

        // dessiner les limites de patrouille dans l'éditeur
        Gizmos.color = Color.magenta;
        Gizmos.DrawLine(new Vector3(patrolMinX, transform.position.y - 0.2f, 0f), new Vector3(patrolMaxX, transform.position.y - 0.2f, 0f));
    }
}
