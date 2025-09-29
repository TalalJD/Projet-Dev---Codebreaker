using CodeBreaker;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gromar : MonoBehaviour
{
    public Transform MINSHOOT;
    public Transform MAXSHOOT;
    public Transform MAPMIDPOINT;
    public List<Transform> mapPoints;
    public GameObject bigBullet;
    public GameObject smallBullet;
    public GromarStateMachine StateMachine;
    public Player player;
    public Transform ShootingPoint;
    public int maxHealth = 3;
    public int currentHealth;

    public Vector3 GetRandomShootPosition()
    {
        var diff = (MAXSHOOT.position - MINSHOOT.position) * UnityEngine.Random.Range(0f, 1f);
        return MINSHOOT.position+ diff;
    }


    public void FacePlayer()
    {
        if (player == null) { return; }

        Vector3 direction = player.transform.position - transform.position;
        if(direction.x > 0)
        {
            transform.rotation = Quaternion.Euler(0f, 180f, 0f);
        }
        else if (direction.x < 0)
        {
            transform.rotation = Quaternion.Euler(0f, 0f, 0f);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        
        if (other.CompareTag("PlayerBullet")) // Make sure your bullet has tag "Bullet"
        {
            ModifyHealth(-1);
        }
    }

    void Start()
    {
        player = FindObjectOfType<Player>();
        currentHealth = maxHealth;
        if (StateMachine != null)
        {
            StateMachine.Init();
        }
    }
    /// <summary>
    /// methode qui permet au boss de perdre ou gagner de la vie sans depacer sa vie max et sans aller dans le negatif si le boss perd trop de vie
    /// </summary>
    /// <param name="amount">chifre positif ou negatif pour le heal / damage que le boss prend</param>
    public void ModifyHealth(int amount)
    {
        currentHealth += amount;


        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        //OnHealthChanged?.Invoke(currentHealth, maxHealth);


        if (amount < 0)
        {
            Debug.Log($"Le gromar a pris {-amount} degat! Vie = {currentHealth}/{maxHealth}");
        }
        else if (amount > 0)
        {
            Debug.Log($"Le gromar a heal {amount}! Vie = {currentHealth}/{maxHealth}");
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }
    private void Die()
    {
        GameObject.Destroy(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        FacePlayer();
    }
}
