using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Ennemy : MonoBehaviour
{
    public abstract void TakeDamage();
    public abstract void Attack();
    public abstract void FindPlayer();

}
