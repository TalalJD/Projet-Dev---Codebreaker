using System.Collections;
using UnityEngine;

public class SpeedBuff : MonoBehaviour
{
    public float duration = 5f;
    private Player _player;
    private PhysicsInfo _physicsInfo;
    [SerializeField] private BoxCollider2D _boxCollider;
    [SerializeField] private SpriteRenderer _spriteRenderer;

    void Start()
    {

        _player = FindObjectOfType<Player>();
        if (_player != null)
        {
            _physicsInfo = _player.PhysicsInfo;
            _physicsInfo.MaxSpeed = 9.7f;
            _physicsInfo.Acceleration = 20f;
}
    }


    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            
            if (_player != null && _physicsInfo != null)
            {
                Debug.Log($"[Before Buff] MaxSpeed: {_physicsInfo.MaxSpeed}, Acceleration: {_physicsInfo.Acceleration}");
                StartCoroutine(ApplyTempSpeedBuff());
                _boxCollider.enabled = false;
                if (_spriteRenderer != null)
                    _spriteRenderer.enabled = false;
            }
            
            
        }
    }

    /// <summary>
    /// methode qui applique le power up au joueur en modifiant sa vitesse max et son acceleration
    /// </summary>
    
    private IEnumerator ApplyTempSpeedBuff()
    {
        // Savegarder les valeurs initiales 
        float initialSpeed = _physicsInfo.MaxSpeed;
        float initialAcceleration = _physicsInfo.Acceleration;

        // Appliquer le power up
        _physicsInfo.MaxSpeed = initialSpeed * 1.5f;
        _physicsInfo.Acceleration = initialAcceleration * 1.5f;
        Debug.Log($"[Buff Applied] MaxSpeed: {_physicsInfo.MaxSpeed}, Acceleration: {_physicsInfo.Acceleration}");

        // attendre la fin du power up
        yield return new WaitForSeconds(duration);

        // Remttre les valeures initialles
        _physicsInfo.MaxSpeed = initialSpeed;
        _physicsInfo.Acceleration = initialAcceleration;
        Debug.Log($"[Buff Ended] MaxSpeed: {_physicsInfo.MaxSpeed}, Acceleration: {_physicsInfo.Acceleration}");
        DestroyImmediate(gameObject);
    }
}
