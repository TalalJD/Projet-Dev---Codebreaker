using CodeBreaker;
using UnityEngine;

public class WallState : PlayerState
{
    public WallState() : base(2) { }

    private bool isOnRightWall;
    private bool jumpRequested;
    private float wallSlideSpeed = 0.6f;
    private float inputKeyTimer = 0f;

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
            Debug.Log("On wall");
        }

        if ((isOnRightWall && inputX < 0) || (!isOnRightWall && inputX > 0))
        {
            Machine.Set<MoveState>();
            Debug.Log("Detached from wall");
            return;
        }

        // retourn au move state quand player touch le sol
        if (Player.CheckOnGround())
        {
            Machine.Set<MoveState>();
        }

        // Rentre Blocking State
        if (Input.GetKey(KeyCode.M))
        {
            Machine.Set<BlockingState>();
            return;
        }
    }

    public override void OnFixedUpdate()
    {


        // verifier si player est sur le mur
        if (!Player.CheckWall())
        {
            Machine.Set<AirState>();
            return;
        }
        

        // check input
        if (Input.GetAxisRaw("Horizontal") == 0 && Input.GetAxisRaw("Vertical") == 0)
        {
            inputKeyTimer += Time.fixedDeltaTime;
            //Player.YSpeed = -wallSlideSpeed;
            if (jumpRequested)
            {
                //Debug.Log("Jmped");
                inputKeyTimer = 0f;
            }
            else if (inputKeyTimer >= 0.15f) // petit delai avant de tomber
            {
                //Debug.Log("sliding down");
                Player.YSpeed = -wallSlideSpeed;
            }
        }
        else
        {
            inputKeyTimer = 0f;
            //Debug.Log("wasd not clicked");
        }
        Player.Rb.linearVelocity = new Vector2(0, Player.YSpeed);
        

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