using UnityEngine;

public class Explosion : MonoBehaviour
{
    [SerializeField] private int damage = 2;
    [SerializeField] private float lifetime = 0.2f;  // temps avant que l'explosion disparaisse
    [SerializeField] private LayerMask enemyMask;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {

        if (other.CompareTag("Player"))
        {
            return;
        }
        var enemy = other.GetComponentInParent<Ennemy>();
        if (enemy == null)
        {
            return;
        }

        enemy.TakeDamage(damage);
    }
}
