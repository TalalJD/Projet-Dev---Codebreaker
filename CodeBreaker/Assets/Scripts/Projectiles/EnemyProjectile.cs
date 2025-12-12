using UnityEngine;

public class EnemyProjectile : Projectile
{
   
    private Vector2 direction;

    public override void Initialize(Vector2 origin, Vector2 target)
    {
        base.Initialize(origin, target);

        
        transform.position = origin;
        direction = (target - origin).normalized;

        
        if (direction != Vector2.zero)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }
    }

   
    protected override void Update()
    {
        base.Update();
        transform.position += (Vector3)(direction * speed * Time.deltaTime);
    }

    
    public override void OnTriggerEnter2D(Collider2D other)
    {
        if (other == null) return;

        
        if (other.CompareTag("Player"))
        {
            Destroy(gameObject);
            return;
        }

        
        if (other.CompareTag("Ennemy") || other.CompareTag("EnnemyBullet") || other.CompareTag("Boss"))
            return;

        
        Destroy(gameObject);
    }
}