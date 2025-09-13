using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class ItemPickUp : MonoBehaviour
{
    public ScriptableObject objectRecup; //L'arme qui sera recupere
    public KeyCode pickupKey = KeyCode.E;

    public string playerTag = "Player";
    private bool _playerInRange = false;
    private Player _player;

    void Update()
    {
        if (_playerInRange && Input.GetKeyDown(pickupKey)) {
            Pickup();
        }
    }

    //Le joueur peut recuperer l'arme lorsqu'il est dans la zone du OnTrigger de l'arme
    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag(playerTag) && other.TryGetComponent(out Player player))
        {
            _playerInRange = true;
            _player = player;
        }
    }

    //Le joueur ne peut plus recuperer l'arme lorsqu'il quitte la zone du OnTrigger
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag(playerTag))
        {
            _playerInRange = false;
            _player = null;
        }
    }

    //Ajoute l'arme a l'inventaire du joueur en appelant la methode de la classe joueur
    private void Pickup()
    {
        if (_player != null && objectRecup != null)
        {
            _player.AddItemToInventory(objectRecup);
            Destroy(gameObject);
        }
    }

}
