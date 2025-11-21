using UnityEngine;

public class GromarAnimationEventHandler : MonoBehaviour
{
    public Gromar gromar;

    public void CallMissilAttack() { gromar.CallMissilAttack(); }
    public void CallLobe() { gromar.CallLobe(); }
    public void CallWarp() { gromar.CallWarp(); }
    public void CallCone() { gromar.CallCone(); }
    public void CallLaser() { gromar.CallLaser(); }
    public void CallExplosion() { gromar.CallExplosion(); }

    // Event à mettre à la FIN de chaque anim d’attaque
    public void NextPatternStep()
    {
        if (gromar == null || gromar.StateMachine == null)
            return;

        var current = gromar.StateMachine.CurrentState;
        if (current is GromarState gs)
        {
            gs.NotifyAnimationFinished();
        }
    }
}
