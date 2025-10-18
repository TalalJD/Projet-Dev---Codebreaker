using PlasticGui.WorkspaceWindow.BrowseRepository;
using UnityEngine;

public class Gs_Cone : GromarState
{
    public override void OnEnter()
    {

        shootCone();
    }

    public override void OnExit()
    {
    }

    private void shootCone()
    {
        Vector2 origin = gromar.ShootingPoint.position;
        Vector2 target = gromar.player.transform.position + Vector3.up * 0.5f;
        var cone = ProjectileManager.Instance.Spawn(
                    ProjectileType.Cone,
                    origin,
                    target
                );
        if (cone != null) { cone.speed = 10f; }
    }
}
