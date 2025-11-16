using System;
using UnityEngine;

public abstract class Projectile : MonoBehaviour
{
    [Header("Général")]
    public float speed = 10f;
    public float lifetime = 10f;
    public bool isBouncy;

    protected float spawnTime;

    // Méthode appelée après l’instanciation pour configurer le projectile
    public virtual void Initialize(Vector2 origin, Vector2 target)
    {
        spawnTime = Time.time;
    }

   public virtual void OnTriggerEnter2D(Collider2D collision)
{
    // Ignore other projectiles
    if (collision.GetComponent<Projectile>() != null)
        return;

    // Ignore friendly fire (boss or enemies)
    if (collision.CompareTag("Boss") || collision.CompareTag("Ennemy"))
        return;

    // Ignore the player — player script already handles damage logic
    if (collision.CompareTag("Player"))
        return;

    // Destroy if it hits anything else (like walls, ground, etc.)
    //Destroy(gameObject);
}


    protected virtual void Update()
    {
        if (Time.time - spawnTime > lifetime)
            Destroy(gameObject);
    }
}
