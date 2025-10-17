using UnityEngine;

public class ParabolicMissile : Projectile
{
    public float arcHeight = 5f;
    public float apexBias = 0.3f;

    private Vector2 startPos;
    private Vector2 targetPos;
    private float journeyLength;

    public override void Initialize(Vector2 origin, Vector2 target)
    {
        base.Initialize(origin, target);
        startPos = origin;
        targetPos = target;
        journeyLength = Vector2.Distance(origin, target);
    }

    protected override void Update()
    {
        base.Update();

        float elapsed = (Time.time - spawnTime) * speed;
        float t = Mathf.Clamp01(elapsed / journeyLength);

        Vector2 pos = Vector2.Lerp(startPos, targetPos, t);

        // Courbe douce
        float parabola;
        if (t < apexBias)
            parabola = Mathf.Sin((t / apexBias) * Mathf.PI * 0.5f);
        else
            parabola = Mathf.Cos(((t - apexBias) / (1f - apexBias)) * Mathf.PI * 0.5f);

        pos.y += (arcHeight + journeyLength * 0.5f) * parabola;
        transform.position = pos;

        // Rotation dans la direction du mouvement
        Vector2 next = Vector2.Lerp(startPos, targetPos, Mathf.Clamp01(t + 0.01f));
        Vector2 dir = (next - pos).normalized;
        transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg);

        if (t >= 1f) Destroy(gameObject);
    }
}
