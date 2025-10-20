using UnityEngine;

public enum ProjectileType
{
    SmallBullet,
    BigBullet,
    ParabolicMissile,
    HomingMissile,
    Cone
}

public class ProjectileManager : MonoBehaviour
{
    public static ProjectileManager Instance;

    [Header("Prefab de projectiles")]
    public GameObject smallBulletPrefab;
    public GameObject bigBulletPrefab;
    public GameObject parabolicMissilePrefab;
    public GameObject homingMissilePrefab;
    public GameObject Cone;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }


    public Projectile Spawn(ProjectileType type, Vector2 origin, Vector2 target)
    {
        GameObject prefab = type switch
        {
            ProjectileType.SmallBullet => smallBulletPrefab,
            ProjectileType.BigBullet => bigBulletPrefab,
            ProjectileType.ParabolicMissile => parabolicMissilePrefab,
            ProjectileType.HomingMissile => homingMissilePrefab,
            ProjectileType.Cone => Cone,
            _ => null
        };

        if (prefab == null) return null;

        GameObject obj = Instantiate(prefab, origin, Quaternion.identity);
        Projectile projectile = obj.GetComponent<Projectile>();
        projectile.Initialize(origin, target);
        return projectile;
    }
}
