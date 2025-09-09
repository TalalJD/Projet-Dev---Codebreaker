using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class  Bullet : MonoBehaviour
{
    [SerializeField] private WeaponInfo _weaponInfo;
    private float _speed;
    private float _lifeTime;
    private float _damage;
    private Rigidbody2D rb;

    protected virtual void Start()
    {
        _speed = _weaponInfo.bulletSpeed;
        _lifeTime = _weaponInfo.bulletLifeTime;
        _damage = _weaponInfo.damage;

        rb = GetComponent<Rigidbody2D>();
        Destroy(gameObject, _lifeTime);
    }

    protected virtual void FixedUpdate()
    {
        rb.velocity = transform.up * _speed;
    }

    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    if (collision.gameObject.CompareTag("Enemy"))
    //    {
    //        Destroy(gameObject);
    //        Enemy enemy = collision.gameObject.GetComponent<Enemy>();
    //        enemy.TakeDamage(damage);


    //    }
    //}
}
