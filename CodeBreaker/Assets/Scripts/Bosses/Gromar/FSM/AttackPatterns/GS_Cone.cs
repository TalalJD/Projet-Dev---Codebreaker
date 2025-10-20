using Codice.CM.Common.Serialization;
using PlasticGui.WorkspaceWindow.BrowseRepository;
using System.Collections;
using UnityEngine;

public class GS_Cone : GromarState
{
    

    int numberOfCones = 1;
    float delay = .3f;
    
    public GS_Cone() : base(2) {}

    public override void OnEnter()
    {

        gromar.StartCoroutine(shootCone());
        
    }

    public override void SetParam(object[] args)
    {
        numberOfCones =  (int)args[0];
        delay = (float)args[1];
    }

    public override void OnExit()
    {
    }

    private IEnumerator shootCone()
    {
        for(int i = 0; i < numberOfCones; i++)
        {
            Vector2 origin = gromar.ShootingPoint.position;
            Vector2 target = gromar.player.transform.position + Vector3.up * 1.5f;
            var cone = ProjectileManager.Instance.Spawn(ProjectileType.Cone, origin, target);
            if (cone != null) { cone.speed = 10f; }
            yield return new WaitForSeconds(delay);
        }
        

        yield return new WaitForSeconds(.3f);
        Machine.ExecuteNextState();
    }
}
