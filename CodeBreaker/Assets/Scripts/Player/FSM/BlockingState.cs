using CodeBreaker;
using UnityEngine;

public class BlockingState : PlayerState
{
    
    public BlockingState() : base(4) { }


    public override void OnEnter()
    {
        Player.canTakeDmg = false;

    }

    public override void OnExit()
    {
        Player.canTakeDmg = true;

        // Commencer le cooldown quand M n'est plus touché
        Player.blockTimer = Player.blockCooldown;  
    }

    public override void OnUpdate()
    {
        if (!Input.GetKey(KeyCode.M))
        {
            
            if (Player.CheckOnGround())
                Machine.Set<MoveState>();
            else
                Machine.Set<AirState>();
        }

        // La direction du player (Sprite)
        float inputX = Input.GetAxisRaw("Horizontal");
        if (inputX != 0)
        {
            Player.Direction = (int)Mathf.Sign(inputX);
            Player.spriteRenderer.flipX = Player.Direction < 0;
        }
    }

    public override void OnFixedUpdate()
    {
        float inputX = Input.GetAxisRaw("Horizontal");

        // vitesse reduit
        float maxSpeedDec = Player.PhysicsInfo.TopSpeed * 0.15f;

        float accel = Player.PhysicsInfo.Acceleration;
        float decel = Player.PhysicsInfo.Deceleration;

        if (inputX == 0)
        {
            // deaccelere si pas de input
            Player.GroundSpeed = Mathf.MoveTowards(
                Player.GroundSpeed,
                0,
                decel * Time.fixedDeltaTime
            );
        }
        else
        {
            // applique le ralentissement
            float target = inputX * maxSpeedDec;

            Player.GroundSpeed = Mathf.MoveTowards(
                Player.GroundSpeed,
                target,
                accel * Time.fixedDeltaTime
            );
        }

        // la velocité
        Player.XSpeed = Player.GroundSpeed;
    }

}

