using PlasticGui.WorkspaceWindow.BrowseRepository;
using System.Collections;
using UnityEngine;

public class GS_Cone : GromarState
{
    public override void OnEnter()
    {

        gromar.StartCoroutine(shootCone());
        
    }

    public override void OnExit()
    {
    }

    private IEnumerator shootCone()
    {
        Vector2 origin = gromar.ShootingPoint.position;
        Vector2 target = gromar.player.transform.position + Vector3.up * 1.5f;
        var cone = ProjectileManager.Instance.Spawn(ProjectileType.Cone, origin, target);
        if (cone != null) { cone.speed = 10f; }

        yield return new WaitForSeconds(1.5f);
        Machine.ExecuteNextState();
    }
}
