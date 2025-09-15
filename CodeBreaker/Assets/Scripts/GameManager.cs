using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] private List<GameObject> vies;
    private int nbVies = 3;

    [SerializeField] private GameObject inventaireArme1;
    [SerializeField] private GameObject inventaireArme2;
    [SerializeField] private GameObject inventaireConsomable1;
    [SerializeField] private GameObject inventaireConsomable2;

    [SerializeField] private Image invArmeImg1;
    [SerializeField] private Image invArmeImg2;
    [SerializeField] private Image invConsImg1;
    [SerializeField] private Image invConsImg2;


    private Player player;

    public List<ScriptableObject> WeaponInventory;

    public TextMeshProUGUI horlogeText;
    public float temps = 0f;
    private bool isDemarrer = true;
    public Image rotationHorloge;
    public float rotationAngle = 90f;
    public float rotationInterval = 1f;
    public float rotationTimer = 0f;

    

    void Start()
    {
        

         player = FindObjectOfType<Player>();
         WeaponInventory = player.WeaponInventory;
        player.OnWeaponInventoryChanged += setInventoryWeaponIcons;
        player.OnHealthChanged += UpdateHeartsUI;
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

       
        if (Input.GetKeyDown(KeyCode.Alpha0))
        {
            AjouteVie();
        }

        if (Input.GetKeyDown(KeyCode.Alpha9))
        {
            EnleveVie();
        }
    }
    /// <summary>
    /// Methode qui update les coeurs sur le ui par rapport a la vie du joueur
    /// </summary>
    /// <param name="currentHealth">vie du joueur en ce moment</param>
    /// <param name="maxHealth">Vie max du joueur</param>
    private void UpdateHeartsUI(int currentHealth, int maxHealth)
    {
       
        for (int i = 0; i < vies.Count; i++)
        {
            if (i < currentHealth)
            {
                vies[i].SetActive(true); 
            }
            else
            {
                vies[i].SetActive(false); 
            }
        }

        nbVies = currentHealth;
    }


    /// <summary>
    /// Methode qui update les icones des armes dans l'inventaire du joueur sur le UI
    /// </summary>
    public void setInventoryWeaponIcons()
    {
        if (WeaponInventory.Count > 0 && WeaponInventory[0] is WeaponInfo weaponInfo1)
        {
            invArmeImg1.sprite = weaponInfo1.weaponSprite;
        }

        if (WeaponInventory.Count > 1 && WeaponInventory[1] is WeaponInfo weaponInfo2)
        {
            invArmeImg2.sprite = weaponInfo2.weaponSprite;
        }
    }



    /// <summary>
    /// Methode pour ajout une vie
    /// </summary>
    public void AjouteVie()
    {
        if (nbVies >= vies.Count) return;

        vies[nbVies].SetActive(true);
        nbVies++;
    }

    /// <summary>
    /// Methode pour enlever une vie
    /// </summary>
    public void EnleveVie()
    {
        if (nbVies <= 0) return;

        nbVies--;
        vies[nbVies].SetActive(false);
    }

    /// <summary>
    /// Methode horloge/minuterie
    /// </summary>
    public void MisAJourHorloge(float tempsIU)
    {
        int millisecondes = Mathf.FloorToInt((tempsIU * 1000) % 1000);
        int secondes = Mathf.FloorToInt(tempsIU % 60);
        int minutes = Mathf.FloorToInt(tempsIU / 60);

        horlogeText.text = string.Format("{0:00}:{1:00}:{2:000}", minutes, secondes, millisecondes);
    }

    /// <summary>
    /// Methode tourne image
    /// </summary>
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
