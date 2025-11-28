using UnityEngine;

// Enemy projectile that plugs into your existing Projectile system.
// Moves straight toward the target set at spawn and lets the Player
// receive the hit (Player.OnTriggerEnter2D) to apply damage — avoids double-damage.
public class EnemyProjectile : Projectile
{
    //[SerializeField] private int damage = 1;

    private Vector2 direction;

    public override void Initialize(Vector2 origin, Vector2 target)
    {
        base.Initialize(origin, target);

        // start at origin and compute direction toward target
        transform.position = origin;
        direction = (target - origin).normalized;

        // rotate sprite to face travel direction (optional)
        if (direction != Vector2.zero)
        {
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, angle);
        }
    }

    // Move the projectile each frame using the Projectile.speed field
    protected override void Update()
    {
        base.Update();
        transform.position += (Vector3)(direction * speed * Time.deltaTime);
    }

    // NOTE: Player already handles projectile hits in its OnTriggerEnter2D.
    // To avoid applying damage twice we no longer call Player.ModifyHealth from here.
    // We destroy the projectile on hitting the player and on other non-ignored collisions.
    public override void OnTriggerEnter2D(Collider2D other)
    {
        if (other == null) return;

        // If we hit the player, let Player's OnTriggerEnter2D handle health change.
        if (other.CompareTag("Player"))
        {
            Destroy(gameObject);
            return;
        }

        // Ignore collisions with enemies, enemy bullets and boss
        if (other.CompareTag("Ennemy") || other.CompareTag("EnnemyBullet") || other.CompareTag("Boss"))
            return;

        // Anything else: destroy the projectile (walls, player projectiles, etc.)
        Destroy(gameObject);
    }
}