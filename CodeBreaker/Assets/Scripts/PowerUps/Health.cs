using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{

    public float maxHealth;
    public float currentHealth;
    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
    }

    void takeDamageTest(int amount)
    {
        currentHealth -= amount;

        if (currentHealth < 0) {
            Console.WriteLine("dead");
        }
    }
    
}
