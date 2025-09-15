using CodeBreaker;
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
    public int currentHealth = 3;
    

    //attributs pour la logique d'inventarie et d'items

    public Weapon SelectedWeapon; //l'arme selectionnee
    public List<ScriptableObject> Inventory; //liste des scriptable object dans l'inventaire
    public Transform WeaponHolder;
    private int inventoryIndex = -1; //index de l'item selectionee dans l'inventaire


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
            CycleInventory();
        }
    }
    /// <summary>
    /// Methode qui permet de cycler dans l'inventaire du joueur
    /// </summary>
    public void CycleInventory()
    {
        inventoryIndex++;
        if (inventoryIndex >= Inventory.Count)
        {
            inventoryIndex = 0; // retour au d�but
        }

        EquipWeapon(inventoryIndex);
    }

    /// <summary>
    /// �quipe une arme � partir de l'inventaire en utilisant l'index donn�.
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

        var weaponInfo = Inventory[index] as WeaponInfo;
        if (weaponInfo != null)
        {
            GameObject weaponObj = Instantiate(weaponInfo.weaponPrefab, WeaponHolder);
            var weapon = weaponObj.GetComponent<Weapon>();
            weapon.AssignWeapon(weaponInfo);
            SelectedWeapon = weapon;
        }
        else
        {
            Debug.LogWarning($"Item at index {index} is not a WeaponInfo!");
        }
    }

   public void AddItemToInventory(ScriptableObject item)
    {
        if (Inventory != null)
        {
            Inventory.Add(item);
        }
    }
}
