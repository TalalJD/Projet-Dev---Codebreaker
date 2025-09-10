using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public List<GameObject> vies;
    public int nbVies = 3;
    public GameObject inventaire1;
    public GameObject inventaire2;

    private void Start()
    {
        inventaire2.SetActive(false);
    }

    void Update()
    {

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
}
