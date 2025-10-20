using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class GS_Idle : GromarState
{

    public override int StateNumber => 0;
    public float Idletimer = 1f;
   
    public GS_Idle(float timer = 1f) { Idletimer = timer; }

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
