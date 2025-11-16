using CodeBreaker;
using UnityEngine;

public class GromarState : State
{
    public int StateNumber = -1;
    public Gromar gromar;
    public GromarStateMachine Machine;

    public GromarState(int number = -1) { StateNumber = number; }

    public override void OnEnter()
    {
        // Sync avec l’Animator
        if (gromar != null && gromar.animator != null)
        {
            gromar.animator.SetInteger("StateNumber", StateNumber);
        }
    }
    public override void OnExit() 
    {
        // Sync avec l’Animator
        if (gromar != null && gromar.animator != null)
        {
            gromar.animator.SetInteger("StateNumber", 0);
        }
    }
}
