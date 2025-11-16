using CodeBreaker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirState : PlayerState
{
    public AirState() : base(1) { }

    public bool isJump;
    private bool hasDashed;

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

        if (Player.YSpeed < -PhysicsInfo.MaxFallSpeed)
        {
            Player.YSpeed = -PhysicsInfo.MaxFallSpeed;
        }

    }

    public void MovementHorizontal()
    {
        float inputX = Input.GetAxisRaw("Horizontal");
        float maxSpeed = PhysicsInfo.TopSpeed;
        Vector2 velocity = Player.Rb.linearVelocity;
        float airControl = 0.6f;
        float airMaxSpeed = PhysicsInfo.TopSpeed * 0.3f;
       

        float wallCheck = 0.55f;

        //Mettre à jour Player.Direction dans les airs
        if (inputX != 0f)
        {
            Player.Direction = (int)Mathf.Sign(inputX);

            // Flip le Player après le changement de direction dans les airs
            if (Player.spriteRenderer != null)
                Player.spriteRenderer.flipX = Player.Direction < 0;
        }

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

            }
            else
            {
                float accel = PhysicsInfo.AirAcceleration * airControl;
                float decel = PhysicsInfo.AirDeceleration * airControl;

                if (Mathf.Sign(inputX) == Mathf.Sign(Player.GroundSpeed))
                {

                    Player.GroundSpeed = Mathf.MoveTowards(
                        Player.GroundSpeed, // Speed actuelle
                        inputX * airMaxSpeed, // maxSpeed
                        accel * Time.fixedDeltaTime
                    );
                }
                else
                {
                    // direction opposé
                    Player.GroundSpeed = Mathf.MoveTowards(
                        Player.GroundSpeed,
                        inputX * airMaxSpeed,
                        decel * Time.fixedDeltaTime
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
        // Rentre Blocking State
        if (Input.GetKey(KeyCode.M))
        {
            Machine.Set<BlockingState>();
            return;
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            // Peut pas dash 2 fois dans les airs
            if (!hasDashed)
            {
                //Debug.Log("Dashed in the air 1/1");
                hasDashed = true;
                Machine.Set<DashState>();
            }
             
        }
           
    
    }

    // on verifie si le joueur a fini de jump 
    public override void OnFixedUpdate()
    {
        MovementVertical();
        MovementHorizontal();

        Player.Rb.linearVelocity = new Vector2(Player.GroundSpeed, Player.YSpeed);

        if (Player.YSpeed < 0 && Player.CheckWall())
        {
            Machine.Set<WallState>();
            return;
        }

        if (Player.YSpeed <= 0 && Player.CheckOnGround())
        {
            // hasDashed false pour faire une autre dash dans les airs
            hasDashed = false;
            Machine.Set<MoveState>();
            //Debug.Log("Dashed Done 1/1, back on the ground");

        }
    }


}
