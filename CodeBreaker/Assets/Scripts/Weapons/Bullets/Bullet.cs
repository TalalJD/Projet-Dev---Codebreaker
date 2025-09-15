using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class  Bullet : MonoBehaviour
{
    [SerializeField] private WeaponInfo _weaponInfo;
    private float _speed;
    private float _lifeTime;
    public float damage;
    private Rigidbody2D rb;
    protected virtual void Start()
    {
        _speed = _weaponInfo.bulletSpeed;
        _lifeTime = _weaponInfo.bulletLifeTime;
        damage = _weaponInfo.damage;

        rb = GetComponent<Rigidbody2D>();
        if (rb == null)
            Debug.LogError("Bullet requires a Rigidbody2D on the same GameObject!", this);

        Destroy(gameObject, _lifeTime);
    }


    protected virtual void FixedUpdate()
    {
        transform.Translate(Vector3.up * _speed * Time.fixedDeltaTime, Space.Self);

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
