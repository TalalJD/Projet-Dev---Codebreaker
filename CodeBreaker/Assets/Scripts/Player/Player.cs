using CodeBreaker;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class Player : MonoBehaviour
{

    public SpriteRenderer spriteRenderer;

    public Animator animator;



    //attributs pour la logique de deplacement
    public float GroundRayLenght; //longueur du ray pour dectecter le ground check
    public Rigidbody2D Rb;
    public PhysicsInfo PhysicsInfo;
    public float GroundSpeed;
    public int Direction = 1; //-1 = gauche , 1 = droite
    public PlayerStateMachine StateMachine;
    public LayerMask LayerMask;

    //attributs pour la logique d'inventarie et d'items

    public Weapon SelectedWeapon; //l'arme selectionnee
    public WeaponInfo SelectedWeaponInfo;//info de l'arme selectionne
    public List<ScriptableObject> WeaponInventory; //liste des armes scriptable object dans l'inventaire
    public List<ScriptableObject> ConsumableInventory; //liste des consomable scriptable object dans l'inventaire
    public Transform WeaponHolder;
    private int inventoryIndex = -1; //index de l'item selectionee dans l'inventaire


    public event Action OnWeaponInventoryChanged;
    //public event Action OnConsInventoryChanged;
    public event Action<int, int> OnHealthChanged;
   


    public int currentHealth;
    public int maxHealth = 3;

    public float XSpeed //vitesse horizontale du joueur
    {
        get => Rb.velocity.x;

        set => Rb.velocity = new Vector2(value, Rb.velocity.y);
    }
    public float YSpeed //vitesse verticale du joueur
    {
        get => Rb.velocity.y;
        set => Rb.velocity = new Vector2(Rb.velocity.x, value);
    }


    public void UpdateAnimator()
    {
        if (animator != null) {
            spriteRenderer.flipX = Direction == -1;
            animator.SetFloat("Xspeed", Mathf.Abs(XSpeed));
            animator.SetFloat("Yspeed", YSpeed);
            animator.SetInteger("StateNumber", StateMachine.CurrentState.StateNumber);
        }
        
    }

    /// <summary>
    /// Regarde si le joueur touche le sol en envoyant un raycast
    /// </summary>
    /// <returns></returns>
    public bool CheckOnGround()
    {
        var GroundRay = Physics2D.Raycast(transform.position, Vector2.down, GroundRayLenght, LayerMask);
        Debug.DrawRay(transform.position, Vector2.down* GroundRayLenght);
        if (GroundRay)
        {
            Rb.position = GroundRay.point;
            return true;
        }
        return false;
    }




    void Start()
    {
        currentHealth = maxHealth;
        if (StateMachine != null)
        {
            StateMachine.Init();
        }
            


    }
    void Update()
    {

        UpdateAnimator();

        if (Input.GetKeyDown(KeyCode.J))
        {
            Debug.Log("imshooting");
            SelectedWeapon?.Attack();
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            CycleWeaponInventory();
        }
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            ModifyHealth(-1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            ModifyHealth(1);
        }
    }
    /// <summary>
    /// Methode qui permet de cycler dans l'inventaire du joueur
    /// </summary>
    public void CycleWeaponInventory()
    {
        if (WeaponInventory.Count > 0) 
        {
            inventoryIndex++;
            if (inventoryIndex >= WeaponInventory.Count)
            {
                inventoryIndex = 0; // retour au d�but
            }

            EquipWeapon(inventoryIndex);
        }
    }

    /// <summary>
    /// Equipe une arme a partir de l'inventaire en utilisant l'index donn�.
    /// D�truit l'arme actuellement �quip�e si n�cessaire, instancie le prefab associ�
    /// au ScriptableObject <see cref="WeaponInfo"/>, puis assigne ses donn�es
    /// (logique, stats, sprite, etc.) au composant <see cref="Weapon"/>.
    /// </summary>
    /// <param name="index">Index de l'arme dans la liste Inventory</param>
    private void EquipWeapon(int index)
    {
        // d�truire l�arme pr�c�dente si elle existe
        if (SelectedWeapon != null)
        {
            Destroy(SelectedWeapon.gameObject);
        }

        var weaponInfo = WeaponInventory[index] as WeaponInfo;
        if (weaponInfo != null)
        {
            GameObject weaponObj = Instantiate(weaponInfo.weaponPrefab, WeaponHolder);
            var weapon = weaponObj.GetComponent<Weapon>();
            weapon.AssignWeapon(weaponInfo);
            SelectedWeapon = weapon;
            SelectedWeaponInfo = weaponInfo;
        }
        else
        {
            Debug.LogWarning($"Item at index {index} is not a WeaponInfo!");
        }
    }

    /// <summary>
    /// Methode qui regarde si l'item est une arme ou un objet et la rajoute dans la liste d'inventaire specifique du joueur
    /// </summary>
    /// <param name="item">scriptable object recupere</param>
   public void AddItemToInventory(ScriptableObject item)
    {
        if(item.GetType() == typeof(WeaponInfo) && WeaponInventory != null)
        {
            if (WeaponInventory.Count < 2) 
            {
                WeaponInventory.Add(item);
            }
            else
            {
                if (SelectedWeapon != null) 
                {
                   int selectedIndex = WeaponInventory.IndexOf(SelectedWeaponInfo);
                    if (selectedIndex != -1)
                    {
                        WeaponInventory[selectedIndex] = item; // replace in place
                        EquipWeapon(selectedIndex); // equip the new one immediately
                    }
                }
                else
                {
                    // If no weapon selected, just replace the first slot (or any default slot)
                    WeaponInventory[0] = item;
                    EquipWeapon(0);
                }

            }
            OnWeaponInventoryChanged?.Invoke();
        }

    }

    /// <summary>
    /// methode qui permet au joueur de perdre ou gagner de la vie sans depacer sa vie max et sans aller dans le negatif si le joueur perd trop de vie
    /// </summary>
    /// <param name="amount">chifre positif ou negatif pour le heal / damage que le joueur prend</param>
    public void ModifyHealth(int amount)
    {
        currentHealth += amount;

       
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);

        OnHealthChanged?.Invoke(currentHealth, maxHealth);


        if (amount < 0)
        {
            Debug.Log($"Le joueur a pris {-amount} degat! Vie = {currentHealth}/{maxHealth}");
        }
        else if (amount > 0)
        {
            Debug.Log($"Le joueur a heal {amount}! Vie = {currentHealth}/{maxHealth}");
        }

        if (currentHealth <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
       
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("EnnemyBullet"))
        {
            ModifyHealth(-1);
            Destroy(other.gameObject); // player decides when bullet is destroyed
        }

        if (other.CompareTag("EnnemyHitbox"))
        {
            var ennemyHitbox = other.GetComponent<EnnemyAttackHitbox>();
            ModifyHealth(-ennemyHitbox._ennemyInfo.attackDamage);
        }
    }
}
