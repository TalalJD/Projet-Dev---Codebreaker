using CodeBreaker;
using UnityEngine;

public class WallState : PlayerState
{
    public WallState() : base(2) { }

    private bool isOnRightWall;
    private bool jumpRequested;
    private float wallSlideSpeed = 0.6f;

    public override void OnEnter()
    {
        // quelle direction on est
        isOnRightWall = Player.CheckRightWall();
        Player.GroundSpeed = 0;
        Player.YSpeed = Mathf.Min(Player.YSpeed, 0); // stop rising if climbing wall
        Player.Rb.linearVelocity = Vector2.zero;
        // Pour l'animation
        // Player.animator.SetBool("isWallSliding", true);
    }

    public override void OnExit()
    {   
        // Pour l'animation
        // Player.animator.SetBool("isWallSliding", false);
        jumpRequested = false;
    }

    public override void OnUpdate()
    {
        
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpRequested = true;
        }

        // sortir de move state quand player se détach
        float inputX = Input.GetAxisRaw("Horizontal");

        if ((isOnRightWall && inputX > 0) || (!isOnRightWall && inputX < 0))
        {
            Machine.Set<AirState>();
        }

        // retourn au move state quand player touch le sol
        if (Player.CheckOnGround())
        {
            Machine.Set<MoveState>();
        }
    }

    public override void OnFixedUpdate()
    {
        float inputKeyTimer = 0f;

        // verifier si player est sur le mur
        if (!Player.CheckWall())
        {
            Machine.Set<AirState>();
            return;
        }
        if((isOnRightWall && Input.GetKeyDown(KeyCode.A)) || 
            (!isOnRightWall && Input.GetKeyDown(KeyCode.D)))
        {
            Machine.Set<AirState>();
        }

        // check input
        if (Input.GetAxisRaw("Horizontal") == 0 && Input.GetAxisRaw("Vertical") == 0)
        {
            Player.YSpeed = -wallSlideSpeed;

        //    inputKeyTimer += Time.fixedDeltaTime;
        //    if (inputKeyTimer >= 1f)
        //    {
        //        Debug.Log("sliding down");
        //        Player.YSpeed = -wallSlideSpeed;
        //    } 
        //} else
        //{
        //    inputKeyTimer = 0f;
        //    Debug.Log("wasd not clicked");
        }
            Player.Rb.linearVelocity = new Vector2(0, Player.YSpeed);
        if (Input.GetKeyDown(KeyCode.A))
        {
            
        }

            if (jumpRequested)
        {
            jumpRequested = false;

            Vector2 jumpDir;
            if (isOnRightWall)
            {
                jumpDir = new Vector2(-1, 1); // sauter gauche
                Player.Direction = -1;
            }
            else
            {
                jumpDir = new Vector2(1, 1); // sauter droite
                Player.Direction = 1;
            }

            jumpDir.Normalize();

            Player.YSpeed = PhysicsInfo.WallJumpVert;
            Player.GroundSpeed = jumpDir.x * PhysicsInfo.WallJumpHori;

            Machine.Get<AirState>().isJump = true;
            Machine.Set<AirState>();
            // Player.animator.SetTrigger("Jump");
        }
    }
}