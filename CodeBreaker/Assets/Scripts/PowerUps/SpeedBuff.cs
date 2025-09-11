using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Powerups/SpeedBuff")]
public class SpeedBuff : PowerUpEffect
{
    public float duration = 5f;

    public override void Apply(GameObject target)
    {
        // Seulement intéragir avec le Player
        Player player = target.GetComponent<Player>();
        if (player != null && player.PhysicsInfo != null)
        {
            player.StartCoroutine(ApplyTempSpeedBuff(player));
        }
    }

    private IEnumerator ApplyTempSpeedBuff(Player player)
    {
        var info = player.PhysicsInfo;

        // Sauvegarder les valeurs initiaux
        float initialSpeed = info.MaxSpeed;
        float initialAcceleration = info.Acceleration;

        // Implémenter le buff
        info.MaxSpeed = initialSpeed * 1.5f;
        info.Acceleration = initialAcceleration * 1.5f;

        yield return new WaitForSeconds(duration);

        // vitesse initial après "duration"
        info.MaxSpeed = initialSpeed;
        info.Acceleration = initialAcceleration;
    }
}
