using System.Collections;
using System.Collections.Generic;
using Unity.Properties;
using UnityEngine;

public abstract class Ennemy : MonoBehaviour
{

    [SerializeField] protected EnnemyInfo _ennemyInfo;
    [SerializeField] protected Transform firingPoint;

    protected SpriteRenderer sprite;
    protected Rigidbody2D rigidBody;
    private Transform _target;
    protected Vector2 _targetDirection;
    
    private float attackCooldown;
    protected float _currentHealth;
    protected virtual void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();

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
        _currentHealth -= amount;
        if (_currentHealth <= 0f)
        {
            Die();
        }

    }
    public abstract void Attack();
    public virtual bool CanAttack()
    {
        return attackCooldown <= 0f;
    }

    protected virtual void ResetCooldown()
    {
        attackCooldown = 1f / Mathf.Max(1, _ennemyInfo.attackSpeed);
    }

    protected virtual void FindPlayer()
    {
        if (_target != null)
        {
            _targetDirection = (_target.position - transform.position).normalized;
        }
    }
    
    protected virtual void Die()
    {
        Destroy(gameObject);
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Bullet"))
        {
            Bullet bullet = collision.collider.GetComponent<Bullet>();
            Debug.Log("got shot");
            if (bullet != null)
            {
                TakeDamage(bullet.damage);
                Destroy(collision.collider.gameObject);
            }
        }

    }
}
