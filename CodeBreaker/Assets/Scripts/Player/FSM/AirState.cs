using CodeBreaker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirState : PlayerState
{
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

        if (inputX == 0)
        {
            Player.GroundSpeed -= Player.GroundSpeed * PhysicsInfo.AirDrag * Time.fixedDeltaTime;
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

    public override void OnFixedUpdate()
    {
        MovementVertical();
        MovementHorizontal();

        Player.Rb.velocity = new Vector2(Player.GroundSpeed, Player.YSpeed);

        //on verifie si le joueur a fini de jump
        if (Player.YSpeed < 0)
        {
            if (Player.CheckOnGround())
            {
                Machine.Set<MoveState>();
            }
        }
        
    }

}
