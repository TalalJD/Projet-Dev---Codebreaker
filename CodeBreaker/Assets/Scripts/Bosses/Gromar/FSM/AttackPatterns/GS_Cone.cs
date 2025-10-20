using PlasticGui.WorkspaceWindow.BrowseRepository;
using System.Collections;
using UnityEngine;

public class GS_Cone : GromarState
{
    public override int StateNumber => 2;

    int numberOfCones = 1;
    float delay = .3f;
    
    public GS_Cone(int count = 1, float delay = .3f) { numberOfCones = count; this.delay = delay; }

    public override void OnEnter()
    {

        gromar.StartCoroutine(shootCone());
        
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
