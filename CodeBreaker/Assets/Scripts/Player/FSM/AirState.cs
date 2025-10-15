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
    public void AirMovement()
    {
        if (isJump && Player.YSpeed > PhysicsInfo.JumpCutoff && Input.GetKeyUp(KeyCode.Space)) 
        {
            Player.YSpeed = PhysicsInfo.JumpCutoff; 
        }

        Player.YSpeed = Player.YSpeed - PhysicsInfo.Gravity*Time.fixedDeltaTime;

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
        AirMovement();

        if (Player.YSpeed < 0)
        {
            if (Player.CheckOnGround())
            {
               
                Machine.Set<MoveState>();
            }
        }
    }


}
