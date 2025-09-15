using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseEnnemy : Ennemy
{

    protected void FixedUpdate()
    {
        Vector2 vector = rigidBody.velocity;

        if (_ennemyInfo != null && _targetDirection != Vector2.zero)
        {
            vector.x = _targetDirection.x * _ennemyInfo.movementSpeed;
        }
        else
        {
            vector.x = 0f;
        }
        rigidBody.velocity = vector;
    }

    public override void Attack()
    {
        // Not needed yet for base enemy, leave empty or log
        Debug.Log("Base enemy has no attack yet.");
    }

}
