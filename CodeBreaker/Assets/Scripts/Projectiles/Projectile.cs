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
   
    if (collision.GetComponent<Projectile>() != null)
        return;

   
    if (collision.CompareTag("Boss") || collision.CompareTag("Ennemy"))
        return;

    
    if (collision.CompareTag("Player"))
        return;

   
}


    protected virtual void Update()
    {
        if (Time.time - spawnTime > lifetime)
            Destroy(gameObject);
    }
}
