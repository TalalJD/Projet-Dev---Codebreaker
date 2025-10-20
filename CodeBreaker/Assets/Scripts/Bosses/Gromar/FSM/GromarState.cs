using CodeBreaker;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GromarState : State
{
    public virtual int StateNumber => -1;
    public Gromar gromar;
    public GromarStateMachine Machine;
   

    public GromarState()
    {
    }

    public override void OnEnter()
    {
        
    }

    public override void OnExit()
    {
       
    }

}
