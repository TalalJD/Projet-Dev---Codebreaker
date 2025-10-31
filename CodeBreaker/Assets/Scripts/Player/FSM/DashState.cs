using CodeBreaker;
using UnityEngine;

public class DashState : PlayerState
{
    public bool dashRequested;
    private float dashTimer;
    private float dashDuration = 0.2f;
    private float dashSpeed = 15f;
    private bool dashEnd;

    public DashState() : base(3){ }

    public void requestDash()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            dashRequested = true;
        }
    }
    public void dashHorizontal()
    {
        if(Input.GetKeyUp(KeyCode.E))
        {
            dashRequested = true;
        }
    }

    public override void OnEnter()
    {
        dashTimer = 0f;
        dashEnd = false;
    }

    public override void OnExit()
    {
        dashRequested = false;
        Player.GroundSpeed = 0f;
    }

    public override void OnUpdate()
    {
        
    }

    public override void OnFixedUpdate()
    {
        
    }

   
}