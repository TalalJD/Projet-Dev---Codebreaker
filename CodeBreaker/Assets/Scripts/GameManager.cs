using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public List<GameObject> vies;
    public int nbVies = 3;

    void Update()
    {
        //TEST
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            AjouteVie();
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
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
