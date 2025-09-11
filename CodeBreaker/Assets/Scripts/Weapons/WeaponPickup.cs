using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class WeaponPickUp : MonoBehaviour
{
    public WeaponInfo weaponInfo; //L'arme qui sera recupere

    public string playerTag = "Player";
    private bool _playerInRange = false;
    private Player _player;

    //Le joueur peut recuperer l'arme lorsqu'il est dans la zone du OnTrigger de l'arme
    private void OnTriggerEnter2D(Collider2D other)
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

}
