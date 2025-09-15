using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HealthBuff : MonoBehaviour
{
    private Player _player;
    private PhysicsInfo _physicsInfo;
    public int maxHealth = 3;
    [SerializeField] private int _healthIncrease = 1;
    [SerializeField] private BoxCollider2D _boxCollider;
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private GameManager _gameManager;

    void Start()
    {
  
        _player = FindObjectOfType<Player>();

        if (_player != null)
        {
            _player.currentHealth = 1;
            _gameManager = FindObjectOfType<GameManager>();
        }
        
        heartsVisuelUI();
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Debug.Log($"[Avant Buff] currentHealth: {_player.currentHealth}");

            if (_player != null && _gameManager != null)
            {
                // ne pas dépasser la valeur maxHealth de 3
                _player.currentHealth = Mathf.Min(_player.currentHealth + _healthIncrease, maxHealth);

                heartsVisuelUI();
            }
            Debug.Log($"[Après Buff] currentHealth: {_player.currentHealth}");

            // enlève de pickup visuel
            _boxCollider.enabled = false;
            if (_spriteRenderer != null)
            {
                _spriteRenderer.enabled = false;
                Destroy(gameObject);
            }
        }


    }
    private void heartsVisuelUI()
    {
        if (_gameManager == null || _gameManager.vies == null) return;

        for (int i = 0; i < _gameManager.vies.Count; i++)
        {
            _gameManager.vies[i].SetActive(i < _player.currentHealth);
        }
    }

}
