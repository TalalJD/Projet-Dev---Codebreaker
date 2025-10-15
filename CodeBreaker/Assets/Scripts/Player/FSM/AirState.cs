using CodeBreaker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirState : PlayerState
{
    public AirState() : base(1) { }

    public bool isJump;
    
    /// <summary>
    /// Gere le movement du joueur quand il est dans les aires en appliquant la gravite. 
    /// permet aussi de varier la force du saut dependant que quand le joueur relache la touche espace
    /// </summary>
    public void MovementVertical()
    {
        if (isJump && Player.YSpeed > PhysicsInfo.JumpCutoff && Input.GetKeyUp(KeyCode.Space)) 
        {
            Player.YSpeed = PhysicsInfo.JumpCutoff; 
        }

        Player.YSpeed -= PhysicsInfo.Gravity * Time.fixedDeltaTime;

        if(Player.YSpeed < -PhysicsInfo.MaxFallSpeed)
        {
            Player.YSpeed = -PhysicsInfo.MaxFallSpeed;
        }

    }

    public void MovementHorizontal()
    {
        float inputX = Input.GetAxisRaw("Horizontal");
        float maxSpeed = PhysicsInfo.TopSpeed;
        Vector2 velocity = Player.Rb.linearVelocity;

        float wallCheck = 0.35f;


        if (inputX == 0)
        {
            Player.GroundSpeed -= Player.GroundSpeed * PhysicsInfo.AirDrag * Time.fixedDeltaTime; // tombe
        }
        else
        {
            Vector2 origin = (Vector2)Player.transform.position;
            Vector2 dir = new Vector2(Mathf.Sign(inputX), 0f);
            RaycastHit2D hit = Physics2D.Raycast(origin, dir, wallCheck, Player.LayerMask);
            Debug.DrawRay(origin, dir * wallCheck, hit ? Color.red : Color.green);

            if (hit)
            {
                Player.GroundSpeed = 0f;

                if (Mathf.Sign(Player.GroundSpeed) == Mathf.Sign(inputX))
                {
                    Player.GroundSpeed = 0f;
                }
            }
            else
            {
                if (Mathf.Sign(inputX) == Mathf.Sign(Player.GroundSpeed))
                {

                    Player.GroundSpeed = Mathf.MoveTowards(
                        Player.GroundSpeed, // Speed actuelle
                        inputX * maxSpeed, // maxSpeed
                        PhysicsInfo.AirAcceleration * Time.fixedDeltaTime
                    );
                }
                else
                {
                    // direction opposé
                    Player.GroundSpeed = Mathf.MoveTowards(
                        Player.GroundSpeed,
                        inputX * maxSpeed,
                        PhysicsInfo.AirDeceleration * Time.fixedDeltaTime
                    );
                }
            }
        }
    }


    public override void OnEnter()
    {

    }

    public override void OnExit()
    {
        isJump = false;
        
    }
    public override void OnUpdate()
    {
        
    }

    // on verifie si le joueur a fini de jump 
    public override void OnFixedUpdate()
    {
        MovementVertical();
        MovementHorizontal();

        Player.Rb.linearVelocity = new Vector2(Player.GroundSpeed, Player.YSpeed);

        if (Player.YSpeed < 0)
        {
            if (Player.CheckOnGround())
            {
               
                Machine.Set<MoveState>();
            }
        }
    }


}
