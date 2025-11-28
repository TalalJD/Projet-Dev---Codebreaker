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


    //joueur
    private Player player;




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

       
       

        
        player.OnHealthChanged += UpdateHeartsUI;

      
        UpdateHeartsUI(player.currentHealth, player.maxHealth);
    }


    void Update()
    {
    
    }

  
    private void UpdateHeartsUI(int currentHealth, int maxHealth)
    {
        for (int i = 0; i < vies.Count; i++)
            vies[i].SetActive(i < currentHealth);

        nbVies = currentHealth;
    }


    
   
}
