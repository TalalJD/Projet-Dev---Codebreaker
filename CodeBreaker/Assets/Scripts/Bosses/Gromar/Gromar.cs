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


    public int maxHealth = 50;
    public int currentHealth;

    public float explosionRadius = 2.5f;
    [NonSerialized] public bool showExplosionGizmo = false;
    public bool forcedExplosion = false;



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
        // Ne lance l’attaque que si le state courant EST GS_HomingMissile
        if (!StateMachine.IsCurrentState<GS_HomingMissile>())
            return;

        var st = StateMachine.Get<GS_HomingMissile>();
        if (st == null) return;
        if (st.LogicStarted) return;

        st.LogicStarted = true;
        StartCoroutine(st.ShootHomingMissiles());
    }

    public void CallLobe()
    {
        if (!StateMachine.IsCurrentState<GS_Lobe>())
            return;

        var st = StateMachine.Get<GS_Lobe>();
        if (st == null) return;
        if (st.LogicStarted) return;

        st.LogicStarted = true;
        StartCoroutine(st.ShootAtPlayerContinuously());
    }

    public void CallWarp()
    {
        if (!StateMachine.IsCurrentState<GS_Warp>())
            return;

        var st = StateMachine.Get<GS_Warp>();
        if (st == null) return;
        if (st.LogicStarted) return;

        st.LogicStarted = true;
        StartCoroutine(st.DoWarp());
    }

    public void CallCone()
    {
        if (!StateMachine.IsCurrentState<GS_Cone>())
            return;

        var st = StateMachine.Get<GS_Cone>();
        if (st == null) return;
        if (st.LogicStarted) return;

        st.LogicStarted = true;
        StartCoroutine(st.ShootCone());
    }

    public void CallLaser()
    {
        if (!StateMachine.IsCurrentState<GS_LaserAttack>())
            return;

        var st = StateMachine.Get<GS_LaserAttack>();
        if (st == null) return;
        if (st.LogicStarted) return;

        st.LogicStarted = true;
        StartCoroutine(st.LaserRoutine());
    }

    public void CallExplosion()
    {
        if (!StateMachine.IsCurrentState<GS_Explosion>())
            return;

        var st = StateMachine.Get<GS_Explosion>();
        if (st == null) return;
        if (st.LogicStarted) return;

        st.LogicStarted = true;
        StartCoroutine(st.DoExplosionDamage(explosionRadius, 1));
    }


    // Update is called once per frame
    void Update()
    {
        FacePlayer();
        CheckExplosionProximity();
    }
    private void CheckExplosionProximity()
    {
        if (player == null || StateMachine == null)
            return;

        // Distance 2D entre Gromar et le joueur
        float dist = Vector2.Distance(player.transform.position, transform.position);

        // Si le joueur est dans le rayon de l'explosion, on force l'explosion
        if (dist <= explosionRadius)
        {
            forcedExplosion = true;
            StateMachine.ForceExplosion();
        }
    }

}
