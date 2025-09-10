using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public List<GameObject> vies;
    public int nbVies = 3;
    public GameObject inventaire1;
    public GameObject inventaire2;
    public TextMeshProUGUI horlogeText;
    public float temps = 0f;
    private bool isDemarrer = true;



    void Start()
    {
        inventaire2.SetActive(false);

    }

    void Update()
    {
        if (isDemarrer)
        {
            temps += Time.deltaTime;
            MisAJourHorloge(temps);
        }

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            inventaire1.SetActive(true);
            inventaire2.SetActive(false);
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            inventaire1.SetActive(false);
            inventaire2.SetActive(true);
        }

        //TEST
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
    /// Methode horloge
    /// </summary>
    public void MisAJourHorloge(float tempsIU)
    {
        int millisecondes = Mathf.FloorToInt((tempsIU * 1000) % 1000);
        int secondes = Mathf.FloorToInt(tempsIU % 60);
        int minutes = Mathf.FloorToInt(tempsIU / 60);

        horlogeText.text = string.Format("{0:00}:{1:00}:{2:000}", minutes, secondes, millisecondes);
    }
}
