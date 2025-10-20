using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GS_Idle : GromarState
{
    public float timer = .5f;
    public float currentTimer;

    public override void OnEnter()
    {
        gromar.StartCoroutine(WaitAndAttack());
    }
    private IEnumerator WaitAndAttack()
    {
        yield return new WaitForSeconds(2f); // short delay between patterns
        Machine.StartRandomPattern();
    }
}
