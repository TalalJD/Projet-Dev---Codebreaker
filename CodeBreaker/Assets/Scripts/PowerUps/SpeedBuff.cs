using System.Collections;
using UnityEngine;

[CreateAssetMenu(menuName = "Powerups/SpeedBuff")]
public class SpeedBuff : PowerUpEffect
{

    public float duration = 5f;
    public float speedStart;
    public float accelerationStart;
    public bool reset = false;
    void Start()
    {
        // On peut trouver le Player au lancement
        Player player = FindObjectOfType<Player>();
        if (player != null && player.PhysicsInfo != null)
        {
            speedStart = player.PhysicsInfo.MaxSpeed;
            accelerationStart = player.PhysicsInfo.Acceleration;
            reset = true;
        }
    }
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
