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
        if (collision.tag != "Player" && collision.tag != "Ennemy" && collision.tag != "Boss" && collision.tag != "EnnemyBullet")
        {
            Destroy(gameObject);
        }

    }

    protected virtual void Update()
    {
        if (Time.time - spawnTime > lifetime)
            Destroy(gameObject);
    }
}
