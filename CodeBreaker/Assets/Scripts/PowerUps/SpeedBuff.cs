using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Powerups/SpeedBuff")]
public class SpeedBuff : PowerUpEffect
{
    public float speedBonus = 3f;
    public float accelerationBonus = 10f;
    public float duration = 5f;

    public override void Apply(GameObject target)
    {
        // Intéragit seulement avec le player 
        Player player = target.GetComponent<Player>();
        if (player != null && player.PhysicsInfo != null)
        {
            player.StartCoroutine(ApplyTempSpeedBuff(player));
        }
    }

    private IEnumerator ApplyTempSpeedBuff(Player player)
    {   
        // Acceleration temporaire
        var info = player.PhysicsInfo;

        info.MaxSpeed -= speedBonus;
        info.Acceleration -= accelerationBonus;

        yield return new WaitForSeconds(duration);

        // retourn à la vitesse normale
        info.MaxSpeed += speedBonus;
        info.Acceleration += accelerationBonus;
    }
}
