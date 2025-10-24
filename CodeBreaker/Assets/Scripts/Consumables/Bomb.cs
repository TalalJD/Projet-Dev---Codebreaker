using UnityEngine;

public class Bomb : MonoBehaviour
{
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private float delay = 0.3f; // delai avant l'explosion

    private bool armed = false;
    private bool exploded = false;

    private void Start()
    {
        //Appele la methode Arm apres 0.3 secondes pour eviter contact avec le joueur
        Invoke(nameof(Arm), delay);
    }

    private void Arm()
    {
        armed = true;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!armed || exploded)
        {
            return;
        }

        if (other.CompareTag("Player"))
        {
            return;
        }

        exploded = true;

        if (explosionPrefab != null)
        {
            Instantiate(explosionPrefab, transform.position, Quaternion.identity);
        }

        Destroy(gameObject);
    }
}
