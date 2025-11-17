using CodeBreaker;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gromar : MonoBehaviour
{
    public GameObject laserPrefab;


    public Transform MAPMIDPOINT;
    public Transform SPAWNPOINT;
    public List<Transform> mapPoints;


    public GromarStateMachine StateMachine;
    public Player player;
    public Transform LobeSP;
    public Transform LaserSP;
    public Transform HomingSP;
    public Transform ConeSP;
    public Animator animator;


    public int maxHealth = 3;
    public int currentHealth;

    public float explosionRadius = 2.5f;
    [NonSerialized] public bool showExplosionGizmo = false;



    private void OnDrawGizmos()
    {
        // On ne dessine que si on est en Play + le flag est actif
        if (!Application.isPlaying) return;
        if (!showExplosionGizmo) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
    public void FacePlayer()
    {
        if (player == null) { return; }

        Vector3 direction = player.transform.position - transform.position;
        if (direction.x > 0)
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
        player = FindAnyObjectByType<Player>();
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

    public void CallMissilAttack()
    {
        StartCoroutine(StateMachine.Get<GS_HomingMissile>().ShootHomingMissiles());
    }

    public void CallLobe()
    {
        StartCoroutine(StateMachine.Get<GS_Lobe>().ShootAtPlayerContinuously());

    }

    public void CallWarp()
    {
        StartCoroutine(StateMachine.Get<GS_Warp>().DoWarp());
    }

    public void CallCone()
    {
        StartCoroutine(StateMachine.Get<GS_Cone>().ShootCone());
    }

    public void CallLaser()
    {
        StartCoroutine(StateMachine.Get<GS_LaserAttack>().LaserRoutine());
    }

    public void CallExplosion()
    {
        StartCoroutine(StateMachine.Get<GS_Explosion>().DoExplosionDamage(explosionRadius, 1));
    }

    // Update is called once per frame
    void Update()
    {
        FacePlayer();
    }

}
