using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class GS_Idle : GromarState
{

 
    public float Idletimer = 1f;
 
    public GS_Idle() : base(0) {}

    public override void SetParam(object[] args)
    {
        Idletimer = (float)args[0];
       
    }

    public override void OnEnter()
    {
        gromar.StartCoroutine(WaitAndAttack());
    }
    private IEnumerator WaitAndAttack()
    {
        yield return new WaitForSeconds(Idletimer); // short delay between patterns
        Machine.StartRandomPattern();
    }
}
