using CodeBreaker;
using UnityEngine;

public class DashState : PlayerState
{
    public bool dashRequested;
    private float dashTimer;
    private float dashDuration = 0.2f;
    private float dashSpeed = 15f;
    private bool isDashing;
    private bool dashEnd;

    public DashState() : base(3){ }

    public override void OnEnter()
    {
        isDashing = true;
        dashTimer = dashDuration;

        Player.Rb.linearVelocity = new Vector2(Player.Direction * dashSpeed, 0);
        Player.YSpeed = 0;

        Debug.Log("Dash started");
    }

    public override void OnExit()
    {
        isDashing = false;
        Debug.Log("Dash ended");
    }

    public override void OnUpdate()
    {
        SetDirection(Player.Direction);

        if (Input.GetKeyDown(KeyCode.B) && !isDashing)
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
            Player.Rb.linearVelocity = new Vector2(Player.Direction * dashSpeed, 0);
        }
        else
        {
            Machine.Set<MoveState>();
        }

    
    }

   
}