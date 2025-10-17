using UnityEngine;

public class StraightBullet : Projectile
{
    private Vector2 direction;

    public override void Initialize(Vector2 origin, Vector2 target)
    {
        base.Initialize(origin, target);
        transform.position = origin;
        direction = (target - origin).normalized;
        transform.right = direction;
    }

    protected override void Update()
    {
        base.Update();
        transform.position += (Vector3)(direction * speed * Time.deltaTime);
    }
}
