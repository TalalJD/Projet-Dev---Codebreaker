using UnityEngine;

public class ConeProjectile : Projectile
{
    private Vector2 direction;

    public override void Initialize(Vector2 origin, Vector2 target)
    {
        base.Initialize(origin, target);
        transform.position = origin;

        // direction toward player
        direction = (target - origin).normalized;

        // angle of that direction (0° = right)
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // rotate sprite to face that direction
        transform.rotation = Quaternion.Euler(0f, 0f, angle+180f);
    }

    protected override void Update()
    {
        base.Update();
        transform.position += (Vector3)(direction * speed * Time.deltaTime);
    }
}
