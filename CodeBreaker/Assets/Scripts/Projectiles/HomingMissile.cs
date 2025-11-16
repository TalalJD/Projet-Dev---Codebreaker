using UnityEngine;

public class HomingMissile : Projectile
{
    //Homing
    public float homingDuration = 5f;        // temps de suivi
    public float turnSpeed = 45f;           // vitesse de rotation (deg/s)

    //Après homing
    public float boostDuration = 2f;         // temps avant destruction
    public float boostSpeedMultiplier = 2f;  // vitesse * X pendant le boost

    private Transform target;
    private float homingEndTime;
    private float boostEndTime;
    private bool boosting;
    private Vector2 lastDirection;           // vraie direction de mouvement

    public override void Initialize(Vector2 origin, Vector2 initialTargetPos)
    {
        base.Initialize(origin, initialTargetPos);

        transform.position = origin;

        // direction initiale vers la cible
        Vector2 dir = (initialTargetPos - origin).normalized;
        if (dir == Vector2.zero)
            dir = Vector2.left;             // par défaut vers la gauche

        // le sprite regarde vers la GAUCHE => forward = -transform.right
        // donc on règle la rotation pour que -right pointe vers 'dir'
        transform.right = -dir;

        lastDirection = dir;                // on mémorise la vraie direction de mouvement

        homingEndTime = Time.time + homingDuration;

        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            target = playerObj.transform;
    }

    protected override void Update()
    {
        base.Update(); // gère la lifetime standard

        Vector2 moveDir = lastDirection;

        bool homingActive = Time.time < homingEndTime && target != null;

        if (homingActive)
        {
            // PHASE 1 : homing
            Vector2 toTarget = (Vector2)target.position - (Vector2)transform.position;

            if (toTarget.sqrMagnitude > 0.0001f)
            {
                toTarget.Normalize();

                // forward visuel = -transform.right (sprite vers la gauche)
                float angleToTarget = Vector3.SignedAngle(-transform.right, toTarget, Vector3.forward);

                float maxStep = turnSpeed * Time.deltaTime;
                float step = Mathf.Clamp(angleToTarget, -maxStep, maxStep);

                transform.Rotate(0f, 0f, step);

                // mettre à jour la vraie direction de mouvement
                lastDirection = -transform.right;
            }

            moveDir = lastDirection;
        }
        else
        {
            // PHASE 2 : boost puis destruction
            if (!boosting)
            {
                boosting = true;
                boostEndTime = Time.time + boostDuration;
            }

            if (Time.time >= boostEndTime)
            {
                Destroy(gameObject);
                return;
            }

            moveDir = lastDirection; // continue tout droit
        }

        float currentSpeed = boosting ? speed * boostSpeedMultiplier : speed;
        transform.position += (Vector3)(moveDir * currentSpeed * Time.deltaTime);
    }
}
