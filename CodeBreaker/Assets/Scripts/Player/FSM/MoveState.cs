using CodeBreaker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveState : PlayerState
{
    public bool jumpRequested; //indique si un saut a ete demander par le joueur

    /// <summary>
    /// Gère le mouvement horizontal du joueur au sol en appliquant 
    /// une accélération, une décélération et une friction selon les inputs.
    /// </summary>
    public void GroundMovement()
    {
        Vector2 MoveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        float maxSpeed = PhysicsInfo.MaxSpeed;

        if (MoveInput.x == 0)
        {
            if (Player.GroundSpeed != 0)
            {
                Player.GroundSpeed -= Mathf.Sign(Player.Direction) * Mathf.Min(PhysicsInfo.Friction * Time.fixedDeltaTime, Mathf.Abs(Player.GroundSpeed));
                if (Mathf.Abs(Player.GroundSpeed) < .0001f)
                {
                    Player.GroundSpeed = 0;
                }
            }
        }
        else if (MoveInput.x > 0)
        {
            if (Player.GroundSpeed < 0)
            {
                Player.GroundSpeed += PhysicsInfo.Deceleration * Time.fixedDeltaTime;
            }
            else if (Player.GroundSpeed < maxSpeed)
            {
                Player.GroundSpeed = Mathf.Min(Player.GroundSpeed + PhysicsInfo.Acceleration * Time.fixedDeltaTime, maxSpeed);
            }
        }
        else
        {
            if (Player.GroundSpeed > 0)
            {
                Player.GroundSpeed -= PhysicsInfo.Deceleration * Time.fixedDeltaTime;
            }
            else if (Player.GroundSpeed > -maxSpeed)
            {
                Player.GroundSpeed = Mathf.Max(Player.GroundSpeed - PhysicsInfo.Acceleration * Time.fixedDeltaTime, -maxSpeed);
            }
        }
    }
    public override void OnEnter()
    {

    }

    public override void OnExit()
    {
        jumpRequested = false; //on le set a false pour eviter que la machine croit que l'on saute toujours
    }
    public override void OnUpdate()
    {
        SetDirection(Player.GroundSpeed); //met a jouer la direction du joueur

        if (Input.GetKeyDown(KeyCode.Space)) 
        {
            jumpRequested = true;
            
        }

    }

    public override void OnFixedUpdate()
    {
        //si le joueur a demander un saut on aplique le jumpstrenght et on change de state
        if (jumpRequested)
        {
            Player.YSpeed = PhysicsInfo.JumpStrength;
            Machine.Get<AirState>().isJump = true;
            Machine.Set<AirState>();
            return;
        }
        
        //si le joueur est pas au sol on change de state
        if (!Player.CheckOnGround())
        {
            Machine.Set<AirState>();
        }

        GroundMovement();
        Player.Rb.velocity = new Vector2(Player.GroundSpeed, 0); //on update la vitesse du rigidbody
    }
}
