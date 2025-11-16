using UnityEngine;
using UnityEngine.UIElements.Experimental;

public class BombConsumable : Consumable
{
    public float throwSpeed = 8f;
    public float upFactor = 0.3f;
    [SerializeField] private GameObject bombPrefab;
    public override void UseConsumable()
    {

        Player player = FindFirstObjectByType<Player>();
        int facing = player.Direction >= 0 ? 1 : -1;

        Vector3 spawnPos = player.WeaponHolder != null ? player.WeaponHolder.position : player.transform.position;

        GameObject bomb = Object.Instantiate(bombPrefab, spawnPos, Quaternion.identity);


        Rigidbody2D rb = bomb.GetComponent<Rigidbody2D>();
        if (rb == null)
        {
            rb = bomb.AddComponent<Rigidbody2D>();
        }

        Vector2 dir = new Vector2(facing, upFactor).normalized;
        rb.linearVelocity = dir * throwSpeed;
    }
}
