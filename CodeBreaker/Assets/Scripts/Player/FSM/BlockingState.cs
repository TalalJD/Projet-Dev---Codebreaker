using CodeBreaker;
using UnityEngine;

public class BlockingState : PlayerState
{
    public BlockingState() : base(4) { }

    public override void OnEnter()
    {
        Player.canTakeDmg = false;
        Player.IsBlocking = true;

        if (Player != null && Player.Rb != null)
        {
            Player.Rb.linearVelocity = Vector2.zero;
            Player.XSpeed = 0f;
            Player.YSpeed = 0f;
        }

        // --- MODIFICATION ICI ---
        // Au lieu de détruire manuellement et d'appeler RaiseWeaponInventoryChanged,
        // utilise la méthode publique qui existe déjà dans Player :
        Player.ClearHeldItem();
        // ------------------------

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

