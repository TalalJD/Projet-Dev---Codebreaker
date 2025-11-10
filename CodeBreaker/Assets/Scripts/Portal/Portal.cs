using System.Collections.Generic;
using UnityEngine;

public class Portal : MonoBehaviour
{
    // Les GameObjects qui rentre 
    private HashSet<GameObject> portalObjects = new HashSet<GameObject>();

    // La destination 
    [SerializeField] private Transform destination;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            // ne pas rester dans une loop
            if (portalObjects.Contains(collision.gameObject))
            {
                return;
            }

            // Pour la destination
            if (destination.TryGetComponent(out Portal destinationPortal))
            {
                destinationPortal.portalObjects.Add(collision.gameObject);
            }

            // La teleportation
            // Calcul la position pour exit sur le sol
            Vector3 relativePos = collision.transform.position - transform.position;
            collision.transform.position = destination.position + destination.TransformDirection(transform.InverseTransformDirection(relativePos));

           
            collision.transform.rotation = destination.rotation;

        } 
        
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        portalObjects.Remove(collision.gameObject); 
    }

}
