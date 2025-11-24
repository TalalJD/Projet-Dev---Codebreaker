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
    public float GroundRayLenght; //longueur du ray pour dectetecter le ground check
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
    public Consumable SelectedConsumable;
    public ConsumableInfo SelectedConsumableInfo;
    public Transform WeaponHolder;
    private Vector3 weaponHolderInitialLocalPos; // store initial local pos to flip on direction change
    private Vector3 weaponHolderInitialLocalScale; // store initial local scale to flip the gun sprite
    private int inventoryIndex = -1; //index de l'item selectionee dans l'inventaire (legacy)
    private int ConsumableInventoryIndex = -1;

    // Combined-cycling state
    private int heldCombinedIndex = -1; // -1 = nothing held

    public event Action OnWeaponInventoryChanged;
    public event Action OnConsInventoryChanged;
    // Event to notify UI of held item change: pass currently held ScriptableObject or null
    public event Action<ScriptableObject> OnHeldItemChanged;
    public event Action<int, int> OnHealthChanged;



    public int currentHealth;
    public int maxHealth = 3;


    public bool canTakeDmg;
    public float blockCooldown = 4f;   // la duration 
    public float blockTimer = 0f;      // timer

    // New flag to indicate blocking state (used to prevent weapon equip/instantiate)
    public bool IsBlocking = false;


    public float WallRayLength = 0.6f; // Longueur du rayon pour détecter le mur

    public float XSpeed //vitesse horizontale du joueur
    {
        get => Rb.linearVelocity.x;

        set => Rb.linearVelocity = new Vector2(value, Rb.linearVelocity.y);
    }
    public float YSpeed //vitesse verticale du joueur
    {
        get => Rb.linearVelocity.y;
        set => Rb.linearVelocity = new Vector2(Rb.linearVelocity.x, value);
    }


    public void UpdateAnimator()
    {
        if (animator != null)
        {
            spriteRenderer.flipX = Direction == -1;
            animator.SetFloat("Xspeed", Mathf.Abs(XSpeed));
            animator.SetFloat("Yspeed", YSpeed);
            animator.SetInteger("StateNumber", StateMachine.CurrentState.StateNumber);
        }

        // Keep WeaponHolder on the correct side based on Direction.
        // Use the initial local position X as magnitude and apply sign by Direction.
        if (WeaponHolder != null)
        {
            // Ensure we captured initial local pos/scale in Start()
            Vector3 initialPos = weaponHolderInitialLocalPos;
            Vector3 initialScale = weaponHolderInitialLocalScale;

            int dirSign = (Direction == 0) ? 1 : Math.Sign(Direction);

            float newLocalX = Mathf.Abs(initialPos.x) * dirSign;
            WeaponHolder.localPosition = new Vector3(newLocalX, initialPos.y, initialPos.z);

            // Flip the WeaponHolder's localScale.x so the weapon sprite/axes are mirrored when facing left
            float newScaleX = Mathf.Abs(initialScale.x) * dirSign;
            WeaponHolder.localScale = new Vector3(newScaleX, initialScale.y, initialScale.z);
        }
    }

    /// <summary>
    /// Regarde si le joueur touche le sol en envoyant un raycast
    /// </summary>
    /// <returns></returns>
    public bool CheckOnGround()
    {
        var GroundRay = Physics2D.Raycast(transform.position, Vector2.down, GroundRayLenght, LayerMask);
        Debug.DrawRay(transform.position, Vector2.down * GroundRayLenght);
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
        canTakeDmg = true;
        if (StateMachine != null)
        {
            StateMachine.Init();
        }

        // record WeaponHolder initial local position & scale so we can flip it later
        if (WeaponHolder != null)
        {
            weaponHolderInitialLocalPos = WeaponHolder.localPosition;
            weaponHolderInitialLocalScale = WeaponHolder.localScale;
        }

        // ensure inventories are not null
        if (WeaponInventory == null) WeaponInventory = new List<ScriptableObject>();
        if (ConsumableInventory == null) ConsumableInventory = new List<ScriptableObject>();

        // if something is already equipped in inspector, do not override
        // otherwise, try to initialize heldCombinedIndex to first item if any
        if (heldCombinedIndex == -1)
        {
            if (WeaponInventory.Count + ConsumableInventory.Count > 0)
            {
                heldCombinedIndex = 0;
                EquipHeldFromCombinedIndex(heldCombinedIndex);
            }
        }
    }
    void Update()
    {


        UpdateAnimator();



        if (Input.GetKeyDown(KeyCode.J))
        {
            // unified use key: use currently held item (weapon preferred over consumable)
            if (IsBlocking) return; // don't use while blocking

            if (SelectedWeapon != null)
            {
                SelectedWeapon.Attack();
            }
            else if (SelectedConsumable != null)
            {
                SelectedConsumable.UseConsumable();
                // if consumable should be consumed/removed, ensure the Consumable implementation handles destruction
            }
        }
        if (Input.GetKeyDown(KeyCode.O))
        {
            Debug.Log("Using Consumable");
            SelectedConsumable?.UseConsumable();
        }
        if (Input.GetKeyDown(KeyCode.K))
        {
            CycleConsumableInventory();
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            // unified cycle between consumables and weapons
            CycleHeldItem();
        }
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            ModifyHealth(-1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            ModifyHealth(1);
        }

        // Commencer le timer
        if (blockTimer > 0f)
        {
            blockTimer -= Time.deltaTime;
            if (blockTimer < 0f) blockTimer = 0f;
        }

        //if (blockTimer > 0)
        //{
        //    Debug.Log($"Block cooldown: {blockTimer:F1} seconds left");
        //}
        //else
        //{
        //    Debug.Log("Block READY");
        //}
    }

    /// <summary>
    /// Returns combined inventory as list of tuples (item, isWeapon)
    /// weapon entries come first (in order), then consumables.
    /// </summary>
    private List<(ScriptableObject item, bool isWeapon)> GetCombinedInventory()
    {
        var combined = new List<(ScriptableObject, bool)>();
        if (WeaponInventory != null)
        {
            foreach (var w in WeaponInventory) combined.Add((w, true));
        }
        if (ConsumableInventory != null)
        {
            foreach (var c in ConsumableInventory) combined.Add((c, false));
        }
        return combined;
    }

    /// <summary>
    /// Cycle through combined inventory and equip next item.
    /// </summary>
    public void CycleHeldItem()
    {
        if (IsBlocking) return;

        var combined = GetCombinedInventory();
        if (combined.Count == 0)
        {
            // clear any held item
            ClearHeldItem();
            return;
        }

        heldCombinedIndex = (heldCombinedIndex + 1) % combined.Count;
        EquipHeldFromCombinedIndex(heldCombinedIndex);
    }

    /// <summary>
    /// Equip the item referenced by combined index.
    /// </summary>
    private void EquipHeldFromCombinedIndex(int combinedIndex)
    {
        var combined = GetCombinedInventory();
        if (combinedIndex < 0 || combinedIndex >= combined.Count)
        {
            ClearHeldItem();
            return;
        }

        var entry = combined[combinedIndex];
        if (entry.isWeapon)
        {
            // destroy consumable object if present
            if (SelectedConsumable != null)
            {
                Destroy(SelectedConsumable.gameObject);
                SelectedConsumable = null;
                SelectedConsumableInfo = null;
                OnConsInventoryChanged?.Invoke();
            }

            // Find index in weapon list and equip using existing method
            int wIndex = WeaponInventory.IndexOf(entry.item);
            if (wIndex >= 0)
            {
                EquipWeapon(wIndex);
            }
            else
            {
                // fallback: instantiate directly
                var wi = entry.item as WeaponInfo;
                if (wi != null)
                {
                    if (SelectedWeapon != null) Destroy(SelectedWeapon.gameObject);
                    GameObject weaponObj = Instantiate(wi.weaponPrefab, WeaponHolder);
                    var weapon = weaponObj.GetComponent<Weapon>();
                    weapon.AssignWeapon(wi);
                    SelectedWeapon = weapon;
                    SelectedWeaponInfo = wi;
                    OnWeaponInventoryChanged?.Invoke();
                }
            }

            // notify held change
            OnHeldItemChanged?.Invoke(entry.item);
        }
        else
        {
            // equip consumable: destroy weapon object if present
            if (SelectedWeapon != null)
            {
                Destroy(SelectedWeapon.gameObject);
                SelectedWeapon = null;
                SelectedWeaponInfo = null;
                OnWeaponInventoryChanged?.Invoke();
            }

            int cIndex = ConsumableInventory.IndexOf(entry.item);
            if (cIndex >= 0)
            {
                EquipConsumable(cIndex);
            }
            else
            {
                var ci = entry.item as ConsumableInfo;
                if (ci != null)
                {
                    if (SelectedConsumable != null) Destroy(SelectedConsumable.gameObject);
                    GameObject consObj = Instantiate(ci.consumablePrefab, WeaponHolder);
                    var cons = consObj.GetComponent<Consumable>();
                    cons.AssignConsumable(ci);
                    SelectedConsumable = cons;
                    SelectedConsumableInfo = ci;
                    OnConsInventoryChanged?.Invoke();
                }
            }

            OnHeldItemChanged?.Invoke(entry.item);
        }
    }

    /// <summary>
    /// Clears any held object in WeaponHolder and resets selection.
    /// </summary>
    public void ClearHeldItem()
    {
        if (SelectedWeapon != null)
        {
            Destroy(SelectedWeapon.gameObject);
            SelectedWeapon = null;
            SelectedWeaponInfo = null;
            OnWeaponInventoryChanged?.Invoke();
        }
        if (SelectedConsumable != null)
        {
            Destroy(SelectedConsumable.gameObject);
            SelectedConsumable = null;
            SelectedConsumableInfo = null;
            OnConsInventoryChanged?.Invoke();
        }
        heldCombinedIndex = -1;
        OnHeldItemChanged?.Invoke(null);
    }

    /// <summary>
    /// Methode qui permet de cycler dans l'inventaire du joueur
    /// (legacy: still available if you need separate cycling)
    /// </summary>
    public void CycleWeaponInventory()
    {
        // Prevent weapon cycling/instantiation while blocking
        if (IsBlocking) return;

        if (WeaponInventory.Count > 0)
        {
            inventoryIndex++;
            if (inventoryIndex >= WeaponInventory.Count)
            {
                inventoryIndex = 0; // retour au d�but
            }

            EquipWeapon(inventoryIndex);
            // keep heldCombinedIndex in sync (point to first matching combined entry for this weapon)
            var combined = GetCombinedInventory();
            for (int i = 0; i < combined.Count; i++)
                if (combined[i].item == WeaponInventory[inventoryIndex]) { heldCombinedIndex = i; break; }
        }
    }

    public void CycleConsumableInventory()
    {
        if (ConsumableInventory.Count > 0)
        {
            ConsumableInventoryIndex++;
            if (ConsumableInventoryIndex >= ConsumableInventory.Count)
            {
                ConsumableInventoryIndex = 0; // retour au d�but
            }
            EquipConsumable(ConsumableInventoryIndex);

            // keep heldCombinedIndex in sync
            var combined = GetCombinedInventory();
            for (int i = 0; i < combined.Count; i++)
                if (combined[i].item == ConsumableInventory[ConsumableInventoryIndex]) { heldCombinedIndex = i; break; }
        }
    }

    /// <summary>
    /// Equipe une arme a partir de l'inventaire en utilisant l'index donn�
    /// (unchanged)
    /// </summary>
    private void EquipWeapon(int index)
    {
        // Prevent equipping while blocking (safety check)
        if (IsBlocking) return;

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
            OnWeaponInventoryChanged?.Invoke();
            OnHeldItemChanged?.Invoke(weaponInfo);
        }
        else
        {
            Debug.LogWarning($"Item at index {index} is not a WeaponInfo!");
        }
    }

    private void EquipConsumable(int index)
    {
        if (SelectedConsumable != null)
        {
            Destroy(SelectedConsumable.gameObject);
        }

        var consumableInfo = ConsumableInventory[index] as ConsumableInfo;
        if (consumableInfo != null)
        {
            GameObject consObj = Instantiate(consumableInfo.consumablePrefab, WeaponHolder);
            var cons = consObj.GetComponent<Consumable>();
            cons.AssignConsumable(consumableInfo);
            SelectedConsumable = cons;
            SelectedConsumableInfo = consumableInfo;
            OnConsInventoryChanged?.Invoke();
            OnHeldItemChanged?.Invoke(consumableInfo);
        }
        else
        {
            Debug.LogWarning($"Item at index {index} is not a ConsumableInfo!");
        }
    }

    /// <summary>
    /// Methode qui regarde si l'item est une arme ou un objet et la rajoute dans la liste d'inventaire specifique du joueur
    /// </summary>
    /// <param name="item">scriptable object recupere</param>
    public void AddItemToInventory(ScriptableObject item)
    {
        if (item.GetType() == typeof(WeaponInfo) && WeaponInventory != null)
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

        if (item.GetType() == typeof(ConsumableInfo) && ConsumableInventory != null)
        {
            if (ConsumableInventory.Count < 2)
            {
                ConsumableInventory.Add(item);
            }
            else
            {
                if (SelectedConsumable != null)
                {
                    int selectedIndex = ConsumableInventory.IndexOf(SelectedConsumableInfo);
                    if (selectedIndex != -1)
                    {
                        ConsumableInventory[selectedIndex] = item;
                        EquipConsumable(selectedIndex);
                    }
                }
                else
                {
                    ConsumableInventory[0] = item;
                    EquipConsumable(0);
                }
            }
            OnConsInventoryChanged?.Invoke();
        }

    }

    /// <summary>
    /// methode qui permet au joueur de perdre ou gagner de la vie sans depacer sa vie max et sans aller dans le negatif si le joueur perd trop de vie
    /// </summary>
    /// <param name="amount">chifre positif ou negatif pour le heal / damage que le joueur prend</param>
    public void ModifyHealth(int amount)
    {

        if (canTakeDmg && amount < 0)
        {

            if (amount < 0)
            {
                currentHealth += amount;
                currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
                OnHealthChanged?.Invoke(currentHealth, maxHealth);
                Debug.Log($"Le joueur a pris {-amount} degat! Vie = {currentHealth}/{maxHealth}");
            }
        }
        else if (amount > 0)
        {
            currentHealth += amount;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
            OnHealthChanged?.Invoke(currentHealth, maxHealth);
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
        if (other.GetComponent<Projectile>() != null)
        {
            ModifyHealth(-1);
            if (other.GetComponent<HomingMissile>() != null || other.GetComponent<ParabolicMissile>() != null)
            {
                Destroy(other.gameObject);
            }

        }

        //if (other.CompareTag("EnnemyBullet"))
        //{
        //    health--;
        //    if (health <= 0)
        //    {
        //        Die();
        //    }
        //    Destroy(other.gameObject);
        //}
    }

    /// <summary>
    /// Vérifie s'il y a un mur à droite
    /// </summary>
    public bool CheckRightWall()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.right, WallRayLength, LayerMask);
        Debug.DrawRay(transform.position, Vector2.right * WallRayLength, Color.red); // Pour voir le rayon dans la scène
        return hit.collider != null;
    }

    /// <summary>
    /// Vérifie s'il y a un mur à gauche
    /// </summary>
    public bool CheckLeftWall()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.left, WallRayLength, LayerMask);
        Debug.DrawRay(transform.position, Vector2.left * WallRayLength, Color.red);
        return hit.collider != null;
    }

    /// <summary>
    /// Vérifie s'il y a un mur d'un côté ou de l'autre (utilisé pour rester dans l'état Wall)
    /// </summary>
    public bool CheckWall()
    {
        return CheckRightWall() || CheckLeftWall();
    }


}
