using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthBuff : MonoBehaviour
{
    private Player _player;
    [SerializeField] private BoxCollider2D _boxCollider;
    void Start()
    {

        _player = FindAnyObjectByType<Player>();
    }
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
           
            if (_player != null)
            {
                Debug.Log($"[Avant Buff] currentHealth: {_player.currentHealth}");
                _player.ModifyHealth(1);
                Debug.Log($"[Après Buff] currentHealth: {_player.currentHealth}");
                Destroy(gameObject);

            }
           

        }
    }
    
}
