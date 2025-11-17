using UnityEngine;

public class GromarAnimationEventHandler : MonoBehaviour
{
    public Gromar gromar;

    public void CallMissilAttack()
    {
        gromar.CallMissilAttack();
    }

    public void CallLobe()
    {
        gromar.CallLobe();

    }

    public void CallWarp()
    {
        gromar.CallWarp();
    }

    public void CallCone()
    {
        gromar.CallCone();
    }

    public void CallLaser()
    {
        gromar.CallLaser();
    }

    public void CallExplosion()
    {
        gromar.CallExplosion();
    }
}
