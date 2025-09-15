using CodeBreaker;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    //attributs pour la logique de deplacement

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
    public event Action OnConsInventoryChanged;
   



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

    /// <summary>
    /// Regarde si le joueur touche le sol en envoyant un raycast
    /// </summary>
    /// <returns></returns>
    public bool CheckOnGround()
    {
        var GroundRay = Physics2D.Raycast(transform.position, Vector2.down, 0.25f, LayerMask);
        Debug.DrawRay(transform.position, Vector2.down*.25f);
        if (GroundRay)
        {
            Rb.position = GroundRay.point;
            return true;
        }
        return false;
    }

    void Start()
    {
        if (StateMachine != null)
        {
            StateMachine.Init();
        }
            


    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.J))
        {
            Debug.Log("imshooting");
            SelectedWeapon?.Attack();
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            CycleWeaponInventory();
        }
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            ModifyHealth(-1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
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

        {
            Inventory.Add(item);
        }
    }
}
