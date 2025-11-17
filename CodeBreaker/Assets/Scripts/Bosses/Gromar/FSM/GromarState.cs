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

    }
    public override void OnExit() 
    {
      
    }
}
