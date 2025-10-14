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

        Player.spriteRenderer.flipX = Player.Direction == -1;

        if (Player.YSpeed > 0)
        {
            Player.spriteRenderer.sprite = Player.jumpSprite;
        }
        else
        {
            Player.spriteRenderer.sprite = Player.fallSprite;
        }

        if (Player.YSpeed < 0)
        {
            if (Player.CheckOnGround())
            {
                Player.spriteRenderer.sprite = Player.idleSprite;
                Machine.Set<MoveState>();
            }
        }
    }


}
