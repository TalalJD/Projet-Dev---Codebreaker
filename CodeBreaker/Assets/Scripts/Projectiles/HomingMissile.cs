using UnityEngine;

public class HomingMissile : Projectile
{
    public Transform target;
    public float turnSpeed = 200f;

    private Rigidbody2D rb;

    void Awake() => rb = GetComponent<Rigidbody2D>();

    public override void Initialize(Vector2 origin, Vector2 targetPos)
    {
        base.Initialize(origin, targetPos);
        transform.position = origin;
    }

    protected override void Update()
    {
        base.Update();
        if (target == null) return;

        Vector2 direction = ((Vector2)target.position - (Vector2)transform.position).normalized;
        float rotateAmount = Vector3.Cross(direction, transform.right).z;

        rb.angularVelocity = -rotateAmount * turnSpeed;
        rb.linearVelocity = transform.right * speed;
    }
}
