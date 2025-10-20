using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using UnityEngine;

public class GS_Warp : GromarState
{

    SpriteRenderer[] sprites;
    Queue<Transform> WarpHistory = new Queue<Transform>();
    const int maxHistory = 3;

    int warpTimes = 1; //nb de warps

    bool tpOnPlayer = false;//ecq on tp sur le joueur

    bool tpMiddle = false;//ecq on tp au millieu de la map

    bool tpCornerOnly = false; //ecq on tp dans les corner only

    bool tpSpawn = false;//ecq on tp au spawn


    public GS_Warp() : base(1) { }
    

    public override void SetParam(object[] args)
    {
        warpTimes = (int)args[0];
        tpOnPlayer = (bool)args[1];
        tpMiddle = (bool)args[2];
        tpCornerOnly = (bool)args[3];
        tpSpawn = (bool)args[4];
    }


    public override void OnEnter()
    {
        sprites = gromar.GetComponentsInChildren<SpriteRenderer>();
        gromar.StartCoroutine(DoWarp());
    }

    /// <summary>
    /// Permet de set le nombre de fois que le boss va warp
    /// </summary>
    /// <param name="count">nombre de warps</param>
   
    

    /// <summary>
    /// coroutine qui va faire warp le boss avec un delai de 0.3s entre chaque warp
    /// </summary>
    /// <returns></returns>
    private IEnumerator DoWarp()
    {
        for (int i = 0; i < warpTimes; i++)
        {
            WarpPosition();
            yield return new WaitForSeconds(0.3f);
        }
        yield return new WaitForSeconds(.3f);
        Machine.ExecuteNextState();
    }

    /// <summary>
    /// methode qui fait warp le boss avec un point random et qui ajoute ce point dans une liste de memoire 
    /// </summary>
   public void WarpPosition()
    {
        DisableOrEnableSprites(false);

        if (tpOnPlayer)
        {
            gromar.transform.position = new Vector3 (
                gromar.player.transform.position.x,
                1,
                gromar.transform.position.z);
        }
        else if (tpMiddle)
        {
            gromar.transform.position = gromar.MAPMIDPOINT.position;
        }
        else if (tpSpawn)
        {
            gromar.transform.position = gromar.SPAWNPOINT.position;
        }
        else
        {
            Transform targetPoint = GetRandomPointAvoidPattern();

            gromar.transform.position = targetPoint.position;

            WarpHistory.Enqueue(targetPoint);

            if (WarpHistory.Count > maxHistory)
            {
                WarpHistory.Dequeue();
            }
        }
       

        DisableOrEnableSprites(true);

        

    }

    /// <summary>
    /// methode qui va regarder la liste de memoir pour verifier que le prochain warp point ne replique pas un pattern ABAB ou qui retourne 
    /// un point random simple si la liste de memoire n'est pas assez longue pour verifier le patterne
    /// </summary>
    /// <returns></returns>
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
               if (tpCornerOnly && point.position == gromar.MAPMIDPOINT.position)
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

    /// <summary>
    /// methdoe qui retourne un point random sur la map parmis la liste des points en evitant de prendre un point ou le boss se trouve deja
    /// </summary>
    /// <returns></returns>
    private Transform GetRandomPointSimple()
    {
        var possiblePoints = gromar.mapPoints
            .Where(point =>
            {
                // Never pick where Gromar already is
                if (point.position == gromar.transform.position)
                    return false;

                // If we're in corner-only mode, exclude the midpoint
                if (tpCornerOnly && point.position == gromar.MAPMIDPOINT.position)
                    return false;

                return true;
            })
            .ToList();

        int randomIndex = Random.Range(0, possiblePoints.Count);
        return possiblePoints[randomIndex];
    }


    /// <summary>
    /// methode qui active ou desactive les sprite du boss pour le faire disparaitre visuellement
    /// </summary>
    /// <param name="choice"></param>
    public void DisableOrEnableSprites(bool choice)
    {
        foreach (var sprite in sprites)
        {
            sprite.enabled = choice;
        }
    }
    
}
