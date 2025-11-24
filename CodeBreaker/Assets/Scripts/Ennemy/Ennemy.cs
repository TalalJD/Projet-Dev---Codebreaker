using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Properties;
using UnityEngine;

public abstract class Ennemy : MonoBehaviour
{

    [SerializeField] public EnnemyInfo _ennemyInfo;

    protected SpriteRenderer sprite;
    protected Rigidbody2D rigidBody;
    protected Animator animator;
    protected Transform _target;
    protected Vector2 _targetDirection;
    
    protected float attackCooldown;
    protected float _currentHealth;
    protected bool inAttack;

    protected virtual void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
        animator = GetComponentInChildren<Animator>();
        attackCooldown = 0f;
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if (player != null)
        {
            _target = player.transform;
        }

        if (_ennemyInfo != null)
        {
            _currentHealth = _ennemyInfo.healthPoints;
        }
    }

    protected virtual void OnTriggerEnter2D(Collider2D other)
    {

        if (other.CompareTag("PlayerBullet")) // Make sure your bullet has tag "Bullet"
        {
            TakeDamage(1);
        }
    }

    protected virtual void Update()
    {
        if (attackCooldown > 0f)
        {
            attackCooldown -= Time.deltaTime;
        }
        FindPlayer();
    }
    public virtual void TakeDamage(float amount)
    {
        animator.SetTrigger("Damaged");
        _currentHealth -= amount;
        Debug.Log($"took damage {_currentHealth}");
        if (_currentHealth <= 0f)
        {
            Die();
        }

    }
    public virtual void Attack()
    {
        animator.SetTrigger("Attack");
    }
    public virtual bool CanAttack()
    {
        return attackCooldown <= 0f;
    }

    protected virtual void ResetCooldown()
    {
        attackCooldown = _ennemyInfo.attackSpeed;
    }

    protected virtual void RegarderJoueur(float directionX)
    {
        if (Mathf.Approximately(directionX, 0f))
        {
            return;
        }

        Vector3 localScale = transform.localScale;

        if (directionX > 0f)
        {
            localScale.x = Mathf.Abs(localScale.x);
        }
        else if (directionX < 0f)
        {
            localScale.x = -Mathf.Abs(localScale.x);
        }
        transform.localScale = localScale;
    }

    protected virtual void FindPlayer()
    {
        // Re-acquire the player if we lost the reference (handles player respawn/new instance)
        if (_target == null)
        {
            var go = GameObject.FindGameObjectWithTag("Player");
            if (go != null)
                _target = go.transform;
        }

        if (_target != null)
        {
            _targetDirection = (_target.position - transform.position).normalized;
        }
    }
    
    protected virtual void Die()
    {
        Destroy(gameObject);
    }

    public float CurrentHealth => _currentHealth;
}
