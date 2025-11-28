using CodeBreaker;
using UnityEngine;

public class BlockingState : PlayerState
{
    public BlockingState() : base(4) { }

    private float timer = 1f;

    public override void OnFixedUpdate()
    {
        // use fixedDeltaTime in fixed update
        timer -= Time.fixedDeltaTime;
        if (timer <= 0f)
        {
            // Do a proper state transition instead of calling OnExit directly.
            // This ensures the state machine runs OnExit and the Player animator/state are updated correctly.
            Machine.Set<MoveState>();

            // reset the timer for next time we enter the blocking state
            timer = 1f;
        }
    }

    public override void OnEnter()
    {
        // reset timer each time we enter blocking
        timer = 1f;

        Player.canTakeDmg = false;
        Player.IsBlocking = true;

        if (Player != null && Player.Rb != null)
        {
            Player.Rb.linearVelocity = Vector2.zero;
            Player.XSpeed = 0f;
            Player.YSpeed = 0f;
        }

        // clear held item while blocking
        Player.ClearHeldItem();

        var move = Machine.Get<MoveState>();
        if (move != null) move.jumpRequested = false;
    }

    public override void OnExit()
    {
        Player.canTakeDmg = true;
        Player.IsBlocking = false;

        // Prevent immediate re-entry while the block key may still be held:
        // start the block cooldown so MoveState won't immediately set BlockingState again.
        Player.blockTimer = Player.blockCooldown;

        // Optionally trigger a quick animator reset if needed
        if (Player.animator != null)
        {
            // If you have a specific trigger/bool for blocking, reset it here.
            // Example: Player.animator.ResetTrigger("Block");
            // We leave this commented because your animator is driven by StateNumber in UpdateAnimator().
            // Player.animator.ResetTrigger("Block");
        }

        // Put player back to next held item if you want; previously you used CycleHeldItem()
        Player.CycleHeldItem();

        // ensure jump is cleared when leaving block
        var move = Machine.Get<MoveState>();
        if (move != null) move.jumpRequested = false;
    }

    public override void OnUpdate()
    {
        // Prevent the player from queuing a jump while blocking.
        var move = Machine.Get<MoveState>();
        if (move != null) move.jumpRequested = false;

        // Exit blocking when the block key is released.
        // If OnExit was invoked via timer/other path, we still want the state machine to handle the exit.
        if (!Input.GetKey(KeyCode.M))
        {
            Machine.Set<MoveState>();
            return;
        }
    }
}

