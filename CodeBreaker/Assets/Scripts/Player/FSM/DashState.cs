using CodeBreaker;
using UnityEngine;

public class DashState : PlayerState
{
    public bool dashRequested;
    private float dashTimer;
    private float dashDuration = 0.2f;
    private float dashSpeed = 15f;
    private int dashDirection; // -1 ou 1 / gauche ou droite
    private bool isDashing;

    private static float dashCooldown = 2f; // 2-second cooldown
    public static float dashCurrentCooldown = 0f;



    public DashState() : base(3){ }

    public override void OnEnter()
    {
        // Gauche ou droite
        float inputX = Input.GetAxisRaw("Horizontal");

        // Utilise le valeur input pour le dashDirection
        if (inputX != 0f)
        {
            dashDirection = (int)Mathf.Sign(inputX);
        } else
        {
            // Garder la même direction
            dashDirection = (int)Mathf.Sign(Player.Direction == 0 ? 1 : Player.Direction);
        }

        isDashing = true;
        dashTimer = dashDuration;

        // Permet le dash Horizontal
        Player.Rb.linearVelocity = new Vector2(Player.GroundSpeed, Player.Rb.linearVelocity.y);
        Player.YSpeed = 0;


        // Commencer le timer
        dashCurrentCooldown = dashCooldown;

        Debug.Log("Dash started");
    }

    public override void OnExit()
    {
        isDashing = false;

        // Débarassée de movement Horizontal de Dash pour retourner au move state
        Player.GroundSpeed = 0f;
        Player.Rb.linearVelocity = new Vector2(0f, Player.Rb.linearVelocity.y);
    }

    public override void OnUpdate()
    {
        SetDirection(Player.Direction);

        
        

        
        if (Input.GetKeyDown(KeyCode.B) && !isDashing && dashCurrentCooldown <= 0)
        {
            
            Machine.Set<DashState>();
        }


    }

    public override void OnFixedUpdate()
    {
        if (!isDashing) return;

        dashTimer -= Time.fixedDeltaTime;

        if (dashTimer > 0)
        {
            Player.Rb.linearVelocity = new Vector2(dashDirection * dashSpeed, 0);
        }
        else
        {
            Machine.Set<MoveState>();
        }

    
    }

   
}