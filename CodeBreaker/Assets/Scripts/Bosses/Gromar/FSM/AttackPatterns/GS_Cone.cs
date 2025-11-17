using System.Collections;
using UnityEngine;

public class GS_Cone : GromarState
{
    private int numberOfCones = 1;
    private float delay = 0.3f;
    private float nextStateDelay = 0.3f;
    private float speed = 10f;


    public GS_Cone() : base(2) { }

    public override void SetParam(object args)
    {
        // reset to defaults each time
        numberOfCones = 1;
        delay = 0.3f;
        nextStateDelay = 0.3f;
        speed = 10f;

        if (args is ConeArgs a)
        {
            numberOfCones = Mathf.Max(1, a.Count);
            delay = Mathf.Max(0f, a.Delay);
            nextStateDelay = Mathf.Max(0f, a.nextStateDelay);
            speed = Mathf.Max(0f, a.Speed);
        }
    }

    public override void OnEnter()
    {
        gromar.animator.SetTrigger("ConeAttack");
        //gromar.StartCoroutine(ShootCone());
    }

    public override void OnExit()
    {
        base.OnExit();
    }
    public IEnumerator ShootCone()
    {
        for (int i = 0; i < numberOfCones; i++)
        {
            Vector2 origin = gromar.ConeSP.position;
            Vector2 target = gromar.player.transform.position + Vector3.up * 1.5f;

            var cone = ProjectileManager.Instance.Spawn(ProjectileType.Cone, origin, target);
            if (cone != null) cone.speed = 10f;

            if (delay > 0f) yield return new WaitForSeconds(delay);
        }

        yield return new WaitForSeconds(nextStateDelay);
        Machine.ExecuteNextState();
    }
}
