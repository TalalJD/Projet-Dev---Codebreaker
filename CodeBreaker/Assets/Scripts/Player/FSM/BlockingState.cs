using CodeBreaker;
using UnityEngine;

public class BlockingState : PlayerState
{
    public BlockingState() : base(4) { }

    public override void OnEnter()
    {
        Player.canTakeDmg = false;
        Player.IsBlocking = true;

        // stop any movement immediately to avoid "floating" while blocking
        if (Player != null && Player.Rb != null)
        {
            // zero the Rigidbody2D velocity
            Player.Rb.linearVelocity = Vector2.zero;

            // also zero the convenience properties so other code sees 0 speed
            Player.XSpeed = 0f;
            Player.YSpeed = 0f;
        }

        // destroy equipped weapon so player can't shoot while blocking
        if (Player.SelectedWeapon != null)
        {
            GameObject.Destroy(Player.SelectedWeapon.gameObject);
            Player.SelectedWeapon = null;
            Player.SelectedWeaponInfo = null;
            // notify listeners via raiser method (can't invoke event from outside Player)
            Player.RaiseWeaponInventoryChanged();
        }

        // make sure there is no pending jump when we start blocking
        var move = Machine.Get<MoveState>();
        if (move != null) move.jumpRequested = false;
    }

    public override void OnExit()
    {
        Player.canTakeDmg = true;
        Player.IsBlocking = false;

        // ensure jump is cleared when leaving block
        var move = Machine.Get<MoveState>();
        if (move != null) move.jumpRequested = false;
    }

    public override void OnUpdate()
    {
        // Prevent the player from queuing a jump while blocking.
        // Keep clearing any jump request every frame while blocking.
        var move = Machine.Get<MoveState>();
        if (move != null) move.jumpRequested = false;

        // Exit blocking when the block key is released
        if (!Input.GetKey(KeyCode.M))
        {
            Machine.Set<MoveState>();
            return;
        }
    }
}

