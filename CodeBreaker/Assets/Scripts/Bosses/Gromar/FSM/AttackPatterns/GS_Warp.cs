using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GS_Warp : GromarState
{
    SpriteRenderer[] sprites;
    Queue<Transform> WarpHistory = new Queue<Transform>();
    int maxHistory = 3;
    public override void OnEnter()
    {
        sprites = gromar.GetComponentsInChildren<SpriteRenderer>();
        WarpPosition();
    }
   public void WarpPosition()
    {
        DisableOrEnableSprites(false);

        Transform targetPoint = GetRandomPointAvoidPattern();

        gromar.transform.position = targetPoint.position;

        WarpHistory.Enqueue(targetPoint);

        if (WarpHistory.Count > maxHistory)
        {
            WarpHistory.Dequeue();
        }

        DisableOrEnableSprites(true);

        Machine.Set<GS_Idle>(); 

    }

    public Transform GetRandomPointAvoidPattern()
    {
        // Si l'historique est pas assez long on utilise la methode simplifiee
        if (WarpHistory.Count < 3)
            return GetRandomPointSimple();

        // recuperer les 2 derniers points pour verifier le patterne
        Transform last = WarpHistory.Last(); //point le plus recent
        Transform secondLast = WarpHistory.ElementAt(WarpHistory.Count - 2); // deuxieme point

        // faire la liste de points potentiels
        var possiblePoints = gromar.mapPoints
            .Where(point =>
            {
                // ignorer le point ou le boss est deja present
                if (point.position == gromar.transform.position)
                    return false;

                // Verifier pour un pattern A B A B
                // Si les 2 derniers points sont A & B, ne pas choisir A encore
                if (WarpHistory.Count >= 3)
                {
                    Transform thirdLast = WarpHistory.First(); //point le plus ancien
                    if (thirdLast == last && point == secondLast) //Si il ya patterne ABAB de potentiel on ignore le point B
                        return false; 
                }

                return true;
            })
            .ToList();

       //choisir un point random dans la liste
        int randomIndex = Random.Range(0, possiblePoints.Count);
        return possiblePoints[randomIndex];
    }

    private Transform GetRandomPointSimple()
    {
        var possiblePoints = gromar.mapPoints
            .Where(p => p.position != gromar.transform.position)
            .ToList();

        int randomIndex = Random.Range(0, possiblePoints.Count);
        return possiblePoints[randomIndex];
    }


    public void DisableOrEnableSprites(bool choice)
    {
        foreach (var sprite in sprites)
        {
            sprite.enabled = choice;
        }
    }
    
}
