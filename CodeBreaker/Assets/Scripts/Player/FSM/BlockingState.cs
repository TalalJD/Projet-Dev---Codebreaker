using CodeBreaker;
using UnityEngine;

public class BlockingState : PlayerState
{
    public BlockingState() : base(4) { }

    public override void OnEnter()
    {
        Player.canTakeDmg = false;

    }

    public override void OnExit()
    {
        Player.canTakeDmg = true;
    }

    public override void OnUpdate()
    {
        if (!Input.GetKey(KeyCode.M))
        {
            Machine.Set<MoveState>();  
        }
    }
}
