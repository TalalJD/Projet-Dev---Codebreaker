using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    //placeholder pour le nb de vies
    private int nbVies = 3;

    //liste des coeurs
    private List<GameObject> vies = new List<GameObject>();

    // Images de l'inventaire
    private Image invArmeImg1;
    private Image invArmeImg2;
    private Image invConsImg1;
    private Image invConsImg2;

    //joueur
    private Player player;

    //liste des inventaires
    public List<ScriptableObject> WeaponInventory;
    public List<ScriptableObject> ConsumableInventory;

    //horloge et timer
    private TextMeshProUGUI horlogeText;
    private Image rotationHorloge;

    //rotation de l'horologe et temps
    public float temps = 0f;
    private bool isDemarrer = true;
    public float rotationAngle = 90f;
    public float rotationInterval = 1f;
    public float rotationTimer = 0f;

    void Awake()
    {
        // trouver le canvas dans la scene
        Canvas canvas = FindAnyObjectByType<Canvas>();

        if (canvas == null)
        {
            Debug.LogError("No Canvas found in the scene!");
            return;
        }

        // Trouver les coeurs sur le UI
        vies.Clear();
        for (int i = 1; i <= 3; i++)
        {
            Transform vie = canvas.transform.Find($"Vie{i}");
            if (vie != null)
                vies.Add(vie.gameObject);
            else
                Debug.LogWarning($"Vie{i} not found in Canvas!");
        }

        //Trouver les image d'inventaire dans les parents
        invArmeImg1 = canvas.transform.Find("InventaireArme1/Image")?.GetComponent<Image>();
        invArmeImg2 = canvas.transform.Find("InventaireArme2/Image")?.GetComponent<Image>();
        invConsImg1 = canvas.transform.Find("InventaireCons1/Image")?.GetComponent<Image>();
        invConsImg2 = canvas.transform.Find("InventaireCons2/Image")?.GetComponent<Image>();

        //trouver l'horloge et le timer
        rotationHorloge = canvas.transform.Find("Horloge")?.GetComponent<Image>();
        horlogeText = canvas.transform.Find("Temps")?.GetComponent<TextMeshProUGUI>();

        if (rotationHorloge == null)
            Debug.LogWarning("Horloge image not found!");
        if (horlogeText == null)
            Debug.LogWarning("Temps TextMeshProUGUI not found!");
    }

    void Start()
    {
        StartCoroutine(InitAfterPlayerReady());
    }

    private IEnumerator InitAfterPlayerReady()
    {
        // attendre une frame pour etre sur que le start du player se lance avant
        yield return null;

        player = FindAnyObjectByType<Player>();
        if (player == null)
        {
            Debug.LogError(" Player not found in scene!");
            yield break;
        }

        WeaponInventory = player.WeaponInventory;
        ConsumableInventory = player.ConsumableInventory;

        player.OnWeaponInventoryChanged += setInventoryWeaponIcons;
        player.OnHealthChanged += UpdateHeartsUI;

        // initialiser le ui
        setInventoryWeaponIcons();
        UpdateHeartsUI(player.currentHealth, player.maxHealth);
    }


    void Update()
    {
        if (isDemarrer)
        {
            temps += Time.deltaTime;
            MisAJourHorloge(temps);
        }
        MisAJourRotatitionHorloge();
    }

  
    private void UpdateHeartsUI(int currentHealth, int maxHealth)
    {
        for (int i = 0; i < vies.Count; i++)
            vies[i].SetActive(i < currentHealth);

        nbVies = currentHealth;
    }


    public void setInventoryWeaponIcons()
    {
        if (WeaponInventory.Count > 0 && WeaponInventory[0] is WeaponInfo weaponInfo1)
            invArmeImg1.sprite = weaponInfo1.weaponSprite;

        if (WeaponInventory.Count > 1 && WeaponInventory[1] is WeaponInfo weaponInfo2)
            invArmeImg2.sprite = weaponInfo2.weaponSprite;
    }

    public void setInventoryConsumableIcons()
    {
        if (ConsumableInventory.Count > 0 && ConsumableInventory[0] is ItemInfo itemInfo1)
            invConsImg1.sprite = itemInfo1.ItemSprite;

        if (ConsumableInventory.Count > 1 && ConsumableInventory[1] is ItemInfo itemInfo2)
            invConsImg2.sprite = itemInfo2.ItemSprite;
    }
    public void MisAJourHorloge(float tempsIU)
    {
        if (horlogeText == null) return;

        int millisecondes = Mathf.FloorToInt((tempsIU * 1000) % 1000);
        int secondes = Mathf.FloorToInt(tempsIU % 60);
        int minutes = Mathf.FloorToInt(tempsIU / 60);

        horlogeText.text = string.Format("{0:00}:{1:00}:{2:000}", minutes, secondes, millisecondes);
    }

   
    public void MisAJourRotatitionHorloge()
    {
        if (rotationHorloge == null) return;

        rotationTimer += Time.deltaTime;
        if (rotationTimer >= rotationInterval)
        {
            rotationHorloge.transform.Rotate(0f, 0f, -rotationAngle);
            rotationTimer = 0f;
        }
    }
}
